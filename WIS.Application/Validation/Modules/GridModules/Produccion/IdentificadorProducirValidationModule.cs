using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class IdentificadorProducirValidationModule : GridValidationModule
    {
        protected readonly bool _modificando;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public IdentificadorProducirValidationModule(IUnitOfWork uow,
            IFormatProvider culture,
            bool modificando)
        {
            this._uow = uow;
            this._culture = culture;
            this._modificando = modificando;

            this.Schema = new GridValidationSchema
            {
                ["NU_ORDEN"] = this.ValidateOrden,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_STOCK"] = this.ValidateStock,
                ["DT_VENCIMIENTO"] = this.ValidateVencimiento,
                ["CD_ENDERECO"] = this.ValidateUbicacion
            };
        }

        public virtual GridValidationGroup ValidateOrden(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new ExisteEmpresaValidationRule(this._uow, cell.Value)
                },
                OnSuccess = this.ValidateEmpresaOnSuccess
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string ubicacion = parameters
                .Where(d => d.Id == "ubicacion")?.FirstOrDefault()?.Value;

            string empresaString = row.GetCell("CD_EMPRESA").Value;
            string ordenString = row.GetCell("NU_ORDEN").Value;

            if (string.IsNullOrEmpty(ubicacion))
                ubicacion = row.GetCell("CD_ENDERECO").Value;

            if (string.IsNullOrEmpty(empresaString))
                return null;

            int empresa = int.Parse(empresaString);

            var validationGroup = new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40),
                    new ProductoExistsValidationRule(this._uow, empresaString, cell.Value)
                },
                Dependencies = { "CD_EMPRESA" },
                OnSuccess = this.ValidateProductoOnSuccess
            };

            if (_modificando && !string.IsNullOrEmpty(ordenString))
            {
                int orden = int.Parse(ordenString);

                validationGroup.Rules.Add(new IdentificadorProducirDuplicateValidationRule(this._uow, ubicacion, empresa, orden, cell.Value));
                validationGroup.Dependencies.Add("NU_ORDEN");
            }

            return validationGroup;
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var validationGroup = new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 40)
                },
                Dependencies = { "CD_EMPRESA", "CD_PRODUTO" }
            };

            string empresaString = row.GetCell("CD_EMPRESA").Value;
            string codigoProducto = row.GetCell("CD_PRODUTO").Value;

            if (!string.IsNullOrEmpty(empresaString))
            {
                int empresa = int.Parse(empresaString);

                Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoProducto);

                if (!producto.IsIdentifiedByProducto())
                    validationGroup.Rules.Add(new NonNullValidationRule(cell.Value));
            }

            return validationGroup;
        }

        public virtual GridValidationGroup ValidateStock(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateVencimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var validationGroup = new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value)
                }
            };

            string empresaString = row.GetCell("CD_EMPRESA").Value;
            string codigoProducto = row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(empresaString))
                return null;

            int empresa = int.Parse(empresaString);

            Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoProducto);

            if (producto.TipoManejoFecha == ManejoFechaProductoDb.Duradero)
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new DateTimeValidationRule(cell.Value),
                    new DateTimeGreaterThanValidationRule(cell.Value, DateTime.Now.ToIsoString(), "KIT191_Sec0_Error_WB005")
                }
            };
        }

        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            row.GetCell("DS_PRODUTO").Value = this._uow.ProductoRepository.GetDescripcion(empresa, cell.Value);

            Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cell.Value);

            var cellIdentificador = row.GetCell("NU_IDENTIFICADOR");

            if (producto.IsIdentifiedByProducto())
                cellIdentificador.Value = producto.GetDefaultIdentificador();

            cellIdentificador.Editable = !producto.IsIdentifiedByProducto();
        }

        public virtual void ValidateEmpresaOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string usaMaestroLotes = this._uow.ParametroRepository.GetParameter("USA_MAESTRO_LOTES", new Dictionary<string, string>
            {
                ["CD_EMPRESA"] = row.GetCell("CD_EMPRESA").Value
            });

            row.GetCell("DT_VENCIMIENTO").Editable = usaMaestroLotes != "S";
        }

        public virtual GridValidationGroup ValidateUbicacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new ExisteUbicacionValidationRule(this._uow, cell.Value)
                }
            };
        }
    }
}
