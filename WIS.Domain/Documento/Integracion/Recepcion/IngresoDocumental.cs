using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Serializables.Entrada;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Security;

namespace WIS.Domain.Documento.Integracion.Recepcion
{
    public class IngresoDocumental
    {
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly IIdentityService _identity;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IngresoDocumental(
            IFactoryService factoryService,
            IParameterService parameterService,
            IIdentityService identity)
        {
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._identity = identity;
        }

        public virtual void CerrarAgendaDocumental(IUnitOfWork uow, int numeroAgenda, int usuario, string aplicacion)
        {
            try
            {
                var nuTransaccion = uow.GetTransactionNumber();
                var documento = uow.DocumentoRepository.GetIngresoPorAgenda(numeroAgenda);

                if (documento != null)
                {
                    if (documento.IsHabilitadoParaBalanceo(uow))
                    {
                        var agenda = uow.AgendaRepository.GetAgenda(numeroAgenda);

                        if (agenda == null)
                            throw new ValidationFailedException("General_Sec0_Error_AgendaNoExiste");

                        if (this.BalancearDocumentoIngreso(uow, documento, agenda.Id, agenda.Detalles, usuario, aplicacion, out string errorMsg))
                            uow.DocumentoRepository.UpdateIngreso(documento, nuTransaccion);
                        else
                            throw new ValidationFailedException(errorMsg);
                    }
                    else
                        throw new ValidationFailedException("General_Sec0_Error_DocumentoNoBalanceable");
                }
                else
                    throw new ValidationFailedException("General_Sec0_Error_AgendaSinDocumentoIngreso", new string[] { numeroAgenda.ToString() });
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error(ex, ex.Message);
                throw new ValidationFailedException(ex.Message, ex.StrArguments);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception("Manejo Documental: " + ex.Message, ex);
            }
        }

        public virtual IDocumentoIngreso CrearCabezalIngreso(int empresa, string tipoDocumento, IUnitOfWork uow, int usuario, string numeroDocumento = null)
        {
            var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(tipoDocumento);
            var tpDoc = uow.DocumentoTipoRepository.GetTipoDocumento(tipoDocumento);
            var documento = this._factoryService.CreateDocumentoIngreso(tipoDocumento);

            if (!string.IsNullOrEmpty(numeroDocumento))
            {
                IDocumentoIngreso documentoExistente = uow.DocumentoRepository.GetIngreso(numeroDocumento, tipoDocumento);
                if (documentoExistente != null)
                {
                    throw new Exception("General_Sec0_Error_Error48");
                }
            }

            if (documento != null)
            {
                documento.Tipo = tipoDocumento;

                if (uow.DocumentoTipoRepository.DocumentoNumeracionAutogenerada(tipoDocumento))
                    documento.Numero = documento.GetNumeroDocumento(uow);
                else if (string.IsNullOrEmpty(numeroDocumento))
                    throw new ValidationFailedException("General_Sec0_Error_NumeroDocumentoNoEspecificado");
                else
                    documento.Numero = numeroDocumento;

                documento.Descripcion = tpDoc.DescripcionTipoDocumento;
                documento.Usuario = usuario;
                documento.DocumentoAduana = new DocumentoAduana();
                documento.ValorArbitraje = 1;
                documento.GeneraAgenda = false;
                documento.Situacion = 15;
                documento.FechaAlta = DateTime.Now;
                documento.Empresa = empresa;
                documento.Estado = estadoInicial;
                documento.IdManual = "N";
                documento.AgendarAutomaticamente = tpDoc.AutoAgendable;
            }

            return documento;
        }

        public virtual List<DocumentoLinea> GenerarLineasIngreso(IUnitOfWork uow, List<LineaIngresoDocumental> lineasIngreso, IDocumentoIngreso documentoIngreso)
        {
            List<DocumentoLinea> nuevasLineas = new List<DocumentoLinea>();
            if (documentoIngreso != null)
            {
                foreach (var lineaIngreso in lineasIngreso)
                {
                    Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(lineaIngreso.Empresa, lineaIngreso.Producto);
                    var nroIdentificadorFinal = producto.ParseIdentificador(lineaIngreso.Identificador);
                    var linea = documentoIngreso.Lineas.FirstOrDefault(x => x.Producto == lineaIngreso.Producto && x.Empresa == lineaIngreso.Empresa && x.Identificador == nroIdentificadorFinal && x.Faixa == lineaIngreso.Faixa);

                    if (linea != null)
                    {
                        linea.CantidadIngresada = linea.CantidadIngresada + lineaIngreso.CantidadAfectada;

                        if (!string.IsNullOrEmpty(lineaIngreso.Semiacabado) && lineaIngreso.Semiacabado == "S")
                        {
                            if (linea.CantidadReservada == null)
                                linea.CantidadReservada = 0;
                            linea.CantidadReservada = linea.CantidadReservada + lineaIngreso.CantidadAfectada;
                        }
                    }
                    else
                    {
                        linea = new DocumentoLinea
                        {
                            Producto = lineaIngreso.Producto,
                            Empresa = lineaIngreso.Empresa,
                            Faixa = lineaIngreso.Faixa ?? 1,
                            Identificador = nroIdentificadorFinal,
                            CantidadIngresada = lineaIngreso.CantidadAfectada,
                            ValorMercaderia = null,
                            FechaAlta = DateTime.Now,
                            Situacion = 15,
                            CantidadDesafectada = null,
                            CantidadDescargada = null,
                            Disponible = false,
                            FechaModificacion = null,
                            FechaDisponible = null,
                            CIF = null,
                            DescripcionProducto = null
                        };

                        if (!string.IsNullOrEmpty(lineaIngreso.Semiacabado) && lineaIngreso.Semiacabado == "S")
                        {
                            if (linea.CantidadReservada == null)
                                linea.CantidadReservada = 0;

                            linea.CantidadReservada = linea.CantidadReservada + lineaIngreso.CantidadAfectada;
                        }

                        nuevasLineas.Add(linea);
                    }
                }
            }

            return nuevasLineas;
        }

