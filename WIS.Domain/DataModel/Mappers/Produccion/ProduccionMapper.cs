using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.Produccion.Interfaces.Entrada;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Produccion
{
    public class ProduccionMapper : Mapper
    {
        protected readonly IngresoFactory _factory;
        protected readonly LineaMapper _mapperLinea;
        protected readonly FormulaMapper _mapperFormula;

        public LineaMapper LineaMapper
        {
            get { return this._mapperLinea; }
        }

        public FormulaMapper FormulaMapper
        {
            get { return this._mapperFormula; }
        }

        public ProduccionMapper()
        {
            this._factory = new IngresoFactory();
            this._mapperLinea = new LineaMapper();
            this._mapperFormula = new FormulaMapper(new FormulaAccionMapper());
        }

        public ProduccionMapper(LineaMapper mapperLinea, FormulaMapper mapperFormula)
        {
            this._factory = new IngresoFactory();
            this._mapperLinea = mapperLinea;
            this._mapperFormula = mapperFormula;
        }

        public virtual IIngreso MapIngresoEntityToObject(T_PRDC_INGRESO entity, List<T_PRDC_LINEA_CONSUMIDO> consumidos, List<T_PRDC_LINEA_PRODUCIDO> producidos)
        {
            var ingreso = this._factory.CreateIngreso(this.MapStringToTipoIngreso(entity.ND_TIPO));

            this.SetPropertiesIngreso(ingreso, entity);

            return ingreso;
        }

        public virtual IIngresoPanel MapIngresoPanelEntityToObject(T_PRDC_INGRESO entity, T_PRDC_LINEA lineaEntity, List<T_PRDC_LINEA_CONSUMIDO> consumidos, List<T_PRDC_LINEA_PRODUCIDO> producidos, List<T_PRDC_INGRESO_PASADA> pasadas)
        {
            if (entity == null)
                return null;

            IIngresoPanel ingreso = this._factory.CreateIngresoPanel(this.MapStringToTipoIngreso(entity.ND_TIPO));

            this.SetPropertiesIngresoPanel(ingreso, entity, lineaEntity);

            foreach (var consumido in consumidos)
            {
                //ingreso.Consumidos.Add(this.MapLineaConsumidaEntityToObject(consumido));
            }

            foreach (var producido in producidos)
            {
                //ingreso.Producidos.Add(this.MapLineaProducidaEntityToObject(producido));
            }

            foreach (var pasada in pasadas)
            {
                ingreso.Pasadas.Add(this.MapPasadaEntityToObject(ingreso, pasada));
            }

            return ingreso;
        }

        public virtual IIngresoColector MapIngresoColectorEntityToObject(T_PRDC_INGRESO entity, List<T_PRDC_LINEA_CONSUMIDO> consumidos, List<T_PRDC_LINEA_PRODUCIDO> producidos)
        {
            if (entity == null)
                return null;

            IIngresoColector ingreso = this._factory.CreateIngresoColector(this.MapStringToTipoIngreso(entity.ND_TIPO));

            this.SetPropertiesIngreso(ingreso, entity);

            foreach (var consumido in consumidos)
            {
                //ingreso.Consumidos.Add(this.MapLineaConsumidaEntityToObject(consumido));
            }

            foreach (var producido in producidos)
            {
                //ingreso.Producidos.Add(this.MapLineaProducidaEntityToObject(producido));
            }

            return ingreso;
        }

        public virtual T_PRDC_INGRESO MapObjectToIngreso(IIngreso ingreso)
        {
            return new T_PRDC_INGRESO()
            {
                NU_PRDC_INGRESO = ingreso.Id,
                CD_FUNCIONARIO = ingreso.Funcionario,
                DT_ADDROW = ingreso.FechaAlta,
                DT_UPDROW = ingreso.FechaActualizacion,
                DS_ANEXO3 = ingreso.Anexo3,
                DS_ANEXO2 = ingreso.Anexo2,
                DS_ANEXO1 = ingreso.Anexo1,
                DS_ANEXO4 = ingreso.Anexo4,
                CD_PRDC_DEFINICION = ingreso.Formula.Id,
                CD_SITUACAO = ingreso.Situacion,
                ID_GENERAR_PEDIDO = this.MapBooleanToString(ingreso.GeneraPedido),
                ND_TIPO = this.MapTipoIngresoToString(ingreso.TipoProduccion),
                NU_INTERFAZ_EJECUCION_ENTRADA = ingreso.EjecucionEntrada,
                NU_PRDC_ORIGINAL = ingreso.NumeroProduccionOriginal,
                QT_FORMULA = ingreso.CantidadIteracionesFormula,
                NU_PREDIO = ingreso.Predio,
                DT_INICIO_PRODUCCION = ingreso.FechaInicioProduccion,
                DT_FIN_PRODUCCION = ingreso.FechaFinProduccion,
                ID_MANUAL = ingreso.IdManual,
                NU_ULT_INTERFAZ_EJECUCION = ingreso.NroUltInterfazEjecucion,
            };
        }

        public virtual IngresoWhiteBox MapIngresoWhiteBoxEntityToObject(T_PRDC_INGRESO entity, T_PRDC_LINEA lineaEntity, List<T_PRDC_LINEA_CONSUMIDO> consumidos, List<T_PRDC_LINEA_PRODUCIDO> producidos, List<T_PRDC_INGRESO_PASADA> pasadas, T_PRDC_INGRESO_DOCUMENTO documento)
        {
            if (entity == null)
                return null;

            TipoProduccionIngreso tipo = this.MapStringToTipoIngreso(entity.ND_TIPO);
            IngresoWhiteBox ingreso = new IngresoWhiteBox();

            var lineasConsumidas = new List<LineaConsumida>();
            var lineasProducidas = new List<LineaProducida>();
            var ingresoPasadas = new List<Pasada>();

            this.SetPropertiesIngreso(ingreso, entity);

            foreach (var consumido in consumidos)
            {
                lineasConsumidas.Add(this.MapLineaConsumidaEntityToObject(consumido));
            }

            foreach (var producido in producidos)
            {
                lineasProducidas.Add(this.MapLineaProducidaEntityToObject(producido));
            }

            foreach (var pasada in pasadas)
            {
                ingresoPasadas.Add(this.MapPasadaEntityToObject(ingreso, pasada));
            }

            //this.SetPropertiesIngresoWhiteBox(tipo, entity, ingreso, lineasConsumidas, lineasProducidas, ingresoPasadas, lineaEntity, documento);

            return ingreso;
        }

        public virtual T_PRDC_INGRESO MapObjectToIngresoPanel(IIngresoPanel ingreso)
        {
            return new T_PRDC_INGRESO()
            {
                NU_PRDC_INGRESO = ingreso.Id,
                CD_FUNCIONARIO = ingreso.Funcionario,
                DT_ADDROW = ingreso.FechaAlta,
                DT_UPDROW = ingreso.FechaActualizacion,
                DS_ANEXO3 = ingreso.Anexo3,
                DS_ANEXO2 = ingreso.Anexo2,
                DS_ANEXO1 = ingreso.Anexo1,
                DS_ANEXO4 = ingreso.Anexo4,
                CD_PRDC_DEFINICION = ingreso.Formula.Id,
                CD_PRDC_LINEA = ingreso.Linea != null ? ingreso.Linea.Id : null,
                CD_SITUACAO = ingreso.Situacion,
                ID_GENERAR_PEDIDO = this.MapBooleanToString(ingreso.GeneraPedido),
                ND_TIPO = this.MapTipoIngresoToString(ingreso.TipoProduccion),
                NU_INTERFAZ_EJECUCION_ENTRADA = ingreso.EjecucionEntrada,
                NU_PRDC_ORIGINAL = ingreso.NumeroProduccionOriginal,
                QT_FORMULA = ingreso.CantidadIteracionesFormula,
                NU_PREDIO = ingreso.Predio,
                DT_INICIO_PRODUCCION = ingreso.FechaInicioProduccion,
                DT_FIN_PRODUCCION = ingreso.FechaFinProduccion,
                ID_MANUAL = ingreso.IdManual,
                NU_ULT_INTERFAZ_EJECUCION = ingreso.NroUltInterfazEjecucion,
            };
        }

        public virtual IngresoBlackBox MapIngresoBlackBoxEntityToObject(T_PRDC_INGRESO entity, T_PRDC_LINEA lineaEntity, List<T_PRDC_LINEA_CONSUMIDO> consumidos, List<T_PRDC_LINEA_PRODUCIDO> producidos, List<T_PRDC_INGRESO_PASADA> pasadas)
        {
            if (entity == null)
                return null;

            TipoProduccionIngreso tipo = this.MapStringToTipoIngreso(entity.ND_TIPO);
            IngresoBlackBox ingreso = new IngresoBlackBox();

            var lineasConsumidas = new List<LineaConsumida>();
            var lineasProducidas = new List<LineaProducida>();
            var ingresoPasadas = new List<Pasada>();

            this.SetPropertiesIngreso(ingreso, entity);

            foreach (var consumido in consumidos)
            {
                lineasConsumidas.Add(MapLineaConsumidaEntityToObject(consumido));
            }

            foreach (var producido in producidos)
            {
                lineasProducidas.Add(MapLineaProducidaEntityToObject(producido));
            }

            foreach (var pasada in pasadas)
            {
                ingresoPasadas.Add(this.MapPasadaEntityToObject(ingreso, pasada));
            }

            this.SetPropertiesIngresoBlackBox(tipo, entity, ingreso, lineasConsumidas, lineasProducidas, ingresoPasadas, lineaEntity);

            return ingreso;
        }

        public virtual void SetPropertiesIngreso(IIngreso ingreso, T_PRDC_INGRESO entity)
        {
            ingreso.Id = entity.NU_PRDC_INGRESO;
            ingreso.Empresa = entity.CD_EMPRESA;
            ingreso.TipoProduccion = this.MapStringToTipoIngreso(entity.ND_TIPO);
            ingreso.Situacion = entity.CD_SITUACAO;
            ingreso.GeneraPedido = this.MapStringToBoolean(entity.ID_GENERAR_PEDIDO);
            ingreso.CantidadIteracionesFormula = entity.QT_FORMULA ?? 0;
            //ingreso.Formula = this._mapperFormula.MapEntityToObject(entity.T_PRDC_DEFINICION);
            ingreso.Anexo1 = entity.DS_ANEXO1;
            ingreso.Anexo2 = entity.DS_ANEXO2;
            ingreso.Anexo3 = entity.DS_ANEXO3;
            ingreso.Anexo4 = entity.DS_ANEXO4;
            ingreso.EjecucionEntrada = entity.NU_INTERFAZ_EJECUCION_ENTRADA;
            ingreso.FechaActualizacion = entity.DT_UPDROW;
            ingreso.FechaAlta = entity.DT_ADDROW;
            ingreso.Funcionario = entity.CD_FUNCIONARIO;
            ingreso.NumeroProduccionOriginal = entity.NU_PRDC_ORIGINAL;
            ingreso.Predio = entity.NU_PREDIO;
            ingreso.FechaInicioProduccion = entity.DT_INICIO_PRODUCCION;
            ingreso.FechaFinProduccion = entity.DT_FIN_PRODUCCION;
            ingreso.IdManual = entity.ID_MANUAL;
            ingreso.NroUltInterfazEjecucion = entity.NU_ULT_INTERFAZ_EJECUCION;
        }

        public virtual void SetPropertiesIngresoPanel(IIngresoPanel ingreso, T_PRDC_INGRESO entity, T_PRDC_LINEA lineaEntity)
        {
            this.SetPropertiesIngreso(ingreso, entity);
            ingreso.Linea = this._mapperLinea.MapLineaEntityToObject(lineaEntity);
        }

        public virtual void SetPropertiesIngresoWhiteBox(TipoProduccionIngreso tipo, T_PRDC_INGRESO entity, IngresoWhiteBox ingreso, List<LineaConsumida> lineasConsumidas, List<LineaProducida> lineasProducidas, List<Pasada> pasadas, T_PRDC_LINEA lineaEntity, T_PRDC_INGRESO_DOCUMENTO documento)
        {
            ingreso.Linea = this._mapperLinea.MapLineaWhiteBoxEntityToObject(lineaEntity);

            if (documento != null)
            {
                ingreso.Documento = new Domain.Produccion.Documento()
                {
                    NumeroEgreso = documento.NU_DOCUMENTO_EGR,
                    TipoEgreso = documento.TP_DOCUMENTO_EGR,
                    NumeroIngreso = documento.NU_DOCUMENTO_ING,
                    TipoIngreso = documento.TP_DOCUMENTO_ING
                };
            }

            //ingreso.Consumidos = lineasConsumidas;
            //ingreso.Producidos = lineasProducidas;
            ingreso.Pasadas = pasadas;
        }

        public virtual void SetPropertiesIngresoBlackBox(TipoProduccionIngreso tipo, T_PRDC_INGRESO entity, IngresoBlackBox ingreso, List<LineaConsumida> lineasConsumidas, List<LineaProducida> lineasProducidas, List<Pasada> pasadas, T_PRDC_LINEA lineaEntity)
        {
            this.SetPropertiesIngreso(ingreso, entity);
            ingreso.Linea = this._mapperLinea.MapLineaBlackBoxEntityToObject(lineaEntity);
        }

        public virtual LineaConsumida MapLineaConsumidaEntityToObject(T_PRDC_LINEA_CONSUMIDO consumido)
        {
            return new LineaConsumida
            {
                Producto = consumido.CD_PRODUTO,
                Empresa = consumido.CD_EMPRESA,
                Faixa = consumido.CD_FAIXA,
                Iteracion = consumido.NU_FORMULA,
                Pasada = consumido.NU_PASADA,
                Identificador = consumido.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Cantidad = consumido.QT_CONSUMIDO ?? 0,
                FechaAlta = consumido.DT_ADDROW,
                NumeroTransaccion = consumido.NU_TRANSACCION
            };
        }

        public virtual LineaProducida MapLineaProducidaEntityToObject(T_PRDC_LINEA_PRODUCIDO producido)
        {
            return new LineaProducida
            {
                Producto = producido.CD_PRODUTO,
                Empresa = producido.CD_EMPRESA,
                Faixa = producido.CD_FAIXA,
                Iteracion = producido.NU_FORMULA,
                Pasada = producido.NU_PASADA,
                Identificador = producido.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Cantidad = producido.QT_PRODUCIDO ?? 0,
                Vencimiento = producido.DT_VENCIMIENTO,
                FechaAlta = producido.DT_ADDROW,
                NumeroTransaccion = producido.NU_TRANSACCION
            };
        }

        public virtual Pasada MapPasadaEntityToObject(IIngreso ingreso, T_PRDC_INGRESO_PASADA entity)
        {
            return new Pasada(ingreso)
            {
                NuIngreso = entity.NU_PRDC_INGRESO,
                Linea = entity.CD_PRDC_LINEA,
                Accion = entity.CD_ACCION_INSTANCIA == null ? null : ingreso.Formula.Configuracion.Where(d => d.Accion.Id == entity.CD_ACCION_INSTANCIA).FirstOrDefault()?.Accion,
                Numero = entity.QT_PASADAS,
                FechaAlta = entity.DT_ADDROW,
                NumeroFormula = entity.NU_FORMULA_ENSAMBLADA ?? 0,
                Orden = entity.NU_ORDEN,
                Valor = entity.VL_ACCION_INSTANCIA,
            };
        }

        public virtual T_PRDC_INGRESO_PASADA MapPasadaObjectToEntity(Pasada pasada)
        {
            return new T_PRDC_INGRESO_PASADA
            {
                NU_PRDC_INGRESO = pasada.NuIngreso,
                CD_PRDC_LINEA = pasada.Linea,
                CD_ACCION_INSTANCIA = pasada.Accion?.Id,
                QT_PASADAS = pasada.Numero,
                DT_ADDROW = pasada.FechaAlta,
                NU_FORMULA_ENSAMBLADA = pasada.NumeroFormula,
                NU_ORDEN = pasada.Orden,
                VL_ACCION_INSTANCIA = pasada.Valor
            };
        }

        public virtual T_PRDC_LINEA_CONSUMIDO MapObjectToLineaConsumidaEntity(IIngreso ingreso, LineaConsumida consumido)
        {
            return new T_PRDC_LINEA_CONSUMIDO
            {
                NU_PRDC_INGRESO = ingreso.Id,
                CD_PRDC_DEFINICION = ingreso.Formula.Id,
                CD_PRODUTO = consumido.Producto,
                CD_EMPRESA = consumido.Empresa,
                CD_FAIXA = consumido.Faixa,
                NU_FORMULA = consumido.Iteracion,
                NU_PASADA = consumido.Pasada,
                NU_IDENTIFICADOR = consumido.Identificador?.Trim()?.ToUpper(),
                QT_CONSUMIDO = consumido.Cantidad,
                NU_TRANSACCION = consumido.NumeroTransaccion
            };
        }

        public virtual T_PRDC_LINEA_PRODUCIDO MapObjectToLineaProducidaEntity(IIngreso ingreso, LineaProducida producido)
        {
            int empresaInt = (int)producido.Empresa;

            return new T_PRDC_LINEA_PRODUCIDO
            {
                NU_PRDC_INGRESO = ingreso.Id,
                CD_PRDC_DEFINICION = ingreso.Formula.Id,
                CD_PRODUTO = producido.Producto,
                CD_EMPRESA = empresaInt,
                CD_FAIXA = producido.Faixa,
                NU_FORMULA = producido.Iteracion,
                NU_PASADA = producido.Pasada,
                NU_IDENTIFICADOR = producido.Identificador?.Trim()?.ToUpper(),
                DT_VENCIMIENTO = producido.Vencimiento,
                QT_PRODUCIDO = producido.Cantidad,
                NU_TRANSACCION = producido.NumeroTransaccion
            };
        }

        public virtual T_HIST_PRDC_LINEA_CONSUMIDO MapObjectToLineaConsumidaHistoricoEntity(IIngreso ingreso, LineaConsumidaHistorica consumido)
        {
            int empresaInt = (int)ingreso.Empresa;

            return new T_HIST_PRDC_LINEA_CONSUMIDO
            {

                NU_PRDC_INGRESO = ingreso.Id,
                CD_PRDC_DEFINICION = ingreso.Formula.Id,
                CD_PRODUTO = consumido.Producto,
                CD_EMPRESA = empresaInt,
                CD_FAIXA = consumido.Faixa,
                NU_FORMULA = consumido.Iteracion,
                NU_PASADA = consumido.Pasada,
                NU_IDENTIFICADOR = consumido.Identificador?.Trim()?.ToUpper(),
                QT_CONSUMIDO = consumido.Cantidad,
                DT_ADDHIST = consumido.FechaConsumo
            };
        }

        public virtual T_HIST_PRDC_LINEA_PRODUCIDO MapObjectToLineaProducidaHistoricoEntity(IIngreso ingreso, LineaProducidaHistorica producido)
        {
            int empresaInt = (int)ingreso.Empresa;


            return new T_HIST_PRDC_LINEA_PRODUCIDO
            {
                NU_PRDC_INGRESO = ingreso.Id,
                CD_PRDC_DEFINICION = ingreso.Formula.Id,
                CD_PRODUTO = producido.Producto,
                CD_EMPRESA = empresaInt,
                CD_FAIXA = producido.Faixa,
                NU_FORMULA = producido.Iteracion,
                NU_PASADA = producido.Pasada,
                NU_IDENTIFICADOR = producido.Identificador?.Trim()?.ToUpper(),
                DT_VENCIMIENTO = producido.FechaVencimiento,
                QT_PRODUCIDO = producido.Cantidad,
                DT_ADDHIST = producido.FechaProducido
            };
        }

        public virtual T_HIST_PRDC_INGRESO_PASADA MapObjectToPasdaHistoricoEntity(PasadaHistorica pasada)
        {
            return new T_HIST_PRDC_INGRESO_PASADA()
            {
                CD_ACCION_INSTANCIA = pasada.AccionIntancia,
                CD_PRDC_LINEA = pasada.Linea,
                DT_ADDHIST = pasada.FechaHistorica,
                DT_ADDROW = pasada.FechaCreacion,
                NU_FORMULA_ENSAMBLADA = pasada.NumeroFormulaEnsamblada,
                NU_HISTORICO = pasada.NumeroHistorico,
                NU_ORDEN = pasada.Orden,
                NU_PRDC_INGRESO = pasada.Ingreso,
                QT_PASADAS = pasada.CantidadPasadas,
                VL_ACCION_INSTANCIA = pasada.ValorAccionInstancia
            };
        }

        public virtual T_PRDC_INGRESO_DOCUMENTO MapObjectDocumentoEntity(IIngreso ingreso, Domain.Produccion.Documento documento)
        {
            return new T_PRDC_INGRESO_DOCUMENTO()
            {
                NU_DOCUMENTO_EGR = documento.NumeroEgreso,
                TP_DOCUMENTO_EGR = documento.TipoEgreso,
                NU_DOCUMENTO_ING = documento.NumeroIngreso,
                TP_DOCUMENTO_ING = documento.TipoIngreso,
                NU_PRDC_INGRESO = ingreso.Id
            };
        }

        public virtual TipoProduccionIngreso MapStringToTipoIngreso(string tipo)
        {
            switch (tipo)
            {
                case TipoIngresoProduccion.PanelWeb:
                    return TipoProduccionIngreso.PanelWeb;
                case TipoIngresoProduccion.BlackBox:
                    return TipoProduccionIngreso.BlackBox;
                case TipoIngresoProduccion.Colector:
                    return TipoProduccionIngreso.Colector;
            }

            return TipoProduccionIngreso.Unknown;
        }

        public virtual string MapTipoIngresoToString(TipoProduccionIngreso tipo)
        {
            switch (tipo)
            {
                case TipoProduccionIngreso.BlackBox:
                    return TipoIngresoProduccion.BlackBox;
                case TipoProduccionIngreso.PanelWeb:
                    return TipoIngresoProduccion.PanelWeb;
                case TipoProduccionIngreso.Colector:
                    return TipoIngresoProduccion.Colector;
            }

            return "";
        }

        public virtual string GetDescripcionSituacion(short? situacion)
        {
            string descripcion = "";

            switch (situacion)
            {
                case SituacionDb.PRODUCCION_INICIADA:
                    return "Producción iniciada";
                case SituacionDb.PRODUCCION_FINALIZADA:
                    return "Producción finalizada";
            }

            return descripcion;
        }

        public virtual InterfazEntradaProduccionProducido MapToInterfazEntradaProduccionProducido(I_E_PRDC_SALIDA_PRD_PRODUCIDO entity)
        {
            if (entity == null)
                return null;

            return this.SetPorpertiesInterfazEntradaProduccionProducido(entity);
        }

        public virtual InterfazEntradaProduccionProducido SetPorpertiesInterfazEntradaProduccionProducido(I_E_PRDC_SALIDA_PRD_PRODUCIDO entity)
        {
            return new InterfazEntradaProduccionProducido()
            {
                AccionMovimiento = entity.ND_ACCION_MOVIMIENTO,
                CantidadProducido = entity.QT_PRODUCIDO,
                CodigoFaixa = entity.CD_FAIXA,
                CodigoProducto = entity.CD_PRODUTO,
                FechaVencimiento = entity.DT_VENCIMIENTO,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdProcesado = entity.ID_PROCESADO,
                NumeroEjecucion = entity.NU_INTERFAZ_EJECUCION,
                NumeroRegistro = entity.NU_REGISTRO,
                NumeroRegistroPadre = entity.NU_REGISTRO_PADRE,
                ValorMercaderia = entity.VL_MERCADERIA,
                ValorTributo = entity.VL_TRIBUTO,
                Semiacabado = entity.FL_SEMIACABADO
            };
        }

        public virtual InterfazEntradaProduccionInsumo MapToInterfazEntradaProduccionInsumo(I_E_PRDC_SALIDA_PRD_INSUMO entity)
        {
            if (entity == null)
                return null;

            return this.SetPorpertiesInterfazEntradaProduccionInsumo(entity);
        }

        public virtual InterfazEntradaProduccionInsumo SetPorpertiesInterfazEntradaProduccionInsumo(I_E_PRDC_SALIDA_PRD_INSUMO entity)
        {
            return new InterfazEntradaProduccionInsumo()
            {
                AccionMovimiento = entity.ND_ACCION_MOVIMIENTO,
                CantidadSalida = entity.QT_SALIDA,
                CodigoFaixa = entity.CD_FAIXA,
                CodigoProducto = entity.CD_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdProcesado = entity.ID_PROCESADO,
                NumeroEjecucion = entity.NU_INTERFAZ_EJECUCION,
                NumeroRegistro = entity.NU_REGISTRO,
                NumeroRegistroPadre = entity.NU_REGISTRO_PADRE,
                Semiacabado = entity.FL_SEMIACABADO ?? "N",
                Consumible = entity.FL_CONSUMIBLE ?? "N"
            };
        }

        public virtual InterfazEntradaProduccion MapToInterfazEntradaProduccion(I_E_PRDC_SALIDA_PRD entity)
        {
            if (entity == null)
                return null;

            return this.SetPropertiesInterfazEntradaProduccion(entity);
        }

        public virtual InterfazEntradaProduccion SetPropertiesInterfazEntradaProduccion(I_E_PRDC_SALIDA_PRD entity)
        {
            return new InterfazEntradaProduccion()
            {
                FechaAgregado = entity.DT_ADDROW_INTERFAZ,
                IdProcesado = entity.ID_PROCESADO,
                NumeroEjecucion = entity.NU_INTERFAZ_EJECUCION,
                NumeroIngreso = entity.NU_PRDC_INGRESO,
                NumeroRegistro = entity.NU_REGISTRO
            };
        }
    }
}
