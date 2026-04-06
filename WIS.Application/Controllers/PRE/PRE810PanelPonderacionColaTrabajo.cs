using crypto;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE810PanelPonderacionColaTrabajo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;
        protected readonly IFormValidationService _formValidationService;
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE810PanelPonderacionColaTrabajo(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService)
        {
            this._formValidationService = formValidationService;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
        }

        #region FORM
        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string strNroColaDeTrabajo = query.GetParameter("COLADETRABAJO");

                this.InicializarSelectColaDeTrabajo(uow, form);

                if (string.IsNullOrEmpty(strNroColaDeTrabajo) && string.IsNullOrEmpty(form.GetField("colaDeTrabajo").Value))
                {
                    query.AddParameter("flagEmpresa", "N");
                    query.AddParameter("incrementoEmpresa", "");
                    query.AddParameter("flagCliente", "N");
                    query.AddParameter("incrementoCliente", "");
                }
                else
                {
                    FormField fieldTipoMotivo = new FormField();
                    if (string.IsNullOrEmpty(strNroColaDeTrabajo))
                        fieldTipoMotivo = form.GetField("colaDeTrabajo");
                    else if (string.IsNullOrEmpty(fieldTipoMotivo.Value))
                        fieldTipoMotivo.Value = strNroColaDeTrabajo;

                    int.TryParse(strNroColaDeTrabajo, out int nroColaDeTrabajo);
                    List<ColaDeTrabajoPonderador> colPonderadores = uow.ColaDeTrabajoRepository.GetPonderadoresByNumero(nroColaDeTrabajo);
                    var colaTrabajo = uow.ColaDeTrabajoRepository.GetColaDeTrabajo(nroColaDeTrabajo);
                    form.GetField("flOrdenCalendario").Value = colaTrabajo?.flOrdenCalendario == "S" ? "true" : "false";
                    ColaDeTrabajoPonderador curr = null;
                    PonderadorInstancia instanciaDefault = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "CD_EMPRESA");
                    if (curr != null)
                    {
                        form.GetField("flagEmpresa").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoEmpresa").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("CD_EMPRESA");

                        form.GetField("flagEmpresa").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoEmpresa").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "CD_CLIENTE");
                    if (curr != null)
                    {
                        form.GetField("flagCliente").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoCliente").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("CD_CLIENTE");
                        form.GetField("flagCliente").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoCliente").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "CD_ROTA");
                    if (curr != null)
                    {
                        form.GetField("flagRuta").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoRuta").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("CD_ROTA");
                        form.GetField("flagRuta").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoRuta").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "CD_ZONA");
                    if (curr != null)
                    {
                        form.GetField("flagZona").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoZona").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("CD_ZONA");
                        form.GetField("flagZona").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoZona").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "TP_PEDIDO");
                    if (curr != null)
                    {
                        form.GetField("flagTipoPedido").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoTipoPedido").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("TP_PEDIDO");
                        form.GetField("flagTipoPedido").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoTipoPedido").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "TP_EXPEDICION");
                    if (curr != null)
                    {
                        form.GetField("flagTipoExpedicion").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoTipoExpedicion").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("TP_EXPEDICION");
                        form.GetField("flagTipoExpedicion").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoTipoExpedicion").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "CD_CONDICION_LIBERACION");
                    if (curr != null)
                    {
                        form.GetField("flagConficionLiberacion").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoCondicionLiberacion").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("CD_CONDICION_LIBERACION");
                        form.GetField("flagConficionLiberacion").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoCondicionLiberacion").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "DT_ENTREGA");
                    if (curr != null)
                    {
                        form.GetField("flagFechaEntrega").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoFechaEntrega").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("DT_ENTREGA");
                        form.GetField("flagFechaEntrega").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoFechaEntrega").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "DT_EMITIDO");
                    if (curr != null)
                    {
                        form.GetField("flagFechaEmitido").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoFechaEmitido").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("DT_EMITIDO");
                        form.GetField("flagFechaEmitido").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoFechaEmitido").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "DT_LIBERADO");
                    if (curr != null)
                    {
                        form.GetField("flagFechaLiberado").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoFechaLiberado").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("DT_LIBERADO");
                        form.GetField("flagFechaLiberado").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoFechaLiberado").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                    curr = null;

                    curr = colPonderadores.FirstOrDefault(x => x.Ponderador == "VL_PONDERACION_GENERICA");
                    if (curr != null)
                    {
                        form.GetField("flagPonderacionGenerica").Value = curr.Habilitado ? "S" : "N";
                        form.GetField("incrementoPonderacionGenerica").Value = (curr.Incremento ?? 0).ToString();
                    }
                    else
                    {
                        instanciaDefault = uow.ColaDeTrabajoRepository.GetPonderadorDefault("VL_PONDERACION_GENERICA");
                        form.GetField("flagPonderacionGenerica").Value = instanciaDefault.Habilitado ? "S" : "N";
                        form.GetField("incrementoPonderacionGenerica").Value = (instanciaDefault.IncrementoDefault ?? 0).ToString();
                    }
                }


            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {


                string ponderador = form.GetField("colaDeTrabajo").Value;

                if (string.IsNullOrEmpty(ponderador) || !int.TryParse(ponderador, out int nuPonderador))
                    throw new ValidationFailedException("");

                List<ColaDeTrabajoPonderador> col = new List<ColaDeTrabajoPonderador>();

                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagEmpresa").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoEmpresa").Value),
                    Ponderador = "CD_EMPRESA",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagCliente").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoCliente").Value),
                    Ponderador = "CD_CLIENTE",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagRuta").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoRuta").Value),
                    Ponderador = "CD_ROTA",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagZona").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoZona").Value),
                    Ponderador = "CD_ZONA",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagTipoPedido").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoTipoPedido").Value),
                    Ponderador = "TP_PEDIDO",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagTipoExpedicion").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoTipoExpedicion").Value),
                    Ponderador = "TP_EXPEDICION",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagConficionLiberacion").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoCondicionLiberacion").Value),
                    Ponderador = "CD_CONDICION_LIBERACION",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagFechaEntrega").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoFechaEntrega").Value),
                    Ponderador = "DT_ENTREGA",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagFechaEmitido").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoFechaEmitido").Value),
                    Ponderador = "DT_EMITIDO",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagFechaLiberado").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoFechaLiberado").Value),
                    Ponderador = "DT_LIBERADO",
                    Numero = nuPonderador
                });
                col.Add(new ColaDeTrabajoPonderador()
                {
                    Habilitado = form.GetField("flagPonderacionGenerica").Value == "true" ? true : false,
                    Incremento = int.Parse(form.GetField("incrementoPonderacionGenerica").Value),
                    Ponderador = "VL_PONDERACION_GENERICA",
                    Numero = nuPonderador
                });


                ColaDeTrabajoPonderador curr;

                foreach (var item in col)
                {
                    curr = uow.ColaDeTrabajoRepository.GetPonderador(item.Numero, item.Ponderador);

                    if (curr != null)
                    {
                        if (curr.Habilitado != item.Habilitado || curr.Incremento != item.Incremento)
                        {
                            curr.Habilitado = item.Habilitado;
                            curr.Incremento = item.Incremento;
                            uow.ColaDeTrabajoRepository.UpdatePonderador(curr);
                        }
                    }
                    else
                        uow.ColaDeTrabajoRepository.AddPonderador(item);

                    curr = null;
                }
                var colaTrabajo = uow.ColaDeTrabajoRepository.GetColaDeTrabajo(nuPonderador);
                colaTrabajo.flOrdenCalendario = form.GetField("flOrdenCalendario").Value == "true" ? "S" : "N";
                uow.ColaDeTrabajoRepository.UpdateColaDeTrabajo(colaTrabajo);

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
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

            return this._formValidationService.Validate(new PRE810PonderadoresFormValidationModule(uow), form, context);
        }

        #endregion

        #region MetodosAxiliares

        public virtual void InicializarSelectColaDeTrabajo(IUnitOfWork uow, Form form)
        {
            FormField fieldTipoMotivo = form.GetField("colaDeTrabajo");
            fieldTipoMotivo.Options = new List<SelectOption>();

            List<ColaDeTrabajo> col = new List<ColaDeTrabajo>();

            if (_identity.Predio == "S/D")
                col = uow.ColaDeTrabajoRepository.GetColasDeTrabajo();
            else
                col = uow.ColaDeTrabajoRepository.GetColasDeTrabajo(_identity.Predio);

            foreach (var colaDeTrabajo in col)
            {
                fieldTipoMotivo.Options.Add(new SelectOption(colaDeTrabajo.Numero.ToString(), $"Nº {colaDeTrabajo.Numero} - Predio {colaDeTrabajo.Predio} - {colaDeTrabajo.Descripcion}"));
            }

            if (col.Count == 1)
                fieldTipoMotivo.Value = col.First().Numero.ToString();
        }

        #endregion
    }
}
