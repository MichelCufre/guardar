using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoUbicacionesValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoUbicacionesValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateIdEmpresa,
                ["CD_CLASSE"] = this.ValidateIdClase,
                ["CD_AREA_ARMAZ"] = this.ValidateIdArea,
                ["CD_FAMILIA_PRINCIPAL"] = this.ValidateIdFamiliaProducto,
                ["CD_ROTATIVIDADE"] = this.ValidateIdRotatividad,
                ["CD_TIPO_ENDERECO"] = this.ValidateIdUbicacionTipo,
                ["ID_ENDERECO_BAIXO"] = this.ValidateUbicacionBaja,
                ["NU_COMPONENTE"] = this.ValidateComponente,
                ["CD_CONTROL"] = this.ValidateControl,
                ["CD_ZONA_UBICACION"] = this.ValidateIdZonaUbicacion,
                ["CD_CONTROL_ACCESO"] = this.ValidateControlAcceso,
            };
        }

        public virtual GridValidationGroup ValidateIdEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            if (cell.Old == cell.Value)
                return null;

            string idUbicacion = row.GetCell("CD_ENDERECO").Value;

            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveNumberMaxLengthValidationRule(cell.Value,10),
                    new ExisteEmpresaValidationRule(_uow, cell.Value),
                    new UbicacionConStockValidationRule(_uow, idUbicacion),
                },
                OnSuccess = this.ValidateIdEmpresa_OnSuccess,
                OnFailure = this.ValidateIdEmpresa_OnFailure

            };
        }
        public virtual void ValidateIdEmpresa_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("NM_EMPRESA");

            Empresa empresa = _uow.EmpresaRepository.GetEmpresa(int.Parse(cell.Value));

            field.Value = empresa.Nombre;
        }
        public virtual void ValidateIdEmpresa_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("NM_EMPRESA").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdClase(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            if (cell.Old == cell.Value)
                return null;

            string idUbicacion = row.GetCell("CD_ENDERECO").Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new ClaseNoExistenteValidationRule(_uow, cell.Value),
                    new UbicacionConStockValidationRule(_uow, idUbicacion),
                },
                OnSuccess = this.ValidateClase_OnSuccess,
                OnFailure = this.ValidateClase_OnFailure
            };

        }
        public virtual void ValidateClase_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("DS_CLASSE");

            var clase = _uow.ClaseRepository.GetClaseById(cell.Value);

            field.Value = clase.Descripcion;
        }
        public virtual void ValidateClase_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_CLASSE").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdArea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            if (cell.Old == cell.Value)
                return null;

            string idUbicacion = row.GetCell("CD_ENDERECO").Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value,2),
                    new UbicacionPickingProductoAsigandoValidationRule(_uow, idUbicacion),
                    new IdUbicacionAreaNoExistenteValidationRule(_uow, cell.Value),
                    new UbicacionConStockValidationRule(_uow, idUbicacion),
                    new IdUbicacionAreaNoMantenibleValidationRule(_uow, cell.Value),
                },
                OnSuccess = this.ValidateArea_OnSuccess,
                OnFailure = this.ValidateArea_OnFailure
            };

        }
        public virtual void ValidateArea_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("DS_AREA_ARMAZ");

            UbicacionArea area = _uow.UbicacionAreaRepository.GetUbicacionArea(short.Parse(cell.Value));

            field.Value = area.Descripcion;
        }
        public virtual void ValidateArea_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_AREA_ARMAZ").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdFamiliaProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new IdProductoFamiliaNoExistenteValidationRule(_uow, cell.Value),
                },
                OnSuccess = this.ValidateFamiliaProducto_OnSuccess,
                OnFailure = this.ValidateFamiliaProducto_OnFailure
            };

        }
        public virtual void ValidateFamiliaProducto_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("DS_FAMILIA_PRODUTO");

            ProductoFamilia area = _uow.ProductoFamiliaRepository.GetFamiliaProducto(int.Parse(cell.Value));

            field.Value = area.Descripcion;
        }
        public virtual void ValidateFamiliaProducto_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_FAMILIA_PRODUTO").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdRotatividad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value,2),
                    new IdProductoRotatividadNoExistenteValidationRule(_uow,  cell.Value),
                },
                OnSuccess = this.ValidateRotatividad_OnSuccess,
                OnFailure = this.ValidateRotatividad_OnFailure
            };

        }
        public virtual void ValidateRotatividad_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("DS_ROTATIVIDADE");

            ProductoRotatividad rotatividad = _uow.ProductoRotatividadRepository.GetProductoRotatividad(short.Parse(cell.Value));

            field.Value = rotatividad.Descripcion;
        }
        public virtual void ValidateRotatividad_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_ROTATIVIDADE").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdZonaUbicacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new IdZonaUbicacionValidationRule(_uow,  cell.Value),
                },
                OnSuccess = this.ValidateZonaUbicacion_OnSuccess,
                OnFailure = this.ValidateZonaUbicacion_OnFailure
            };

        }
        public virtual void ValidateZonaUbicacion_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("DS_ZONA_UBICACION");

            ZonaUbicacion zona = _uow.ZonaUbicacionRepository.GetZona(cell.Value);

            field.Value = zona.Descripcion;
        }
        public virtual void ValidateZonaUbicacion_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_ZONA_UBICACION").Value = string.Empty;
        }


        public virtual GridValidationGroup ValidateIdUbicacionTipo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            string idUbicacion = row.GetCell("CD_ENDERECO").Value;

            if (cell.Old == cell.Value)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value,2),
                    new IdUbicacionTipoNoExistenteValidationRule(_uow, cell.Value),
                    new IdUbicacionTipoMonoProductoMonoLoteValidationRule(_uow, short.Parse( cell.Value),idUbicacion),
                },
                OnSuccess = this.ValidateUbicacionTipo_OnSuccess,
                OnFailure = this.ValidateUbicacionTipo_OnFailure
            };

        }
        public virtual void ValidateUbicacionTipo_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var field = row.GetCell("DS_TIPO_ENDERECO");

            UbicacionTipo rotatividad = _uow.UbicacionTipoRepository.GetUbicacionTipo(short.Parse(cell.Value));

            field.Value = rotatividad.Descripcion;
        }
        public virtual void ValidateUbicacionTipo_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_TIPO_ENDERECO").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateUbicacionBaja(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new BooleanStringGridValidationRule(cell.Value),
                },
            };
        }

        public virtual GridValidationGroup ValidateComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            if (!string.IsNullOrEmpty(cell.Value))
            {
                return new GridValidationGroup
                {
                    Rules = new List<IValidationRule> {
                        new StringMaxLengthValidationRule(cell.Value, 4),
                     },
                };
            }
            else
                return null;
        }

        public virtual GridValidationGroup ValidateControl(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            cell.Value = cell.Value.ToUpper();

            if (cell.Old == cell.Value)
            {
                cell.Status = GridStatus.Ok;
                return null;
            }


            if (!string.IsNullOrEmpty(cell.Value))
            {

                string columna = row.GetCell("NU_COLUMNA").Value;

                return new GridValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule> {
                        new StringMaxLengthValidationRule(cell.Value, 5),
                        new StringSoloLetrasValidationRule(cell.Value),
                        new DigitoControlUbicacionValidationRule(_uow, cell.Value,int.Parse(columna)),
                     },
                };
            }
            else
                return null;
        }

        public virtual GridValidationGroup ValidateControlAcceso(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            if (cell.Old == cell.Value)
            {
                cell.Status = GridStatus.Ok;
                return null;
            }

            if (!string.IsNullOrEmpty(cell.Value))
            {
                string columna = row.GetCell("NU_COLUMNA").Value;

                return new GridValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule> {
                        new ExisteControlAccesoValidationRule(_uow,cell.Value),
                     },
                };
            }
            else
                return null;
        }

    }
}
