using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.GridComponent;
using WIS.Persistence.Database;

namespace WIS.Domain.Produccion.Mappers
{
    public class IngresoProduccionMapper : Mapper
    {
        public IngresoProduccionMapper()
        {
        }

        public virtual T_PRDC_INGRESO MapObjectToEntity(IngresoProduccion ingreso)
        {
            var entity = new T_PRDC_INGRESO
            {
                NU_PRDC_INGRESO = ingreso.Id,
                CD_FUNCIONARIO = ingreso.Funcionario,
                DT_ADDROW = ingreso.FechaAlta,
                DT_UPDROW = ingreso.FechaActualizacion,
                DS_ANEXO3 = ingreso.Anexo3,
                DS_ANEXO2 = ingreso.Anexo2,
                DS_ANEXO1 = ingreso.Anexo1,
                DS_ANEXO4 = ingreso.Anexo4,
                DS_ANEXO5 = ingreso.Anexo5,
                CD_SITUACAO = ingreso.Situacion,
                CD_EMPRESA = ingreso.Empresa,
                CD_PRDC_DEFINICION = NullIfEmpty(ingreso.IdFormula),
                ID_GENERAR_PEDIDO = ingreso.GeneraPedido ? "S" : "N",
                ND_TIPO = ingreso.Tipo,
                NU_INTERFAZ_EJECUCION_ENTRADA = ingreso.EjecucionEntrada,
                NU_PRDC_ORIGINAL = ingreso.NumeroProduccionOriginal,
                QT_FORMULA = ingreso.CantidadIteracionesFormula,
                NU_PREDIO = ingreso.Predio,
                CD_PRDC_LINEA = ingreso.EspacioProduccion != null ? ingreso.EspacioProduccion.Id : ingreso.IdEspacioProducion,
                NU_POSICION_EN_COLA = ingreso.PosicionEnCola,
                NU_LOTE = ingreso.Lote,
                ND_ASIGNACION_LOTE = ingreso.IdModalidadLote,
                FL_INGRESO_DIRECTO_A_PRODUCCION = ingreso.IngresoDirectoProduccion,
                FL_PERMITIR_AUTOASIGNAR_LINEA = ingreso.PermitirAutoasignarLinea,
                ID_PRODUCCION_EXTERNO = ingreso.IdProduccionExterno,
                NU_TRANSACCION = ingreso.NuTransaccion,
                DT_INICIO_PRODUCCION = ingreso.FechaInicioProduccion,
                DT_FIN_PRODUCCION = ingreso.FechaFinProduccion,
                ID_MANUAL = ingreso.IdManual,
                NU_ULT_INTERFAZ_EJECUCION = ingreso.NroUltInterfazEjecucion,
            };

            return entity;
        }

        public virtual T_PRDC_DET_INGRESO_TEORICO MapObjectToEntity(IngresoProduccionDetalleTeorico detalle)
        {
            var entity = new T_PRDC_DET_INGRESO_TEORICO
            {
                NU_PRDC_DET_TEORICO = detalle.Id,
                NU_PRDC_INGRESO = detalle.IdIngresoProduccion,
                TP_REGISTRO = detalle.Tipo,
                CD_PRODUTO = detalle.Producto,
                CD_EMPRESA = detalle.Empresa,
                CD_FAIXA = detalle.Faixa,
                QT_TEORICO = detalle.CantidadTeorica,
                QT_PEDIDO_GENERADO = detalle.CantidadPedidoGenerada,
                QT_ABASTECIDO = detalle.CantidadAbastecido,
                QT_CONSUMIDO = detalle.CantidadConsumida,
                NU_IDENTIFICADOR = detalle.Identificador,
                NU_TRANSACCION = detalle.NumeroTransaccion,
                DT_ADDROW = detalle.FechaAlta,
                DS_ANEXO1 = detalle.Anexo1,
                DS_ANEXO2 = detalle.Anexo2,
                DS_ANEXO3 = detalle.Anexo3,
                DS_ANEXO4 = detalle.Anexo4

            };

            return entity;
        }

