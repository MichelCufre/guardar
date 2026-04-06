using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General.Configuracion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PRE250LiberacionValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;
        protected readonly IFormatProvider _formatProvider;

        public PRE250LiberacionValidationModule(IUnitOfWork uow, int idUsuario, IFormatProvider provider)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;
            this._formatProvider = provider;

            this.Schema = new FormValidationSchema
            {
                ["empresa"] = this.ValidateEmpresa,
                ["onda"] = this.ValidateOnda,
                ["predio"] = this.ValidatePredio,
                ["clientesPorPrep"] = this.ValidateClientesPorPreparacion,
                ["tpAgente"] = this.ValidatetpAgente,
                ["manejaVidaUtil"] = this.ValidateVidaUtil,
                ["excluirUbicPicking"] = this.ValidateExcluirUbicPicking,
                ["usarSoloStkPicking"] = this.ValidateUsarSoloStkPicking,
            };

        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                },
                OnSuccess = this.OnSuccessEmpresa,
                OnFailure = this.OnFailureEmpresa
            };
        }

        public virtual void OnSuccessEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            ////Inicializar selects

            var liberaConf = _uow.PreparacionRepository.GetLiberacionConfiguracion(field.Value);

            //UbicacionCompleta
            FormField selectUbicacionCompleta = form.GetField("ubicacionCompleta");
            selectUbicacionCompleta.ReadOnly = !liberaConf.UbicacionCompletaHabilitado;
            selectUbicacionCompleta.Value = liberaConf.UbicacionCompleta;

            //UbicacionIncompleta
            FormField selectUbicacionIncompleta = form.GetField("ubicacionIncompleta");
            selectUbicacionIncompleta.ReadOnly = !liberaConf.UbicacionIncompletaHabilitado;
            selectUbicacionIncompleta.Value = liberaConf.UbicacionIncompleta;

            //Prepa Solo camion
            FormField selectPrepSoloCam = form.GetField("prepSoloCamion");
            selectPrepSoloCam.ReadOnly = !liberaConf.PrepSoloCamionHabilitado;
            selectPrepSoloCam.Value = liberaConf.PrepSoloCamion;

            //Agrup Por camion
            FormField selectAgrupPorCamion = form.GetField("agrupPorCamion");
            selectAgrupPorCamion.ReadOnly = !liberaConf.AgruparCamionHabilitado;
            selectAgrupPorCamion.Value = liberaConf.AgruparCamion;

            //liberarPorUnidades
            FormField liberarPorUnidades = form.GetField("liberarPorUnidades");
            liberarPorUnidades.ReadOnly = !liberaConf.LiberarPorUnidadHabilitado;
            liberarPorUnidades.Value = liberaConf.LiberarPorUnidad;

            //liberarPorCurvas
            FormField liberarPorCurvas = form.GetField("liberarPorCurvas");
            liberarPorCurvas.ReadOnly = !liberaConf.LiberarPorCurvasHabilitado;
            liberarPorCurvas.Value = liberaConf.LiberarPorCurvas;

            //Maneja vida util
            FormField manejaVidaUtil = form.GetField("manejaVidaUtil");
            manejaVidaUtil.ReadOnly = !liberaConf.ManejaVidaUtilHabilitado;
            manejaVidaUtil.Value = liberaConf.ManejaVidaUtil;

            //Agrupador
            FormField agrupacion = form.GetField("agrupacion");
            agrupacion.ReadOnly = !liberaConf.ManejoAgrupadorHabilitado;
            agrupacion.Value = liberaConf.ManejoAgrupador;

            //Stock
            FormField stock = form.GetField("stock");
            stock.ReadOnly = !liberaConf.DefaultStockHabilitado;
            stock.Value = liberaConf.DefaultStock;

            //pedidos
            FormField pedidos = form.GetField("pedidos");
            pedidos.ReadOnly = !liberaConf.PedidosHabilitado;
            pedidos.Value = liberaConf.Pedidos;

            //Repartir escazes
            FormField repartirEscasez = form.GetField("repartirEscasez");
            repartirEscasez.ReadOnly = !liberaConf.RepartirEscazesHabilitado;
            repartirEscasez.Value = liberaConf.RepartirEscazes;

            //RespetaFifo
            FormField respetaFifo = form.GetField("respetaFifo");
            respetaFifo.ReadOnly = !liberaConf.RespetaFifoHabilitado;
            respetaFifo.Value = liberaConf.RespetaFifo;

            //PriorizarDesborde
            FormField priorizarDesborde = form.GetField("priorizarDesborde");
            priorizarDesborde.ReadOnly = !liberaConf.PriorizarDesbordeHabilitado;
            priorizarDesborde.Value = liberaConf.PriorizarDesborde;

            //stockDMTI
            FormField stockDtmi = form.GetField("stockDtmi");
            stockDtmi.ReadOnly = liberaConf.ManejoDocumental || !liberaConf.ControlStockDMTIHabilitado;
            stockDtmi.Value = liberaConf.ManejoDocumental ? "S" : liberaConf.ControlStockDMTI;

            //Excluir ubicaciones de picking
            if (liberaConf.ManejaVidaUtil == "S")
            {
                form.GetField("excluirUbicPicking").Value = "true";
                form.GetField("excluirUbicPicking").ReadOnly = true;
                form.GetField("excluirUbicPicking").Disabled = true;

                form.GetField("usarSoloStkPicking").Value = "false";
                form.GetField("usarSoloStkPicking").ReadOnly = true;
                form.GetField("usarSoloStkPicking").Disabled = true;
            }
            else
            {
                form.GetField("excluirUbicPicking").Value = liberaConf.ExcluirUbicacionesPicking == "S" ? "true" : "false";
                form.GetField("excluirUbicPicking").ReadOnly = !liberaConf.ExcluirUbicacionesPickingHabilitado;
                form.GetField("excluirUbicPicking").Disabled = !liberaConf.ExcluirUbicacionesPickingHabilitado;

                form.GetField("usarSoloStkPicking").Value = liberaConf.UsarSoloStkPicking == "S" && liberaConf.ExcluirUbicacionesPicking != "S" ? "true" : "false";
                form.GetField("usarSoloStkPicking").ReadOnly = !liberaConf.UsarSoloStkPickingHabilitado;
                form.GetField("usarSoloStkPicking").Disabled = !liberaConf.UsarSoloStkPickingHabilitado;
            }
        }
        public virtual void OnFailureEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
        }

        public virtual FormValidationGroup ValidateOnda(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new IdOndaExistenteValidationRule(_uow,field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();
            if (!string.IsNullOrEmpty(field.Value))
                rules.Add(new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario, field.Value));

            //if (string.IsNullOrEmpty(field.Value))
            //return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules,
                OnSuccess = this.ValidatePredio_OnSuccess,
                OnFailure = this.ValidatePredio_OnFailure,
            };
        }

        public virtual void ValidatePredio_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                var fieldOnda = form.GetField("onda");
                fieldOnda.Options = new List<SelectOption>();

                foreach (var onda in _uow.OndaRepository.GetOndasActivas(field.Value).OrderBy(s => s.Id))
                {
                    fieldOnda.Options.Add(new SelectOption(onda.Id.ToString(), $"{onda.Id} - {onda.Descripcion}"));
                }
            }
        }
        public virtual void ValidatePredio_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var campo = form.GetField("onda");
            campo.Options.Clear();
        }


        public virtual FormValidationGroup ValidateClientesPorPreparacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value, 5),
                        new PositiveShortValidationRule(field.Value),
                    }
            };
        }

        public virtual FormValidationGroup ValidatetpAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var nuRegla = parameters.FirstOrDefault(s => s.Id == "nuRegla")?.Value;

            if (string.IsNullOrEmpty(nuRegla))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new ReglaTipoAgenteCompatibilidadValidationRule(_uow, int.Parse(nuRegla), field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateVidaUtil(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new SorNValidationRule(field.Value)
                },
                OnSuccess = this.OnSuccessVidaUtil
            };
        }

        public virtual void OnSuccessVidaUtil(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Value == "S")
            {
                form.GetField("excluirUbicPicking").Value = "true";
                form.GetField("excluirUbicPicking").ReadOnly = true;
                form.GetField("excluirUbicPicking").Disabled = true;

                form.GetField("usarSoloStkPicking").Value = "false";
                form.GetField("usarSoloStkPicking").ReadOnly = true;
                form.GetField("usarSoloStkPicking").Disabled = true;
            }
            else
            {
                form.GetField("excluirUbicPicking").Value = "false";
                form.GetField("excluirUbicPicking").ReadOnly = false;
                form.GetField("excluirUbicPicking").Disabled = false;
            }
        }

        public virtual FormValidationGroup ValidateExcluirUbicPicking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>(),
                OnSuccess = this.OnSuccessExcluirUbicPicking
            };
        }

        public virtual void OnSuccessExcluirUbicPicking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Value == "true")
                form.GetField("usarSoloStkPicking").Value = "false";
        }

        public virtual FormValidationGroup ValidateUsarSoloStkPicking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>(),
                OnSuccess = this.OnSuccessUsarSoloStkPicking
            };
        }

        public virtual void OnSuccessUsarSoloStkPicking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Value == "true")
                form.GetField("excluirUbicPicking").Value = "false";
        }
    }
}
