using WIS.Domain.Documento.Reserva;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
    public class DocumentoEntradaProduccionReservaMapper
    {
        public virtual DocumentoProduccionEntrada MapToEntradaProduccionReserva(T_DOCUMENTO_PRDC_ENTRADA entradaEntity)
        {
            if (entradaEntity == null)
                return null;

            DocumentoProduccionEntrada entrada = new DocumentoProduccionEntrada();

            SetPropertiesEntradaProduccionReserva(entrada, entradaEntity);

            return entrada;
        }

        public virtual T_DOCUMENTO_PRDC_ENTRADA MapFromEntradaProduccionReserva(DocumentoProduccionEntrada anulacion)
        {
            return CreateEntityAnulacionPreparacionReserva(anulacion);
        }

        public virtual void SetPropertiesEntradaProduccionReserva(DocumentoProduccionEntrada entrada, T_DOCUMENTO_PRDC_ENTRADA entradaEntity)
        {
            entrada.NumeroPreparacion = entradaEntity.NU_PREPARACION;
            entrada.NumeroPedido = entradaEntity.NU_PEDIDO;
            entrada.NumeroContenedor = entradaEntity.NU_CONTENEDOR;
            entrada.Empresa = entradaEntity.CD_EMPRESA;
            entrada.Producto = entradaEntity.CD_PRODUTO;
            entrada.Faixa = entradaEntity.CD_FAIXA;
            entrada.Identificador = entradaEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            entrada.CantidadReservada = entradaEntity.QT_PRODUTO;
        }

        public virtual T_DOCUMENTO_PRDC_ENTRADA CreateEntityAnulacionPreparacionReserva(DocumentoProduccionEntrada entrada)
        {
            return new T_DOCUMENTO_PRDC_ENTRADA()
            {
                NU_PREPARACION = entrada.NumeroPreparacion,
                NU_PEDIDO = entrada.NumeroPedido,
                NU_CONTENEDOR = entrada.NumeroContenedor,
                CD_EMPRESA = entrada.Empresa,
                CD_PRODUTO = entrada.Producto,
                CD_FAIXA = entrada.Faixa,
                NU_IDENTIFICADOR = entrada.Identificador?.Trim()?.ToUpper(),
                QT_PRODUTO = entrada.CantidadReservada
            };
        }
    }
}