        public virtual T_PRDC_INGRESO_CONTROLES MapObjectToEntity(IngresoProduccionDetalleControl detalleControl)
        {
            var entity = new T_PRDC_INGRESO_CONTROLES
            {
                NU_PRDC_DET_TEORICO = detalleControl.Id,
                NU_PRDC_INGRESO = detalleControl.IdIngreso,
                CD_CONTROL = detalleControl.IdControl,
                NU_CTR_CALIDAD_PENDIENTE = detalleControl.IdControlPendiente
            };

            return entity;
        }

        public virtual T_PRDC_INGRESO_INSTRUCCION MapObjectToEntity(IngresoProduccionInstruccion instruccion)
        {
            var entity = new T_PRDC_INGRESO_INSTRUCCION
            {
                NU_PRDC_INGRESO_INSTRUCCION = instruccion.Id,
                NU_PRDC_INGRESO = instruccion.IdIngreso,
                VL_INSTRUCCIONES = instruccion.ValorInstruccion,
                TP_VALOR = instruccion.TipoValor
            };

            return entity;
        }

        public virtual IngresoProduccionDetalleControl MapEntityToObject(T_PRDC_INGRESO_CONTROLES entity)
        {
            var detalleControl = new IngresoProduccionDetalleControl
            {
                Id = entity.NU_PRDC_DET_TEORICO,
                IdIngreso = entity.NU_PRDC_INGRESO,
                IdControl = entity.CD_CONTROL,
                IdControlPendiente = entity.NU_CTR_CALIDAD_PENDIENTE,
            };

            return detalleControl;
        }

        public virtual IngresoProduccionDetalleTeorico MapEntityToObject(T_PRDC_DET_INGRESO_TEORICO entity)
        {
            var detalleControl = new IngresoProduccionDetalleTeorico
            {
                Id = entity.NU_PRDC_DET_TEORICO,
                IdIngresoProduccion = entity.NU_PRDC_INGRESO,
                CantidadAbastecido = entity.QT_ABASTECIDO,
                CantidadConsumida = entity.QT_CONSUMIDO,
                CantidadPedidoGenerada = entity.QT_PEDIDO_GENERADO,
                CantidadTeorica = entity.QT_TEORICO,
                Empresa = entity.CD_EMPRESA,
                Faixa = entity.CD_FAIXA,
                Producto = entity.CD_PRODUTO,
                Tipo = entity.TP_REGISTRO,
                Identificador = entity.NU_IDENTIFICADOR,
                NumeroTransaccion = entity.NU_TRANSACCION,
                FechaAlta = entity.DT_ADDROW,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Anexo4 = entity.DS_ANEXO4,
            };

            return detalleControl;
        }

        public virtual IngresoProduccionInstruccion MapEntityToObject(T_PRDC_INGRESO_INSTRUCCION entity)
        {
            var detalleControl = new IngresoProduccionInstruccion
            {
                Id = entity.NU_PRDC_INGRESO_INSTRUCCION,
                IdIngreso = entity.NU_PRDC_INGRESO,
                ValorInstruccion = entity.VL_INSTRUCCIONES,
                TipoValor = entity.TP_VALOR
            };

            return detalleControl;
        }

        public virtual IngresoProduccion MapEntityToObject(T_PRDC_INGRESO entity)
        {
            if (entity == null) return null;

            switch (entity.ND_TIPO)
            {
                case TipoIngresoProduccion.BlackBox:
                    return MapEntityToBlackBoxObject(entity);
                case TipoIngresoProduccion.WhiteBox:
                    return MapEntityToWhiteBoxObject(entity);
                case TipoIngresoProduccion.Colector:
                    return MapEntityToColectorObject(entity);
                default:
                    return MapEntityToBlackBoxObject(entity);
            }
        }

