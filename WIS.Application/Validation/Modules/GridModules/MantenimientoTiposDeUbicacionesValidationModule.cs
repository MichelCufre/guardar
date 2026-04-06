using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Parametrizacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    class MantenimientoTiposDeUbicacionesValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public MantenimientoTiposDeUbicacionesValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["CD_TIPO_ENDERECO"] = this.ValidateCodigo,
                ["DS_TIPO_ENDERECO"] = this.ValidateDescripcionEndereco,
                ["VL_ALTURA"] = this.ValidateAltura,
                ["VL_LARGURA"] = this.ValidateLargo,
                ["VL_COMPRIMENTO"] = this.ValidateAncho,
                ["VL_PESO_MAXIMO"] = this.ValidatePesoMaximo,
                ["CD_TIPO_ESTRUTURA"] = this.ValidateCodigoEstructura,
                ["ID_VARIOS_LOTES"] = this.ValidateVariosLotes,
                ["ID_VARIOS_PRODUTOS"] = this.ValidateVariosProductos,
                ["QT_VOLUMEN_UNIDAD_FACTURACION"] = this.ValidateVolumenUnidadFacturacion,
                ["QT_CAPAC_PALETES"] = this.ValidateCapacidadPallet,
                ["FL_RESPETA_CLASE"] = this.ValidateRespetaClase,
            };
        }

        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            ExisteCodigoTipoDeUbicacionValidationRule existeTipoCodigoBarraRegistradaValidationRule = new ExisteCodigoTipoDeUbicacionValidationRule(cell.Value, _uow);
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value, 2),
                }
            };

            if (!string.IsNullOrEmpty(cell.Value) && row.IsNew)
                rules.Rules.Add(existeTipoCodigoBarraRegistradaValidationRule);
            else
                rules.Rules.Remove(existeTipoCodigoBarraRegistradaValidationRule);

            return rules;
        }
        public virtual GridValidationGroup ValidateDescripcionEndereco(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 30),
                    new StringSoloUpperValidationRule(cell.Value),
                }
            };
        }
        public virtual GridValidationGroup ValidateAltura(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }
        public virtual GridValidationGroup ValidateLargo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }
        public virtual GridValidationGroup ValidateAncho(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }
        public virtual GridValidationGroup ValidatePesoMaximo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }
        public virtual GridValidationGroup ValidateCodigoEstructura(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortValidationRule(cell.Value)
                };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.OnSuccessValidateCodigoEstructura
            };
        }
        public virtual void OnSuccessValidateCodigoEstructura(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            TipoDeEstructura tipo = this._uow.UbicacionTipoRepository.GetTipoEstructura(short.Parse(cell.Value));

            row.GetCell("DS_TP_ESTRUCTURA").Value = tipo.Tipo;
        }
        public virtual GridValidationGroup ValidateVariosLotes(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                }
            };
        }
        public virtual GridValidationGroup ValidateVariosProductos(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                    new StringToBooleanValidationRule(cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateRespetaClase(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                    new StringToBooleanValidationRule(cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateVolumenUnidadFacturacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }
        public virtual GridValidationGroup ValidateCapacidadPallet(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                }
            };
        }
    }
}
