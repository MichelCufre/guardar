using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General.Configuracion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Recepcion
{
    public class MantenimientoAgregarLogicaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;
        protected readonly UbicacionConfiguracion _ubicacionConfiguracion;
        protected readonly List<ComponentParameter> _parameters;

        public MantenimientoAgregarLogicaGridValidationModule(IUnitOfWork uow, IIdentityService securityService, List<ComponentParameter> parameters)
        {
            this._uow = uow;
            this._securityService = securityService;
            this._parameters = parameters;
            this._ubicacionConfiguracion = this._uow.UbicacionRepository.GetUbicacionConfiguracion();

            this.Schema = new GridValidationSchema
            {
                ["VL_ALM_PARAMETRO_DEFAULT"] = this.GridValidateParametro,
            };
        }

        public virtual GridValidationGroup GridValidateParametro(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string parametro = row.GetCell("NM_ALM_PARAMETRO").Value;

            var rules = new List<IValidationRule>();
            var dependecies = new List<string>();

            if (cell.Value != "")
            {
                switch (parametro)
                {
                    case "ALTURA_DESDE":

                        string alturaHasta = this._parameters.FirstOrDefault(p => p.Id == "ALTURA_HASTA")?.Value;
                        ParametroDesdeValidationRule validacionParametrosAlturaDesde = new ParametroDesdeValidationRule(alturaHasta, cell.Value);

                        if (this._parameters.Any(p => p.Id == "ALTURA_HASTA"))
                        {
                            rules.Add(new AlturaDesdeUbicacionValidationRule(alturaHasta, cell.Value));
                            rules.Remove(validacionParametrosAlturaDesde);
                        }
                        else
                        {
                            rules.Add(validacionParametrosAlturaDesde);
                        }

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.AlturaLargo));
                        break;

                    case "ALTURA_HASTA":

                        string alturaDesde = this._parameters.FirstOrDefault(p => p.Id == "ALTURA_DESDE")?.Value;
                        ParametroHastaValidationRule validacionParametrosAlturaHasta = new ParametroHastaValidationRule(alturaDesde, cell.Value);

                        if (this._parameters.Any(p => p.Id == "ALTURA_DESDE"))
                        {
                            rules.Add(new AlturaHastaUbicacionValidationRule(cell.Value, alturaDesde));
                            rules.Remove(validacionParametrosAlturaHasta);
                        }
                        else
                        {
                            rules.Add(validacionParametrosAlturaHasta);
                        }

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.AlturaLargo));
                        break;

                    case "AREA":

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, 2));
                        rules.Add(new IdUbicacionAreaNoExistenteValidationRule(this._uow, cell.Value));
                        rules.Add(new IdUbicacionAreaNoAlmacenableValidationRule(this._uow, cell.Value));
                        break;

                    case "BLOQUE_DESDE":

                        rules.Add(new StringMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.BloqueLargo));
                        rules.Add(new AnyBloqueValidationRule(this._uow, cell.Value.ToUpper()));
                        string bloqueHasta = this._parameters.FirstOrDefault(p => p.Id == "BLOQUE_HASTA")?.Value;
                        ParametroDesdeValidationRule validacionParametrosBloqueDesde = new ParametroDesdeValidationRule(bloqueHasta, cell.Value);

                        if (_ubicacionConfiguracion.BloqueNumerico)
                        {
                            rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.BloqueLargo));

                            if (this._parameters.Any(p => p.Id == "BLOQUE_HASTA"))
                            {
                                rules.Add(new BloqueDesdeUbicacionValidationRule(bloqueHasta, cell.Value));
                                rules.Remove(validacionParametrosBloqueDesde);
                            }
                            else
                            {
                                rules.Add(validacionParametrosBloqueDesde);
                            }
                        }
                        else
                        {
                            rules.Add(new StringSoloLetrasValidationRule(cell.Value.ToUpper()));

                            if (this._parameters.Any(p => p.Id == "BLOQUE_HASTA"))
                            {
                                bloqueHasta = this._parameters.FirstOrDefault(p => p.Id == "BLOQUE_HASTA").Value;
                                rules.Add(new BloqueDesdeCaracterUbicacionValidationRule(bloqueHasta, cell.Value));
                                rules.Remove(validacionParametrosBloqueDesde);
                            }
                            else
                            {
                                rules.Add(validacionParametrosBloqueDesde);
                            }
                        }
                        break;

                    case "BLOQUE_HASTA":

                        string bloqueDesde = this._parameters.FirstOrDefault(p => p.Id == "BLOQUE_DESDE")?.Value;

                        ParametroHastaValidationRule validacionParametrosBloqueHasta = new ParametroHastaValidationRule(bloqueDesde, cell.Value);

                        rules.Add(new StringMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.BloqueLargo));
                        rules.Add(new AnyBloqueValidationRule(this._uow, cell.Value.ToUpper()));

                        if (_ubicacionConfiguracion.BloqueNumerico)
                        {
                            rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.BloqueLargo));

                            if (this._parameters.Any(p => p.Id == "BLOQUE_DESDE"))
                            {
                                rules.Add(new BloqueHastaUbicacionValidationRule(cell.Value, bloqueDesde));
                                rules.Remove(validacionParametrosBloqueHasta);
                            }
                            else
                            {
                                rules.Add(validacionParametrosBloqueHasta);
                            }
                        }
                        else
                        {
                            rules.Add(new StringSoloLetrasValidationRule(cell.Value.ToUpper()));

                            if (this._parameters.Any(p => p.Id == "BLOQUE_DESDE"))
                            {
                                rules.Add(new BloqueHastaCaracterUbicacionValidationRule(bloqueDesde, cell.Value));
                                rules.Remove(validacionParametrosBloqueHasta);
                            }
                            else
                            {
                                rules.Add(validacionParametrosBloqueHasta);
                            }
                        }
                        break;

                    case "CALLE_DESDE":

                        rules.Add(new StringMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.CalleLargo));
                        string calleHasta = this._parameters.FirstOrDefault(p => p.Id == "CALLE_HASTA")?.Value;

                        ParametroDesdeValidationRule validacionParametrosCalleDesde = new ParametroDesdeValidationRule(calleHasta, cell.Value);

                        if (_ubicacionConfiguracion.CalleNumerico)
                        {
                            rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.CalleLargo));

                            if (this._parameters.Any(p => p.Id == "CALLE_HASTA"))
                            {
                                rules.Add(new CalleDesdeUbicacionValidationRule(cell.Value, calleHasta));
                                rules.Remove(validacionParametrosCalleDesde);
                            }
                            else
                            {
                                rules.Add(validacionParametrosCalleDesde);
                            }
                        }
                        else
                        {
                            rules.Add(new StringSoloLetrasValidationRule(cell.Value.ToUpper()));

                            if (this._parameters.Any(p => p.Id == "CALLE_HASTA"))
                            {
                                rules.Add(new CalleDesdeCaracterUbicacionValidationRule(cell.Value, calleHasta));
                                rules.Remove(validacionParametrosCalleDesde);
                            }
                            else
                            {
                                rules.Add(validacionParametrosCalleDesde);
                            }
                        }
                        break;

                    case "CALLE_HASTA":

                        rules.Add(new StringMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.CalleLargo));
                        string calleDesde = this._parameters.FirstOrDefault(p => p.Id == "CALLE_DESDE")?.Value;
                        ParametroHastaValidationRule validacionParametrosCalleHasta = new ParametroHastaValidationRule(calleDesde, cell.Value);

                        if (_ubicacionConfiguracion.CalleNumerico)
                        {
                            rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.CalleLargo));

                            if (this._parameters.Any(p => p.Id == "CALLE_DESDE"))
                            {
                                rules.Add(new CalleHastaUbicacionValidationRule(cell.Value, calleDesde));
                                rules.Remove(validacionParametrosCalleHasta);
                            }
                            else
                            {
                                rules.Add(validacionParametrosCalleHasta);
                            }
                        }
                        else
                        {
                            rules.Add(new StringSoloLetrasValidationRule(cell.Value.ToUpper()));

                            if (this._parameters.Any(p => p.Id == "CALLE_DESDE"))
                            {
                                rules.Add(new CalleHastaCaracterUbicacionValidationRule(calleDesde, cell.Value));
                                rules.Remove(validacionParametrosCalleHasta);
                            }
                            else
                            {
                                rules.Add(validacionParametrosCalleHasta);
                            }
                        }
                        break;

                    case "COLUMNA_DESDE":

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.ColumnaLargo));

                        string columnaHasta = this._parameters.FirstOrDefault(p => p.Id == "COLUMNA_HASTA")?.Value;
                        ParametroDesdeValidationRule validacionParametrosColumnaDesde = new ParametroDesdeValidationRule(columnaHasta, cell.Value);


                        if (this._parameters.Any(p => p.Id == "COLUMNA_HASTA"))
                        {
                            rules.Add(new ColumnaDesdeUbicacionValidationRule(cell.Value, columnaHasta));
                            rules.Remove(validacionParametrosColumnaDesde);
                        }
                        else
                        {
                            rules.Add(validacionParametrosColumnaDesde);
                        }
                        break;

                    case "COLUMNA_HASTA":

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.ColumnaLargo));
                        string columnaDesde = this._parameters.FirstOrDefault(p => p.Id == "COLUMNA_DESDE")?.Value;

                        ParametroHastaValidationRule validacionParametrosColumnaHasta = new ParametroHastaValidationRule(columnaDesde, cell.Value);

                        if (this._parameters.Any(p => p.Id == "COLUMNA_DESDE"))
                        {
                            rules.Add(new ColumnaHastaUbicacionValidationRule(cell.Value, columnaDesde));
                            rules.Remove(validacionParametrosColumnaHasta);
                        }
                        else
                        {
                            rules.Add(validacionParametrosColumnaHasta);
                        }
                        break;

                    case "CONTROL_ACCESO":

                        rules.Add(new StringMaxLengthValidationRule(cell.Value, 20));
                        rules.Add(new ExisteControlAccesoValidationRule(this._uow, cell.Value));
                        break;

                    case "EMPRESA":

                        rules.Add(new PositiveIntValidationRule(cell.Value));
                        rules.Add(new ExisteEmpresaValidationRule(this._uow, cell.Value));
                        break;

                    case "FAMILIA":

                        rules.Add(new PositiveIntValidationRule(cell.Value));
                        rules.Add(new IdProductoFamiliaNoExistenteValidationRule(_uow, cell.Value));
                        break;

                    case "ROTATIVIDAD":

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, 2));
                        rules.Add(new ExisteCodigoRotatividadValidationRule(this._uow, cell.Value));
                        break;

                    case "TIPO_UBICACION":

                        rules.Add(new PositiveShortNumberMaxLengthValidationRule(cell.Value, 2));
                        rules.Add(new IdUbicacionTipoNoExistenteValidationRule(this._uow, cell.Value));
                        break;

                    case "UBIC_INICIO":

                        rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                        rules.Add(new ExisteUbicacionValidationRule(this._uow, cell.Value));
                        break;

                    case "MAX_CANTIDAD_USOS":

                        rules.Add(new PositiveShortValidationRule(cell.Value));
                        break;

                    case "ZONA_UBICACION":

                        rules.Add(new IdZonaUbicacionValidationRule(this._uow, cell.Value));
                        break;

                    case "PALLET":

                        rules.Add(new PositiveShortValidationRule(cell.Value));
                        rules.Add(new ExisteCodigoPalletValidationRule(this._uow, cell.Value));
                        break;

                    case "PORCENTAJE_OCUPACION":

                        rules.Add(new PositiveShortValidationRule(cell.Value));
                        rules.Add(new PositiveShortLessThanValidationRule(cell.Value, 100));
                        break;
                    case "PORCENTAJE_FORZADO":
                        string modalidadReabastecimiento = this._parameters.FirstOrDefault(p => p.Id == "MODALIDAD_REABASTECIMIENTO")?.Value;

                        rules.Add(new PositiveIntMayorACeroValidationRule(cell.Value));
                        rules.Add(new ModalidadReabastecimientoForzadoValidationRule(modalidadReabastecimiento, cell.Value));
                        break;

                    case "PADRON":

                        rules.Add(new PositiveIntMayorACeroValidationRule(cell.Value));
                        break;

                    case "TIPO_PICKING":
                        rules.Add(new TipoPickingValidationRule(this._uow, cell.Value));
                        break;

                    case "PORCENTAJE_PARCIALIZACION":

                        rules.Add(new PositiveIntLessThanValidationRule(cell.Value, 100));
                        break;

                    case "VENTANA_FEFO":

                        rules.Add(new PositiveIntValidationRule(cell.Value, true));
                        break;

                    case "UBIC_BAJAS_ALTAS":
                    case "UBIC_MULTIPRODUCTO":
                    case "UBIC_MULTILOTE":
                    case "RESPETA_FIFO":
                    case "PRODUCTOS_COINCIDENTES":
                    case "LOTES_COINCIDENTES":
                    case "IGNORAR_VENCIMIENTO_STOCK":

                        rules.Add(new StringSoloUpperValidationRule(cell.Value));
                        rules.Add(new SorNValidationRule(cell.Value));
                        break;
                }
            }
            else
            {
                switch (parametro)
                {
                    case "MODALIDAD_REABASTECIMIENTO":
                        string porcentajeForzado = this._parameters.FirstOrDefault(p => p.Id == "PORCENTAJE_FORZADO")?.Value;

                        rules.Add(new NonNullValidationRule(cell.Value));
                        rules.Add(new ModalidadReabastecimientoValidationRule(cell.Value));
                        rules.Add(new ModalidadReabastecimientoForzadoValidationRule(cell.Value, porcentajeForzado));
                        break;
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
