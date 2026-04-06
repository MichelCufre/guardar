using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class ContenedorImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly IUnitOfWork _uow;
        protected Contenedor _contenedor;
        protected readonly int _cantidadGenerar;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;
        protected readonly ContenedorMapper _contenedorMapper;
        protected readonly IOptions<DatabaseSettings> _databaseSettings;

        public ContenedorImpresionStrategy(IEstiloTemplate estilo,
            IUnitOfWork uow,
            IPrintingService printingService,
            Contenedor contenedor,
            IOptions<DatabaseSettings> databaseSettings,
            IBarcodeService barcodeService,
            int cantidadGenerar = 0)
        {
            this._estilo = estilo;
            this._contenedor = contenedor;
            this._cantidadGenerar = cantidadGenerar;
            this._uow = uow;
            this._contenedorMapper = new ContenedorMapper();
            this._barcodeService = barcodeService;
            this._databaseSettings = databaseSettings;
            this._printingService = printingService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            var template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();

            //Generar etiquetas
            if (this._contenedor != null && this._cantidadGenerar > 0)
            {
                for (int i = 0; i < this._cantidadGenerar; i++)
                {
                    var numEtiqueta = i + 1;

                    this._contenedor.Numero = _uow.ContenedorRepository.GetUltimaSecuenciaTipoContenedor(this._contenedor.TipoContenedor);
                    this._contenedor.IdExterno = _contenedor.Numero.ToString(); //Por manejo de envases
                    this._contenedor.Estado = EstadoContenedor.Unknown;
                    this._contenedor.CodigoBarras = string.Empty; //Por manejo de envases

                    var claves = this.GetDiccionarioInformacion(this._contenedor, numEtiqueta);

                    detalles.Add(new DetalleImpresion
                    {
                        Contenido = template.Parse(claves),
                        Estado = _printingService.GetEstadoInicial(),
                        FechaProcesado = DateTime.Now,
                    });

                    this.MarcarContenedorImpresion(this._contenedor, _uow, impresora.Predio);
                }
            }
            //Reimprimir una cierta etiqueta dada
            else if (this._contenedor != null && this._cantidadGenerar == 0)
            {
                var claves = this.GetDiccionarioInformacion(this._contenedor);

                detalles.Add(new DetalleImpresion
                {
                    Contenido = template.Parse(claves),
                    Estado = _printingService.GetEstadoInicial(),
                    FechaProcesado = DateTime.Now,
                });

                this.MarcarContenedorImpresion(this._contenedor, _uow, impresora.Predio);
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(Contenedor contenedor, int? numeroEtiqueta = null)
        {
            var cdBarras = contenedor.CodigoBarras = !string.IsNullOrEmpty(contenedor.CodigoBarras) ? contenedor.CodigoBarras :
                           _barcodeService.GenerateBarcode(contenedor.Numero.ToString(), contenedor.TipoContenedor);

            var claves = new Dictionary<string, string>
            {
                { "T_CONTENEDOR.NU_PREPARACION", contenedor.NumeroPreparacion.ToString() },
                { "T_CONTENEDOR.NU_CONTENEDOR", contenedor.Numero.ToString() },
                { "T_CONTENEDOR.TP_CONTENEDOR", contenedor.TipoContenedor },
                { "T_CONTENEDOR.CD_SITUACAO", this._contenedorMapper.MapEstadoContenedor(contenedor.Estado).ToString() },
                { "T_CONTENEDOR.CD_ENDERECO", contenedor.Ubicacion },
                { "T_CONTENEDOR.CD_PORTA", contenedor.CodigoPuerta?.ToString() },
                { "T_CONTENEDOR.CD_CAMION", contenedor.CodigoCamion?.ToString() },
                { "T_CONTENEDOR.DT_PULMON", contenedor.FechaPulmon?.ToString() },
                { "T_CONTENEDOR.DT_EXPEDIDO", contenedor.FechaExpedido.ToString() },
                { "T_CONTENEDOR.DT_ADDROW", contenedor.FechaAgregado?.ToString() },
                { "T_CONTENEDOR.DT_UPDROW", contenedor.FechaModificado?.ToString() },
                { "T_CONTENEDOR.CD_FUNCIONARIO_EXPEDICION", contenedor.CodigoFuncionarioExpedicion?.ToString() },
                { "T_CONTENEDOR.PS_REAL", contenedor.PesoReal?.ToString() },
                { "T_CONTENEDOR.VL_ALTURA", contenedor.Altura?.ToString() },
                { "T_CONTENEDOR.VL_LARGURA", contenedor.Largo?.ToString() },
                { "T_CONTENEDOR.VL_PROFUNDIDADE", contenedor.Profundidad?.ToString() },
                { "T_CONTENEDOR.CD_UNIDAD_BULTO", contenedor?.CodigoUnidadBulto },
                { "T_CONTENEDOR.QT_BULTO", contenedor.CantidadBulto?.ToString() },
                { "T_CONTENEDOR.DS_CONTENEDOR", contenedor.DescripcionContenedor },
                { "T_CONTENEDOR.CD_CAMION_CONGELADO", contenedor.CodigoCamionCongelado?.ToString() },
                { "T_CONTENEDOR.NU_UNIDAD_TRANSPORTE", contenedor.NumeroUnidadTransporte?.ToString() },
                { "T_CONTENEDOR.CD_AGRUPADOR", contenedor.CodigoAgrupador?.ToString() },
                { "T_CONTENEDOR.NU_VIAJE", contenedor.NumeroViaje?.ToString() },
                { "T_CONTENEDOR.CD_CANAL", contenedor.CodigoCanal?.ToString() },
                { "T_CONTENEDOR.ID_CONTENEDOR_EMPAQUE", contenedor.IdContenedorEmpaque },
                { "T_CONTENEDOR.CD_CAMION_FACTURADO", contenedor.CamionFacturado?.ToString() },
                { "T_CONTENEDOR.TP_CONTROL", contenedor.TipoControl },
                { "T_CONTENEDOR.VL_CUBAGEM", contenedor.ValorCubagem?.ToString() },
                { "T_CONTENEDOR.ID_PRECINTO_1", contenedor.Precinto1 },
                { "T_CONTENEDOR.ID_PRECINTO_2", contenedor.Precinto2 },
                { "T_CONTENEDOR.FL_HABILITADO", contenedor.Habilitado },
                { "T_CONTENEDOR.VL_CONTROL", contenedor.ValorControl },
                { "T_CONTENEDOR.NU_TRANSACCION", contenedor.NumeroTransaccion?.ToString() },
                { "T_CONTENEDOR.ID_EXTERNO_CONTENEDOR", contenedor.IdExterno },
                { "WIS.P_ETIQUETA_HIJA", numeroEtiqueta != null ? numeroEtiqueta.ToString() : "-1" },
                { "WIS.P_CONTENEDOR", string.IsNullOrEmpty(contenedor.IdExterno) ? contenedor.Numero.ToString() : contenedor.IdExterno },
                { "WIS.CD_BARRAS_ETIQUETA", cdBarras },
                { "WIS.CD_BARRAS_ETIQUETA2", cdBarras },
                { "WIS.P_CONTENEDOR2", contenedor.Numero.ToString() }
            };

            var contenedorPredefinido = _uow.ContenedorRepository.GetContenedorPredefinidoByIdExternoTipo(contenedor.IdExterno, contenedor.TipoContenedor);

            if (contenedorPredefinido != null)
            {
                claves.Add("T_CONTENEDORES_PREDEFINIDOS.CD_EMPRESA", contenedorPredefinido.CodigoEmpresa.ToString());
                claves.Add("T_CONTENEDORES_PREDEFINIDOS.CD_CLIENTE", contenedorPredefinido.CodigoCliente);
            }

            return claves;
        }

        public virtual void MarcarContenedorImpresion(Contenedor contenedor, IUnitOfWork uow, string predio)
        {
            uow.CreateTransactionNumber("Reimpresion de contenedores");

            long transaccion = uow.GetTransactionNumber();

            uow.ContenedorRepository.MarcarImpresionContenedor(contenedor, predio, transaccion, this._cantidadGenerar == 0);

            uow.SaveChanges();
        }
    }
}