        public virtual IngresoProduccionColector MapEntityToColectorObject(T_PRDC_INGRESO entity)
        {
            var ingreso = new IngresoProduccionColector
            {
                Id = entity.NU_PRDC_INGRESO,
                Funcionario = entity.CD_FUNCIONARIO,
                FechaAlta = entity.DT_ADDROW,
                FechaActualizacion = entity.DT_UPDROW,
                Empresa = entity.CD_EMPRESA,
                Anexo3 = entity.DS_ANEXO3,
                Anexo2 = entity.DS_ANEXO2,
                Anexo1 = entity.DS_ANEXO1,
                Anexo4 = entity.DS_ANEXO4,
                Anexo5 = entity.DS_ANEXO5,
                Situacion = entity.CD_SITUACAO,
                GeneraPedido = entity.ID_GENERAR_PEDIDO == "S",
                Tipo = entity.ND_TIPO,
                EjecucionEntrada = entity.NU_INTERFAZ_EJECUCION_ENTRADA,
                NumeroProduccionOriginal = entity.NU_PRDC_ORIGINAL,
                IdEspacioProducion = entity.CD_PRDC_LINEA,
                IdFormula = entity.CD_PRDC_DEFINICION,
                CantidadIteracionesFormula = (int)entity.QT_FORMULA,
                Predio = entity.NU_PREDIO,
                PosicionEnCola = entity.NU_POSICION_EN_COLA,
                Lote = entity.NU_LOTE,
                IdModalidadLote = entity.ND_ASIGNACION_LOTE,
                IngresoDirectoProduccion = entity.FL_INGRESO_DIRECTO_A_PRODUCCION,
                PermitirAutoasignarLinea = entity.FL_PERMITIR_AUTOASIGNAR_LINEA,
                IdProduccionExterno = entity.ID_PRODUCCION_EXTERNO,
                NuTransaccion = entity.NU_TRANSACCION,
                FechaInicioProduccion = entity.DT_INICIO_PRODUCCION,
                FechaFinProduccion = entity.DT_FIN_PRODUCCION,
                IdManual = entity.ID_MANUAL,
                NroUltInterfazEjecucion = entity.NU_ULT_INTERFAZ_EJECUCION,
            };

            return ingreso;
        }
        public virtual IngresoProduccionBlackBox MapEntityToBlackBoxObject(T_PRDC_INGRESO entity)
        {
            var ingreso = new IngresoProduccionBlackBox
            {
                Id = entity.NU_PRDC_INGRESO,
                Funcionario = entity.CD_FUNCIONARIO,
                FechaAlta = entity.DT_ADDROW,
                FechaActualizacion = entity.DT_UPDROW,
                Empresa = entity.CD_EMPRESA,
                Anexo3 = entity.DS_ANEXO3,
                Anexo2 = entity.DS_ANEXO2,
                Anexo1 = entity.DS_ANEXO1,
                Anexo4 = entity.DS_ANEXO4,
                Anexo5 = entity.DS_ANEXO5,
                Situacion = entity.CD_SITUACAO,
                GeneraPedido = entity.ID_GENERAR_PEDIDO == "S",
                Tipo = entity.ND_TIPO,
                EjecucionEntrada = entity.NU_INTERFAZ_EJECUCION_ENTRADA,
                NumeroProduccionOriginal = entity.NU_PRDC_ORIGINAL,
                IdEspacioProducion = entity.CD_PRDC_LINEA,
                IdFormula = entity.CD_PRDC_DEFINICION,
                CantidadIteracionesFormula = (int)entity.QT_FORMULA,
                Predio = entity.NU_PREDIO,
                PosicionEnCola = entity.NU_POSICION_EN_COLA,
                Lote = entity.NU_LOTE,
                IdModalidadLote = entity.ND_ASIGNACION_LOTE,
                IngresoDirectoProduccion = entity.FL_INGRESO_DIRECTO_A_PRODUCCION,
                PermitirAutoasignarLinea = entity.FL_PERMITIR_AUTOASIGNAR_LINEA,
                IdProduccionExterno = entity.ID_PRODUCCION_EXTERNO,
                NuTransaccion = entity.NU_TRANSACCION,
                FechaInicioProduccion = entity.DT_INICIO_PRODUCCION,
                FechaFinProduccion = entity.DT_FIN_PRODUCCION,
                IdManual = entity.ID_MANUAL,
                NroUltInterfazEjecucion = entity.NU_ULT_INTERFAZ_EJECUCION,
            };

            return ingreso;
        }

