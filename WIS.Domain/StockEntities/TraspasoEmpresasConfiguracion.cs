using System;

namespace WIS.Domain.StockEntities
{
    public class TraspasoEmpresasConfiguracion
    {
        public long Id { get; set; } // NU_TRASPASO_CONFIGURACION

        public int EmpresaOrigen { get; set; } // CD_EMPRESA

        public bool TodaEmpresa { get; set; } // FL_TODA_EMPRESA

        public bool TodoTipoTraspaso { get; set; } // FL_TODO_TIPO_TRASPASO

        public bool GeneraCabezalAuto { get; set; } // FL_GENERACION_AUTO_CABEZAL

        public bool ReplicaProductos { get; set; } // FL_REPLICA_PRODUCTOS

        public bool ReplicaCodBarras { get; set; } // FL_REPLICA_CODIGOS_BARRAS

        public bool ControlaCaractIguales { get; set; } // FL_CTRL_CARACT_IGUALES

        public bool ReplicaAgentes { get; set; } // FL_REPLICA_AGENTES

        public string TipoDocumentoIngreso { get; set; } // CD_TIPO_DOCUMENTO_INGRESO

        public string TipoDocumentoEgreso { get; set; } // CD_TIPO_DOCUMENTO_EGRESO

        public DateTime? FechaAlta { get; set; } //DT_ADDROW

        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
    }
}
