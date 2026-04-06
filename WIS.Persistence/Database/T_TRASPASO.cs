using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{

    [Table("T_TRASPASO")]
    public partial class T_TRASPASO
    {
        public long NU_TRASPASO { get; set; }

        [StringLength(100)]
        public string DS_TRASPASO { get; set; }

        [Required]
        [StringLength(18)]
        public string ID_TRASPASO_EXTERNO { get; set; }

        public int CD_EMPRESA { get; set; }

        public int CD_EMPRESA_DESTINO { get; set; }

        [Required]
        [StringLength(50)]
        public string TP_TRASPASO { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_ESTADO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_EGRESO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_EGRESO { get; set; }

        public int? NU_PREPARACION { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_FINALIZAR_CON_PREPARACION { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_PROPAGAR_LPN { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_GENERACION_AUTO_CABEZAL { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_REPLICA_PRODUCTOS { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_REPLICA_CODIGOS_BARRAS { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_CTRL_CARACT_IGUALES { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_REPLICA_AGENTES { get; set; }

        [StringLength(50)]
        public string DS_CONFIG_PEDIDO_DESTINO { get; set; }

        public int? NU_PREPARACION_DESTINO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
