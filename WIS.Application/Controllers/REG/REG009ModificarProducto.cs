using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG009ModificarProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ProductoMapper _productoMapper;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;

        public REG009ModificarProducto(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IAutomatismoAutoStoreClientService automatismoAutoStoreClientService)
        {
            this._identity = identity;
            this._productoMapper = new ProductoMapper();
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._automatismoAutoStoreClientService = automatismoAutoStoreClientService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(form, uow);

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idEmpresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "idProducto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "idProducto")?.Value;

                context.AddParameter("REG009_PRODUCTO_EDIT", idProducto);

                Producto produ = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(idEmpresa, idProducto);

                form.GetField("codigo").Value = produ.Codigo?.Trim();
                form.GetField("empresa").Value = produ.CodigoEmpresa.ToString();
                form.GetField("descripcion").Value = produ.Descripcion;
                form.GetField("reducida").Value = produ.DescripcionReducida;
                form.GetField("mercadologico").Value = produ.CodigoMercadologico?.Trim();
                form.GetField("manejoIdentificador").Value = this._productoMapper.MapManejoIdentificador(produ.ManejoIdentificador);
                form.GetField("manejoFecha").Value = produ.TipoManejoFecha;
                form.GetField("situacion").Value = produ.Situacion == 0 ? string.Empty : produ.Situacion.ToString();
                form.GetField("NCM").Value = produ.NAM;
                form.GetField("clase").Value = produ.CodigoClase;
                form.GetField("rotatividad").Value = produ.CodigoRotatividad.ToString();
                form.GetField("ramo").Value = produ.Ramo.ToString();
                form.GetField("familia").Value = produ.CodigoFamilia.ToString();

                form.GetField("modalidadIngresoLote").Value = produ.ModalidadIngresoLote;
                form.GetField("unidadDistribucion").Value = produ.UnidadDistribucion.ToString(this._identity.GetFormatProvider());
                form.GetField("unidadBultos").Value = produ.UnidadBulto.ToString(this._identity.GetFormatProvider());

                form.GetField("unidadMedida").Value = produ.UnidadMedida;
                form.GetField("diasLiberacion").Value = produ.DiasLiberacion.ToString();
                form.GetField("diasDuracion").Value = produ.DiasDuracion.ToString();
                form.GetField("diasValidez").Value = produ.DiasValidez.ToString();
                form.GetField("stockMaximo").Value = produ.StockMaximo.ToString();
                form.GetField("stockMinimo").Value = produ.StockMinimo.ToString();
                form.GetField("pesoBruto").Value = produ.PesoBruto?.ToString(this._identity.GetFormatProvider());
                form.GetField("pesoNeto").Value = produ.PesoNeto?.ToString(this._identity.GetFormatProvider());
                form.GetField("altura").Value = produ.Altura?.ToString(this._identity.GetFormatProvider());
                form.GetField("ancho").Value = produ.Ancho?.ToString(this._identity.GetFormatProvider());
                form.GetField("profundidad").Value = produ.Profundidad?.ToString(this._identity.GetFormatProvider());
                form.GetField("volumenCC").Value = produ.VolumenCC?.ToString(this._identity.GetFormatProvider());
                form.GetField("reducida").Value = produ.DescripcionReducida;
                form.GetField("mercadologico").Value = produ.CodigoMercadologico;
                form.GetField("NCM").Value = produ.NAM;
                form.GetField("productoEmpresaRef").Value = produ.CodigoProductoEmpresa;
                form.GetField("ajusteInventario").Value = produ.AvisoAjusteInventario?.ToString(this._identity.GetFormatProvider());
                form.GetField("ultimoCosto").Value = produ.UltimoCosto?.ToString(this._identity.GetFormatProvider());
                form.GetField("precioVenta").Value = produ.PrecioVenta?.ToString(this._identity.GetFormatProvider());
                form.GetField("display").Value = produ.TipoDisplay;
                form.GetField("ayudaColector").Value = produ.AyudaColector;
                form.GetField("componente1").Value = produ.Componente1;
                form.GetField("componente2").Value = produ.Componente2;
                form.GetField("anexo1").Value = produ.Anexo1;
                form.GetField("anexo2").Value = produ.Anexo2;
                form.GetField("anexo3").Value = produ.Anexo3;
                form.GetField("anexo4").Value = produ.Anexo4;
                form.GetField("anexo5").Value = produ.Anexo5;
                form.GetField("aceptaDecimales").Value = this._productoMapper.MapBooleanToString(produ.AceptaDecimales);
                form.GetField("grupoConsulta").Value = produ.GrupoConsulta;
                form.GetField("exclusivo").Value = produ.Exclusivo.ToString();

                form.GetField("codigoBase").Value = produ.CodigoBase;
                form.GetField("talle").Value = produ.Talle;
                form.GetField("color").Value = produ.Color;
                form.GetField("temporada").Value = produ.Temporada;
                form.GetField("categoria1").Value = produ.Categoria1;
                form.GetField("categoria2").Value = produ.Categoria2;
                form.GetField("ventanaLiberacion").Value = produ.VentanaLiberacion;

                // Carga de search Empresa
                var fieldEmpresa = form.GetField("empresa");
                fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
                {
                    SearchValue = produ.CodigoEmpresa.ToString()
                });
                fieldEmpresa.Value = produ.CodigoEmpresa.ToString();

                // Carga de search FamiliaProducto
                var fieldFamiliaProducto = form.GetField("familia");
                fieldFamiliaProducto.Options = SearchProductoFamilia(form, new FormSelectSearchContext()
                {
                    SearchValue = produ.Familia.Id.ToString()
                });
                fieldFamiliaProducto.Value = produ.Familia.Id.ToString();

                // Carga de search UnidadMedida
                var fieldUnidadMedida = form.GetField("unidadMedida");
                fieldUnidadMedida.Options = SearchUnidadMedida(form, new FormSelectSearchContext()
                {
                    SearchValue = produ.UnidadMedida.ToString()
                });

                fieldUnidadMedida.Value = produ.UnidadMedida.ToString();

                // Carga de search NCM
                var fieldNCM = form.GetField("NCM");

                if (!string.IsNullOrEmpty(produ.NAM))
                {
                    fieldNCM.Options = SearchNCM(form, new FormSelectSearchContext()
                    {
                        SearchValue = produ.NAM
                    });
                }
                fieldNCM.Value = produ.NAM;

                //Campos no editables
                form.GetField("empresa").ReadOnly = true;
                form.GetField("codigo").ReadOnly = true;
                form.GetField("mercadologico").ReadOnly = true;

                this.CamposNoEditablesPorStock(form, uow, idProducto, idEmpresa);
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);

            var cdProducto = form.GetField("codigo").Value;
            var empresa = int.Parse(form.GetField("empresa").Value);

            var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cdProducto);

            producto.Descripcion = form.GetField("descripcion").Value;
            producto.ManejoIdentificador = this._productoMapper.MapManejoIdentificador(form.GetField("manejoIdentificador")?.Value);
            producto.TipoManejoFecha = form.GetField("manejoFecha").Value;
            producto.Situacion = short.Parse(form.GetField("situacion").Value);
            producto.ModalidadIngresoLote = form.GetField("modalidadIngresoLote").Value;
            producto.UnidadDistribucion = decimal.Parse(form.GetField("unidadDistribucion")?.Value, this._identity.GetFormatProvider());
            producto.UnidadBulto = decimal.Parse(form.GetField("unidadBultos")?.Value, this._identity.GetFormatProvider());

            producto.UnidadMedida = form.GetField("unidadMedida").Value;

            producto.DiasLiberacion = this.CampoNulleableShort(form.GetField("diasLiberacion"));
            producto.DiasDuracion = this.CampoNulleableShort(form.GetField("diasDuracion"));
            producto.DiasValidez = this.CampoNulleableShort(form.GetField("diasValidez"));

            producto.AceptaDecimales = this._productoMapper.MapStringToBoolean(form.GetField("aceptaDecimales").Value);

            producto.StockMaximo = this.CampoNulleableEntero(form.GetField("stockMaximo"));
            producto.StockMinimo = this.CampoNulleableEntero(form.GetField("stockMinimo"));

            producto.PesoBruto = this.CampoNulleableDecimal(form.GetField("pesoBruto"));
            producto.PesoNeto = this.CampoNulleableDecimal(form.GetField("pesoNeto"));
            producto.Altura = this.CampoNulleableDecimal(form.GetField("altura"));
            producto.Ancho = this.CampoNulleableDecimal(form.GetField("ancho"));
            producto.Profundidad = this.CampoNulleableDecimal(form.GetField("profundidad"));
            producto.VolumenCC = this.CampoNulleableDecimal(form.GetField("volumenCC"));

            producto.GrupoConsulta = form.GetField("grupoConsulta").Value;

            producto.DescripcionReducida = form.GetField("reducida").Value;
            producto.CodigoMercadologico = form.GetField("mercadologico").Value;
            producto.NAM = form.GetField("NCM").Value;
            producto.CodigoProductoEmpresa = form.GetField("productoEmpresaRef").Value;

            producto.AvisoAjusteInventario = this.CampoNulleableDecimal(form.GetField("ajusteInventario"));

            producto.UltimoCosto = this.CampoNulleableDecimal(form.GetField("ultimoCosto"));
            producto.PrecioVenta = this.CampoNulleableDecimal(form.GetField("precioVenta"));

            producto.TipoDisplay = form.GetField("display").Value;
            producto.AyudaColector = form.GetField("ayudaColector").Value;
            producto.Componente1 = form.GetField("componente1").Value;
            producto.Componente2 = form.GetField("componente2").Value;

            producto.Anexo1 = form.GetField("anexo1").Value;
            producto.Anexo2 = form.GetField("anexo2").Value;
            producto.Anexo3 = form.GetField("anexo3").Value;
            producto.Anexo4 = form.GetField("anexo4").Value;
            producto.Anexo5 = form.GetField("anexo5").Value;

            producto.Exclusivo = this.CampoNulleableShort(form.GetField("exclusivo"));
            producto.CodigoRotatividad = this.CampoNulleableShort(form.GetField("rotatividad"));
            producto.Ramo = string.IsNullOrEmpty(form.GetField("ramo").Value) ? RamoProductoDb.General : short.Parse(form.GetField("ramo").Value);
            producto.CodigoClase = form.GetField("clase").Value;
            producto.CodigoFamilia = int.Parse(form.GetField("familia")?.Value);

            producto.CodigoBase = form.GetField("codigoBase").Value;
            producto.Talle = form.GetField("talle").Value;
            producto.Color = form.GetField("color").Value;
            producto.Temporada = form.GetField("temporada").Value;
            producto.Categoria1 = form.GetField("categoria1").Value;
            producto.Categoria2 = form.GetField("categoria2").Value;

            producto.NumeroTransaccion = uow.GetTransactionNumber();
            producto.FechaModificacion = DateTime.Now;
            producto.FechaSituacion = DateTime.Now;
            producto.VentanaLiberacion = form.GetField("ventanaLiberacion").Value;

            uow.ProductoRepository.UpdateProducto(producto);

            uow.SaveChanges();
            try
            {
                NotificarAutomatismo(uow, producto);

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (AutomatismoException ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoProductoFormValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                case "familia": return this.SearchProductoFamilia(form, context);
                case "unidadMedida": return this.SearchUnidadMedida(form, context);
                case "NCM": return this.SearchNCM(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(context.SearchValue))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProductoFamilia(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(context.SearchValue))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ProductoFamilia> familias = uow.ProductoFamiliaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var familia in familias)
            {
                opciones.Add(new SelectOption(familia.Id.ToString(), $"{familia.Id} - {familia.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchUnidadMedida(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(context.SearchValue))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<UnidadMedida> unidades = uow.UnidadMedidaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var unidad in unidades)
            {
                opciones.Add(new SelectOption(unidad.Id.ToString(), $"{unidad.Id} - {unidad.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchNCM(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(context.SearchValue))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<CodigoNomenclaturaComunMercosur> ncms = uow.NcmRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var ncm in ncms)
            {
                opciones.Add(new SelectOption(ncm.Id.ToString(), $"{ncm.Id} - {ncm.Descripcion}"));
            }

            return opciones;
        }

        public virtual void InicializarSelects(Form form, IUnitOfWork uow)
        {

            //Inicializar selects
            FormField selectClase = form.GetField("clase");
            FormField selectRotatividad = form.GetField("rotatividad");
            FormField selectRamo = form.GetField("ramo");
            FormField selectManejoIdentificador = form.GetField("manejoIdentificador");
            FormField selectSituacion = form.GetField("situacion");
            FormField selectDisplay = form.GetField("display");
            FormField selectManejoFecha = form.GetField("manejoFecha");
            FormField selectIngresoLote = form.GetField("modalidadIngresoLote");
            FormField selectComponente1 = form.GetField("componente1");
            FormField selectComponente2 = form.GetField("componente2");
            FormField selectManejoDiasLiberacion = form.GetField("manejoDiasLiberacion");
            FormField selectAceptaDecimales = form.GetField("aceptaDecimales");
            FormField selectGrupoConsulta = form.GetField("grupoConsulta");
            FormField selectVentanaLiberacion = form.GetField("ventanaLiberacion");

            selectClase.Options = new List<SelectOption>();
            selectRotatividad.Options = new List<SelectOption>();
            selectRamo.Options = new List<SelectOption>();
            selectManejoIdentificador.Options = new List<SelectOption>();
            selectSituacion.Options = new List<SelectOption>();
            selectDisplay.Options = new List<SelectOption>();
            selectManejoFecha.Options = new List<SelectOption>();
            selectComponente1.Options = new List<SelectOption>();
            selectComponente2.Options = new List<SelectOption>();
            selectManejoDiasLiberacion.Options = new List<SelectOption>();
            selectAceptaDecimales.Options = new List<SelectOption>();
            selectGrupoConsulta.Options = new List<SelectOption>();
            selectVentanaLiberacion.Options = new List<SelectOption>();

            // Clases
            List<Clase> clases = uow.ClaseRepository.GetClases();
            foreach (var clase in clases)
            {
                selectClase.Options.Add(new SelectOption(clase.Id.ToString(), $"{clase.Id} - {clase.Descripcion}")); ;
            }

            // Rotatividad
            List<ProductoRotatividad> rotatividades = uow.ProductoRotatividadRepository.GetProductoRotatividades();
            foreach (var rotatividad in rotatividades)
            {
                selectRotatividad.Options.Add(new SelectOption(rotatividad.Id.ToString(), $"{rotatividad.Id} - {rotatividad.Descripcion}")); ;
            }

            // Ramo
            List<ProductoRamo> ramos = uow.ProductoRamoRepository.GetProductoRamos();
            foreach (var ramo in ramos)
            {
                selectRamo.Options.Add(new SelectOption(ramo.Id.ToString(), $"{ramo.Id} - {ramo.Descripcion}")); ;
            }

            //ManejoIdentificador
            selectManejoIdentificador.Options = new List<SelectOption>
            {
                new SelectOption(this._productoMapper.MapManejoIdentificador(ManejoIdentificador.Serie),"REG009_frm_opt_ManejoIdentificador_Serie"),
                new SelectOption(this._productoMapper.MapManejoIdentificador(ManejoIdentificador.Lote),"REG009_frm_opt_ManejoIdentificador_Lote"),
                new SelectOption(this._productoMapper.MapManejoIdentificador(ManejoIdentificador.Producto),"REG009_frm_opt_ManejoIdentificador_Producto"),
            };

            //ManejoFecha
            selectManejoFecha.Options = new List<SelectOption>
            {
                new SelectOption(this._productoMapper.MapManejoFecha(ManejoFechaProducto.Duradero),"REG009_frm_opt_ManejoFecha_Duradero"),
                new SelectOption(this._productoMapper.MapManejoFecha(ManejoFechaProducto.Fifo),"REG009_frm_opt_ManejoFecha_Fifo"),
                new SelectOption(this._productoMapper.MapManejoFecha(ManejoFechaProducto.Expirable),"REG009_frm_opt_ManejoFecha_Fefo"),
            };

            //Modalidad ingreso lote

            selectIngresoLote.Options = new List<SelectOption>
            {
                new SelectOption(ModalidadIngresoLoteDb.Normal,"REG009_frm_opt_ModalidadLote_Normal"),
                new SelectOption(ModalidadIngresoLoteDb.Vencimiento,"REG009_frm_opt_ModalidadLote_Vencimiento"),
                new SelectOption(ModalidadIngresoLoteDb.Agenda,"REG009_frm_opt_ModalidadLote_Agenda"),
                new SelectOption(ModalidadIngresoLoteDb.Documento,"REG009_frm_opt_ModalidadLote_Documento"),
                new SelectOption(ModalidadIngresoLoteDb.VencimientoYYYYMM,"REG009_frm_opt_ModalidadLote_VencimientoYYYYMM"),
            };

            //Situacion
            selectSituacion.Options = new List<SelectOption>
            {
                //TODO: pasarlo a enumerado
                new SelectOption(SituacionDb.Activo.ToString(),"REG009_frm_opt_Situacion_Activo"),
                new SelectOption(SituacionDb.Inactivo.ToString(),"REG009_frm_opt_Situacion_Inactivo"),
            };

            //Display
            selectDisplay.Options = new List<SelectOption>
            {
                new SelectOption(TipoDisplayDb.Codigo,"REG009_frm_opt_Display_Codigo"),
                new SelectOption(TipoDisplayDb.Descripcion,"REG009_frm_opt_Display_Descripcion"),
            };

            //Componente1
            Componente1Producto dbQuery = new Componente1Producto();
            uow.HandleQuery(dbQuery);
            var componentes1 = dbQuery.GetComponentes();
            foreach (var componente in componentes1)
            {
                selectComponente1.Options.Add(new SelectOption(componente, componente));
            }

            //Componente2
            var componentes2 = uow.DominioRepository.GetDominios(FacturacionDb.FacturaProductoCompra);
            foreach (var componente in componentes2)
            {
                selectComponente2.Options.Add(new SelectOption(componente.Id, $"{componente.Id} - {componente.Descripcion}"));
            }

            //Manejo dias liberacion
            List<Validez> listValidez = uow.ValidezRepository.GetListaValidez();
            foreach (var validez in listValidez)
            {
                selectManejoDiasLiberacion.Options.Add(new SelectOption(validez.Id, $"{validez.Id} - {validez.Descripcion}"));
            }

            //AceptaDecimales
            selectAceptaDecimales.Options = new List<SelectOption> {
                new SelectOption("S","REG009_frm_opt_decimales_SI"),
                new SelectOption("N","REG009_frm_opt_decimales_NO"),
            };
            form.GetField("aceptaDecimales").Value = "N";

            //Grupo consulta
            List<GrupoConsulta> gruposAsignados = uow.GrupoConsultaRepository.GetGrupoConsultaAsignados(this._identity.UserId);
            foreach (var grupo in gruposAsignados)
            {
                selectGrupoConsulta.Options.Add(new SelectOption(grupo.Id, $"{grupo.Id} - {grupo.Descripcion}"));
            }
            form.GetField("grupoConsulta").Value = "S/N";

            List<DominioDetalle> dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.VentanaLiberacion);
            foreach (var dominio in dominios)
            {
                selectVentanaLiberacion.Options.Add(new SelectOption(dominio.Valor.ToString(), $"{dominio.Valor} - {dominio.Descripcion}")); ;
            }


        }

        public virtual void NotificarAutomatismo(IUnitOfWork uow, Producto producto)
        {
            if (_automatismoAutoStoreClientService.IsEnabled()
                && uow.AutomatismoRepository.AnyAutomatismo(producto))
            {
                _automatismoAutoStoreClientService.SendProductos(new ProductosAutomatismoRequest
                {
                    DsReferencia = producto.NumeroTransaccion.ToString(),
                    Empresa = producto.CodigoEmpresa,
                    Productos = new List<ProductoAutomatismoRequest>
                    {
                        new ProductoAutomatismoRequest
                        {
                            Altura = producto.Altura,
                            Ancho = producto.Ancho,
                            Codigo = producto.Codigo,
                            Descripcion = producto.Descripcion,
                            ManejoIdentificador = this._productoMapper.MapManejoIdentificador(producto.ManejoIdentificador),
                            PesoBruto = producto.PesoBruto,
                            Profundidad = producto.Profundidad,
                            TipoManejoFecha = producto.TipoManejoFecha,
                            UnidadBulto = producto.UnidadBulto
                        },
                    },
                });
            }
        }

        public virtual void CamposNoEditablesPorStock(Form form, IUnitOfWork uow, string producto, int empresa)
        {

        }

        public virtual decimal? CampoNulleableDecimal(FormField field)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;
            else
                return decimal.Parse(field.Value, this._identity.GetFormatProvider());
        }

        public virtual int? CampoNulleableEntero(FormField field)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;
            else
                return int.Parse(field.Value);
        }

        public virtual short? CampoNulleableShort(FormField field)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;
            else
                return short.Parse(field.Value);
        }

        #endregion

    }
}