        public virtual IngresoProduccionWhiteBox MapEntityToWhiteBoxObject(T_PRDC_INGRESO entity)
        {
            var ingreso = new IngresoProduccionWhiteBox
            {
                Id = entity.NU_PRDC_INGRESO,
                Funcionario = entity.CD_FUNCIONARIO,
                FechaAlta = entity.DT_ADDROW,
                FechaActualizacion = entity.DT_UPDROW,
                Empresa = entity.CD_EMPRESA,
                Anexo3 = entity.DS_ANEXO3,
                Anexo2 = entity.DS_ANEXO2,
                Anexo1 = entity.DS_ANEXO1,
                Anexo4 = entity.DS_ANEXO4,
                Anexo5 = entity.DS_ANEXO5,
                Situacion = entity.CD_SITUACAO,
                GeneraPedido = entity.ID_GENERAR_PEDIDO == "S",
                Tipo = entity.ND_TIPO,
                EjecucionEntrada = entity.NU_INTERFAZ_EJECUCION_ENTRADA,
                NumeroProduccionOriginal = entity.NU_PRDC_ORIGINAL,
                IdEspacioProducion = entity.CD_PRDC_LINEA,
                IdFormula = entity.CD_PRDC_DEFINICION,
                CantidadIteracionesFormula = (int)entity.QT_FORMULA,
                Predio = entity.NU_PREDIO,
                PosicionEnCola = entity.NU_POSICION_EN_COLA,
                Lote = entity.NU_LOTE,
                IdModalidadLote = entity.ND_ASIGNACION_LOTE,
                IngresoDirectoProduccion = entity.FL_INGRESO_DIRECTO_A_PRODUCCION,
                PermitirAutoasignarLinea = entity.FL_PERMITIR_AUTOASIGNAR_LINEA,
                IdProduccionExterno = entity.ID_PRODUCCION_EXTERNO,
                NuTransaccion = entity.NU_TRANSACCION,
                FechaInicioProduccion = entity.DT_INICIO_PRODUCCION,
                FechaFinProduccion = entity.DT_FIN_PRODUCCION,
                IdManual = entity.ID_MANUAL,
                NroUltInterfazEjecucion = entity.NU_ULT_INTERFAZ_EJECUCION,
            };

            return ingreso;
        }


        public virtual IngresoProduccionDetalleReal MapEntityToObject(T_PRDC_DET_INGRESO_REAL detalleIngresoReal)
        {
            return new IngresoProduccionDetalleReal()
            {
                Empresa = detalleIngresoReal.CD_EMPRESA,
                Faixa = detalleIngresoReal.CD_FAIXA,
                Producto = detalleIngresoReal.CD_PRODUTO,
                Identificador = detalleIngresoReal.NU_IDENTIFICADOR,
                DsAnexo1 = detalleIngresoReal.DS_ANEXO1,
                DsAnexo2 = detalleIngresoReal.DS_ANEXO2,
                DsAnexo3 = detalleIngresoReal.DS_ANEXO3,
                DsAnexo4 = detalleIngresoReal.DS_ANEXO4,
                DtAddrow = detalleIngresoReal.DT_ADDROW,
                NuOrden = detalleIngresoReal.NU_ORDEN,
                NuPrdcIngreso = detalleIngresoReal.NU_PRDC_INGRESO,
                NuPrdcIngresoReal = detalleIngresoReal.NU_PRDC_INGRESO_REAL,
                NuTransaccion = detalleIngresoReal.NU_TRANSACCION,
                QtMerma = detalleIngresoReal.QT_MERMA,
                QtDesafectado = detalleIngresoReal.QT_DESAFECTADA,
                QtNotificado = detalleIngresoReal.QT_NOTIFICADO,
                QtReal = detalleIngresoReal.QT_REAL,
                QtRealOriginal = detalleIngresoReal.QT_REAL_ORIGINAL,
                Referencia = detalleIngresoReal.DS_REFERENCIA,
                Estado = detalleIngresoReal.ND_ESTADO
            };
        }

        public virtual List<IngresoProduccionDetalleReal> MapEntityToObject(List<T_PRDC_DET_INGRESO_REAL> detalleIngresoReal)
        {
            List<IngresoProduccionDetalleReal> colIngresoProduccion = new List<IngresoProduccionDetalleReal>();

            foreach (var item in detalleIngresoReal)
            {
                colIngresoProduccion.Add(MapEntityToObject(item));
            }

            return colIngresoProduccion;
        }

