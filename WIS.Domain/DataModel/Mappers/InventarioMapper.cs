using WIS.Domain.Inventario;
using WIS.Domain.Inventario.Factories;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class InventarioMapper : Mapper
    {
        public InventarioMapper()
        {
        }

        public virtual IInventario MapToInventario(T_INVENTARIO entity)
        {
            if (entity == null)
                return null;

            var obj = InventarioFactory.Create(entity.TP_INVENTARIO);

            obj.Empresa = entity.CD_EMPRESA;
            obj.Predio = entity.NU_PREDIO;
            obj.Descripcion = entity.DS_INVENTARIO;
            obj.FechaAlta = entity.DT_ADDROW;

            obj.ActualizarConteoFin = this.MapStringToBoolean(entity.FL_ACTUALIZAR_CONTEO_FIN_AUTO);
            obj.BloquearConteoConsecutivoUsuario = this.MapStringToBoolean(entity.FL_BLOQ_USR_CONTEO_SUCESIVO);
            obj.ControlarVencimiento = this.MapStringToBoolean(entity.FL_CONTROLAR_VENCIMIENTO);
            obj.IsCreacionWeb = this.MapStringToBoolean(entity.FL_CREACION_WEB);
            obj.ModificarStockEnDiferencia = this.MapStringToBoolean(entity.FL_MODIFICAR_STOCK_EN_DIF);
            obj.PermiteIngresarMotivo = this.MapStringToBoolean(entity.FL_PERMITE_INGRESAR_MOTIVO);
            obj.SoloRegistroFoto = this.MapStringToBoolean(entity.FL_SOLO_REGISTROS_FOTO);
            obj.ExcluirSueltos = this.MapStringToBoolean(entity.FL_EXCLUIR_SUELTOS);
            obj.ExcluirLpns = this.MapStringToBoolean(entity.FL_EXCLUIR_LPNS);
            obj.RestablecerLpnFinalizado = this.MapStringToBoolean(entity.FL_RESTABLECER_LPN_FINALIZADO);
            obj.GenerarPrimerConteo = this.MapStringToBoolean(entity.FL_GENERAR_PRIMER_CONTEO);
            obj.PermiteUbicacionesDeOtrosInventarios = this.MapStringToBoolean(entity.FL_PERMITE_ASOC_UBIC_OTRO_INV);

            obj.CierreConteo = entity.ND_CIERRE_CONTEO;
            obj.Estado = entity.ND_ESTADO_INVENTARIO;
            obj.NumeroConteo = entity.NU_CONTEO;
            obj.NumeroInventario = entity.NU_INVENTARIO;
            obj.TipoInventario = entity.TP_INVENTARIO;
            obj.NumeroTransaccion = entity.NU_TRANSACCION;
            obj.NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE;
            obj.FechaModificacion = entity.DT_UPDROW;

            if (entity.T_INVENTARIO_ENDERECO != null && entity.T_INVENTARIO_ENDERECO.Count > 0)
            {
                foreach (var e in entity.T_INVENTARIO_ENDERECO)
                {
                    obj.Ubicaciones.Add(this.MapToInventarioEndereco(e));
                }
            }

            return obj;
        }

        public virtual InventarioUbicacion MapToInventarioEndereco(T_INVENTARIO_ENDERECO entity)
        {
            if (entity == null)
                return null;

            var obj = new InventarioUbicacion
            {
                Id = entity.NU_INVENTARIO_ENDERECO,
                IdInventario = entity.NU_INVENTARIO,
                Estado = entity.ND_ESTADO_INVENTARIO_ENDERECO,
                Ubicacion = entity.CD_ENDERECO,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE
            };

            if (entity.T_INVENTARIO_ENDERECO_DET != null && entity.T_INVENTARIO_ENDERECO_DET.Count > 0)
            {
                foreach (var det in entity.T_INVENTARIO_ENDERECO_DET)
                {
                    obj.Detalles.Add(this.MapToInventarioEnderecoDetalle(det));
                }
            }

            return obj;
        }

        public virtual InventarioUbicacionDetalle MapToInventarioEnderecoDetalle(T_INVENTARIO_ENDERECO_DET entity)
        {
            if (entity == null)
                return null;

            var obj = new InventarioUbicacionDetalle
            {
                Id = entity.NU_INVENTARIO_ENDERECO_DET,
                IdInventarioUbicacion = entity.NU_INVENTARIO_ENDERECO,
                NuConteoDetalle = entity.NU_CONTEO,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                CantidadInventario = entity.QT_INVENTARIO,
                CantidadDiferencia = entity.QT_DIFERENCIA,
                Vencimiento = entity.DT_VENCIMIENTO,
                Estado = entity.ND_ESTADO_INV_ENDERECO_DET,
                UserId = entity.CD_USUARIO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                TiempoInsumido = entity.QT_TIEMPO_INSUMIDO,
                MotivoAjuste = entity.CD_MOTIVO_AJUSTE,
                NuInstanciaConteo = entity.NU_INSTANCIA_CONTEO,
                Faixa = entity.CD_FAIXA,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                NumeroLPN = entity.NU_LPN,
                IdDetalleLPN = entity.ID_LPN_DET,
                ConteoEsperado = entity.FL_CONTEO_ESPERADO,
            };

            if (entity.T_INVENTARIO_ENDERECO != null)
            {
                obj.InventarioUbicacion.Id = entity.T_INVENTARIO_ENDERECO.NU_INVENTARIO;
                obj.InventarioUbicacion.IdInventario = entity.T_INVENTARIO_ENDERECO.NU_INVENTARIO_ENDERECO;
                obj.InventarioUbicacion.Estado = entity.T_INVENTARIO_ENDERECO.ND_ESTADO_INVENTARIO_ENDERECO;
                obj.InventarioUbicacion.Ubicacion = entity.T_INVENTARIO_ENDERECO.CD_ENDERECO;
                obj.InventarioUbicacion.NumeroTransaccion = entity.T_INVENTARIO_ENDERECO.NU_TRANSACCION;
                obj.InventarioUbicacion.NumeroTransaccionDelete = entity.T_INVENTARIO_ENDERECO.NU_TRANSACCION_DELETE;
                obj.InventarioUbicacion.NumeroTransaccionDelete = entity.T_INVENTARIO_ENDERECO.NU_TRANSACCION_DELETE;
            }

            return obj;
        }

        public virtual InventarioUbicacionDetalleAtributo Map(T_INVENTARIO_ENDERECO_DET_ATR entity)
        {
            if (entity == null)
                return null;

            return new InventarioUbicacionDetalleAtributo
            {
                Id = entity.NU_INVENTARIO_ENDERECO_DET,
                IdAtributo = entity.ID_ATRIBUTO,
                Valor = entity.VL_ATRIBUTO,
            };
        }

        public virtual T_INVENTARIO MapFromInventario(IInventario obj, bool addNavegables = true)
        {
            if (obj == null)
                return null;

            var entity = new T_INVENTARIO
            {
                CD_EMPRESA = obj.Empresa,
                DS_INVENTARIO = obj.Descripcion,
                NU_PREDIO = obj.Predio,
                DT_ADDROW = obj.FechaAlta,
                ND_CIERRE_CONTEO = obj.CierreConteo,
                ND_ESTADO_INVENTARIO = obj.Estado,
                NU_CONTEO = obj.NumeroConteo,
                NU_INVENTARIO = obj.NumeroInventario,
                TP_INVENTARIO = obj.TipoInventario,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                DT_UPDROW = obj.FechaModificacion,

                FL_ACTUALIZAR_CONTEO_FIN_AUTO = this.MapBooleanToString(obj.ActualizarConteoFin),
                FL_BLOQ_USR_CONTEO_SUCESIVO = this.MapBooleanToString(obj.BloquearConteoConsecutivoUsuario),
                FL_CONTROLAR_VENCIMIENTO = this.MapBooleanToString(obj.ControlarVencimiento),
                FL_CREACION_WEB = this.MapBooleanToString(obj.IsCreacionWeb),
                FL_MODIFICAR_STOCK_EN_DIF = this.MapBooleanToString(obj.ModificarStockEnDiferencia),
                FL_PERMITE_INGRESAR_MOTIVO = this.MapBooleanToString(obj.PermiteIngresarMotivo),
                FL_SOLO_REGISTROS_FOTO = this.MapBooleanToString(obj.SoloRegistroFoto),
                FL_EXCLUIR_SUELTOS = this.MapBooleanToString(obj.ExcluirSueltos),
                FL_EXCLUIR_LPNS = this.MapBooleanToString(obj.ExcluirLpns),
                FL_RESTABLECER_LPN_FINALIZADO = this.MapBooleanToString(obj.RestablecerLpnFinalizado),
                FL_GENERAR_PRIMER_CONTEO = this.MapBooleanToString(obj.GenerarPrimerConteo),
                FL_PERMITE_ASOC_UBIC_OTRO_INV = this.MapBooleanToString(obj.PermiteUbicacionesDeOtrosInventarios),
            };

            if (addNavegables && obj.Ubicaciones != null && obj.Ubicaciones.Count > 0)
            {
                foreach (var i in obj.Ubicaciones)
                {
                    i.NumeroTransaccion = obj.NumeroTransaccion;
                    i.NumeroTransaccionDelete = obj.NumeroTransaccionDelete;

                    entity.T_INVENTARIO_ENDERECO.Add(this.MapFromInventarioEndereco(i));
                }
            }

            return entity;
        }

        public virtual T_INVENTARIO_ENDERECO MapFromInventarioEndereco(InventarioUbicacion obj)
        {
            if (obj == null)
                return null;

            return new T_INVENTARIO_ENDERECO
            {
                NU_INVENTARIO_ENDERECO = obj.Id,
                NU_INVENTARIO = obj.IdInventario,
                ND_ESTADO_INVENTARIO_ENDERECO = obj.Estado,
                CD_ENDERECO = obj.Ubicacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete
            };

        }

        public virtual T_INVENTARIO_ENDERECO_DET MapFromInventarioEnderecoDetalle(InventarioUbicacionDetalle obj, bool addNavegables = false)
        {
            if (obj == null)
                return null;

            var entity = new T_INVENTARIO_ENDERECO_DET
            {
                NU_INVENTARIO_ENDERECO_DET = obj.Id,
                NU_INVENTARIO_ENDERECO = obj.IdInventarioUbicacion,
                NU_CONTEO = obj.NuConteoDetalle,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                QT_INVENTARIO = obj.CantidadInventario,
                QT_DIFERENCIA = obj.CantidadDiferencia,
                DT_VENCIMIENTO = obj.Vencimiento,
                ND_ESTADO_INV_ENDERECO_DET = obj.Estado,
                CD_USUARIO = obj.UserId,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                QT_TIEMPO_INSUMIDO = obj.TiempoInsumido,
                CD_MOTIVO_AJUSTE = obj.MotivoAjuste,
                NU_INSTANCIA_CONTEO = obj.NuInstanciaConteo,
                CD_FAIXA = obj.Faixa,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                NU_LPN = obj.NumeroLPN,
                ID_LPN_DET = obj.IdDetalleLPN,
                FL_CONTEO_ESPERADO = obj.ConteoEsperado
            };

            if (addNavegables && obj.InventarioUbicacion != null)
            {
                entity.T_INVENTARIO_ENDERECO = this.MapFromInventarioEndereco(obj.InventarioUbicacion);
            }

            return entity;
        }

        public virtual T_INVENTARIO_ENDERECO_DET_ATR Map(InventarioUbicacionDetalleAtributo obj)
        {
            if (obj == null)
                return null;

            return new T_INVENTARIO_ENDERECO_DET_ATR
            {
                NU_INVENTARIO_ENDERECO_DET = obj.Id,
                ID_ATRIBUTO = obj.IdAtributo,
                VL_ATRIBUTO = obj.Valor,
            };
        }

        public virtual InventarioSelectRegistroLpn Map(V_INV417_REG_DISP entity)
        {
            if (entity == null) return null;

            return new InventarioSelectRegistroLpn()
            {
                NuInventario = entity.NU_INVENTARIO,
                Ubicacion = entity.CD_ENDERECO,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NroLpn = entity.NU_LPN,
                IdDetalleLpn = entity.ID_LPN_DET,
                Cantidad = entity.QT_ESTOQUE.Value,
                Vencimiento = entity.DT_FABRICACAO
            };
        }
    }
}
