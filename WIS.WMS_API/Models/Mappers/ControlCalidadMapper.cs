using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class ControlCalidadMapper : IControlCalidadMapper
    {
        public virtual List<ControlCalidadAPI> Map(ControlCalidadRequest request) =>
            request.ControlesDeCalidad
                .Select(
                    (control, controlIndex) =>
                        new ControlCalidadAPI
                        {
                            CodigoControlCalidad = (int)control.CodigoControlCalidad,
                            Estado = this.Map(control.Estado),
                            Descripcion = control.TextoInformativo,
                            Empresa = request.Empresa,
                            Criterios =
                                control.CriteriosDeSeleccion
                                    .Select(
                                        (criterio, criterioIndex) =>
                                            new CriterioControlCalidadAPI
                                            {
                                                Empresa = request.Empresa,
                                                Predio = criterio.Predio,
                                                EtiquetaExterna = criterio.EtiquetaExterna,
                                                TipoEtiquetaExterna = criterio.TipoEtiquetaExterna,
                                                Ubicacion = criterio.Ubicacion,
                                                Producto = criterio.Producto,
                                                Lote =
                                                    (string.IsNullOrEmpty(criterio.Lote)
                                                        ? ManejoIdentificadorDb.IdentificadorProducto
                                                        : criterio.Lote)?.Trim()?.ToUpper(),
                                                Faixa = 1
                                            })
                                    .ToList()
                        })
                .ToList();

        public virtual ControlCalidadOperacion Map(string estado) =>
            estado switch
            {
                ControlCalidadOperacionDb.ControlAsociar => ControlCalidadOperacion.Asociar,
                ControlCalidadOperacionDb.ControlAprobar => ControlCalidadOperacion.Aprobar,
                _ => ControlCalidadOperacion.Indefinido
            };
    }
}
