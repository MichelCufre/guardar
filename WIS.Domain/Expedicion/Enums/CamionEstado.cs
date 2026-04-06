using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion.Enums
{
    public enum CamionEstado
    {
        Unknown,
        AguardandoCarga = 650,
        Cargando = 651,
        IniciandoCierre = 653,
        Cerrado = 652,
        SinOrdenDeTrabajo = 70
    }
}
