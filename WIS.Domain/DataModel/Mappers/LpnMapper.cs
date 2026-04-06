using System.Collections.Generic;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class LpnMapper : Mapper
    {
        public LpnMapper()
        {

        }

        public virtual LpnTipo MapToObject(T_LPN_TIPO entity)
        {
            if (entity == null)
            {
                return null;
            }

            var obj = new LpnTipo
            {
                Tipo = entity.TP_LPN_TIPO,
                Nombre = entity.NM_LPN_TIPO,
                Descripcion = entity.DS_LPN_TIPO,
                PermiteConsolidar = entity.FL_PERMITE_CONSOLIDAR,
                PermiteExtraerLineas = entity.FL_PERMITE_EXTRAER_LINEAS,
                PermiteAgregarLineas = entity.FL_PERMITE_AGREGAR_LINEAS,
                CrearSoloAlIngreso = entity.FL_CREAR_SOLO_AL_INGRESO,
                MultiProducto = entity.FL_MULTIPRODUCTO,
                MultiLote = entity.FL_MULTI_LOTE,
                PermiteAnidacion = entity.FL_PERMITE_ANIDACION,
                NumeroTemplate = entity.NU_TEMPLATE_ETIQUETA,
                NumeroComponente = entity.NU_COMPONENTE,
                ContenedorLPN = entity.FL_CONTENEDOR_LPN,
                NumeroSecuencia = entity.NU_SEQ_LPN,
                PermiteGenerar = entity.FL_PERMITE_GENERAR,
                IngresoRecepcionAtributo = entity.FL_INGRESO_RECEPCION_ATRIBUTO,
                IngresoPickingAtributo = entity.FL_INGRESO_PICKING_ATRIBUTO,
                Prefijo = entity.VL_PREFIJO,
                EtiquetaRecepcion = entity.TP_ETIQUETA_RECEPCION,
                PermiteDestruirAlmacenaje = entity.FL_PERMITE_DESTRUIR_ALM,
                PuntajePicking = new LpnTipoPuntajePicking(),
            };

            obj.PuntajePicking.Bonus = entity.VL_PICKING_SCORE_BONUS;
            obj.PuntajePicking.IgualConReserva = entity.VL_PICKING_SCORE_EQNR;
            obj.PuntajePicking.IgualSinReserva = entity.VL_PICKING_SCORE_LTNR;
            obj.PuntajePicking.Inexistente = entity.VL_PICKING_SCORE_NE;
            obj.PuntajePicking.Mayor = entity.VL_PICKING_SCORE_GT;
            obj.PuntajePicking.MenorConReserva = entity.VL_PICKING_SCORE_LTR;
            obj.PuntajePicking.MenorSinReserva = entity.VL_PICKING_SCORE_LTNR;

            return obj;
        }

        public virtual T_LPN_TIPO MapToEntity(LpnTipo obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new T_LPN_TIPO
            {
                TP_LPN_TIPO = obj.Tipo,
                NM_LPN_TIPO = obj.Nombre,
                DS_LPN_TIPO = obj.Descripcion,
                FL_PERMITE_CONSOLIDAR = obj.PermiteConsolidar,
                FL_PERMITE_EXTRAER_LINEAS = obj.PermiteExtraerLineas,
                FL_PERMITE_AGREGAR_LINEAS = obj.PermiteAgregarLineas,
                FL_CREAR_SOLO_AL_INGRESO = obj.CrearSoloAlIngreso,
                FL_MULTIPRODUCTO = obj.MultiProducto,
                FL_MULTI_LOTE = obj.MultiLote,
                FL_PERMITE_ANIDACION = obj.PermiteAnidacion,
                NU_TEMPLATE_ETIQUETA = obj.NumeroTemplate,
                NU_COMPONENTE = obj.NumeroComponente,
                FL_CONTENEDOR_LPN = obj.ContenedorLPN,
                NU_SEQ_LPN = obj.NumeroSecuencia,
                FL_PERMITE_GENERAR = obj.PermiteGenerar,
                FL_INGRESO_RECEPCION_ATRIBUTO = obj.IngresoRecepcionAtributo,
                FL_INGRESO_PICKING_ATRIBUTO = obj.IngresoPickingAtributo,
                VL_PREFIJO = obj.Prefijo,
                TP_ETIQUETA_RECEPCION = obj.EtiquetaRecepcion,
                FL_PERMITE_DESTRUIR_ALM = obj.PermiteDestruirAlmacenaje,
                VL_PICKING_SCORE_BONUS = obj.PuntajePicking?.Bonus,
                VL_PICKING_SCORE_EQNR = obj.PuntajePicking?.IgualSinReserva,
                VL_PICKING_SCORE_EQR = obj.PuntajePicking?.IgualConReserva,
                VL_PICKING_SCORE_GT = obj.PuntajePicking?.Mayor,
                VL_PICKING_SCORE_LTNR = obj.PuntajePicking?.MenorSinReserva,
                VL_PICKING_SCORE_LTR = obj.PuntajePicking?.MenorConReserva,
                VL_PICKING_SCORE_NE = obj.PuntajePicking?.Inexistente,
            };
        }

        public virtual Lpn MapToObject(T_LPN entity)
        {
            if (entity == null)
                return null;

            var lpn = new Lpn
            {
                NumeroLPN = entity.NU_LPN,
                IdExterno = entity.ID_LPN_EXTERNO,
                Tipo = entity.TP_LPN_TIPO,
                Estado = entity.ID_ESTADO,
                Ubicacion = entity.CD_ENDERECO,
                FechaAdicion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                FechaActivacion = entity.DT_ACTIVACION,
                FechaFin = entity.DT_FIN,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Empresa = entity.CD_EMPRESA,
                IdPacking = entity.ID_PACKING,
                NroAgenda = entity.NU_AGENDA,
                DisponibleLiberacion = entity.FL_DISPONIBLE_LIBERACION
            };

            lpn.Detalles = new List<LpnDetalle>();

            foreach (var det in entity.T_LPN_DET)
            {
                lpn.Detalles.Add(MapToObject(det));
            }

            return lpn;
        }

        public virtual T_LPN MapToEntity(Lpn obj)
        {
            if (obj == null) return null;

            return new T_LPN
            {
                NU_LPN = obj.NumeroLPN,
                ID_LPN_EXTERNO = obj.IdExterno,
                TP_LPN_TIPO = obj.Tipo,
                ID_ESTADO = obj.Estado,
                CD_ENDERECO = obj.Ubicacion,
                DT_ADDROW = obj.FechaAdicion,
                DT_UPDROW = obj.FechaModificacion,
                DT_ACTIVACION = obj.FechaActivacion,
                DT_FIN = obj.FechaFin,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                CD_EMPRESA = obj.Empresa,
                ID_PACKING = NullIfEmpty(obj.IdPacking),
                NU_AGENDA = obj.NroAgenda,
                FL_DISPONIBLE_LIBERACION = obj.DisponibleLiberacion
            };
        }

        public virtual LpnDetalle MapToObject(T_LPN_DET entity)
        {
            if (entity == null)
                return null;

            return new LpnDetalle
            {
                Id = entity.ID_LPN_DET,
                NumeroLPN = entity.NU_LPN,
                CodigoProducto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Empresa = entity.CD_EMPRESA,
                Lote = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Cantidad = entity.QT_ESTOQUE,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Vencimiento = entity.DT_FABRICACAO,
                CantidadDeclarada = entity.QT_DECLARADA,
                CantidadRecibida = entity.QT_RECIBIDA,
                CantidadReserva = entity.QT_RESERVA_SAIDA,
                CantidadExpedida = entity.QT_EXPEDIDA,
                IdLineaSistemaExterno = entity.ID_LINEA_SISTEMA_EXTERNO,
                IdAveria = entity.ID_AVERIA,
                IdCtrlCalidad = entity.ID_CTRL_CALIDAD,
                IdInventario = entity.ID_INVENTARIO,
                MotivoAveria = entity.CD_MOTIVO_AVERIA
            };
        }

        public virtual T_LPN_DET MapToEntity(LpnDetalle obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new T_LPN_DET
            {
                ID_LPN_DET = obj.Id,
                NU_LPN = obj.NumeroLPN,
                CD_PRODUTO = obj.CodigoProducto,
                CD_FAIXA = obj.Faixa,
                CD_EMPRESA = obj.Empresa,
                NU_IDENTIFICADOR = obj.Lote?.Trim()?.ToUpper(),
                QT_ESTOQUE = obj.Cantidad,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                DT_FABRICACAO = obj.Vencimiento,
                QT_DECLARADA = obj.CantidadDeclarada,
                QT_RECIBIDA = obj.CantidadRecibida,
                QT_RESERVA_SAIDA = obj.CantidadReserva,
                QT_EXPEDIDA = obj.CantidadExpedida,
                ID_LINEA_SISTEMA_EXTERNO = obj.IdLineaSistemaExterno,
                ID_AVERIA = obj.IdAveria,
                ID_CTRL_CALIDAD = obj.IdCtrlCalidad,
                ID_INVENTARIO = obj.IdInventario,
                CD_MOTIVO_AVERIA = obj.MotivoAveria
            };
        }

        public virtual LpnBarras MapToObject(T_LPN_BARRAS entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new LpnBarras
            {
                NumeroLpn = entity.NU_LPN,
                CodigoBarras = entity.CD_BARRAS,
                IdLpnBarras = entity.ID_LPN_BARRAS,
                Orden = entity.NU_ORDEN,
                Tipo = entity.TP_BARRAS
            };
        }

        public virtual T_LPN_BARRAS MapToEntity(LpnBarras obj)
        {
            if (obj == null)
            {
                return null;
            }
            return new T_LPN_BARRAS
            {
                NU_LPN = obj.NumeroLpn,
                CD_BARRAS = obj.CodigoBarras,
                ID_LPN_BARRAS = obj.IdLpnBarras,
                NU_ORDEN = obj.Orden,
                TP_BARRAS = obj.Tipo
            };
        }

        public virtual LpnTipoAtributo MapToObject(T_LPN_TIPO_ATRIBUTO entity)
        {
            if (entity == null)
                return null;

            return new LpnTipoAtributo
            {
                TipoLpn = entity.TP_LPN_TIPO,
                IdAtributo = entity.ID_ATRIBUTO,
                ValorInicial = entity.VL_INICIAL,
                Requerido = entity.FL_REQUERIDO,
                ValidoInterfaz = entity.VL_VALIDO_INTERFAZ,
                Orden = entity.NU_ORDEN,
                IdConsolidador = entity.ID_CONSOLIDACION_TIPO,
                EstadoInicial = entity.ID_ESTADO_INICIAL,
            };
        }

        public virtual T_LPN_TIPO_ATRIBUTO MapToEntity(LpnTipoAtributo obj)
        {
            if (obj == null)
                return null;

            return new T_LPN_TIPO_ATRIBUTO
            {
                TP_LPN_TIPO = obj.TipoLpn,
                ID_ATRIBUTO = obj.IdAtributo,
                VL_INICIAL = obj.ValorInicial,
                FL_REQUERIDO = obj.Requerido,
                VL_VALIDO_INTERFAZ = obj.ValidoInterfaz,
                NU_ORDEN = obj.Orden,
                ID_CONSOLIDACION_TIPO = obj.IdConsolidador,
                ID_ESTADO_INICIAL = obj.EstadoInicial
            };
        }

        public virtual T_LPN_AUDITORIA MapToEntity(AuditoriaLpn obj)
        {
            if (obj == null)
            {
                return null;
            }
            return new T_LPN_AUDITORIA()
            {
                NU_AUDITORIA = obj.Auditoria,
                ID_LPN_DET = obj.LpnDet,
                NU_LPN = obj.Lpn,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                CD_EMPRESA = obj.Empresa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                NU_TRANSACCION = obj.Transaccion,
                QT_ESTOQUE = obj.Estoque,
                QT_AUDITADA = obj.Auditada,
                QT_DIFERENCIA = obj.Diferencia,
                ID_ESTADO = obj.Estado,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                CD_FUNC_ADDROW = obj.FuncionarioAlta,
                CD_FUNC_UPDROW = obj.FuncionarioModificacion,
                CD_FUNC_UPDROW_ESTADO = obj.FuncionarioModificacionEstado,
                ID_NIVEL = obj.Nivel,
                DT_FABRICACAO = obj.Vencimiento,
                NU_AUDITORIA_AGRUPADOR = obj.AuditoriaAgrupador,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete
            };
        }

        public virtual AuditoriaLpn MapToObject(T_LPN_AUDITORIA entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new AuditoriaLpn()
            {
                Auditoria = entity.NU_AUDITORIA,
                LpnDet = entity.ID_LPN_DET,
                Lpn = entity.NU_LPN,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Empresa = entity.CD_EMPRESA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Transaccion = entity.NU_TRANSACCION,
                Estoque = entity.QT_ESTOQUE,
                Auditada = entity.QT_AUDITADA,
                Diferencia = entity.QT_DIFERENCIA,
                Estado = entity.ID_ESTADO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                FuncionarioAlta = entity.CD_FUNC_ADDROW,
                FuncionarioModificacion = entity.CD_FUNC_UPDROW,
                FuncionarioModificacionEstado = entity.CD_FUNC_UPDROW_ESTADO,
                Nivel = entity.ID_NIVEL,
                Vencimiento = entity.DT_FABRICACAO,
                AuditoriaAgrupador = entity.NU_AUDITORIA_AGRUPADOR,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE
            };


        }

        public virtual List<AuditoriaLpn> MapToObject(List<T_LPN_AUDITORIA> objs)
        {
            var detalles = new List<AuditoriaLpn>();
            foreach (var det in objs)
            {
                detalles.Add(new AuditoriaLpn()
                {
                    Auditoria = det.NU_AUDITORIA,
                    LpnDet = det.ID_LPN_DET,
                    Lpn = det.NU_LPN,
                    Producto = det.CD_PRODUTO,
                    Faixa = det.CD_FAIXA,
                    Empresa = det.CD_EMPRESA,
                    Identificador = det.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                    Transaccion = det.NU_TRANSACCION,
                    Estoque = det.QT_ESTOQUE,
                    Auditada = det.QT_AUDITADA,
                    Diferencia = det.QT_DIFERENCIA,
                    Estado = det.ID_ESTADO,
                    FechaAlta = det.DT_ADDROW,
                    FechaModificacion = det.DT_UPDROW,
                    FuncionarioAlta = det.CD_FUNC_ADDROW,
                    FuncionarioModificacion = det.CD_FUNC_UPDROW,
                    FuncionarioModificacionEstado = det.CD_FUNC_UPDROW_ESTADO,
                    Nivel = det.ID_NIVEL,
                    Vencimiento = det.DT_FABRICACAO,
                    AuditoriaAgrupador = det.NU_AUDITORIA_AGRUPADOR,
                    TransaccionDelete = det.NU_TRANSACCION_DELETE
                });
            }

            return detalles;
        }

        public virtual LpnAuditoriaAtributo MapToObject(T_LPN_AUDITORIA_ATRIBUTO entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new LpnAuditoriaAtributo
            {
                Auditoria = entity.NU_AUDITORIA,
                IdAtributo = entity.ID_ATRIBUTO,
                ValorAtributo = entity.VL_LPN_DET_ATRIBUTO,

            };
        }

        public virtual LpnConsolidadorTipo MapToObject(T_LPN_CONSOLIDACION_TIPO entity)
        {
            if (entity == null)
            {
                return null;
            }
            LpnConsolidadorTipo consolidadortipo = new LpnConsolidadorTipo();
            consolidadortipo.IdConsolidador = entity.ID_CONSOLIDACION_TIPO;
            consolidadortipo.NombreConsolidador = entity.NM_CONSOLIDACION;
            return consolidadortipo;

        }

        public virtual LpnAtributo MapToObject(T_LPN_ATRIBUTO entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new LpnAtributo
            {
                NumeroLpn = entity.NU_LPN,
                Tipo = entity.TP_LPN_TIPO,
                Id = entity.ID_ATRIBUTO,
                Nombre = entity.T_ATRIBUTO?.NM_ATRIBUTO,
                Valor = entity.VL_LPN_ATRIBUTO,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Estado = entity.ID_ESTADO
            };
        }

        public virtual T_LPN_ATRIBUTO MapToEntity(LpnAtributo obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new T_LPN_ATRIBUTO
            {
                NU_LPN = obj.NumeroLpn,
                TP_LPN_TIPO = obj.Tipo,
                ID_ATRIBUTO = obj.Id,
                VL_LPN_ATRIBUTO = obj.Valor,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                ID_ESTADO = obj.Estado
            };
        }

        public virtual LpnDetalleAtributo MapToObject(T_LPN_DET_ATRIBUTO entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new LpnDetalleAtributo
            {
                IdLpnDetalle = entity.ID_LPN_DET,
                NumeroLpn = entity.NU_LPN,
                Tipo = entity.TP_LPN_TIPO,
                IdAtributo = entity.ID_ATRIBUTO,
                NombreAtributo = entity.T_ATRIBUTO?.NM_ATRIBUTO,
                ValorAtributo = entity.VL_LPN_DET_ATRIBUTO,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Empresa = entity.CD_EMPRESA,
                Lote = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Estado = entity.ID_ESTADO
            };
        }

        public virtual T_LPN_DET_ATRIBUTO MapToEntity(LpnDetalleAtributo obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new T_LPN_DET_ATRIBUTO
            {
                ID_LPN_DET = obj.IdLpnDetalle,
                NU_LPN = obj.NumeroLpn,
                TP_LPN_TIPO = obj.Tipo,
                ID_ATRIBUTO = obj.IdAtributo,
                VL_LPN_DET_ATRIBUTO = obj.ValorAtributo,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,

                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                CD_EMPRESA = obj.Empresa,
                NU_IDENTIFICADOR = obj.Lote?.Trim()?.ToUpper(),
                ID_ESTADO = obj.Estado
            };
        }

        public virtual LpnTipoAtributoDet MapToObject(T_LPN_TIPO_ATRIBUTO_DET entity)
        {
            if (entity == null)
                return null;

            return new LpnTipoAtributoDet
            {
                TipoLpn = entity.TP_LPN_TIPO,
                IdAtributo = entity.ID_ATRIBUTO,
                ValorInicial = entity.VL_INICIAL,
                Requerido = entity.FL_REQUERIDO,
                ValidoInterfaz = entity.VL_VALIDO_INTERFAZ,
                Orden = entity.NU_ORDEN,
                EstadoInicial = entity.ID_ESTADO_INICIAL
            };
        }

        public virtual T_LPN_TIPO_ATRIBUTO_DET MapToEntity(LpnTipoAtributoDet obj)
        {
            if (obj == null)
                return null;

            return new T_LPN_TIPO_ATRIBUTO_DET
            {
                TP_LPN_TIPO = obj.TipoLpn,
                ID_ATRIBUTO = obj.IdAtributo,
                VL_INICIAL = obj.ValorInicial,
                FL_REQUERIDO = obj.Requerido,
                VL_VALIDO_INTERFAZ = obj.ValidoInterfaz,
                NU_ORDEN = obj.Orden,
                ID_ESTADO_INICIAL = obj.EstadoInicial
            };
        }

        public virtual DetallePreparacionLpn MapToObject(T_DET_PICKING_LPN entity)
        {
            if (entity == null) return null;

            return new DetallePreparacionLpn()
            {
                NroPreparacion = entity.NU_PREPARACION,
                IdDetallePickingLpn = entity.ID_DET_PICKING_LPN,
                IdDetalleLpn = entity.ID_LPN_DET,
                NroLpn = entity.NU_LPN,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Lote = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Atributos = entity.VL_ATRIBUTOS,
                CantidadReservada = entity.QT_RESERVA,
                TipoLpn = entity.TP_LPN_TIPO,
                Ubicacion = entity.CD_ENDERECO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
                IdExternoLpn = entity.ID_LPN_EXTERNO,
                IdConfiguracion = entity.NU_DET_PED_SAI_ATRIB,
            };
        }

        public virtual T_DET_PICKING_LPN MapToEntity(DetallePreparacionLpn obj)
        {
            if (obj == null) return null;

            return new T_DET_PICKING_LPN()
            {
                NU_PREPARACION = obj.NroPreparacion,
                ID_DET_PICKING_LPN = obj.IdDetallePickingLpn,
                ID_LPN_DET = obj.IdDetalleLpn,
                NU_LPN = obj.NroLpn,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Lote?.Trim()?.ToUpper(),
                VL_ATRIBUTOS = obj.Atributos,
                QT_RESERVA = obj.CantidadReservada,
                TP_LPN_TIPO = obj.TipoLpn,
                CD_ENDERECO = obj.Ubicacion,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
                NU_DET_PED_SAI_ATRIB = obj.IdConfiguracion,
                ID_LPN_EXTERNO = obj.IdExternoLpn
            };
        }

        public virtual DetallePedidoLpn MapToObject(T_DET_PEDIDO_SAIDA_LPN entity)
        {
            if (entity == null) return null;

            return new DetallePedidoLpn()
            {
                Pedido = entity.NU_PEDIDO,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdEspecificaIdentificador = entity.ID_ESPECIFICA_IDENTIFICADOR,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                IdLpnExterno = entity.ID_LPN_EXTERNO,
                Tipo = entity.TP_LPN_TIPO,
                CantidadPedida = entity.QT_PEDIDO,
                CantidadLiberada = entity.QT_LIBERADO,
                CantidadAnulada = entity.QT_ANULADO,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
                NumeroLpn = entity.NU_LPN,
            };
        }

        public virtual T_DET_PEDIDO_SAIDA_LPN MapToEntity(DetallePedidoLpn obj)
        {
            if (obj == null) return null;

            return new T_DET_PEDIDO_SAIDA_LPN()
            {
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = obj.IdEspecificaIdentificador,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                ID_LPN_EXTERNO = obj.IdLpnExterno,
                TP_LPN_TIPO = obj.Tipo,
                QT_PEDIDO = obj.CantidadPedida,
                QT_LIBERADO = obj.CantidadLiberada,
                QT_ANULADO = obj.CantidadAnulada,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
                NU_LPN = obj.NumeroLpn,
            };
        }

        public virtual DetallePedidoLpnAtributo MapToObject(T_DET_PEDIDO_SAIDA_LPN_ATRIB entity)
        {
            if (entity == null) return null;

            return new DetallePedidoLpnAtributo()
            {
                Pedido = entity.NU_PEDIDO,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdEspecificaIdentificador = entity.ID_ESPECIFICA_IDENTIFICADOR,
                Tipo = entity.TP_LPN_TIPO,
                IdLpnExterno = entity.ID_LPN_EXTERNO,
                IdConfiguracion = entity.NU_DET_PED_SAI_ATRIB,
                CantidadPedida = entity.QT_PEDIDO,
                CantidadLiberada = entity.QT_LIBERADO,
                CantidadAnulada = entity.QT_ANULADO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual T_DET_PEDIDO_SAIDA_LPN_ATRIB MapToEntity(DetallePedidoLpnAtributo obj)
        {
            if (obj == null) return null;

            return new T_DET_PEDIDO_SAIDA_LPN_ATRIB()
            {
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = obj.IdEspecificaIdentificador,
                TP_LPN_TIPO = obj.Tipo,
                ID_LPN_EXTERNO = obj.IdLpnExterno,
                NU_DET_PED_SAI_ATRIB = obj.IdConfiguracion,
                QT_PEDIDO = obj.CantidadPedida,
                QT_LIBERADO = obj.CantidadLiberada,
                QT_ANULADO = obj.CantidadAnulada,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
            };
        }

        public virtual DetallePedidoAtributo MapToObject(T_DET_PEDIDO_SAIDA_ATRIB entity)
        {
            if (entity == null) return null;

            return new DetallePedidoAtributo()
            {
                Pedido = entity.NU_PEDIDO,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdEspecificaIdentificador = entity.ID_ESPECIFICA_IDENTIFICADOR,
                IdConfiguracion = entity.NU_DET_PED_SAI_ATRIB,
                CantidadPedida = entity.QT_PEDIDO,
                CantidadLiberada = entity.QT_LIBERADO,
                CantidadAnulada = entity.QT_ANULADO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual T_DET_PEDIDO_SAIDA_ATRIB MapToEntity(DetallePedidoAtributo obj)
        {
            if (obj == null) return null;

            return new T_DET_PEDIDO_SAIDA_ATRIB()
            {
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = obj.IdEspecificaIdentificador,
                NU_DET_PED_SAI_ATRIB = obj.IdConfiguracion,
                QT_PEDIDO = obj.CantidadPedida,
                QT_LIBERADO = obj.CantidadLiberada,
                QT_ANULADO = obj.CantidadAnulada,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
            };
        }

        public virtual DetallePedidoAtributoDefinicion MapToObject(T_DET_PEDIDO_SAIDA_ATRIB_DET entity)
        {
            if (entity == null) return null;

            return new DetallePedidoAtributoDefinicion()
            {
                IdConfiguracion = entity.NU_DET_PED_SAI_ATRIB,
                IdAtributo = entity.ID_ATRIBUTO,
                IdCabezal = entity.FL_CABEZAL,
                Valor = entity.VL_ATRIBUTO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual T_DET_PEDIDO_SAIDA_ATRIB_DET MapToEntity(DetallePedidoAtributoDefinicion obj)
        {
            if (obj == null) return null;

            return new T_DET_PEDIDO_SAIDA_ATRIB_DET()
            {
                NU_DET_PED_SAI_ATRIB = obj.IdConfiguracion,
                ID_ATRIBUTO = obj.IdAtributo,
                FL_CABEZAL = obj.IdCabezal,
                VL_ATRIBUTO = obj.Valor,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
            };
        }

        public virtual DetallePedidoAtributoTemporal MapToObject(T_TEMP_DET_PEDIDO_SAIDA_ATRIB entity)
        {
            if (entity == null) return null;

            return new DetallePedidoAtributoTemporal()
            {
                Pedido = entity.NU_PEDIDO,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdEspecificaIdentificador = entity.ID_ESPECIFICA_IDENTIFICADOR,
                IdAtributo = entity.ID_ATRIBUTO,
                UserId = entity.USERID,
                IdCabezal = entity.FL_CABEZAL,
                Valor = entity.VL_ATRIBUTO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual T_TEMP_DET_PEDIDO_SAIDA_ATRIB MapToEntity(DetallePedidoAtributoTemporal obj)
        {
            if (obj == null) return null;

            return new T_TEMP_DET_PEDIDO_SAIDA_ATRIB()
            {
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = obj.IdEspecificaIdentificador,
                ID_ATRIBUTO = obj.IdAtributo,
                USERID = obj.UserId,
                FL_CABEZAL = obj.IdCabezal,
                VL_ATRIBUTO = obj.Valor,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
            };
        }

        public virtual DetallePedidoAtributoLpnTemporal MapToObject(T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB entity)
        {
            if (entity == null) return null;

            return new DetallePedidoAtributoLpnTemporal()
            {
                Pedido = entity.NU_PEDIDO,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                IdEspecificaIdentificador = entity.ID_ESPECIFICA_IDENTIFICADOR,
                TipoLpn = entity.TP_LPN_TIPO,
                IdExternoLpn = entity.ID_LPN_EXTERNO,
                IdAtributo = entity.ID_ATRIBUTO,
                UserId = entity.USERID,
                IdCabezal = entity.FL_CABEZAL,
                Valor = entity.VL_ATRIBUTO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB MapToEntity(DetallePedidoAtributoLpnTemporal obj)
        {
            if (obj == null) return null;

            return new T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB()
            {
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = obj.IdEspecificaIdentificador,
                TP_LPN_TIPO = obj.TipoLpn,
                ID_LPN_EXTERNO = obj.IdExternoLpn,
                ID_ATRIBUTO = obj.IdAtributo,
                USERID = obj.UserId,
                FL_CABEZAL = obj.IdCabezal,
                VL_ATRIBUTO = obj.Valor,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
            };
        }

    }
}