        public virtual IngresoProduccionDetalleSalida MapEntityToObject(T_PRDC_DET_SALIDA_REAL detalleSalidaReal)
        {
            return new IngresoProduccionDetalleSalida()
            {
                Empresa = detalleSalidaReal.CD_EMPRESA,
                Faixa = detalleSalidaReal.CD_FAIXA,
                Producto = detalleSalidaReal.CD_PRODUTO,
                Identificador = detalleSalidaReal.NU_IDENTIFICADOR,
                DsAnexo1 = detalleSalidaReal.DS_ANEXO1,
                DsAnexo2 = detalleSalidaReal.DS_ANEXO2,
                DsAnexo3 = detalleSalidaReal.DS_ANEXO3,
                DsAnexo4 = detalleSalidaReal.DS_ANEXO4,
                DtAddrow = detalleSalidaReal.DT_ADDROW,
                NuOrden = detalleSalidaReal.NU_ORDEN,
                NuPrdcIngreso = detalleSalidaReal.NU_PRDC_INGRESO,
                NuPrdcIngresoSalida = detalleSalidaReal.NU_PRDC_SALIDA_REAL,
                NuPrdcIngresoTeorico = detalleSalidaReal.NU_PRDC_DET_TEORICO,
                NuTransaccion = detalleSalidaReal.NU_TRANSACCION,
                QtProducido = detalleSalidaReal.QT_PRODUCIDO,
                DsMotivo = detalleSalidaReal.DS_MOTIVO,
                NdEstado = detalleSalidaReal.ND_ESTADO,
                NdMotivo = detalleSalidaReal.ND_MOTIVO,
                DtVencimiento = detalleSalidaReal.DT_VENCIMIENTO,
                QtNotificado = detalleSalidaReal.QT_NOTIFICADO,
            };
        }

        public virtual List<IngresoProduccionDetalleSalida> MapEntityToObject(List<T_PRDC_DET_SALIDA_REAL> detalleIngresoReal)
        {
            List<IngresoProduccionDetalleSalida> colIngresoProduccion = new List<IngresoProduccionDetalleSalida>();

            foreach (var item in detalleIngresoReal)
            {
                colIngresoProduccion.Add(MapEntityToObject(item));
            }

            return colIngresoProduccion;
        }

        public virtual T_PRDC_DET_SALIDA_REAL MapObjectToEntity(IngresoProduccionDetalleSalida detalleSalidaReal)
        {
            return new T_PRDC_DET_SALIDA_REAL()
            {
                CD_EMPRESA = detalleSalidaReal.Empresa,
                CD_FAIXA = detalleSalidaReal.Faixa,
                CD_PRODUTO = detalleSalidaReal.Producto,
                NU_IDENTIFICADOR = detalleSalidaReal.Identificador,
                DS_ANEXO1 = detalleSalidaReal.DsAnexo1,
                DS_ANEXO2 = detalleSalidaReal.DsAnexo2,
                DS_ANEXO3 = detalleSalidaReal.DsAnexo3,
                DS_ANEXO4 = detalleSalidaReal.DsAnexo4,
                DT_ADDROW = detalleSalidaReal.DtAddrow,
                NU_ORDEN = detalleSalidaReal.NuOrden,
                NU_PRDC_INGRESO = detalleSalidaReal.NuPrdcIngreso,
                NU_PRDC_SALIDA_REAL = detalleSalidaReal.NuPrdcIngresoSalida,
                NU_TRANSACCION = detalleSalidaReal.NuTransaccion,
                DS_MOTIVO = NullIfEmpty(detalleSalidaReal.DsMotivo),
                ND_ESTADO = detalleSalidaReal.NdEstado,
                ND_MOTIVO = detalleSalidaReal.NdMotivo,
                DT_VENCIMIENTO = detalleSalidaReal.DtVencimiento,
                QT_NOTIFICADO = detalleSalidaReal.QtNotificado,
                NU_PRDC_DET_TEORICO = detalleSalidaReal.NuPrdcIngresoTeorico,
                QT_PRODUCIDO = detalleSalidaReal.QtProducido,
            };
        }

