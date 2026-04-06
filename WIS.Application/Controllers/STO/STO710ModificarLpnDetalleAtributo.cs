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
using WIS.Domain.Parametrizacion;
using WIS.Domain.Tracking;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO710ModificarLpnDetalleAtributo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public STO710ModificarLpnDetalleAtributo(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            { 
                string LpnTipo = query.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;
                string IdAtributo = query.Parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value;
                long NU_LPN = long.Parse(query.Parameters.FirstOrDefault(x => x.Id == "NuLpn").Value);

                string prodructo = query.Parameters.FirstOrDefault(x => x.Id == "CdProduto").Value;
                string identificador = query.Parameters.FirstOrDefault(x => x.Id == "NuIdentificador").Value;
                int empresa = int.Parse(query.Parameters.FirstOrDefault(x => x.Id == "CdEmpresa").Value);
                int id_lpn_Det = int.Parse(query.Parameters.FirstOrDefault(x => x.Id == "IdLpnDet").Value);


                LpnDetalleAtributo lpnTipoAtributo = uow.ManejoLpnRepository.GetLpnDetalleAtributo(id_lpn_Det, NU_LPN, int.Parse(IdAtributo), LpnTipo, prodructo, empresa, identificador);
                Atributo atributo = uow.AtributoRepository.GetAtributo(lpnTipoAtributo.IdAtributo);
                form.GetField("DS_ATRIBUTO").Value = $"{lpnTipoAtributo.IdAtributo.ToString()} - {atributo.Descripcion}";
                string formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);


                query.Parameters.Add(new ComponentParameter { Id = "isDominio", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isSistema", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "F" });
                string separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                switch (atributo.IdTipo)
                {
                    case TipoAtributoDb.NUMERICO:
                        query.Parameters.RemoveAll(w => w.Id == "isNumerico");
                        query.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "T" });
                        if (!string.IsNullOrEmpty(atributo.Separador))
                        {
                            form.GetField("NUMERO").Value = lpnTipoAtributo.ValorAtributo.Replace(separador, atributo.Separador);
                        }
                        else
                        {
                            form.GetField("NUMERO").Value = lpnTipoAtributo.ValorAtributo;
                        }
                        break;

                    case TipoAtributoDb.FECHA:
                        query.Parameters.RemoveAll(w => w.Id == "isFecha");
                        query.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "T" });

                        if (!string.IsNullOrEmpty(lpnTipoAtributo.ValorAtributo))
                        {
                            form.GetField("FECHA").Value = DateTime.ParseExact(lpnTipoAtributo.ValorAtributo, formaterDateTime, _identity.GetFormatProvider()).ToIsoString();
                        }
                        break;
                    case TipoAtributoDb.HORA:
                        query.Parameters.RemoveAll(w => w.Id == "isHora");
                        query.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "T" });

                        if (!string.IsNullOrEmpty(lpnTipoAtributo.ValorAtributo))
                        {
                            form.GetField("HORA").Value = lpnTipoAtributo.ValorAtributo;
                        }
                        break;
                    case TipoAtributoDb.TEXTO:
                        query.Parameters.RemoveAll(w => w.Id == "isTexto");
                        query.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "T" });

                        form.GetField("TEXTO").Value = lpnTipoAtributo.ValorAtributo;
                        break;
                    case TipoAtributoDb.DOMINIO:
                        query.Parameters.RemoveAll(w => w.Id == "isDominio");
                        query.Parameters.Add(new ComponentParameter { Id = "isDominio", Value = "T" });
                        FormField selectDominio = form.GetField("CD_DOMINIO");
                        var ListaDominios = uow.DominioRepository.GetDominios(atributo.CodigoDominio);

                        selectDominio.Options = new List<SelectOption>();
                        foreach (var dominio in ListaDominios)
                        {
                            selectDominio.Options.Add(new SelectOption(dominio.Id.ToString(), $"{dominio.Codigo} - {dominio.Descripcion}"));
                        }
                        selectDominio.Value = lpnTipoAtributo.ValorAtributo;
                        break;
                }

                string descripcionAtributoTipo = uow.AtributoRepository.GetDescripcionAtributoTipo(atributo.IdTipo);
                form.GetField("DS_ATRIBUTO_TIPO").Value = descripcionAtributoTipo;

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

            uow.CreateTransactionNumber("STO710  Modificar Detalle Atributo");
            uow.BeginTransaction();

            try
            {
                string tplpn = "";
                string pIdAtributo = context.Parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value;
                var pLpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo");
                
                if (pLpnTipo != null)
                {
                    tplpn = pLpnTipo.Value;
                }
                
                long nuLpn = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "NuLpn").Value);

                AtributoMapper atributoMapper = new AtributoMapper();
                string lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;
                string idAtributo = context.Parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value;

                string prodructo = context.Parameters.FirstOrDefault(x => x.Id == "CdProduto").Value;
                string identificador = context.Parameters.FirstOrDefault(x => x.Id == "NuIdentificador").Value;
                int empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "CdEmpresa").Value);
                int idLpnDet = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "IdLpnDet").Value);

                LpnDetalleAtributo lpnDetalleAtributo = uow.ManejoLpnRepository.GetLpnDetalleAtributo(idLpnDet, nuLpn, int.Parse(idAtributo), lpnTipo, prodructo, empresa, identificador);

                lpnDetalleAtributo.IdAtributo = int.Parse(pIdAtributo);
                lpnDetalleAtributo.Tipo = tplpn;
                lpnDetalleAtributo.NumeroTransaccion =  uow.GetTransactionNumber();

                string formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
                var culture = _identity.GetFormatProvider();
                var atributo = uow.AtributoRepository.GetAtributo(int.Parse(pIdAtributo));
                string separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                lpnDetalleAtributo.ValorAtributo = AtributoHelper.GetValorByIdTipo(form, atributo, separador, formaterDateTime);
                lpnDetalleAtributo.Estado = EstadoLpnAtributo.Ingresado;

                uow.ManejoLpnRepository.UpdateLpnDetalleAtributo(lpnDetalleAtributo);
                uow.SaveChanges();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

                uow.Commit();
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

            return this._formValidationService.Validate(new LpnDetalleAtributoFormValidationModule(uow, this._identity, this._security, this._identity.GetFormatProvider()), form, context);
        }
    }
}
