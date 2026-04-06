using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Documento
{
    public class DOC080FormValidationModule : FormValidationModule
    {
        protected readonly bool _editMode;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly IIdentityService _identity;

        public DOC080FormValidationModule(
            bool editMode,
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._editMode = editMode;
            this._uow = uow;
            this._identity = identity;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["tpIngreso"] = this.ValidateTP_INGRESO,
                ["nroDocNoGenerado"] = this.ValidateNU_DOCUMENTO_MANUAL,
                ["cdEmpresa"] = this.ValidateCD_EMPRESA,
                ["cdCliente"] = this.ValidateCD_CLIENTE,
                ["nroExport"] = this.ValidateNU_EXPORT,
                ["nroImport"] = this.ValidateNU_IMPORT,
                ["cdDespachante"] = this.ValidateCD_DESPACHANTE,
                ["nroFactura"] = this.ValidateNU_FACTURA,
                ["nroConocimiento"] = this.ValidateNU_CONOCIMIENTO,
                ["qtBultos"] = this.ValidateQT_BULTO,
                ["cdUnidadMedida"] = this.ValidateCD_UNIDAD_MEDIDA_BULTO,
                ["qtPeso"] = this.ValidateQT_PESO,
                ["qtContenedor20"] = this.ValidateQT_CONTENEDOR20,
                ["qtContenedor40"] = this.ValidateQT_CONTENEDOR40,
                ["tpAlmacSeguro"] = this.ValidateTP_ALMACENAJE_Y_SEGURO,
                ["cdVia"] = this.ValidateCD_VIA,
                ["cdTransportadora"] = this.ValidateCD_TRANSPORTISTA,
                ["fechProgramado"] = this.ValidateDT_PROGRAMADO,
                ["qtVolumen"] = this.ValidateQT_VOLUMEN,
                ["vlArbitraje"] = this.ValidateVL_ARBITRAJE,
                ["descDocumento"] = this.ValidateDS_DOCUMENTO,
                ["descAnexo1"] = this.ValidateDS_ANEXO1,
                ["descAnexo2"] = this.ValidateDS_ANEXO2,
                ["descAnexo3"] = this.ValidateDS_ANEXO3,
                ["descAnexo4"] = this.ValidateDS_ANEXO4,
                ["descAnexo5"] = this.ValidateDS_ANEXO5,
                ["cdMoneda"] = this.ValidateCD_MONEDA,
                ["vlSeguro"] = this.ValidateVL_SEGURO,
                ["vlFlete"] = this.ValidateVL_FLETE,
                ["vlOtrosGastos"] = this.ValidateVL_OTROS_GASTOS,
                ["icms"] = this.ValidateICMS,
                ["ii"] = this.ValidateII,
                ["ipi"] = this.ValidateIPI,
                ["iisuspenso"] = this.ValidateIISUSPENSO,
                ["ipisuspenso"] = this.ValidateIPISUSPENSO,
                ["pisconfins"] = this.ValidatePISCONFINS,
                ["cdRegimenAduana"] = this.ValidateCD_REGIMEN_ADUANA,
                ["predio"] = this.ValidatePredio
            };
        }

        public virtual FormValidationGroup ValidateTP_INGRESO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new TipoDocumentoIngresoPermiteAltaManual(field.Value,this._uow)
                },
                OnSuccess = this.ValidateTP_INGRESO_OnSuccess
            };
        }

        public virtual FormValidationGroup ValidateNU_DOCUMENTO_MANUAL(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpIngreso = form.GetField("tpIngreso").Value;
            var tipoDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(tpIngreso);

            if (!this._editMode && !tipoDocumento.NumeroAutogenerado)
            {
                var mask = tipoDocumento.Mask;
                if (!string.IsNullOrEmpty(mask) && parameters.Any(a => a.Id == "nroDocNoGenerado"))
                {
                    var value = parameters.FirstOrDefault(a => a.Id == "nroDocNoGenerado").Value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        var maskChars = tipoDocumento.MaskChars ?? string.Empty;
                        foreach (var c in tipoDocumento.MaskChars)
                        {
                            value = value.Replace(c.ToString(), string.Empty);
                        }
                    }

                    field.Value = value;
                }

                return new FormValidationGroup
                {
                    Dependencies = { "tpIngreso" },
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,10),
                        new PositiveLongValidationRule(field.Value),
                        new NumeroDocumentoIngresoNoAutogeneradoValidationRule(tpIngreso, field.Value, this._uow)
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateCD_EMPRESA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteEmpresaValidationRule(this._uow, field.Value)
                },
                OnSuccess = this.ValidateCD_EMPRESA_OnSuccess
            };
        }

        public virtual FormValidationGroup ValidateNU_EXPORT(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,20)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_IMPORT(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,20)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCD_DESPACHANTE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new PositiveShortNumberMaxLengthValidationRule(field.Value,5),
                        new ExisteDespachanteValidationRule(this._uow, field.Value)
                    },
                    OnSuccess = this.ValidateCD_DESPACHANTE_OnSuccess
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_FACTURA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_CONOCIMIENTO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,12)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateQT_BULTO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new PositiveNumberMaxLengthValidationRule(field.Value,6)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCD_UNIDAD_MEDIDA_BULTO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,10),
                    new ExisteUnidadDeMedidaValidationRule(this._uow, field.Value)
                },
                    OnSuccess = this.ValidateCD_UNIDAD_MEDIDA_BULTO_OnSuccess
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateQT_PESO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,12,3, this._culture )
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateQT_CONTENEDOR20(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateQT_CONTENEDOR40(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateTP_ALMACENAJE_Y_SEGURO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,3),
                    new ExisteTipoAlmacenajeSeguroValidationRule(this._uow, field.Value)
                },
                    OnSuccess = this.ValidateTP_ALMACENAJE_Y_SEGURO_OnSuccess
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCD_VIA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var editMode = bool.Parse(parameters.Find(p => p.Id == "editar")?.Value ?? false.ToString());

            if (editMode)
                return null;

            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,4),
                    new ExisteViaActivaValidationRule(this._uow, field.Value)
                },
                    OnSuccess = this.ValidateCD_VIA_OnSuccess
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCD_TRANSPORTISTA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteTransportadoraValidationRule(this._uow, field.Value)
                },
                    OnSuccess = this.ValidateCD_TRANSPORTISTA_OnSuccess
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDT_PROGRAMADO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringIsoDateToDateTimeValidationRule(field.Value)
                    }
            };
        }

        public virtual FormValidationGroup ValidateQT_VOLUMEN(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._culture),
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateVL_ARBITRAJE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if ((form.GetField("cdMoneda").Value != "1" && !string.IsNullOrEmpty(form.GetField("cdMoneda").Value)))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new ValorArbitrajeValidationRule(field.Value,form.GetField("cdMoneda").Value, this._culture),
                        new DecimalCultureSeparatorValidationRule(this._culture, field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDS_DOCUMENTO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,100)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDS_ANEXO1(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDS_ANEXO2(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDS_ANEXO3(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDS_ANEXO4(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDS_ANEXO5(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCD_DEPOSITO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,3)
                }
            };
        }

        public virtual FormValidationGroup ValidateCD_CLIENTE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            var editMode = bool.Parse(parameters.Find(p => p.Id == "editar")?.Value ?? false.ToString());

            if (editMode)
                return null;

            var cdEmpresa = form.GetField("cdEmpresa");

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new ExisteClienteProveedorEmpresaValidationRule(this._uow, field.Value, cdEmpresa.Value)
                },
                OnSuccess = this.ValidateCD_CLIENTE_OnSuccess
            };
        }

        public virtual FormValidationGroup ValidateCD_MONEDA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,15),
                    new ExisteMonedaValidationRule(this._uow, field.Value)
                },
                    OnSuccess = this.ValidateCD_MONEDA_OnSuccess
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateVL_SEGURO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateVL_FLETE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalLengthWithPresicionValidationRule(field.Value,12,3, this._culture),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateVL_OTROS_GASTOS(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._culture),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value)
                }
                };
            else
                return null;
        }

        public virtual void ValidateTP_INGRESO_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                var tipoDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(field.Value);
                parameters.RemoveAll(p => p.Id == "isAutoGenerado" || p.Id == "formatoNumDoc");
                parameters.Add(new ComponentParameter() { Id = "isAutoGenerado", Value = tipoDocumento.NumeroAutogenerado.ToString().ToLower() });
                parameters.Add(new ComponentParameter() { Id = "formatoNumDoc", Value = tipoDocumento.Mask ?? "" });
            }
        }

        public virtual void ValidateCD_EMPRESA_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descEmpresa = form.GetField("descEmpresa");

            int cdEmpresa = int.Parse(field.Value);

            descEmpresa.Value = this._uow.EmpresaRepository.GetNombre(cdEmpresa);
        }

        public virtual void ValidateCD_CLIENTE_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descProveedor = form.GetField("descProveedor");
            var cdEmpresaValue = form.GetField("cdEmpresa");
            int cdEmpresa = int.Parse(cdEmpresaValue.Value);
            var agente = this._uow.AgenteRepository.GetAgente(cdEmpresa, field.Value);

            descProveedor.Value = agente.Descripcion;
        }

        public virtual void ValidateCD_DESPACHANTE_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descDespachante = form.GetField("descDespachante");
            short cdDspachante = short.Parse(field.Value);
            var despachante = this._uow.DespachanteRepository.GetDespachante(cdDspachante);

            descDespachante.Value = despachante.Nombre;
        }

        public virtual void ValidateCD_MONEDA_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descMoneda = form.GetField("descMoneda");

            descMoneda.Value = this._uow.MonedaRepository.GetDescripcion(field.Value);
        }

        public virtual void ValidateCD_UNIDAD_MEDIDA_BULTO_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descUniMedida = form.GetField("descUniMedida");

            descUniMedida.Value = this._uow.UnidadMedidaRepository.GetDescripcion(field.Value);
        }

        public virtual void ValidateTP_ALMACENAJE_Y_SEGURO_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descAlmacSeguro = form.GetField("descAlmacSeguro");
            short tpAlmacenajeSeguro = short.Parse(field.Value);

            descAlmacSeguro.Value = this._uow.TipoAlmacenajeSeguroRepository.GetDescripcion(tpAlmacenajeSeguro);
        }

        public virtual void ValidateCD_VIA_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descVia = form.GetField("descVia");

            descVia.Value = this._uow.ViaRepository.GetDescripcion(field.Value);
        }

        public virtual void ValidateCD_TRANSPORTISTA_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var descTransportista = form.GetField("descTransportadora");
            var cdTransportista = int.Parse(field.Value);

            descTransportista.Value = this._uow.TransportistaRepository.GetDescripcion(cdTransportista);
        }

        public virtual FormValidationGroup ValidateICMS(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateII(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateIPI(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateIISUSPENSO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateIPISUSPENSO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidatePISCONFINS(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                {
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,9,4,this._culture)
                }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCD_REGIMEN_ADUANA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new PositiveNumberMaxLengthValidationRule(field.Value,10),
                        new ExisteRegimenAduanero(this._uow, field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15),
                   new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
                }
            };
        }
    }
}
