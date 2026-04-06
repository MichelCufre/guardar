using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General.Configuracion
{
    public class LiberacionConfiguracion
    {
        public string UbicacionCompleta { get; set; }
        public bool UbicacionCompletaHabilitado { get; set; }

        public string UbicacionIncompleta { get; set; }
        public bool UbicacionIncompletaHabilitado { get; set; }

        public string AgruparCamion { get; set; }
        public bool AgruparCamionHabilitado { get; set; }

        public string PrepSoloCamion { get; set; }
        public bool PrepSoloCamionHabilitado { get; set; }

        public string ControlStockDMTI { get; set; }
        public bool ControlStockDMTIHabilitado { get; set; }

        public string Pedidos { get; set; }
        public bool PedidosHabilitado { get; set; }

        public string RespetaFifo { get; set; }
        public bool RespetaFifoHabilitado { get; set; }

        public string RepartirEscazes { get; set; }
        public bool RepartirEscazesHabilitado { get; set; }

        public string LiberarPorCurvas { get; set; }
        public bool LiberarPorCurvasHabilitado { get; set; }

        public string LiberarPorUnidad { get; set; }
        public bool LiberarPorUnidadHabilitado { get; set; }

        public string DefaultStock { get; set; }
        public bool DefaultStockHabilitado { get; set; }

        public bool ManejoDocumental { get; set; }

        public string ManejoAgrupador { get; set; }
        public bool ManejoAgrupadorHabilitado { get; set; }

        public string ManejaVidaUtil { get; set; }
        public bool ManejaVidaUtilHabilitado { get; set; }

        public string PriorizarDesborde { get; set; }
        public bool PriorizarDesbordeHabilitado { get; set; }
        public string RequiereUbicacion { get; set; }
        public bool RequiereUbicacionHabilitado { get; set; }

        public string ExcluirUbicacionesPicking { get; set; }
        public bool ExcluirUbicacionesPickingHabilitado { get; set; }

        public string UsarSoloStkPicking { get; set; }
        public bool UsarSoloStkPickingHabilitado { get; set; }
    }
}
