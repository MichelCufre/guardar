using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.Recepcion.Enums
{
	public enum ModalidadImpresion
	{
		Unidades,
		Embalajes,
		CampoVirtual,
	}

	public static class ModalidadImpresionExtensions
	{
		public static string ToDisplayString (this ModalidadImpresion modalidad) =>
			modalidad switch
			{
				ModalidadImpresion.Unidades => "REC171_frm1_lbl_Unidades",
				ModalidadImpresion.Embalajes => "REC171_frm1_lbl_Embalajes",
				ModalidadImpresion.CampoVirtual => "REC171_frm1_lbl_CampoVirtual",
				_ => throw new ArgumentOutOfRangeException (),
			};

		public static Dictionary <string, string> StringValues () =>
			Enum.GetValues (typeof (ModalidadImpresion))
				.Cast <ModalidadImpresion> ()
				.ToDictionary (e => e.ToString (), e => e.ToDisplayString ());
	}
}