        public virtual List<T_PRDC_DET_SALIDA_REAL> MapEntityToObject(List<IngresoProduccionDetalleSalida> detalleIngresoReal)
        {
            List<T_PRDC_DET_SALIDA_REAL> colIngresoProduccion = new List<T_PRDC_DET_SALIDA_REAL>();

            foreach (var item in detalleIngresoReal)
            {
                colIngresoProduccion.Add(MapObjectToEntity(item));
            }

            return colIngresoProduccion;
        }

        public virtual T_PRDC_DET_INGRESO_REAL MapObjectToEntity(IngresoProduccionDetalleReal detalleIngresoReal)
        {
            return new T_PRDC_DET_INGRESO_REAL()
            {
                CD_EMPRESA = detalleIngresoReal.Empresa,
                CD_FAIXA = detalleIngresoReal.Faixa,
                CD_PRODUTO = detalleIngresoReal.Producto,
                NU_IDENTIFICADOR = detalleIngresoReal.Identificador,
                DS_ANEXO1 = detalleIngresoReal.DsAnexo1,
                DS_ANEXO2 = detalleIngresoReal.DsAnexo2,
                DS_ANEXO3 = detalleIngresoReal.DsAnexo3,
                DS_ANEXO4 = detalleIngresoReal.DsAnexo4,
                DT_ADDROW = detalleIngresoReal.DtAddrow,
                NU_ORDEN = detalleIngresoReal.NuOrden,
                QT_DESAFECTADA = detalleIngresoReal.QtDesafectado,
                NU_PRDC_INGRESO = detalleIngresoReal.NuPrdcIngreso,
                NU_PRDC_INGRESO_REAL = detalleIngresoReal.NuPrdcIngresoReal,
                NU_TRANSACCION = detalleIngresoReal.NuTransaccion,
                QT_MERMA = detalleIngresoReal.QtMerma,
                QT_NOTIFICADO = detalleIngresoReal.QtNotificado,
                QT_REAL = detalleIngresoReal.QtReal,
                QT_REAL_ORIGINAL = detalleIngresoReal.QtRealOriginal,
                DS_REFERENCIA = detalleIngresoReal.Referencia,
                ND_ESTADO = detalleIngresoReal.Estado
            };
        }

        public virtual IngresoProduccionDetalleTeorico MapRowToObject(GridRow row, IFormatProvider formatProvider)
        {
            var detalle = new IngresoProduccionDetalleTeorico
            {
                Tipo = row.GetCell("TP_REGISTRO").Value,
                Producto = row.GetCell("CD_PRODUTO").Value,
                Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value),
                Faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, formatProvider),
                Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                CantidadTeorica = decimal.Parse(row.GetCell("QT_TEORICO").Value, formatProvider),
            };

