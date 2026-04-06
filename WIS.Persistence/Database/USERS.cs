namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("USERS")]
    public partial class USERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USERS()
        {
            T_EMPRESA_FUNCIONARIO = new HashSet<T_EMPRESA_FUNCIONARIO>();
            USERPERMISSIONS = new HashSet<USERPERMISSIONS>();
            T_GRUPO_CONSULTA_FUNCIONARIO = new HashSet<T_GRUPO_CONSULTA_FUNCIONARIO>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        [StringLength(50)]
        [Column]
        public string DOMAINNAME { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        [StringLength(100)]
        [Column]
        public string EMAIL { get; set; }

        public short ISENABLED { get; set; }

        public int? USERTYPEID { get; set; }

        [StringLength(3)]
        [Column]
        public string LANGUAGE { get; set; }

        [StringLength(40)]
        [Column]
        public string SESSIONTOKEN { get; set; }

        [StringLength(40)]
        [Column]
        public string SESSIONTOKENWEB { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EMPRESA_FUNCIONARIO> T_EMPRESA_FUNCIONARIO { get; set; }

        public virtual USER_TOKEN USER_TOKEN { get; set; }

        public virtual USERDATA USERDATA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USERPERMISSIONS> USERPERMISSIONS { get; set; }

        public virtual USERTYPES USERTYPES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_GRUPO_CONSULTA_FUNCIONARIO> T_GRUPO_CONSULTA_FUNCIONARIO { get; set; }

        public virtual T_USUARIO_CONFIGURACION T_USUARIO_CONFIGURACION { get; set; }
    }
}
