using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class CodigoBarrasMapper : Mapper, ICodigoBarrasMapper
    {
        public CodigoBarrasMapper()
        {
        }

        public virtual List<CodigoBarras> Map(CodigosBarrasRequest request)
        {
            List<CodigoBarras> codigos = new List<CodigoBarras>();

            foreach (var c in request.CodigosDeBarras)
            {
                CodigoBarras codigo = new CodigoBarras(c.TipoOperacion);
                codigo.Codigo = c.Codigo;
                codigo.Producto = c.Producto;
                codigo.TipoCodigo = c.TipoCodigo;
                codigo.PrioridadUso = c.PrioridadUso;
                codigo.CantidadEmbalaje = c.CantidadEmbalaje;
                codigo.Empresa = request.Empresa;
                codigos.Add(codigo);
            }
            return codigos;
        }

        public virtual CodigoBarrasResponse MapToResponse(CodigoBarras codigoBarras)
        {
            return new CodigoBarrasResponse()
            {
                Codigo = codigoBarras.Codigo,
                Empresa = codigoBarras.Empresa,
                Producto = codigoBarras.Producto,
                TipoCodigo = codigoBarras.TipoCodigo,
                PrioridadUso = codigoBarras.PrioridadUso,
                CantidadEmbalaje = codigoBarras.CantidadEmbalaje,
                FechaInsercion = codigoBarras.FechaInsercion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = codigoBarras.FechaModificacion.ToString(CDateFormats.DATE_ONLY)
            };
        }
    }
}