            return detalle;
        }

        public virtual List<IngresoProduccionDetalleTeorico> MapFormulaToObject(Formula formula, int cantidad, IUnitOfWork uow)
        {
            List<IngresoProduccionDetalleTeorico> detalles = new List<IngresoProduccionDetalleTeorico>();

            foreach (var entrada in formula.Entrada)
            {

                var manejoIdentificador = uow.ProductoRepository.GetProductoManejoIdentificador(entrada.Empresa, entrada.Producto);
                var identificador = manejoIdentificador == ManejoIdentificador.Producto ? "*" : ManejoIdentificadorDb.IdentificadorAuto;

                var detalle = new IngresoProduccionDetalleTeorico
                {
                    Tipo = CIngresoProduccionDetalleTeorico.TipoDetalleEntrada,
                    Producto = entrada.Producto,
                    Empresa = entrada.Empresa,
                    Faixa = entrada.Faixa,
                    CantidadTeorica = entrada.CantidadCompleta * cantidad,
                    Identificador = identificador,
                };

                detalles.Add(detalle);
            }

            foreach (var salida in formula.Salida)
            {
                var manejoIdentificador = uow.ProductoRepository.GetProductoManejoIdentificador(salida.Empresa, salida.Producto);
                var identificador = manejoIdentificador == ManejoIdentificador.Producto ? "*" : ManejoIdentificadorDb.IdentificadorAuto;

                var detalle = new IngresoProduccionDetalleTeorico
                {
                    Tipo = CIngresoProduccionDetalleTeorico.TipoDetalleSalida,
                    Producto = salida.Producto,
                    Empresa = salida.Empresa,
                    Faixa = salida.Faixa,
                    CantidadTeorica = salida.CantidadCompleta * cantidad,
                    Identificador = identificador,
                };

                detalles.Add(detalle);
            }

            return detalles;
        }

        public virtual T_PRDC_DET_SALIDA_REAL_MOV MapObjectToEntity(SalidaProduccionDetalle detalle)
        {
            if (detalle == null)
                return null;

            return new T_PRDC_DET_SALIDA_REAL_MOV
            {
                NU_SALIDA_REAL_MOV = detalle.Id,
                NU_PRDC_INGRESO = detalle.NuPrdcIngreso,
                CD_ENDERECO = detalle.Ubicacion,
                CD_PRODUTO = detalle.Producto,
                NU_IDENTIFICADOR = detalle.Identificador,
                CD_EMPRESA = detalle.Empresa,
                CD_FAIXA = detalle.Faixa,
                QT_ESTOQUE = detalle.Cantidad,
                ND_MOTIVO = detalle.Motivo,
                NU_TRANSACCION = detalle.NuTransaccion,
                DT_ADDROW = detalle.FechaAlta,
                DT_VENCIMIENTO = detalle.Vencimiento,
                FL_PENDIENTE_NOTIFICAR = detalle.FlPendienteNotificar,
                DS_MOTIVO = NullIfEmpty(detalle.DsMotivo)
            };
        }

        public virtual T_PRDC_DET_INGRESO_REAL_MOV MapObjectToEntity(IngresoProduccionDetalle detalle)
        {
            if (detalle == null)
                return null;

            return new T_PRDC_DET_INGRESO_REAL_MOV
            {
                NU_INGRESO_REAL_MOV = detalle.Id,
                NU_PRDC_INGRESO = detalle.NuPrdcIngreso,
                CD_ENDERECO = detalle.Ubicacion,
                CD_PRODUTO = detalle.Producto,
                NU_IDENTIFICADOR = detalle.Identificador,
                CD_EMPRESA = detalle.Empresa,
                CD_FAIXA = detalle.Faixa,
                QT_ESTOQUE = detalle.Cantidad,
                NU_TRANSACCION = detalle.NuTransaccion,
                DT_ADDROW = detalle.FechaAlta,
                DT_VENCIMIENTO = detalle.Vencimiento,
                ND_MOTIVO = detalle.Motivo,
                FL_PENDIENTE_NOTIFICAR = detalle.FlPendienteNotificar,
                NU_PRDC_INGRESO_REAL = detalle.NuPrdcIngresoReal
            };
        }

        public virtual List<IngresoProduccionDetalleTeorico> MapEntityToObject(List<T_PRDC_DET_INGRESO_TEORICO> detallesEntity)
        {
            List<IngresoProduccionDetalleTeorico> detalles = new List<IngresoProduccionDetalleTeorico>();
            foreach (var entity in detallesEntity)
            {
                detalles.Add(new IngresoProduccionDetalleTeorico
                {
                    Id = entity.NU_PRDC_DET_TEORICO,
                    IdIngresoProduccion = entity.NU_PRDC_INGRESO,
                    Tipo = entity.TP_REGISTRO,
                    Producto = entity.CD_PRODUTO,
                    Empresa = entity.CD_EMPRESA,
                    Faixa = entity.CD_FAIXA,
                    CantidadTeorica = entity.QT_TEORICO,
                    CantidadPedidoGenerada = entity.QT_PEDIDO_GENERADO,
                    CantidadAbastecido = entity.QT_ABASTECIDO,
                    CantidadConsumida = entity.QT_CONSUMIDO,
                    Identificador = entity.NU_IDENTIFICADOR,
                    NumeroTransaccion = entity.NU_TRANSACCION,
                    FechaAlta = entity.DT_ADDROW,
                    Anexo1 = entity.DS_ANEXO1,
                    Anexo2 = entity.DS_ANEXO2,
                    Anexo3 = entity.DS_ANEXO3,
                    Anexo4 = entity.DS_ANEXO4
                });

            }

            return detalles;
        }
    }
}
