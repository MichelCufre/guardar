using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WIS.Domain.Registro
{
	public class UsuarioConfiguracion
	{
		public int Id { get; set; }
		public string ManejaNuevasEmpresas { get; set; }
	}
}
