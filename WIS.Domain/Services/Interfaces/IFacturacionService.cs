using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Interfaces;

namespace WIS.Domain.Services.Interfaces
{
    public interface IFacturacionService
    {
        byte[] DescargarExcel(int nuEjecucion, int cdEmpresa, string cdFacturacion, string nameFile);
        void Start(int? nroFactEjecucion = null);
    }
}
