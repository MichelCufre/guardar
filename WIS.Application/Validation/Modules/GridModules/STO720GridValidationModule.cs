using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class STO720GridValidationModule : GridValidationModule
    {
        protected IUnitOfWork _uow;
        protected IFormatProvider _culture;

        public STO720GridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;
            this.Schema = new GridValidationSchema
            {
                ["DT_FABRICACAO"] = this.ValidateVencimiento,
                ["ID_AVERIA"] = this.ValidateAveria,
                ["CD_MOTIVO_AVERIA"] = this.ValidateMotivoAveria
            };
        }

        public virtual GridValidationGroup ValidateVencimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (cell.Value == null)
                return null;

			string ubicacion = row.GetCell("CD_ENDERECO").Value;
			string producto = row.GetCell("CD_PRODUTO").Value;
			string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
			int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
			long nuLpn = long.Parse(row.GetCell("NU_LPN").Value);
			int idLpnDet = int.Parse(row.GetCell("ID_LPN_DET").Value);
			decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _culture);
			decimal qtStock = decimal.Parse(row.GetCell("QT_ESTOQUE").Value, _culture);
            DateTime? value = DateTimeExtension.ParseFromIso(row.GetCell("DT_FABRICACAO").Value);
			DateTime? oldValue = DateTimeExtension.ParseFromIso(row.GetCell("DT_FABRICACAO").Old);

			bool isExpirable = _uow.ProductoRepository.GetProducto(empresa, producto).IsFefo();

			return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new DateTimeValidationRule(cell.Value),
                    new VencimientoStockLpnValidationRule(_uow, value, oldValue,  nuLpn,idLpnDet,ubicacion,empresa,producto,identificador,faixa,qtStock, isExpirable)
				}
            };
        }

        public virtual GridValidationGroup ValidateMotivoAveria(GridCell cell, GridRow row, List<ComponentParameter> list)
        {
            string averiado = row.GetCell("ID_AVERIA").Value;
            if (averiado == "N")
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                     new NonNullValidationRule(cell.Value),
                }
            };
        }

        public virtual GridValidationGroup ValidateAveria(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string ubicacion = row.GetCell("CD_ENDERECO").Value;
            string producto = row.GetCell("CD_PRODUTO").Value;
            string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            long nuLpn = long.Parse(row.GetCell("NU_LPN").Value);
            int idLpnDet = int.Parse(row.GetCell("ID_LPN_DET").Value);
            decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _culture);
            decimal qtStock = decimal.Parse(row.GetCell("QT_ESTOQUE").Value, _culture);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new NonNullValidationRule(cell.Value),
                    new AveriaStockValidationRule(cell.Value),
                    new AveriaStockLpnValidationRule(_uow,cell.Value,cell.Old, nuLpn,idLpnDet,ubicacion,empresa,producto,identificador,faixa,qtStock)
                },OnSuccess = this.ValidateColumnaAveria
            };
        }
        public virtual void ValidateColumnaAveria(GridCell cell, GridRow row, List<ComponentParameter> list)
        {
            if (cell.Value == "S")
            {
                row.GetCell("CD_MOTIVO_AVERIA").Editable = true;
            }
            else
            {
                row.GetCell("CD_MOTIVO_AVERIA").Editable = false;
                row.GetCell("CD_MOTIVO_AVERIA").Value = string.Empty;
            }
        }
    }
}