        public virtual void GenerarLineasIngresoBB(List<LineaIngresoDocumental> lineasIngreso, IDocumentoIngreso documentoIngreso, IUnitOfWork uow)
        {
            if (documentoIngreso != null)
            {
                List<DocumentoLinea> lista = new List<DocumentoLinea>();
                foreach (var lineaIngreso in lineasIngreso)
                {
                    Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(lineaIngreso.Empresa, lineaIngreso.Producto);
                    var nroIdentificadorFinal = producto.ParseIdentificador(lineaIngreso.Identificador);
                    var linea = lista.FirstOrDefault(x => x.Producto == lineaIngreso.Producto && x.Empresa == lineaIngreso.Empresa && x.Identificador == nroIdentificadorFinal && x.Faixa == lineaIngreso.Faixa);

                    if (linea == null)
                    {
                        linea = new DocumentoLinea
                        {
                            Producto = lineaIngreso.Producto,
                            Empresa = lineaIngreso.Empresa,
                            Faixa = lineaIngreso.Faixa ?? 1,
                            Identificador = nroIdentificadorFinal,
                            CantidadIngresada = lineaIngreso.CantidadAfectada,
                            ValorMercaderia = null,
                            FechaAlta = DateTime.Now,
                            Situacion = 15,
                            CantidadDesafectada = null,
                            CantidadDescargada = null,
                            Disponible = false,
                            FechaModificacion = null,
                            FechaDisponible = null,
                            CIF = null,
                            DescripcionProducto = null
                        };

                        if (!string.IsNullOrEmpty(lineaIngreso.Semiacabado) && lineaIngreso.Semiacabado == "S")
                        {
                            if (linea.CantidadReservada == null)
                            {
                                linea.CantidadReservada = 0;
                            }
                            linea.CantidadReservada = linea.CantidadReservada + lineaIngreso.CantidadAfectada;
                        }

                        lista.Add(linea);
                    }
                    else
                    {
                        lista.Remove(linea);
                        linea.CantidadIngresada = linea.CantidadIngresada + lineaIngreso.CantidadAfectada;

                        if (!string.IsNullOrEmpty(lineaIngreso.Semiacabado) && lineaIngreso.Semiacabado == "S")
                        {
                            if (linea.CantidadReservada == null)
                            {
                                linea.CantidadReservada = 0;
                            }
                            linea.CantidadReservada = linea.CantidadReservada + lineaIngreso.CantidadAfectada;
                        }

                        lista.Add(linea);
                    }
                }

                foreach (var linea in lista)
                {
                    documentoIngreso.Lineas.Add(linea);
                }
            }
        }

        public virtual bool BalancearDocumentoIngreso(IUnitOfWork uow, IDocumentoIngreso documentoIngreso, int agenda, List<AgendaDetalle> detalles, int userId, string aplicacion, out string errorMsg)
        {
            var accion = AccionDocumento.Finalizar;
            bool result = false;
            errorMsg = "";

            try
            {
                //Separar datos necesarios para trabajar
                var productosDeclarados = documentoIngreso.Lineas.Select(l => new { l.Producto, l.Identificador, l.CantidadIngresada }).ToList();

                //Generar datos para balanceo de documento y para creacion de Acta
                var balanceo = new List<InformacionBalanceo>();

                foreach (var detalle in detalles)
                {
                    var lineaDeclaradaRecibida = documentoIngreso.Lineas
                        .FirstOrDefault(l => l.Producto == detalle.CodigoProducto
                            && l.Identificador == detalle.Identificador
                            && detalle.CantidadRecibida == l.CantidadIngresada);

                    var existeLineaDeclarada = documentoIngreso.Lineas.Any(l => l.Producto == detalle.CodigoProducto);

                    if (existeLineaDeclarada || detalle.CantidadAgendada > 0)
                    {
                        //Dato para Balancear
                        balanceo.Add(new InformacionBalanceo()
                        {
                            CantidadIngresada = detalle.CantidadRecibida,
                            NumeroIdentificador = detalle.Identificador,
                            Producto = detalle.CodigoProducto,
                            RecepcionCompleta = lineaDeclaradaRecibida != null,
                        });
                    }
                }

                var nuTransaccion = uow.GetTransactionNumber();
                var resultBalanceo = documentoIngreso.BalancearLote(balanceo);

                foreach (DocumentoLinea lineaModificada in resultBalanceo.LineasModificadas)
                {
                    uow.DocumentoRepository.UpdateDetail(documentoIngreso, lineaModificada, nuTransaccion);
                }

                foreach (DocumentoLinea lineaAgregada in resultBalanceo.LineasAgregadas)
                {
                    uow.DocumentoRepository.AddDetail(documentoIngreso, lineaAgregada, nuTransaccion);
                }

                foreach (DocumentoLinea lineaEliminada in resultBalanceo.LineasEliminadas)
                {
                    lineaEliminada.FechaModificacion = DateTime.Now;
                    lineaEliminada.NumeroTransaccionDelete = nuTransaccion;
                    uow.DocumentoRepository.UpdateDetailWithoutDocument(documentoIngreso.Numero, documentoIngreso.Tipo, lineaEliminada, nuTransaccion);
                }

                uow.SaveChanges();

                foreach (DocumentoLinea lineaEliminada in resultBalanceo.LineasEliminadas)
                {
                    uow.DocumentoRepository.RemoveDetail(documentoIngreso, lineaEliminada, nuTransaccion);
                }

                //Marcar documento como balanceado
                documentoIngreso.ConfirmarBalanceo();

                //Crear Acta de ingreso si es necesario, a partir de documento balanceado
                List<DocumentoLinea> lineasDesafectar = new List<DocumentoLinea>();
                List<InformacionActaRecepcion> actaBalanceo = new List<InformacionActaRecepcion>();

                foreach (var detalle in detalles)
                {
                    if (detalle.CantidadRecibida > 0)
                    {
                        var lineaDeclaradaRecibida = documentoIngreso.Lineas.FirstOrDefault(l => l.Producto == detalle.CodigoProducto && l.Identificador == detalle.Identificador);

                        //Si existe una linea en el documento para la pareja Producto-Identificador y las cantidades Ingresadas difieren, la linea es candidata para recibir un acta
                        if (lineaDeclaradaRecibida != null && lineaDeclaradaRecibida.CantidadIngresada != detalle.CantidadRecibida) //Producto declarado y recibido, diferencia en cantidades
                        {
                            decimal? cantidadActa = detalle.CantidadRecibida - lineaDeclaradaRecibida.CantidadIngresada;

                            //Dato para acta
                            actaBalanceo.Add(new InformacionActaRecepcion()
                            {
                                NumeroIdentificador = detalle.Identificador,
                                Producto = detalle.CodigoProducto,
                                CantidadActa = cantidadActa,
                                CIF = (lineaDeclaradaRecibida.CIF / lineaDeclaradaRecibida.CantidadIngresada) * Math.Abs((decimal)cantidadActa),
                                FOB = (lineaDeclaradaRecibida.ValorMercaderia / lineaDeclaradaRecibida.CantidadIngresada) * Math.Abs((decimal)cantidadActa),
                                Empresa = lineaDeclaradaRecibida.Empresa,
                                Faixa = lineaDeclaradaRecibida.Faixa
                            });

                            if (cantidadActa < 0)
                            {
                                // Remover la reserva documental de las unidades desafectadas (ej: cuando se realiza cross-docking de 1 fase)
                                lineaDeclaradaRecibida.CantidadReservada = Math.Max(0, (lineaDeclaradaRecibida.CantidadReservada ?? 0) - Math.Abs((decimal)cantidadActa));

                                lineaDeclaradaRecibida.CantidadDesafectada = (lineaDeclaradaRecibida.CantidadDesafectada ?? 0) + (Math.Abs((decimal)cantidadActa));
                                lineasDesafectar.Add(lineaDeclaradaRecibida);
                            }
                        }
                        else if (lineaDeclaradaRecibida == null && detalle.CantidadRecibida > 0) //Producto recibido que no esta declarado en documento de ingreso
                        {
                            //Dato para acta
                            actaBalanceo.Add(new InformacionActaRecepcion()
                            {
                                NumeroIdentificador = detalle.Identificador,
                                Producto = detalle.CodigoProducto,
                                CantidadActa = detalle.CantidadRecibida,
                                CIF = null,
                                FOB = null,
                                Empresa = detalle.IdEmpresa,
                                Faixa = 1
                            });
                        }
                    }
                }

                //Recorrer el documento comparando contra lo recibido para saber si existen lineas declaradas en el documento que no se recibieron
                foreach (var lineaDeclarada in documentoIngreso.Lineas)
                {
                    var productoRecibido = detalles.Any(d => d.CodigoProducto == lineaDeclarada.Producto && d.Identificador == lineaDeclarada.Identificador && d.CantidadRecibida > 0);

                    if (!productoRecibido) //Producto declarado no recibido
                    {
                        decimal? cantidadActa = -lineaDeclarada.CantidadIngresada;

                        //Dato para acta
                        actaBalanceo.Add(new InformacionActaRecepcion()
                        {
                            NumeroIdentificador = lineaDeclarada.Identificador,
                            Producto = lineaDeclarada.Producto,
                            CantidadActa = cantidadActa,
                            CIF = lineaDeclarada.CIF,
                            FOB = lineaDeclarada.ValorMercaderia,
                            Empresa = lineaDeclarada.Empresa,
                            Faixa = lineaDeclarada.Faixa
                        });

                        // Remover la reserva documental de las unidades desafectadas (ej: cuando se realiza cross-docking de 1 fase)
                        lineaDeclarada.CantidadReservada = Math.Max(0, (lineaDeclarada.CantidadReservada ?? 0) - Math.Abs((decimal)cantidadActa));

                        lineaDeclarada.CantidadDesafectada = (lineaDeclarada.CantidadDesafectada ?? 0) + (Math.Abs((decimal)cantidadActa));
                        lineasDesafectar.Add(lineaDeclarada);
                    }
                }

                if (actaBalanceo.Count > 0)
                {
                    accion = AccionDocumento.FinalizarConDiferencia;
                    this.CrearActaIngresoPorBalanceoDeLote(uow, documentoIngreso, actaBalanceo, userId);
                }

                //Modificar lineas de ingreso desafectadas por el acta
                foreach (DocumentoLinea lineaModificada in lineasDesafectar)
                {
                    uow.DocumentoRepository.UpdateDetail(documentoIngreso, lineaModificada, nuTransaccion);
                }

                var estadoDestino = uow.DocumentoRepository.GetEstadoDestino(documentoIngreso.Tipo, documentoIngreso.Estado, accion);

                if (string.IsNullOrEmpty(estadoDestino))
                {
                    if (accion == AccionDocumento.Finalizar)
                        throw new ValidationFailedException("General_Sec0_Error_FinalizacionDocumentoNoDefinida", new string[] { documentoIngreso.Tipo, documentoIngreso.Estado });
                    else
                        throw new ValidationFailedException("General_Sec0_Error_FinalizacionConDiferenciaDocumentoNoDefinida", new string[] { documentoIngreso.Tipo, documentoIngreso.Estado });
                }

                documentoIngreso.Estado = estadoDestino;
                documentoIngreso.Finalizar();

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                errorMsg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                result = false;
            }

            return result;
        }

