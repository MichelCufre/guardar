using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class T_ARCHIVO_MANEJO_DOCUMENTO
    {
        [Key]
        [Column]
        public string CD_MANEJO { get; set; }
        [Key]
        [Column]
        public string CD_DOCUMENTO { get; set; }
        public virtual T_ARCHIVO_MANEJO T_ARCHIVO_MANEJO { get; set; }
        public virtual T_ARCHIVO_DOCUMENTO T_ARCHIVO_DOCUMENTO { get; set; }
    }
}
