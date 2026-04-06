using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Inventario
{
    public abstract partial class Inventario : IInventario
    {
        public decimal NumeroInventario { get; set; }
        public string Descripcion { get; set; }
        public bool ControlarVencimiento { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string CierreConteo { get; set; }
        public bool BloquearConteoConsecutivoUsuario { get; set; }
        public int? Empresa { get; set; }
        public decimal? NumeroConteo { get; set; }
        public bool SoloRegistroFoto { get; set; }
        public bool PermiteIngresarMotivo { get; set; }        
        public bool ModificarStockEnDiferencia { get; set; }
        public string TipoInventario { get; set; }
        public bool IsCreacionWeb { get; set; }
        public bool ActualizarConteoFin { get; set; }
        public string Predio { get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool ExcluirSueltos { get; set; }
        public bool ExcluirLpns { get; set; }
        public bool RestablecerLpnFinalizado { get; set; }
        public bool GenerarPrimerConteo { get; set; }                   //FL_GENERAR_PRIMER_CONTEO
        public bool PermiteUbicacionesDeOtrosInventarios { get; set; }  //FL_PERMITE_ASOC_UBIC_OTRO_INV

        public List<InventarioUbicacion> Ubicaciones { get; set; }

        public Inventario()
        {
            this.Ubicaciones = new List<InventarioUbicacion>();
        }

        public virtual bool IsEditable()
        {
            return this.Estado == EstadoInventario.EnEdicion;
        }
        public virtual bool EnProceso()
        {
            return this.Estado == EstadoInventario.EnProceso;
        }

        public virtual bool PermiteCancelar()
        {
            return (this.Estado == EstadoInventario.EnEdicion || this.Estado == EstadoInventario.EnProceso);
        }

        public virtual bool PermiteRegenerar()
        {
            return (this.Estado == EstadoInventario.Cancelado || this.Estado == EstadoInventario.Cerrado);
        }
    }
}
