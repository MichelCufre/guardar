using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class RegistroCapacidadDePalletVlidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public RegistroCapacidadDePalletVlidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateCodigoEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["CD_PALLET"] = this.ValidatePallet,
                ["NU_PRIORIDAD"] = this.ValidateNumeroPrioridad,
                ["QT_UNIDADES"] = this.ValidateUnidades,
            };
        }

        public virtual GridValidationGroup ValidateCodigoEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : cell.Value;
            var existeEmpresaValidationRule = new ExisteEmpresaValidationRule(this._uow, idEmpresa);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(idEmpresa),
            };

            if (!string.IsNullOrEmpty(idEmpresa))
                rules.Add(existeEmpresaValidationRule);
            else
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessGridValidateEmpresa
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;
            var idProducto = parameters.Any(s => s.Id == "producto") ? parameters.FirstOrDefault(s => s.Id == "producto").Value : cell.Value;

            if (string.IsNullOrEmpty(idEmpresa))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(idProducto),
                    new StringMaxLengthValidationRule(idProducto, 40),
                    new ProductoExisteEmpresaValidationRule(_uow, idEmpresa, idProducto),
                },
                OnSuccess = this.OnSuccessGridValidateProducto
            };
        }

        public virtual GridValidationGroup ValidatePallet(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;
            var idProducto = parameters.Any(s => s.Id == "producto") ? parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(idEmpresa) || string.IsNullOrEmpty(idProducto))
                return null;
            else if (cell.Old == cell.Value && !row.IsNew)
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value,3),
                    new PalletYaDefinido(this._uow, cell.Value, int.Parse(idEmpresa), idProducto),
                },
                OnSuccess = this.OnSuccessGridValidatePallet
            };
        }

        public virtual GridValidationGroup ValidateNumeroPrioridad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value, 3)
                }
            };
        }

        public virtual GridValidationGroup ValidateUnidades(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveLongValidationRule(cell.Value),
                },
            };
        }

        public virtual void OnSuccessGridValidatePallet(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var pallet = this._uow.FacturacionRepository.GetPallet(short.Parse(cell.Value));
            row.GetCell("DS_PALLET").Value = pallet?.Descripcion;
        }

        public virtual void OnSuccessGridValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;
            var idProducto = parameters.Any(s => s.Id == "producto") ? parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;
            var descProd = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(idEmpresa), idProducto).Descripcion;

            if (row.GetCell("DS_PRODUTO") != null)
                row.GetCell("DS_PRODUTO").Value = descProd;
        }

        public virtual void OnSuccessGridValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;

            if (row.GetCell("NM_EMPRESA") != null)
                row.GetCell("NM_EMPRESA").Value = _uow.EmpresaRepository.GetEmpresa(int.Parse(idEmpresa)).Nombre;
        }
    }
}