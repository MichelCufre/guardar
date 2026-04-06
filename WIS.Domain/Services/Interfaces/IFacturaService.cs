using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IFacturaService
    {
        Task<ValidationsResult> AgregarFacturas(List<Factura> factura, int empresa);

        Task<Factura> GetFactura(string nuFactura, int codigoEmpresa, string serie, string codigoAgente, string tipoAgente);
    }
}
