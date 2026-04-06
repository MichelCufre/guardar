using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Impresiones.Dtos;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class ContenedorFinPickingImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;
        protected readonly int _cantidadGenerar;
        protected readonly DatosContenedorFinPicking _datos;
        protected readonly ContenedorFinPicking _contenedor;

        public ContenedorFinPickingImpresionStrategy(IEstiloTemplate estilo,
            IUnitOfWork uow,
            IPrintingService printingService,
            DatosContenedorFinPicking datos,
            ContenedorFinPicking contenedor,
            IBarcodeService barcodeService,
            int cantidadGenerar = 0)
        {
            _uow = uow;
            _datos = datos;
            _estilo = estilo;
            _contenedor = contenedor;
            _barcodeService = barcodeService;
            _printingService = printingService;
            _cantidadGenerar = cantidadGenerar;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            var template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();

            if (this._contenedor != null && this._cantidadGenerar > 0)
            {
                for (int i = 0; i < this._cantidadGenerar; i++)
                {
                    var numEtiqueta = i + 1;
                    var claves = this.GetDiccionarioInformacion(this._contenedor, numEtiqueta);

                    detalles.Add(new DetalleImpresion
                    {
                        Contenido = template.Parse(claves),
                        Estado = _printingService.GetEstadoInicial(),
                        FechaProcesado = DateTime.Now,

                    });

                    _uow.SaveChanges();
                }
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(ContenedorFinPicking contenedor, int? numeroEtiqueta = null)
        {
            var cdBarras = !string.IsNullOrEmpty(contenedor.CodigoBarras) ? contenedor.CodigoBarras :
                _barcodeService.GenerateBarcode(contenedor.IdExterno, contenedor.TipoContenedor);

            var claves = new Dictionary<string, string>()
            {
                { "T_PEDIDO_SAIDA.DS_ANEXO1", contenedor.Anexo1 },
                { "T_PEDIDO_SAIDA.DT_ENTREGA", contenedor.FechaEntrega.ToString() },
                { "WIS.P_PEDIDO", contenedor.NumeroPedido },
                { "WIS.P_CLIENTE",  contenedor.CodigoCliente },
                { "T_CLIENTE.DS_CLIENTE" , contenedor.DescripcionCliente },
                { "T_PEDIDO_SAIDA.DS_ENDERECO", contenedor.DescripcionUbicacion },
                { "T_PEDIDO_SAIDA.DS_ANEXO4", contenedor.Anexo4 },
                { "T_PEDIDO_SAIDA.DS_ROTA" , contenedor.Anexo4 },
                { "WIS.P_TOTAL_BULTOS", contenedor.TotalBultos },
                { "WIS.LCONT001", _datos.vLCont001 },
                { "WIS.LCONT002", _datos.vLCont002 },
                { "WIS.LCONT003", _datos.vLCont003 },
                { "WIS.LCONT004", _datos.vLCont004 },
                { "WIS.LCONT005", _datos.vLCont005 },
                { "WIS.LCONT006", _datos.vLCont006 },
                { "WIS.LCONT007", _datos.vLCont007 },
                { "WIS.LCONT008", _datos.vLCont008 },
                { "WIS.CD_BARRAS_ETIQUETA" , cdBarras },
                { "T_CONTENEDOR.TP_CONTENEDOR", contenedor.TipoContenedor },
                { "T_CONTENEDOR.DS_CONTENEDOR", contenedor.DescripcionContenedor },
                { "T_CONTENEDOR.NU_CONTENEDOR", contenedor.NumeroContenedor.ToString() },
                { "T_CONTENEDOR.ID_EXTERNO_CONTENEDOR", contenedor.IdExterno }
            };

            return claves;
        }
    }
}
