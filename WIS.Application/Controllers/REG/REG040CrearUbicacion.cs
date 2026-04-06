using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Recorridos;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG040CrearUbicacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG040CrearUbicacion> _logger;
        protected readonly IFormValidationService _formValidationService;

        public REG040CrearUbicacion(IIdentityService identity, IUnitOfWorkFactory uowFactory, ILogger<REG040CrearUbicacion> logger, IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._logger = logger;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(form, uow);
            this.InicializarCamposConParametros(form, context, uow);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("FormSubmit", _identity.Application, _identity.UserId);

            try
            {
                var configuracion = uow.UbicacionRepository.GetUbicacionConfiguracion();
                var contadorDeUbicacionesCreadas = 0;
                var ubicacionesACrear = new List<string>();
                var recorridoPorDefecto = uow.RecorridoRepository.GetRecorridoPorDefectoParaPredio(form.GetField("numeroPredio").Value);

                // Itero en columnas desde - hasta

                var saltoColumna = form.GetField("columnaSalto").Value.Equals("0") ? 1 : int.Parse(form.GetField("columnaSalto").Value);

                for (int columna = int.Parse(form.GetField("columnaDesde").Value); columna <= int.Parse(form.GetField("columnaHasta").Value); columna += saltoColumna)
                {
                    // Consulto si debo omitir la creación de columnas pares 
                    if (form.GetField("omitirPares").Value.Equals("pares") && columna % 2 == 0)
                        continue;

                    // Consulto si debo omitir la creación de columnas impares 
                    if (form.GetField("omitirPares").Value.Equals("impares") && columna % 2 != 0)
                        continue;

                    var saltoAltura = form.GetField("alturaSalto").Value.Equals("0") ? 1 : int.Parse(form.GetField("alturaSalto").Value);
                    var area = uow.UbicacionAreaRepository.GetUbicacionArea(short.Parse(form.GetField("idArea").Value));

                    for (int altura = int.Parse(form.GetField("alturaDesde").Value); altura <= int.Parse(form.GetField("alturaHasta").Value); altura += saltoAltura)
                    {
                        var ubicacion = GenerarUbicacion(uow, form, configuracion, columna, altura);

                        if (ubicacion.Id.Length > 40)
                            throw new ValidationFailedException("REG040_Frm1_Error_LargoTotalUbicacionInvalido", new string[] { ubicacion.Id });

                        uow.UbicacionRepository.AddUbicacion(ubicacion);

                        // Solo agrega detalle al recorrido por defecto si es área recorrible
                        if (area.EsAreaMantenible && ((area.EsAreaStockGeneral && area.DisponibilizaStock) || (area.EsAreaPicking && area.DisponibilizaStock) || area.EsAreaAveria))
                        {
                            InsertarDetalleRecorrido(uow, recorridoPorDefecto, ubicacion);
                        }

                        contadorDeUbicacionesCreadas++;
                        ubicacionesACrear.Add(ubicacion.Id);
                    }
                }

                // Se controla que las ubicaciones generadas no existan
                foreach (var id in ubicacionesACrear)
                {
                    // Verifico que la ubicacion no exista
                    if (uow.UbicacionRepository.AnyUbicacion(id))
                        throw new ValidationFailedException("REG040_Frm1_Error_UbicacionExiste", new string[] { id });
                    else if (id.Length > 40)
                        throw new ValidationFailedException("REG040_Frm1_Error_LargoUbicacionNoPermitida", new string[] { id });
                }

                uow.SaveChanges();

                context.AddSuccessNotification("REG040_Frm1_Succes_Creacion", new List<string>() { contadorDeUbicacionesCreadas.ToString() });
                context.ResetForm = true;
            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "FormSubmit");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FormSubmit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CrearUbicacionFormValidationModule(uow, this._identity), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "idEmpresa": return this.SearchEmpresa(form, context);
                case "idProductoFamilia": return this.SearchProductoFamilia(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Auxs

        public virtual void InicializarSelects(Form form, IUnitOfWork uow)
        {
            //Inicializar selects
            FormField selectArea = form.GetField("idArea");
            FormField selectTipoUbicacion = form.GetField("idTipoUbicacion");
            FormField selectClase = form.GetField("idProductoClase");
            FormField selectRotatividad = form.GetField("idProductoRotatividad");
            FormField selectPredio = form.GetField("numeroPredio");
            FormField selectPares = form.GetField("omitirPares");
            FormField selectZonas = form.GetField("idZonaUbicacion");
            FormField selectControlAcceso = form.GetField("controlAcceso");

            selectArea.Options = new List<SelectOption>();
            selectTipoUbicacion.Options = new List<SelectOption>();
            selectClase.Options = new List<SelectOption>();
            selectRotatividad.Options = new List<SelectOption>();
            selectPredio.Options = new List<SelectOption>();
            selectPares.Options = new List<SelectOption>();
            selectZonas.Options = new List<SelectOption>();
            selectControlAcceso.Options = new List<SelectOption>();


            // Areas
            List<UbicacionArea> areas = uow.UbicacionAreaRepository.GetUbicacionAreasMantenibles();
            foreach (var area in areas)
            {
                selectArea.Options.Add(new SelectOption(area.Id.ToString(), $"{area.Id} - {area.Descripcion}"));
            }

            // Tipos ubicación
            List<UbicacionTipo> tiposUbicaciones = uow.UbicacionTipoRepository.GetUbicacionTipos();
            foreach (var tipo in tiposUbicaciones)
            {
                selectTipoUbicacion.Options.Add(new SelectOption(tipo.Id.ToString(), $"{tipo.Id} - {tipo.Descripcion}"));
            }

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

            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            // Omitir pares / impares
            selectPares.Options.Add(new SelectOption("noOmitir", "REG040_frm1_select_NoOmitir"));
            selectPares.Options.Add(new SelectOption("pares", "REG040_frm1_select_OmitirPares"));
            selectPares.Options.Add(new SelectOption("impares", "REG040_frm1_select_OmitirImpares"));
            selectPares.Value = "noOmitir";

            // Zonas
            List<ZonaUbicacion> zonas = uow.ZonaUbicacionRepository.GetZonasHabilitadas();
            foreach (var zona in zonas)
            {
                selectZonas.Options.Add(new SelectOption(zona.Id, $"{zona.Id} - {zona.Descripcion}"));
            }

            // Control Acceso
            List<ControlAcceso> controlesAcceso = uow.ZonaUbicacionRepository.GetControlesAcceso();
            foreach (var controlAcceso in controlesAcceso)
            {
                selectControlAcceso.Options.Add(new SelectOption(controlAcceso.Id, $"{controlAcceso.Id} - {controlAcceso.Descripcion}"));
            }
        }

        public virtual void InicializarCamposConParametros(Form form, FormInitializeContext context, IUnitOfWork uow)
        {

            if (context.Parameters.FirstOrDefault(s => s.Id == "idEmpresa") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idEmpresa").Value))
            {
                // Carga de search Empresa
                var fieldEmpresa = form.GetField("idEmpresa");
                fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
                {
                    SearchValue = context.Parameters.FirstOrDefault(s => s.Id == "idEmpresa").Value
                }); ;

                fieldEmpresa.Value = context.Parameters.FirstOrDefault(s => s.Id == "idEmpresa").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "idArea") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idArea").Value))
            {
                form.GetField("idArea").Value = context.Parameters.FirstOrDefault(s => s.Id == "idArea").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "idTipoUbicacion") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idTipoUbicacion").Value))
            {
                form.GetField("idTipoUbicacion").Value = context.Parameters.FirstOrDefault(s => s.Id == "idTipoUbicacion").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "idProductoClase") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idProductoClase").Value))
            {
                form.GetField("idProductoClase").Value = context.Parameters.FirstOrDefault(s => s.Id == "idProductoClase").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "idProductoFamilia") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idProductoFamilia").Value))
            {
                form.GetField("idProductoFamilia").Value = context.Parameters.FirstOrDefault(s => s.Id == "idProductoFamilia").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "idProductoRotatividad") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "idProductoRotatividad").Value))
            {
                form.GetField("idProductoRotatividad").Value = context.Parameters.FirstOrDefault(s => s.Id == "idProductoRotatividad").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "numeroPredio") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "numeroPredio").Value))
            {
                form.GetField("numeroPredio").Value = context.Parameters.FirstOrDefault(s => s.Id == "numeroPredio").Value;
            }

            if (context.Parameters.FirstOrDefault(s => s.Id == "codigoBloque") != null && !string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "codigoBloque").Value))
            {
                form.GetField("codigoBloque").Value = context.Parameters.FirstOrDefault(s => s.Id == "codigoBloque").Value;
            }
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

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

            using var uow = this._uowFactory.GetUnitOfWork();

            List<ProductoFamilia> familias = uow.ProductoFamiliaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var familia in familias)
            {
                opciones.Add(new SelectOption(familia.Id.ToString(), $"{familia.Id} - {familia.Descripcion}"));
            }

            return opciones;
        }

        public virtual Ubicacion GenerarUbicacion(IUnitOfWork uow, Form form, UbicacionConfiguracion configuracion, int columna, int altura)
        {
            var ubicacion = new Ubicacion()
            {
                Columna = columna,
                Altura = altura,

                IdEmpresa = int.Parse(form.GetField("idEmpresa").Value),
                IdUbicacionArea = short.Parse(form.GetField("idArea").Value),
                IdUbicacionTipo = short.Parse(form.GetField("idTipoUbicacion").Value),
                CodigoClase = form.GetField("idProductoClase").Value,
                IdProductoFamilia = int.Parse(form.GetField("idProductoFamilia").Value),
                IdProductoRotatividad = short.Parse(form.GetField("idProductoRotatividad").Value),
                EsUbicacionBaja = bool.Parse(form.GetField("ubicacionBaja").Value),
                CodigoSituacion = configuracion.EstadoCreacion,
                NumeroPredio = form.GetField("numeroPredio").Value,
                NecesitaReabastecer = false,
                Profundidad = 1,
                IdControlAcceso = form.GetField("controlAcceso").Value
            };

            if (string.IsNullOrEmpty(form.GetField("idZonaUbicacion").Value))
                ubicacion.IdUbicacionZona = configuracion.UbicacionZonaPorDefecto;
            else
                ubicacion.IdUbicacionZona = form.GetField("idZonaUbicacion").Value;

            if (configuracion.BloqueNumerico)
                ubicacion.Bloque = form.GetField("codigoBloque").Value.PadLeft(configuracion.BloqueLargo, '0').ToUpper();
            else
                ubicacion.Bloque = form.GetField("codigoBloque").Value.ToUpper();

            if (configuracion.CalleNumerico)
                ubicacion.Calle = form.GetField("codigoCalle").Value.PadLeft(configuracion.CalleLargo, '0').ToUpper();
            else
                ubicacion.Calle = form.GetField("codigoCalle").Value.ToUpper();

            ubicacion.Id = $"{ubicacion.NumeroPredio}{ubicacion.Bloque}{ubicacion.Calle}{columna.ToString().PadLeft(configuracion.ColumnaLargo, '0')}{altura.ToString().PadLeft(configuracion.AlturaLargo, '0')}";
            ubicacion.CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacion.Id}";

            return ubicacion;
        }


        public virtual void InsertarDetalleRecorrido(IUnitOfWork uow, Recorrido recorridoPorDefecto, Ubicacion ubicacion)
        {
            var detalleRecorrido = new DetalleRecorrido
            {
                IdRecorrido = recorridoPorDefecto.Id,
                Ubicacion = ubicacion.Id,
                ValorOrden = ubicacion.Id,
                NumeroOrden = -1,
                Transaccion = uow.GetTransactionNumber()
            };

            uow.RecorridoRepository.AddDetalleRecorrido(detalleRecorrido);
        }
        #endregion
    }
}
