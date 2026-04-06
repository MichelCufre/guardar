using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.CheckboxListComponent;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoAgendaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;
        protected readonly string _predioLogueado;

        public MantenimientoAgendaFormValidationModule(IUnitOfWork uow, int idUsuario, string predioLogueado)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;
            this._predioLogueado = predioLogueado;

            this.Schema = new FormValidationSchema
            {
                ["idEmpresa"] = this.ValidateIdEmpresa,
                ["codigoInternoAgente"] = this.ValidateCodigoInternoAgente,
                ["numeroPredio"] = this.ValidateNumeroPredio,
                ["tipoRecepcionExterno"] = this.ValidateTipoRecepcionExterno,

                ["fechaEntrega"] = this.ValidateFechaEntrega,
                ["funcionarioAsignado"] = this.ValidateFuncionarioAsignado,
                ["placa"] = this.ValidatePlaca,

                ["referenciaLibre"] = this.ValidateReferenciaLibre,
                ["referencia"] = this.ValidateReferencia,

                ["anexo1"] = this.ValidateAnexo,
                ["anexo2"] = this.ValidateAnexo,
                ["anexo3"] = this.ValidateAnexo,
                ["anexo4"] = this.ValidateAnexo,

            };
        }

        public virtual FormValidationGroup ValidateIdEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                },
                OnSuccess = this.ValidateIdEmpresa_OnSuccess,
                OnFailure = this.ValidateIdEmpresa_OnFailure,
            };
        }
        public virtual void ValidateIdEmpresa_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                this.LimpiarCamposDependientesDeEmpresa(field, form, parameters);
            }
        }
        public virtual void ValidateIdEmpresa_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {

            this.LimpiarCamposDependientesDeEmpresa(field, form, parameters);

        }
        public virtual void LimpiarCamposDependientesDeEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var campoAgente = form.GetField("codigoInternoAgente");
            campoAgente.Value = string.Empty;            
            campoAgente.Options.Clear();

            var campo = form.GetField("tipoRecepcionExterno");
            campo.Options.Clear();

            parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = "Fail" });

            form.GetField("autoCargarDetalle").Disabled = true;
            form.GetField("autoCargarDetalle").Value = "false";
        }

        public virtual FormValidationGroup ValidateCodigoInternoAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (string.IsNullOrEmpty(form.GetField("idEmpresa").Value))
            {
                form.GetField("idEmpresa").SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteClienteValidationRule(_uow,field.Value, form.GetField("idEmpresa").Value),

                },
                // Dependencies = { "idEmpresa" },
                OnSuccess = this.ValidateCodigoInternoAgente_OnSuccess,
                OnFailure = this.ValidateCodigoInternoAgente_OnFailure,
            };

        }
        public virtual void ValidateCodigoInternoAgente_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                int idEmpresa = int.Parse(form.GetField("idEmpresa").Value);

                string tipoAgente = _uow.AgenteRepository.GetTipoAgente(field.Value);

                var campo = form.GetField("tipoRecepcionExterno");
                campo.Options = new List<SelectOption>();

                // Tipos de recepción
                List<EmpresaRecepcionTipo> tiposRecepcionExterno = _uow.RecepcionTipoRepository.GetRecepcionTiposEmpresaHabilitados(idEmpresa, tipoAgente);

                foreach (var tipo in tiposRecepcionExterno)
                {
                    campo.Options.Add(new SelectOption(tipo.TipoExterno, $"{tipo.TipoExterno} - {tipo.DescripcionExterna}"));
                }
                campo.Value = string.Empty;
            }
        }
        public virtual void ValidateCodigoInternoAgente_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var campo = form.GetField("tipoRecepcionExterno");
            campo.Options.Clear();

            parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = "Fail" });
        }

        public virtual FormValidationGroup ValidateNumeroPredio(FormField field, Form form, List<ComponentParameter> parameters)
        {

            //if (_predioLogueado != GeneralDb.PredioSinDefinir)
            //    return null;

            var rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario ,field.Value)
                };


            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules,
                OnSuccess = this.ValidatePredio_OnSuccess,
            };

        }
        public virtual void ValidatePredio_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            // Advertencia Orden de carga
            // var campo = form.GetField("tipoRecepcionExterno");

        }

        public virtual FormValidationGroup ValidateTipoRecepcionExterno(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (string.IsNullOrEmpty(form.GetField("idEmpresa").Value))
            {
                form.GetField("idEmpresa").SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            if (string.IsNullOrEmpty(form.GetField("codigoInternoAgente").Value))
            {
                form.GetField("codigoInternoAgente").SetError(new ComponentError("General_Sec0_Error_CampoAgenteReuqerido", new List<string>()));
                return null;
            }

            var tipoAgente = _uow.AgenteRepository.GetTipoAgente(form.GetField("codigoInternoAgente").Value);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new RecepcionTipoExternoExistenteValidationRule(_uow,form.GetField("idEmpresa").Value, tipoAgente),
                },
                OnSuccess = this.ValidatetipoRecepcionExterno_OnSuccess,
                OnFailure = this.ValidatetipoRecepcionExterno_OnFailure,
            };

        }
        public virtual void ValidatetipoRecepcionExterno_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                var recepcionTipo = _uow.RecepcionTipoRepository.GetRecepcionTipoExterno(int.Parse(form.GetField("idEmpresa").Value), field.Value);

                parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia.ToString() });

                form.GetField("autoCargarDetalle").Disabled = false;
                form.GetField("autoCargarDetalle").Value = recepcionTipo.RecepcionTipoInterno.CargaDetalleAutomaticamente.ToString();

                form.GetButton("btnSubmitConfirmarIrDetalle").Disabled = true;
                if (recepcionTipo.RecepcionTipoInterno.PermiteDigitacion)
                    form.GetButton("btnSubmitConfirmarIrDetalle").Disabled = false;

                if (recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion)
                {
                    form.GetField("referencia").Disabled = true;
                    CargarReferenciasParaSeleccionMultiple(field, form, parameters);
                }
                else if (recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion)
                {
                    form.GetField("referencia").Disabled = false;
                }
                else if (recepcionTipo.RecepcionTipoInterno.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Lpn)
                {
                    form.GetField("autoCargarDetalle").Disabled = true;
                    form.GetField("referencia").Disabled = true;
                }
                else
                {
                    form.GetField("referencia").Disabled = true;
                }

            }
        }
        public virtual void ValidatetipoRecepcionExterno_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = "Fail" });
            }

        }
        public virtual void CargarReferenciasParaSeleccionMultiple(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tipoRecepcion = _uow.RecepcionTipoRepository.GetRecepcionTipoByExterno(int.Parse(form.GetField("idEmpresa").Value), field.Value);

            // Cargar referencias disponibles
            var dbQuery = new GetReferenciasDisponiblesParaSeleccionQuery(int.Parse(form.GetField("idEmpresa").Value)
                                                                        , tipoRecepcion.TipoReferencia
                                                                        , form.GetField("codigoInternoAgente").Value
                                                                        , form.GetField("numeroPredio").Value
                                                                        , null);
            _uow.HandleQuery(dbQuery);

            var disponibles = dbQuery.GetReferenciasDisponibles();

            List<CheckboxListItem> listCheck = new List<CheckboxListItem>();
            foreach (var referencia in disponibles)
            {
                listCheck.Add(new CheckboxListItem
                {
                    Id = referencia.Id.ToString(),
                    Label = referencia.ToString(),
                    Selected = false
                });
            }

            parameters.Add(new ComponentParameter("referenciasDisponibles", JsonConvert.SerializeObject(listCheck)));

            // Cargar referencias seleccionadas

            List<CheckboxListItem> listCheckSeleccinado = new List<CheckboxListItem>();

            if (parameters.Any(d => d.Id == "idAgenda"))
            {

                var dbQuerySeleccionadas = new GetReferenciasSeleccionadasAgendaQuery(int.Parse(form.GetField("idEmpresa").Value)
                                                                                    , tipoRecepcion.TipoReferencia
                                                                                    , form.GetField("codigoInternoAgente").Value
                                                                                    , form.GetField("numeroPredio").Value
                                                                                    , int.Parse(form.GetField("idAgenda").Value));

                _uow.HandleQuery(dbQuerySeleccionadas);

                var seleccionadas = dbQuerySeleccionadas.GetReferenciasAsociadas();


                foreach (var referencia in seleccionadas)
                {
                    listCheckSeleccinado.Add(new CheckboxListItem
                    {
                        Id = referencia.Id.ToString(),
                        Label = referencia.ToString(),
                        Selected = false
                    });
                }
            }

            parameters.Add(new ComponentParameter("referenciasAsociadas", JsonConvert.SerializeObject(listCheckSeleccinado)));

        }

        public virtual FormValidationGroup ValidateFechaEntrega(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new DateTimeValidationRule(field.Value),
                        new DateTimeGreaterThanValidationRule(field.Value,DateTimeExtension.ToIsoString( DateTime.Now.Date), "General_Sec0_Error_InvalidDateAnteriorHoy")
                    }
            };
        }

        public virtual FormValidationGroup ValidateFuncionarioAsignado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            if (field.Value == "-1")
                return null;

            if (string.IsNullOrEmpty(form.GetField("idEmpresa").Value))
            {
                form.GetField("idEmpresa").SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            var rules = new List<IValidationRule> {
                new PositiveIntValidationRule(field.Value),
                new ExisteIdUsuarioValidationRule(this._uow, field.Value),
                new UsuarioAsociadoAEmpresaValidationRule(this._uow, field.Value,form.GetField("idEmpresa").Value)
            };

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules,
                OnSuccess = this.ValidatePredio_OnSuccess,
            };
        }

        public virtual FormValidationGroup ValidatePlaca(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 30)
                    }
            };
        }

        public virtual FormValidationGroup ValidateReferenciaLibre(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 20)
                    }
            };
        }

        public virtual FormValidationGroup ValidateReferencia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Disabled) return null;

            if (string.IsNullOrEmpty(form.GetField("idEmpresa").Value))
            {
                form.GetField("idEmpresa").SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            if (string.IsNullOrEmpty(form.GetField("codigoInternoAgente").Value))
            {
                form.GetField("codigoInternoAgente").SetError(new ComponentError("General_Sec0_Error_CampoAgenteReuqerido", new List<string>()));
                return null;
            }

            if (string.IsNullOrEmpty(form.GetField("tipoRecepcionExterno").Value))
            {
                form.GetField("tipoRecepcionExterno").SetError(new ComponentError("General_Sec0_Error_CampoAgenteReuqerido", new List<string>()));
                return null;
            }

            if (string.IsNullOrEmpty(form.GetField("numeroPredio").Value))
            {
                form.GetField("numeroPredio").SetError(new ComponentError("General_Sec0_Error_CampoAgenteReuqerido", new List<string>()));
                return null;
            }


            string tipoAgente = _uow.AgenteRepository.GetTipoAgente(form.GetField("codigoInternoAgente").Value);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new PositiveIntValidationRule(field.Value),
                    new ReferenciaDisponibleRecepcionValidationRule(_uow,form.GetField("idEmpresa").Value
                                                                        ,form.GetField("codigoInternoAgente").Value
                                                                        ,form.GetField("tipoRecepcionExterno").Value
                                                                        ,form.GetField("numeroPredio").Value
                                                                        ,field.Value),

                },

                //OnSuccess = this.ValidatetipoRecepcionExterno_OnSuccess,
                //OnFailure = this.ValidatetipoRecepcionExterno_OnFailure,
            };
        }

        public virtual FormValidationGroup ValidateAnexo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 200)
                }
            };
        }
    }
}
