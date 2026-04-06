using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class TransferenciaImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly List<EtiquetaTransferencia> _etiquetas;
        protected readonly int _cantidadGenerar;

        protected readonly IUnitOfWork _uow;
        protected readonly IEstiloTemplate _estilo;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;

        public TransferenciaImpresionStrategy(IEstiloTemplate estilo,
            List<EtiquetaTransferencia> etiquetas,
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
            TemplateImpresion template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();
            var estilo = EstiloEtiquetaDb.Transferencia;

            //Generar etiquetas
            if (this._cantidadGenerar > 0)
            {
                bool isLocalTransaction = !_uow.AnyTransaction();

                if (isLocalTransaction)
                {
                    _uow.BeginTransaction();
                }

                for (int i = 0; i < this._cantidadGenerar; i++)
                {
                    EtiquetaTransferencia etiquetaGenerada = this._etiquetas.First();

                    etiquetaGenerada.NumeroEtiqueta = _uow.EtiquetaTransferenciaRepository.GetProximoNumeroEtiqueta();

                    Dictionary<string, string> claves = this.GetDiccionarioInformacion(etiquetaGenerada, estilo);

                    detalles.Add(new DetalleImpresion
                    {
                        Contenido = template.Parse(claves),
                        Estado = _printingService.GetEstadoInicial(),
                        FechaProcesado = DateTime.Now,
                    });
                }

                if (isLocalTransaction)
                {
                    _uow.Commit();
                }
            }
            //Reimprimir una cierta etiqueta dada
            else if (this._cantidadGenerar == 0)
            {
                foreach (var etiqueta in this._etiquetas.OrderBy(s => s.NumeroEtiqueta))
                {
                    Dictionary<string, string> claves = this.GetDiccionarioInformacion(etiqueta, estilo);

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

        public virtual Dictionary<string, string> GetDiccionarioInformacion(EtiquetaTransferencia etiqueta, string estiloEtiqueta)
        {
            var barra = _barcodeService.GenerateBarcode(etiqueta.NumeroEtiqueta.ToString(), estiloEtiqueta);

            if (!string.IsNullOrEmpty(etiqueta.TipoEtiquetaTransferencia))
            {
                if (etiqueta.TipoEtiquetaTransferencia.StartsWith(TipoEtiquetaTransferencia.Contenedor))
                {
                    var tipo = etiqueta.TipoEtiquetaTransferencia.Replace(TipoEtiquetaTransferencia.Contenedor, "");
                    barra = _barcodeService.GenerateBarcode(etiqueta.IdExternoEtiqueta.ToString(), tipo);

                }
                else if (etiqueta.TipoEtiquetaTransferencia == TipoEtiquetaTransferencia.Lpn)
                    barra = _uow.ManejoLpnRepository.GetCodigoDeBarras(etiqueta.NroLpn ?? 0).FirstOrDefault().CodigoBarras;
            }

            return new Dictionary<string, string>
            {
                { "WIS.P_ETIQUETA", "" + etiqueta.NumeroEtiqueta.ToString() },
                { "WIS.CD_BARRAS_ETIQUETA", barra }
            };
        }


    }
}
