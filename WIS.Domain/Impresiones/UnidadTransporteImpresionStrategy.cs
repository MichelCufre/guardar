using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Impresiones
{
    public class UnidadTransporteImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly List<UnidadTransporte> _etiquetas;
        protected readonly int _cantidadGenerar;

        protected readonly IUnitOfWork _uow;
        protected readonly IEstiloTemplate _estilo;
        protected readonly IPrintingService _printingService;
        protected readonly IBarcodeService _barcodeService;

        public UnidadTransporteImpresionStrategy(IEstiloTemplate estilo,
            List<UnidadTransporte> etiquetas,
            IUnitOfWork uow,
            IPrintingService printingService,
            IBarcodeService barcodeService,
            int cantidadGenerar = 0)
        {
            this._estilo = estilo;
            this._etiquetas = etiquetas;
            this._cantidadGenerar = cantidadGenerar;
            this._uow = uow;
            this._printingService = printingService;
            this._barcodeService = barcodeService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            TemplateImpresion template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();

            //Generar etiquetas
            if (this._cantidadGenerar > 0)
            {
                for (int i = 0; i < this._cantidadGenerar; i++)
                {
                    UnidadTransporte etiquetaGenerada = this._etiquetas.First();

                    etiquetaGenerada.NumeroExternoUnidad = _uow.UnidadTransporteRepository.GetProximoNumeroEtiquetaUnidadTransporte().ToString();

                    Dictionary<string, string> claves = this.GetDiccionarioInformacion(etiquetaGenerada);

                    detalles.Add(new DetalleImpresion
                    {
                        Contenido = template.Parse(claves),
                        Estado = _printingService.GetEstadoInicial(),
                        FechaProcesado = DateTime.Now,
                    });
                }
            }
            //Reimprimir una cierta etiqueta dada
            else if (this._cantidadGenerar == 0)
            {
                foreach (var etiqueta in this._etiquetas.OrderBy(s => s.NumeroUnidadTransporte))
                {
                    Dictionary<string, string> claves = this.GetDiccionarioInformacion(etiqueta);

                    detalles.Add(new DetalleImpresion
                    {
                        Contenido = template.Parse(claves),
                        Estado = _printingService.GetEstadoInicial(),
                        FechaProcesado = DateTime.Now,
                    });
                }
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(UnidadTransporte etiqueta)
        {
            var barra = _barcodeService.GenerateBarcode(etiqueta.NumeroExternoUnidad.ToString(), BarcodeDb.TIPO_ET_UT);
            return new Dictionary<string, string>
            {
                { "WIS.P_ETIQUETA_UT", "" + etiqueta.NumeroExternoUnidad.ToString() },
                { "WIS.CD_BARRAS_ETIQUETA", barra },

                { "WIS.NU_UNIDAD_TRANSPORTE", etiqueta.NumeroUnidadTransporte.ToString() },
                { "WIS.NU_EXTERNO_UNIDAD", etiqueta.NumeroExternoUnidad?.ToString() },
                { "WIS.TP_UNIDAD_TRANSPORTE", etiqueta.TipoUnidadTransporte?.ToString() },
                { "WIS.CD_ENDERECO", etiqueta.Ubicacion?.ToString() },
                { "WIS.CD_SITUACAO", etiqueta.Situacion?.ToString() }
            };
        }
    }
}
