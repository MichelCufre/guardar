using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
    public class DocumentoTipoMapper : Mapper
    {
        public virtual DocumentoTipo MapToDocumentoTipo(T_DOCUMENTO_TIPO documentoTipoEntity)
        {
            DocumentoTipo documentoTipo = new DocumentoTipo();

            SetDocumentoTipoProperties(documentoTipo, documentoTipoEntity);

            return documentoTipo;
        }

        public virtual void SetDocumentoTipoProperties(DocumentoTipo documentoTipo, T_DOCUMENTO_TIPO documentoTipoEntity)
        {
            if (documentoTipoEntity == null)
                return;

            documentoTipo.Habilitado = MapStringToBoolean(documentoTipoEntity.FL_HABILITADO);
            documentoTipo.IngresoManual = MapStringToBoolean(documentoTipoEntity.FL_INGRESO_MANUAL);
            documentoTipo.NumeroAutogenerado = MapStringToBoolean(documentoTipoEntity.FL_NUMERO_AUTOGENERADO);
            documentoTipo.TipoDocumento = documentoTipoEntity.TP_DOCUMENTO;
            documentoTipo.TipoOperacion = documentoTipoEntity.TP_OPERACION;
            documentoTipo.ManejaCambioEstado = MapStringToBoolean(documentoTipoEntity.FL_MANEJA_CAMBIO_ESTADO);
            documentoTipo.RequiereDUA = MapStringToBoolean(documentoTipoEntity.FL_REQUIERE_DUA);
            documentoTipo.RequiereDTI = MapStringToBoolean(documentoTipoEntity.FL_REQUIERE_DTI);
            documentoTipo.RequiereReferenciaExterna = MapStringToBoolean(documentoTipoEntity.FL_REQUIERE_REFERENCIA_EXTERNA);
            documentoTipo.DescripcionTipoDocumento = documentoTipoEntity.DS_TP_DOCUMENTO;
            documentoTipo.RequiereFactura = MapStringToBoolean(documentoTipoEntity.FL_REQUIERE_FACTURA);
            documentoTipo.AutoAgendable = MapStringToBoolean(documentoTipoEntity.FL_AUTOAGENDABLE);
            documentoTipo.ManejaAgenda = MapStringToBoolean(documentoTipoEntity.FL_MANEJA_AGENDA);
            documentoTipo.ManejaCamion = MapStringToBoolean(documentoTipoEntity.FL_MANEJA_CAMION);
            documentoTipo.Secuencia = documentoTipoEntity.NM_SECUENCIA;
            documentoTipo.PermiteAgregarDetalle = MapStringToBoolean(documentoTipoEntity.FL_PERMITE_AGREGAR_DETALLE);
            documentoTipo.PermiteRemoverDetalle = MapStringToBoolean(documentoTipoEntity.FL_PERMITE_REMOVER_DETALLE);
            documentoTipo.Mask = documentoTipoEntity.VL_MASK;
            documentoTipo.MaskChars = documentoTipoEntity.VL_MASK_CHARS;
            documentoTipo.InterfazEntradaHabilitada = MapStringToBoolean(documentoTipoEntity.FL_IE_HABILITADA);
            documentoTipo.LargoMaximoNumeroDocumento = documentoTipoEntity.VL_LARGO_MAX_NU_DOCUMENTO;
            documentoTipo.LargoPrefijo = documentoTipoEntity.VL_LARGO_PREFIJO;
            documentoTipo.CantidadMinimaIngresada = documentoTipoEntity.QT_MIN_INGRESADA;
        }

    }
}
