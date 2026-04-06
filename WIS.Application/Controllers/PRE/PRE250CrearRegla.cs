using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.CheckboxListComponent;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE250CrearRegla : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public PRE250CrearRegla(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (query.Parameters.Count > 0)
            {
                if (query.Parameters.Count > 1)
                    this.InicializarSelects(uow, form, query, true);
                else if (int.TryParse(query.Parameters.Find(x => x.Id == "nuRegla")?.Value, out int nuRegla))
                    this.InicializarSelectsUpdate(uow, form, query, nuRegla);
                else
                    this.InicializarSelects(uow, form, query);//por prevencion
            }
            else
                this.InicializarSelects(uow, form, query);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new PRE250ReglasValidationModule(uow, this._identity.UserId, this._identity.GetFormatProvider()), form, context);
        }

        public virtual void InicializarSelects(IUnitOfWork uow, Form form, FormInitializeContext query, bool sesion = false)
        {
            ////Inicializar selects

            List<CheckboxListItem> checkListDias = new List<CheckboxListItem>();
            var tpFrecuencia = form.GetField("tpFrecuencia");

            if (sesion)
            {
                form.GetField("descripcion").Value = query.Parameters.FirstOrDefault(s => s.Id == "descripcion")?.Value;
                form.GetField("nuOrden").Value = query.Parameters.FirstOrDefault(s => s.Id == "nuOrden")?.Value;
                form.GetField("dtInicio").Value = query.Parameters.FirstOrDefault(s => s.Id == "dtInicio")?.Value;
                form.GetField("dtFin").Value = query.Parameters.FirstOrDefault(s => s.Id == "dtFin")?.Value;
                form.GetField("nuFrecuencia").Value = query.Parameters.FirstOrDefault(s => s.Id == "nuFrecuencia")?.Value;
                form.GetField("activa").Value = query.Parameters.FirstOrDefault(s => s.Id == "activa")?.Value;
                form.GetField("respetarIntervalos").Value = query.Parameters.FirstOrDefault(s => s.Id == "respetarIntervalos")?.Value;

                string tpFrec = query.Parameters.FirstOrDefault(s => s.Id == "tpFrecuencia")?.Value;

                tpFrecuencia.Options = new List<SelectOption>
                {
                    new SelectOption("M", "Minutos"),
                    new SelectOption("H", "Horas"),
                    new SelectOption("D", "Diario")
                };
                tpFrecuencia.Value = tpFrec;

                if (tpFrec != "D")
                {
                    var hIni = TimeSpan.Parse(query.Parameters.FirstOrDefault(s => s.Id == "horaInicio").Value, this._identity.GetFormatProvider());
                    var hFin = TimeSpan.Parse(query.Parameters.FirstOrDefault(s => s.Id == "horaFin").Value, this._identity.GetFormatProvider());

                    form.GetField("horaFin").Disabled = false;
                    form.GetField("horaInicio").Disabled = false;
                    form.GetField("horaInicio").Value = hIni.ToString(@"hh\:mm", this._identity.GetFormatProvider());
                    form.GetField("horaFin").Value = hFin.ToString(@"hh\:mm", this._identity.GetFormatProvider());
                }
                else
                {
                    form.GetField("horaFin").Disabled = true;
                    form.GetField("horaInicio").Disabled = true;
                }

                checkListDias = JsonConvert.DeserializeObject<List<CheckboxListItem>>(query.Parameters.Find(x => x.Id == "dias").Value);
            }
            else
            {
                form.GetField("descripcion").Value = string.Empty;
                form.GetField("horaInicio").Value = string.Empty;
                form.GetField("horaFin").Value = string.Empty;
                form.GetField("nuOrden").Value = string.Empty;
                form.GetField("nuFrecuencia").Value = string.Empty;
                form.GetField("activa").Value = "true";
                form.GetField("respetarIntervalos").Value = uow.ParametroRepository.GetParameter("PRE250_DEFAULT_RESP_INTERVALO") == "S" ? "true" : "false";

                tpFrecuencia.Options = new List<SelectOption>
                {
                    new SelectOption("M", "Minutos"),
                    new SelectOption("H", "Horas"),
                    new SelectOption("D", "Diario")
                };
                tpFrecuencia.Value = string.Empty;

                checkListDias.Add(new CheckboxListItem { Id = "1", Label = "Lunes", Selected = true });
                checkListDias.Add(new CheckboxListItem { Id = "2", Label = "Martes", Selected = false });
                checkListDias.Add(new CheckboxListItem { Id = "3", Label = "Miércoles", Selected = false });
                checkListDias.Add(new CheckboxListItem { Id = "4", Label = "Jueves", Selected = false });
                checkListDias.Add(new CheckboxListItem { Id = "5", Label = "Viernes", Selected = false });
                checkListDias.Add(new CheckboxListItem { Id = "6", Label = "Sábado", Selected = false });
                checkListDias.Add(new CheckboxListItem { Id = "7", Label = "Domingo", Selected = false });
            }
            query.AddParameter("ListItemsDias", JsonConvert.SerializeObject(checkListDias));
        }

        public virtual void InicializarSelectsUpdate(IUnitOfWork uow, Form form, FormInitializeContext query, int nuRegla)
        {
            ////Inicializar selects

            var regla = uow.LiberacionRepository.GetReglaLiberacion(nuRegla, false);

            form.GetField("descripcion").Value = regla.DsRegla;
            form.GetField("nuOrden").Value = regla.NuOrden.ToString();

            if (regla.DtInicio != null)
                form.GetField("dtInicio").Value = ((DateTime)regla.DtInicio).ToString("o");
            if (regla.DtFin != null)
                form.GetField("dtFin").Value = ((DateTime)regla.DtFin).ToString("o");

            form.GetField("nuFrecuencia").Value = regla.NuFrecuencia.ToString();
            form.GetField("activa").Value = regla.FlActiva ? "true" : "false";
            form.GetField("respetarIntervalos").Value = regla.RespetarIntervalo ? "true" : "false";

            var tpFrecuencia = form.GetField("tpFrecuencia");
            tpFrecuencia.Options = new List<SelectOption>
            {
                new SelectOption("M", "Minutos"),
                new SelectOption("H", "Horas"),
                new SelectOption("D", "Diario")
            };
            tpFrecuencia.Value = regla.TpFrecuencia;

            if (regla.TpFrecuencia != "D")
            {
                form.GetField("horaFin").Disabled = false;
                form.GetField("horaInicio").Disabled = false;
                //form.GetField("horaInicio").Value = TimeSpan.FromMilliseconds((double)(regla.hrInicio ?? 0)).ToString(@"hh\:mm", this._identity.GetFormatProvider());
                //form.GetField("horaFin").Value = TimeSpan.FromMilliseconds((double)(regla.hrFin ?? 0)).ToString(@"hh\:mm", this._identity.GetFormatProvider());
                form.GetField("horaInicio").Value = regla.HrInicio?.ToString(@"hh\:mm", this._identity.GetFormatProvider());
                form.GetField("horaFin").Value = regla.HrFin?.ToString(@"hh\:mm", this._identity.GetFormatProvider());
            }
            else
            {
                form.GetField("horaFin").Disabled = true;
                form.GetField("horaInicio").Disabled = true;
            }
            query.AddParameter("ListItemsDias", JsonConvert.SerializeObject(CargarDias(regla.DsDias)));
        }
        public virtual List<CheckboxListItem> CargarDias(string dias)
        {
            List<CheckboxListItem> checkListDias = new List<CheckboxListItem>();
            checkListDias.Add(new CheckboxListItem { Id = "1", Label = "Lunes", Selected = dias.Contains("1") ? true : false });
            checkListDias.Add(new CheckboxListItem { Id = "2", Label = "Martes", Selected = dias.Contains("2") ? true : false });
            checkListDias.Add(new CheckboxListItem { Id = "3", Label = "Miércoles", Selected = dias.Contains("3") ? true : false });
            checkListDias.Add(new CheckboxListItem { Id = "4", Label = "Jueves", Selected = dias.Contains("4") ? true : false });
            checkListDias.Add(new CheckboxListItem { Id = "5", Label = "Viernes", Selected = dias.Contains("5") ? true : false });
            checkListDias.Add(new CheckboxListItem { Id = "6", Label = "Sábado", Selected = dias.Contains("6") ? true : false });
            checkListDias.Add(new CheckboxListItem { Id = "7", Label = "Domingo", Selected = dias.Contains("7") ? true : false });
            return checkListDias;
        }
    }
}
