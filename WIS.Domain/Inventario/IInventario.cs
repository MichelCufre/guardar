using System;
using System.Collections.Generic;

namespace WIS.Domain.Inventario
{
    public interface IInventario
    {
        decimal NumeroInventario { get; set; }
        string Descripcion { get; set; }
        bool ControlarVencimiento { get; set; }
        string Estado { get; set; }
        DateTime? FechaAlta { get; set; }
        string CierreConteo { get; set; }
        bool BloquearConteoConsecutivoUsuario { get; set; }
        int? Empresa { get; set; }
        decimal? NumeroConteo { get; set; }
        bool SoloRegistroFoto { get; set; }
        bool PermiteIngresarMotivo { get; set; }
        bool ModificarStockEnDiferencia { get; set; }
        string TipoInventario { get; set; }
        bool IsCreacionWeb { get; set; }
        bool ActualizarConteoFin { get; set; }
        string Predio { get; set; }
        long? NumeroTransaccion { get; set; }
        long? NumeroTransaccionDelete { get; set; }
        DateTime? FechaModificacion { get; set; }
        bool ExcluirSueltos { get; set; }
        bool ExcluirLpns { get; set; }
        bool RestablecerLpnFinalizado { get; set; }
        bool GenerarPrimerConteo { get; set; }                   //FL_GENERAR_PRIMER_CONTEO
        bool PermiteUbicacionesDeOtrosInventarios { get; set; }  //FL_PERMITE_ASOC_UBIC_OTRO_INV

        List<InventarioUbicacion> Ubicaciones { get; set; }

        bool EnProceso();
        bool IsEditable();
        bool PermiteCancelar();
        bool PermiteRegenerar();
    }
}
