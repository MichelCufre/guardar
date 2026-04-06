using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class EtiquetaRecepcionImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly List<EtiquetaLote> _etiquetas;
        protected readonly int _cantidadGenerar;

        protected readonly IUnitOfWork _uow;
        protected readonly IEstiloTemplate _estilo;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;

        public EtiquetaRecepcionImpresionStrategy(IEstiloTemplate estilo,
            List<EtiquetaLote> etiquetas,
            IUnitOfWork uow,
            IPrintingService printingService,
            IBarcodeService barcodeService,
            int cantidadGenerar = 0)
        {
            this._uow = uow;
            this._estilo = estilo;
            this._etiquetas = etiquetas;
            this._cantidadGenerar = cantidadGenerar;
            this._printingService = printingService;
            this._barcodeService = barcodeService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            var template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();

            //Generar etiquetas
            if (this._cantidadGenerar > 0)
            {
                for (int i = 0; i < this._cantidadGenerar; i++)
                {
                    var numEtiqueta = i + 1;

                    var etiquetaGenerada = this._etiquetas.First();

                    etiquetaGenerada.Numero = _uow.EtiquetaLoteRepository.GetProximoNumeroEtiquetaEntrada();

                    var claves = this.GetDiccionarioInformacion(etiquetaGenerada);

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
                foreach (var etiqueta in this._etiquetas.OrderBy(s => s.Numero))
                {
                    var claves = this.GetDiccionarioInformacion(etiqueta);

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


        public virtual Dictionary<string, string> GetDiccionarioInformacion(EtiquetaLote etiqueta)
        {
            var externo = string.IsNullOrEmpty(etiqueta.NumeroExterno) ? etiqueta.Numero.ToString() : etiqueta.NumeroExterno;
            var claves = new Dictionary<string, string>
            {
                { "WIS.P_ETIQUETA_LOTE", "" + externo },
                { "WIS.CD_BARRAS_ETIQUETA", _barcodeService.GenerateBarcode(externo, etiqueta.TipoEtiqueta) }
            };

            return claves;
        }


    }
}
