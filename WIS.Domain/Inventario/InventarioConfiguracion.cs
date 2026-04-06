using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Inventario
{
    public class InventarioConfiguracion
    {
        public bool ControlarVencimiento { get; set; }
        public bool ControlarVencimientoHabilitado { get; set; }

        public bool BloquearConteo { get; set; }
        public bool BloquearConteoHabilitado { get; set; }

        public bool PermiteIngresarMotivo { get; set; }
        public bool PermiteIngresarMotivoHabilitado { get; set; }

        public bool MarcarDiferencia { get; set; }
        public bool MarcarDiferenciaHabilitado { get; set; }

        public bool ActualizarConteo { get; set; }
        public bool ActualizarConteoHabilitado { get; set; }

        public bool ExcluirSueltos { get; set; }
        public bool ExcluirSueltosHabilitado { get; set; }

        public bool ExcluirLpns { get; set; }
        public bool ExcluirLpnsHabilitado { get; set; }

        public bool RestablecerLpnFinalizado{ get; set; }
        public bool RestablecerLpnFinalizadoHabilitado { get; set; }

        public bool GenerarPrimerConteo{ get; set; }
        public bool GenerarPrimerConteoHabilitado { get; set; }

        public bool PermiteUbicacionesDeOtrosInventarios { get; set; }
        public bool PermiteUbicacionesDeOtrosInventariosHabilitado { get; set; }
    }
}
