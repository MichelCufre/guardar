using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class ContenedorEntregaImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IEstiloTemplate _estilo;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;

        protected readonly Pedido _pedido;
        protected readonly Agente _cliente;
        protected readonly int _cantidadGenerar;
        protected readonly Contenedor _contenedorDestino;

        public ContenedorEntregaImpresionStrategy(IEstiloTemplate estilo,
            IUnitOfWork uow,
            IPrintingService printingService,
            Contenedor contenedorDestino,
            Pedido pedido,
            Agente cliente,
            IBarcodeService barcodeservice,
            int cantidadGenerar = 0)
        {
            _uow = uow;
            _estilo = estilo;
            _pedido = pedido;
            _cliente = cliente;
            _barcodeService = barcodeservice;
            _printingService = printingService;
            _cantidadGenerar = cantidadGenerar;
            _contenedorDestino = contenedorDestino;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            var template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();

            if (_contenedorDestino != null && _cantidadGenerar > 0)
            {
                for (int i = 0; i < this._cantidadGenerar; i++)
                {
                    var numEtiqueta = i + 1;
                    var claves = new Dictionary<string, string>();

                    _uow.ImpresionRepository.GetDiccionarioInformacionPedido(claves, _pedido.Id, _pedido.Cliente, _pedido.Empresa);
                    _uow.ImpresionRepository.GetDiccionarioInformacionCliente(claves, _pedido.Cliente, _pedido.Empresa);
                    _uow.ImpresionRepository.GetDiccionarioInformacionEmpresa(claves, _pedido.Empresa);

                    GetDiccionarioInformacionContenedorEntrega(claves, _pedido.DireccionEntrega, _contenedorDestino);

                    detalles.Add(new DetalleImpresion
                    {
                        Contenido = template.Parse(claves),
                        Estado = _printingService.GetEstadoInicial(),
                        FechaProcesado = DateTime.Now
                    });

                    _uow.SaveChanges();
                }
            }

            return detalles;
        }

        public virtual void GetDiccionarioInformacionContenedorEntrega(Dictionary<string, string> claves, string direccionEntrega, Contenedor contenedorDestino)
        {
            var dsEndereco1 = string.Empty;
            var dsEndereco2 = string.Empty;

            if (!string.IsNullOrEmpty(direccionEntrega))
            {
                if (direccionEntrega.Length > 40)
                {
                    dsEndereco1 = direccionEntrega.Substring(0, 20);
                    dsEndereco2 = direccionEntrega.Substring(21, 20);
                }
                else
                {
                    if (direccionEntrega.Length > 20)
                    {
                        dsEndereco1 = direccionEntrega.Substring(0, 20);
                        dsEndereco2 = direccionEntrega.Substring(21);
                    }
                    else
                        dsEndereco1 = direccionEntrega;
                }
            }

            var cdRuta = (_pedido.Ruta ?? 1).ToString();
            var ruta = _uow.RutaRepository.GetRuta(short.Parse(cdRuta));

            var cdBarras = contenedorDestino.CodigoBarras ??
                _barcodeService.GenerateBarcode(contenedorDestino.Numero.ToString(), contenedorDestino.TipoContenedor);

            claves.Add("T_ROTA.DS_ROTA", ruta.Descripcion ?? _pedido.Zona);
            claves.Add("T_CONTENEDOR.QT_BULTO", (contenedorDestino.CantidadBulto ?? 1).ToString());
            claves.Add("T_PEDIDO_SAIDA.DS_ENDERECO_01_20", dsEndereco1);
            claves.Add("T_PEDIDO_SAIDA.DS_ENDERECO_21_20", dsEndereco2);
            claves.Add("WIS.P_CONTENEDOR", contenedorDestino.IdExterno);
            claves.Add("WIS.P_CONTENEDOR_2", contenedorDestino.Numero.ToString());
            claves.Add("WIS.CD_BARRAS_ETIQUETA", cdBarras);
            claves.Add("WIS.DT_IMPRESION_ETIQUETA", DateTime.Now.ToString("yyyy-MM-dd_HH:mm"));
        }
    }
}