        public virtual void CrearActaIngresoPorBalanceoDeLote(IUnitOfWork uow, IDocumentoIngreso documentoIngreso, List<InformacionActaRecepcion> infoLineasActa, int userId)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var tipoActa = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_ACTA, documentoIngreso.Empresa.Value);
            var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(tipoActa);
            var tpDoc = uow.DocumentoTipoRepository.GetTipoDocumento(tipoActa);

            IDocumentoActa actaIngreso = this._factoryService.CreateDocumentoActa(tpDoc.TipoDocumento);
            actaIngreso.InReference = documentoIngreso;
            actaIngreso.Tipo = tpDoc.TipoDocumento;
            actaIngreso.Numero = actaIngreso.GetNumeroDocumento(uow);
            //actaIngreso.Agenda = documentoIngreso.Agenda;
            actaIngreso.Anexo1 = documentoIngreso.Anexo1;
            actaIngreso.Anexo2 = documentoIngreso.Anexo2;
            actaIngreso.Camion = documentoIngreso.Camion;
            actaIngreso.CantidadBulto = documentoIngreso.CantidadBulto;
            actaIngreso.CantidadContenedor20 = documentoIngreso.CantidadContenedor20;
            actaIngreso.CantidadContenedor40 = documentoIngreso.CantidadContenedor40;
            actaIngreso.Cliente = documentoIngreso.Cliente;
            actaIngreso.Configuracion = documentoIngreso.Configuracion;
            actaIngreso.Conocimiento = documentoIngreso.Conocimiento;
            actaIngreso.Predio = documentoIngreso.Predio;
            actaIngreso.Descripcion = tpDoc.DescripcionTipoDocumento;
            actaIngreso.Despachante = documentoIngreso.Despachante;
            actaIngreso.DocumentoAduana = documentoIngreso.DocumentoAduana;
            actaIngreso.DocumentoReferenciaExterna = documentoIngreso.DocumentoReferenciaExterna;
            actaIngreso.DocumentoTransporte = documentoIngreso.DocumentoTransporte;
            actaIngreso.Empresa = documentoIngreso.Empresa;
            actaIngreso.Estado = estadoInicial;
            actaIngreso.Factura = documentoIngreso.Factura;
            actaIngreso.FechaAlta = DateTime.Now;
            actaIngreso.FechaDTI = documentoIngreso.FechaDTI;
            actaIngreso.Moneda = documentoIngreso.Moneda;
            actaIngreso.NumeroDTI = documentoIngreso.NumeroDTI;
            actaIngreso.Transportista = documentoIngreso.Transportista;
            actaIngreso.UnidadMedida = documentoIngreso.UnidadMedida;
            actaIngreso.Usuario = userId;
            actaIngreso.ValorArbitraje = documentoIngreso.ValorArbitraje;
            actaIngreso.ValorFlete = 0;
            actaIngreso.ValorOtrosGastos = 0;
            actaIngreso.ValorSeguro = 0;
            actaIngreso.Via = documentoIngreso.Via;
            actaIngreso.Volumen = documentoIngreso.Volumen;

