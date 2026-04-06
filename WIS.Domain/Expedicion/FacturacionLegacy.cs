using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Extension;

namespace WIS.Domain.Expedicion
{
    public class FacturacionLegacy
    {
        public static void FacturarPedido(IUnitOfWork uow, List<ContenedorFacturar> contenedores, string predio, int userId, out List<string> keysCamionesFacturados, out List<string> errores)
        {
            short? nroPuerta = null;
            errores = new List<string>();
            keysCamionesFacturados = new List<string>();

            var cam = new CamionLogic();

            var facturaPuertaAuto = uow.ParametroRepository.GetParameter(ParamManager.FACTURA_PUERTA_AUTO) ?? "N";

            if (!short.TryParse(facturaPuertaAuto, out short parsedValue) || !uow.PuertaEmbarqueRepository.AnyPuertaEmbarque(parsedValue))
            {
                nroPuerta = uow.PuertaEmbarqueRepository.GetFirstPuertaByPredio(predio);

                if (nroPuerta == null)
                    throw new ValidationFailedException("EXP330_form1_Error_SinPuertaEmbarque");
            }
            else
            {
                var puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(parsedValue);
                if (puerta.NumPredio != predio)
                    throw new ValidationFailedException("EXP330_msg_Error_PuertaConfiguradaDistintoPredio", [$"{puerta.Id}-{puerta.Descripcion}", predio]);

                nroPuerta = parsedValue;
            }

            var cdCamion = -1;
            var empresaActual = -1;
            var listCamiones = new List<int>();

            contenedores = contenedores.OrderBy(x => x.CodigoEmpresa).ToList();

            foreach (var contenedor in contenedores)
            {
                var dsCamion = string.Format("Facturación de pedido {0}", contenedor.NumeroPedido);
                if (empresaActual == -1)
                {
                    cdCamion = CrearCamionFactura(uow, contenedor.CodigoEmpresa, nroPuerta, predio, dsCamion);
                    listCamiones.Add(cdCamion);

                    empresaActual = contenedor.CodigoEmpresa;
                }
                else if (empresaActual != contenedor.CodigoEmpresa)
                {
                    cdCamion = CrearCamionFactura(uow, contenedor.CodigoEmpresa, nroPuerta, predio, dsCamion);
                    listCamiones.Add(cdCamion);
                }

                contenedor.CodigoCamion = cdCamion;

                cam.AgregarCargaCamion(uow, contenedor);

                uow.PreparacionRepository.NivelarCargasDetallePicking(contenedor);

                uow.SaveChanges();
            }

            foreach (int nucam in listCamiones)
            {
                var msgError = string.Empty;
                var camion = uow.CamionRepository.GetCamion(nucam);

                ValidacionesFacturacion(uow, nucam, out msgError);

                if (camion.IsControlContenedoresHabilitado && uow.PreparacionRepository.AnyContenedorSinControl(camion, out int cantCont))
                    msgError = $"Hay {cantCont} contenedor/es sin controlar o finalizar su control.";
                else if (uow.PreparacionRepository.AnyContenedorSinFinalizarControl(camion, out cantCont))
                    msgError = $"Hay {cantCont} contenedor/es sin terminar de controlar.";

                if (string.IsNullOrEmpty(msgError))
                {
                    bool contenedoresSinFacturar = uow.PreparacionRepository.AnyContenedorNoFacturado(camion);
                    if (contenedoresSinFacturar == true)
                    {
                        var contL = new ContenedorLogic();
                        bool tieneFacturacion = contL.FacturarContenedores(uow, camion, userId);

                        if (tieneFacturacion)
                            camion.NumeroInterfazEjecucionFactura = -1;
                        else
                            camion.NumeroInterfazEjecucionFactura = 0;

                        camion.NumeroTransaccion = uow.GetTransactionNumber();
                        uow.CamionRepository.UpdateCamion(camion);

                        if (camion.NumeroInterfazEjecucionFactura == -1)
                            keysCamionesFacturados.Add(camion.Id.ToString());
                    }
                }
                else
                {
                    if (errores.Count == 0)
                    {
                        errores.Add(msgError);
                    }
                    else
                    {
                        bool existeError = false;
                        foreach (var error in errores)
                        {
                            if (msgError.Equals(error))
                            {
                                existeError = true;
                            }
                        }
                        if (existeError == false)
                        {
                            errores.Add(msgError);
                        }
                    }

                }
            }
        }

