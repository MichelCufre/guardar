using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.CheckboxListComponent;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General.Configuracion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class LiberacionOndaFiltroFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;

        public LiberacionOndaFiltroFormValidationModule(IUnitOfWork uow, int idUsuario)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;

            this.Schema = new FormValidationSchema
            {
                ["idEmpresa"] = this.ValidateIdEmpresa,
                ["onda"] = this.ValidateOnda,
                ["predio"] = this.ValidatePredio,
                ["tipoDocumento"] = this.ValidateTipoDocumento,
                ["documento"] = this.ValidateDocumento,
                ["manejaVidaUtil"] = this.ValidateVidaUtil,
                ["excluirUbicPicking"] = this.ValidateExcluirUbicPicking,
                ["usarSoloStkPicking"] = this.ValidateUsarSoloStkPicking,
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
                    new PositiveNumberMaxLengthValidationRule(field.Value, 10),
                    new ExisteEmpresaValidationRule(_uow, field.Value),
                    new EmpresaPoseeOndasValidationRule(this._uow, field.Value),
                },
                OnSuccess = this.OnSuccessIdEmpresa,
                OnFailure = this.OnFailureIdEmpresa
            };
        }

        public virtual void OnSuccessIdEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            bool submit = false;
            if (parameters.Any(s => s.Id == "isSubmit"))
                submit = bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value);

            if (!submit)
            {
                LiberacionConfiguracion liberaConf = this._uow.PreparacionRepository.GetLiberacionConfiguracion(field.Value);

                //UbicacionCompleta
                form.GetField("ubicacionCompleta").ReadOnly = !liberaConf.UbicacionCompletaHabilitado;
                form.GetField("ubicacionCompleta").Value = liberaConf.UbicacionCompleta;

                //UbicacionIncompleta
                form.GetField("ubicacionIncompleta").ReadOnly = !liberaConf.UbicacionIncompletaHabilitado;
                form.GetField("ubicacionIncompleta").Value = liberaConf.UbicacionIncompleta;

                //Prepa Solo camion
                form.GetField("prepSoloCamion").ReadOnly = !liberaConf.PrepSoloCamionHabilitado;
                form.GetField("prepSoloCamion").Value = liberaConf.PrepSoloCamion;

                //Agrup Por camion
                form.GetField("agrupPorCamion").ReadOnly = !liberaConf.AgruparCamionHabilitado;
                form.GetField("agrupPorCamion").Value = liberaConf.AgruparCamion;

                //Maneja vida util
                form.GetField("manejaVidaUtil").ReadOnly = !liberaConf.ManejaVidaUtilHabilitado;
                form.GetField("manejaVidaUtil").Value = liberaConf.ManejaVidaUtil;

                //PriorizarDesborde
                form.GetField("priorizarDesborde").ReadOnly = !liberaConf.PriorizarDesbordeHabilitado;
                form.GetField("priorizarDesborde").Value = liberaConf.PriorizarDesborde;

                //Stock
                form.GetField("stock").ReadOnly = !liberaConf.DefaultStockHabilitado;
                form.GetField("stock").Value = liberaConf.DefaultStock;

                //pedidos
                form.GetField("pedidos").ReadOnly = !liberaConf.PedidosHabilitado;
                form.GetField("pedidos").Value = liberaConf.Pedidos;

                //Repartir escazes
                form.GetField("repartirEscasez").ReadOnly = !liberaConf.RepartirEscazesHabilitado;
                form.GetField("repartirEscasez").Value = liberaConf.RepartirEscazes;

                //liberarPorUnidades
                form.GetField("liberarPorUnidades").ReadOnly = !liberaConf.LiberarPorUnidadHabilitado;
                form.GetField("liberarPorUnidades").Value = liberaConf.LiberarPorUnidad;

                //liberarPorCurvas
                form.GetField("liberarPorCurvas").ReadOnly = !liberaConf.LiberarPorCurvasHabilitado;
                form.GetField("liberarPorCurvas").Value = liberaConf.LiberarPorCurvas;

                //stockDMTI
                form.GetField("stockDtmi").ReadOnly = liberaConf.ManejoDocumental || !liberaConf.ControlStockDMTIHabilitado;
                form.GetField("stockDtmi").Value = liberaConf.ManejoDocumental ? "S" : liberaConf.ControlStockDMTI;

                //RespetaFifo
                form.GetField("respetaFifo").ReadOnly = !liberaConf.RespetaFifoHabilitado;
                form.GetField("respetaFifo").Value = liberaConf.RespetaFifo;

                //Agrupador
                form.GetField("agrupacion").ReadOnly = !liberaConf.ManejoAgrupadorHabilitado;
                form.GetField("agrupacion").Value = liberaConf.ManejoAgrupador;

                form.GetField("onda").ReadOnly = false;
                form.GetField("predio").ReadOnly = false;

                //Requiere ubicacion Picking
                form.GetField("ubicacionPicking2Fases").ReadOnly = !liberaConf.RequiereUbicacionHabilitado;
                form.GetField("ubicacionPicking2Fases").Value = liberaConf.RequiereUbicacion;

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

                //Manejo documental
                List<CheckboxListItem> checkListDoc = new List<CheckboxListItem>();
                List<CheckboxListItem> checkListTipoDoc = new List<CheckboxListItem>();

                if (liberaConf.ManejoDocumental && int.TryParse(field.Value, out int empresa))
                {
                    var tiposDocumentos = this._uow.DocumentoRepository.GetTiposDocumentosLiberables();
                    var documentos = this._uow.DocumentoRepository.GetDocumentosLiberables(empresa);

                    foreach (var t in tiposDocumentos)
                    {
                        checkListTipoDoc.Add(new CheckboxListItem
                        {
                            Id = t.Tipo,
                            Label = t.Descripcion,
                            Selected = false
                        });
                    }

                    foreach (var d in documentos)
                    {
                        checkListDoc.Add(new CheckboxListItem
                        {
                            Id = d.Tipo + "###" + d.Numero,
                            Label = d.Descripcion,
                            Selected = false
                        });
                    }
                }

                parameters.RemoveAll(p => p.Id == "ManejoDocumental");
                parameters.RemoveAll(p => p.Id == "ListItemsTipoDoc");
                parameters.RemoveAll(p => p.Id == "ListItemsDoc");

                parameters.Add(new ComponentParameter("ManejoDocumental", liberaConf.ManejoDocumental.ToString().ToLower()));
                parameters.Add(new ComponentParameter("ListItemsTipoDoc", JsonConvert.SerializeObject(checkListTipoDoc)));
                parameters.Add(new ComponentParameter("ListItemsDoc", JsonConvert.SerializeObject(checkListDoc)));
            }
        }
        public virtual void OnFailureIdEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("onda").ReadOnly = true;
            form.GetField("onda").Value = string.Empty;

            form.GetField("predio").ReadOnly = true;
            form.GetField("predio").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateOnda(FormField field, Form form, List<ComponentParameter> parameters)
        {
            //if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteOndaValidationRule(this._uow, field.Value, form.GetField("idEmpresa").Value)
                }
            };
        }
        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario,field.Value)
                }
            };
        }


        public virtual FormValidationGroup ValidateTipoDocumento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            EstablecerConfirmacion(form, parameters);
            return null;
        }

        public virtual FormValidationGroup ValidateDocumento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            EstablecerConfirmacion(form, parameters);
            return null;
        }

        public virtual void EstablecerConfirmacion(Form form, List<ComponentParameter> parameters)
        {
            bool tpDocSeleccionados = false;
            bool docSeleccionados = false;

            var documentos = new List<CheckboxListItem>();
            var tiposDocumentos = new List<CheckboxListItem>();

            string empresa = form.GetField("idEmpresa").Value;
            var submitButton = form.GetButton("btnSubmitConfirmar");

            var toggleExcluir = form.GetField("excluirUbicPicking");
            toggleExcluir.Disabled = false;
            toggleExcluir.ReadOnly = false;

            var toggleSoloStkPicking = form.GetField("usarSoloStkPicking");
            toggleSoloStkPicking.Disabled = false;
            toggleSoloStkPicking.ReadOnly = false;

            LiberacionConfiguracion config = this._uow.PreparacionRepository.GetLiberacionConfiguracion(empresa);

            if (!string.IsNullOrEmpty(form.GetField("documento").Value))
                documentos = JsonConvert.DeserializeObject<List<CheckboxListItem>>(form.GetField("documento").Value);

            if (!string.IsNullOrEmpty(form.GetField("tipoDocumento").Value))
                tiposDocumentos = JsonConvert.DeserializeObject<List<CheckboxListItem>>(form.GetField("tipoDocumento").Value);

            if (tiposDocumentos != null && tiposDocumentos.Count > 0 && tiposDocumentos.Any(t => t.Selected))
                tpDocSeleccionados = true;

            if (documentos != null && documentos.Count > 0 && documentos.Any(t => t.Selected))
                docSeleccionados = true;

            if (config.ManejoDocumental && (tpDocSeleccionados || docSeleccionados))
            {
                IConfirmMessage confirmarSeleccion = new ConfirmMessage()
                {
                    AcceptLabel = "General_Sec0_btn_Confirmar",
                    CancelLabel = "General_Sec0_btn_Cancelar",
                    Message = "PRE052_Sec0_btn_MsgConfirmar"
                };
                submitButton.ConfirmMessage = confirmarSeleccion;

                toggleExcluir.Value = "true";
                toggleExcluir.Disabled = true;
                toggleExcluir.ReadOnly = true;

                toggleSoloStkPicking.Value = "false";
                toggleSoloStkPicking.Disabled = true;
                toggleSoloStkPicking.ReadOnly = true;
            }
            else
            {
                submitButton.ConfirmMessage = null;

                bool submit = false;
                if (parameters.Any(s => s.Id == "isSubmit"))
                    submit = bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value);

                if (!submit)
                    toggleExcluir.Value = "false";
            }
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
