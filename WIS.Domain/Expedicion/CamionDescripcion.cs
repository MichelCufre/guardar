using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class CamionDescripcion : Camion
    {
        public string DescEmpresa { get; set; }
        public string DescPuerta { get; set; }
        public string DescRuta { get; set; }
        public string DescSituacion { get; set; }
    }
}