        public static void ValidacionesFacturacion(IUnitOfWork uow, int camion, out string MSG_ERROR)
        {
            MSG_ERROR = "";
            string controlaPreparacion = null;
            controlaPreparacion = uow.ParametroRepository.GetParameter("CTRL_FACT_PREPARACION");

            List<DetPickingCamionWEXP040> lst = uow.PreparacionRepository.GetsDetPreparacionCamion(camion);
            if (lst.GroupBy(s => s.NumeroCarga).Count() == 0)
                throw new Exception("EXP040_Sec0_Error_FacturarCamionVacio");

            var lstControl = lst.Where(s => s.NumeroContenedor != null)
                              .GroupBy(s => new { s.NumeroPreparacion, s.NumeroContenedor, s.Agrupacion })
                               .Select(dp => new { dp.Key.NumeroPreparacion, dp.Key.Agrupacion, NumeroContenedor = (dp.Key.NumeroContenedor ?? -1) }).ToList();

            bool contComp = false;
            List<T_CONT_ERROR> lista_Cont_ERROR = new List<T_CONT_ERROR>();

            List<int[]> contenedoresConProblemas = uow.CamionRepository.GetContenedoresConProblemas(camion);

            foreach (var cont in lstControl)
            {
                if (contenedoresConProblemas.Any(w => w[0] == cont.NumeroPreparacion && w[1] == cont.NumeroContenedor))
                {
                    T_CONT_ERROR element = new T_CONT_ERROR();
                    element.NU_CONTENEDOR = cont.NumeroContenedor;
                    element.NU_PREPARACION = cont.NumeroPreparacion;
                    lista_Cont_ERROR.Add(element);
                    contComp = true;
                }
            }

            string msg, prep, conte, msgAux, EsLieberadoCompletamente, esTodoPickeado, esTodoAsignadoCamion, esTodoEmpaquetado, esTodoTienePrecinto, Coma, NoAdmiteFacturacionParcial;
            string message = string.Empty;
            Coma = ",";

            /*if (!string.IsNullOrEmpty(cliente) && cliente == "BH")
            {
                msg = "Os contêineres a seguir apresentam problemas: ";
                prep = "Preparação:";
                conte = "Contêiner:";
                msgAux = "Não é possível verificar o caminhão";

                NoAdmiteFacturacionParcial = "Los pedidos no admiten modalidad de facturación parcial." + message;
                EsLieberadoCompletamente = "<br/>Têm mercadorias sem liberação são em  ordens: ";
                esTodoPickeado = "<br/>Têm mercadorias sem despreparada são em ordens: ";
                esTodoAsignadoCamion = "<br/>Não possuem todas as cobranças associadas ao caminhão em ordens: ";
                esTodoEmpaquetado = "<br/>Têm mercadorias sem empacotar são em ordens: ";
                esTodoTienePrecinto = "<br/>Têm mercadorias sem selo são em ordens: ";

            }
            else
            {*/
            msg = "Los siguientes contenedores tienen problemas ";
            prep = "Preparación";
            conte = "Contenedor";
            msgAux = "No es posible facturar el camión";

            NoAdmiteFacturacionParcial = "\nLos pedidos no admiten modalidad de facturación parcial.";
            EsLieberadoCompletamente = "\nTiene mercadería sin liberar en el/los pedido/s ";
            esTodoPickeado = "\nTiene mercadería sin preparar en el/los pedido/s ";
            esTodoAsignadoCamion = "\nNo tiene todas las cargas asociadas al camion de los pedidos ";
            esTodoEmpaquetado = "\nTiene mercadería sin empaquetar en los pedidos ";
            esTodoTienePrecinto = "\nTiene mercadería sin precinto en los pedidos ";

            //}
            // string NU_PEDIDO = contendedor.NU_PEDIDO;
            //   string CD_CLIENTE = contendedor.CD_CLIENTE;
            //  int CD_EMPRESA = contendedor.CD_EMPRESA;
            if (contComp)
            {
                foreach (var error in lista_Cont_ERROR)
                {
                    var cont = uow.ContenedorRepository.GetContenedor(error.NU_PREPARACION, error.NU_CONTENEDOR);
                    msg = msg + "\n\r" + " " + prep + " " + error.NU_PREPARACION + " " + conte + " " + cont.TipoContenedor + " - " + cont.IdExterno;
                }
                MSG_ERROR = msg = msg + "\n\r" + " " + msgAux;
                return;
            }

            var lstPedido = lst.GroupBy(s => new { s.CodigoEmpresa, s.CodigoCliente, s.NumeroPedido, s.EmpaquetaContenedor, s.PermiteFactSinPrecinto }).ToList();

            string validarLiberacionTotal = uow.ParametroRepository.GetParameter("WEXP040_VALIDAR_LIB_COMPLETA") + "";
            if (string.IsNullOrEmpty(validarLiberacionTotal))
            {
                validarLiberacionTotal = "S";
            }

            string validarTodoAsignadoCamion = uow.ParametroRepository.GetParameter("WEXP040_VALIDAR_TOTAL_ASIGNADO") + "";
            if (string.IsNullOrEmpty(validarTodoAsignadoCamion))
            {
                validarTodoAsignadoCamion = "S";
            }
            bool AdmiteFacturacionParcial = true;
            bool LieberadoCompletamente = true;
            bool TodoPickeado = true;
            bool TodoAsignadoCamion = true;
            bool TodoEmpaquetado = true;
            bool TodoTienePrecinto = true;

            foreach (var linea in lstPedido)
            {
                Pedido ped = uow.PedidoRepository.GetPedido(linea.Key.CodigoEmpresa, linea.Key.CodigoCliente, linea.Key.NumeroPedido);

                if (ped.ConfiguracionExpedicion.IsFacturacionRequerida)
                {
                    string NU_PEDIDO = linea.Key.NumeroPedido;
                    string CD_CLIENTE = linea.Key.CodigoCliente;
                    int CD_EMPRESA = linea.Key.CodigoEmpresa;
                    string FL_EMPAQUETA_CONTENEDOR = linea.Key.EmpaquetaContenedor;
                    string FL_PERMITE_FACT_SIN_PRECINTO = linea.Key.PermiteFactSinPrecinto;

                    if (controlaPreparacion != null && controlaPreparacion == "S")
                    {
                        if (!ped.ConfiguracionExpedicion.PermiteFacturacionParcial && !uow.PedidoRepository.LiberadoCompletamente(NU_PEDIDO, CD_CLIENTE, CD_EMPRESA) && validarLiberacionTotal == "S")
                        {
                            EsLieberadoCompletamente += NU_PEDIDO + Coma;
                            LieberadoCompletamente = false;
                        }

                        if (!ped.ConfiguracionExpedicion.PermiteFacturacionParcial && !uow.PedidoRepository.TodoPickeado(NU_PEDIDO, CD_CLIENTE, CD_EMPRESA))
                        {
                            esTodoPickeado += NU_PEDIDO + Coma;
                            TodoPickeado = false;
                        }

                        if (!uow.PedidoRepository.TodoAsignadoCamion(NU_PEDIDO, CD_CLIENTE, CD_EMPRESA, camion) && validarTodoAsignadoCamion == "S")
                        {
                            esTodoAsignadoCamion += NU_PEDIDO + Coma;
                            TodoAsignadoCamion = false;
                        }

                        if (FL_EMPAQUETA_CONTENEDOR == "S" && !uow.PedidoRepository.TodoEmpaquetado(NU_PEDIDO, CD_CLIENTE, CD_EMPRESA))
                        {
                            esTodoEmpaquetado += NU_PEDIDO + Coma;
                            TodoEmpaquetado = false;
                        }
                    }

                    if (FL_PERMITE_FACT_SIN_PRECINTO != "S" && !uow.PedidoRepository.TodoTienePrecinto(uow, NU_PEDIDO, CD_CLIENTE, CD_EMPRESA, camion))
                    {
                        esTodoTienePrecinto = esTodoTienePrecinto + "\n\r" + NU_PEDIDO;
                        TodoTienePrecinto = false;
                    }
                }
            }
            if (!LieberadoCompletamente)
            {
                message += EsLieberadoCompletamente;
                AdmiteFacturacionParcial = false;
            }
            if (!TodoPickeado)
            {
                message += esTodoPickeado;
                AdmiteFacturacionParcial = false;
            }
            if (!TodoAsignadoCamion)
            {
                message += esTodoAsignadoCamion;
            }
            if (!TodoEmpaquetado)
            {
                message += esTodoEmpaquetado;
            }
            if (!TodoTienePrecinto)
            {
                message += esTodoTienePrecinto;
            }

            if (!string.IsNullOrEmpty(message))
            {
                if (!AdmiteFacturacionParcial)
                    message = NoAdmiteFacturacionParcial + message;

                MSG_ERROR = message;
            }
        }

