using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
    public class DocumentoAnulacionReservaMapper
    {
        public virtual DocumentoAnulacionPreparacionReserva MapToAnulacionPreparacionReserva(T_DOCUMENTO_ANU_PREP_RESERVA anulacionEntity)
        {
            if (anulacionEntity == null)
                return null;

            DocumentoAnulacionPreparacionReserva anulacion = new DocumentoAnulacionPreparacionReserva();

            SetPropertiesAnulacionPreparacionReserva(anulacion, anulacionEntity);

            return anulacion;
        }

        public virtual T_DOCUMENTO_ANU_PREP_RESERVA MapFromAnulacionPreparacionReserva(DocumentoAnulacionPreparacionReserva anulacion)
        {
            return CreateEntityAnulacionPreparacionReserva(anulacion);
        }

        public virtual void SetPropertiesAnulacionPreparacionReserva(DocumentoAnulacionPreparacionReserva anulacion, T_DOCUMENTO_ANU_PREP_RESERVA anulacionEntity)
        {
            anulacion.CantidadAnular = anulacionEntity.QT_ANULAR;
            anulacion.Empresa = anulacionEntity.CD_EMPRESA;
            anulacion.Estado = anulacionEntity.ID_ESTADO;
            anulacion.Faixa = anulacionEntity.CD_FAIXA;
            anulacion.Identificador = anulacionEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            anulacion.IdentificadorAnulacion = anulacionEntity.ID_ANULACION;
            anulacion.EspecificaIdentificador = anulacionEntity.ID_ESPECIFICA_IDENTIFICADOR;
            anulacion.NumeroPreparacion = anulacionEntity.NU_PREPARACION;
            anulacion.Producto = anulacionEntity.CD_PRODUTO;
            anulacion.FechaAlta = anulacionEntity.DT_ADDROW;
            anulacion.FechaModificacion = anulacionEntity.DT_UPDATEROW;
        }

        public virtual T_DOCUMENTO_ANU_PREP_RESERVA CreateEntityAnulacionPreparacionReserva(DocumentoAnulacionPreparacionReserva anulacion)
        {
            return new T_DOCUMENTO_ANU_PREP_RESERVA()
            {
                CD_EMPRESA = anulacion.Empresa,
                CD_FAIXA = anulacion.Faixa,
                CD_PRODUTO = anulacion.Producto,
                ID_ANULACION = anulacion.IdentificadorAnulacion,
                ID_ESPECIFICA_IDENTIFICADOR = anulacion.EspecificaIdentificador,
                ID_ESTADO = anulacion.Estado,
                NU_IDENTIFICADOR = anulacion.Identificador?.Trim()?.ToUpper(),
                NU_PREPARACION = anulacion.NumeroPreparacion,
                QT_ANULAR = anulacion.CantidadAnular,
                DT_ADDROW = anulacion.FechaAlta,
                DT_UPDATEROW = anulacion.FechaModificacion
            };
        }
    }
}
