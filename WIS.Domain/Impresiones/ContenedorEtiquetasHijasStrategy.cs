using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Impresiones.Dtos;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class ContenedorEtiquetasHijasStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;

        protected readonly Pedido _pedido;
        protected readonly int _cantidadGenerar;
        protected readonly ContenedorEtiquetaHijas _contenedor;

        public ContenedorEtiquetasHijasStrategy(IEstiloTemplate estilo,
            IUnitOfWork uow,
            IPrintingService printingService,
            ContenedorEtiquetaHijas contenedor,
            Pedido pedido,
            IBarcodeService barcodeService,
            int cantidadGenerar = 0)
        {
            _uow = uow;
            _estilo = estilo;
            _pedido = pedido;
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
                    var inicio = _contenedor.ImprimePrimerBulto ? 1 : 2;
                    for (int bulto = inicio; bulto <= _contenedor.CantidadBultos; bulto++)
                    {
                        var numEtiqueta = bulto == 1 ? bulto : bulto + 1;
                        var claves = new Dictionary<string, string>();
                        _contenedor.NumeroEtiquetaHija = bulto;

                        _uow.ImpresionRepository.GetDiccionarioInformacionPedido(claves, _pedido.Id, _pedido.Cliente, _pedido.Empresa);
                        _uow.ImpresionRepository.GetDiccionarioInformacionCliente(claves, _pedido.Cliente, _pedido.Empresa);
                        _uow.ImpresionRepository.GetDiccionarioInformacionEmpresa(claves, _pedido.Empresa);

                        GetDiccionarioInformacion(claves, _contenedor, numEtiqueta);

                        detalles.Add(new DetalleImpresion
                        {
                            Contenido = template.Parse(claves),
                            Estado = _printingService.GetEstadoInicial(),
                            FechaProcesado = DateTime.Now
                        });
                    }

                    _uow.SaveChanges();
                }
            }

            return detalles;
        }

        public virtual void GetDiccionarioInformacion(Dictionary<string, string> claves, ContenedorEtiquetaHijas contenedor, int? numeroEtiqueta = null)
        {
            var cdBarras = !string.IsNullOrEmpty(contenedor.CodigoBarras) ? contenedor.CodigoBarras :
               _barcodeService.GenerateBarcode(contenedor.IdExterno, contenedor.TipoContenedor);

            var dsEndereco1 = string.Empty;
            var dsEndereco2 = string.Empty;

            if (!string.IsNullOrEmpty(contenedor.DescripcionUbicacion))
            {
                if (contenedor.DescripcionUbicacion.Length > 40)
                {
                    dsEndereco1 = contenedor.DescripcionUbicacion.Substring(0, 20);
                    dsEndereco2 = contenedor.DescripcionUbicacion.Substring(21, 20);
                }
                else
                {
                    if (contenedor.DescripcionUbicacion.Length > 20)
                    {
                        dsEndereco1 = contenedor.DescripcionUbicacion.Substring(0, 20);
                        dsEndereco2 = contenedor.DescripcionUbicacion.Substring(21);
                    }
                    else
                        dsEndereco1 = contenedor.DescripcionUbicacion;
                }
            }

            var cdRuta = (_pedido.Ruta ?? 1).ToString();
            var ruta = _uow.RutaRepository.GetRuta(short.Parse(cdRuta));

            claves.Add("T_ROTA.DS_ROTA", ruta.Descripcion ?? _pedido.Zona);
            claves.Add("T_CONTENEDOR.QT_BULTO", contenedor.CantidadBultos.ToString());
            claves.Add("T_CONTENEDOR.DS_MEMO", contenedor.Memo);
            claves.Add("T_CONTENEDOR.DS_CONTENEDOR", contenedor.DescripcionContenedor);
            claves.Add("T_CONTENEDOR.NU_CONTENEDOR", contenedor.NumeroContenedor.ToString());
            claves.Add("T_CONTENEDOR.ID_EXTERNO_CONTENEDOR", contenedor.IdExterno);
            claves.Add("T_CONTENEDOR.TP_CONTENEDOR", contenedor.TipoContenedor);
            claves.Add("T_PEDIDO_SAIDA.DS_ENDERECO_01_20", dsEndereco1);
            claves.Add("T_PEDIDO_SAIDA.DS_ENDERECO_21_20", dsEndereco2);
            claves.Add("WIS.P_ETIQUETA_HIJA", contenedor.NumeroEtiquetaHija.ToString());
            claves.Add("WIS.P_CONTENEDOR", contenedor.IdExterno);
            claves.Add("WIS.CD_BARRAS_ETIQUETA", cdBarras);
        }
    }
}