        public static int CrearCamionFactura(IUnitOfWork uow, int empresa, short? NuPuerta, string predio, string dsCamion)
        {
            Camion camion = new Camion();

            camion.Empresa = empresa;
            camion.RespetaOrdenCarga = false;
            camion.Puerta = NuPuerta;
            camion.FechaCreacion = DateTime.Now;
            camion.Estado = CamionEstado.AguardandoCarga;
            camion.Descripcion = dsCamion.Truncate(50);
            camion.Predio = predio;
            camion.IsRuteoHabilitado = true;

            var controlarContenedores = (uow.ParametroRepository.GetParameter(ParamManager.CREAR_CAMION_AUTO_CONTROL_CONT, new Dictionary<string, string>
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}"
            }) ?? "N") == "S";

            camion.IsControlContenedoresHabilitado = controlarContenedores;

            string FL_CIERRE_PARCIAL = uow.ParametroRepository.GetParameter("WEXP040_CAMION_CIERRE_PARCIAL", new Dictionary<string, string>
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}"
            });

            if (!string.IsNullOrEmpty(FL_CIERRE_PARCIAL))
                camion.IsCierreParcialHabilitado = FL_CIERRE_PARCIAL == "S";

            camion.NumeroTransaccion = uow.GetTransactionNumber();
            camion.Id = uow.CamionRepository.GetNextCdCamion();
            camion.Transportista = 1;
            camion.TipoArmadoEgreso = TipoArmadoEgreso.Retira;

            uow.CamionRepository.AddCamion(camion);

            return camion.Id;
        }

        private class T_CONT_ERROR
        {
            public int NU_PREPARACION;
            public int NU_CONTENEDOR;
        }
    }

}
