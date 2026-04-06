using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Services
{
    public class GruposService
    {
        public static List<SelectOption> GetOptionsParam(IUnitOfWork uow, string codigoParametro, string tipoParametro, string searchValue, int userId)
        {
            var opciones = new List<SelectOption>();

            if (tipoParametro == "SEARCH" && string.IsNullOrEmpty(searchValue))
                return opciones;

            switch (codigoParametro)
            {
                case "CD_EMPRESA":

                    var empresasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(searchValue, userId);
                    foreach (var emp in empresasUsuario)
                    {
                        opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
                    }

                    break;
                case "CD_UNIDADE_MEDIDA":

                    var unidades = uow.UnidadMedidaRepository.GetByNombreOrCodePartial(searchValue);
                    foreach (var unidad in unidades)
                    {
                        opciones.Add(new SelectOption(unidad.Id.ToString(), $"{unidad.Id} - {unidad.Descripcion}"));
                    }

                    break;
                case "CD_CLASSE":

                    var clases = uow.ClaseRepository.GetClases();
                    foreach (var clase in clases)
                    {
                        opciones.Add(new SelectOption(clase.Id.ToString(), $"{clase.Id} - { clase.Descripcion}")); ;
                    }

                    break;
                case "CD_FAMILIA_PRODUTO":

                    var familias = uow.ProductoFamiliaRepository.GetByNombreOrCodePartial(searchValue);
                    foreach (var familia in familias)
                    {
                        opciones.Add(new SelectOption(familia.Id.ToString(), $"{familia.Id} - {familia.Descripcion}"));
                    }

                    break;
                case "CD_RAMO_PRODUTO":

                    var ramos = uow.ProductoRamoRepository.GetProductoRamos();
                    foreach (var ramo in ramos)
                    {
                        opciones.Add(new SelectOption(ramo.Id.ToString(), $"{ramo.Id} - { ramo.Descripcion}")); ;
                    }

                    break;
                case "CD_ROTATIVIDADE":

                    var rotatividades = uow.ProductoRotatividadRepository.GetProductoRotatividades();
                    foreach (var rotatividad in rotatividades)
                    {
                        opciones.Add(new SelectOption(rotatividad.Id.ToString(), $"{rotatividad.Id} - { rotatividad.Descripcion}")); ;
                    }

                    break;
                case "CD_SITUACAO":

                    opciones.Add(new SelectOption(SituacionDb.Activo.ToString(), "REG009_frm_opt_Situacion_Activo"));
                    opciones.Add(new SelectOption(SituacionDb.Inactivo.ToString(), "REG009_frm_opt_Situacion_Inactivo"));

                    break;
                case "ID_MANEJO_IDENTIFICADOR":

                    opciones.Add(new SelectOption(ManejoIdentificadorDb.Lote, "REG009_frm_opt_ManejoIdentificador_Lote"));
                    opciones.Add(new SelectOption(ManejoIdentificadorDb.Serie, "REG009_frm_opt_ManejoIdentificador_Serie"));
                    opciones.Add(new SelectOption(ManejoIdentificadorDb.Producto, "REG009_frm_opt_ManejoIdentificador_Producto"));

                    break;
                case "ND_MODALIDAD_INGRESO_LOTE":

                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Normal, "REG009_frm_opt_ModalidadLote_Normal"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Vencimiento, "REG009_frm_opt_ModalidadLote_Vencimiento"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Agenda, "REG009_frm_opt_ModalidadLote_Agenda"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Documento, "REG009_frm_opt_ModalidadLote_Documento"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.VencimientoYYYYMM, "REG009_frm_opt_ModalidadLote_VencimientoYYYYMM"));

                    break;
                case "TP_MANEJO_FECHA":

                    opciones.Add(new SelectOption(ManejoFechaProductoDb.Duradero, "REG009_frm_opt_ManejoFecha_Duradero"));
                    opciones.Add(new SelectOption(ManejoFechaProductoDb.Fifo, "REG009_frm_opt_ManejoFecha_Fifo"));
                    opciones.Add(new SelectOption(ManejoFechaProductoDb.Expirable, "REG009_frm_opt_ManejoFecha_Fefo"));

                    break;
                case "CD_GRUPO_CONSULTA":

                    var gruposAsignados = uow.GrupoConsultaRepository.GetGrupoConsultaAsignados(userId);
                    foreach (var grupo in gruposAsignados)
                    {
                        opciones.Add(new SelectOption(grupo.Id, $"{grupo.Id} - {grupo.Descripcion}"));
                    }

                    break;
                case "CD_NAM":
                    var ncms = uow.NcmRepository.GetByNombreOrCodePartial(searchValue);

                    foreach (var ncm in ncms)
                    {
                        opciones.Add(new SelectOption(ncm.Id.ToString(), $"{ncm.Id} - {ncm.Descripcion}"));
                    }
                    break;
                default:
                    //FL_ACEPTA_DECIMALES
                    opciones = new List<SelectOption>
                    {
                        new SelectOption("S", "General_form_select_SI"),
                        new SelectOption("N", "General_form_select_NO"),
                        new SelectOption(" ", "General_form_select_Todos"),
                    };
                    break;
            }
            return opciones;
        }
    }
}
