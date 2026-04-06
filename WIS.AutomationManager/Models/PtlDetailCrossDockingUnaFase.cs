using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.General;

namespace WIS.AutomationManager.Models
{
    public class PtlDetailCrossDockingUnaFase
    {
        public int NuPreparacion { get; set; }
        public int NuAgenda { get; set; }
        public string NuIdentificador { get; set; }
        public string Ubicacion { get; set; }
        public int Contenedor { get; set; }
        public string IdEspecificaIdentificador { get; set; }
        public string TipoEtiqueta { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoAgente { get; set; }

        public void Clonar(PtlDetailCrossDockingUnaFase data)
        {
            NuPreparacion = data.NuPreparacion;
            NuAgenda = data.NuAgenda;
            NuIdentificador = data.NuIdentificador;
            Ubicacion = data.Ubicacion;
            Contenedor = data.Contenedor;
            IdEspecificaIdentificador = data.IdEspecificaIdentificador;
        }
    }
}
