using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Helpers;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PAR
{
    public class PAR401ModificarLpnTipoAtributoDet : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public PAR401ModificarLpnTipoAtributoDet(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;
                var idAtributo = context.Parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value;

                var lpnTipoAtributo = uow.ManejoLpnRepository.GetLpnAtributoTipoDet(int.Parse(idAtributo), lpnTipo);
                var atributo = uow.AtributoRepository.GetAtributo(lpnTipoAtributo.IdAtributo);

                form.GetField("ID_ATRIBUTO").Value = lpnTipoAtributo.IdAtributo.ToString();
                form.GetField("DS_ATRIBUTO").Value = $"{lpnTipoAtributo.IdAtributo} - {atributo.Descripcion}";

                var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);

                var lpnEnUso = uow.ManejoLpnRepository.AnyTipoLpnEnUso(lpnTipoAtributo.TipoLpn);
                if (lpnEnUso)
                {
                    form.GetField("FL_REQUERIDO").Disabled = true;
                    form.GetField("ID_ESTADO_INICIAL").Disabled = true;
                    form.GetField("VL_VALIDO_INTERFAZ").Disabled = true;
                }

                var selectEstado = form.GetField("ID_ESTADO_INICIAL");
                selectEstado.Options = new List<SelectOption>();

                var estados = uow.AtributoRepository.GetEstados();
                foreach (var estado in estados)
                {
                    selectEstado.Options.Add(new SelectOption(estado.Id, $"{estado.Id} - {estado.Descripcion}"));
                }
                selectEstado.Value = lpnTipoAtributo.EstadoInicial;

                context.Parameters.Add(new ComponentParameter { Id = "isDominio", Value = "F" });
                context.Parameters.Add(new ComponentParameter { Id = "isSistema", Value = "F" });
                context.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "F" });
                context.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "F" });
                context.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "F" });
                context.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "F" });
                
                var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                switch (atributo.IdTipo)
                {
                    case TipoAtributoDb.NUMERICO:

                        context.Parameters.RemoveAll(w => w.Id == "isNumerico");
                        context.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "T" });
                        
                        if (!string.IsNullOrEmpty(atributo.Separador))
                        {
                            if (!string.IsNullOrEmpty(lpnTipoAtributo.ValorInicial))
                                form.GetField("NUMERO").Value = lpnTipoAtributo.ValorInicial.Replace(separador, atributo.Separador);
                        }
                        else
                            form.GetField("NUMERO").Value = lpnTipoAtributo.ValorInicial;

                        break;
                    case TipoAtributoDb.FECHA:

                        context.Parameters.RemoveAll(w => w.Id == "isFecha");
                        context.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "T" });

                        if (!string.IsNullOrEmpty(lpnTipoAtributo.ValorInicial))
                            form.GetField("FECHA").Value = DateTime.ParseExact(lpnTipoAtributo.ValorInicial, formaterDateTime, _identity.GetFormatProvider()).ToIsoString();

                        break;
                    case TipoAtributoDb.HORA:

                        context.Parameters.RemoveAll(w => w.Id == "isHora");
                        context.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "T" });

                        if (!string.IsNullOrEmpty(lpnTipoAtributo.ValorInicial))
                            form.GetField("HORA").Value = lpnTipoAtributo.ValorInicial;

                        break;
                    case TipoAtributoDb.TEXTO:

                        context.Parameters.RemoveAll(w => w.Id == "isTexto");
                        context.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "T" });

                        form.GetField("TEXTO").Value = lpnTipoAtributo.ValorInicial;

                        break;
                    case TipoAtributoDb.DOMINIO:

                        context.Parameters.RemoveAll(w => w.Id == "isDominio");
                        context.Parameters.Add(new ComponentParameter { Id = "isDominio", Value = "T" });

                        var selectDominio = form.GetField("CD_DOMINIO");
                        selectDominio.Options = new List<SelectOption>();

                        var detallesDominio = uow.DominioRepository.GetDominios(atributo.CodigoDominio);
                        foreach (var detalle in detallesDominio)
                        {
                            selectDominio.Options.Add(new SelectOption(detalle.Id.ToString(), $"{detalle.Id} - {detalle.Descripcion}"));
                        }

                        selectDominio.Value = lpnTipoAtributo.ValorInicial;

                        break;
                    case TipoAtributoDb.SISTEMA:

                        context.Parameters.RemoveAll(w => w.Id == "isSistema");
                        context.Parameters.Add(new ComponentParameter { Id = "isSistema", Value = "T" });

                        form.GetField("FL_REQUERIDO").Disabled = true;
                        form.GetField("ID_ESTADO_INICIAL").Disabled = true;
                        form.GetField("VL_VALIDO_INTERFAZ").Disabled = true;

                        break;
                }

                form.GetField("FL_REQUERIDO").Value = (lpnTipoAtributo.Requerido == "S").ToString();
                form.GetField("VL_VALIDO_INTERFAZ").Value = (lpnTipoAtributo.ValidoInterfaz == "S").ToString();
                form.GetField("DS_ATRIBUTO_TIPO").Value = uow.AtributoRepository.GetDescripcionAtributoTipo(atributo.IdTipo);
            }
            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var tplpn = string.Empty;
                var Id_Atributo = form.GetField("ID_ATRIBUTO").Value;
                var parametro = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo");

                if (parametro != null)
                    tplpn = parametro.Value;

                var lpnTipoAtributoDet = uow.ManejoLpnRepository.GetLpnAtributoTipoDet(int.Parse(Id_Atributo), tplpn);
                
                if (lpnTipoAtributoDet != null)
                {
                    lpnTipoAtributoDet.IdAtributo = int.Parse(Id_Atributo);
                    lpnTipoAtributoDet.TipoLpn = tplpn;
                    lpnTipoAtributoDet.Requerido = Mapper.MapStringToBooleanString(form.GetField("FL_REQUERIDO").Value);
                    lpnTipoAtributoDet.ValidoInterfaz = Mapper.MapStringToBooleanString(form.GetField("VL_VALIDO_INTERFAZ").Value);
                    lpnTipoAtributoDet.EstadoInicial = form.GetField("ID_ESTADO_INICIAL").Value;

                    var tipoAtributo = uow.AtributoRepository.GetIdAtributoTipo(int.Parse(Id_Atributo));
                    var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
                    var culture = _identity.GetFormatProvider();
                    var atributo = uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));
                    var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                    lpnTipoAtributoDet.ValorInicial = AtributoHelper.GetValorByIdTipo(form, atributo, separador, formaterDateTime);

                    uow.ManejoLpnRepository.UpdateLpnTipoAtributoDetalle(lpnTipoAtributoDet);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new CreateAtributoTipoLpnDetFormValidationModule(uow, this._identity, this._security, this._identity.GetFormatProvider()), form, context);
        }
    }
}
