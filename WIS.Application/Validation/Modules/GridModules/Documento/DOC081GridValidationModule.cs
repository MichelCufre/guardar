using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Documento
{
    public class DOC081GridValidationModule : GridValidationModule
    {
        protected readonly string _nroDocumento;
        protected readonly string _tipoDocumento;
        protected readonly string _cdEmpresa;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public DOC081GridValidationModule(
            string nroDocumento,
            string tipoDocumento,
            string cdEmpresa,
            IUnitOfWork uow,
            IFormatProvider culture)
        {
            this._nroDocumento = nroDocumento;
            this._tipoDocumento = tipoDocumento;
            this._cdEmpresa = cdEmpresa;
            this._uow = uow;
            this._culture = culture;

            Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_INGRESADA"] = this.ValidateCantidadIngresada,
                ["VL_MERCADERIA"] = this.ValidateValorMercaderia,
                ["VL_TRIBUTO"] = this.ValidateValorTributo
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 40),
                new ProductoExistsValidationRule(this._uow, this._cdEmpresa, cell.Value)
            };

            if (row.IsNew && !string.IsNullOrEmpty(cell.Value))
            {
                Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(this._cdEmpresa), cell.Value, false);

                if (producto != null && producto.IsIdentifiedByProducto())
                    rules.Add(new DocumentoProductoNotExistsValidationRule(this._uow, this._nroDocumento, this._tipoDocumento, int.Parse(this._cdEmpresa), cell.Value, producto.ParseIdentificador(null), 1));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateCD_PRODUTO_OnSuccess,
                OnFailure = this.ValidateCD_PRODUTO_OnFailure
            };
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int cdEmpresa = int.Parse(this._cdEmpresa);

            string cdProducto = row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(cdProducto))
                return null;

            var rules = new List<IValidationRule> {
                new StringMaxLengthValidationRule(cell.Value, 40),
                new IdentificadorProductoValidationRule(this._uow, cdEmpresa, cdProducto, cell.Value),
            };

            if (row.IsNew)
            {
                Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cdProducto, false);

                if (producto != null)
                    rules.Add(new DocumentoProductoNotExistsValidationRule(this._uow, this._nroDocumento, this._tipoDocumento, cdEmpresa, cdProducto, producto.ParseIdentificador(cell.Value), 1));
            }

            return new GridValidationGroup
            {
                Dependencies = { "CD_PRODUTO" },
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup ValidateCantidadIngresada(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int cdEmpresa = int.Parse(this._cdEmpresa);
            string cdProducto = row.GetCell("CD_PRODUTO").Value;
            string nroIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
            var tipo = this._uow.DocumentoTipoRepository.GetTipoDocumento(this._tipoDocumento);

            if (string.IsNullOrEmpty(cdProducto) || string.IsNullOrEmpty(nroIdentificador))
                return null;

            var producto = _uow.ProductoRepository.GetProducto(cdEmpresa, cdProducto);

            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new DecimalCultureSeparatorValidationRule(this._culture, cell.Value),
                new PositiveDecimalValidationRule(this._culture, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture, producto.AceptaDecimales),
                new DecimalGreaterThanValidationRule(this._culture, cell.Value, tipo.CantidadMinimaIngresada)
            };

            if (nroIdentificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture));

            return new GridValidationGroup
            {
                Dependencies = { "NU_IDENTIFICADOR" },
                BreakValidationChain = true,
                Rules = rules
            };
        }
        public virtual GridValidationGroup ValidateValorMercaderia(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture),
                    new DecimalCultureSeparatorValidationRule(this._culture, cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                    new DecimalGreaterThanValidationRule(this._culture, cell.Value, Convert.ToDecimal(0.001))
                }
            };
        }
        public virtual GridValidationGroup ValidateValorTributo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            GridValidationGroup gridValidationGroup = new GridValidationGroup();

            if (!string.IsNullOrEmpty(cell.Value))
            {
                gridValidationGroup.Rules = new List<IValidationRule>
                {
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture),
                    new DecimalCultureSeparatorValidationRule(this._culture, cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                    new DecimalGreaterThanValidationRule(this._culture, cell.Value, Convert.ToDecimal(0.001))
                };
            }

            return gridValidationGroup;
        }

        public virtual void ValidateCD_PRODUTO_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int cdEmpresa = int.Parse(this._cdEmpresa);
            var cellDsProducto = row.GetCell("DS_PRODUTO");
            var cellDsProductoIngreso = row.GetCell("DS_PRODUTO_INGRESO");
            var cellIdentificador = row.GetCell("NU_IDENTIFICADOR");

            Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cell.Value, false);

            if (producto != null)
            {
                if (cellDsProducto != null)
                    cellDsProducto.Value = producto.Descripcion;

                if (cellDsProductoIngreso != null)
                    cellDsProductoIngreso.Value = producto.Descripcion;

                if (producto.IsIdentifiedByProducto())
                {
                    cellIdentificador.Value = producto.ParseIdentificador(string.Empty);
                    cellIdentificador.Editable = false;
                }
                else
                    cellIdentificador.Editable = true;
            }
            else
                cellIdentificador.Editable = true;
        }

        public virtual void ValidateCD_PRODUTO_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_PRODUTO_INGRESO").Value = string.Empty;
            row.GetCell("DS_PRODUTO").Value = string.Empty;
        }

    }
}
