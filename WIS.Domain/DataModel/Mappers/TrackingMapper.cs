using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Security;
using WIS.Domain.Tracking.Models;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TrackingMapper : Mapper
    {
        #region Agente
        public virtual AgenteRequest MapToRequest(Agente agente)
        {
            AgenteRequest nuevo = new AgenteRequest
            {
                Codigo = agente.Codigo,
                Descripcion = agente.Descripcion,
                Tipo = agente.Tipo,
                CodigoEmpresa = agente.Empresa
            };

            return nuevo;
        }
        #endregion

        #region Objeto

        public virtual ModificarObjetosRequest MapToRequest(object datosObjeto, bool planificacion, bool baja, Dictionary<string, string> config, ModificarObjetosRequest request = null)
        {
            if (request == null)
                request = new ModificarObjetosRequest();

            ObjetoRequest objeto = null;

            if (planificacion)
            {
                var d = datosObjeto as PlanificacionCamion;
                var bultoReal = d.TipoBulto == TipoBultoContenedor.Real;

                objeto = new ObjetoRequest()
                {
                    Numero = bultoReal ? $"{d.IdExterno}#{d.TipoContenedor}#{d.IdExternoTracking}" : d.Numero.ToString(),
                    Descripcion = d.DescContenedor,
                    Tipo = bultoReal ? config["tpCont"] : config["tpContFicticio"],
                    CodigoBarras = bultoReal ? d.CodigoBarras : null,
                    Cantidad = 1, //contenedor.CantidadBulto
                    Volumen = d.VolumenTotal,
                    Peso = d.PesoTotal,
                    Alto = d.Alto,
                    Largo = d.Largo,
                    Profundidad = d.Produndidad,
                };
            }
            else
            {
                var d = datosObjeto as ContenedorEntrega;
                objeto = new ObjetoRequest()
                {
                    Numero = $"{d.IdExterno}#{d.TipoContenedor}#{d.IdExternoTracking}",
                    Descripcion = d.Descripcion,
                    Tipo = string.IsNullOrEmpty(d.TpObjetoTracking) ? "BULTO" : d.TpObjetoTracking,
                    CodigoBarras = d.CodigoBarras,
                    Cantidad = 1, //contenedor.CantidadBulto
                    Volumen = d.Volumen,
                    Peso = d.PesoTotal,
                    Alto = d.Alto,
                    Largo = d.Largo,
                    Profundidad = d.Produndidad,
                };
            }

            request.Objetos.Add(new ModificarObjetoRequest()
            {
                IdExternoObjeto = objeto.Numero,
                Accion = baja ? "B" : "M",
                Objeto = objeto,
            });

            return request;
        }
        #endregion

        #region Puntos de Entrega
        public virtual PuntoDeEntregaRequest MapToRequest(Pedido pedido, Agente agente, string direccion)
        {
            var culture = CultureInfo.InvariantCulture;
            //culture.NumberFormat.NumberDecimalSeparator = ".";

            PuntoDeEntregaRequest nuevo = new PuntoDeEntregaRequest
            {
                Descripcion = agente.Descripcion ?? "Default",
                Telefono = agente.TelefonoPrincipal,
                TipoTarea = "INTE_WIS",
                Direccion = new PuntoEntregaDireccion
                {
                    Direccion = direccion,
                    Zona = string.Empty,
                    Latitud = pedido.Latitud?.ToString(culture),
                    Longitud = pedido.Longitud?.ToString(culture)
                },
                //Agrupacion = pedido.Agrupacion,
                Agentes = new List<PuntoDeEntregaAgente>()
                {
                    new PuntoDeEntregaAgente()
                    {
                        Codigo= agente.Codigo,
                        CodigoEmpresa= agente.Empresa,
                        Tipo= agente.Tipo
                    }
                }
            };
            return nuevo;
        }
        public virtual PuntoDeEntregaRequest MapToRequest(Predio predio, string agrupacion)
        {
            PuntoDeEntregaRequest nuevo = new PuntoDeEntregaRequest
            {
                Descripcion = predio.Descripcion,
                Direccion = new PuntoEntregaDireccion
                {
                    Direccion = predio.Direccion,
                    Zona = string.Empty
                },
                Agrupacion = agrupacion
            };

            return nuevo;
        }

        public virtual PedidoNoPlanificado MapPedidoNoPlanificado(V_PEDIDOS_NO_PLANIFICADOS entity)
        {
            return new PedidoNoPlanificado
            {
                Pedido = entity.NU_PEDIDO,
                Empresa = entity.CD_EMPRESA,
                CodigoCliente = entity.CD_CLIENTE,
                CodigoAgente = entity.CD_AGENTE,
                TipoAgente = entity.TP_AGENTE,
                FechaEntrega = entity.DT_ENTREGA,
                FechaEmitido = entity.DT_EMITIDO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                ComparteContedorPicking = entity.VL_COMPARTE_CONTENEDOR_PICKING,
                ComparteContedorEntrega = entity.VL_COMPARTE_CONTENEDOR_ENTREGA
            };
        }
        public virtual PedidoNoPlanificado MapPedidoNoPlanificado(V_PEDIDOS_NO_PLANIFICADOS_JOB entity)
        {
            return new PedidoNoPlanificado
            {
                Pedido = entity.NU_PEDIDO,
                Empresa = entity.CD_EMPRESA,
                CodigoCliente = entity.CD_CLIENTE,
                CodigoAgente = entity.CD_AGENTE,
                TipoAgente = entity.TP_AGENTE,
                FechaEntrega = entity.DT_ENTREGA,
                FechaEmitido = entity.DT_EMITIDO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                ComparteContedorPicking = entity.VL_COMPARTE_CONTENEDOR_PICKING,
                ComparteContedorEntrega = entity.VL_COMPARTE_CONTENEDOR_ENTREGA
            };
        }
        public virtual PedidoNoFinalizado MapPedidoNoFinalizado(V_PEDIDOS_SIN_CERRAR entity)
        {
            return new PedidoNoFinalizado
            {
                Pedido = entity.NU_PEDIDO,
                Empresa = entity.CD_EMPRESA,
                CodigoCliente = entity.CD_CLIENTE,
                CodigoAgente = entity.CD_AGENTE,
                TipoAgente = entity.TP_AGENTE,
                FechaEntrega = entity.DT_ENTREGA,
                FechaEmitido = entity.DT_EMITIDO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual PuntoDeEntregaCliente Map(V_PUNTOS_ENTREGA_CLIENTE e)
        {
            return new PuntoDeEntregaCliente()
            {
                CodigoCliente = e.CD_CLIENTE,
                Empresa = e.CD_EMPRESA,
                CodigoAgente = e.CD_AGENTE,
                TipoAgente = e.TP_AGENTE,
                PuntoEntregaCliente = e.CD_PUNTO_ENTREGA_CLIENTE,
                DireccionCliente = e.DS_ENDERECO_CLIENTE,
                PuntoEntregaPedido = e.CD_PUNTO_ENTREGA_PEDIDO,
                DireccionPedido = e.DS_ENDERECO_PEDIDO,
            };
        }
        #endregion

        #region Tareas
        public virtual TareaRequest MapToRequest(Pedido pedido, Agente agente, string puntoEntrega, string fechaPrometida)
        {
            string CodigoExterno = !string.IsNullOrEmpty(pedido.ComparteContenedorEntrega) ? pedido.ComparteContenedorEntrega
                : $"{agente.Tipo}-{agente.Codigo}";

            string descTelefono = !string.IsNullOrEmpty(pedido.Telefono)
                    ? (pedido.Telefono + (!string.IsNullOrEmpty(pedido.TelefonoSecundario) ? (";" + pedido.TelefonoSecundario) : ""))
                    : !string.IsNullOrEmpty(pedido.TelefonoSecundario) ? pedido.TelefonoSecundario : agente.TelefonoPrincipal;

            TareaRequest nueva = new TareaRequest
            {
                CodigoExterno = CodigoExterno,
                Descripcion = $"Tarea de Entrega {CodigoExterno}",
                CodigoPuntoEntregaDestino = puntoEntrega,
                Prometida = fechaPrometida,
                SistemaCreacion = "WMS",
                Telefono = descTelefono,
                Pedidos = new List<PedidoRequest>()
                {
                    new PedidoRequest()
                    {
                        Numero = pedido.Id,
                        CodigoEmpresa = pedido.Empresa,
                        CodigoAgente = agente.Codigo,
                        TipoAgente = agente.Tipo,
                        FechaEmitido = pedido.FechaEmision?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        FechaEntrega = pedido.FechaEntrega?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        FechaRecibido = pedido.FechaAlta?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        Memo = pedido.Memo,
                        Anexo1 = pedido.Anexo,
                        Anexo2 = pedido.Anexo2,
                        Anexo3 = pedido.Anexo3,
                        Anexo4 = pedido.Anexo4,
                    }
                }
            };
            return nueva;
        }

        public virtual AnularPedidoRequest MapToRequest(Pedido pedido, Agente agente)
        {
            AnularPedidoRequest nuevo = new AnularPedidoRequest
            {
                Numero = pedido.Id,
                CodigoEmpresa = pedido.Empresa,
                CodigoAgente = agente.Codigo,
                TipoAgente = agente.Tipo
            };
            return nuevo;
        }
        public virtual AnularPedidoRequest MapToRequest(PedidoNoFinalizado pedido)
        {
            AnularPedidoRequest nuevo = new AnularPedidoRequest
            {
                Numero = pedido.Pedido,
                CodigoEmpresa = pedido.Empresa,
                CodigoAgente = pedido.CodigoAgente,
                TipoAgente = pedido.TipoAgente
            };
            return nuevo;
        }
        public virtual AnularPedidoRequest MapToRequest(PedidoPlanificadoCamion pedido)
        {
            AnularPedidoRequest nuevo = new AnularPedidoRequest
            {
                Numero = pedido.Pedido,
                CodigoEmpresa = pedido.Empresa,
                CodigoAgente = pedido.CodigoAgente,
                TipoAgente = pedido.TipoAgente
            };
            return nuevo;
        }

        public virtual PlanificacionDevolucion Map(V_PLANIFICACION_DEVOLUCION entity)
        {
            return new PlanificacionDevolucion
            {
                Agenda = entity.NU_AGENDA,
                Empresa = entity.CD_EMPRESA,
                CodigoAgente = entity.CD_AGENTE,
                TipoAgente = entity.TP_AGENTE,
                Numero = entity.NU_CONTENEDOR,
                DescripcionContenedor = entity.DS_CONTENEDOR,
                TipoBulto = entity.TP_BULTO,
                CantidadBulto = entity.QT_AGENDADO,
                VolumenTotal = entity.VL_CUBAGEM_TOTAL,
                PesoTotal = entity.VL_PESO_TOTAL,
                Alto = entity.VL_ALTURA,
                Largo = entity.VL_LARGURA,
                Produndidad = entity.VL_PROFUNDIDADE,
                TipoReferencia = entity.TP_REFERENCIA,
                CodigoReferencia = entity.CD_REFERENCIA,
                TipoContenedor = entity.TP_CONTENEDOR,
                Telefono = entity.NU_TELEFONO,
                FechaPrometida = entity.DT_PROMETIDA
            };
        }
        public virtual PlanificacionDevolucionDetalle Map(V_PLANIFICACION_DEVOLUCION_DET entity)
        {
            return new PlanificacionDevolucionDetalle
            {
                Agenda = entity.NU_AGENDA,
                Empresa = entity.CD_EMPRESA,
                CodigoExterno = entity.CD_EXTERNO,
                TipoLinea = entity.TP_LINEA,
                CodigoProducto = entity.CD_PRODUTO,
                CodigoBarras = entity.CD_BARRAS,
                DescipcionProducto = entity.DS_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Faixa = entity.CD_FAIXA,
                CantidadAgendada = entity.QT_AGENDADO,
                FechaVencimiento = entity.DT_FABRICACAO,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Anexo4 = entity.DS_ANEXO4
            };
        }
        #endregion

        #region Usuario
        public virtual UserRequest MapToRequest(Usuario usuario)
        {
            UserRequest nuevo = new UserRequest
            {
                UserId = usuario.UserId,
                LoginName = usuario.Username,
                DomainName = usuario.DomainName,
                FullName = usuario.Name,
                Email = usuario.Email
            };

            return nuevo;
        }
        #endregion

        #region Vehiculo
        public virtual TipoVehiculoRequest MapToRequest(VehiculoEspecificacion tpVehiculo)
        {
            TipoVehiculoRequest tipoNuevo = new TipoVehiculoRequest
            {
                CodigoExterno = tpVehiculo.Id.ToString(),
                Descripcion = tpVehiculo.Tipo,
                AdmiteZorra = tpVehiculo.AdmiteZorra,
                CargaLateral = tpVehiculo.AdmiteCargaLateral,
                Frigorificado = tpVehiculo.TieneRefrigeracion,
                SoloCabina = tpVehiculo.TieneSoloCabina
            };

            if (tpVehiculo.CapacidadPeso == null)
                tipoNuevo.PesoMaximo = null;
            else
                tipoNuevo.PesoMaximo = int.Parse(decimal.Round(tpVehiculo.CapacidadPeso ?? 0, 10).ToString(), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo); //TODO: REVISAR QUE FUNCIONE CORRECTAMENTE

            if (tpVehiculo.CapacidadVolumen == null)
                tipoNuevo.VolumenMaximo = null;
            else
                tipoNuevo.VolumenMaximo = int.Parse(decimal.Round(tpVehiculo.CapacidadVolumen ?? 0, 10).ToString(), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo); //TODO: REVISAR QUE FUNCIONE CORRECTAMENTE

            if (tpVehiculo.CapacidadPallet == null)
                tipoNuevo.CantidadBultosMaxima = null;
            else
                tipoNuevo.CantidadBultosMaxima = int.Parse(decimal.Round(tpVehiculo.CapacidadPallet ?? 0, 10).ToString(), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo); //TODO: REVISAR QUE FUNCIONE CORRECTAMENTE

            return tipoNuevo;
        }
        public virtual VehiculoRequest MapToRequest(Vehiculo vehiculo)
        {
            VehiculoRequest nuevo = new VehiculoRequest
            {
                CodigoExterno = vehiculo.Id.ToString(),
                Descripcion = vehiculo.Descripcion,
                CodigoExternoTipoVehiculo = vehiculo.Caracteristicas.Id.ToString(),
                Matricula = vehiculo.Matricula,
                Estado = vehiculo.Estado
            };

            return nuevo;
        }
        #endregion

        #region Viaje
        public virtual ViajeTeoricoRequest MapToViajeTeoricoRequest(Camion camion, string puntoEntregaPredio)
        {
            ViajeTeoricoRequest viaje = new ViajeTeoricoRequest
            {
                Numero = camion.Id,
                Descripcion = camion.Descripcion,
                CodigoExternoVehiculo = (camion.Vehiculo ?? 0).ToString(),
                Deposito = puntoEntregaPredio,
                NumeroEjecucionRuteo = 0,
                FechaEstimadaInicio = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };
            return viaje;
        }
        public virtual ViajeRealRequest MapToViajeRealRequest(Camion camion, string puntoEntregaPredio)
        {
            ViajeRealRequest viaje = new ViajeRealRequest
            {
                Numero = camion.Id,
                Descripcion = camion.Descripcion,
                CodigoExternoVehiculo = (camion.Vehiculo ?? 0).ToString(),
                Deposito = puntoEntregaPredio,
                NumeroEjecucionRuteo = 0,
                FechaEstimadaInicio = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };
            return viaje;
        }
        public virtual PuntoEntregaCarga MapPuntoEntregaCarga(V_PUNTOS_ENTREGA_TRACKING entity)
        {
            return new PuntoEntregaCarga
            {
                Camion = entity.CD_CAMION,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Pedido = entity.NU_PEDIDO,
                Predio = entity.NU_PREDIO,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                NuOrdenEntrega = entity.NU_ORDEN_ENTREGA,
                PedidoSincronizado = this.MapStringToBoolean(entity.PEDIDO_SINCRONIZADO),
                TipoExpedicion = entity.TP_EXPEDICION,
                TpExpManejaTracking = this.MapStringToBoolean(entity.TP_EXP_MANEJA_TRACKING),
                NuPrioridadCarga = entity.NU_PRIOR_CARGA,
            };
        }
        public virtual ContenedorEntrega MapContenedorEntregaCarga(V_CONTENEDORES_ENTREGA entity)
        {
            return new ContenedorEntrega
            {
                Contenedor = entity.NU_CONTENEDOR,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Pedido = entity.NU_PEDIDO,
                TipoContenedor = entity.TP_CONTENEDOR,
                Situacion = entity.CD_SITUACAO,
                Camion = entity.CD_CAMION,
                TpObjetoTracking = entity.TP_OBJETO_TRACKING,
                IdExterno = entity.ID_EXTERNO_CONTENEDOR,
                CantidadBulto = entity.QT_PRODUTO,
                Volumen = entity.VL_CUBAGEM,
                PesoTotal = entity.PS_BRUTO_TOTAL,
                Alto = entity.VL_ALTURA,
                Largo = entity.VL_LARGURA,
                Produndidad = entity.VL_PROFUNDIDADE,
                IdExternoTracking = entity.ID_EXTERNO_TRACKING,
                Descripcion = entity.DS_CONTENEDOR,
                CodigoBarras = entity.CD_BARRAS
            };
        }
        public virtual ContenedorEntrega MapContenedorEntregaExpCarga(V_CONTENEDORES_ENTREGA_EXP entity)
        {
            return new ContenedorEntrega
            {
                Contenedor = entity.NU_CONTENEDOR,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Pedido = entity.NU_PEDIDO,
                TipoContenedor = entity.TP_CONTENEDOR,
                Situacion = entity.CD_SITUACAO,
                Camion = entity.CD_CAMION,
                TpObjetoTracking = entity.TP_OBJETO_TRACKING,
                IdExterno = entity.ID_EXTERNO_CONTENEDOR,
                CantidadBulto = entity.QT_PRODUTO,
                Volumen = entity.VL_CUBAGEM,
                PesoTotal = entity.PS_BRUTO_TOTAL,
                Alto = entity.VL_ALTURA,
                Largo = entity.VL_LARGURA,
                Produndidad = entity.VL_PROFUNDIDADE,
                IdExternoTracking = entity.ID_EXTERNO_TRACKING,
                Descripcion = entity.DS_CONTENEDOR,
                CodigoBarras = entity.CD_BARRAS,
            };
        }
        public virtual PedidoPlanificadoCamion MapToObject(V_PEDIDOS_PLANIFICADOS_CAMION entity)
        {
            return new PedidoPlanificadoCamion
            {
                Camion = entity.CD_CAMION,
                CodigoCliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Pedido = entity.NU_PEDIDO,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                NuOrdenEntrega = entity.NU_ORDEN_ENTREGA,
                Predio = entity.NU_PREDIO,
                Sincronizado = entity.PEDIDO_SINCRONIZADO,
                TipoExpedicion = entity.TP_EXPEDICION,
                TpExpManejaTracking = entity.TP_EXP_MANEJA_TRACKING,
                NuOrdenCarga = entity.NU_PRIOR_CARGA,
                CodigoAgente = entity.CD_AGENTE,
                TipoAgente = entity.TP_AGENTE,
            };
        }
        public virtual PlanificacionCamion MapPlanificacion(V_PLANIFICACION_CAMION entity)
        {
            return new PlanificacionCamion
            {
                Camion = entity.CD_CAMION,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                ComparteContenedorEntrega = entity.VL_COMPARTE_CONTENEDOR_ENTREGA,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Numero = entity.NU_CONTENEDOR,
                CodigoAgente = entity.CD_AGENTE,
                TipoAgente = entity.TP_AGENTE,
                DescContenedor = entity.DS_CONTENEDOR,
                TipoBulto = entity.TP_BULTO,
                CantidadBulto = entity.QT_PRODUTO,
                VolumenTotal = entity.VL_CUBAGEM_TOTAL,
                PesoTotal = entity.VL_PESO_TOTAL,
                Alto = entity.VL_ALTURA,
                Largo = entity.VL_LARGURA,
                Produndidad = entity.VL_PROFUNDIDADE,
                OrdenDeCarga = entity.NU_PRIOR_CARGA,
                TipoContenedor = entity.TP_CONTENEDOR,
                IdExterno = entity.ID_EXTERNO_CONTENEDOR,
                IdExternoTracking = entity.ID_EXTERNO_TRACKING,
                CodigoBarras = entity.CD_BARRAS
            };
        }
        #endregion
    }
}
