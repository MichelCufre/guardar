using System;
using WIS.Domain.Liberacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class LiberacionMapper : Mapper
    {

        public LiberacionMapper()
        {
        }

        #region >> TO
        // Entity => Object
        public virtual PedidoPendLib MapPedidoPendLibToObject(V_PRE050_PEND_LIB e, bool addNavegables = true)
        {
            var obj = new PedidoPendLib
            {
                cdCliente = e.CD_CLIENTE,
                auxHrEntrega = e.AUX_HR_ENTREGA,
                auxNuOrden = e.AUX_NU_ORDEN,
                auxNuOrdenLiberacion = e.AUX_NU_ORDEN_LIBERACION,
                auxVlVolumen = e.AUX_VL_VOLUMEN,
                cdCondicionLiberacion = e.CD_CONDICION_LIBERACION,
                cdEmpresa = e.CD_EMPRESA,
                cdGrupoExpedicion = e.CD_GRUPO_EXPEDICION,
                cdRuta = e.CD_ROTA,
                cdTransportadora = e.CD_TRANSPORTADORA,
                cdUf = e.CD_UF,
                cdZona = e.CD_ZONA,
                cdOnda = e.CD_ONDA,
                dsAnexo4 = e.DS_ANEXO4,
                dsAnexo1 = e.DS_ANEXO1,
                dsCliente = e.DS_CLIENTE,
                dsEndereco = e.DS_ENDERECO,
                dsRuta = e.DS_ENDERECO,
                dsTransportadora = e.DS_TRANSPORTADORA,
                dsUf = e.DS_UF,
                dtEmitido = e.DT_EMITIDO,
                dtentrega = e.DT_ENTREGA,
                dtliberarDesde = e.DT_LIBERAR_DESDE,
                dtLiberarhasta = e.DT_LIBERAR_HASTA,
                nuOrdenLiberacion = e.NU_ORDEN_LIBERACION,
                nuPedido = e.NU_PEDIDO,
                nuPredio = e.NU_PREDIO,
                nuUltimaPreparacion = e.NU_ULT_PREPARACION,
                qtLiberado = e.QT_LIBERADO,
                qtlineas = e.QT_LINEAS,
                qtProductoSinPeso = e.QT_PRODUCTOS_SIN_PESO_BRUTO,
                qtProductoSinVolumen = e.QT_PRODUCTOS_SIN_VOLUMEN,
                qtUnidades = e.QT_UNIDADES,
                tpExpedicion = e.TP_EXPEDICION,
                tpPedido = e.TP_PEDIDO,
                vlComparteContenedorPicking = e.VL_COMPARTE_CONTENEDOR_PICKING,
                vlPesoTotal = e.VL_PESO_TOTAL_BRUTO,
                vlVolumenTotal = e.VL_VOLUMEN_TOTAL
            };
            return obj;
        }

        public virtual ReglaLiberacion MapReglaLiberacionToObject(T_REGLA_LIBERACION e, bool addNavegables = true)
        {
            var obj = new ReglaLiberacion
            {
                CdAgrupacion = e.CD_AGRUPACION,
                CdAgruparPorCamion = e.CD_AGRUPAR_POR_CAMION,
                CdControlaStock = e.CD_CONTROLA_STOCK,
                CdLiberarPorCurvas = e.CD_LIBERAR_POR_CURVAS,
                CdLiberarPorUnidad = e.CD_LIBERAR_POR_UNIDAD,
                CdOrdenPedidos = e.CD_ORDEN_PEDIDOS,
                CdPalletCompeto = e.CD_PALLET_COMPLETO,
                CdpalletIncompleto = e.CD_PALLET_INCOMPLETO,
                CdPrepararSoloCamion = e.CD_PREPARAR_SOLO_CAMION,
                CdRepartirEscasez = e.CD_REPARTIR_ESCASEZ,
                CdRespetarFifo = e.CD_RESPETAR_FIFO,
                CdStock = e.CD_STOCK,
                DsDias = e.DS_DIAS,
                DsRegla = e.DS_REGLA,
                DtAddRow = e.DT_ADDROW,
                DtFin = e.DT_FIN,
                DtInicio = e.DT_INICIO,
                DtUpdRow = e.DT_UPDROW,
                FlActiva = this.MapStringToBoolean(e.FL_ACTIVE),
                HrInicio = null,
                HrFin = null,
                NuOrden = e.NU_ORDEN,
                NuFrecuencia = e.NU_FRECUENCIA,
                NuRegla = e.NU_REGLA,
                TpFrecuencia = e.TP_FRECUENCIA,
                CdEmpresa = e.CD_EMPRESA,
                CdOnda = e.CD_ONDA,
                TpAgente = e.TP_AGENTE,
                NuPredio = e.NU_PREDIO,
                CdOrdenPedidosAuto = e.CD_ORDEN_PEDIDOS_AUTO,
                NuClisPorPreparacion = e.NU_CLIS_POR_PREPARACION,
                DtUltimaEjecucion = e.DT_ULTIMA_EJECUCION,
                FlSoloPedidosNuevos = this.MapStringToBoolean(e.FL_SOLO_PEDIDOS_NUEVOS),
                ManejaVidaUtil = this.MapStringToBoolean(e.FL_VENTANA_POR_CLIENTE),
                ValorVidaUtil = e.VL_PORCENTAJE_VENTANA,
                PriozarDesborde = this.MapStringToBoolean(e.FL_PRIORIZAR_DESBORDE),
                RespetarIntervalo = this.MapStringToBoolean(e.FL_RESPETAR_INTERVALO),
                ExcluirUbicacionesPicking = this.MapStringToBoolean(e.FL_EXCLUIR_UBICACIONES_PICKING),
                UsarSoloStkPicking = this.MapStringToBoolean(e.FL_USAR_SOLO_STK_PICKING),
                NuDiasColaTrabajo = e.NU_DIAS_COLA_TRABAJO,
            };

            if (e.HR_INICIO != null)
                obj.HrInicio = TimeSpan.FromMilliseconds(e.HR_INICIO.Value);

            if (e.HR_FIN != null)
                obj.HrFin = TimeSpan.FromMilliseconds(e.HR_FIN.Value);

            if (addNavegables)
            {
                foreach (var i in e.T_REGLA_TIPO_PEDIDO)
                    obj.LstReglaTipoPedido.Add(MapReglaTipoPedidoToObject(i, false));
                foreach (var i in e.T_REGLA_TIPO_EXPEDICION)
                    obj.LstReglaTipoExpedicion.Add(MapReglaTipoExpedicionToObject(i, false));
                foreach (var i in e.T_REGLA_CONDICION_LIBERACION)
                    obj.LstReglaCondicionLiberacion.Add(MapReglaCondicionLiberacionToObject(i, false));
                foreach (var i in e.T_REGLA_CLIENTES)
                    obj.LstReglaCliente.Add(MapReglaClienteToObject(i, false));
            }
            return obj;
        }

        public virtual ReglaTipoExpedicion MapReglaTipoExpedicionToObject(T_REGLA_TIPO_EXPEDICION e, bool addNavegables = true)
        {
            var obj = new ReglaTipoExpedicion
            {
                dtAddRow = e.DT_ADDROW,
                nuRegla = e.NU_REGLA,
                dtUpdRow = e.DT_UPDROW,
                tpExpedicion = e.TP_EXPEDICION
            };
            if (addNavegables)
            {
                obj.ReglaLiberacion = MapReglaLiberacionToObject(e.T_REGLA_LIBERACION);
            }
            return obj;
        }

        public virtual ReglaTipoPedido MapReglaTipoPedidoToObject(T_REGLA_TIPO_PEDIDO e, bool addNavegables = true)
        {
            var obj = new ReglaTipoPedido
            {
                dtAddRow = e.DT_ADDROW,
                nuRegla = e.NU_REGLA,
                dtUpdRow = e.DT_UPDROW,
                tpPedido = e.TP_PEDIDO
            };
            if (addNavegables)
            {
                obj.ReglaLiberacion = MapReglaLiberacionToObject(e.T_REGLA_LIBERACION);
            }
            return obj;
        }

        public virtual ReglaCondicionLiberacion MapReglaCondicionLiberacionToObject(T_REGLA_CONDICION_LIBERACION e, bool addNavegables = true)
        {
            var obj = new ReglaCondicionLiberacion
            {
                dtAddRow = e.DT_ADDROW,
                nuRegla = e.NU_REGLA,
                dtUpdRow = e.DT_UPDROW,
                cdCondicionLiberacion = e.CD_CONDICION_LIBERACION
            };
            if (addNavegables)
            {
                obj.ReglaLiberacion = MapReglaLiberacionToObject(e.T_REGLA_LIBERACION);
            }
            return obj;
        }

        public virtual Onda MapOndaToObject(V_ONDA e, bool addNavegables = true)
        {
            var obj = new Onda
            {
                Id = e.CD_ONDA,
                Estado = e.CD_SITUACAO,
                Descripcion = e.DS_ONDA
            };
            return obj;
        }

        public virtual ReglaCliente MapReglaClienteToObject(T_REGLA_CLIENTES e, bool addNavegables = true)
        {
            if (e == null)
                return null;
            var obj = new ReglaCliente
            {
                Cliente = e.CD_CLIENTE,
                Empesa = e.CD_EMPRESA,
                FechaAlta = e.DT_ADDROW,
                FechaModificacion = e.DT_UPDROW,
                NuOrden = e.NU_ORDEN,
                NuRegla = e.NU_REGLA,
            };
            return obj;
        }

        public virtual CondicionLiberacion MapCondicionLiberacionToObject(T_CONDICION_LIBERACION condicion)
        {
            return new CondicionLiberacion
            {
                Condicion = condicion.CD_CONDICION_LIBERACION,
                Descripcion = condicion.DS_CONDICION_LIBERACION,
                MostrarMarcada = condicion.VL_EXPRESION_MOSTRAR_MARCADA == "TRUE" ? true : false
            };
        }


        #endregion << TO

        #region >> FROM 

        public virtual T_REGLA_CONDICION_LIBERACION MapReglaCondicionLiberacionToEntity(ReglaCondicionLiberacion obj, bool addNavegables = true)
        {
            var entity = new T_REGLA_CONDICION_LIBERACION
            {
                DT_ADDROW = obj.dtAddRow,
                DT_UPDROW = obj.dtUpdRow,
                NU_REGLA = obj.nuRegla,
                CD_CONDICION_LIBERACION = obj.cdCondicionLiberacion
            };
            return entity;
        }
        public virtual T_REGLA_LIBERACION MapReglaLiberacionToEntity(ReglaLiberacion obj, bool addNavegables = true)
        {
            var entity = new T_REGLA_LIBERACION
            {
                CD_AGRUPACION = obj.CdAgrupacion,
                CD_AGRUPAR_POR_CAMION = obj.CdAgruparPorCamion,
                CD_CONTROLA_STOCK = obj.CdControlaStock,
                CD_LIBERAR_POR_CURVAS = obj.CdLiberarPorCurvas,
                CD_LIBERAR_POR_UNIDAD = obj.CdLiberarPorUnidad,
                CD_ORDEN_PEDIDOS = obj.CdOrdenPedidos,
                CD_PALLET_COMPLETO = obj.CdPalletCompeto,
                CD_PALLET_INCOMPLETO = obj.CdpalletIncompleto,
                CD_PREPARAR_SOLO_CAMION = obj.CdPrepararSoloCamion,
                CD_REPARTIR_ESCASEZ = obj.CdRepartirEscasez,
                CD_RESPETAR_FIFO = obj.CdRespetarFifo,
                CD_STOCK = obj.CdStock,
                DS_DIAS = obj.DsDias,
                DS_REGLA = obj.DsRegla,
                DT_ADDROW = obj.DtAddRow,
                DT_FIN = obj.DtFin,
                DT_INICIO = obj.DtInicio,
                DT_UPDROW = obj.DtUpdRow,
                FL_ACTIVE = this.MapBooleanToString(obj.FlActiva),
                HR_INICIO = null,
                HR_FIN = null,
                NU_ORDEN = obj.NuOrden,
                NU_FRECUENCIA = obj.NuFrecuencia,
                NU_REGLA = obj.NuRegla,
                TP_FRECUENCIA = obj.TpFrecuencia,
                CD_EMPRESA = obj.CdEmpresa,
                TP_AGENTE = obj.TpAgente,
                NU_PREDIO = obj.NuPredio,
                CD_ONDA = obj.CdOnda,
                CD_ORDEN_PEDIDOS_AUTO = obj.CdOrdenPedidosAuto,
                DT_ULTIMA_EJECUCION = obj.DtUltimaEjecucion,
                NU_CLIS_POR_PREPARACION = obj.NuClisPorPreparacion,
                FL_SOLO_PEDIDOS_NUEVOS = this.MapBooleanToString(obj.FlSoloPedidosNuevos),
                VL_PORCENTAJE_VENTANA = obj.ValorVidaUtil,
                FL_VENTANA_POR_CLIENTE = this.MapBooleanToString(obj.ManejaVidaUtil),
                FL_PRIORIZAR_DESBORDE = this.MapBooleanToString(obj.PriozarDesborde),
                FL_RESPETAR_INTERVALO = this.MapBooleanToString(obj.RespetarIntervalo),
                FL_EXCLUIR_UBICACIONES_PICKING = this.MapBooleanToString(obj.ExcluirUbicacionesPicking),
                FL_USAR_SOLO_STK_PICKING = this.MapBooleanToString(obj.UsarSoloStkPicking),
                NU_DIAS_COLA_TRABAJO = obj.NuDiasColaTrabajo
            };

            if (obj.HrInicio != null)
                entity.HR_INICIO = (long)obj.HrInicio?.TotalMilliseconds;

            if (obj.HrFin != null)
                entity.HR_FIN = (long)obj.HrFin?.TotalMilliseconds;


            return entity;
        }

        public virtual T_REGLA_CLIENTES MapReglaClienteToEntity(ReglaCliente obj)
        {
            var entity = new T_REGLA_CLIENTES
            {
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empesa,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_ORDEN = obj.NuOrden,
                NU_REGLA = obj.NuRegla,
            };

            return entity;
        }

        public virtual T_REGLA_TIPO_EXPEDICION MapReglaTipoExpedicionToEntity(ReglaTipoExpedicion obj, bool addNavegables = true)
        {
            var entity = new T_REGLA_TIPO_EXPEDICION
            {
                DT_ADDROW = obj.dtAddRow,
                DT_UPDROW = obj.dtUpdRow,
                NU_REGLA = obj.nuRegla,
                TP_EXPEDICION = obj.tpExpedicion
            };
            return entity;
        }

        public virtual T_REGLA_TIPO_PEDIDO MapReglaTipoPedidoToEntity(ReglaTipoPedido obj, bool addNavegables = true)
        {
            var entity = new T_REGLA_TIPO_PEDIDO
            {
                DT_ADDROW = obj.dtAddRow,
                DT_UPDROW = obj.dtUpdRow,
                NU_REGLA = obj.nuRegla,
                TP_PEDIDO = obj.tpPedido
            };
            return entity;
        }
 
        public virtual T_CONDICION_LIBERACION MapCondicionLiberacionToEntity(CondicionLiberacion condicion)
        {
            return new T_CONDICION_LIBERACION
            {
                CD_CONDICION_LIBERACION = condicion.Condicion,
                DS_CONDICION_LIBERACION = condicion.Descripcion,
                VL_EXPRESION_MOSTRAR_MARCADA = condicion.MostrarMarcada ? "TRUE" : "FALSE"
            };
        }
        #endregion << FROM 


    }
}
