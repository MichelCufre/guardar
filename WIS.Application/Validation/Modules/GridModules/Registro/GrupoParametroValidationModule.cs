using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Registro
{
    public class GrupoParametroValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _proveedor;
        protected readonly List<ComponentParameter> _parameters;

        public GrupoParametroValidationModule(IUnitOfWork uow, List<ComponentParameter> parameters, IFormatProvider proveedor)
        {
            _uow = uow;
            _proveedor = proveedor;
            _parameters = parameters;

            this.Schema = new GridValidationSchema
            {
                ["VL_PARAM"] = this.GridValidateParametro,
            };
        }

        public virtual GridValidationGroup GridValidateParametro(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();

            var validar = true;
            if (!string.IsNullOrEmpty(cell.Value))
            {
                var parametro = new GrupoParametroValidate()
                {
                    Codigo = row.GetCell("NM_PARAM").Value,
                    Tipo = row.GetCell("TP_PARAM").Value,
                    Precision = null,
                    Dependencia = string.Empty,
                    ValidacionDesdeHasta = true,
                };

                switch (parametro.Codigo)
                {
                    case "CD_EXCLUSIVO":
                        parametro.ValidacionDesdeHasta = false;
                        parametro.Largo = 4;
                        break;

                    case "QT_DIAS_VALIDADE_DESDE":
                        parametro.Largo = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_DIAS_VALIDADE_HASTA")?.Value;
                        break;
                    case "QT_DIAS_VALIDADE_HASTA":
                        parametro.Largo = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_DIAS_VALIDADE_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_DIAS_DURACAO_DESDE":
                        parametro.Largo = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_DIAS_DURACAO_HASTA")?.Value;
                        break;
                    case "QT_DIAS_DURACAO_HASTA":
                        parametro.Largo = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_DIAS_DURACAO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_DIAS_VALIDADE_LIBERACION_DESDE":
                        parametro.Largo = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_DIAS_VALIDADE_LIBERACION_HASTA")?.Value;
                        break;
                    case "QT_DIAS_VALIDADE_LIBERACION_HASTA":
                        parametro.Largo = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_DIAS_VALIDADE_LIBERACION_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_ESTOQUE_MINIMO_DESDE":
                        parametro.Largo = 9;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_ESTOQUE_MINIMO_HASTA")?.Value;
                        break;
                    case "QT_ESTOQUE_MINIMO_HASTA":
                        parametro.Largo = 9;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_ESTOQUE_MINIMO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_ESTOQUE_MAXIMO_DESDE":
                        parametro.Largo = 9;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_ESTOQUE_MAXIMO_HASTA")?.Value;
                        break;
                    case "QT_ESTOQUE_MAXIMO_HASTA":
                        parametro.Largo = 9;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_ESTOQUE_MAXIMO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "PS_LIQUIDO_DESDE":
                        parametro.Largo = 15;
                        parametro.Precision = 6;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "PS_LIQUIDO_HASTA")?.Value;
                        break;
                    case "PS_LIQUIDO_HASTA":
                        parametro.Largo = 15;
                        parametro.Precision = 6;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "PS_LIQUIDO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "PS_BRUTO_DESDE":
                        parametro.Largo = 15;
                        parametro.Precision = 6;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "PS_BRUTO_HASTA")?.Value;
                        break;
                    case "PS_BRUTO_HASTA":
                        parametro.Largo = 15;
                        parametro.Precision = 6;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "PS_BRUTO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_ALTURA_DESDE":
                        parametro.Largo = 10;
                        parametro.Precision = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_ALTURA_HASTA")?.Value;
                        break;
                    case "VL_ALTURA_HASTA":
                        parametro.Largo = 10;
                        parametro.Precision = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_ALTURA_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_LARGURA_DESDE":
                        parametro.Largo = 10;
                        parametro.Precision = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_LARGURA_HASTA")?.Value;
                        break;
                    case "VL_LARGURA_HASTA":
                        parametro.Largo = 10;
                        parametro.Precision = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_LARGURA_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_PROFUNDIDADE_DESDE":
                        parametro.Largo = 10;
                        parametro.Precision = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_PROFUNDIDADE_HASTA")?.Value;
                        break;
                    case "VL_PROFUNDIDADE_HASTA":
                        parametro.Largo = 10;
                        parametro.Precision = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_PROFUNDIDADE_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_CUBAGEM_DESDE":
                        parametro.Largo = 14;
                        parametro.Precision = 4;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_CUBAGEM_HASTA")?.Value;
                        break;
                    case "VL_CUBAGEM_HASTA":
                        parametro.Largo = 14;
                        parametro.Precision = 4;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_CUBAGEM_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_UND_BULTO_DESDE":
                        parametro.Largo = 9;
                        parametro.Precision = 3;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_UND_BULTO_HASTA")?.Value;
                        break;
                    case "QT_UND_BULTO_HASTA":
                        parametro.Largo = 9;
                        parametro.Precision = 3;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_UND_BULTO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_UND_DISTRIBUCION_DESDE":
                        parametro.Largo = 9;
                        parametro.Precision = 3;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_UND_DISTRIBUCION_HASTA")?.Value;
                        break;
                    case "QT_UND_DISTRIBUCION_HASTA":
                        parametro.Largo = 9;
                        parametro.Precision = 3;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_UND_DISTRIBUCION_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_SUBBULTO_DESDE":
                        parametro.Largo = 3;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_SUBBULTO_HASTA")?.Value;
                        break;
                    case "QT_SUBBULTO_HASTA":
                        parametro.Largo = 3;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_SUBBULTO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_CUSTO_ULT_ENT_DESDE":
                        parametro.Largo = 16;
                        parametro.Precision = 2;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_CUSTO_ULT_ENT_HASTA")?.Value;
                        break;
                    case "VL_CUSTO_ULT_ENT_HASTA":
                        parametro.Largo = 16;
                        parametro.Precision = 2;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_CUSTO_ULT_ENT_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_PRECO_VENDA_DESDE":
                        parametro.Largo = 16;
                        parametro.Precision = 2;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_PRECO_VENDA_HASTA")?.Value;
                        break;
                    case "VL_PRECO_VENDA_HASTA":
                        parametro.Largo = 16;
                        parametro.Precision = 2;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_PRECO_VENDA_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "QT_GENERICO_DESDE":
                        parametro.Largo = 9;
                        parametro.Precision = 3;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "QT_GENERICO_HASTA")?.Value;
                        break;
                    case "QT_GENERICO_HASTA":
                        parametro.Largo = 9;
                        parametro.Precision = 3;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "QT_GENERICO_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;

                    case "VL_PRECIO_STOCK_DESDE":
                        parametro.Largo = 16;
                        parametro.Precision = 2;
                        parametro.ValorDesde = cell.Value;
                        parametro.ValorHasta = _parameters.FirstOrDefault(p => p.Id == "VL_PRECIO_STOCK_HASTA")?.Value;
                        break;
                    case "VL_PRECIO_STOCK_HASTA":
                        parametro.Largo = 16;
                        parametro.Precision = 2;
                        parametro.ValorDesde = _parameters.FirstOrDefault(p => p.Id == "VL_PRECIO_STOCK_DESDE")?.Value;
                        parametro.ValorHasta = cell.Value;
                        break;
                    default:
                        validar = false;
                        break;

                }

                if (validar)
                {
                    if (parametro.ValidacionDesdeHasta)
                        rules.Add(new ParametroDesdeHastaValidationRule(parametro.ValorDesde, parametro.ValorHasta, _proveedor));
                    rules.Add(new ParametroNumericoValidationRule(cell.Value, parametro.Largo, _proveedor, parametro.Precision));
                }
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,

            };
        }
    }
}
