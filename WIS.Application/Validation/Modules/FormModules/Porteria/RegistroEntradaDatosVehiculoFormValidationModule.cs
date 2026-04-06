using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Porteria;
using WIS.Domain.Recepcion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Porteria
{
    public class RegistroEntradaDatosVehiculoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly int _user;
        protected readonly bool _isSubmitting;
        protected readonly string _predioUsuario;

        public RegistroEntradaDatosVehiculoFormValidationModule(IUnitOfWork uow,
           int user, string predioUsuario, bool isSubmitting,
             IFormatProvider culture)
        {
            this._uow = uow;
            this._user = user;
            this._predioUsuario = predioUsuario;
            this._isSubmitting = isSubmitting;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["CD_TRANSPORTE_V1"] = this.ValidateCD_TRANSPORTE_V1,
                ["VL_MATRICULA_1_ENTRADA"] = this.ValidateVL_MATRICULA_1_ENTRADA,
                ["VL_MATRICULA_2_ENTRADA"] = this.ValidateVL_MATRICULA_2_ENTRADA,
                ["VL_PESO_ENTRADA_VE"] = this.ValidateVL_PESO_ENTRADA_VE,
                ["CD_SECTOR"] = this.ValidateCD_SECTOR,
                ["NU_PREDIO"] = this.ValidateNU_PREDIO,

            };
        }

        public virtual FormValidationGroup ValidateCD_TRANSPORTE_V1(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 20),
                   new ExisteDominioValidationRule(field.Value, this._uow),
                },
            };
        }
        public virtual FormValidationGroup ValidateVL_MATRICULA_1_ENTRADA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) && form.GetField("CD_TRANSPORTE_V1").Value == "PORREGSITAPI")
            {
                this.CargarDatos_OnSuccess(field, form, parameters);
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 30),
                },
                OnSuccess = this.CargarDatos_OnSuccess,
                OnFailure = this.CargarDatos_OnSuccess,

            };
        }
        public virtual void CargarDatos_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (this._isSubmitting) return;

            try
            {
                FormField fieldNU_PREDIO = form.GetField("NU_PREDIO");
                FormField fieldCD_EMPRESA = form.GetField("CD_EMPRESA");
                FormField fieldCD_CLIENTE = form.GetField("CD_CLIENTE");
                FormField fieldCD_SECTOR = form.GetField("CD_SECTOR");

                FormField fieldCD_TP_POTERIA_REGISTRO = form.GetField("CD_TP_POTERIA_REGISTRO");
                FormField fieldCD_TRANSPORTE_V1 = form.GetField("CD_TRANSPORTE_V1");
                FormField fieldVL_MATRICULA_1_ENTRADA = form.GetField("VL_MATRICULA_1_ENTRADA");
                FormField fieldVL_MATRICULA_2_ENTRADA = form.GetField("VL_MATRICULA_2_ENTRADA");
                FormField fieldCD_POTERIA_MOTIVO = form.GetField("CD_POTERIA_MOTIVO");

                Agenda agenda = string.IsNullOrEmpty(field.Value) ? null : this._uow.AgendaRepository.GetAgendaSinPorteriaByMatriculaVehiculo(field.Value);

                PorteriaRegistroVehiculo preRegistro = this._uow.PorteriaRepository.GetPreRegistroVehiculoByMatricula(field.Value);

                if ((agenda != null || preRegistro != null) && field.Value != "")
                {

                    if (agenda != null)
                    {
                        if (form.Id == "POR080_form_1")
                        {

                            ComponentParameter parameterContainers = parameters.FirstOrDefault(w => w.Id == "SelectionGridContainers");

                            if (parameterContainers != null)
                            {
                                parameterContainers.Value = JsonConvert.SerializeObject(this._uow.PorteriaRepository.GetContainersAgenda(agenda.Id).Select(w => w.Id));
                            }

                        }

                        fieldNU_PREDIO.Value = agenda.Predio ?? fieldNU_PREDIO.Options?.FirstOrDefault()?.Value ?? "";
                        fieldNU_PREDIO.ReadOnly = true;

                        fieldCD_EMPRESA.Value = agenda.IdEmpresa.ToString();
                        fieldCD_EMPRESA.Options = new List<SelectOption> { new SelectOption(agenda.IdEmpresa.ToString(), this._uow.EmpresaRepository.GetNombre(agenda.IdEmpresa).ToString()) };
                        fieldCD_EMPRESA.ReadOnly = true;

                        fieldCD_CLIENTE.Value = agenda.CodigoInternoCliente;
                        fieldCD_CLIENTE.Options = new List<SelectOption> { new SelectOption(agenda.CodigoInternoCliente, this._uow.AgenteRepository.GetDescripcionAgente(agenda.IdEmpresa, agenda.CodigoInternoCliente)) };
                        fieldCD_CLIENTE.ReadOnly = true;

                        fieldCD_SECTOR.Options = SelectSector(this._uow, agenda.Predio);
                        fieldCD_SECTOR.Value = this._uow.PorteriaRepository.GetSector(agenda.Predio, agenda.CodigoPuerta)?.CD_SECTOR ??
                            fieldCD_SECTOR.Options?.FirstOrDefault().Value ?? "";

                        parameters.Add(new ComponentParameter { Id = "SelectionGridPersonas", Value = "" });

                    }
                    else
                    {

                        fieldNU_PREDIO.Value = preRegistro.NU_PREDIO ?? fieldNU_PREDIO.Options?.FirstOrDefault()?.Value ?? "";
                        fieldNU_PREDIO.ReadOnly = true;

                        fieldCD_EMPRESA.Value = preRegistro.CD_EMPRESA?.ToString() ?? "";

                        if (preRegistro.CD_EMPRESA != null)
                        {
                            fieldCD_EMPRESA.Options = new List<SelectOption> { new SelectOption(preRegistro.CD_EMPRESA.ToString(), this._uow.EmpresaRepository.GetNombre((int)preRegistro.CD_EMPRESA).ToString()) };
                        }
                        else
                        {
                            fieldCD_EMPRESA.Options = new List<SelectOption>();
                        }

                        fieldCD_EMPRESA.ReadOnly = true;

                        if (preRegistro.CD_EMPRESA != null &&
                            !string.IsNullOrEmpty(preRegistro.CD_AGENTE) && !string.IsNullOrEmpty(preRegistro.TP_AGENTE)
                            )
                        {
                            Agente agente = this._uow.AgenteRepository.GetAgente(
                                (int)preRegistro.CD_EMPRESA, preRegistro.CD_AGENTE, preRegistro.TP_AGENTE);

                            fieldCD_CLIENTE.Value = agente.CodigoInterno;
                            fieldCD_CLIENTE.Options = new List<SelectOption> { new SelectOption(agente.CodigoInterno, this._uow.AgenteRepository.GetDescripcionAgente(agente.Empresa, agente.CodigoInterno)) };
                        }
                        else
                        {
                            fieldCD_CLIENTE.Value = "";
                            fieldCD_CLIENTE.Options = new List<SelectOption>();
                        }

                        fieldCD_CLIENTE.ReadOnly = true;

                        fieldCD_SECTOR.Value = preRegistro.CD_SECTOR ??
                            fieldCD_SECTOR.Options?.FirstOrDefault().Value ?? "";
                        fieldCD_SECTOR.ReadOnly = true;

                        //fieldCD_TP_POTERIA_REGISTRO.Value = preRegistro.nd

                        fieldCD_TRANSPORTE_V1.Value = preRegistro.ND_TRANSPORTE ?? "";
                        fieldCD_TRANSPORTE_V1.ReadOnly = true;

                        field.Value = preRegistro.VL_MATRICULA_1 ?? "";
                        field.ReadOnly = true;

                        fieldVL_MATRICULA_2_ENTRADA.Value = preRegistro.VL_MATRICULA_2 ?? "";
                        fieldVL_MATRICULA_2_ENTRADA.ReadOnly = true;

                        fieldCD_POTERIA_MOTIVO.Value = preRegistro.ND_POTERIA_MOTIVO ?? fieldCD_POTERIA_MOTIVO.Options?.FirstOrDefault()?.Value ?? "";
                        fieldCD_POTERIA_MOTIVO.ReadOnly = true;

                        parameters.Add(new ComponentParameter
                        {
                            Id = "SelectionGridPersonas",
                            Value = JsonConvert.SerializeObject(preRegistro.Personas.Select(w => w.NU_POTERIA_PERSONA).ToList())
                        });
                    }
                }
                else
                {



                    fieldCD_POTERIA_MOTIVO.Value = fieldCD_POTERIA_MOTIVO.Options?.FirstOrDefault()?.Value ?? "";
                    fieldCD_POTERIA_MOTIVO.ReadOnly = false;

                    fieldCD_SECTOR.Value = fieldCD_SECTOR.Options?.FirstOrDefault()?.Value ?? "";
                    fieldCD_SECTOR.ReadOnly = false;

                    fieldNU_PREDIO.Value = fieldNU_PREDIO.Options?.FirstOrDefault()?.Value ?? "";
                    fieldNU_PREDIO.ReadOnly = false;

                    fieldCD_EMPRESA.Value = "";
                    fieldCD_EMPRESA.Options = new List<SelectOption> { };
                    fieldCD_EMPRESA.ReadOnly = false;

                    fieldCD_CLIENTE.Value = "";
                    fieldCD_CLIENTE.Options = new List<SelectOption> { };
                    fieldCD_CLIENTE.ReadOnly = false;

                    parameters.Add(new ComponentParameter { Id = "SelectionGridPersonas", Value = "" });

                    if (form.Id != "POR010_form_1")
                    {
                        //throw new WISException("Este vehiculo ya tiene un pre-registro, no puede reingresar otro.");
                    }

                }
            }
            catch (Exception ex)
            {
                field.SetError(ex.Message);
            }

        }
        public virtual FormValidationGroup ValidateVL_MATRICULA_2_ENTRADA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 30),

                },
            };
        }
        public virtual FormValidationGroup ValidateVL_PESO_ENTRADA_VE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NumberValidationRule<decimal?>(field.Value,_culture),

                },
            };
        }
        public virtual FormValidationGroup ValidateCD_SECTOR(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 20),
                },
            };
        }
        public virtual FormValidationGroup ValidateNU_PREDIO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new ExistePredioValidationRule(this._uow, this._user, this._predioUsuario, field.Value)
                }
            };
        }

        public virtual List<SelectOption> SelectSector(IUnitOfWork uow, string predio)
        {
            return uow.PorteriaRepository.GetSectoresByPredio(predio)
                   .Select(w => new SelectOption(w.CD_SECTOR, w.DS_SECTOR))
                   .ToList();
        }
    }
}
