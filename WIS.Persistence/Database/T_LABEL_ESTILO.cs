namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("T_LABEL_ESTILO")]
	public partial class T_LABEL_ESTILO
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public T_LABEL_ESTILO()
		{
			T_LABEL_TEMPLATE = new HashSet<T_LABEL_TEMPLATE>();
			T_REL_LABELESTILO_TIPOCONT = new HashSet<T_REL_LABELESTILO_TIPOCONT>();
		}

		[Key]
		[StringLength(15)]
		[Column]
		public string CD_LABEL_ESTILO { get; set; }

		[StringLength(30)]
		[Column]
		public string DS_LABEL_ESTILO { get; set; }

		[Required]
		[StringLength(20)]
		[Column]
		public string TP_LABEL { get; set; }

		[StringLength(1)]
		[Column]
		public string FL_HABILITADO { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<T_LABEL_TEMPLATE> T_LABEL_TEMPLATE { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<T_REL_LABELESTILO_TIPOCONT> T_REL_LABELESTILO_TIPOCONT { get; set; }
	}
}
