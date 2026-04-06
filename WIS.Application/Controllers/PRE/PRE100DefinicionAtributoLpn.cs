using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.Extension;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent;
using WIS.Security;
using WIS.Domain.Picking.Dtos;
using WIS.Components.Common.Select;
using WIS.Domain.Picking;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Win32;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100DefinicionAtributoLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public PRE100DefinicionAtributoLpn(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var listaAtributos = JsonConvert.DeserializeObject<List<KeyAtributoTipo>>(context.Parameters.FirstOrDefault(x => x.Id == "listAtributos").Value);
                var atributoDefinicion = listaAtributos.OrderBy(x => x.IdAtributo).FirstOrDefault();

                var atributo = uow.AtributoRepository.GetAtributo(atributoDefinicion.IdAtributo);
                var atributoTipo = uow.AtributoRepository.GetTipoAtributoById(atributo.IdTipo);

                foreach (var field in form.Fields)
                    field.Value = string.Empty;
                
                context.Parameters.RemoveAll(w => w.Id == "idAtributo");
                context.Parameters.RemoveAll(w => w.Id == "nombreAtributo");

                context.AddParameter("idAtributo", atributo.Id.ToString());
                context.AddParameter("nombreAtributo", atributo.Nombre);

                context.Parameters.Add(new ComponentParameter { Id = "isField", Value = "N" });
                context.Parameters.Add(new ComponentParameter { Id = "isFieldTime", Value = "N" });
                context.Parameters.Add(new ComponentParameter { Id = "isFieldSelect", Value = "N" });
                context.Parameters.Add(new ComponentParameter { Id = "isFieldDateTime", Value = "N" });

                context.Parameters.Add(new ComponentParameter { Id = "labelInput", Value = atributoTipo.Descripcion });

                switch (atributo.IdTipo)
                {
                    case TipoAtributoDb.NUMERICO:
                        context.Parameters.RemoveAll(w => w.Id == "isField");
                        context.Parameters.Add(new ComponentParameter { Id = "isField", Value = "S" });
                        break;
                    case TipoAtributoDb.FECHA:
                        context.Parameters.RemoveAll(w => w.Id == "isFieldDateTime");
                        context.Parameters.Add(new ComponentParameter { Id = "isFieldDateTime", Value = "S" });
                        break;
                    case TipoAtributoDb.HORA:
                        context.Parameters.RemoveAll(w => w.Id == "isFieldTime");
                        context.Parameters.Add(new ComponentParameter { Id = "isFieldTime", Value = "S" });
                        break;
                    case TipoAtributoDb.TEXTO:
                        context.Parameters.RemoveAll(w => w.Id == "isField");
                        context.Parameters.Add(new ComponentParameter { Id = "isField", Value = "S" });
                        break;
                    case TipoAtributoDb.DOMINIO:

                        var options = new List<SelectOption>();
                        var dominios = uow.DominioRepository.GetDominios(atributo.CodigoDominio);

                        foreach (var dominio in dominios)
                        {
                            options.Add(new SelectOption(dominio.Id.ToString(), $"{dominio.Id} - {dominio.Descripcion}"));
                        }

                        form.GetField("inputFieldSelect").Options = options;

                        context.Parameters.RemoveAll(w => w.Id == "isFieldSelect");
                        context.Parameters.Add(new ComponentParameter { Id = "isFieldSelect", Value = "S" });
                        break;

                }
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
            uow.CreateTransactionNumber("PRE100DefinicionAtributoLpn - FormSubmit");

            try
            {
                var listaAtributos = JsonConvert.DeserializeObject<List<KeyAtributoTipo>>(context.Parameters.FirstOrDefault(x => x.Id == "listAtributos").Value);

                if (listaAtributos.Count > 0)
                {                    
                    var datos = GetDatos(context.Parameters);
                    var atributoDefinicion = listaAtributos.OrderBy(x => x.IdAtributo).FirstOrDefault();

                    var newAtributoTemporal = CrearDefinicionAtributo(uow, form, datos, atributoDefinicion);

                    uow.ManejoLpnRepository.AddAtributoLpnTemporal(newAtributoTemporal);

                    listaAtributos.Remove(atributoDefinicion);

                    context.Parameters.RemoveAll(w => w.Id == "listAtributos");

                    if (listaAtributos.Count > 0)
                        context.Parameters.Add(new ComponentParameter() { Id = "listAtributos", Value = JsonConvert.SerializeObject(listaAtributos) });
                    else
                        context.Parameters.Add(new ComponentParameter() { Id = "listAtributos", Value = string.Empty });
                }
                else
                    context.Parameters.Add(new ComponentParameter() { Id = "listAtributos", Value = string.Empty });

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new PedidoDefinicionAtributosFormValidationModule(uow, this._identity), form, context);
        }


        #region Auxs

        public virtual DetallePedidoAtributoLpnTemporal CrearDefinicionAtributo(IUnitOfWork uow, Form form, DetallePedidoLpnEspecifico datos, KeyAtributoTipo atributoDefinicion)
        {
            var newAtributoTemporal = new DetallePedidoAtributoLpnTemporal()
            {
                Pedido = datos.Pedido,
                Cliente = datos.Cliente,
                Empresa = datos.Empresa,
                Producto = datos.Producto,
                Faixa = datos.Faixa,
                Identificador = datos.Identificador,
                IdEspecificaIdentificador = datos.IdEspecificaIdentificador,
                TipoLpn = datos.TipoLpn,
                IdExternoLpn = datos.IdExternoLpn,
                IdAtributo = atributoDefinicion.IdAtributo,
                UserId = _identity.UserId,
                IdCabezal = atributoDefinicion.Cabezal,
                FechaAlta = DateTime.Now,
                Transaccion = uow.GetTransactionNumber()
            };

            var parametros = uow.ParametroRepository.GetParameters(new List<string> {
                ParamManager.NUMBER_DECIMAL_SEPARATOR,
                ParamManager.DATETIME_FORMAT_DATE_SECONDS
            });

            var separador = parametros[ParamManager.NUMBER_DECIMAL_SEPARATOR];
            var formaterDateTime = parametros[ParamManager.DATETIME_FORMAT_DATE_SECONDS];

            var atributo = uow.AtributoRepository.GetAtributo(atributoDefinicion.IdAtributo);

            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.NUMERICO:
                    if (!string.IsNullOrEmpty(atributo.Separador))
                        newAtributoTemporal.Valor = form.GetField("inputField").Value.Replace(atributo.Separador, separador);
                    else
                        newAtributoTemporal.Valor = form.GetField("inputField").Value;
                    break;
                case TipoAtributoDb.FECHA:
                    newAtributoTemporal.Valor = DateTimeExtension.ParseFromIso(form.GetField("inputFieldDateTime").Value)?.Date.ToString(formaterDateTime);
                    break;
                case TipoAtributoDb.HORA:
                    newAtributoTemporal.Valor = form.GetField("inputFieldTime").Value;
                    break;
                case TipoAtributoDb.TEXTO:
                    newAtributoTemporal.Valor = form.GetField("inputField").Value;
                    break;
                case TipoAtributoDb.DOMINIO:
                    newAtributoTemporal.Valor = form.GetField("inputFieldSelect").Value;
                    break;
            }

            return newAtributoTemporal;
        }

        public virtual DetallePedidoLpnEspecifico GetDatos(List<ComponentParameter> parametros, bool gridDetalle = false)
        {
            var dto = new DetallePedidoLpnEspecifico()
            {
                Pedido = parametros.FirstOrDefault(p => p.Id == "pedido").Value,
                Cliente = parametros.FirstOrDefault(p => p.Id == "cliente").Value,
                Empresa = int.Parse(parametros.FirstOrDefault(p => p.Id == "empresa").Value),
                Producto = parametros.FirstOrDefault(p => p.Id == "producto").Value,
                Faixa = decimal.Parse(parametros.FirstOrDefault(p => p.Id == "faixa").Value, _identity.GetFormatProvider()),
                Identificador = parametros.FirstOrDefault(p => p.Id == "identificador").Value,
                IdEspecificaIdentificador = parametros.FirstOrDefault(p => p.Id == "idEspecificaIdentificador").Value,
                TipoLpn = parametros.FirstOrDefault(p => p.Id == "tipoLpn").Value,
                IdExternoLpn = parametros.FirstOrDefault(p => p.Id == "idExternoLpn").Value,
                Update = (parametros.FirstOrDefault(p => p.Id == "update")?.Value ?? "N") == "S",
                UserId = _identity.UserId,
                Detalle = gridDetalle
            };

            var idConfiguracion = parametros.FirstOrDefault(p => p.Id == "idConfiguracion")?.Value;
            if (!string.IsNullOrEmpty(idConfiguracion))
                dto.IdConfiguracion = long.Parse(idConfiguracion);

            var cantidad = parametros.FirstOrDefault(p => p.Id == "cantidad")?.Value;
            if (!string.IsNullOrEmpty(cantidad))
                dto.Cantidad = decimal.Parse(cantidad, _identity.GetFormatProvider());

            return dto;
        }

        #endregion
    }
}
