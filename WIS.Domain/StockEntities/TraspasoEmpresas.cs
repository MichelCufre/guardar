using System;

namespace WIS.Domain.StockEntities
{
    public class TraspasoEmpresas
    {
        public long Id { get; set; } // NU_TRASPASO

        public string Descripcion { get; set; } // DS_TRASPASO

        public string IdExterno { get; set; } // ID_TRASPASO_EXTERNO

        public int EmpresaOrigen { get; set; } // CD_EMPRESA

        public int EmpresaDestino { get; set; } // CD_EMPRESA_DESTINO

        public string TipoTraspaso { get; set; } // TP_TRASPASO

        public string Estado { get; set; } // ID_ESTADO

        public string DocumentoIngreso { get; set; } // NU_DOCUMENTO_INGRESO

        public string TipoDocumentoIngreso { get; set; } // TP_DOCUMENTO_INGRESO

        public string DocumentoEgreso { get; set; } // NU_DOCUMENTO_EGRESO

        public string TipoDocumentoEgreso { get; set; } // TP_DOCUMENTO_EGRESO

        public int? Preparacion { get; set; } // NU_PREPARACION

        public bool FinalizarConPreparacion { get; set; } // FL_FINALIZAR_CON_PREPARACION

        public bool PropagarLPN { get; set; } // FL_PROPAGAR_LPN

        public bool GeneraCabezalAuto { get; set; } // FL_GENERACION_AUTO_CABEZAL

        public bool ReplicaProductos { get; set; } // FL_REPLICA_PRODUCTOS

        public bool ReplicaCodBarras { get; set; } // FL_REPLICA_CODIGOS_BARRAS

        public bool ControlaCaractIguales { get; set; } // FL_CTRL_CARACT_IGUALES

        public bool ReplicaAgentes { get; set; } // FL_REPLICA_AGENTES

        public string ConfigPedidoDestino { get; set; } // DS_CONFIG_PEDIDO_DESTINO

        public int? PreparacionDestino { get; set; } // NU_PREPARACION_DESTINO

        public DateTime? FechaAlta { get; set; } // DT_ADDROW

        public DateTime? FechaModificacion { get; set; } // DT_UPDROW
    }
}
