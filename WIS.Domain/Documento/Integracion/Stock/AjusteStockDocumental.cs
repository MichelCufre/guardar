using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Documento.Integracion.Stock
{
    public class AjusteStockDocumental
    {
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;

        public AjusteStockDocumental(IFactoryService factoryService,
            IParameterService parameterService)
        {
            this._factoryService = factoryService;
            this._parameterService = parameterService;
        }

        /// <summary>
        /// Se crea el acta de stock en caso de existir una acta en edición para el documento se actualizan las lineas
        /// </summary>
        public virtual IDocumentoActa CrearActaStock(IUnitOfWork uow, IDocumentoIngreso documentoIngreso, List<InformacionActaStock> infoLineasActa, int userId, bool positiva = false)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var flagExistePreviamente = true;
            var tipoDocumento = this._parameterService.GetValueByEmpresa(ParamManager.TP_DOC_ACTA_STOCK, documentoIngreso.Empresa.Value);
            var tipoActaStock = uow.DocumentoTipoRepository.GetTipoDocumento(tipoDocumento);

            //Busco acta en estado de edición asociada al documento
            var actaStock = uow.DocumentoRepository.GetActaStockEnEdicion(documentoIngreso.Numero, documentoIngreso.Tipo, tipoActaStock.TipoDocumento, positiva);

            if (actaStock == null)
            {
                var estado = uow.DocumentoTipoRepository.GetEstadoInicial(tipoActaStock.TipoDocumento);

                //En caso de no existir acta se crea
                actaStock = this._factoryService.CreateDocumentoActa(tipoActaStock.TipoDocumento);

                actaStock.InReference = documentoIngreso;
                actaStock.Tipo = tipoActaStock.TipoDocumento;
                actaStock.Numero = actaStock.GetNumeroDocumento(uow);
                actaStock.Agenda = documentoIngreso.Agenda;
                actaStock.Anexo1 = documentoIngreso.Anexo1;
                actaStock.Anexo2 = documentoIngreso.Anexo2;
                actaStock.Camion = documentoIngreso.Camion;
                actaStock.CantidadBulto = documentoIngreso.CantidadBulto;
                actaStock.CantidadContenedor20 = documentoIngreso.CantidadContenedor20;
                actaStock.CantidadContenedor40 = documentoIngreso.CantidadContenedor40;
                actaStock.Cliente = documentoIngreso.Cliente;
                actaStock.Configuracion = documentoIngreso.Configuracion;
                actaStock.Conocimiento = documentoIngreso.Conocimiento;
                actaStock.Predio = documentoIngreso.Predio;
                actaStock.Descripcion = tipoActaStock.DescripcionTipoDocumento;
                actaStock.Despachante = documentoIngreso.Despachante;
                actaStock.DocumentoAduana = documentoIngreso.DocumentoAduana;
                actaStock.DocumentoReferenciaExterna = documentoIngreso.DocumentoReferenciaExterna;
                actaStock.DocumentoTransporte = documentoIngreso.DocumentoTransporte;
                actaStock.Empresa = documentoIngreso.Empresa;
                actaStock.Estado = estado;
                actaStock.Factura = documentoIngreso.Factura;
                actaStock.FechaAlta = DateTime.Now;
                actaStock.FechaDTI = documentoIngreso.FechaDTI;
                actaStock.Moneda = documentoIngreso.Moneda;
                actaStock.NumeroDTI = documentoIngreso.NumeroDTI;
                actaStock.Transportista = documentoIngreso.Transportista;
                actaStock.UnidadMedida = documentoIngreso.UnidadMedida;
                actaStock.Usuario = userId;
                actaStock.ValorArbitraje = documentoIngreso.ValorArbitraje;
                actaStock.ValorFlete = 0;
                actaStock.ValorOtrosGastos = 0;
                actaStock.ValorSeguro = 0;
                actaStock.Via = documentoIngreso.Via;
                actaStock.Volumen = documentoIngreso.Volumen;

                //Crear relacion acta / documento ingreso
                actaStock.ActaDetail.Add(new DocumentoActaDetalle()
                {
                    NumeroDocumento = documentoIngreso.Numero,
                    TipoDocumento = documentoIngreso.Tipo,
                    NumeroDocumentoActa = actaStock.Numero,
                    TipoDocumentoActa = actaStock.Tipo
                });

                uow.DocumentoRepository.AddActa(actaStock, nuTransaccion);
                flagExistePreviamente = false;
            }

            //Generar Lineas
            var lineasEgreso = infoLineasActa.Where(l => l.CantidadActa < 0).ToList();
            var lineasIngreso = infoLineasActa.Where(l => l.CantidadActa > 0).ToList();

            actaStock.Lineas = this.CrearLineasIngresoActa(lineasIngreso, actaStock, documentoIngreso, uow);
            actaStock.OutDetail = this.CrearLineasEgresoActa(lineasEgreso, actaStock, documentoIngreso, userId, uow);

            actaStock.ValidarActa();

            if (flagExistePreviamente)
            {
                actaStock.FechaModificacion = DateTime.Now;
                uow.DocumentoRepository.UpdateActaSinDetalle(actaStock, nuTransaccion);
            }

            return actaStock;
        }

        /// <summary>
        /// Crea las lineas de ingreso asociadas al acta de stock, en caso que exista la linea suma la cantidad y carga cif y fob en 0, Para obligar a cargar los valores si es que ya estaban cargados.
        /// </summary>
        public virtual List<DocumentoLinea> CrearLineasIngresoActa(List<InformacionActaStock> infoLineasActa, IDocumentoActa acta, IDocumentoIngreso documentoIngreso, IUnitOfWork uow)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var lineas = new List<DocumentoLinea>();

            foreach (var lineaBalanceo in infoLineasActa)
            {
                var linea = acta.Lineas.FirstOrDefault(l => l.Empresa == lineaBalanceo.Empresa
                    && l.Producto == lineaBalanceo.Producto
                    && l.Faixa == lineaBalanceo.Faixa
                    && l.Identificador == lineaBalanceo.NumeroIdentificador);

                if (linea == null)
                {
                    Producto producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(lineaBalanceo.Empresa, lineaBalanceo.Producto);

                    DocumentoLinea lineaActa = new DocumentoLinea()
                    {
                        Empresa = lineaBalanceo.Empresa,
                        Producto = lineaBalanceo.Producto,
                        Faixa = lineaBalanceo.Faixa,
                        Identificador = lineaBalanceo.NumeroIdentificador,

                        CIF = (decimal)(lineaBalanceo.CIF ?? 0),
                        ValorMercaderia = (decimal)(lineaBalanceo.FOB ?? 0),
                        CantidadIngresada = Math.Abs((decimal)lineaBalanceo.CantidadActa),
                        DescripcionProducto = producto.Descripcion,
                        FechaAlta = DateTime.Now,
                        Situacion = 15,
                        ValorTributo = lineaBalanceo.Tributo
                    };

                    lineas.Add(lineaActa);
                    uow.DocumentoRepository.AddLineaActa(acta, lineaActa, nuTransaccion);
                }
                else
                {
                    linea.CantidadIngresada = linea.CantidadIngresada + Math.Abs((decimal)lineaBalanceo.CantidadActa);
                    linea.FechaModificacion = DateTime.Now;
                    linea.CIF = (decimal)(lineaBalanceo.CIF ?? 0);
                    linea.ValorMercaderia = (decimal)(lineaBalanceo.FOB ?? 0);

                    lineas.Add(linea);
                    uow.DocumentoRepository.UpdateLineaActa(linea, acta.Numero, acta.Tipo, nuTransaccion);
                }
            }

            return lineas;
        }

        public virtual List<DocumentoLineaEgreso> CrearLineasEgresoActa(List<InformacionActaStock> infoLineasActa, IDocumentoActa acta, IDocumentoIngreso documentoIngreso, int userId, IUnitOfWork uow)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var lineas = new List<DocumentoLineaEgreso>();

            foreach (var lineaBalanceo in infoLineasActa)
            {
                var linea = acta.OutDetail.FirstOrDefault(l => l.Empresa == lineaBalanceo.Empresa
                    && l.Producto == lineaBalanceo.Producto
                    && l.Faixa == lineaBalanceo.Faixa
                    && l.Identificador == lineaBalanceo.NumeroIdentificador);

                if (linea == null)
                {
                    linea = new DocumentoLineaEgreso()
                    {
                        DocumentoIngreso = documentoIngreso,
                        CantidadDesafectada = Math.Abs((decimal)lineaBalanceo.CantidadActa),
                        CIF = (decimal)(lineaBalanceo.CIF ?? 0),
                        FOB = (decimal)(lineaBalanceo.FOB ?? 0),
                        Empresa = lineaBalanceo.Empresa,
                        Faixa = lineaBalanceo.Faixa,
                        FechaAlta = DateTime.Now,
                        Identificador = lineaBalanceo.NumeroIdentificador,
                        Producto = lineaBalanceo.Producto,
                        Usuario = userId,
                        Numero = uow.DocumentoRepository.GetNumeroSecuenciaDetalleEgreso()
                    };
                    lineas.Add(linea);
                    uow.DocumentoRepository.AddLineaEgresoActa(acta, linea, nuTransaccion);
                }
                else
                {
                    linea.CantidadDesafectada = linea.CantidadDesafectada + Math.Abs((decimal)lineaBalanceo.CantidadActa);
                    linea.FechaModificacion = DateTime.Now;
                    linea.CIF = (decimal)(lineaBalanceo.CIF ?? 0);
                    linea.FOB = (decimal)(lineaBalanceo.FOB ?? 0);

                    lineas.Add(linea);
                    uow.DocumentoRepository.UpdateLineaEgresoActa(linea, acta.Numero, acta.Tipo, nuTransaccion);
                }
            }

            return lineas;
        }
    }
}