            //Generar Lineas
            var lineasEgreso = infoLineasActa.Where(l => l.CantidadActa < 0).ToList();
            var lineasIngreso = infoLineasActa.Where(l => l.CantidadActa > 0).ToList();

            actaIngreso.Lineas = this.CrearLineasIngresoActa(lineasIngreso, actaIngreso, documentoIngreso, uow);
            actaIngreso.OutDetail = this.CrearLineasEgresoActa(lineasEgreso, actaIngreso, documentoIngreso, userId, uow);

            //Crear relacion acta / documento ingreso
            actaIngreso.ActaDetail.Add(new DocumentoActaDetalle()
            {
                NumeroDocumento = documentoIngreso.Numero,
                TipoDocumento = documentoIngreso.Tipo,
                NumeroDocumentoActa = actaIngreso.Numero,
                TipoDocumentoActa = actaIngreso.Tipo
            });

            uow.DocumentoRepository.AddActa(actaIngreso, nuTransaccion);
        }

        public virtual List<DocumentoLineaEgreso> CrearLineasEgresoActa(List<InformacionActaRecepcion> infoLineasActa, IDocumentoActa acta, IDocumentoIngreso documentoIngreso, int userId, IUnitOfWork uow)
        {
            List<DocumentoLineaEgreso> lineas = new List<DocumentoLineaEgreso>();

            foreach (var lineaBalanceo in infoLineasActa)
            {
                var lineaModificada = documentoIngreso.Lineas.FirstOrDefault(l => l.Producto == lineaBalanceo.Producto && l.Identificador == lineaBalanceo.NumeroIdentificador);

                lineas.Add(new DocumentoLineaEgreso()
                {
                    DocumentoIngreso = documentoIngreso,
                    CantidadDesafectada = Math.Abs((decimal)lineaBalanceo.CantidadActa),
                    CIF = (decimal)(lineaBalanceo.CIF ?? 0),
                    FOB = (decimal)(lineaBalanceo.FOB ?? 0),
                    Empresa = lineaBalanceo.Empresa,
                    Faixa = lineaBalanceo.Faixa,
                    FechaAlta = DateTime.Now,
                    Identificador = lineaBalanceo.NumeroIdentificador,
                    Producto = lineaBalanceo.Producto,
                    Usuario = userId,
                    Numero = uow.DocumentoRepository.GetNumeroSecuenciaDetalleEgreso()
                });

                lineaModificada.CantidadDesafectada = Math.Abs((decimal)lineaBalanceo.CantidadActa);
            }

            return lineas;
        }

        public virtual List<DocumentoLinea> CrearLineasIngresoActa(List<InformacionActaRecepcion> infoLineasActa, IDocumentoActa acta, IDocumentoIngreso documentoIngreso, IUnitOfWork uow)
        {
            List<DocumentoLinea> lineas = new List<DocumentoLinea>();

            foreach (var lineaBalanceo in infoLineasActa)
            {
                Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(lineaBalanceo.Empresa, lineaBalanceo.Producto);
                var nroIdentificadorFinal = producto.ParseIdentificador(lineaBalanceo.NumeroIdentificador);

                lineas.Add(new DocumentoLinea()
                {
                    CIF = (decimal)(lineaBalanceo.CIF ?? 0),
                    ValorMercaderia = (decimal)(lineaBalanceo.FOB ?? 0),
                    Empresa = lineaBalanceo.Empresa,
                    Faixa = lineaBalanceo.Faixa,
                    FechaAlta = DateTime.Now,
                    Identificador = lineaBalanceo.NumeroIdentificador,
                    Producto = lineaBalanceo.Producto,
                    CantidadIngresada = Math.Abs((decimal)lineaBalanceo.CantidadActa),
                    DescripcionProducto = producto.Descripcion,
                    Situacion = 15
                });
            }

            return lineas;
        }

        #region INTERFACES

        public virtual CrearDocumentoIngresoResponse CrearDocumentoIngresoDANF(CrearDocumentoIngresoRequest request, IUnitOfWork uow)
        {
            int usuario = 0;
            var response = new CrearDocumentoIngresoResponse();
            var errors = new List<Error>();
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                //Validar usuario requerido
                if (!int.TryParse(request.USER, out usuario))
                {
                    errors.Add(new Error("USER", request.USER, "Usuario inválido"));
                }

                string nuDocumentoIngreso = "";

                //Validar datos obligatorios
                errors.AddRange(this.ValidarCamposObligatoriosIngreso(request, uow, usuario, out nuDocumentoIngreso));
                //Validar datos opcionales
                errors.AddRange(this.ValidarCamposOpcionalesIngreso(request, uow));
                //Validar detalles de documento de ingreso
                errors.AddRange(this.ValidarDetallesIngreso(request, uow));

                if (errors.Count == 0)
                {
                    // Busco agente para enviar a creción de documento
                    Agente agente = uow.AgenteRepository.GetAgente(int.Parse(request.CD_EMPRESA), request.CD_AGENTE, request.TP_AGENTE);

                    //Crear cabezal de documento de ingreso
                    IDocumentoIngreso documento = this.CrearDocumentoObject(request, usuario, agente, nuDocumentoIngreso, uow);

                    //Crear detalles de ingreso
                    this.AgregarDetallesDocumento(documento, request.DETALLES, uow);

                    //Guardar documento de ingreso
                    uow.DocumentoRepository.AddIngreso(documento, nuTransaccion);

                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Errors = errors;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                errors.Add(new Error("Mod.Documental", "Excepcion CrearDocumentoIngresoDANF", ex.InnerException == null ? ex.Message : ex.InnerException.Message));

                response.Errors = errors;
                response.Success = false;
            }

            return response;
        }

        public virtual IDocumentoIngreso CrearDocumentoObject(CrearDocumentoIngresoRequest request, int usuario, Agente agente, string nuDocumentoIngreso, IUnitOfWork uow)
        {
            var tipoDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(request.TP_DOCUMENTO);
            var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(request.TP_DOCUMENTO);
            var documento = this._factoryService.CreateDocumentoIngreso(request.TP_DOCUMENTO);
            var culture = this._identity.GetFormatProvider();

            if (documento != null)
            {
                documento.Numero = nuDocumentoIngreso;
                documento.Tipo = request.TP_DOCUMENTO;
                documento.Descripcion = tipoDocumento.DescripcionTipoDocumento;
                documento.Usuario = usuario;
                documento.DocumentoAduana = new DocumentoAduana();
                documento.Moneda = request.CD_MONEDA;

                if ((request.CD_MONEDA != "1" && !string.IsNullOrEmpty(request.CD_MONEDA)))
                {
                    decimal arbitraje;
                    if (decimal.TryParse(request.VL_ARBITRAJE, NumberStyles.Any, culture, out arbitraje))
                        documento.ValorArbitraje = arbitraje;
                }
                else
                {
                    documento.ValorArbitraje = 1;
                }

                documento.GeneraAgenda = tipoDocumento.AutoAgendable;
                documento.Situacion = 15;
                documento.FechaAlta = DateTime.Now;
                documento.Empresa = int.Parse(request.CD_EMPRESA);
                documento.Via = request.CD_VIA;
                documento.Factura = request.NU_FACTURA;
                documento.Conocimiento = request.NU_CONOCIMIENTO;

                decimal qtBultos;
                if (decimal.TryParse(request.QT_BULTO, NumberStyles.Number, culture, out qtBultos))
                    documento.CantidadBulto = qtBultos;

                int transportista;
                if (int.TryParse(request.CD_TRANSPORTISTA, out transportista))
                    documento.Transportista = transportista;

                documento.Estado = estadoInicial;

                DateTime? parsedDate;
                if (request.DT_PROGRAMADO.TryParseFromIso(out parsedDate))
                {
                    documento.FechaProgramado = parsedDate;
                }

                documento.UnidadMedida = request.CD_UNIDAD_MEDIDA_BULTO;
                documento.NumeroImportacion = request.NU_IMPORT;
                documento.NumeroExportacion = request.NU_EXPORT;

                short cdDespachante;
                if (short.TryParse(request.CD_DESPACHANTE, out cdDespachante))
                    documento.Despachante = cdDespachante;

                decimal ValorSeguro;
                if (decimal.TryParse(request.VL_SEGURO, NumberStyles.Any, culture, out ValorSeguro))
                    documento.ValorSeguro = ValorSeguro;
                else
                    documento.ValorSeguro = 0;

                decimal qtVolumen;
                if (decimal.TryParse(request.QT_VOLUMEN, NumberStyles.Any, culture, out qtVolumen))
                    documento.Volumen = qtVolumen;

                decimal qtPeso;
                if (decimal.TryParse(request.QT_PESO, NumberStyles.Any, culture, out qtPeso))
                    documento.Peso = qtPeso;

                short tpAlmacSeguro;
                if (short.TryParse(request.TP_ALMACENAJE_Y_SEGURO, out tpAlmacSeguro))
                    documento.TipoAlmacenajeYSeguro = tpAlmacSeguro;

                documento.Predio = request.NU_PREDIO;

                short qtContenedor20;
                if (short.TryParse(request.QT_CONTENEDOR20, out qtContenedor20))
                    documento.CantidadContenedor20 = qtContenedor20;

                short qtContenedor40;
                if (short.TryParse(request.QT_CONTENEDOR40, out qtContenedor40))
                    documento.CantidadContenedor40 = qtContenedor40;

                decimal vlFlete;
                if (decimal.TryParse(request.VL_FLETE, NumberStyles.Any, culture, out vlFlete))
                    documento.ValorFlete = vlFlete;
                else
                    documento.ValorFlete = 0;

                documento.IdManual = "S";
                documento.AgendarAutomaticamente = tipoDocumento.AutoAgendable;

                decimal vlOtrosGastos;
                if (decimal.TryParse(request.VL_OTROS_GASTOS, NumberStyles.Any, culture, out vlOtrosGastos))
                    documento.ValorOtrosGastos = vlOtrosGastos;
                else
                    documento.ValorOtrosGastos = 0;

                documento.DocumentoTransporte = "";
                documento.Anexo1 = request.DS_ANEXO1;
                documento.Anexo2 = request.DS_ANEXO2;
                documento.Anexo3 = request.DS_ANEXO3;
                documento.Anexo4 = request.DS_ANEXO4;
                documento.Anexo5 = request.DS_ANEXO5;
                documento.Cliente = agente.CodigoInterno;
                documento.Validado = true;

                decimal icms;
                if (decimal.TryParse(request.ICMS, NumberStyles.Any, culture, out icms))
                    documento.ICMS = icms;

                decimal ii;
                if (decimal.TryParse(request.II, NumberStyles.Any, culture, out ii))
                    documento.II = ii;

                decimal ipi;
                if (decimal.TryParse(request.IPI, NumberStyles.Any, culture, out ipi))
                    documento.IPI = ipi;

                decimal iisuspenso;
                if (decimal.TryParse(request.IISUSPENSO, NumberStyles.Any, culture, out iisuspenso))
                    documento.IISUSPENSO = iisuspenso;

                decimal ipisuspenso;
                if (decimal.TryParse(request.IPISUSPENSO, NumberStyles.Any, culture, out ipisuspenso))
                    documento.IPISUSPENSO = ipisuspenso;

                decimal pisconfins;
                if (decimal.TryParse(request.PISCONFINS, NumberStyles.Any, culture, out pisconfins))
                    documento.PISCONFINS = pisconfins;

                int cdRegimenAduana;
                if (int.TryParse(request.CD_REGIMEN_ADUANA, NumberStyles.Any, culture, out cdRegimenAduana))
                    documento.RegimenAduana = cdRegimenAduana;
            }

            return documento;
        }

        public virtual void AgregarDetallesDocumento(IDocumentoIngreso documento, List<CrearDetalleIngresoRequest> detalles, IUnitOfWork uow)
        {
            var culture = this._identity.GetFormatProvider();

            //Calcular CIF Y FOB para detalles de ingreso
            decimal? cantTotal = detalles.Sum(l => decimal.Parse(l.QT_INGRESADA ?? "0", culture));
            decimal? valorTotal = detalles.Sum(l => decimal.Parse(l.VL_MERCADERIA ?? "0", culture));

            decimal? raz = ((valorTotal + (documento.ValorSeguro ?? 0) + (documento.ValorFlete ?? 0) + (documento.ValorOtrosGastos ?? 0)) / valorTotal);

            //Crear detalle de ingreso con manejo de identificador
            Producto producto;
            DocumentoLinea linea;
            foreach (var detalle in detalles)
            {
                linea = new DocumentoLinea();
                producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario((int)documento.Empresa, detalle.CD_PRODUTO);

                linea.Empresa = (int)documento.Empresa;
                linea.Producto = detalle.CD_PRODUTO;
                linea.Faixa = int.Parse(detalle.CD_FAIXA);
                linea.Identificador = producto.ParseIdentificador(detalle.NU_IDENTIFICADOR);
                linea.CantidadIngresada = Math.Round(Convert.ToDecimal(detalle.QT_INGRESADA), 3);
                linea.ValorMercaderia = Math.Round(Convert.ToDecimal(detalle.VL_MERCADERIA), 3);
                linea.ValorTributo = Math.Round(Convert.ToDecimal(detalle.VL_TRIBUTO), 3);
                linea.CIF = (linea.ValorMercaderia * raz * documento.ValorArbitraje);
                linea.FechaAlta = DateTime.Now;
                linea.Situacion = documento.Situacion;
                linea.IdentificadorIngreso = producto.ParseIdentificador(detalle.NU_IDENTIFICADOR);

                documento.Lineas.Add(linea);
            }
        }

        public virtual List<Error> ValidarCamposObligatoriosIngreso(CrearDocumentoIngresoRequest request, IUnitOfWork uow, int usuario, out string nuDocumento)
        {
            var tipoDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(request.TP_DOCUMENTO);

            List<Error> errors = new List<Error>();

            nuDocumento = request.NU_DOCUMENTO;

            //Validar empresa existente
            int cdEmpresa = -1;
            if (string.IsNullOrEmpty(request.CD_EMPRESA) || !int.TryParse(request.CD_EMPRESA, out cdEmpresa))
            {
                errors.Add(new Error("CD_EMPRESA", request.CD_EMPRESA, "Valor código empresa inválido"));
            }

            if (!uow.EmpresaRepository.AnyEmpresa(cdEmpresa))
            {
                errors.Add(new Error("CD_EMPRESA", request.CD_EMPRESA, "Empresa no existe"));
            }

            //Validar Cliente obligatorio
            if (!uow.AgenteRepository.AnyAgente(request.CD_AGENTE, request.TP_AGENTE, cdEmpresa))
            {
                errors.Add(new Error("CD_AGENTE", request.CD_AGENTE, "Agente no encontrado para la empresa"));
            }

            //Validacion para numero de documento de ingreso -> Largo 10 numerico
            long nuDocValidator;
            if (string.IsNullOrEmpty(request.NU_DOCUMENTO) || !long.TryParse(request.NU_DOCUMENTO, out nuDocValidator))
            {
                errors.Add(new Error("NU_DOCUMENTO", request.NU_DOCUMENTO, "Número de documento inválido"));
            }

            if (request.NU_DOCUMENTO.Length > tipoDocumento.LargoMaximoNumeroDocumento)
            {
                errors.Add(new Error("NU_DOCUMENTO", request.NU_DOCUMENTO, "Largo de numero de documento excedido"));
            }

            if (tipoDocumento.LargoPrefijo > 0)
            {
                string prefijo = this.ObtenerPrefijo(cdEmpresa, tipoDocumento.LargoPrefijo, uow);
                nuDocumento = string.Format("{0}{1}", prefijo, request.NU_DOCUMENTO);
            }

            if (!uow.DocumentoTipoRepository.GetDocumentosIngresoHabilitados().Any(a => a.TipoDocumento == request.TP_DOCUMENTO))
            {
                errors.Add(new Error("TP_DOCUMENTO", request.TP_DOCUMENTO, "Tipo de documento no existe o no está habilitado"));
            }

            if (tipoDocumento.InterfazEntradaHabilitada)
            {
                errors.Add(new Error("TP_DOCUMENTO", request.TP_DOCUMENTO, "Tipo de documento no válido"));
            }

            //Validar si existe documento para pareja nu_documento y tp_documento
            var documentoExistente = uow.DocumentoRepository.GetIngreso(nuDocumento, request.TP_DOCUMENTO);
            if (documentoExistente != null)
            {
                errors.Add(new Error("TP_DOCUMENTO - NU_DOCUMENTO", string.Format("{0}-{1}", request.TP_DOCUMENTO, nuDocumento), "Documento duplicado"));
            }

            //Fecha de retiro obligatoria
            DateTime? parsedDate;
            if (!request.DT_PROGRAMADO.TryParseFromIso(out parsedDate))
            {
                errors.Add(new Error("DT_PROGRAMADO", request.DT_PROGRAMADO, "Fecha programado inválida"));
            }

            //Validar via - obligatoria y existente
            if (!uow.ViaRepository.AnyVia(request.CD_VIA))
            {
                errors.Add(new Error("CD_VIA", request.CD_VIA, "Via no válida"));
            }

            if (!validoPredio(uow, request.NU_PREDIO, usuario, out string msg))
                errors.Add(new Error("NU_PREDIO", request.NU_PREDIO, msg));


            //Documento ingreso debe tener al menos 1 detalle
            if (request.DETALLES == null || request.DETALLES.Count == 0)
            {
                errors.Add(new Error("DETALLES", "DETALLES", "Documento de ingreso debe poseer al menos un detalle"));
            }

            //Validar que no existan lineas duplicadas
            var detallesAgrupados = request.DETALLES.GroupBy(d => new { d.CD_PRODUTO, d.NU_IDENTIFICADOR, d.CD_FAIXA }).Select(d => d);

            if (detallesAgrupados.Any(grupo => grupo.Count() > 1))
            {
                errors.Add(new Error("DETALLES", "DETALLES", "Detalles de documento duplicados"));
            }

            return errors;
        }

        public virtual bool validoPredio(IUnitOfWork uow, string predio, int usuario, out string msg)
        {
            msg = string.Empty;
            if (string.IsNullOrEmpty(predio) || predio.Length > 10)
                msg = "Predio no válido";
            else if (!uow.PredioRepository.AnyPredio(predio))
                msg = "Predio no existe";
            else if (!uow.PredioRepository.AnyPrediosUsuario(predio, usuario))
                msg = "Predio no válido";

            return string.IsNullOrEmpty(msg);
        }

        public virtual List<Error> ValidarCamposOpcionalesIngreso(CrearDocumentoIngresoRequest request, IUnitOfWork uow)
        {
            List<Error> errors = new List<Error>();

            //Despachante
            int cdDespachante;
            if (!string.IsNullOrEmpty(request.CD_DESPACHANTE) && (!int.TryParse(request.CD_DESPACHANTE, out cdDespachante) || !uow.DespachanteRepository.AnyDespachante(cdDespachante)))
            {
                errors.Add(new Error("CD_DESPACHANTE", request.CD_DESPACHANTE, "Despachante inválido o inexistente"));
            }

            //Contenedor20
            short qtContenedor;
            if (!string.IsNullOrEmpty(request.QT_CONTENEDOR20) && (request.QT_CONTENEDOR20.Length > 2 || !short.TryParse(request.QT_CONTENEDOR20, out qtContenedor)))
            {
                errors.Add(new Error("QT_CONTENEDOR20", request.QT_CONTENEDOR20, "Valor de contenedor 20 inválido"));
            }

            //Contendor40
            if (!string.IsNullOrEmpty(request.QT_CONTENEDOR40) && (request.QT_CONTENEDOR40.Length > 2 || !short.TryParse(request.QT_CONTENEDOR40, out qtContenedor)))
            {
                errors.Add(new Error("QT_CONTENEDOR40", request.QT_CONTENEDOR40, "Valor de contenedor 40 inválido"));
            }

            //Moneda
            if (!string.IsNullOrEmpty(request.CD_MONEDA) && (request.CD_MONEDA.Length > 15 || !uow.MonedaRepository.ExisteMoneda(request.CD_MONEDA)))
            {
                errors.Add(new Error("CD_MONEDA", request.CD_MONEDA, "Código de moneda inválido - Moneda inexistente"));
            }

            //Arbitraje
            if (!string.IsNullOrEmpty(request.CD_MONEDA) && request.CD_MONEDA != "1" && (string.IsNullOrEmpty(request.VL_ARBITRAJE) || !this.ValidarDecimal(request.VL_ARBITRAJE, 9, 4)))
            {
                errors.Add(new Error("VL_ARBITRAJE", request.VL_ARBITRAJE, "Arbitraje inválido (9,4)"));
            }

            //Unidad Medida
            if (!string.IsNullOrEmpty(request.CD_UNIDAD_MEDIDA_BULTO) && (request.CD_UNIDAD_MEDIDA_BULTO.Length > 10 || !uow.UnidadMedidaRepository.ExisteUnidadMedida(request.CD_UNIDAD_MEDIDA_BULTO)))
            {
                errors.Add(new Error("CD_UNIDAD_MEDIDA_BULTO", request.CD_UNIDAD_MEDIDA_BULTO, "Código de unidad de medida inválido - o - Unidad de medida inexistente"));
            }

            //Desc.Doc
            if (!string.IsNullOrEmpty(request.DS_DOCUMENTO) && (request.CD_UNIDAD_MEDIDA_BULTO.Length > 100))
            {
                errors.Add(new Error("DS_DOCUMENTO", request.DS_DOCUMENTO, "Largo 100 excedido"));
            }

            //Factura
            if (!string.IsNullOrEmpty(request.NU_FACTURA) && (request.NU_FACTURA.Length > 200))
            {
                errors.Add(new Error("NU_FACTURA", request.NU_FACTURA, "Largo 200 excedido"));
            }

            //QT_BULTO
            int qtBulto = -1;
            if (!string.IsNullOrEmpty(request.QT_BULTO) && (request.QT_BULTO.Length > 6 || !int.TryParse(request.QT_BULTO, out qtBulto) || qtBulto < 0))
            {
                errors.Add(new Error("QT_BULTO", request.QT_BULTO, "Valor de bultos inválido - Largo máximo (6) - Positivo"));
            }

            //QT_VOLUMEN
            if (!string.IsNullOrEmpty(request.QT_VOLUMEN) && (!this.ValidarDecimal(request.QT_VOLUMEN, 12, 3)))
            {
                errors.Add(new Error("QT_VOLUMEN", request.QT_VOLUMEN, "Valor de volumen inválido - (12,3)"));
            }

            //QT_PESO
            if (!string.IsNullOrEmpty(request.QT_PESO) && (!this.ValidarDecimal(request.QT_PESO, 12, 3)))
            {
                errors.Add(new Error("QT_PESO", request.QT_PESO, "Valor de peso inválido - (12,3)"));
            }

            //NU_CONOCIMIENTO
            if (!string.IsNullOrEmpty(request.NU_CONOCIMIENTO) && request.NU_CONOCIMIENTO.Length > 12)
            {
                errors.Add(new Error("NU_CONOCIMIENTO", request.NU_CONOCIMIENTO, "Valor NU_CONOCIMIENTO excedido - (12)"));
            }

            //TRANSPORTADORA
            if (!string.IsNullOrEmpty(request.CD_TRANSPORTISTA) && (!int.TryParse(request.CD_TRANSPORTISTA, out int cdTransportadora) || !uow.TransportistaRepository.AnyTransportista(cdTransportadora)))
            {
                errors.Add(new Error("CD_TRANSPORTISTA", request.CD_TRANSPORTISTA, "Código de transportadora - o - Transportadora inexistente"));
            }

            //TIPO DE ALMACENAJE SEGURO
            if (!string.IsNullOrEmpty(request.TP_ALMACENAJE_Y_SEGURO) && (request.TP_ALMACENAJE_Y_SEGURO.Length > 3 || !short.TryParse(request.TP_ALMACENAJE_Y_SEGURO, out short tpAlmacenajeSeguro) || uow.TipoAlmacenajeSeguroRepository.AnyTipoAlmacenajeYSeguro(tpAlmacenajeSeguro)))
            {
                errors.Add(new Error("TP_ALMACENAJE_Y_SEGURO", request.TP_ALMACENAJE_Y_SEGURO, "Tipo almacenaje y seguro inválido o inexistente"));
            }

            //VALOR FLETE
            if (!string.IsNullOrEmpty(request.VL_FLETE) && (!this.ValidarDecimal(request.VL_FLETE, 12, 3)))
            {
                errors.Add(new Error("VL_FLETE", request.VL_FLETE, "Valor de flete inválido - (12,3)"));
            }

            //VALOR SEGURO
            if (!string.IsNullOrEmpty(request.VL_SEGURO) && (!this.ValidarDecimal(request.VL_SEGURO, 9, 4)))
            {
                errors.Add(new Error("VL_SEGURO", request.VL_SEGURO, "Valor de SEGURO inválido - (9,4)"));
            }

            //VALOR OTROS GASTOS
            if (!string.IsNullOrEmpty(request.VL_OTROS_GASTOS) && (!this.ValidarDecimal(request.VL_OTROS_GASTOS, 12, 3)))
            {
                errors.Add(new Error("VL_OTROS_GASTOS", request.VL_OTROS_GASTOS, "Valor de otros gastos inválido - (12,3)"));
            }

            //ANEXO1
            if (!string.IsNullOrEmpty(request.DS_ANEXO1) && request.DS_ANEXO1.Length > 200)
            {
                errors.Add(new Error("DS_ANEXO1", request.DS_ANEXO1, "Valor DS_ANEXO1 excedido - (12)"));
            }

            //ANEXO2
            if (!string.IsNullOrEmpty(request.DS_ANEXO2) && request.DS_ANEXO2.Length > 200)
            {
                errors.Add(new Error("DS_ANEXO2", request.DS_ANEXO2, "Valor DS_ANEXO2 excedido - (12)"));
            }

            //ICMS
            if (!string.IsNullOrEmpty(request.ICMS) && (!this.ValidarDecimal(request.ICMS, 9, 4)))
            {
                errors.Add(new Error("ICMS", request.ICMS, "Valor de ICMS inválido - (9,4)"));
            }

            //II
            if (!string.IsNullOrEmpty(request.II) && (!this.ValidarDecimal(request.II, 9, 4)))
            {
                errors.Add(new Error("II", request.II, "Valor de II inválido - (9,4)"));
            }

            //IPI
            if (!string.IsNullOrEmpty(request.IPI) && (!this.ValidarDecimal(request.IPI, 9, 4)))
            {
                errors.Add(new Error("IPI", request.IPI, "Valor de IPI inválido - (9,4)"));
            }

            //IISUSPENSO
            if (!string.IsNullOrEmpty(request.IISUSPENSO) && (!this.ValidarDecimal(request.IISUSPENSO, 9, 4)))
            {
                errors.Add(new Error("IISUSPENSO", request.IISUSPENSO, "Valor de IISUSPENSO inválido - (9,4)"));
            }

            //IPISUSPENSO
            if (!string.IsNullOrEmpty(request.IPISUSPENSO) && (!this.ValidarDecimal(request.IPISUSPENSO, 9, 4)))
            {
                errors.Add(new Error("IPISUSPENSO", request.IPISUSPENSO, "Valor de IPISUSPENSO inválido - (9,4)"));
            }

            //PISCONFINS
            if (!string.IsNullOrEmpty(request.PISCONFINS) && (!this.ValidarDecimal(request.PISCONFINS, 9, 4)))
            {
                errors.Add(new Error("PISCONFINS", request.PISCONFINS, "Valor de PISCONFINS inválido - (9,4)"));
            }

            //CD_REGIMEN_ADUANA
            if (!string.IsNullOrEmpty(request.CD_REGIMEN_ADUANA) && (request.CD_REGIMEN_ADUANA.Length > 10 || !int.TryParse(request.CD_REGIMEN_ADUANA, out int cdRegimenAduanero) || !uow.RegimenAduaneroRepository.AnyRegimenAduanero(cdRegimenAduanero)))
            {
                errors.Add(new Error("QT_BULTO", request.QT_BULTO, "Valor de bultos inválido - Largo máximo (6) - Positivo"));
            }

            return errors;
        }

        public virtual List<Error> ValidarDetallesIngreso(CrearDocumentoIngresoRequest request, IUnitOfWork uow)
        {
            var culture = this._identity.GetFormatProvider();
            var errors = new List<Error>();

            if (int.TryParse(request.CD_EMPRESA, out int cdEmpresa))
            {
                foreach (var detalle in request.DETALLES)
                {
                    //Validar producto
                    if (string.IsNullOrEmpty(detalle.CD_PRODUTO))
                    {
                        errors.Add(new Error("CD_PRODUTO", "CD_PRODUCTO", "Producto obligatorio", detalle.NU_REGISTRO));
                    }

                    if (!uow.ProductoRepository.AnyProducto(detalle.CD_PRODUTO, cdEmpresa))
                    {
                        errors.Add(new Error("CD_PRODUTO", detalle.CD_PRODUTO, "Producto no existe para empresa " + request.CD_EMPRESA, detalle.NU_REGISTRO));
                    }

                    //Validar identificador
                    if (!string.IsNullOrEmpty(detalle.NU_IDENTIFICADOR) && detalle.NU_IDENTIFICADOR.Length > 40)
                    {
                        errors.Add(new Error("NU_IDENTIFICADOR", detalle.NU_IDENTIFICADOR, "Largo de número indetificador excedido (40)", detalle.NU_REGISTRO));
                    }

                    //Valdiar cantidad ingresada
                    if (string.IsNullOrEmpty(detalle.QT_INGRESADA) || !this.ValidarDecimal(detalle.QT_INGRESADA, 12, 3))
                    {
                        errors.Add(new Error("QT_INGRESADA", detalle.QT_INGRESADA, "Cantidad ingresada inválida", detalle.NU_REGISTRO));
                    }
                    else if (decimal.Parse(detalle.QT_INGRESADA, culture) < 1)
                    {
                        errors.Add(new Error("QT_INGRESADA", detalle.QT_INGRESADA, "Cantidad ingresada debe ser mayor o igual a 1", detalle.NU_REGISTRO));
                    }

                    //Vlidar valor de mercaderia
                    if (string.IsNullOrEmpty(detalle.VL_MERCADERIA) || !this.ValidarDecimal(detalle.VL_MERCADERIA, 12, 3))
                    {
                        errors.Add(new Error("VL_MERCADERIA", detalle.VL_MERCADERIA, "Valor de mercaderia inválido", detalle.NU_REGISTRO));
                    }
                    else if (decimal.Parse(detalle.VL_MERCADERIA, culture) < Convert.ToDecimal(0.001))
                    {
                        errors.Add(new Error("VL_MERCADERIA", detalle.VL_MERCADERIA, "Valor de mercaderia debe ser mayor o igual a 0.001", detalle.NU_REGISTRO));
                    }

                    //Validar tributo
                    if (!string.IsNullOrEmpty(detalle.VL_TRIBUTO) && !this.ValidarDecimal(detalle.VL_TRIBUTO, 12, 3))
                    {
                        errors.Add(new Error("VL_TRIBUTO", detalle.VL_TRIBUTO, "Valor de tributo inválido", detalle.NU_REGISTRO));
                    }
                    else if (decimal.Parse(detalle.VL_TRIBUTO, culture) < 0)
                    {
                        errors.Add(new Error("VL_TRIBUTO", detalle.VL_TRIBUTO, "Valor de tributo debe ser mayor o igual a 0", detalle.NU_REGISTRO));
                    }

                    //Validar Faixa
                    if (!int.TryParse(detalle.CD_FAIXA, out int cdFaixa))
                    {
                        errors.Add(new Error("CD_FAIXA", detalle.CD_FAIXA, "Valor de faixa inválido", detalle.NU_REGISTRO));
                    }
                }
            }

            return errors;
        }

        public virtual bool ValidarDecimal(string decimalValue, int largo, int precision)
        {
            bool result = true;
            var culture = this._identity.GetFormatProvider();
            var separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (separator == ",")
            {
                if (decimalValue.Contains("."))
                    result = false;
            }
            else if (decimalValue.Contains(","))
                result = false;

            string pattern = @"^-?[0-9]{0," + largo.ToString() + "}([.,][0-9]{0," + precision.ToString() + "})?$";

            decimal outValue;
            if (!decimal.TryParse(decimalValue, NumberStyles.Any, culture, out outValue))
            {
                result = false;
            }
            else
            {
                if (!Regex.IsMatch(decimalValue, pattern))
                {
                    result = false;
                }
            }

            return result;
        }

        public virtual string ObtenerPrefijo(int cdEmpresa, short largo, IUnitOfWork uow)
        {
            string prefijo = "";
            var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);

            if (empresa != null)
            {
                prefijo = empresa.Nombre;
                Regex.Replace(prefijo, @"\s+", "");

                if (prefijo.Length > 2)
                    prefijo = prefijo.Substring(0, largo);
            }

            return prefijo;
        }

        #endregion
    }
}
