using System.Collections.Generic;
using System.Linq;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;

namespace WIS.AutomationManager.Models.Mappers
{
    public class PtlAutomatismoMapper : IPtlAutomatismoMapper
    {
        public PtlResponse Map(IAutomatismo ptl)
        {
            if (ptl == null) return null;

            var tipoPtl = ptl.GetCaracteristicaByCodigo(PtlCaracteristicaDb.TipoPtl);

            return new PtlResponse
            {
                Ptl = ptl.ZonaUbicacion,
                Description = ptl.Descripcion,
                CodeType = tipoPtl.Valor,
                DescriptionType = tipoPtl.Descripcion,
                IsEnabled = ptl.IsEnabled,
                Id = ptl.Numero
            };

        }

        public List<PtlResponse> Map(List<IAutomatismo> ptls)
        {
            List<PtlResponse> result = new List<PtlResponse>();

            if (ptls != null)
            {
                foreach (var obj in ptls)
                {
                    result.Add(this.Map(obj));
                }
            }

            return result;
        }

        public PtlPosicionEnUso Map(IPtl ptl, PtlActionRequest accion, int nroEjecucion)
        {
            return new PtlPosicionEnUso
            {
                Empresa = accion.Company,
                Producto = accion.Product,
                Ubicacion = ptl.Posiciones.FirstOrDefault(w => w.IdUbicacion == accion.Position)?.Id ?? -1,
                Detalle = accion.Detail,
                Display = accion.Display,
                DisplayFn = accion.DisplayFn,
                Color = accion.Color,
                Ptl = ptl.Numero,
                UserId = accion.UserId,
                Id = nroEjecucion,
                Key = accion.Key,
                Referencia = accion.Referencia,
                Agrupacion = accion.Agrupacion
            };
        }

        public PtlPosicionEnUso Map(IPtl ptl, PtlCommandConfirmRequest command)
        {
            return new PtlPosicionEnUso
            {
                Ubicacion = ptl.Posiciones.FirstOrDefault(w => w.PosicionExterna == command.Address)?.Id ?? -1,
                Display = command.Cantidad,
                DisplayFn = command.DisplayText,
                Color = command.Color,
                Ptl = ptl.Numero,
                Id = command.CommandId,
            };
        }

        public List<PtlCommandConfirmRequest> Map(IPtl ptl, List<PtlPosicionEnUso> ptlPosicionEnUsos)
        {
            List<PtlCommandConfirmRequest> result = new List<PtlCommandConfirmRequest>();

            foreach (var pos in ptlPosicionEnUsos)
            {
                result.Add(this.Map(ptl, pos));
            }

            return result;
        }

        public PtlCommandConfirmRequest Map(IPtl ptl, PtlPosicionEnUso pos)
        {
            if (pos == null) return null;

            return new PtlCommandConfirmRequest
            {
                Address = ptl.GetPosicion(pos.Ubicacion).PosicionExterna,
                Cantidad = pos.Display,
                Color = pos.Color,
                DisplayText = pos.Display,
                Id = ptl.Codigo,
                UserId = pos.UserId,
                Detail = pos.Detalle
            };
        }
    }
}
