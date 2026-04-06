using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.Services
{
    public class RecepcionService : IRecepcionService
    {
        public virtual List<SelectOption> GetAllValues(IUnitOfWork uow, string codigoParametro, string searchValue, int userId)
        {
            switch (codigoParametro.ToUpper())
            {
                case "CONTROL_ACCESO":

                    List<SelectOption> opcionesControlAcceso = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<ControlAcceso> controlesAcceso = uow.ZonaUbicacionRepository.GetControlAccesoByNombreOrCodePartial(searchValue);

                        foreach (var controlAcceso in controlesAcceso)
                        {
                            opcionesControlAcceso.Add(new SelectOption(controlAcceso.Id.ToString(), $"{controlAcceso.Id} - {controlAcceso.Descripcion}"));
                        }
                    }

                    return opcionesControlAcceso;

                case "EMPRESA":

                    List<SelectOption> opciones = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<Empresa> empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(searchValue, userId);

                        foreach (var emp in empresasAsignadasUsuario)
                        {
                            opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
                        }
                    }

                    return opciones;

                case "FAMILIA":

                    List<SelectOption> opcionesFamilias = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<ProductoFamilia> familias = uow.ProductoFamiliaRepository.GetByNombreOrCodePartial(searchValue);

                        foreach (var familia in familias)
                        {
                            opcionesFamilias.Add(new SelectOption(familia.Id.ToString(), $"{familia.Id} - {familia.Descripcion}"));
                        }
                    }

                    return opcionesFamilias;

                case "ROTATIVIDAD":

                    List<SelectOption> opcionesRotatividad = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<ProductoRotatividad> rotatividades = uow.ProductoRotatividadRepository.GetByNombreOrCodePartial(searchValue);

                        foreach (var rotatividad in rotatividades)
                        {
                            opcionesRotatividad.Add(new SelectOption(rotatividad.Id.ToString(), $"{rotatividad.Id} - {rotatividad.Descripcion}"));
                        }
                    }

                    return opcionesRotatividad;

                case "TIPO_UBICACION":

                    List<SelectOption> opcionesTipoUbicacion = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<UbicacionTipo> tipoUbicaciones = uow.UbicacionTipoRepository.GetByNombreOrCodePartial(searchValue);

                        foreach (var ubicacion in tipoUbicaciones)
                        {
                            opcionesTipoUbicacion.Add(new SelectOption(ubicacion.Id.ToString(), $"{ubicacion.Id} - {ubicacion.Descripcion}"));
                        }
                    }

                    return opcionesTipoUbicacion;

                case "UBIC_INICIO":

                    List<SelectOption> opcionesUbicacion = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<Ubicacion> ubicaciones = uow.UbicacionRepository.GetUbicacionesDispoCodigoPartial(searchValue);

                        foreach (var ubicacion in ubicaciones)
                        {
                            opcionesUbicacion.Add(new SelectOption(ubicacion.Id.ToString(), $"{ubicacion.Id}"));
                        }
                    }

                    return opcionesUbicacion;

                case "ZONA_UBICACION":

                    List<SelectOption> opcionesZonaUbicacion = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<ZonaUbicacion> zonaUbicaciones = uow.ZonaUbicacionRepository.GetByZonaUbicacionNombreOrCodePartial(searchValue);

                        foreach (var ubicacion in zonaUbicaciones)
                        {
                            opcionesZonaUbicacion.Add(new SelectOption(ubicacion.Id.ToString(), $"{ubicacion.Id} - {ubicacion.Descripcion}"));
                        }
                    }

                    return opcionesZonaUbicacion;

                case "PALLET":

                    List<SelectOption> opcionesPallet = new List<SelectOption>();

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        List<Pallet> pallets = uow.FacturacionRepository.GetPalletByNombreOrCodePartial(searchValue);

                        foreach (var pallet in pallets)
                        {
                            opcionesPallet.Add(new SelectOption(pallet.Id.ToString(), $"{pallet.Id} - {pallet.Descripcion}"));
                        }
                    }

                    return opcionesPallet;

                case "UBIC_BAJAS_ALTAS":

                    return new List<SelectOption>
                    {
                        new SelectOption("S", "REC275_form_select_Baja"),
                        new SelectOption("N", "REC275_form_select_Alta"),
                        new SelectOption(" ", "REC275_form_select_Todas"),
                    };

                case "UBIC_MULTIPRODUCTO":

                    return new List<SelectOption>
                    {
                        new SelectOption("S", "REC275_form_select_MultiProducto"),
                        new SelectOption("N", "REC275_form_select_MonoProducto"),
                        new SelectOption(" ",  "REC275_form_select_Todas"),
                    };

                case "UBIC_MULTILOTE":

                    return new List<SelectOption>
                    {
                        new SelectOption("S", "REC275_form_select_MultiLote"),
                        new SelectOption("N", "REC275_form_select_MonoLote"),
                        new SelectOption(" ", "REC275_form_select_Todas"),
                    };

                case "AREA":
                    List<SelectOption> opcionesArea = new List<SelectOption>();
                    List<UbicacionArea> areas = uow.UbicacionAreaRepository.GetUbicacionAreasPermiteAlmacenar();

                    foreach (var area in areas)
                    {
                        opcionesArea.Add(new SelectOption(area.Id.ToString(), area.Descripcion));
                    }
                    return opcionesArea;

                case "LOTES_COINCIDENTES":
                case "PRODUCTOS_COINCIDENTES":
                case "RESPETA_VENCIMIENTO":
                case "RESPETA_LOTE":
                case "IGNORAR_VENCIMIENTO_STOCK":
                case "RESPETA_FIFO":
                    return new List<SelectOption>
                    {
                        new SelectOption("S", "REC275_form_select_Si"),
                        new SelectOption("N", "REC275_form_select_No"),
                    };

                case "MODALIDAD_REABASTECIMIENTO":

                    List<SelectOption> opcionesModalidadReabastecimiento = new List<SelectOption>();

                    List<string> modalidades = EstrategiaAlmacenajeReabastecimientoDb.GetModalidadesReabastecimiento();

                    foreach (var modalidad in modalidades)
                    {
                        opcionesModalidadReabastecimiento.Add(new SelectOption(modalidad, modalidad));
                    }

                    return opcionesModalidadReabastecimiento;

                case "TIPO_PICKING":
                    List<SelectOption> opcionesTiposPicking = new List<SelectOption>();

                    var dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.TipoPicking);

                    foreach (var dominio in dominios)
                    {
                        opcionesTiposPicking.Add(new SelectOption(dominio.Valor, dominio.Descripcion));
                    }
                    return opcionesTiposPicking;

                default:
                    return new List<SelectOption>
                    {
                        new SelectOption("S", "REC275_form_select_Si"),
                        new SelectOption("N", "REC275_form_select_No"),
                        new SelectOption(" ", "REC275_form_select_Todas"),
                    };
            }
        }
    }
}
