using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoProductoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly ProductoMapper _productoMapper;

        public MantenimientoProductoFormValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;
            this._productoMapper = new ProductoMapper();

            this.Schema = new FormValidationSchema
            {
                ["empresa"] = this.ValidateCodigoEmpresa,
                ["codigo"] = this.ValidateCodigo,
                ["descripcion"] = this.ValidateDescripcion,
                ["unidadMedida"] = this.ValidateUnidadMedida,
                ["clase"] = this.ValidateClase,
                ["familia"] = this.ValidateFamilia,
                ["rotatividad"] = this.ValidateRotatividad,
                ["ramo"] = this.ValidateRamo,
                ["situacion"] = this.ValidateSituacion,
                ["manejoIdentificador"] = this.ValidateManejoIdentificador,
                ["manejoFecha"] = this.ValidateManejoFecha,
                ["manejoDiasLiberacion"] = this.ValidateManejoDiasLiberacion,
                ["diasLiberacion"] = this.ValidateDiasLiberacion,
                ["diasDuracion"] = this.ValidateDiasDuracion,
                ["diasValidez"] = this.ValidateDiasValidez,
                ["modalidadIngresoLote"] = this.ValidateModalidadLote,
                ["stockMinimo"] = this.ValidateNumber,
                ["stockMaximo"] = this.ValidateNumber,
                ["pesoBruto"] = this.ValidatePesoBruto,
                ["pesoNeto"] = this.ValidateDecimal,
                ["altura"] = this.ValidateDecimal,
                ["ancho"] = this.ValidateDecimal,
                ["profundidad"] = this.ValidateDecimal,
                ["volumenCC"] = this.ValidateVolumenCC,
                ["unidadBultos"] = this.ValidateUnidadBultos,
                ["unidadDistribucion"] = this.ValidateUnidadDistribucion,
                ["mercadologico"] = this.ValidateMercadologico,
                ["reducida"] = this.ValidateReducida,
                ["productoEmpresaRef"] = this.ValidateCodigoProductoEmpresa,
                ["NCM"] = this.ValidateNCM,
                ["ajusteInventario"] = this.ValidateDecimal,
                ["display"] = this.ValidateDisplay,
                ["subBulto"] = this.ValidateShort,
                ["ultimoCosto"] = this.ValidateDecimal,
                ["precioVenta"] = this.ValidateDecimal,
                ["exclusivo"] = this.ValidateExclusivo,
                ["ayudaColector"] = this.ValidateAyudaColector,
                ["componente1"] = this.ValidateComponente1,
                ["componente2"] = this.ValidateComponente2,
                ["anexo1"] = this.ValidateAnexo,
                ["anexo2"] = this.ValidateAnexo,
                ["anexo3"] = this.ValidateAnexo,
                ["anexo4"] = this.ValidateAnexo,
                ["anexo5"] = this.ValidateAnexo5,
                ["aceptaDecimales"] = this.ValidateAceptaDecimales,
                ["grupoConsulta"] = this.ValidateGrupoConsulta,
                ["codigoBase"] = this.ValidateCodigoBase,
                ["talle"] = this.ValidateTalle,
                ["color"] = this.ValidateColor,
                ["temporada"] = this.ValidateTemporada,
                ["categoria1"] = this.ValidateCategoria,
                ["categoria2"] = this.ValidateCategoria,
                ["ventanaLiberacion"] = this.ValidateVentanaLiberacion,
            };
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly && form.GetField("empresa").IsValidated)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
                OnSuccess = this.ValidateCodigoEmpresa_OnSucess,
                OnFailure = this.ValidateCodigoEmpresa_Failure,
            };
        }

        public virtual void ValidateCodigoEmpresa_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Any(x => x.Id == "limpiarField") && parameters.FirstOrDefault(x => x.Id == "limpiarField").Value.Equals("true"))
                form.GetField("codigo").Value = string.Empty;

        }
        public virtual void ValidateCodigoEmpresa_Failure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("codigo").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateCodigo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new CaracteresAdmitidosCodigoProductoValidationRule(_uow, field.Value),
                    new IdProductoNoExistenteValidationRule(_uow,field.Value, form.GetField("empresa").Value),
                },
                Dependencies = { "empresa" }
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value, 65)
                    },
                Dependencies = { "codigo" }
            };

        }

        public virtual FormValidationGroup ValidateUnidadMedida(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                        {
                            new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value, 10)
                    },

            };
        }

        public virtual FormValidationGroup ValidateClase(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "codigo", "empresa" },
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 2),
                    new CambioDeClaseValidationRule(_uow, field.Value, form.GetField("codigo").Value, form.GetField("empresa").Value)
                },
                OnSuccess = this.ValidateClase_OnSucess
            };
        }

        public virtual void ValidateClase_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.RemoveAll(p => p.Id == "SHOW_CONFIRMACION_CAMBIO_CLASE");

            var cdProducto = form.GetField("codigo").Value;
            var empresa = int.Parse(form.GetField("empresa").Value);
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cdProducto, false);

            var ubicacionesPermitidas = new List<short>()
            {
                AreaUbicacionDb.PuertaEmbarque,
                AreaUbicacionDb.Transferencia
            };

            if (producto != null && producto.CodigoClase != field.Value)
            {
                if (_uow.StockRepository.AnyStockUbicacion(producto.Codigo, producto.CodigoEmpresa))
                {
                    List<string> ubicacionesConStock = _uow.StockRepository.GetUbicacionesConOSinStock(producto.Codigo, producto.CodigoEmpresa);

                    foreach (var ubicacion in ubicacionesConStock)
                    {
                        if (ubicacionesPermitidas.Contains(_uow.UbicacionRepository.GetUbicacion(ubicacion).IdUbicacionArea) &&
                            !parameters.Any(x => x.Id == "SHOW_CONFIRMACION_CAMBIO_CLASE"))
                        {
                            parameters.Add(new ComponentParameter(
                                "SHOW_CONFIRMACION_CAMBIO_CLASE",
                                "true"
                            ));

                            return;
                        }
                    }
                }
            }
        }

        public virtual FormValidationGroup ValidateFamilia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                       new NonNullValidationRule(field.Value),
                    },
            };

        }

        public virtual FormValidationGroup ValidateRotatividad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 2)
                    },

            };
        }
        public virtual FormValidationGroup ValidateRamo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 2)
                    },

            };
        }
        public virtual FormValidationGroup ValidateSituacion(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value, 3)
                    },

            };
        }

        public virtual FormValidationGroup ValidateManejoIdentificador(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (field.ReadOnly)
                return null;

            var reglas = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
            };

            if (!string.IsNullOrEmpty(form.GetField("empresa").Value) && !string.IsNullOrEmpty(form.GetField("codigo").Value))
                reglas.Add(new CambioDeIndentificadorValidationRule(_uow, field.Value, form.GetField("codigo").Value, form.GetField("empresa").Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = reglas,
                Dependencies = { "codigo", "empresa", "aceptaDecimales" },
                OnSuccess = this.ValidateManejoIdentificador_OnSucess,
                OnFailure = this.ValidateManejoIdentificador_Failure,
            };

        }
        public virtual void ValidateManejoIdentificador_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.RemoveAll(p => p.Id == "SHOW_CONFIRMACION_CAMBIO_MANEJO_ID");

            var tpManejo = form.GetField("manejoIdentificador").Value;

            var selectIngresoLote = form.GetField("modalidadIngresoLote");
            selectIngresoLote.Options = new List<SelectOption>();

            selectIngresoLote.Options.Add(new SelectOption(ModalidadIngresoLoteDb.Normal, "REG009_frm_opt_ModalidadLote_Normal"));

            if (tpManejo == ManejoIdentificadorDb.Lote)
            {
                selectIngresoLote.Options.Add(new SelectOption(ModalidadIngresoLoteDb.Vencimiento, "REG009_frm_opt_ModalidadLote_Vencimiento"));
                selectIngresoLote.Options.Add(new SelectOption(ModalidadIngresoLoteDb.Agenda, "REG009_frm_opt_ModalidadLote_Agenda"));
                selectIngresoLote.Options.Add(new SelectOption(ModalidadIngresoLoteDb.Documento, "REG009_frm_opt_ModalidadLote_Documento"));
                selectIngresoLote.Options.Add(new SelectOption(ModalidadIngresoLoteDb.VencimientoYYYYMM, "REG009_frm_opt_ModalidadLote_VencimientoYYYYMM"));
            }

            if (selectIngresoLote.Options.Count == 1)
                selectIngresoLote.Value = selectIngresoLote.Options.FirstOrDefault().Value;

            if (tpManejo == ManejoIdentificadorDb.Producto)
            {
                var cdProducto = form.GetField("codigo").Value;
                var empresa = int.Parse(form.GetField("empresa").Value);
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cdProducto, false);

                if (producto != null && producto.ManejoIdentificadorId != tpManejo)
                {
                    if (_uow.StockRepository.AnyStockUbicacion(producto.Codigo, producto.CodigoEmpresa))
                    {
                        if (!parameters.Any(x => x.Id == "SHOW_CONFIRMACION_CAMBIO_MANEJO_ID"))
                        {
                            parameters.Add(new ComponentParameter(
                                "SHOW_CONFIRMACION_CAMBIO_MANEJO_ID",
                                "true"
                            ));
                        }
                    }
                }
            }
        }
        public virtual void ValidateManejoIdentificador_Failure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("modalidadIngresoLote").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateManejoFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "codigo", "empresa" },
                Rules = new List<IValidationRule> {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,1),
                        new CambioDeManejoFechaValidationRule(_uow, field.Value, form.GetField("codigo").Value, form.GetField("empresa").Value),
                        new TipoManejoFechaModalidadLoteValidationRule(_uow, field.Value, form.GetField("modalidadIngresoLote").Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateDiasLiberacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NumeroEnteroValidationRule(field.Value),
                        new NumeroEnteroMayorQueValidationRule(int.Parse(field.Value), -1)
                    },

            };
        }

        public virtual FormValidationGroup ValidateDiasDuracion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NumeroEnteroValidationRule(field.Value),
                        new NumeroEnteroMayorQueValidationRule(int.Parse(field.Value), -1)
                    },

            };
        }

        public virtual FormValidationGroup ValidateDiasValidez(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NumeroEnteroValidationRule(field.Value),
                        new NumeroEnteroMayorQueValidationRule(int.Parse(field.Value), -1)
                    },

            };
        }

        public virtual FormValidationGroup ValidateModalidadLote(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ModalidadIngresoLoteValidationRule(_uow, field.Value, form.GetField("manejoIdentificador").Value)
                },
                Dependencies = { "manejoIdentificador" }
            };
        }

        public virtual FormValidationGroup ValidateNumber(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                    new NumeroEnteroValidationRule(field.Value),
                        new NumeroEnteroMayorQueValidationRule(int.Parse(field.Value),-1),
                    },

            };
        }

        public virtual FormValidationGroup ValidateShort(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new PositiveShortValidationRule(field.Value),
                    },

            };
        }

        public virtual FormValidationGroup ValidateDecimal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new PositiveDecimalValidationRule(this._culture,field.Value),
                    },

            };
        }

        public virtual FormValidationGroup ValidatePesoBruto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            string empresa = form.GetField("empresa").Value;

            List<IValidationRule> reglas = new List<IValidationRule>
            {
                new PesoVolumenRequeridoValidationRule(this._uow, field.Value, empresa)
            };

            if (!string.IsNullOrEmpty(field.Value))
            {
                reglas.Add(new PositiveDecimalValidationRule(this._culture, field.Value));

                if (!string.IsNullOrEmpty(form.GetField("pesoNeto").Value))
                    reglas.Add(new DecimalGreaterThanValidationRule(this._culture, field.Value, decimal.Parse(form.GetField("pesoNeto").Value, this._culture)));
            }

            return new FormValidationGroup
            {

                Rules = reglas,
                Dependencies = { "pesoNeto" }

            };
        }

        public virtual FormValidationGroup ValidateVolumenCC(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            var empresa = form.GetField("empresa").Value;

            var reglas = new List<IValidationRule>
            {
                new PesoVolumenRequeridoValidationRule(this._uow, field.Value, empresa, true)
            };

            if (!string.IsNullOrEmpty(field.Value))
            {
                reglas.Add(new PositiveDecimalValidationRule(this._culture, field.Value));
                reglas.Add(new DecimalLengthWithPresicionValidationRule(field.Value, 14, 4, this._culture, aceptaDecimales: true));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = reglas
            };
        }

        public virtual FormValidationGroup ValidateDecimalRequired(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new PositiveDecimalValidationRule(this._culture,field.Value),
                    },

            };
        }

        public virtual FormValidationGroup ValidateNumberRequired(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new NumeroEnteroValidationRule(field.Value),
                        new NumeroEnteroMayorQueValidationRule(int.Parse(field.Value), -1)
                    },

            };
        }

        public virtual FormValidationGroup ValidateAnexo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 200)
                    },

            };
        }

        public virtual FormValidationGroup ValidateAnexo5(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 18)
                    },

            };
        }

        public virtual FormValidationGroup ValidateAyudaColector(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 200)
                    },

            };
        }

        public virtual FormValidationGroup ValidateComponente1(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 200),
                        new CodigoFacturacionExistenteValidationRule(_uow,field.Value),
                    },

            };
        }

        public virtual FormValidationGroup ValidateComponente2(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 200),
                        new ExisteDominioFacturacionValidationRule(_uow,field.Value),
                    },

            };
        }

        public virtual FormValidationGroup ValidateExclusivo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new PositiveShortValidationRule(field.Value)
                    },

            };
        }

        public virtual FormValidationGroup ValidateReducida(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 20)
                    },

            };
        }

        public virtual FormValidationGroup ValidateMercadologico(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            var reglas = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value,40),
                new StringSoloUpperValidationRule(field.Value),
            };

            if (!string.IsNullOrEmpty(form.GetField("empresa").Value) && !string.IsNullOrEmpty(form.GetField("codigo").Value))
                reglas.Add(new NoExisteMercadologicoProductoEmpresaValidationRule(_uow, field.Value, form.GetField("empresa").Value, form.GetField("empresa").Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = reglas,
                Dependencies = { "codigo", "empresa" }

            };
        }

        public virtual FormValidationGroup ValidateDisplay(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 30)
                    },

            };
        }

        public virtual FormValidationGroup ValidateNCM(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 30)
                    },

            };
        }

        public virtual FormValidationGroup ValidateCodigoProductoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            var reglas = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 30)
            };

            if (!string.IsNullOrEmpty(form.GetField("empresa").Value) && !string.IsNullOrEmpty(form.GetField("codigo").Value))
                reglas.Add(new NoExisteReferenciaProductoEmpresaValidationRule(_uow, field.Value, form.GetField("empresa").Value, form.GetField("codigo").Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = reglas,
                Dependencies = { "codigo", "empresa" }
            };
        }

        public virtual FormValidationGroup ValidateManejoDiasLiberacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 6)
                },
                OnSuccess = this.ValidateManejoDiasLiberacion_OnSuccess,
                OnFailure = this.ValidateManejoDiasLiberacion_OnFailure,
            };
        }

        public virtual void ValidateManejoDiasLiberacion_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                Validez validezDias = _uow.ValidezRepository.GetValidez(field.Value);

                form.GetField("diasLiberacion").Value = validezDias.DiasValidezLibracion.ToString();
                form.GetField("diasDuracion").Value = validezDias.DiasDuracion.ToString();
                form.GetField("diasValidez").Value = validezDias.DiasValidez.ToString();
            }

        }
        public virtual void ValidateManejoDiasLiberacion_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("diasLiberacion").Value = string.Empty;
            form.GetField("diasDuracion").Value = string.Empty;
            form.GetField("diasValidez").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateAceptaDecimales(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            var manejoIdentificador = form.GetField("manejoIdentificador").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 1),
                    new ManejoIdentificadorSerieAceptaDecimalesValidationRule(field.Value, manejoIdentificador)
                }
            };
        }

        public virtual FormValidationGroup ValidateCodigoBase(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 40)
                    },

            };
        }

        public virtual FormValidationGroup ValidateTalle(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 40)
                    },

            };
        }

        public virtual FormValidationGroup ValidateColor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 40)
                    },

            };
        }

        public virtual FormValidationGroup ValidateTemporada(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 40)
                    },

            };
        }

        public virtual FormValidationGroup ValidateCategoria(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 40)
                    },

            };
        }


        public virtual FormValidationGroup ValidateGrupoConsulta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 50)
                    },

            };
        }

        public virtual FormValidationGroup ValidateUnidadBultos(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            var manejoIdentificador = _productoMapper.MapManejoIdentificador(form.GetField("manejoIdentificador").Value);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new PositiveDecimalValidationRule(this._culture, field.Value, false),
                        new ManejoIdentificadorSerieCantidadValidationRule(field.Value, manejoIdentificador, _culture)
                    },

            };
        }

        public virtual FormValidationGroup ValidateUnidadDistribucion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            var manejoIdentificador = _productoMapper.MapManejoIdentificador(form.GetField("manejoIdentificador").Value);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new PositiveDecimalValidationRule(this._culture, field.Value, false),
                        new ManejoIdentificadorSerieCantidadValidationRule(field.Value, manejoIdentificador, _culture)
                    },

            };
        }

        public virtual FormValidationGroup ValidateVentanaLiberacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 20),
                    },
            };

        }
    }
}