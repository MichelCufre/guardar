using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Porteria;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;
using WIS.Extension;
using Newtonsoft.Json;

namespace WIS.Application.Validation.Modules.FormModules.Porteria
{
    public class DatosPorteriaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public DatosPorteriaFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["VL_MATRICULA_1"] = this.ValidateVL_MATRICULA_1,
                ["VL_DOCUMENTO"] = this.ValidateVL_DOCUMENTO,
            };
        }

        public virtual FormValidationGroup ValidateVL_DOCUMENTO(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 10),
                },
                OnSuccess = this.CargarDatos_OnSuccess,
            };
        }
        public virtual FormValidationGroup ValidateVL_MATRICULA_1(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 10)
                },
                OnSuccess = this.CargarDatos_OnSuccess
            };
        }

        public virtual void CargarDatos_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            FormField FieldNU_PORTERIA_VEHICULO = form.GetField("NU_PORTERIA_VEHICULO");
            FormField FieldVL_MATRICULA_2 = form.GetField("VL_MATRICULA_2");
            FormField FieldVL_PESO_ENTRADA = form.GetField("VL_PESO_ENTRADA");
            FormField FieldDS_SECTOR = form.GetField("DS_SECTOR");
            FormField FieldDS_POTERIA_MOTIVO = form.GetField("DS_POTERIA_MOTIVO");
            FormField FieldNM_EMPRESA = form.GetField("NM_EMPRESA");
            FormField FieldNM_AGENTE = form.GetField("NM_AGENTE");
            FormField FieldDT_PORTERIA_ENTRADA = form.GetField("DT_PORTERIA_ENTRADA");

            try
            {
                if (string.IsNullOrEmpty(field.Value))
                    throw new ValidationFailedException("POR020_frm1_Error_Error1");

                PorteriaRegistroVehiculo currVehicula = this._uow.PorteriaRepository.GetRegistroVehiculoByMatricula(field.Value);

                if (currVehicula == null)
                    currVehicula = this._uow.PorteriaRepository.GetRegistroVehiculoByDocumento(field.Value);

                if (currVehicula == null || currVehicula.DT_PORTERIA_SALIDA != null)
                    throw new ValidationFailedException("POR020_frm1_Error_Error1");

                FieldNU_PORTERIA_VEHICULO.Value = currVehicula.NU_PORTERIA_VEHICULO.ToString();
                FieldVL_MATRICULA_2.Value = currVehicula.VL_MATRICULA_2;
                FieldVL_PESO_ENTRADA.Value = (currVehicula.VL_PESO_ENTRADA ?? 0).ToString();

                if (currVehicula.Personas != null && currVehicula.Personas.Count > 0)
                {
                    if (currVehicula.Personas.FirstOrDefault().ND_SECTOR != null)
                    {
                        PorteriaSector domSector = this._uow.PorteriaRepository.GetSector(currVehicula.Personas.FirstOrDefault().CD_SECTOR);
                        if (domSector != null)
                        {
                            FieldDS_SECTOR.Value = domSector.DS_SECTOR;
                        }
                    }

                    if (currVehicula.Personas.FirstOrDefault().ND_POTERIA_MOTIVO != null)
                    {
                        DominioDetalle domMotivo = this._uow.DominioRepository.GetDominio(currVehicula.Personas.FirstOrDefault().ND_POTERIA_MOTIVO);

                        if (domMotivo != null)
                        {
                            FieldDS_POTERIA_MOTIVO.Value = domMotivo.Descripcion;
                        }
                    }
                }

                if (currVehicula.CD_EMPRESA != null)
                {
                    FieldNM_EMPRESA.Value = this._uow.EmpresaRepository.GetNombre(currVehicula.CD_EMPRESA ?? 1);
                    if (!string.IsNullOrEmpty(currVehicula.Personas.FirstOrDefault().CD_AGENTE))
                    {
                        var agente = this._uow.AgenteRepository.GetAgente(currVehicula.CD_EMPRESA ?? 1, currVehicula.CD_AGENTE, currVehicula.TP_AGENTE);

                        FieldNM_AGENTE.Value = agente.Descripcion;
                    }
                }
                FieldDT_PORTERIA_ENTRADA.Value = currVehicula.DT_PORTERIA_ENTRADA.ToIsoString();

                parameters.Add(new ComponentParameter("SelectionGridContainers", JsonConvert.SerializeObject(this._uow.PorteriaRepository.GetContainersDeVehiculo(currVehicula.NU_PORTERIA_VEHICULO))));

            }
            catch (Exception ex)
            {

                FieldNU_PORTERIA_VEHICULO.Value = "";
                FieldVL_MATRICULA_2.Value = "";
                FieldVL_PESO_ENTRADA.Value = "";
                FieldDS_SECTOR.Value = "";
                FieldDS_POTERIA_MOTIVO.Value = "";
                FieldNM_EMPRESA.Value = "";
                FieldNM_AGENTE.Value = "";
                FieldDT_PORTERIA_ENTRADA.Value = "";

                field.SetError(ex.Message);

                parameters.Add(new ComponentParameter("SelectionGridContainers", "[]"));
            }
        }
    }
}
