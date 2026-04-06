using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class EgresoMapper : Mapper, IEgresoMapper
    {
        //Entrada
        public virtual List<Camion> Map(EgresoRequest request)
        {
            var egresos = new List<Camion>();

            foreach (var c in request.Camiones)
            {
                var model = CreateCamion(c);
                model.Id = -1;
                model.IdExterno = c.IdExterno;
                model.Ruta = c.Ruta;
                model.Puerta = c.Puerta;
                model.Predio = c.Predio;
                model.Empresa = c.Empresa;
                model.EmpresaExterna = request.Empresa;
                model.Descripcion = c.Descripcion;
                model.Documento = c.Documento;
                model.Vehiculo = c.CodigoVehiculo;
                model.Matricula = c.Matricula;
                model.Transportista = (c.Transportista ?? -1);
                model.FechaProgramado = c.ProgramacionFecha;
                model.Estado = CamionEstado.AguardandoCarga;
                model.TipoArmadoEgreso = TipoArmadoEgreso.Manual;
                model.ArmadoEgreso = TipoArmadoEgreso.ArmadoManual;
                model.FechaCreacion = DateTime.Now;
                model.IgnorarCargasInexistentes = c.IgnorarCargasInexistentes;

                MapDetalle(model, c);
                egresos.Add(model);
            }

            return egresos;
        }

        public virtual void MapDetalle(Camion model, CamionRequest c)
        {
            if (c.Detalles != null)
            {
                if (c.Detalles.Pedidos != null)
                {
                    foreach (var ped in c.Detalles.Pedidos)
                    {
                        model.SetPedidoCamion(ped.NroPedido, ped.CodigoAgente, ped.TipoAgente, ped.Empresa);
                    }
                }

                if (c.Detalles.Cargas != null)
                {
                    foreach (var car in c.Detalles.Cargas)
                    {
                        model.SetCargaCamion(car.Carga, car.CodigoAgente, car.TipoAgente, car.Empresa);
                    }
                }

                if (c.Detalles.Contenedores != null)
                {
                    foreach (var cont in c.Detalles.Contenedores)
                    {
                        var idExterno = cont.IdExternoContenedor;
                        var tpContenedor = cont.TipoContenedor;

                        if (c.TrackingSincronizado ?? false)
                        {
                            var valorConcatenado = cont.IdExternoContenedor;
                            idExterno = valorConcatenado.Split('#')[0];
                            tpContenedor = valorConcatenado.Split('#')[1];
                        }

                        model.SetContenedorCamion(idExterno, tpContenedor, cont.Empresa);
                    }
                }
            }
        }
        public virtual Camion CreateCamion(CamionRequest c)
        {
            return new Camion
                (
                    MapEstado(CamionEstado.AguardandoCarga),
                    MapBooleanToString(c.RespetaOrdenCarga ?? false),
                    MapBooleanToString(c.Tracking ?? false),
                    MapBooleanToString(c.Ruteo ?? false),
                    MapBooleanToString(c.TrackingSincronizado ?? false),
                    MapBooleanToString(false),
                    MapBooleanToString(c.CargaHabilitada ?? true),
                    MapBooleanToString(c.CierreHabilitado ?? true),
                    MapBooleanToString(c.ArmadoHabilitado ?? true),
                    MapBooleanToString(c.CierreParcial ?? true),
                    MapBooleanToString(c.CierreAutomatico ?? false),
                    MapBooleanToString(c.CargaLibre ?? false),
                    MapBooleanToString(c.RequiereControlContenedores ?? false),
                    MapBooleanToString(c.HabilitarUsoCargaAsignada ?? false),
                    c.ProgramacionHoraInicio,
                    c.ProgramacionHoraFin,
                    c.Necesidades,
                    c.PredioExterno
                );
        }

        //Salida
        public virtual EgresoResponse MapToResponse(Camion camion, Dictionary<string, Agente> agentes)
        {
            var result = new EgresoResponse()
            {
                Camion = camion.Id,
                Descripcion = camion.Descripcion,
                Predio = camion.Predio,
                Empresa = camion.Empresa,
                Ruta = camion.Ruta,
                Puerta = camion.Puerta,
                Estado = MapEstado(camion.Estado),
                Matricula = camion.Matricula,
                Transportista = camion.Transportista,
                Vehiculo = camion.Vehiculo,
                FechaCreacion = camion.FechaCreacion.ToString(CDateFormats.DATE_ONLY),
                FechaProgramado = camion.FechaProgramado.ToString(CDateFormats.DATE_ONLY),
                //FechaArriboCamion = camion.FechaArriboCamion.ToString(CDateFormats.DATE_ONLY),
                FechaFacturacion = camion.FechaFacturacion.ToString(CDateFormats.DATE_ONLY),
                FechaCierre = camion.FechaCierre.ToString(CDateFormats.DATE_ONLY),
                //FechaModificacion = camion.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                FuncionarioCierre = camion.FuncionarioCierre,
                NumeroInterfazEjecucionCierre = camion.NumeroInterfazEjecucionCierre,
                NumeroInterfazEjecucionFactura = camion.NumeroInterfazEjecucionFactura,
                ArmadoEgreso = camion.ArmadoEgreso,
                TipoArmadoEgreso = camion.TipoArmadoEgreso,
                Documento = camion.Documento,
                RespetaOrdenCarga = MapBooleanToString(camion.RespetaOrdenCarga),
                TrackingHabilitado = MapBooleanToString(camion.IsTrackingHabilitado),
                RuteoHabilitado = MapBooleanToString(camion.IsRuteoHabilitado),
                SincronizacionRealizada = MapBooleanToString(camion.IsSincronizacionRealizada),
                ConfirmacionViajeRealizada = MapBooleanToString(camion.ConfirmacionViajeRealizada),
                CargaHabilitada = MapBooleanToString(camion.IsCargaHabilitada),
                CierreHabilitado = MapBooleanToString(camion.IsCierreHabilitado),
                ArmadoHabilitado = MapBooleanToString(camion.ArmadoHabilitado),
                CierreParcialHabilitado = MapBooleanToString(camion.IsCierreParcialHabilitado),
                CierreAutomaticoHabilitado = MapBooleanToString(camion.IsCierreAutomaticoHabilitado),
                CargaAutomaticaHabilitada = MapBooleanToString(camion.IsCargaAutomaticaHabilitada),
                ControlContenedoresHabilitado = MapBooleanToString(camion.IsControlContenedoresHabilitado),
                NumeroTransaccion = camion.NumeroTransaccion,
                NumeroOrtOrden = camion.NumeroOrtOrden,
                IdExterno = camion.IdExterno,
                EmpresaExterna = camion.EmpresaExterna
            };

            if (camion.Cargas != null && camion.Cargas.Count > 0)
            {
                foreach (var carga in camion.Cargas)
                {
                    string keyAgente = $"{carga.Cliente}{carga.Empresa}";
                    result.CargasAsociadas.Add(MapCargaToResponse(carga, agentes[keyAgente]));
                }
            }

            return result;
        }
        public virtual CargasAsociadasResponse MapCargaToResponse(CargaCamion carga, Agente agente)
        {
            if (carga == null || agente == null)
                return null;

            var c = new CargasAsociadasResponse
            {
                Carga = carga.Carga,
                CodigoAgente = agente.Codigo,
                TipoAgente = agente.Tipo,
                Empresa = carga.Empresa,
                ParaCargar = carga.IdCargar,
                ModalidadDeCarga = carga.TipoModalidad,
                SincronizacionRealizada = MapBooleanToString(carga.SincronizacionRealizada),
                //FechaAlta = carga.FechaAlta,
                //FechaModificacion = carga.FechaModificacion,
            };

            return c;
        }
        public virtual short MapEstado(CamionEstado estado)
        {
            switch (estado)
            {
                case CamionEstado.AguardandoCarga: return SituacionDb.CamionAguardandoCarga;
                case CamionEstado.Cargando: return SituacionDb.CamionCargando;
                case CamionEstado.Cerrado: return SituacionDb.CamionCerrado;
                case CamionEstado.IniciandoCierre: return SituacionDb.CamionIniciandoCierre;
                case CamionEstado.SinOrdenDeTrabajo: return SituacionDb.CamionSinOrdenDeTrabajo;
            }

            return -1;
        }
    }
}
