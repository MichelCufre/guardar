using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Configuracion;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ReferenciaRecepcionRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly ReferenciaRecepcionMapper _mapper;
        protected readonly AgendaRepository _AgendaRepository;
        protected readonly RecepcionTipoMapper _mapperRecepcionTipo;

        public ReferenciaRecepcionRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ReferenciaRecepcionMapper();
            this._AgendaRepository = new AgendaRepository(this._context, this._cdAplicacion, this._userId, dapper);
            this._mapperRecepcionTipo = new RecepcionTipoMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyReferencia(int idEmpresa, string codigoInternoAgente, string tipoReferencia, string numeroReferencia)
        {
            return this._context.T_RECEPCION_REFERENCIA.Any(s => s.CD_EMPRESA == idEmpresa && s.CD_CLIENTE == codigoInternoAgente && s.TP_REFERENCIA == tipoReferencia && s.NU_REFERENCIA == numeroReferencia);
        }

        public virtual bool ReferenciaEnUso(int idReferencia)
        {
            return this._context.T_RECEPCION_REFERENCIA_DET.Any(rd => rd.NU_RECEPCION_REFERENCIA == idReferencia && (rd.QT_RECIBIDA != 0 || rd.QT_AGENDADA != 0 || rd.QT_ANULADA != 0));
        }

        public virtual bool AnyReferenciaRecapcionTipo(string tipo)
        {
            return this._context.T_RECEPCION_REFERENCIA_TIPO.Any(s => s.TP_REFERENCIA == tipo);
        }

        public virtual bool AnyRecepcionTipo(string tipoReferencia)
        {
            return this._context.T_RECEPCION_TIPO.Any(r => r.TP_REFERENCIA == tipoReferencia);
        }

        public virtual bool AnyRecepcionTipo(string tipoReferencia, string tipoAgente)
        {
            return this._context.T_RECEPCION_TIPO.Any(r => r.TP_REFERENCIA == tipoReferencia && r.TP_AGENTE == tipoAgente);
        }

        public virtual bool AnyReferenciaDetalle(int idReferencia, int idEmpresa, string codigoProducto, string identificador, decimal faixa)
        {
            return this._context.T_RECEPCION_REFERENCIA_DET.Any(s => s.NU_RECEPCION_REFERENCIA == idReferencia && s.CD_EMPRESA == idEmpresa && s.CD_PRODUTO == codigoProducto && s.NU_IDENTIFICADOR == identificador && s.CD_FAIXA == faixa);
        }

        public virtual bool AnyReferenciaEnAgenda(int numeroAgenda)
        {
            return _context.T_RECEPC_AGENDA_REFERENCIA_REL.AsNoTracking().Any(s => s.NU_AGENDA == numeroAgenda);
        }

        public virtual bool AnyReferenciaAsociada(int referencia)
        {
            return _context.T_RECEPC_AGENDA_REFERENCIA_REL.AsNoTracking().Any(s => s.NU_RECEPCION_REFERENCIA == referencia);
        }

        public virtual bool PuedeCancelarReferencia(int referencia)
        {
            var asociaciones = _context.T_RECEPC_AGENDA_REFERENCIA_REL.Where(s => s.NU_RECEPCION_REFERENCIA == referencia).ToList();
            var estados = new List<EstadoAgenda>() { EstadoAgenda.Cerrada, EstadoAgenda.Cancelada };
            foreach (var asociacion in asociaciones)
            {
                if (!estados.Contains(this._AgendaRepository.GetAgendaEstado(asociacion.NU_AGENDA)))
                    return false;
            }
            return true;
        }

        public virtual bool AnyAsociacionesAgendaConDetallesReferencias(int numeroAgenda)
        {
            return _context.T_RECEPCION_AGENDA_REFERENCIA.AsNoTracking().Any(rar => rar.NU_AGENDA == numeroAgenda);

        }

        #endregion

        #region Get

        public virtual ReferenciaRecepcionConfiguracion GetReferenciaConfiguracion()
        {
            ReferenciaRecepcionConfiguracion configuracion = new ReferenciaRecepcionConfiguracion();

            configuracion.PermiteDigitacion = (this._context.T_LPARAMETRO_CONFIGURACION.AsNoTracking().FirstOrDefault(s => s.CD_PARAMETRO == "DIGITA_REFERENCIAS")?.VL_PARAMETRO ?? "N") == "S";

            return configuracion;
        }

        public virtual ReferenciaRecepcion GetReferencia(int idReferencia)
        {
            return this._mapper.MapToObjectWithDetail(this._context.T_RECEPCION_REFERENCIA.AsNoTracking().FirstOrDefault(s => s.NU_RECEPCION_REFERENCIA == idReferencia));
        }

        public virtual ReferenciaRecepcion GetReferenciaConDetalle(int idReferencia)
        {
            T_RECEPCION_REFERENCIA entity = this._context.T_RECEPCION_REFERENCIA
                .Include("T_RECEPCION_REFERENCIA_DET")
                .Include("T_RECEPC_AGENDA_REFERENCIA_REL")
                .AsNoTracking()
                .FirstOrDefault(s => s.NU_RECEPCION_REFERENCIA == idReferencia);

            return this._mapper.MapToObjectWithDetail(entity);
        }

        public virtual string GetEstadoReferencia(int idReferencia)
        {
            return this._context.T_RECEPCION_REFERENCIA.FirstOrDefault(rd => rd.NU_RECEPCION_REFERENCIA == idReferencia).ND_ESTADO_REFERENCIA;
        }

        public virtual List<ReferenciaRecepcionTipo> GetReferenciaRecepcionTipos()
        {
            return this._context.T_RECEPCION_REFERENCIA_TIPO.AsNoTracking()
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual string GetDescripcionTipoReferencia(string tipoReferencia)
        {
            return this._context.T_RECEPCION_TIPO.FirstOrDefault(r => r.TP_REFERENCIA == tipoReferencia)?.DS_TIPO_RECEPCION;
        }

        public virtual string GetTipoReferencia(string tipoRecepcion)
        {
            return this._context.T_RECEPCION_TIPO.FirstOrDefault(r => r.TP_RECEPCION == tipoRecepcion)?.TP_REFERENCIA;
        }

        public virtual ReferenciaRecepcionDetalle GetReferenciaDetalle(int idDetalleReferencia)
        {
            return this._mapper.MapDetalleToObject(this._context.T_RECEPCION_REFERENCIA_DET
                .AsNoTracking()
                .FirstOrDefault(s => s.NU_RECEPCION_REFERENCIA_DET == idDetalleReferencia));
        }

        public virtual List<ReferenciaRecepcionDetalle> GetReferenciasRecepcionModalidadBolsa(string tipoReferencia, int idEmpresa, string codigoInternoCliente, string codigoProducto, decimal faixa, string identificador)
        {
            var listaDetalle = (from REF in _context.T_RECEPCION_REFERENCIA
                                join DET in _context.T_RECEPCION_REFERENCIA_DET on new { REF.NU_RECEPCION_REFERENCIA } equals new { DET.NU_RECEPCION_REFERENCIA }
                                where REF.TP_REFERENCIA == tipoReferencia
                                   && REF.ND_ESTADO_REFERENCIA == EstadoReferenciaRecepcionDb.Abierta
                                   && DET.CD_EMPRESA == idEmpresa
                                   && REF.CD_CLIENTE == codigoInternoCliente
                                   && DET.CD_PRODUTO == codigoProducto
                                   && DET.CD_FAIXA == faixa
                                   && (DET.NU_IDENTIFICADOR == identificador || DET.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                                select new ReferenciaRecepcionDetalle
                                {
                                    Id = DET.NU_RECEPCION_REFERENCIA_DET,
                                    IdReferencia = DET.NU_RECEPCION_REFERENCIA,
                                    IdEmpresa = DET.CD_EMPRESA,
                                    CodigoProducto = DET.CD_PRODUTO,
                                    Faixa = DET.CD_FAIXA,
                                    Identificador = DET.NU_IDENTIFICADOR,
                                    IdLineaSistemaExterno = DET.ID_LINEA_SISTEMA_EXTERNO,
                                    FechaVencimiento = DET.DT_VENCIMIENTO,
                                    FechaInsercion = DET.DT_ADDROW,

                                    CantidadReferencia = DET.QT_REFERENCIA ?? 0,
                                    CantidadAgendada = DET.QT_AGENDADA ?? 0,
                                    CantidadAnulada = DET.QT_ANULADA ?? 0,
                                    CantidadRecibida = DET.QT_RECIBIDA ?? 0,

                                    ReferenciaRecepcion = new ReferenciaRecepcion()
                                    {
                                        FechaVencimientoOrden = REF.DT_VENCIMIENTO_ORDEN
                                    }

                                }).ToList();

            return listaDetalle;
        }

        public virtual List<ReferenciaRecepcionDetalle> GetReferenciasRecepcionModalidadSeleccion(string tipoReferencia, int numeroAgenda, int idEmpresa, string codigoInternoCliente, string codigoProducto, decimal faixa, string identificador)
        {
            var listaDetalle = (from RE in _context.T_RECEPCION_REFERENCIA
                                join DRE in _context.T_RECEPCION_REFERENCIA_DET on new { RE.NU_RECEPCION_REFERENCIA } equals new { DRE.NU_RECEPCION_REFERENCIA }
                                where RE.T_RECEPC_AGENDA_REFERENCIA_REL.Where(d => d.NU_AGENDA == numeroAgenda).Any()
                                   && RE.TP_REFERENCIA == tipoReferencia
                                   && RE.ND_ESTADO_REFERENCIA == EstadoReferenciaRecepcionDb.Abierta
                                   && DRE.CD_PRODUTO == codigoProducto
                                   && DRE.CD_EMPRESA == idEmpresa
                                   && RE.CD_CLIENTE == codigoInternoCliente
                                   && DRE.CD_FAIXA == faixa
                                   && (DRE.NU_IDENTIFICADOR == identificador || DRE.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                                select new ReferenciaRecepcionDetalle
                                {
                                    Id = DRE.NU_RECEPCION_REFERENCIA_DET,
                                    IdReferencia = DRE.NU_RECEPCION_REFERENCIA,
                                    IdEmpresa = DRE.CD_EMPRESA,
                                    CodigoProducto = DRE.CD_PRODUTO,
                                    Faixa = DRE.CD_FAIXA,
                                    Identificador = DRE.NU_IDENTIFICADOR,
                                    IdLineaSistemaExterno = DRE.ID_LINEA_SISTEMA_EXTERNO,
                                    FechaVencimiento = DRE.DT_VENCIMIENTO,
                                    FechaInsercion = DRE.DT_ADDROW,

                                    CantidadReferencia = DRE.QT_REFERENCIA ?? 0,
                                    CantidadAgendada = DRE.QT_AGENDADA ?? 0,
                                    CantidadAnulada = DRE.QT_ANULADA ?? 0,
                                    CantidadRecibida = DRE.QT_RECIBIDA ?? 0,

                                    ReferenciaRecepcion = new ReferenciaRecepcion()
                                    {
                                        FechaVencimientoOrden = RE.DT_VENCIMIENTO_ORDEN
                                    }

                                }).ToList();

            return listaDetalle;
        }

        public virtual List<ReferenciaRecepcion> GetReferenciasAgenda(int idAgenda)
        {
            List<ReferenciaRecepcion> referencias = new List<ReferenciaRecepcion>();

            var entries = _context.T_RECEPC_AGENDA_REFERENCIA_REL.AsNoTracking().Include("T_RECEPCION_REFERENCIA")
                                                                .AsNoTracking()
                                                                .Where(w => w.NU_AGENDA == idAgenda)
                                                                .Select(w => w.T_RECEPCION_REFERENCIA)
                                                                .ToList();

            foreach (var entry in entries)
            {
                entry.T_RECEPCION_REFERENCIA_DET = _context.T_RECEPCION_REFERENCIA_DET.AsNoTracking().Where(s => s.NU_RECEPCION_REFERENCIA == entry.NU_RECEPCION_REFERENCIA).ToList();
                referencias.Add(this._mapper.MapToObjectWithDetail(entry));
            }

            return referencias;
        }

        public virtual List<ReferenciaRecepcion> GetCabezalReferenciasAgenda(int idAgenda)
        {
            return _context.T_RECEPC_AGENDA_REFERENCIA_REL
                .Include("T_RECEPCION_REFERENCIA")
                .AsNoTracking()
                .Where(w => w.NU_AGENDA == idAgenda)
                .Select(w => _mapper.MapToObject(w.T_RECEPCION_REFERENCIA))
                .ToList();
        }

        public virtual List<ReferenciaRecepcionDetalle> GetExcesoSaldoReferencia(Agenda agenda, AgendaDetalle detAgenda)
        {
            var tpReferencia = GetTipoReferencia(agenda.TipoRecepcionInterno);

            var colDetRef = (from RE in _context.T_RECEPCION_REFERENCIA
                             join DRE in _context.T_RECEPCION_REFERENCIA_DET on new { RE.NU_RECEPCION_REFERENCIA } equals new { DRE.NU_RECEPCION_REFERENCIA }
                             where RE.T_RECEPC_AGENDA_REFERENCIA_REL.Where(d => d.NU_AGENDA == detAgenda.IdAgenda).Any() && RE.TP_REFERENCIA == tpReferencia
                             && RE.CD_SITUACAO == SituacionDb.Activo
                             && DRE.CD_PRODUTO == detAgenda.CodigoProducto
                             && DRE.CD_EMPRESA == detAgenda.IdEmpresa
                             && RE.CD_CLIENTE == agenda.CodigoInternoCliente
                             && DRE.CD_FAIXA == detAgenda.Faixa
                             && (DRE.NU_IDENTIFICADOR == detAgenda.Identificador || DRE.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                             select new ReferenciaRecepcionDetalle
                             {
                                 IdEmpresa = DRE.CD_EMPRESA,
                                 CodigoProducto = DRE.CD_PRODUTO,
                                 Faixa = DRE.CD_FAIXA,
                                 Identificador = DRE.NU_IDENTIFICADOR,
                                 IdReferencia = DRE.NU_RECEPCION_REFERENCIA,
                                 Id = DRE.NU_RECEPCION_REFERENCIA_DET,
                                 IdLineaSistemaExterno = DRE.ID_LINEA_SISTEMA_EXTERNO,
                                 FechaVencimiento = DRE.DT_VENCIMIENTO,
                                 FechaInsercion = DRE.DT_ADDROW,

                                 CantidadAgendada = DRE.QT_AGENDADA ?? 0,
                                 CantidadReferencia = DRE.QT_REFERENCIA ?? 0,
                                 CantidadAnulada = DRE.QT_ANULADA ?? 0,
                                 CantidadRecibida = DRE.QT_RECIBIDA ?? 0,

                                 ReferenciaRecepcion = new ReferenciaRecepcion()
                                 {
                                     FechaVencimientoOrden = RE.DT_VENCIMIENTO_ORDEN
                                 }
                             }).ToList();

            return colDetRef;
        }

        public virtual List<DetalleAgendaReferenciaAsociada> GetDetalleAgendaReferenciaAsociada(AgendaDetalle detalleAgenda)
        {
            var entries = this._context.T_RECEPCION_AGENDA_REFERENCIA
                .Include("T_RECEPCION_REFERENCIA_DET").AsNoTracking()
                .Where(d => d.NU_AGENDA == detalleAgenda.IdAgenda
                    && d.CD_EMPRESA == detalleAgenda.IdEmpresa
                    && d.CD_PRODUTO == detalleAgenda.CodigoProducto
                    && d.CD_FAIXA == detalleAgenda.Faixa
                    && (d.NU_IDENTIFICADOR == detalleAgenda.Identificador || d.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)).ToList();

            var asociaciones = new List<DetalleAgendaReferenciaAsociada>();

            foreach (var entry in entries)
            {
                asociaciones.Add(new DetalleAgendaReferenciaAsociada()
                {
                    DetalleAgenda = detalleAgenda,
                    CantidadAgendada = entry.QT_AGENDADA ?? 0,
                    CantidadRecibida = entry.QT_RECIBIDA ?? 0,
                    DetalleReferencia = this._mapper.MapDetalleToObject(entry.T_RECEPCION_REFERENCIA_DET),
                    FechaInsercion = entry.DT_ADDROW,
                    NumeroTransaccion = entry.NU_TRANSACCION,
                    NumeroTransaccionDelete = entry.NU_TRANSACCION_DELETE,
                    NumeroInterfazEjecucion = entry.NU_INTERFAZ_EJECUCION,
                });
            }

            return asociaciones;
        }

        public virtual DetalleAgendaReferenciaAsociada GetDetalleAgendaReferenciaAsociada(AgendaDetalle detalleAgenda, ReferenciaRecepcionDetalle detalleReferencia)
        {
            var entry = this._context.T_RECEPCION_AGENDA_REFERENCIA.AsNoTracking()
               .FirstOrDefault(d => d.NU_AGENDA == detalleAgenda.IdAgenda
                                 && d.NU_RECEPCION_REFERENCIA_DET == detalleReferencia.Id
                                 && d.CD_EMPRESA == detalleAgenda.IdEmpresa
                                 && d.CD_PRODUTO == detalleAgenda.CodigoProducto
                                 && d.CD_FAIXA == detalleAgenda.Faixa
                                 && d.NU_IDENTIFICADOR == detalleAgenda.Identificador);

            if (entry == null)
                return null;

            return new DetalleAgendaReferenciaAsociada()
            {
                DetalleAgenda = detalleAgenda,
                CantidadAgendada = entry.QT_AGENDADA ?? 0,
                CantidadRecibida = entry.QT_RECIBIDA ?? 0,
                DetalleReferencia = detalleReferencia

            };

        }

        public virtual List<DetalleAgendaReferenciaAsociada> GetAsociacionesAgendaConDetallesAuxiliares(int numeroAgenda, string claveDetalleAuxiliar)
        {
            var entries = _context.T_RECEPCION_AGENDA_REFERENCIA.Include("T_RECEPCION_REFERENCIA_DET").AsNoTracking()
                                                               .Where(rar => rar.NU_AGENDA == numeroAgenda
                                                                          && rar.T_RECEPCION_REFERENCIA_DET.ID_LINEA_SISTEMA_EXTERNO == claveDetalleAuxiliar)
                                                                 .ToList();

            var asociaciones = new List<DetalleAgendaReferenciaAsociada>();

            foreach (var entry in entries)
            {
                asociaciones.Add(new DetalleAgendaReferenciaAsociada()
                {
                    CantidadAgendada = entry.QT_AGENDADA ?? 0,
                    CantidadRecibida = entry.QT_RECIBIDA ?? 0,
                    DetalleReferencia = this._mapper.MapDetalleToObject(entry.T_RECEPCION_REFERENCIA_DET),
                    FechaInsercion = entry.DT_ADDROW,
                    NumeroTransaccion = entry.NU_TRANSACCION,
                    NumeroTransaccionDelete = entry.NU_TRANSACCION_DELETE,
                });
            }

            return asociaciones;

        }

        public virtual List<int> GetNumeroAgendasByReferenciaRecepcion(int referencia)
        {
            return this._context.T_RECEPC_AGENDA_REFERENCIA_REL.Where(x => x.NU_RECEPCION_REFERENCIA == referencia).ToList().Select(x => x.NU_AGENDA).ToList();

        }

        public virtual List<DetalleAgendaReferenciaAsociada> GetAllDetalleAgendaReferenciaAsociadaActivas(Agenda agenda)
        {
            // Entro por el detalle porque las referecnias lote (AUTO) puede estar en mas de un detalle de agenda con lote diferente a auto

            // Levanta los detalles de referencias que participan en la agenda
            var entries = this._context.T_RECEPCION_REFERENCIA_DET.Include("T_RECEPCION_AGENDA_REFERENCIA")
                .Include("T_RECEPCION_REFERENCIA")
                .AsNoTracking()
                 .Where(d => d.T_RECEPCION_AGENDA_REFERENCIA.Any(s => s.NU_AGENDA == agenda.Id)
                 && d.T_RECEPCION_REFERENCIA.ND_ESTADO_REFERENCIA == EstadoReferenciaRecepcionDb.Abierta)
                 .OrderBy(r => r.T_RECEPCION_REFERENCIA.DT_ADDROW).ToList();

            var asociaciones = new List<DetalleAgendaReferenciaAsociada>();

            foreach (var entry in entries)
            {
                var detalleReferencia = this._mapper.MapDetalleToObject(entry);

                detalleReferencia.ReferenciaRecepcion = this._mapper.MapToObject(entry.T_RECEPCION_REFERENCIA);

                // Recorro las asociaciones correspondientes a la agenda, La query inicial levanta todas las asociaciones donde participa el detalle de referencia.
                foreach (var entryAsociacion in entry.T_RECEPCION_AGENDA_REFERENCIA.Where(s => s.NU_AGENDA == agenda.Id))
                {

                    var asociacion = new DetalleAgendaReferenciaAsociada()
                    {
                        DetalleReferencia = detalleReferencia,

                        CantidadAgendada = entryAsociacion.QT_AGENDADA ?? 0,
                        CantidadRecibida = entryAsociacion.QT_RECIBIDA ?? 0,
                        FechaInsercion = entryAsociacion.DT_ADDROW,

                        NumeroTransaccion = entryAsociacion.NU_TRANSACCION,
                        NumeroTransaccionDelete = entryAsociacion.NU_TRANSACCION_DELETE,
                    };

                    // Asocio el detalle de agenda a la asociación
                    asociacion.DetalleAgenda = agenda.Detalles.FirstOrDefault(s => s.IdAgenda == entryAsociacion.NU_AGENDA
                                                                              && s.CodigoProducto == entryAsociacion.CD_PRODUTO
                                                                              && s.Identificador == entryAsociacion.NU_IDENTIFICADOR
                                                                              && s.Faixa == entryAsociacion.CD_FAIXA
                                                                              && s.IdEmpresa == entryAsociacion.CD_EMPRESA);

                    // Agrego la asociacion al detalle de la agenda para obtener la referencia desde la agenda
                    if (asociacion.DetalleAgenda != null)
                        asociacion.DetalleAgenda.AsociacionesDetalleReferencia.Add(asociacion);

                    asociaciones.Add(asociacion);

                }

            }

            return asociaciones;

        }

        public virtual List<ReferenciaRecepcionDetalle> GetDetalleReferenciasNoAsociadasDisponibles(Agenda agenda, List<DetalleAgendaReferenciaAsociada> asociaciones)
        {
            // Obtengo las lineas (AUTO) de las referecnias asociadas a la agenda que no contengan asociación a nivel de linea de agenda
            // que esten en situación abiertas y con saldo

            var referencias = this._context.T_RECEPC_AGENDA_REFERENCIA_REL.Where(s => s.NU_AGENDA == agenda.Id
                                                                                   && s.T_RECEPCION_REFERENCIA.ND_ESTADO_REFERENCIA == EstadoReferenciaRecepcionDb.Abierta)
                                                                          .Select(s => s.NU_RECEPCION_REFERENCIA)
                                                                          .ToList();

            var detallesAuto = this._context.T_RECEPCION_REFERENCIA_DET.Where(s => referencias.Contains(s.NU_RECEPCION_REFERENCIA)
                                                                               //  && s.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto
                                                                               && (s.QT_REFERENCIA ?? 0) - (s.QT_AGENDADA ?? 0) - (s.QT_ANULADA ?? 0) - (s.QT_RECIBIDA ?? 0) > 0)
                                                                       .OrderBy(r => r.T_RECEPCION_REFERENCIA.DT_ADDROW).ToList();

            List<ReferenciaRecepcionDetalle> detallesReferencias = new List<ReferenciaRecepcionDetalle>();

            foreach (var entry in detallesAuto)
            {
                if (!asociaciones.Any(s => s.DetalleReferencia.Id == entry.NU_RECEPCION_REFERENCIA_DET))
                {
                    detallesReferencias.Add(this._mapper.MapDetalleToObject(entry));
                }

            }

            return detallesReferencias;
        }

        #endregion

        #region Add

        public virtual void AddReferencia(ReferenciaRecepcion referencia)
        {
            referencia.Id = this._context.GetNextSequenceValueInt(_dapper, "S_RECEPCION_REFERENCIA");

            T_RECEPCION_REFERENCIA entity = this._mapper.MapToEntity(referencia);

            this._context.T_RECEPCION_REFERENCIA.Add(entity);
        }

        public virtual void AddDetalleRecepcionReferencia(int nuRecepcionRef, string cdProducto, string nuIdent, int cdEmpresa,
                    decimal cdFaixa, string idLineaSistemaExterno, decimal? qtReferencia, decimal? qtAnulada, decimal? qtAgendada, decimal? qtRecibida,
                    decimal? qtConfirmarInterfaz, decimal? imUnitario, string anexo1, DateTime? dtVencimiento, long nuTransaccionDB)
        {
            T_RECEPCION_REFERENCIA_DET recRefDet = new T_RECEPCION_REFERENCIA_DET();
            var recRefDetEntity = this._context.T_RECEPCION_REFERENCIA_DET.AsNoTracking().FirstOrDefault(x => x.NU_RECEPCION_REFERENCIA == nuRecepcionRef && x.CD_EMPRESA == cdEmpresa && x.CD_PRODUTO == cdProducto && x.NU_IDENTIFICADOR == nuIdent && x.CD_FAIXA == cdFaixa);

            if (recRefDet == null)
            {
                recRefDet = new T_RECEPCION_REFERENCIA_DET
                {
                    NU_RECEPCION_REFERENCIA = nuRecepcionRef,
                    CD_PRODUTO = cdProducto,
                    CD_EMPRESA = cdEmpresa,
                    CD_FAIXA = cdFaixa,
                    NU_IDENTIFICADOR = nuIdent,
                    ID_LINEA_SISTEMA_EXTERNO = idLineaSistemaExterno,
                    QT_AGENDADA = qtAgendada,
                    QT_ANULADA = qtAnulada,
                    QT_CONFIRMADA_INTERFAZ = qtConfirmarInterfaz,
                    QT_REFERENCIA = qtReferencia,
                    QT_RECIBIDA = qtRecibida,
                    IM_UNITARIO = imUnitario,
                    DS_ANEXO1 = anexo1,
                    DT_ADDROW = DateTime.Now,
                    DT_UPDROW = DateTime.Now,
                    DT_VENCIMIENTO = dtVencimiento,
                    NU_TRANSACCION = nuTransaccionDB,
                };
                this._context.T_RECEPCION_REFERENCIA_DET.Add(recRefDet);
            }
        }

        public virtual void AddReferenciaDetalle(ReferenciaRecepcionDetalle detalle)
        {
            detalle.Id = this._context.GetNextSequenceValueInt(_dapper, "S_RECEPCION_REFERENCIA_DET");

            detalle.FechaInsercion = DateTime.Now;
            detalle.FechaModificacion = DateTime.Now;

            T_RECEPCION_REFERENCIA_DET entity = this._mapper.MapDetalleToEntity(detalle);

            this._context.T_RECEPCION_REFERENCIA_DET.Add(entity);
        }

        public virtual void AsociarReferenciaAgenda(int numeroAgenda, int idReferencia)
        {
            T_RECEPC_AGENDA_REFERENCIA_REL entity = new T_RECEPC_AGENDA_REFERENCIA_REL()
            {
                NU_AGENDA_REFERENCIA_REL = this._context.GetNextSequenceValueInt(_dapper, "S_RECEPC_AGENDA_REFERENCIA_REL"),
                NU_AGENDA = numeroAgenda,
                NU_RECEPCION_REFERENCIA = idReferencia,
            };

            this._context.T_RECEPC_AGENDA_REFERENCIA_REL.Add(entity);

        }

        public virtual void AsociarDetalleReferenciaConDetalleAgenda(AgendaDetalle detalleAgenda, ReferenciaRecepcionDetalle detalleReferencia, decimal cantidadAgendada, decimal cantidadRecibida)
        {
            T_RECEPCION_AGENDA_REFERENCIA entity = new T_RECEPCION_AGENDA_REFERENCIA()
            {
                CD_EMPRESA = detalleAgenda.IdEmpresa,
                CD_FAIXA = detalleAgenda.Faixa,
                CD_PRODUTO = detalleAgenda.CodigoProducto,
                NU_AGENDA = detalleAgenda.IdAgenda,
                NU_IDENTIFICADOR = detalleAgenda.Identificador,

                NU_RECEPCION_REFERENCIA_DET = detalleReferencia.Id,
                QT_AGENDADA = cantidadAgendada,
                QT_RECIBIDA = cantidadRecibida,
                DT_ADDROW = DateTime.Now,

                NU_TRANSACCION = detalleAgenda.NumeroTransaccion,
            };

            if (detalleReferencia.ReferenciaRecepcion != null)
                entity.NU_INTERFAZ_EJECUCION = detalleReferencia.ReferenciaRecepcion.NumeroInterfazEjecucion;

            this._context.T_RECEPCION_AGENDA_REFERENCIA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateReferencia(ReferenciaRecepcion referencia)
        {
            var entity = this._mapper.MapToEntity(referencia);
            var attachedEntity = this._context.T_RECEPCION_REFERENCIA.Local
                .FirstOrDefault(r => r.NU_RECEPCION_REFERENCIA == entity.NU_RECEPCION_REFERENCIA);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_RECEPCION_REFERENCIA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateReferencia(EntityChanges<ReferenciaRecepcion> records)
        {
            //foreach (var deletedRecord in records.DeletedRecords)
            //{
            //    this.DeleteReferencia(deletedRecord);
            //}

            //foreach (var newRecord in records.AddedRecords)
            //{
            //    this.AddReferencia(newRecord);
            //}

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                this.UpdateReferencia(updatedRecord);
            }
        }

        public virtual void UpdateReferencia(ReferenciaRecepcion referencia, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalles)
        {
            this.UpdateReferencia(referencia);
            this.UpdateReferenciaDetalles(cambiosDetalles);
        }

        public virtual void UpdateReferenciaDetalle(ReferenciaRecepcionDetalle detalle)
        {
            var entity = this._mapper.MapDetalleToEntity(detalle);
            var attachedEntity = this._context.T_RECEPCION_REFERENCIA_DET.Local
                .FirstOrDefault(d => d.NU_RECEPCION_REFERENCIA_DET == detalle.Id
                    && d.NU_RECEPCION_REFERENCIA == detalle.IdReferencia);

            detalle.FechaModificacion = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_RECEPCION_REFERENCIA_DET.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateReferenciaDetalles(EntityChanges<ReferenciaRecepcionDetalle> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                this.DeleteReferenciaDetalle(deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                this.AddReferenciaDetalle(newRecord);
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                this.UpdateReferenciaDetalle(updatedRecord);
            }
        }

        public virtual void UpdateDetalleAgendaReferenciaAsociada(EntityChanges<DetalleAgendaReferenciaAsociada> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                this.RemoveDetalleAgendaReferenciaAsociada(deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                newRecord.DetalleAgenda.NumeroTransaccion = newRecord.NumeroTransaccion;
                this.AsociarDetalleReferenciaConDetalleAgenda(newRecord.DetalleAgenda, newRecord.DetalleReferencia, newRecord.CantidadAgendada, newRecord.CantidadRecibida);
            }
        }

        public virtual void UpdateDetalleAgendaReferenciaAsociada(DetalleAgendaReferenciaAsociada detalle)
        {
            var entity = this._mapper.MapToEntity(detalle);
            var attachedEntity = this._context.T_RECEPCION_AGENDA_REFERENCIA.Local
                .FirstOrDefault(d => d.NU_AGENDA == detalle.DetalleAgenda.IdAgenda
                    && d.NU_RECEPCION_REFERENCIA_DET == detalle.DetalleReferencia.Id
                    && d.CD_EMPRESA == detalle.DetalleAgenda.IdEmpresa
                    && d.CD_PRODUTO == detalle.DetalleAgenda.CodigoProducto
                    && d.CD_FAIXA == detalle.DetalleAgenda.Faixa
                    && d.NU_IDENTIFICADOR == detalle.DetalleAgenda.Identificador);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_RECEPCION_AGENDA_REFERENCIA.Attach(entity);
                _context.Entry<T_RECEPCION_AGENDA_REFERENCIA>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteReferenciaDetalle(ReferenciaRecepcionDetalle detalle)
        {
            var entity = this._mapper.MapDetalleToEntity(detalle);
            var attachedEntity = this._context.T_RECEPCION_REFERENCIA_DET.Local
                .FirstOrDefault(d => d.NU_RECEPCION_REFERENCIA_DET == detalle.Id
                    && d.NU_RECEPCION_REFERENCIA == detalle.IdReferencia);

            if (attachedEntity != null)
            {
                this._context.T_RECEPCION_REFERENCIA_DET.Remove(attachedEntity);
            }
            else
            {
                this._context.T_RECEPCION_REFERENCIA_DET.Attach(entity);
                this._context.T_RECEPCION_REFERENCIA_DET.Remove(entity);
            }
        }

        public virtual void RemoveReferenciaDetetalle(int nroAgenda, List<ReferenciaRecepcionDetalle> detsReferencia, string aplicacion, int userid, long nuTransaccionDB)
        {
            if (detsReferencia != null)
            {
                List<int> nrosReferenciaDet = detsReferencia
                    .Where(d => d.IdLineaSistemaExterno == ProblemaAgendaDb.DescripcionExcedeSaldoReferenciaRecepcion + nroAgenda)
                    .Select(d => d.Id).ToList();

                if (nrosReferenciaDet.Count > 0)
                {
                    var detallesReferencia = _mapper.MapDetalleToObject(_context.T_RECEPCION_REFERENCIA_DET
                        .Where(d => nrosReferenciaDet.Contains(d.NU_RECEPCION_REFERENCIA_DET)).ToList());

                    foreach (var det in detallesReferencia)
                    {
                        det.NumeroTransaccion = nuTransaccionDB;
                        det.NumeroTransaccionDelete = nuTransaccionDB;
                        det.FechaModificacion = DateTime.Now;
                        UpdateReferenciaDetalle(det);
                    }

                    _context.SaveChanges();

                    foreach (var det in detallesReferencia)
                    {
                        var entity = _mapper.MapDetalleToEntity(det);
                        var attachedEntity = this._context.T_RECEPCION_REFERENCIA_DET.Local
                                              .FirstOrDefault(d => d.NU_RECEPCION_REFERENCIA_DET == det.Id);

                        if (attachedEntity != null)
                        {
                            this._context.T_RECEPCION_REFERENCIA_DET.Remove(attachedEntity);
                        }
                        else
                        {
                            this._context.T_RECEPCION_REFERENCIA_DET.Attach(entity);
                            this._context.T_RECEPCION_REFERENCIA_DET.Remove(entity);
                        }

                    }
                }
            }
        }

        public virtual void RemoveDetalleAgendaReferenciaAsociada(DetalleAgendaReferenciaAsociada detalle)
        {
            var entity = this._mapper.MapToEntity(detalle);
            var attachedEntity = this._context.T_RECEPCION_AGENDA_REFERENCIA.Local
               .FirstOrDefault(d => d.NU_AGENDA == detalle.DetalleAgenda.IdAgenda
                   && d.NU_RECEPCION_REFERENCIA_DET == detalle.DetalleReferencia.Id
                   && d.CD_EMPRESA == detalle.DetalleAgenda.IdEmpresa
                   && d.CD_PRODUTO == detalle.DetalleAgenda.CodigoProducto
                   && d.CD_FAIXA == detalle.DetalleAgenda.Faixa
                   && d.NU_IDENTIFICADOR == detalle.DetalleAgenda.Identificador);

            if (attachedEntity != null)
            {
                this._context.T_RECEPCION_AGENDA_REFERENCIA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_RECEPCION_AGENDA_REFERENCIA.Attach(entity);
                this._context.T_RECEPCION_AGENDA_REFERENCIA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<ReferenciaRecepcion> GetReferencias(IEnumerable<ReferenciaRecepcion> referencias)
        {
            IEnumerable<ReferenciaRecepcion> resultado = new List<ReferenciaRecepcion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_RECEPCION_REFERENCIA_TEMP (NU_REFERENCIA, CD_EMPRESA, TP_REFERENCIA, CD_CLIENTE) VALUES (:Numero, :IdEmpresa, :TipoReferencia, :CodigoCliente)";
                    _dapper.Execute(connection, sql, referencias, transaction: tran);

                    sql = @"SELECT RR.NU_RECEPCION_REFERENCIA AS Id
                        , RR.NU_REFERENCIA AS Numero
                        , RR.TP_REFERENCIA AS TipoReferencia
                        , RR.CD_EMPRESA AS IdEmpresa
                        , RR.CD_CLIENTE AS CodigoCliente
                        , RR.ND_ESTADO_REFERENCIA AS Estado
                        , RR.NU_PREDIO AS IdPredio
                        FROM T_RECEPCION_REFERENCIA RR 
                        INNER JOIN T_RECEPCION_REFERENCIA_TEMP T ON RR.NU_REFERENCIA = T.NU_REFERENCIA
                            AND RR.CD_EMPRESA = T.CD_EMPRESA
                            AND RR.TP_REFERENCIA = T.TP_REFERENCIA
                            AND RR.CD_CLIENTE = T.CD_CLIENTE";

                    resultado = _dapper.Query<ReferenciaRecepcion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual ReferenciaRecepcion GetReferencia(ReferenciaRecepcion model, DbConnection connection, DbTransaction tran = null)
        {
            var param = new DynamicParameters(new
            {
                idReferencia = model.Id,
                nuReferencia = model.Numero,
                cdEmpresa = model.IdEmpresa,
                tpReferencia = model.TipoReferencia,
                cdCliente = model.CodigoCliente
            });

            string sql = @"SELECT 
                    CD_CLIENTE as CodigoCliente,
                    CD_EMPRESA as IdEmpresa,
                    CD_MONEDA as Moneda,
                    CD_SITUACAO as Situacion,
                    DS_ANEXO1 as Anexo1,
                    DS_ANEXO2 as Anexo2,
                    DS_ANEXO3 as Anexo3,
                    DS_MEMO as Memo,
                    DT_ADDROW as FechaInsercion,
                    DT_EMITIDA as FechaEmitida,
                    DT_ENTREGA as FechaEntrega,
                    DT_UPDROW as FechaModificacion,
                    DT_VENCIMIENTO_ORDEN as FechaVencimientoOrden,
                    ND_ESTADO_REFERENCIA as Estado,
                    NU_INTERFAZ_EJECUCION as NumeroInterfazEjecucion,
                    NU_PREDIO as IdPredio,
                    NU_RECEPCION_REFERENCIA as Id,
                    NU_REFERENCIA as Numero,
                    TP_REFERENCIA as TipoReferencia,
                    VL_SERIALIZADO as Serializado
                FROM T_RECEPCION_REFERENCIA ";

            if (string.IsNullOrEmpty(model.Numero))
            {
                sql += @"WHERE NU_RECEPCION_REFERENCIA = :idReferencia";
            }
            else
            {
                sql += @"WHERE NU_REFERENCIA = :nuReferencia AND CD_EMPRESA = :cdEmpresa
                AND TP_REFERENCIA = :tpReferencia AND CD_CLIENTE = :cdCliente";
            }

            return _dapper.Query<ReferenciaRecepcion>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual async Task<ReferenciaRecepcion> GetReferenciaOrNull(string nuReferencia, int empresa, string tpReferencia, string tipoAgente, string codigoAgente, CancellationToken cancelToken = default)
        {
            var agente = new AgenteRepository(_context, _cdAplicacion, _userId, _dapper).GetAgenteOrNull(empresa, codigoAgente, tipoAgente).Result;

            if (agente == null)
                return null;

            return await GetReferenciaOrNull(nuReferencia, empresa, tpReferencia, agente.CodigoInterno, cancelToken);
        }

        public virtual async Task<ReferenciaRecepcion> GetReferenciaOrNull(string nuReferencia, int empresa, string tpReferencia, string cliente, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetReferencia(new ReferenciaRecepcion()
                {
                    Numero = nuReferencia,
                    IdEmpresa = empresa,
                    TipoReferencia = tpReferencia,
                    CodigoCliente = cliente
                }, connection);

                Fill(connection, model);

                return model;
            }
        }

        public virtual async Task<ReferenciaRecepcion> GetReferenciaOrNull(int idReferencia, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetReferencia(new ReferenciaRecepcion()
                {
                    Id = idReferencia
                }, connection);

                Fill(connection, model);

                return model;
            }
        }

        public virtual void Fill(DbConnection connection, ReferenciaRecepcion model)
        {
            if (model != null)
            {
                model.Detalles = GetDetallesReferencia(connection, new ReferenciaRecepcionDetalle()
                {
                    IdReferencia = model.Id
                });
            }
        }

        public virtual List<ReferenciaRecepcionDetalle> GetDetallesReferencia(DbConnection connection, ReferenciaRecepcionDetalle model)
        {
            var param = new DynamicParameters(new
            {
                idReferencia = model.IdReferencia,
                idLinea = model.IdLineaSistemaExterno,
                producto = model.CodigoProducto,
                empresa = model.IdEmpresa,
                identificador = model.Identificador
            });

            string sql = GetSqlSelectReferenciaRecepcionDetalle() +
                @"WHERE RRD.NU_RECEPCION_REFERENCIA = :idReferencia ";

            if (!string.IsNullOrEmpty(model.IdLineaSistemaExterno))
            {
                sql += @"AND RRD.CD_EMPRESA = :empresa 
                    AND RRD.ID_LINEA_SISTEMA_EXTERNO = :idLinea
                    AND RRD.CD_PRODUTO = :producto 
                    AND RRD.NU_IDENTIFICADOR = :identificador ";
            }

            return _dapper.Query<ReferenciaRecepcionDetalle>(connection, sql, param: param, commandType: CommandType.Text).ToList();
        }

        public virtual IEnumerable<ReferenciaRecepcionDetalle> GetDetallesReferencias(List<ReferenciaRecepcionDetalle> detalles)
        {
            IEnumerable<ReferenciaRecepcionDetalle> resultado = new List<ReferenciaRecepcionDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_RECEPCION_REFERENCIA_DET_TEMP (NU_RECEPCION_REFERENCIA, ID_LINEA_SISTEMA_EXTERNO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR) 
                        VALUES (:IdReferencia, :IdLineaSistemaExterno, :IdEmpresa, :CodigoProducto, :Identificador)";
                    _dapper.Execute(connection, sql, detalles, transaction: tran);

                    sql = GetSqlSelectReferenciaRecepcionDetalle() +
                        @"INNER JOIN T_RECEPCION_REFERENCIA_DET_TEMP T ON RRD.NU_RECEPCION_REFERENCIA = T.NU_RECEPCION_REFERENCIA
                            AND RRD.ID_LINEA_SISTEMA_EXTERNO = T.ID_LINEA_SISTEMA_EXTERNO                            
                            AND RRD.CD_EMPRESA = T.CD_EMPRESA
                            AND RRD.CD_PRODUTO = T.CD_PRODUTO
                            AND RRD.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR";

                    resultado = _dapper.Query<ReferenciaRecepcionDetalle>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<ReferenciaRecepcionDetalle> GetAllDetallesReferencias(IEnumerable<int> referencias, DbConnection connection, DbTransaction tran, bool excluirSobrantes = false, bool saldoDisponible = false)
        {
            IEnumerable<ReferenciaRecepcionDetalle> resultado = new List<ReferenciaRecepcionDetalle>();
            Guid idTemp = Guid.NewGuid();

            if (!referencias.Any())
                return Enumerable.Empty<ReferenciaRecepcionDetalle>();

            var sqlIns = @"INSERT INTO T_RECEPCION_REFERENCIA_TEMP (ID_TEMP, NU_RECEPCION_REFERENCIA) VALUES (:idTemp, :nuReferencia)";
            var keys = referencias.Select(id => new
            {
                idTemp = idTemp.ToString(),
                nuReferencia = id
            });

            _dapper.Execute(connection, sqlIns, keys, transaction: tran);

            var sqlSel = GetSqlSelectReferenciaRecepcionDetalle() +
               @"INNER JOIN T_RECEPCION_REFERENCIA_TEMP T ON RRD.NU_RECEPCION_REFERENCIA = T.NU_RECEPCION_REFERENCIA 
                WHERE T.ID_TEMP = :idTemp ";

            if (excluirSobrantes)
            {
                sqlSel += @"AND RRD.QT_REFERENCIA > 0 ";
            }

            if (saldoDisponible)
            {
                sqlSel += @"AND (COALESCE(RRD.QT_REFERENCIA,0) - COALESCE(RRD.QT_AGENDADA,0) - COALESCE(RRD.QT_RECIBIDA,0) - COALESCE(RRD.QT_ANULADA,0)) > 0 ";
            }

            resultado = _dapper.Query<ReferenciaRecepcionDetalle>(connection, sqlSel, param: new
            {
                idTemp = idTemp.ToString()
            }, transaction: tran);

            var sqlDel = @"DELETE FROM T_RECEPCION_REFERENCIA_TEMP WHERE ID_TEMP = :idTemp AND NU_RECEPCION_REFERENCIA = :nuReferencia";
            _dapper.Execute(connection, sqlDel, keys, transaction: tran);

            return resultado;
        }

        public virtual async Task<ReferenciaRecepcionDetalle> GetDetalleReferencia(int idReferenciaDetalle, CancellationToken cancelToken = default)
        {
            string sql = GetSqlSelectReferenciaRecepcionDetalle() +
                @"WHERE RRD.NU_RECEPCION_REFERENCIA_DET = :idReferenciaDetalle ";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                return _dapper.Query<ReferenciaRecepcionDetalle>(connection, sql, param: new { idReferenciaDetalle = idReferenciaDetalle }, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public static string GetSqlSelectReferenciaRecepcionDetalle()
        {
            return @"SELECT 
                        RRD.CD_EMPRESA as IdEmpresa,
                        RRD.CD_FAIXA as Faixa,
                        RRD.CD_PRODUTO as CodigoProducto,
                        RRD.DS_ANEXO1 as Anexo1,
                        RRD.DT_ADDROW as FechaInsercion,
                        RRD.DT_UPDROW as FechaModificacion,
                        RRD.DT_VENCIMIENTO as FechaVencimiento,
                        RRD.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                        RRD.IM_UNITARIO as ImporteUnitario,
                        RRD.NU_IDENTIFICADOR as Identificador,
                        RRD.NU_RECEPCION_REFERENCIA as IdReferencia,
                        RRD.NU_RECEPCION_REFERENCIA_DET as Id,
                        RRD.QT_AGENDADA as CantidadAgendada,
                        RRD.QT_ANULADA as CantidadAnulada,
                        RRD.QT_CONFIRMADA_INTERFAZ as CantidadConfirmadaInterfaz,
                        RRD.QT_RECIBIDA as CantidadRecibida,
                        RRD.QT_REFERENCIA as CantidadReferencia
                    FROM T_RECEPCION_REFERENCIA_DET RRD ";
        }

        public virtual async Task AddReferencias(List<ReferenciaRecepcion> referencias, IReferenciaRecepcionServiceContext context, CancellationToken cancelToken = default)
        {
            await AddReferencias(GetBulkOperationContext(referencias, context), cancelToken);
        }

        public virtual async Task AddReferencias(ReferenciaRecepcionBulkOperationContext context, CancellationToken cancelToken)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertReferencias(connection, tran, context.NewReferencias);
                    await BulkInsertDetalles(connection, tran, context.NewDetalles);

                    tran.Commit();
                }
            }
        }

        public virtual ReferenciaRecepcionBulkOperationContext GetBulkOperationContext(List<ReferenciaRecepcion> referencias, IReferenciaRecepcionServiceContext serviceContext)
        {
            var context = new ReferenciaRecepcionBulkOperationContext();

            foreach (var referencia in referencias)
            {
                context.NewReferencias.Add(GetReferenciaEntity(referencia));

                foreach (var linea in referencia.Detalles)
                {
                    linea.NumeroTransaccion = referencia.NumeroTransaccion;

                    context.NewDetalles.Add(GetReferenciaDetalleEntity(linea, referencia));
                }
            }

            return context;
        }

        public virtual object GetReferenciaEntity(ReferenciaRecepcion referencia)
        {
            return new
            {
                Referencia = referencia.Numero,
                IdReferencia = referencia.Id,
                Empresa = referencia.IdEmpresa,
                Anexo1 = referencia.Anexo1,
                Anexo2 = referencia.Anexo2,
                Anexo3 = referencia.Anexo3,
                Predio = referencia.IdPredio,
                Memo = referencia.Memo,
                Cliente = referencia.CodigoCliente,
                FechaInsercion = DateTime.Now,
                Situacion = SituacionDb.Activo,
                Moneda = referencia.Moneda,
                Serializado = referencia.Serializado,
                FechaEntrega = referencia.FechaEntrega,
                FechaEmitida = referencia.FechaEmitida,
                TipoReferencia = referencia.TipoReferencia,
                EstadoReferencia = EstadoReferenciaRecepcionDb.Abierta,
                FechaVencimientoOrden = referencia.FechaVencimientoOrden,
                Transaccion = referencia.NumeroTransaccion,
            };
        }

        public virtual async Task BulkInsertReferencias(DbConnection connection, DbTransaction tran, List<object> referencias)
        {
            string sql = @"INSERT INTO T_RECEPCION_REFERENCIA 
                    (CD_CLIENTE, 
                    CD_EMPRESA,
                    CD_MONEDA,
                    CD_SITUACAO,
                    DS_ANEXO1,
                    DS_ANEXO2,
                    DS_ANEXO3,
                    DS_MEMO,
                    DT_ADDROW,
                    DT_EMITIDA,
                    DT_ENTREGA,
                    DT_VENCIMIENTO_ORDEN,
                    ND_ESTADO_REFERENCIA,
                    NU_PREDIO,
                    NU_RECEPCION_REFERENCIA,
                    NU_REFERENCIA,
                    TP_REFERENCIA,
                    VL_SERIALIZADO,
                    NU_TRANSACCION) 
                    VALUES 
                    (:Cliente,
                    :Empresa,
                    :Moneda,
                    :Situacion,
                    :Anexo1,
                    :Anexo2,
                    :Anexo3,
                    :Memo,
                    :FechaInsercion,
                    :FechaEmitida,
                    :FechaEntrega,
                    :FechaVencimientoOrden,
                    :EstadoReferencia,
                    :Predio,
                    :IdReferencia,
                    :Referencia,
                    :TipoReferencia,
                    :Serializado,
                    :Transaccion)";

            await _dapper.ExecuteAsync(connection, sql, referencias, transaction: tran);
        }

        public virtual object GetReferenciaDetalleEntity(ReferenciaRecepcionDetalle detalle, ReferenciaRecepcion referencia)
        {
            return new
            {
                Empresa = detalle.IdEmpresa,
                Faixa = detalle.Faixa,
                CodigoProducto = detalle.CodigoProducto,
                Anexo1 = detalle.Anexo1,
                FechaInsercion = DateTime.Now,
                FechaVencimiento = detalle.FechaVencimiento,
                IdLineaSistemaExterno = detalle.IdLineaSistemaExterno,
                ImporteUnitario = detalle.ImporteUnitario,
                Identificador = detalle.Identificador.Trim(),
                IdReferencia = detalle.IdReferencia,
                IdReferenciaDetalle = detalle.Id,
                CantidadReferencia = detalle.CantidadReferencia,
                CantidadAgendada = 0,
                CantidadAnulada = 0,
                CantidadRecibida = 0,
                Referencia = referencia.Numero,
                Cliente = referencia.CodigoCliente,
                TipoReferencia = referencia.TipoReferencia,
                Transaccion = referencia.NumeroTransaccion,
            };
        }

        public virtual async Task BulkInsertDetalles(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            var sql = @" INSERT INTO T_RECEPCION_REFERENCIA_DET
                            (CD_EMPRESA,
                            CD_FAIXA,
                            CD_PRODUTO,
                            DS_ANEXO1,
                            DT_ADDROW,
                            DT_VENCIMIENTO,
                            ID_LINEA_SISTEMA_EXTERNO,
                            IM_UNITARIO,
                            NU_IDENTIFICADOR,
                            NU_RECEPCION_REFERENCIA,
                            NU_RECEPCION_REFERENCIA_DET,
                            QT_REFERENCIA,
                            QT_ANULADA,
                            QT_RECIBIDA,
                            QT_AGENDADA,
                            NU_TRANSACCION) 
                        SELECT 
                            :Empresa,
                            :Faixa,
                            :CodigoProducto,
                            :Anexo1,
                            :FechaInsercion,
                            :FechaVencimiento,
                            :IdLineaSistemaExterno,
                            :ImporteUnitario,
                            :Identificador,
                            RR.NU_RECEPCION_REFERENCIA,
                            :IdReferenciaDetalle,
                            :CantidadReferencia,
                            :CantidadAgendada,
                            :CantidadAnulada,
                            :CantidadRecibida,
                            :Transaccion 
                        FROM T_RECEPCION_REFERENCIA RR
                        WHERE RR.NU_REFERENCIA = :Referencia
                            AND RR.CD_EMPRESA = :Empresa
                            AND RR.TP_REFERENCIA = :TipoReferencia
                            AND RR.CD_CLIENTE = :Cliente";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }


        public virtual async Task ModificarReferencias(List<ReferenciaRecepcion> referencias, IModificarDetalleReferenciaServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                // The .NET Framework Data Provider for Oracle only supports ReadCommitted and Serializable
                using (var tran = connection.BeginTransaction(this._dapper.GetSnapshotIsolationLevel()))
                {
                    var bulkContext = GetBulkModificarDetalleContext(referencias, context, connection, tran);

                    await BulkAnularDetalles(connection, tran, bulkContext.AnuDetalles);
                    await BulkModificarDetalles(connection, tran, bulkContext.ModDetalles);
                    await BulkInsertDetalles(connection, tran, bulkContext.NewDetalles);

                    tran.Commit();
                }
            }
        }

        public virtual ModificarReferenciaBulkOperationContext GetBulkModificarDetalleContext(List<ReferenciaRecepcion> referencias, IModificarDetalleReferenciaServiceContext serviceContext, DbConnection connection, DbTransaction tran)
        {
            var context = new ModificarReferenciaBulkOperationContext();
            var detalles = new Dictionary<string, ReferenciaRecepcionDetalle>();

            foreach (var d in GetAllDetallesReferencias(serviceContext.ReferenciaIds, connection, tran))
            {
                var key = $"{d.IdReferencia}.{d.IdLineaSistemaExterno}.{d.IdEmpresa}.{d.CodigoProducto}.{d.Identificador}";
                detalles[key] = d;
            }

            foreach (var referencia in referencias)
            {
                foreach (var detalle in referencia.Detalles)
                {
                    var key = $"{detalle.IdReferencia}.{detalle.IdLineaSistemaExterno}.{detalle.IdEmpresa}.{detalle.CodigoProducto}.{detalle.Identificador}";

                    detalle.NumeroTransaccion = referencia.NumeroTransaccion;

                    if (detalles.ContainsKey(key))
                    {
                        var cantidadOperacion = detalle.CantidadReferencia ?? 0; // Para el objeto detalle la CantidadReferencia equivale a la cantidad operacion enviada en la api
                        var detalleRef = detalles[key];

                        decimal saldo = (detalleRef.CantidadReferencia ?? 0) - (detalleRef.CantidadAnulada ?? 0) - (detalleRef.CantidadAgendada ?? 0) - (detalleRef.CantidadRecibida ?? 0);
                        if (saldo < 0)
                            saldo = 0;

                        if (detalle.TipoOperacionId == TipoOperacionReferencia.Anular && saldo > 0)
                        {
                            var cantidadAnular = (detalleRef.CantidadAnulada ?? 0) + saldo;
                            context.AnuDetalles.Add(GetDetalleEntity(detalle, cantidadAnular));

                        }
                        else if (detalle.TipoOperacionId == TipoOperacionReferencia.Modificar && saldo > 0)
                        {
                            decimal cantidad = 0;

                            if (saldo >= detalle.CantidadReferencia)
                                cantidad = (detalleRef.CantidadReferencia ?? 0) - cantidadOperacion;
                            else
                                cantidad = (detalleRef.CantidadReferencia ?? 0) - saldo;

                            context.ModDetalles.Add(GetDetalleEntity(detalle, cantidad));
                        }
                        else
                        {
                            decimal consumido = (detalleRef.CantidadAnulada ?? 0) + (detalleRef.CantidadAgendada ?? 0) + (detalleRef.CantidadRecibida ?? 0);
                            if (cantidadOperacion >= consumido)
                                context.ModDetalles.Add(GetDetalleEntity(detalle, cantidadOperacion));
                        }
                    }
                    else if (detalle.TipoOperacionId == TipoOperacionReferencia.Nuevo)
                    {
                        context.NewDetalles.Add(GetReferenciaDetalleEntity(detalle, referencia));
                    }
                }
            }

            return context;
        }


        public virtual async Task AnularReferencias(List<ReferenciaRecepcion> referencias, IAnularReferenciaServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                // The .NET Framework Data Provider for Oracle only supports ReadCommitted and Serializable
                using (var tran = connection.BeginTransaction(this._dapper.GetSnapshotIsolationLevel()))
                {
                    var bulkContext = GetBulkAnularReferenciaContext(referencias, context, connection, tran);
                    await BulkAnularDetalles(connection, tran, bulkContext.UpdDetalles);
                    await BulkUpdateSituacion(connection, tran, bulkContext.UpdReferencias);

                    tran.Commit();
                }
            }
        }

        public virtual AnularReferenciaBulkOperationContext GetBulkAnularReferenciaContext(List<ReferenciaRecepcion> referencias, IAnularReferenciaServiceContext serviceContext, DbConnection connection, DbTransaction tran)
        {
            var context = new AnularReferenciaBulkOperationContext();
            var detalles = new Dictionary<int, List<ReferenciaRecepcionDetalle>>();

            foreach (var d in GetAllDetallesReferencias(serviceContext.ReferenciaIds, connection, tran, true))
            {
                if (!detalles.ContainsKey(d.IdReferencia))
                {
                    detalles[d.IdReferencia] = new List<ReferenciaRecepcionDetalle>();
                }
                detalles[d.IdReferencia].Add(d);
            }

            foreach (var referencia in referencias)
            {
                if (detalles.ContainsKey(referencia.Id))
                {
                    foreach (var detalle in detalles[referencia.Id])
                    {
                        var cantidadAnular = (detalle.CantidadReferencia ?? 0) - (detalle.CantidadRecibida ?? 0);

                        detalle.NumeroTransaccion = referencia.NumeroTransaccion;
                        detalle.NumeroTransaccionDelete = referencia.NumeroTransaccionDelete;

                        context.UpdDetalles.Add(GetDetalleEntity(detalle, cantidadAnular));
                    }
                }

                context.UpdReferencias.Add(new
                {
                    referencia = referencia.Id,
                    situacion = SituacionDb.AnuladoCompletamente,
                    estado = EstadoReferenciaRecepcionDb.Cancelada,
                    modificacion = DateTime.Now,
                    transaccion = referencia.NumeroTransaccion,
                });
            }

            return context;
        }

        public virtual async Task BulkAnularDetalles(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            string sql = @"UPDATE T_RECEPCION_REFERENCIA_DET SET QT_ANULADA = :cantidadOperacion, DT_UPDROW = :Updrow, NU_TRANSACCION = :transaccion 
                WHERE NU_RECEPCION_REFERENCIA = :idReferencia AND CD_EMPRESA = :empresa AND ID_LINEA_SISTEMA_EXTERNO = :idLinea
                    AND CD_PRODUTO = :producto AND NU_IDENTIFICADOR = :identificador";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual async Task BulkModificarDetalles(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            string sql = @"UPDATE T_RECEPCION_REFERENCIA_DET SET QT_REFERENCIA = :cantidadOperacion, DT_UPDROW = :Updrow, NU_TRANSACCION = :transaccion 
                WHERE NU_RECEPCION_REFERENCIA = :idReferencia AND CD_EMPRESA = :empresa AND ID_LINEA_SISTEMA_EXTERNO = :idLinea
                    AND CD_PRODUTO = :producto AND NU_IDENTIFICADOR = :identificador";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual async Task BulkUpdateSituacion(DbConnection connection, DbTransaction tran, List<object> referencias)
        {
            string sql = @"UPDATE T_RECEPCION_REFERENCIA SET CD_SITUACAO = :situacion, 
                            DT_UPDROW = :modificacion,
                            ND_ESTADO_REFERENCIA = :estado,
                            NU_TRANSACCION = :transaccion 
                        WHERE NU_RECEPCION_REFERENCIA = :referencia";

            await _dapper.ExecuteAsync(connection, sql, referencias, transaction: tran);
        }

        public static object GetDetalleEntity(ReferenciaRecepcionDetalle detalle, decimal cantidadOperacion)
        {
            return new
            {
                idReferencia = detalle.IdReferencia,
                idLinea = detalle.IdLineaSistemaExterno,
                producto = detalle.CodigoProducto,
                empresa = detalle.IdEmpresa,
                identificador = detalle.Identificador,
                cantidadOperacion = cantidadOperacion,
                Updrow = DateTime.Now,
                transaccion = detalle.NumeroTransaccion,
            };
        }

        public virtual IEnumerable<int> GetReferenciasEnUso(IEnumerable<int> referencias)
        {
            IEnumerable<int> resultado = new List<int>();
            Guid idTemp = Guid.NewGuid();

            if (!referencias.Any())
                return Enumerable.Empty<int>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    var sqlIns = @"INSERT INTO T_RECEPCION_REFERENCIA_TEMP (ID_TEMP, NU_RECEPCION_REFERENCIA) VALUES (:idTemp, :nuReferencia)";
                    _dapper.Execute(connection, sqlIns, referencias.Select(id => new
                    {
                        idTemp = idTemp.ToString(),
                        nuReferencia = id
                    }), transaction: tran);

                    var sqlSel = @"SELECT DISTINCT RRD.NU_RECEPCION_REFERENCIA 
                    FROM T_RECEPCION_REFERENCIA_DET RRD
                    INNER JOIN T_RECEPCION_REFERENCIA_TEMP T ON RRD.NU_RECEPCION_REFERENCIA = T.NU_RECEPCION_REFERENCIA  
                    WHERE T.ID_TEMP = :idTemp AND RRD.QT_AGENDADA > 0";

                    resultado = _dapper.Query<int>(connection, sqlSel, param: new { idTemp = idTemp.ToString() }, commandType: CommandType.Text, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<int> GetReferenciasConSaldo(IEnumerable<int> referencias)
        {
            IEnumerable<int> resultado = new List<int>();
            Guid idTemp = Guid.NewGuid();

            if (!referencias.Any())
                return Enumerable.Empty<int>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    var sqlIns = @"INSERT INTO T_RECEPCION_REFERENCIA_TEMP (ID_TEMP, NU_RECEPCION_REFERENCIA) VALUES (:idTemp, :nuReferencia)";
                    _dapper.Execute(connection, sqlIns, referencias.Select(id => new
                    {
                        idTemp = idTemp.ToString(),
                        nuReferencia = id
                    }), transaction: tran);

                    var sqlSel = @"SELECT RD.NU_RECEPCION_REFERENCIA AS Id 
                        FROM V_REC170_REFERENCIAS_DISPONIB RD
                        INNER JOIN T_RECEPCION_REFERENCIA_TEMP T ON RD.NU_RECEPCION_REFERENCIA = T.NU_RECEPCION_REFERENCIA
                        WHERE T.ID_TEMP = :idTemp
                            AND (RD.DT_VENCIMIENTO_ORDEN >= :fecha OR RD.DT_VENCIMIENTO_ORDEN IS NULL)";

                    resultado = _dapper.Query<int>(connection, sqlSel, param: new
                    {
                        idTemp = idTemp.ToString(),
                        fecha = DateTime.Today
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion
    }
}
