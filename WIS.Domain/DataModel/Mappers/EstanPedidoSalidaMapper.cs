using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EstanPedidoSalidaMapper : Mapper
    {
        public virtual EstanPedidoSalida MapToObject(I_E_ESTAN_PEDIDO_SAIDA entity)
        {
            if (entity == null)
                return null;

            return new EstanPedidoSalida
            {
                IdProcesado = entity.ID_PROCESADO,
                Id = entity.NU_INTERFAZ_EJECUCION,
                Registro = entity.NU_REGISTRO,
                Pedido = entity.NU_PEDIDO,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Rota = entity.CD_ROTA,
                DtLiberarDesde = entity.DT_LIBERAR_DESDE,
                DtLiberarHasta = entity.DT_LIBERAR_HASTA,
                DtEntrega = entity.DT_ENTREGA,
                DtEmitido = entity.DT_EMITIDO,
                AddRow = entity.DT_ADDROW,
                Memo = entity.DS_MEMO,
                Memo1 = entity.DS_MEMO_1,
                Origen = entity.CD_ORIGEN,
                CondicionLibreacion = entity.CD_CONDICION_LIBERACION,
                Predio = entity.NU_PREDIO,
                TpPedido = entity.TP_PEDIDO,
                TpExpedicion = entity.TP_EXPEDICION,
                Transportadora = entity.CD_TRANSPORTADORA,
                DsAnexo1 = entity.DS_ANEXO1,
                DsAnexo2 = entity.DS_ANEXO2,
                DsAnexo3 = entity.DS_ANEXO3,
                DsAnexo4 = entity.DS_ANEXO4,
                DescUbicacion = entity.DS_ENDERECO,
                Zona = entity.CD_ZONA,
                PuntoEntrega = entity.CD_PUNTO_ENTREGA,
                AddRowInterfaz = entity.DT_ADDROW_INTERFAZ,
                ModoPedidoNro = entity.ID_MODO_PEDIDO_NRO,
                VlComparteContenedorPicking = entity.VL_COMPARTE_CONTENEDOR_PICKING,
                VlSerializado1 = entity.VL_SERIALIZADO_1,
                DtGenerico1 = entity.DT_GENERICO_1,
                NuGenerico1 = entity.NU_GENERICO_1,
                VlGenerico1 = entity.VL_GENERICO_1,
                VlComparteContenedorEntrega = entity.VL_COMPARTE_CONTENEDOR_ENTREGA
            };
        }

        public virtual EstanPedidoSalidaDet MapToObject(I_E_ESTAN_PEDIDO_SAIDA_DET entity)
        {
            if (entity == null)
                return null;

            return new EstanPedidoSalidaDet
            {
                IdProcesado = entity.ID_PROCESADO,
                Id = entity.NU_INTERFAZ_EJECUCION,
                Registro = entity.NU_REGISTRO,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Pedido = entity.NU_PEDIDO,
                AddRow = entity.DT_ADDROW,
                AddRowInterfaz = entity.DT_ADDROW_INTERFAZ,
                DtGenerico1 = entity.DT_GENERICO_1,
                NuGenerico1 = entity.NU_GENERICO_1,
                VlGenerico1 = entity.VL_GENERICO_1,
                Agrupacion = entity.ID_AGRUPACION,
                DescMemo = entity.DS_MEMO,
                NuIdentificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Producto = entity.CD_PRODUTO,
                QtPedido = entity.QT_PEDIDO,
                VlPorcentajeTolerancia = entity.VL_PORCENTAJE_TOLERANCIA

            };
        }

        public virtual I_E_ESTAN_PEDIDO_SAIDA MapToEntity(EstanPedidoSalida obj)
        {
            if (obj == null)
                return null;

            return new I_E_ESTAN_PEDIDO_SAIDA
            {
                ID_PROCESADO = obj.IdProcesado,
                NU_INTERFAZ_EJECUCION = obj.Id,
                NU_REGISTRO = obj.Registro,
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_ROTA = obj.Rota,
                DT_LIBERAR_DESDE = obj.DtLiberarDesde,
                DT_LIBERAR_HASTA = obj.DtLiberarHasta,
                DT_ENTREGA = obj.DtEntrega,
                DT_EMITIDO = obj.DtEmitido,
                DT_ADDROW = obj.AddRow,
                DS_MEMO = obj.Memo,
                DS_MEMO_1 = obj.Memo1,
                CD_ORIGEN = obj.Origen,
                CD_CONDICION_LIBERACION = obj.CondicionLibreacion,
                NU_PREDIO = obj.Predio,
                TP_PEDIDO = obj.TpPedido,
                TP_EXPEDICION = obj.TpExpedicion,
                CD_TRANSPORTADORA = obj.Transportadora,
                DS_ANEXO1 = obj.DsAnexo1,
                DS_ANEXO2 = obj.DsAnexo2,
                DS_ANEXO3 = obj.DsAnexo3,
                DS_ANEXO4 = obj.DsAnexo4,
                DS_ENDERECO = obj.DescUbicacion,
                CD_ZONA = obj.Zona,
                CD_PUNTO_ENTREGA = obj.PuntoEntrega,
                DT_ADDROW_INTERFAZ = obj.AddRowInterfaz,
                ID_MODO_PEDIDO_NRO = obj.ModoPedidoNro,
                VL_COMPARTE_CONTENEDOR_PICKING = obj.VlComparteContenedorPicking,
                VL_SERIALIZADO_1 = obj.VlSerializado1,
                DT_GENERICO_1 = obj.DtGenerico1,
                NU_GENERICO_1 = obj.NuGenerico1,
                VL_GENERICO_1 = obj.VlGenerico1,
                VL_COMPARTE_CONTENEDOR_ENTREGA = obj.VlComparteContenedorEntrega
            };
        }

        public virtual I_E_ESTAN_PEDIDO_SAIDA_DET MapToEntity(EstanPedidoSalidaDet obj)
        {
            if (obj == null)
                return null;

            return new I_E_ESTAN_PEDIDO_SAIDA_DET
            {
                ID_PROCESADO = obj.IdProcesado,
                NU_INTERFAZ_EJECUCION = obj.Id,
                NU_REGISTRO = obj.Registro,
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                DT_ADDROW = obj.AddRow,
                DS_MEMO = obj.DescMemo,
                DT_GENERICO_1 = obj.DtGenerico1,
                NU_GENERICO_1 = obj.NuGenerico1,
                VL_GENERICO_1 = obj.VlGenerico1,
                CD_PRODUTO = obj.Producto,
                DT_ADDROW_INTERFAZ = obj.AddRowInterfaz,
                ID_AGRUPACION = obj.Agrupacion,
                NU_IDENTIFICADOR = obj.NuIdentificador.Trim(),
                QT_PEDIDO = obj.QtPedido,
                VL_PORCENTAJE_TOLERANCIA = obj.VlPorcentajeTolerancia
            };
        }
    }
}
