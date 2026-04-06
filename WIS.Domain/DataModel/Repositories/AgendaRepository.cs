using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Dtos;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AgendaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly AgendaMapper _mapper;
        protected readonly IDapper _dapper;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly ParametroRepository _parametroRepository;

        public AgendaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new AgendaMapper();
            this._dapper = dapper;
            this._parametroRepository = new ParametroRepository(_context, cdAplicacion, userId, dapper);
        }

        #region Any

        public virtual bool IsAgendaDocumentable(int empresa, int agenda)
        {
            return this._context.T_AGENDA
                .AsNoTracking()
                .Any(a => a.CD_EMPRESA == empresa
                    && a.NU_AGENDA == agenda
                    && a.CD_SITUACAO == EstadoAgendaDb.Abierta
                    && a.TP_RECEPCION == TipoRecepcionDb.DocumentosAduaneros
                    && string.IsNullOrWhiteSpace(a.NU_DOCUMENTO));
        }

        public virtual bool IsAgendaFacturaValida(int nuAgenda)
        {
            return this._context.T_AGENDA
                .Any(x => x.NU_AGENDA == nuAgenda && x.FL_FACTURA_VALIDA == "S");
        }

        public virtual bool AnyAgenda(int nroAgenda)
        {
            return this._context.T_AGENDA.AsNoTracking().Any(a => a.NU_AGENDA == nroAgenda);
        }

        public virtual bool AgendaManejaFactura(int idAgenda, int idEmpresa)
        {
            return this._context.T_AGENDA
                .AsNoTracking()
                .Any(w => w.NU_AGENDA == idAgenda
                    && w.CD_EMPRESA == idEmpresa
                    && this._context.T_RECEPCION_TIPO.Any(a => a.TP_RECEPCION == w.TP_RECEPCION && a.FL_INGRESO_FACTURA == "S"));
        }

        public virtual bool AnyProblemaAgenda(int numeroAgenda)
        {
            return this._context.T_RECEPCION_AGENDA_PROBLEMA
                .AsNoTracking()
                .Any(x => x.NU_AGENDA == numeroAgenda && x.FL_ACEPTADO == "N");
        }

        public virtual bool AgendaTuvoProblemas(int numeroAgenda)
        {
            return this._context.T_RECEPCION_AGENDA_PROBLEMA.AsNoTracking().Any(x => x.NU_AGENDA == numeroAgenda);
        }

        public virtual bool AnyProblemaAgendaDetalle(int numeroAgenda, string codigoProducto, decimal faixa, string identificador)
        {
            if (this._context.T_RECEPCION_AGENDA_PROBLEMA
                .AsNoTracking()
                .Any(x => x.NU_AGENDA == numeroAgenda
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.FL_ACEPTADO == "N"))
                return true;

            if (this._context.T_RECEPCION_AGENDA_PROBLEMA.Local
                .Any(x => x.NU_AGENDA == numeroAgenda
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.FL_ACEPTADO == "N"))
                return true;

            return false;

        }

        public virtual bool AnyProblemaAgendaDetalleLocal(int numeroAgenda, string codigoProducto, decimal faixa, string identificador)
        {
            if (this._context.T_RECEPCION_AGENDA_PROBLEMA.Local.Any(x => x.NU_AGENDA == numeroAgenda
                                                                             && x.CD_PRODUTO == codigoProducto
                                                                             && x.CD_FAIXA == faixa
                                                                             && x.NU_IDENTIFICADOR == identificador
                                                                             && x.FL_ACEPTADO == "N"))
                return true;

            return false;
        }

        public virtual bool AnyAgendaDetalleConEstados(int numeroAgenda, List<EstadoAgenda> estados)
        {
            List<short> situaciones = new List<short>();

            estados.ForEach(estado =>
            {
                situaciones.Add(this._mapper.MapEstado(estado));
            });

            var detalles = this._context.T_DET_AGENDA.Local
                .Where(x => x.NU_AGENDA == numeroAgenda)
                .ToList();
            var detallesEntity = this._context.T_DET_AGENDA
                .AsNoTracking()
                .Where(x => x.NU_AGENDA == numeroAgenda)
                .AsEnumerable()
                .Where(x => !detalles.Any(z => z.NU_AGENDA == x.NU_AGENDA
                    && z.CD_EMPRESA == x.CD_EMPRESA
                    && z.CD_PRODUTO == x.CD_PRODUTO
                    && z.NU_IDENTIFICADOR == x.NU_IDENTIFICADOR
                    && z.CD_FAIXA == x.CD_FAIXA));

            detalles.AddRange(detallesEntity);

            if (detalles.Any(x => situaciones.Contains((x.CD_SITUACAO ?? 0))))
                return true;

            return false;
        }

        public virtual bool AnyAgendaDetalle(int idAgenda, int idEmpresa, string codigoProducto, decimal faixa, string identificador)
        {
            return this._context.T_DET_AGENDA.AsNoTracking().Any(x => x.NU_AGENDA == idAgenda
                                                                             && x.CD_PRODUTO == codigoProducto
                                                                             && x.CD_FAIXA == faixa
                                                                             && x.NU_IDENTIFICADOR == identificador
                                                                             && x.CD_EMPRESA == idEmpresa);
        }

        public virtual bool AnyDetalleLoteNoAuto(int idAgenda)
        {
            return _context.T_DET_AGENDA.AsNoTracking().Any(a => a.NU_AGENDA == idAgenda && a.NU_IDENTIFICADOR != ManejoIdentificadorDb.IdentificadorAuto);
        }

        public virtual bool AnyPlanificacionLpn(int idAgenda)
        {
            return _context.T_AGENDA_LPN_PLANIFICACION.AsNoTracking().Any(a => a.NU_AGENDA == idAgenda);
        }

        #endregion

        #region Get

        public virtual Agenda GetAgenda(int nroAgenda)
        {
            T_AGENDA agenda = this._context.T_AGENDA.AsNoTracking()
                .Include("T_DET_AGENDA")
                .Where(d => d.NU_AGENDA == nroAgenda)
                .AsNoTracking()
                .FirstOrDefault();

            T_RECEPCION_TIPO tpRecepcion = null;
            if (agenda != null)
                tpRecepcion = _context.T_RECEPCION_TIPO.FirstOrDefault(r => r.TP_RECEPCION == agenda.TP_RECEPCION);

            return this._mapper.MapAgendaToObject(agenda, tpRecepcion);
        }

        public virtual Agenda GetAgendaSinDetalles(int nroAgenda)
        {
            T_AGENDA agenda = this._context.T_AGENDA
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_AGENDA == nroAgenda);

            T_RECEPCION_TIPO tpRecepcion = null;
            if (agenda != null)
                tpRecepcion = _context.T_RECEPCION_TIPO.FirstOrDefault(r => r.TP_RECEPCION == agenda.TP_RECEPCION);

            return this._mapper.MapAgendaToObject(agenda, tpRecepcion);
        }

        public virtual Agenda GetAgenda(string nuDocumento, string tipoDocumento)
        {
            var documento = string.Format("{0}{1}", tipoDocumento, nuDocumento);

            var agenda = this._context.T_AGENDA
                .Include("T_DET_AGENDA")
                .AsNoTracking()
                .FirstOrDefault(s => s.NU_DOCUMENTO == documento
                    && s.CD_SITUACAO != EstadoAgendaDb.Cancelada);

            T_RECEPCION_TIPO tpRecepcion = null;
            if (agenda != null)
                tpRecepcion = _context.T_RECEPCION_TIPO.FirstOrDefault(r => r.TP_RECEPCION == agenda.TP_RECEPCION);

            return this._mapper.MapAgendaToObject(agenda, tpRecepcion);
        }

        public virtual List<Agenda> GetAgendasDocumentables(int empresa)
        {
            return this._context.T_AGENDA
                .AsNoTracking()
                .Where(a => a.CD_EMPRESA == empresa
                    && a.CD_SITUACAO == EstadoAgendaDb.Abierta
                    && a.TP_RECEPCION == TipoRecepcionDb.DocumentosAduaneros
                    && string.IsNullOrWhiteSpace(a.NU_DOCUMENTO))
                .OrderBy(a => a.NU_AGENDA)
                .Select(a => this._mapper.MapAgendaToObject(a, null))
                .ToList();
        }

        public virtual Agenda GetAgendaConDetalleProblemas(int nroAgenda)
        {
            var agenda = this.GetAgenda(nroAgenda);

            if (agenda == null)
                return null;

            // Levantar problemas de agendas

            foreach (var detalle in agenda.Detalles)
            {
                detalle.ProblemasRecepcion.AddRange(this.GetAgendaDetalleProblemas(agenda.Id, detalle.CodigoProducto, detalle.Identificador, detalle.Faixa));
            }

            return agenda;
        }

        public virtual string GetAgendaTipoRecepcionInterno(int numeroAgenda)
        {
            string tipo = this._context.T_AGENDA.AsNoTracking().FirstOrDefault(d => d.NU_AGENDA == numeroAgenda).TP_RECEPCION;

            return tipo;
        }

        public virtual Agenda GetAgendaSinPorteriaByMatriculaVehiculo(string matricula)
        {
            T_AGENDA entity = (from p in _context.T_AGENDA
                               join co in _context.T_PORTERIA_VEHICULO_AGENDA on p.NU_AGENDA equals co.NU_AGENDA into CamionPorteria
                               from pco in CamionPorteria.DefaultIfEmpty()
                               where p.DS_PLACA == matricula && pco == null
                               select p).FirstOrDefault();

            return entity == null ? null : _mapper.MapAgendaToObject(entity, null);
        }

        public virtual List<Agenda> GetAgendasByMatriculaVehiculo(string matricula)
        {
            return (from p in _context.T_AGENDA
                    join co in _context.T_PORTERIA_VEHICULO_AGENDA on p.NU_AGENDA equals co.NU_AGENDA into CamionPorteria
                    from pco in CamionPorteria.DefaultIfEmpty()
                    where p.DS_PLACA == matricula && pco == null
                    select p)
                    .ToList()
                    .Select(w => this._mapper.MapAgendaToObject(w, null)).ToList();
        }

        public virtual List<short> GetContainersAgendaParaEntradaByMatricula(string matricula)
        {
            return (from p in _context.T_AGENDA
                    join co in _context.T_PORTERIA_VEHICULO_AGENDA on p.NU_AGENDA equals co.NU_AGENDA into CamionPorteria
                    join rel in _context.T_RECEPC_AGENDA_CONTAINER_REL on p.NU_AGENDA equals rel.NU_AGENDA
                    from pco in CamionPorteria.DefaultIfEmpty()
                    where p.DS_PLACA == matricula && pco == null
                    select rel.NU_SEQ_CONTAINER)
                   .Distinct()
                   .ToList();
        }

        public virtual decimal GetCantidadDisponible(int nroAgenda, int cdEmpresa, string cdProducto, decimal cdFaixa, string nroIdentificador)
        {
            return _context.T_DET_AGENDA
                .Where(d => d.NU_AGENDA == nroAgenda
                    && d.CD_EMPRESA == cdEmpresa
                    && d.CD_PRODUTO == cdProducto
                    && d.CD_FAIXA == cdFaixa
                    && (d.NU_IDENTIFICADOR == nroIdentificador || d.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto))
                .Sum(w => w.QT_AGENDADO - (w.QT_RECIBIDA ?? 0)) ?? 0;
        }

        public virtual EstadoAgenda GetAgendaEstado(int numeroAgenda)
        {
            short? estado = this._context.T_AGENDA.AsNoTracking().FirstOrDefault(d => d.NU_AGENDA == numeroAgenda).CD_SITUACAO;

            return this._mapper.MapEstado((short)estado);
        }

        public virtual List<int> GetAgendasByEmpresa(int empresa)
        {
            return (from a in _context.V_CROSS_DOCK_TEMP_WREC220.AsNoTracking()
                    where a.CD_EMPRESA == empresa
                    select a.NU_AGENDA).Distinct()
                    .ToList();
        }

        public virtual decimal GetSaldoAgendaDetalle(int id, int empresa, string producto, decimal faixa, string identificadorAuto)
        {
            var detalleAgenda = this._context.T_DET_AGENDA
               .Where(d => d.NU_AGENDA == id
                   && d.CD_EMPRESA == empresa
                   && d.CD_PRODUTO == producto
                   && d.CD_FAIXA == faixa
                   && d.NU_IDENTIFICADOR == identificadorAuto)
               .FirstOrDefault();
            if (detalleAgenda == null)
            {
                return 0;
            }
            decimal? saldo;

            if ((detalleAgenda.QT_RECIBIDA ?? 0) > (detalleAgenda.QT_CROSS_DOCKING ?? 0))
            {
                saldo = detalleAgenda.QT_AGENDADO - (detalleAgenda.QT_RECIBIDA ?? 0);
            }
            else
            {
                saldo = detalleAgenda.QT_AGENDADO - (detalleAgenda.QT_CROSS_DOCKING ?? 0);
            }

            return (saldo ?? 0);

        }

        public virtual List<SaldoReferenciaExpedidosExcel> GetAgendaReferenciaExel(int nroAgenda)
        {
            List<V_EVENTO_SALDO_REF> agendaRef = this._context.V_EVENTO_SALDO_REF.AsNoTracking().Where(d => d.NU_AGENDA == nroAgenda).ToList();

            if (agendaRef == null)
                return null;

            return this._mapper.MapAgendaRefEntityToObject(agendaRef);
        }

        public virtual List<SaldoAgendaExportExcel> GetAgendaSaldoExel(int nroAgenda)
        {
            List<V_EVENTO_SALDO_FAC> agendaRef = this._context.V_EVENTO_SALDO_FAC.AsNoTracking().Where(d => d.NU_AGENDA == nroAgenda).ToList();

            if (agendaRef == null)
                return null;

            return this._mapper.MapAgendaFacEntityToObject(agendaRef);
        }

        public virtual AgendaDetalleProblema GetAgendaProblema(int numProblema)
        {
            T_RECEPCION_AGENDA_PROBLEMA agnProb = _context.T_RECEPCION_AGENDA_PROBLEMA
                .FirstOrDefault(x => x.NU_RECEPCION_AGENDA_PROBLEMA == numProblema);

            return (this._mapper.MapAgendaDetalleProblemaEntityToObject(agnProb));
        }

        public virtual AgendaDetalleProblema GetAgendaDetalleProblemaSinAceptar(TipoProblemaAgendaDetalle tipoProblema, ProblemaAgendaDetalle problema, int numeroAgenda, string codigoProducto, string identificador, decimal faixa)
        {
            string auxTipoProblema = _mapper.MapTipoProblemaDetalle(tipoProblema);
            string auxProblema = _mapper.MapProblemaDetalle(problema);

            var problemaDetalle = _context.T_RECEPCION_AGENDA_PROBLEMA
                .AsNoTracking()
                .FirstOrDefault(x => x.FL_ACEPTADO == "N"
                    && x.NU_AGENDA == numeroAgenda
                    && x.ND_TIPO == auxTipoProblema
                    && x.ND_PROBLEMA == auxProblema
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador);

            return this._mapper.MapAgendaDetalleProblemaEntityToObject(problemaDetalle);
        }

        public virtual List<AgendaDetalleProblema> GetAgendaDetalleProblemas(int numeroAgenda, string codigoProducto, string identificador, decimal faixa)
        {
            List<AgendaDetalleProblema> problemas = new List<AgendaDetalleProblema>();

            var entidades = _context.T_RECEPCION_AGENDA_PROBLEMA
                .Where(x => x.NU_AGENDA == numeroAgenda
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador)
                .ToList();

            foreach (var entidad in entidades)
            {
                problemas.Add(this._mapper.MapAgendaDetalleProblemaEntityToObject(entidad));
            }

            return problemas;
        }

        public virtual AgendaDetalle GetAgendaDetalle(int numeroAgenda, int idEmpresa, string codigoProducto, decimal faixa, string identificador)
        {

            var detalle = this._context.T_DET_AGENDA.AsNoTracking().Where(d => d.NU_AGENDA == numeroAgenda
                                                                             && d.CD_EMPRESA == idEmpresa
                                                                             && d.CD_PRODUTO == codigoProducto
                                                                             && d.CD_FAIXA == faixa
                                                                             && d.NU_IDENTIFICADOR == identificador)
                                                                    .FirstOrDefault();
            if (detalle == null)
                return null;

            return this._mapper.MapAgendaDetalleToObject(detalle);

        }

        public virtual List<AgendaDetalle> GetAgendaDetalleSecundarios(int numeroAgenda, int idEmpresa, string codigoProducto, decimal faixa, string identificador)
        {

            List<T_DET_AGENDA> lineasSecundarias = this._context.T_DET_AGENDA
                .AsNoTracking()
                .Where(da => da.NU_AGENDA == numeroAgenda
                    && da.CD_EMPRESA == idEmpresa
                    && da.CD_PRODUTO == codigoProducto
                    && da.CD_FAIXA == faixa
                    && (da.NU_IDENTIFICADOR != ManejoIdentificadorDb.IdentificadorAuto && da.NU_IDENTIFICADOR != identificador)
                    && ((da.QT_RECIBIDA ?? 0) > (da.QT_AGENDADO ?? 0))
                    && ((da.QT_AGENDADO_ORIGINAL ?? 0) == 0))
                .ToList();

            List<AgendaDetalle> listaDetalles = new List<AgendaDetalle>();

            foreach (var detalle in lineasSecundarias)
            {
                listaDetalles.Add(this._mapper.MapAgendaDetalleToObject(detalle));
            }

            return listaDetalles;
        }

        public virtual AgendaDetalleProblema GetProblemaSinAceptar(ProblemaAgendaDetalle problemaDetalle, TipoProblemaAgendaDetalle tpProblemaDetalle, int nuAgenda, string cdProd, string nuIden, decimal cdFaix, bool loteAuto = false)
        {
            var problema = _mapper.MapProblemaDetalle(problemaDetalle);
            var tpProblema = _mapper.MapTipoProblemaDetalle(tpProblemaDetalle);
            if (!loteAuto)
            {
                return this._mapper.MapAgendaDetalleProblemaEntityToObject(_context.T_RECEPCION_AGENDA_PROBLEMA
                    .FirstOrDefault(x => x.FL_ACEPTADO == "N"
                        && x.NU_AGENDA == nuAgenda
                        && x.ND_PROBLEMA == problema
                        && x.ND_TIPO == tpProblema
                        && x.CD_FAIXA == cdFaix
                        && x.CD_PRODUTO == cdProd
                        && x.NU_IDENTIFICADOR == nuIden
                        && x.NU_LPN == null));
            }
            else
            {
                return this._mapper.MapAgendaDetalleProblemaEntityToObject(_context.T_RECEPCION_AGENDA_PROBLEMA
                    .FirstOrDefault(x => x.FL_ACEPTADO == "N"
                        && x.NU_AGENDA == nuAgenda
                        && x.ND_PROBLEMA == problema
                        && x.ND_TIPO == tpProblema
                        && x.CD_FAIXA == cdFaix
                        && x.CD_PRODUTO == cdProd
                        && (x.NU_IDENTIFICADOR == nuIden || x.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                        && x.NU_LPN == null));
            }
        }

        public virtual List<AgendaDetalle> GetDetallesAgenda(int nroAgenda)
        {
            List<AgendaDetalle> ListaDetalle = new List<AgendaDetalle>();
            var detalles = _context.T_DET_AGENDA.Where(x => x.NU_AGENDA == nroAgenda);
            foreach (var det in detalles)
            {
                ListaDetalle.Add(this._mapper.MapAgendaDetalleToObject(det));
            }
            return ListaDetalle;
        }

        #endregion

        #region Add

        public virtual void AddAgenda(Agenda agenda)
        {
            agenda.Id = this._context.GetNextSequenceValueInt(_dapper, "S_NRO_AGENDA");

            T_AGENDA entity = this._mapper.MapAgendaToEntity(agenda);

            this._context.T_AGENDA.Add(entity);

            if (agenda.Detalles != null)
            {
                foreach (var detalle in agenda.Detalles)
                {
                    detalle.NumeroTransaccion = agenda.NumeroTransaccion;
                    this.AddAgendaDetalle(detalle);
                }
            }
        }

        public virtual void AddAgendaConProblemas(Agenda agenda, List<AgendaDetalleProblema> problemas)
        {
            agenda.Id = this._context.GetNextSequenceValueInt(_dapper, "S_NRO_AGENDA");

            foreach (var detalle in agenda.Detalles)
            {
                detalle.IdAgenda = agenda.Id;
                detalle.FechaAlta = DateTime.Now;
                detalle.FechaModificacion = DateTime.Now;
                detalle.NumeroTransaccion = agenda.NumeroTransaccion;
            }

            T_AGENDA entity = this._mapper.MapAgendaToEntity(agenda);

            foreach (var problema in problemas)
            {
                problema.Id = this._context.GetNextSequenceValueInt(_dapper, "S_NU_REC_AGENDA_PROB");
                problema.NumeroAgenda = agenda.Id;
                problema.FechaAlta = DateTime.Now;
                problema.FechaModificacion = DateTime.Now;
                entity.T_RECEPCION_AGENDA_PROBLEMA.Add(this._mapper.MapAgendaDetalleProblemaToEntity(problema));
            }

            this._context.T_AGENDA.Add(entity);
        }

        public virtual void AddAgendaDetalleProblema(AgendaDetalleProblema problema)
        {
            problema.Id = this._context.GetNextSequenceValueInt(_dapper, "S_NU_REC_AGENDA_PROB");
            problema.FechaAlta = DateTime.Now;
            problema.FechaModificacion = DateTime.Now;

            T_RECEPCION_AGENDA_PROBLEMA entity = this._mapper.MapAgendaDetalleProblemaToEntity(problema);

            this._context.T_RECEPCION_AGENDA_PROBLEMA.Add(entity);
        }

        public virtual void AddAgendaDetalles(List<AgendaDetalle> detalles)
        {
            foreach (var detalle in detalles)
            {
                AddAgendaDetalle(detalle);
            }
        }

        public virtual void AddAgendaDetalle(AgendaDetalle detalle)
        {
            detalle.FechaAlta = DateTime.Now;
            detalle.FechaModificacion = DateTime.Now;

            T_DET_AGENDA entity = this._mapper.MapAgendaDetalleToEntity(detalle);

            this._context.T_DET_AGENDA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateAgenda(Agenda agenda)
        {
            T_AGENDA entity = this._mapper.MapAgendaToEntity(agenda);

            entity.DT_UPDROW = DateTime.Now;

            T_AGENDA attachedEntity = _context.T_AGENDA.Local.FirstOrDefault(w => w.NU_AGENDA == entity.NU_AGENDA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AGENDA.Attach(entity);
                _context.Entry<T_AGENDA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateAgendaSinDependencias(Agenda agenda)
        {
            T_AGENDA entity = this._mapper.MapAgendaToEntity(agenda);

            entity.DT_UPDROW = DateTime.Now;

            T_AGENDA attachedEntity = _context.T_AGENDA.Local.FirstOrDefault(w => w.NU_AGENDA == entity.NU_AGENDA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                entity.T_DET_AGENDA = null;
                _context.T_AGENDA.Attach(entity);
                _context.Entry<T_AGENDA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateAgendaFuncionarioResponsable(Agenda r)
        {
            T_AGENDA entity = _context.T_AGENDA.FirstOrDefault(f =>
             f.NU_AGENDA == r.Id);

            entity.CD_FUN_RESP = r.FuncionarioResponsable;
            entity.DT_FUN_RESP = r.FechaFuncionarioResponsable;
        }

        public virtual void UpdateAgendaDetalleProblema(AgendaDetalleProblema problema)
        {
            problema.FechaModificacion = DateTime.Now;

            T_RECEPCION_AGENDA_PROBLEMA entity = this._mapper.MapAgendaDetalleProblemaToEntity(problema);
            T_RECEPCION_AGENDA_PROBLEMA attachedEntity = _context.T_RECEPCION_AGENDA_PROBLEMA.Local
                .FirstOrDefault(w => w.NU_RECEPCION_AGENDA_PROBLEMA == entity.NU_RECEPCION_AGENDA_PROBLEMA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_RECEPCION_AGENDA_PROBLEMA.Attach(entity);
                _context.Entry<T_RECEPCION_AGENDA_PROBLEMA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateAgendaDetalleProblema(EntityChanges<AgendaDetalleProblema> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                this.RemoveAgendaDetalleProblema(deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                this.AddAgendaDetalleProblema(newRecord);
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                this.UpdateAgendaDetalleProblema(updatedRecord);
            }
        }

        public virtual void UpdateAgendaDetalle(AgendaDetalle detalle)
        {
            detalle.FechaModificacion = DateTime.Now;

            T_DET_AGENDA entity = this._mapper.MapAgendaDetalleToEntity(detalle);
            T_DET_AGENDA attachedEntity = this._context.T_DET_AGENDA.Local
                .Where(d => d.NU_AGENDA == detalle.IdAgenda
                    && d.CD_EMPRESA == detalle.IdEmpresa
                    && d.CD_PRODUTO == detalle.CodigoProducto
                    && d.NU_IDENTIFICADOR == detalle.Identificador
                    && d.CD_FAIXA == detalle.Faixa)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_AGENDA.Attach(entity);
                _context.Entry<T_DET_AGENDA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateAgendaDetalles(EntityChanges<AgendaDetalle> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                this.DeleteAgendaDetalle(deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                this.AddAgendaDetalle(newRecord);
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                this.UpdateAgendaDetalle(updatedRecord);
            }
        }

        public virtual void DesvincularLpns(int nuAgenda, long nuTransaccion)
        {
            int? value = null;
            _context.T_LPN
                .Where(d => d.NU_AGENDA == nuAgenda)
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.NU_AGENDA, value)
                    .SetProperty(d => d.NU_TRANSACCION, nuTransaccion)
                    .SetProperty(d => d.DT_UPDROW, DateTime.Now));
        }

        public virtual void DesvincularFacturas(Agenda agenda, long nuTransaccion)
        {
            var facturas = _context.T_RECEPCION_FACTURA
                .Where(rf => rf.NU_AGENDA == agenda.Id)
                .ToList();

            foreach (var factura in facturas)
            {
                factura.NU_AGENDA = null;
                factura.DT_UPDROW = DateTime.Now;
                factura.NU_TRANSACCION = nuTransaccion;

                _context.T_RECEPCION_FACTURA_DET
                    .Where(d => d.NU_RECEPCION_FACTURA == factura.NU_RECEPCION_FACTURA)
                    .ExecuteUpdate(setters => setters
                        .SetProperty(d => d.QT_VALIDADA, 0)
                        .SetProperty(d => d.NU_TRANSACCION, nuTransaccion)
                        .SetProperty(d => d.DT_UPDROW, DateTime.Now));
            }
        }
        #endregion

        #region Remove

        public virtual void RemoveAgendaDetalleProblema(AgendaDetalleProblema problema)
        {
            var entity = this._mapper.MapAgendaDetalleProblemaToEntity(problema);
            var attachedEntity = _context.T_RECEPCION_AGENDA_PROBLEMA.Local
                .FirstOrDefault(w => w.NU_RECEPCION_AGENDA_PROBLEMA == entity.NU_RECEPCION_AGENDA_PROBLEMA);

            if (attachedEntity != null)
            {
                this._context.T_RECEPCION_AGENDA_PROBLEMA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_RECEPCION_AGENDA_PROBLEMA.Attach(entity);
                this._context.T_RECEPCION_AGENDA_PROBLEMA.Remove(entity);
            }
        }

        public virtual void DeleteAgendaDetalle(AgendaDetalle detalle)
        {
            T_DET_AGENDA entity = this._mapper.MapAgendaDetalleToEntity(detalle);
            T_DET_AGENDA attachedEntity = this._context.T_DET_AGENDA.Local
                .Where(d => d.NU_AGENDA == entity.NU_AGENDA
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && d.CD_FAIXA == entity.CD_FAIXA)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                this._context.T_DET_AGENDA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_AGENDA.Attach(entity);
                this._context.T_DET_AGENDA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual async Task<Agenda> GetAgendaOrNull(int nuAgenda, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetAgenda(new Agenda
                {
                    Id = nuAgenda
                }, connection);

                Fill(connection, model);

                return model;
            }
        }

        public virtual Agenda GetAgenda(Agenda model, DbConnection connection)
        {
            var param = new DynamicParameters(new
            {
                nuAgenda = model.Id
            });

            string sql = @"SELECT 
                        ag.NU_AGENDA as Id,
                        ag.CD_EMPRESA as IdEmpresa,
                        ag.CD_CLIENTE as CodigoInternoCliente,
                        ag.NU_DOCUMENTO as NumeroDocumento,
                        ag.DT_ADDROW as FechaInsercion,
                        ag.DT_UPDROW as FechaModificacion,
                        ag.NU_PREDIO as Predio,
                        ag.CD_FUN_RESP as FuncionarioResponsable,
                        ag.DT_FUN_RESP as FechaFuncionarioResponsable,
                        ag.CD_SITUACAO as EstadoId,
                        ag.CD_TIPO_DOCUMENTO as TipoDocumento,
                        ag.CD_OPERACAO as CodigoOperacion,
                        ag.CD_PORTA as CodigoPuerta,
                        ag.DT_INICIO as FechaInicio,
                        ag.DT_FIN as FechaFin,
                        ag.DS_PLACA as PlacaVehiculo,
                        ag.NU_DUA as DUA,
                        ag.DS_ANEXO1 as Anexo1,
                        ag.DS_ANEXO2 as Anexo2,
                        ag.DS_ANEXO3 as Anexo3,
                        ag.DS_ANEXO4 as Anexo4,
                        ag.ID_ENVIO_DOCUMENTACION as EnviaDocumentacionId,
                        ag.CD_FUNC_ENVIO_DOCU as IdUsuarioEnvioDocumentacion,
                        ag.DT_CIERRE as FechaCierre,
                        ag.DT_ENTREGA as FechaEntrega,
                        ag.TP_RECEPCION as TipoRecepcionInterno,
                        ag.CD_FUNCIONARIO_ASIGNADO as IdFuncionarioAsignado,
                        ag.NU_INTERFAZ_EJECUCION as NumeroInterfazEjecucion,
                        ag.FL_CARGA_AUTO_DETALLE as CargaDetalleAutomaticaId,
                        ag.NU_ORT_ORDEN as NroOrdenTarea,
                        re.FL_MANEJO_INTERFAZ as ManejaInterfazId
                FROM T_AGENDA ag INNER JOIN T_RECEPCION_REL_EMPRESA_TIPO re ON ag.CD_EMPRESA = re.CD_EMPRESA AND ag.TP_RECEPCION = re.TP_RECEPCION 
                WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<Agenda>(connection, sql, param, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void Fill(DbConnection connection, Agenda model)
        {
            if (model != null)
            {
                model.Detalles = GetDetallesAgenda(connection, new AgendaDetalle()
                {
                    IdAgenda = model.Id
                });
                model = MapInternal(model);
            }
        }

        public virtual List<AgendaDetalle> GetDetallesAgenda(DbConnection connection, AgendaDetalle model, bool excluirSobrantes = false)
        {
            var param = new DynamicParameters(new
            {
                nuAgenda = model.IdAgenda
            });

            string sql = @"SELECT 
                        NU_AGENDA as IdAgenda,
                        CD_EMPRESA as IdEmpresa,
                        CD_PRODUTO as CodigoProducto,
                        CD_FAIXA as Faixa,
                        NU_IDENTIFICADOR as Identificador,
                        CD_SITUACAO as EstadoId,
                        QT_AGENDADO as CantidadAgendada,
                        QT_RECIBIDA as CantidadRecibida,
                        QT_ACEPTADA as CantidadAceptada,
                        QT_AGENDADO_ORIGINAL as CantidadAgendadaOriginal,
                        QT_RECIBIDA_FICTICIA as CantidadRecibidaFicticia,
                        QT_CROSS_DOCKING as CantidadCrossDocking,
                        DT_FABRICACAO as Vencimiento,
                        DT_ADDROW as FechaAlta,
                        DT_UPDROW as FechaModificacion,
                        DT_ACEPTADA_RECEPCION as FechaAceptacionRecepcion,
                        CD_FUNC_ACEPTO_RECEPCION as IdUsuarioAceptacionRecepcion,
                        VL_CIF as CIF,
                        VL_PRECIO as Precio
                FROM T_DET_AGENDA 
                WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<AgendaDetalle>(connection, sql, param, commandType: CommandType.Text).ToList();
        }

        public virtual Agenda MapInternal(Agenda agenda)
        {
            if (agenda != null)
            {
                switch (agenda.EstadoId)
                {
                    case EstadoAgendaDb.Abierta: agenda.Estado = EstadoAgenda.Abierta; break;
                    case EstadoAgendaDb.Cerrada: agenda.Estado = EstadoAgenda.Cerrada; break;
                    case EstadoAgendaDb.Cancelada: agenda.Estado = EstadoAgenda.Cancelada; break;
                    case EstadoAgendaDb.IngresandoFactura: agenda.Estado = EstadoAgenda.IngresandoFactura; break;
                    case EstadoAgendaDb.ConferidaConDiferencias: agenda.Estado = EstadoAgenda.ConferidaConDiferencias; break;
                    case EstadoAgendaDb.ConferidaSinDiferencias: agenda.Estado = EstadoAgenda.ConferidaSinDiferencias; break;
                    case EstadoAgendaDb.AguardandoDesembarque: agenda.Estado = EstadoAgenda.AguardandoDesembarque; break;
                    case EstadoAgendaDb.DocumentoAsociado: agenda.Estado = EstadoAgenda.DocumentoAsociado; break;
                    default: agenda.Estado = EstadoAgenda.Unknown; break;
                }

                foreach (var detalle in agenda.Detalles)
                {
                    switch (detalle.EstadoId)
                    {
                        case EstadoAgendaDetalleDb.Abierta: detalle.Estado = EstadoAgendaDetalle.Abierta; break;
                        case EstadoAgendaDetalleDb.ConferidaConDiferencias: detalle.Estado = EstadoAgendaDetalle.ConferidaConDiferencias; break;
                        case EstadoAgendaDetalleDb.ConferidaSinDiferencias: detalle.Estado = EstadoAgendaDetalle.ConferidaSinDiferencias; break;
                        default: detalle.Estado = EstadoAgendaDetalle.Unknown; break;
                    }
                }
            }
            return agenda;
        }

        public virtual async Task AddAgendas(List<Agenda> agendas, IAgendaServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var bulkContext = GetBulkOperationContext(agendas, context, connection);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertAgendas(connection, tran, bulkContext.NewAgendas);
                    await BulkInsertAgendaReferencias(connection, tran, bulkContext.NewAgendaReferencias);
                    await BulkInsertAgendaReferenciaDetalles(connection, tran, bulkContext.NewAgendaReferencias);
                    await BulkInsertAgendaDetalles(connection, tran, bulkContext.NewAgendaReferencias);
                    tran.Commit();
                }

                try
                {
                    // The .NET Framework Data Provider for Oracle only supports ReadCommitted and Serializable
                    using (var tran = connection.BeginTransaction(this._dapper.GetSnapshotIsolationLevel()))
                    {
                        await BulkUpdateAgendaDetalles(connection, tran, bulkContext.NewAgendaReferencias);

                        if (bulkContext.NewAgendaLiberadaReferencias.Count > 0)
                            await BulkInsertAgendaProblemas(connection, tran, bulkContext.NewAgendaLiberadaReferencias);

                        await BulkUpdateAgendaReferenciaDetalles(connection, tran, bulkContext.NewAgendaReferencias);
                        await BulkUpdateReferenciaDetalles(connection, tran, bulkContext.NewAgendaReferencias);
                        await BulkDeleteEmptyAgendaReferenciaDetalles(connection, tran, bulkContext.NewAgendaReferencias);
                        await BulkDeleteEmptyAgendaDetalles(connection, tran, bulkContext.NewAgendaReferencias);
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        await RollbackBulkInsertAgendaDetalles(connection, bulkContext.NewAgendaReferencias);
                        await RollbackBulkInsertAgendaReferenciaDetalles(connection, bulkContext.NewAgendaReferencias);
                        await RollbackBulkInsertAgendaReferencias(connection, bulkContext.NewAgendaReferencias);
                        await RollbackBulkInsertAgendas(connection, bulkContext.NewAgendas);
                    }
                    catch (Exception rex)
                    {
                        logger.Error(rex, "Internal error on AgendaRepository.AddAgendas rollback");
                    }

                    throw ex;
                }
            }
        }

        public virtual async Task BulkInsertAgendaProblemas(DbConnection connection, DbTransaction tran, List<object> agendaLiberadaReferencias)
        {
            string sql = @$"
                    INSERT INTO T_RECEPCION_AGENDA_PROBLEMA (
                        NU_RECEPCION_AGENDA_PROBLEMA,
                        NU_AGENDA,
                        CD_PRODUTO,
                        NU_IDENTIFICADOR,
                        ND_TIPO,
                        ND_PROBLEMA,
                        FL_ACEPTADO,
                        CD_FUNCIONARIO,
                        DT_ACEPTADO,
                        DT_ADDROR,
                        DT_UPDROW,
                        CD_FAIXA,
                        CD_FUNCIONARIO_ACEPTA_PROBLEMA,
                        VL_DIFERENCIA,
                        NU_LPN)
                    SELECT
                        0,
                        :agenda,
                        DA.CD_PRODUTO,
                        DA.NU_IDENTIFICADOR,
                        '{ProblemaAgendaDb.TipoProblema}',
                        '{ProblemaAgendaDb.RecibidoMenorAgendado}',
                        'N',
                        {_userId},
                        NULL,
                        :fecha,
                        :fecha,
                        DA.CD_FAIXA,
                        NULL,
                        DA.QT_AGENDADO,
                        NULL
                    FROM
                        T_DET_AGENDA DA
                    WHERE
                        DA.NU_AGENDA = :agenda
                ";

            await _dapper.ExecuteAsync(
                connection,
                sql,
                agendaLiberadaReferencias,
                transaction: tran);
        }

        public virtual async Task RollbackBulkInsertAgendas(DbConnection connection, List<object> agendas)
        {
            string sql = @"DELETE FROM T_AGENDA 
                WHERE NU_AGENDA = :Id";

            await _dapper.ExecuteAsync(connection, sql, agendas);
        }

        public virtual async Task RollbackBulkInsertAgendaReferencias(DbConnection connection, List<object> agendaReferencias)
        {
            string sql = @"DELETE FROM T_RECEPC_AGENDA_REFERENCIA_REL 
                WHERE NU_AGENDA = :agenda AND NU_RECEPCION_REFERENCIA = :referencia";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias);
        }

        public virtual async Task RollbackBulkInsertAgendaReferenciaDetalles(DbConnection connection, List<object> agendaReferencias)
        {
            string sql = @"UPDATE T_RECEPCION_AGENDA_REFERENCIA 
                    SET NU_TRANSACCION = :transaccion,
                        NU_TRANSACCION_DELETE = :transaccion 
                    WHERE NU_AGENDA = :agenda";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias);

            sql = @"DELETE FROM T_RECEPCION_AGENDA_REFERENCIA
                    WHERE NU_AGENDA = :agenda ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias);
        }

        public virtual async Task RollbackBulkInsertAgendaDetalles(DbConnection connection, List<object> agendaReferencias)
        {
            var sql = @$"DELETE FROM T_DET_AGENDA
                        WHERE NU_AGENDA = :agenda ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias);
        }

        public virtual async Task BulkDeleteEmptyAgendaReferenciaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            string sql = @"UPDATE T_RECEPCION_AGENDA_REFERENCIA 
                    SET NU_TRANSACCION = :transaccion,
                        NU_TRANSACCION_DELETE = :transaccion 
                    WHERE NU_AGENDA = :agenda AND QT_AGENDADA = 0 ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);

            sql = @"DELETE FROM T_RECEPCION_AGENDA_REFERENCIA
                    WHERE NU_AGENDA = :agenda AND QT_AGENDADA = 0 ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual async Task BulkDeleteEmptyAgendaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            var sql = @$"DELETE FROM T_DET_AGENDA
                        WHERE NU_AGENDA = :agenda AND QT_AGENDADO = 0";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual AgendaBulkOperationContext GetBulkOperationContext(List<Agenda> agendas, IAgendaServiceContext serviceContext, DbConnection connection)
        {
            var context = new AgendaBulkOperationContext();
            var agendaIds = GetNewAgendaIds(agendas.Count, connection);

            for (int i = 0; i < agendas.Count; i++)
            {
                var agenda = Map(agendas[i], agendaIds[i], serviceContext);

                context.NewAgendas.Add(GetAgendaEntity(agenda));

                if (agenda.ReferenciaId.HasValue)
                {
                    context.NewAgendaReferencias.Add(this.GetAgendaReferenciaEntity(agenda));

                    if (agenda.LiberarAgenda != null && agenda.LiberarAgenda == true)
                        context.NewAgendaLiberadaReferencias.Add(this.GetAgendaReferenciaEntity(agenda));
                }
            }

            return context;
        }

        public virtual List<int> GetNewAgendaIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, "S_NRO_AGENDA", count).ToList();
        }

        public virtual object GetAgendaEntity(Agenda agenda)
        {
            return new
            {
                Id = agenda.Id,
                IdEmpresa = agenda.IdEmpresa,
                CodigoInternoCliente = agenda.CodigoInternoCliente,
                NumeroDocumento = _mapper.NullIfEmpty(agenda.NumeroDocumento),
                FechaInsercion = agenda.FechaInsercion,
                Predio = agenda.Predio,
                FuncionarioResponsable = agenda.FuncionarioResponsable,
                Estado = agenda.EstadoId,
                CodigoOperacion = agenda.CodigoOperacion,
                CodigoPuerta = agenda.CodigoPuerta,
                PlacaVehiculo = agenda.PlacaVehiculo,
                Anexo1 = agenda.Anexo1,
                Anexo2 = agenda.Anexo2,
                Anexo3 = agenda.Anexo3,
                Anexo4 = agenda.Anexo4,
                EnviaDocumentacion = agenda.EnviaDocumentacionId,
                Averiado = agenda.AveriadoId,
                FechaEntrega = agenda.FechaEntrega,
                TipoRecepcion = agenda.TipoRecepcionInterno,
                IdFuncionarioAsignado = agenda.IdFuncionarioAsignado,
                CargaDetalleAutomatica = agenda.CargaDetalleAutomaticaId,
                Transaccion = agenda.NumeroTransaccion
            };
        }

        public virtual object GetAgendaReferenciaEntity(Agenda agenda)
        {
            return new
            {
                agenda = agenda.Id,
                referencia = agenda.ReferenciaId,
                fecha = agenda.FechaInsercion,
                transaccion = agenda.NumeroTransaccion,
                estadoDetalle = agenda.EstadoId == EstadoAgendaDb.AguardandoDesembarque
                    ? EstadoAgendaDetalleDb.ConferidaConDiferencias
                    : EstadoAgendaDetalleDb.Abierta
            };
        }

        public virtual async Task BulkInsertAgendas(DbConnection connection, DbTransaction tran, List<object> agendas)
        {
            string sql = @"INSERT INTO T_AGENDA 
                    (NU_AGENDA, 
                    CD_EMPRESA,
                    CD_CLIENTE,
                    NU_DOCUMENTO,
                    DT_ADDROW,
                    CD_PORTA,
                    NU_PREDIO,
                    CD_FUN_RESP,
                    CD_SITUACAO,
                    CD_OPERACAO,
                    DS_PLACA,
                    DS_ANEXO1,
                    DS_ANEXO2,
                    DS_ANEXO3,
                    DS_ANEXO4,
                    ID_ENVIO_DOCUMENTACION,
                    ID_AVERIA,
                    DT_ENTREGA,
                    TP_RECEPCION,
                    CD_FUNCIONARIO_ASIGNADO,
                    FL_CARGA_AUTO_DETALLE,
                    NU_TRANSACCION) 
                    VALUES (
                    :Id,
                    :IdEmpresa,
                    :CodigoInternoCliente,
                    :NumeroDocumento,
                    :FechaInsercion,
                    :CodigoPuerta,
                    :Predio,
                    :FuncionarioResponsable,
                    :Estado,
                    :CodigoOperacion,
                    :PlacaVehiculo,
                    :Anexo1,
                    :Anexo2,
                    :Anexo3,
                    :Anexo4,
                    :EnviaDocumentacion,
                    :Averiado,
                    :FechaEntrega,
                    :TipoRecepcion,
                    :IdFuncionarioAsignado,
                    :CargaDetalleAutomatica,
                    :Transaccion)";

            await _dapper.ExecuteAsync(connection, sql, agendas, transaction: tran);
        }

        public virtual async Task BulkInsertAgendaReferencias(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            // NU_AGENDA_REFERENCIA_REL se asigna por trigger
            string sql = @"INSERT INTO T_RECEPC_AGENDA_REFERENCIA_REL (NU_AGENDA_REFERENCIA_REL, NU_AGENDA, NU_RECEPCION_REFERENCIA) 
                VALUES (NULL, :agenda, :referencia)";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual async Task BulkInsertAgendaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            var sql = @$"INSERT INTO T_DET_AGENDA
                        (NU_AGENDA,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        CD_SITUACAO,
                        QT_AGENDADO,
                        QT_RECIBIDA,
                        QT_ACEPTADA,
                        QT_AGENDADO_ORIGINAL,
                        QT_RECIBIDA_FICTICIA,
                        QT_CROSS_DOCKING,
                        DT_FABRICACAO,
                        DT_ADDROW,
                        NU_TRANSACCION) 
                    SELECT :agenda,
                       RRD.CD_EMPRESA,
                       RRD.CD_PRODUTO,
                       RRD.CD_FAIXA,
                       RRD.NU_IDENTIFICADOR,
                       :estadoDetalle,
                       0,
                       0,
                       0,
                       0,
                       0,
                       0,
                       MIN(RRD.DT_VENCIMIENTO),
                       :fecha,
                       :transaccion
                    FROM T_RECEPCION_REFERENCIA_DET RRD
                    WHERE RRD.NU_RECEPCION_REFERENCIA = :referencia 
                        AND (COALESCE(RRD.QT_REFERENCIA,0) - COALESCE(RRD.QT_AGENDADA,0) - COALESCE(RRD.QT_RECIBIDA,0) - COALESCE(RRD.QT_ANULADA,0)) > 0 
                    GROUP BY RRD.CD_EMPRESA, RRD.CD_PRODUTO, RRD.CD_FAIXA, RRD.NU_IDENTIFICADOR ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual async Task BulkInsertAgendaReferenciaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            string sql = @"INSERT INTO T_RECEPCION_AGENDA_REFERENCIA 
                        (NU_AGENDA,
                        CD_EMPRESA, 
                        NU_RECEPCION_REFERENCIA_DET,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        QT_AGENDADA,
                        QT_RECIBIDA,
                        DT_ADDROW,
                        NU_TRANSACCION) 
                    SELECT :agenda,
                       RRD.CD_EMPRESA,
                       MIN(RRD.NU_RECEPCION_REFERENCIA_DET),
                       RRD.CD_PRODUTO,
                       RRD.CD_FAIXA,
                       RRD.NU_IDENTIFICADOR,
                       0,
                       0,
                       :fecha,
                       :transaccion 
                    FROM T_RECEPCION_REFERENCIA_DET RRD
                    WHERE RRD.NU_RECEPCION_REFERENCIA = :referencia
                        AND (COALESCE(RRD.QT_REFERENCIA,0) - COALESCE(RRD.QT_AGENDADA,0) - COALESCE(RRD.QT_RECIBIDA,0) - COALESCE(RRD.QT_ANULADA,0)) > 0
                    GROUP BY RRD.CD_EMPRESA, RRD.CD_PRODUTO, RRD.CD_FAIXA, RRD.NU_IDENTIFICADOR ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual async Task BulkUpdateAgendaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            var alias = "DA";
            var from = @"T_DET_AGENDA DA
                INNER JOIN (
	                SELECT 
		                RRD.CD_PRODUTO, 
		                RRD.NU_IDENTIFICADOR, 
		                RRD.CD_FAIXA,
		                RRD.CD_EMPRESA,
		                SUM(COALESCE(RRD.QT_REFERENCIA,0) - COALESCE(RRD.QT_AGENDADA,0) - COALESCE(RRD.QT_RECIBIDA,0) - COALESCE(RRD.QT_ANULADA,0)) QT_DISPONIBLE
                    FROM T_RECEPCION_REFERENCIA_DET RRD
                    WHERE RRD.NU_RECEPCION_REFERENCIA = :referencia 
		                AND (COALESCE(RRD.QT_REFERENCIA,0) - COALESCE(RRD.QT_AGENDADA,0) - COALESCE(RRD.QT_RECIBIDA,0) - COALESCE(RRD.QT_ANULADA,0)) > 0
                    GROUP BY
                        RRD.CD_PRODUTO, 
		                RRD.NU_IDENTIFICADOR, 
		                RRD.CD_FAIXA,
		                RRD.CD_EMPRESA
                ) T ON T.CD_PRODUTO = DA.CD_PRODUTO 
	                AND T.NU_IDENTIFICADOR = DA.NU_IDENTIFICADOR 
                    AND T.CD_FAIXA = DA.CD_FAIXA 
                    AND T.CD_EMPRESA = DA.CD_EMPRESA";
            var set = @" 
                QT_AGENDADO = QT_DISPONIBLE,
	            NU_TRANSACCION = :transaccion";
            var where = "NU_AGENDA = :agenda";

            await _dapper.ExecuteUpdateAsync(connection, alias, from, set, where, param: agendaReferencias, transaction: tran);

            var sql = @"UPDATE T_DET_AGENDA
                        SET QT_AGENDADO_ORIGINAL = QT_AGENDADO,
                            NU_TRANSACCION = :transaccion 
                    WHERE NU_AGENDA = :agenda ";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual async Task BulkUpdateAgendaReferenciaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            var alias = "RAR";
            var from = @"T_RECEPCION_AGENDA_REFERENCIA RAR
                INNER JOIN (
                    SELECT 
                        DA.CD_PRODUTO, 
                        DA.NU_IDENTIFICADOR, 
                        DA.CD_FAIXA, 
                        DA.CD_EMPRESA,
                        DA.QT_AGENDADO
                    FROM T_DET_AGENDA DA 
                    WHERE DA.NU_AGENDA = :agenda 
                ) T ON T.CD_PRODUTO = RAR.CD_PRODUTO 
                    AND T.NU_IDENTIFICADOR = RAR.NU_IDENTIFICADOR 
                    AND T.CD_FAIXA = RAR.CD_FAIXA 
                    AND T.CD_EMPRESA = RAR.CD_EMPRESA";
            var set = @" 
                QT_AGENDADA = QT_AGENDADO,
	            NU_TRANSACCION = :transaccion";
            var where = "NU_AGENDA = :agenda";

            await _dapper.ExecuteUpdateAsync(connection, alias, from, set, where, param: agendaReferencias, transaction: tran);
        }

        public virtual async Task BulkUpdateReferenciaDetalles(DbConnection connection, DbTransaction tran, List<object> agendaReferencias)
        {
            string sql = @"UPDATE T_RECEPCION_REFERENCIA_DET 
                SET QT_AGENDADA = COALESCE(QT_REFERENCIA,0) - COALESCE(QT_RECIBIDA,0) - COALESCE(QT_ANULADA,0), 
                    DT_UPDROW = :fecha, NU_TRANSACCION = :transaccion
                WHERE NU_RECEPCION_REFERENCIA = :referencia
                    AND (COALESCE(QT_REFERENCIA,0) - COALESCE(QT_AGENDADA,0) - COALESCE(QT_RECIBIDA,0) - COALESCE(QT_ANULADA,0)) > 0";

            await _dapper.ExecuteAsync(connection, sql, agendaReferencias, transaction: tran);
        }

        public virtual Agenda Map(Agenda request, int idAgenda, IAgendaServiceContext context)
        {
            var agente = context.GetAgente(request.CodigoAgente, request.IdEmpresa, request.TipoAgente);

            Agenda agenda = new Agenda();

            agenda.Id = request.Id = idAgenda;
            agenda.IdEmpresa = request.IdEmpresa;
            agenda.NumeroDocumento = request.NumeroDocumento;
            agenda.CodigoInternoCliente = agente.CodigoInterno;
            agenda.TipoRecepcionInterno = request.TipoRecepcionInterno;
            agenda.Predio = request.Predio;
            agenda.LiberarAgenda = request.LiberarAgenda;
            agenda.EstadoId = (request.LiberarAgenda == null ? EstadoAgendaDb.Abierta : ((bool)request.LiberarAgenda ? EstadoAgendaDb.AguardandoDesembarque : EstadoAgendaDb.Abierta));
            agenda.FechaEntrega = request.FechaEntrega;
            agenda.FechaInsercion = DateTime.Now;
            agenda.Anexo1 = request.Anexo1;
            agenda.Anexo2 = request.Anexo2;
            agenda.Anexo3 = request.Anexo3;
            agenda.Anexo4 = request.Anexo4;
            agenda.PlacaVehiculo = request.PlacaVehiculo;
            agenda.IdFuncionarioAsignado = request.IdFuncionarioAsignado;
            agenda.FuncionarioResponsable = request.FuncionarioResponsable;
            agenda.CodigoOperacion = OperacionAgendaDb.RecepcionAgrupada;
            agenda.CodigoPuerta = request.CodigoPuerta;
            agenda.EnviaDocumentacionId = "N";
            agenda.AveriadoId = "N";
            agenda.CargaDetalleAutomaticaId = "N";
            agenda.ReferenciaId = request.ReferenciaId;

            return agenda;
        }

        public virtual async Task<List<APITask>> GetAgendasPendientesDeConfirmacion(CancellationToken cancelToken = default)
        {
            var sql = @"SELECT 
                            ID_OPERACION AS Id,
                            DT_OPERACION AS Fecha
                        FROM V_CONFIRMACIONES_PENDIENTES
                        WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna 
                            AND FL_HABILITADA = 'S'
                        ORDER BY 
                            DT_OPERACION ASC, 
                            ID_OPERACION ASC";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                return _dapper.Query<APITask>(connection, sql, param: new { cdInterfazExterna = CInterfazExterna.ConfirmacionDeRecepcion }, commandType: CommandType.Text).ToList();
            }
        }

        public virtual async Task<List<long>> GenerarInterfaces(int nuAgenda, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                logger.Debug($"Confirmación de Recepción. Agenda: {nuAgenda}");

                long nuEjecucion = -2;
                long? nuTransaccion = null;

                try
                {
                    nuTransaccion = await CreateTransaction("GenerarInterfaces", connection, null);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Confirmación de Recepción. Agenda: {nuAgenda} - Error: {ex}");
                }

                if (nuTransaccion.HasValue)
                {
                    try
                    {
                        using (var tran = connection.BeginTransaction())
                        {
                            var agenda = await GetAgendaOrNull(nuAgenda);

                            var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.ConfirmacionDeRecepcion, agenda.IdEmpresa, connection, tran);

                            if (!interfazHabilitada)
                            {
                                logger.Debug($"La interfaz {CInterfazExterna.ConfirmacionDeRecepcion} no esta habilitada para la empresa {agenda.IdEmpresa}.");
                                return new List<long>();
                            }

                            if (agenda.ManejaInterfazId != "D")
                                nuEjecucion = 0;
                            else
                            {
                                var confRecepcion = Map(agenda, connection, tran);
                                var grupoConsulta = GetGrupoConsulta(confRecepcion.Empresa);
                                nuEjecucion = await CrearEjecucion(confRecepcion, grupoConsulta, connection, tran);
                            }

                            await UpdateAgenda(nuAgenda, nuEjecucion, nuTransaccion.Value, connection, tran);

                            tran.Commit();
                            logger.Debug($"Agenda actualizada. Nro InterfazEjecucion: {nuEjecucion}");
                        }
                    }
                    catch (Exception ex)
                    {
                        nuEjecucion = -2;
                        logger.Error(ex, $"Confirmación de Recepción. Agenda: {nuAgenda} - Error: {ex}");
                        await UpdateAgenda(nuAgenda, nuEjecucion, nuTransaccion.Value, connection, null);
                    }
                }

                return new List<long>() { nuEjecucion };
            }
        }

        public virtual async Task<long> CreateTransaction(string dsTransaccion, DbConnection connection, DbTransaction tran)
        {
            var transaccionRepository = new TransaccionRepository(_context, _cdAplicacion, _userId, _dapper);
            return await transaccionRepository.CreateTransaction(dsTransaccion, connection, tran);
        }

        public virtual ConfirmacionRecepcionResponse Map(Agenda agenda, DbConnection connection, DbTransaction tran)
        {
            var referenciaRepository = new ReferenciaRecepcionRepository(_context, _cdAplicacion, _userId, _dapper);
            var agente = new AgenteRepository(_context, _cdAplicacion, _userId, _dapper).GetAgenteOrNull((int)agenda.IdEmpresa, agenda.CodigoInternoCliente).Result;

            var model = new ConfirmacionRecepcionResponse()
            {
                Agenda = agenda.Id,
                Empresa = agenda.IdEmpresa,
                CodigoAgente = agente?.Codigo,
                TipoAgente = agente?.Tipo,
                TipoRecepcion = agenda.TipoRecepcionInterno,
                NumeroDocumento = agenda.NumeroDocumento,
                FechaIngreso = agenda.FechaInsercion?.ToString(CDateFormats.DATE_ONLY),
                FechaCierre = agenda.FechaCierre?.ToString(CDateFormats.DATE_ONLY),
                Anexo1 = agenda.Anexo1,
                Anexo2 = agenda.Anexo2,
                Anexo3 = agenda.Anexo3,
                Anexo4 = agenda.Anexo4,
                Predio = agenda.Predio,
            };

            #region Detalles

            foreach (var detalle in agenda.Detalles)
            {
                model.Detalles.Add(new DetalleAgendaResponse()
                {
                    Producto = detalle.CodigoProducto,
                    Identificador = detalle.Identificador,
                    CantidadTeorica = detalle.CantidadAgendadaOriginal,
                    CantidadRecibida = detalle.CantidadRecibida,
                    FechaVencimiento = detalle.Vencimiento?.ToString(CDateFormats.DATE_ONLY),
                });
            }

            #endregion

            #region Referencias

            var rel = GetRelacionAgendaReferencia(agenda.Id, connection, tran);
            var referencias = rel.Select(x => x.IdReferencia).Distinct().ToList();

            foreach (var idRef in referencias)
            {
                var referencia = referenciaRepository.GetReferencia(new ReferenciaRecepcion()
                {
                    Id = idRef
                }, connection, tran);

                var modelRef = new ReferenciaResponse()
                {
                    NumeroReferencia = referencia.Numero,
                    TipoReferencia = referencia.TipoReferencia,
                    CodigoAgente = agente?.Codigo,
                    TipoAgente = agente?.Tipo,
                    Memo = referencia.Memo,
                    Anexo1 = referencia.Anexo1,
                    Anexo2 = referencia.Anexo2,
                    Anexo3 = referencia.Anexo3,
                    Serializado = referencia.Serializado,
                    Predio = referencia.IdPredio,
                };

                var detalles = rel.Where(x => x.IdReferencia == idRef).Select(x => x.IdDetalleReferencia).Distinct().ToList();

                foreach (var idDetRef in detalles)
                {
                    var detalleReferencia = referenciaRepository.GetDetalleReferencia(idDetRef).Result;

                    var cantidadConsumidaAgenda = GetCantidadConsumidaAgenda(new RecepcionAgendaReferencia()
                    {
                        Agenda = agenda.Id,
                        Empresa = agenda.IdEmpresa,
                        IdDetalleReferencia = idDetRef,
                        CodigoProducto = detalleReferencia.CodigoProducto,
                        Identificador = detalleReferencia.Identificador,
                    }, connection, tran);


                    modelRef.Detalles.Add(new ReferenciaDetalleResponse()
                    {
                        IdLineaSistemaExterno = detalleReferencia.IdLineaSistemaExterno,
                        Producto = detalleReferencia.CodigoProducto,
                        Identificador = detalleReferencia.Identificador,
                        CantidadReferencia = detalleReferencia.CantidadReferencia.Value,
                        CantidadConsumida = detalleReferencia.CantidadRecibida,
                        CantidadConsumidaAgenda = cantidadConsumidaAgenda,
                        FechaVencimiento = detalleReferencia.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY),
                        Anexo = detalleReferencia.Anexo1,
                    });
                }

                model.Referencias.Add(modelRef);
            }

            #endregion

            #region Lpns

            var lpnsAsociados = GetFotoLpnsAsociados(agenda.Id, connection, tran);
            var detallesLpns = GetFotoDetalleLpnsAsociados(agenda.Id, connection, tran);
            var atributosCabezales = GetFotoAtributosCabezalLpnsAsociados(agenda.Id, connection, tran);
            var atributosDetalles = GetFotoAtributosDetalleLpnsAsociados(agenda.Id, connection, tran);

            if (lpnsAsociados != null && lpnsAsociados.Count > 0)
            {
                foreach (var lpn in lpnsAsociados)
                {
                    var modelLpn = new LpnSalidaResponse()
                    {
                        Numero = lpn.NroLPN,
                        IdExterno = lpn.IdExterno,
                        Empresa = lpn.Empresa,
                        Tipo = lpn.Tipo,
                        IdPacking = lpn.IdPacking
                    };

                    if (atributosCabezales != null && atributosCabezales.Count > 0)
                    {
                        var atributosCabezal = atributosCabezales
                            .Where(a => a.NroLPN == lpn.NroLPN)
                            .Select(a => new AtributoResponse()
                            {
                                Nombre = a.Nombre,
                                Valor = a.Valor
                            })
                            .ToList();

                        modelLpn.Atributos = atributosCabezal;
                    }

                    if (detallesLpns != null && detallesLpns.Count > 0)
                    {
                        foreach (var det in detallesLpns.Where(a => a.NroLPN == lpn.NroLPN))
                        {
                            var modelDetalleLpn = new LpnSalidaDetalleResponse()
                            {
                                Id = det.IdLpnDetalle,
                                CodigoProducto = det.CodigoProducto,
                                Faixa = det.Faixa,
                                Lote = det.Identificador,
                                Cantidad = det.CantidadRecibida,
                                Vencimiento = det.Vencimiento?.ToString(CDateFormats.DATE_ONLY),
                                IdLineaSistemaExterno = det.IdLineaSistemaExterno
                            };

                            if (atributosDetalles != null && atributosDetalles.Count > 0)
                            {
                                var atributosDetalle = atributosDetalles
                                    .Where(a => a.NroLPN == det.NroLPN
                                        && a.IdLpnDetalle == det.IdLpnDetalle
                                        && a.CodigoProducto == det.CodigoProducto
                                        && a.Faixa == det.Faixa
                                        && a.Identificador == det.Identificador)
                                    .Select(a => new AtributoResponse()
                                    {
                                        Nombre = a.Nombre,
                                        Valor = a.Valor
                                    })
                                    .ToList();

                                modelDetalleLpn.Atributos = atributosDetalle;
                            }

                            modelLpn.Detalles.Add(modelDetalleLpn);
                        }
                    }

                    model.Lpns.Add(modelLpn);
                }
            }

            #endregion

            #region Facturas


            var facturaRepository = new FacturaRepository(_context, _cdAplicacion, _userId, _dapper);

            var facturas = facturaRepository.GetFacturasByAgenda(agenda.Id);

            foreach (var factura in facturas)
            {
                var nuevaFactura = new FacturaResponse()
                {
                    Id = factura.Id.ToString(),
                    Serie = factura.Serie,
                    Factura = factura.NumeroFactura,
                    TipoFactura = factura.TipoFactura,
                    FechaEmision = factura.FechaEmision?.ToString(CDateFormats.DATE_ONLY),
                    TotalDigitado = factura.TotalDigitado ?? 0,
                    Origen = factura.IdOrigen,
                    CodigoMoneda = factura.CodigoMoneda,
                    CodigoCliente = factura.CodigoInternoCliente,
                    CodigoSituacion = factura.Situacion.ToString(),
                    CodigoEmpresa = factura.IdEmpresa.ToString(),
                    Anexo1 = factura.Anexo1,
                    Anexo2 = factura.Anexo2,
                    Anexo3 = factura.Anexo3,
                    Observacion = factura.Observacion,
                    FechaCreacion = factura.FechaCreacion?.ToString(CDateFormats.DATE_ONLY),
                    FechaVencimiento = factura.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY),
                    Estado = factura.Estado,
                    Predio = factura.Predio,
                    NumeroRemito = factura.Remito,
                    ImporteIvaBase = factura.IvaBase,
                    ImporteIvaMinimo = factura.IvaMinimo,
                    Agenda = factura.Agenda.ToString(),
                    NumeroOrdenCompra = factura.Referencia,
                    Detalles = new List<FacturaDetalleResponse>()
                };

                var detallesFactura = factura.Detalles
                    .OrderBy(d => d.Producto)
                    .ThenBy(d => d.Identificador)
                    .ToList();

                foreach (var detFactura in detallesFactura)
                {
                    var nuevoDetalleFactura = new FacturaDetalleResponse()
                    {
                        Producto = detFactura.Producto,
                        Identificador = detFactura.Identificador,
                        CantidadFacturada = detFactura.CantidadFacturada ?? 0,
                        CantidadValidada = detFactura.CantidadValidada ?? 0,
                        CantidadRecibida = detFactura.CantidadRecibida ?? 0,
                        FechaVencimiento = detFactura.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY),
                        FechaCreacion = detFactura.FechaCreacion?.ToString(CDateFormats.DATE_ONLY),
                        Anexo1 = detFactura.Anexo1,
                        Anexo2 = detFactura.Anexo2,
                        Anexo3 = detFactura.Anexo3,
                        Anexo4 = detFactura.Anexo4,
                    };

                    if (nuevoDetalleFactura.CantidadRecibida > 0)
                    {
                        decimal qtRecibida = nuevoDetalleFactura.CantidadRecibida;

                        var referenciasCandidatas = model.Referencias
                            .Where(w => w.Detalles.Any(a => a.Producto == nuevoDetalleFactura.Producto && a.Identificador == nuevoDetalleFactura.Identificador))
                            .OrderBy(s => s.NumeroReferencia)
                            .ToList();

                        foreach (var referencia in referenciasCandidatas)
                        {
                            if (qtRecibida == 0)
                                break;

                            var detallesReferencia = referencia.Detalles
                                .Where(w => w.Producto == nuevoDetalleFactura.Producto && w.Identificador == nuevoDetalleFactura.Identificador)
                                .OrderBy(s => s.Producto)
                                .OrderBy(s => s.Identificador)
                                .ToList();

                            foreach (var detReferencia in detallesReferencia)
                            {
                                if ((detReferencia.CantidadAsignadaFactura ?? 0) >= (detReferencia.CantidadConsumidaAgenda ?? 0))
                                    continue;

                                var newDetRefFact = new ReferenciaDetalleFacturaResponse()
                                {
                                    NumeroReferencia = referencia.NumeroReferencia,
                                    TipoReferencia = referencia.TipoReferencia,
                                    CantidadReferencia = detReferencia.CantidadConsumidaAgenda ?? 0,
                                    CantidadRecibida = Math.Min((detReferencia.CantidadConsumidaAgenda ?? 0) - (detReferencia.CantidadAsignadaFactura ?? 0), qtRecibida)
                                };

                                detReferencia.CantidadAsignadaFactura = (detReferencia.CantidadAsignadaFactura ?? 0) + newDetRefFact.CantidadRecibida;
                                qtRecibida -= newDetRefFact.CantidadRecibida;

                                nuevoDetalleFactura.Referencias.Add(newDetRefFact);

                                if (qtRecibida == 0)
                                    break;
                            }
                        }

                        if (qtRecibida > 0)
                            throw new Exception("Cantidad recibida mayor a la facturada");
                    }
                    else
                    {
                        var dets = _dapper.Query<ReferenciaDetalleFacturaResponse>(connection, @$" 
                                SELECT RR.NU_REFERENCIA NUMEROREFERENCIA, 
                                       RR.TP_REFERENCIA TIPOREFERENCIA, 
                                       SUM(RRD.QT_REFERENCIA) CANTIDADREFERENCIA, 
                                       0 CANTIDADRECIBIDA 
                                FROM T_RECEPC_AGENDA_REFERENCIA_REL RAR 
                                INNER JOIN T_RECEPCION_REFERENCIA RR
                                    ON RR.NU_RECEPCION_REFERENCIA = RAR.NU_RECEPCION_REFERENCIA
                                INNER JOIN T_RECEPCION_REFERENCIA_DET RRD
                                    ON RR.NU_RECEPCION_REFERENCIA = RRD.NU_RECEPCION_REFERENCIA
                                WHERE RAR.NU_AGENDA = {model.Agenda} 
                                    AND RRD.CD_PRODUTO = '{nuevoDetalleFactura.Producto}'
                                    AND RRD.NU_IDENTIFICADOR = '{nuevoDetalleFactura.Identificador}'
                                GROUP BY 
                                    RR.NU_REFERENCIA, 
                                    RR.TP_REFERENCIA",
                            transaction: tran,
                            commandType: CommandType.Text);

                        foreach (var nuevoDetalleReferenciaFactura in dets)
                        {
                            nuevoDetalleFactura.Referencias.Add(nuevoDetalleReferenciaFactura);
                        }
                    }

                    nuevaFactura.Detalles.Add(nuevoDetalleFactura);
                }

                model.Facturas.Add(nuevaFactura);
            }

            #endregion

            #region etiquetas

            var etiquetas = GetEtiquetasAsociadas(agenda.Id, connection, tran);
            var etiquetasDetalles = GetEtiquetaDetallesAsociados(etiquetas, connection, tran);

            if (etiquetas != null && etiquetas.Count > 0)
            {
                foreach (var etiqueta in etiquetas)
                {
                    var modelEtiqueta = new EtiquetasResponse()
                    {
                        NumeroEtiqueta = etiqueta.NumeroEtiqueta,
                        NumeroAgenda = etiqueta.NumeroAgenda,
                        CodigoEndereco = etiqueta.CodigoEndereco,
                        CodigoEnderecoSugerido = etiqueta.CodigoEnderecoSugerido,
                        CodigoSituacao = etiqueta.CodigoSituacao,
                        CodigoFuncRecepcion = etiqueta.CodigoFuncRecepcion,
                        FechaRecepcion = etiqueta.FechaRecepcion,
                        CodigoFuncAlmacenamiento = etiqueta.CodigoFuncAlmacenamiento,
                        FechaAlmacenamiento = etiqueta.FechaAlmacenamiento,
                        CodigoCliente = etiqueta.CodigoCliente,
                        CodigoGrupo = etiqueta.CodigoGrupo,
                        CodigoPallet = etiqueta.CodigoPallet,
                        CodigoBarras = etiqueta.CodigoBarras
                    };

                    if (etiquetasDetalles != null && etiquetasDetalles.Count > 0)
                    {
                        foreach (var det in etiquetasDetalles.Where(d => d.NumeroEtiquetaLote == etiqueta.NumeroEtiqueta))
                        {
                            var modelEtiquetaDetalle = new EtiquetasDetalleResponse()
                            {
                                NumeroEtiquetaLote = det.NumeroEtiquetaLote,
                                CodigoProducto = det.CodigoProducto,
                                CodigoFaixa = det.CodigoFaixa,
                                CodigoEmpresa = det.CodigoEmpresa,
                                NumeroIdentificador = det.NumeroIdentificador,
                                CantidadProductoRecibido = det.CantidadProductoRecibido,
                                CantidadProducto = det.CantidadProducto,
                                CantidadAjusteRecibido = det.CantidadAjusteRecibido,
                                CantidadEtiquetaGenerada = det.CantidadEtiquetaGenerada,
                                CantidadAlmacenado = det.CantidadAlmacenado,
                                FechaFabricacion = det.FechaFabricacion
                            };

                            modelEtiqueta.Detalles.Add(modelEtiquetaDetalle);
                        }
                    }

                    model.Etiquetas.Add(modelEtiqueta);
                }
            }


                #endregion

                return model;
        }

        public virtual List<AgendaEtiqueta> GetEtiquetasAsociadas(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT  
                    NU_ETIQUETA_LOTE as NumeroEtiqueta,
                    NU_AGENDA as NumeroAgenda,
                    CD_ENDERECO as CodigoEndereco,
                    CD_ENDERECO_SUGERIDO as CodigoEnderecoSugerido,
                    CD_SITUACAO as CodigoSituacao,
                    CD_FUNC_RECEPCION as CodigoFuncRecepcion,
                    DT_RECEPCION as FechaRecepcion,
                    CD_FUNC_ALMACENAMIENTO as CodigoFuncAlmacenamiento,
                    DT_ALMACENAMIENTO as FechaAlmacenamiento,
                    CD_CLIENTE as CodigoCliente,
                    CD_GRUPO as CodigoGrupo,
                    CD_PALLET as CodigoPallet,
                    CD_BARRAS as CodigoBarras
                FROM T_ETIQUETA_LOTE 
                WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<AgendaEtiqueta>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<AgendaEtiquetaDetalle> GetEtiquetaDetallesAsociados(List<AgendaEtiqueta> etiquetas, DbConnection connection, DbTransaction tran)
        {
            if (etiquetas == null || !etiquetas.Any())
                return new List<AgendaEtiquetaDetalle>();

            var numerosEtiqueta = etiquetas.Select(e => e.NumeroEtiqueta).ToList();

            string sql = @"SELECT  
                    NU_ETIQUETA_LOTE as NumeroEtiquetaLote,
                    CD_PRODUTO as CodigoProducto,
                    CD_FAIXA as CodigoFaixa,
                    CD_EMPRESA as CodigoEmpresa,
                    NU_IDENTIFICADOR as NumeroIdentificador,
                    QT_PRODUTO_RECIBIDO as CantidadProductoRecibido,
                    QT_PRODUTO as CantidadProducto,
                    QT_AJUSTE_RECIBIDO as CantidadAjusteRecibido,
                    QT_ETIQUETA_GENERADA as CantidadEtiquetaGenerada,
                    QT_ALMACENADO as CantidadAlmacenado,
                    DT_FABRICACAO as FechaFabricacion
                FROM T_DET_ETIQUETA_LOTE 
                WHERE NU_ETIQUETA_LOTE IN :numerosEtiqueta";


            return _dapper.Query<AgendaEtiquetaDetalle>(connection, sql, new { numerosEtiqueta }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<DTORecepcionAgendaReferencia> GetRelacionAgendaReferencia(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT ag.NU_AGENDA as IdAgenda, 
                            re.NU_RECEPCION_REFERENCIA as IdReferencia, 
                            det.NU_RECEPCION_REFERENCIA_DET  as IdDetalleReferencia 
                        FROM T_RECEPCION_REFERENCIA re 
                        INNER JOIN T_RECEPCION_REFERENCIA_DET det ON re.NU_RECEPCION_REFERENCIA = det.NU_RECEPCION_REFERENCIA 
                        INNER JOIN T_RECEPCION_AGENDA_REFERENCIA ag ON det.NU_RECEPCION_REFERENCIA_DET = ag.NU_RECEPCION_REFERENCIA_DET
                        WHERE ag.NU_AGENDA = :nuAgenda";

            return _dapper.Query<DTORecepcionAgendaReferencia>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual async Task<long> CrearEjecucion(ConfirmacionRecepcionResponse agenda, string grupoConsulta, DbConnection connection, DbTransaction tran)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);

            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = CInterfazExterna.ConfirmacionDeRecepcion,
                Situacion = SituacionDb.ProcesadoPendiente,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = $"Confirmación de Recepción Agenda: {agenda.Agenda}",
                Empresa = agenda.Empresa,
                GrupoConsulta = grupoConsulta
            };

            var data = JsonConvert.SerializeObject(agenda);
            var itfzData = new InterfazData
            {
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };

            interfaz = await ejecucionRepository.AddEjecucion(interfaz, itfzData, connection, tran);

            return interfaz.Id;
        }

        public virtual async Task UpdateAgenda(int nuAgenda, long nroEjecucion, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var template = new
            {
                nuAgenda = nuAgenda,
                nroEjecucion = nroEjecucion,
                Updrow = DateTime.Now,
                nuTransaccion = nuTransaccion,
            };

            var param = new DynamicParameters(template);
            string sql = @"UPDATE T_AGENDA 
                SET NU_INTERFAZ_EJECUCION = :nroEjecucion, 
                    DT_UPDROW = :Updrow,
                    NU_TRANSACCION = :nuTransaccion 
                WHERE NU_AGENDA = :nuAgenda";

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);
        }

        public virtual decimal GetCantidadConsumidaAgenda(RecepcionAgendaReferencia model, DbConnection connection, DbTransaction tran)
        {
            var param = new DynamicParameters(new
            {
                Agenda = model.Agenda,
                IdDetalleReferencia = model.IdDetalleReferencia,
                Empresa = model.Empresa,
                CodigoProducto = model.CodigoProducto,
                Identificador = model.Identificador
            });

            string sql = @"SELECT QT_RECIBIDA
                        FROM T_RECEPCION_AGENDA_REFERENCIA 
                        WHERE NU_AGENDA = :Agenda
                            AND NU_RECEPCION_REFERENCIA_DET = :IdDetalleReferencia
                            AND CD_EMPRESA = :Empresa
                            AND CD_PRODUTO = :CodigoProducto
                            AND NU_IDENTIFICADOR = :Identificador";

            var cantidadRecibida = _dapper.Query<decimal?>(connection, sql, param: param, transaction: tran, commandType: CommandType.Text).FirstOrDefault();

            return cantidadRecibida ?? 0;
        }

        public virtual IEnumerable<Agenda> GetAgenda(IEnumerable<Agenda> agendas)
        {
            IEnumerable<Agenda> resultado = new List<Agenda>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_AGENDA_TEMP (NU_AGENDA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, agendas, transaction: tran);

                    sql = @"SELECT 
                        P.NU_AGENDA AS Id,
                        P.CD_EMPRESA AS IdEmpresa,
                        P.CD_TIPO_DOCUMENTO AS TipoDocumento,
                        P.NU_DOCUMENTO AS NumeroDocumento,
                        P.CD_SITUACAO AS EstadoId,
                        P.CD_PORTA AS CodigoPuerta,
                        P.CD_CLIENTE AS CodigoInternoCliente,
                        P.TP_RECEPCION AS TipoRecepcionInterno,
                        P.NU_PREDIO AS Predio
                        FROM T_AGENDA P 
                        INNER JOIN T_AGENDA_TEMP T ON P.NU_AGENDA = T.NU_AGENDA";

                    resultado = _dapper.Query<Agenda>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<AgendaDetalle> GetDetalleAgenda(IEnumerable<Agenda> agendas)
        {
            IEnumerable<AgendaDetalle> resultado = new List<AgendaDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_AGENDA_TEMP (NU_AGENDA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, agendas, transaction: tran);

                    sql = @"SELECT 
                            P.NU_AGENDA AS IdAgenda,
                            P.CD_PRODUTO AS CodigoProducto,
                            P.NU_IDENTIFICADOR AS Identificador,
                            P.CD_FAIXA AS Faixa,
                            P.CD_EMPRESA AS IdEmpresa,
                            P.CD_SITUACAO AS EstadoId,
                            P.QT_AGENDADO AS CantidadAgendada,
                            P.QT_CROSS_DOCKING AS CantidadCrossDocking,
                            P.DT_FABRICACAO AS Vencimiento,
                            P.VL_PRECIO AS Precio,
                            P.QT_RECIBIDA AS CantidadRecibida,
                            P.DT_ACEPTADA_RECEPCION AS FechaAceptacionRecepcion,
                            P.CD_FUNC_ACEPTO_RECEPCION AS IdUsuarioAceptacionRecepcion,
                            P.DT_ADDROW AS FechaAlta,
                            P.DT_UPDROW AS FechaModificacion,
                            P.QT_ACEPTADA AS CantidadAceptada,
                            P.QT_AGENDADO_ORIGINAL AS CantidadAgendadaOriginal,
                            P.QT_RECIBIDA_FICTICIA AS CantidadRecibidaFicticia,
                            P.VL_CIF AS CIF,
                            P.NU_TRANSACCION AS NumeroTransaccion
                        FROM T_DET_AGENDA P 
                        INNER JOIN T_AGENDA_TEMP T ON P.NU_AGENDA = T.NU_AGENDA";

                    resultado = _dapper.Query<AgendaDetalle>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePendienteCrossDocking> SaldoPendienteXd(IEnumerable<Agenda> agendas)
        {
            IEnumerable<DetallePendienteCrossDocking> resultado = new List<DetallePendienteCrossDocking>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_AGENDA_TEMP (NU_AGENDA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, agendas, transaction: tran);

                    sql = @"SELECT 
                        P.CD_EMPRESA AS Empresa,
                        P.CD_CLIENTE AS Cliente,
                        P.NU_AGENDA AS Id,
                        P.CD_PRODUTO AS Producto,
                        P.NU_IDENTIFICADOR AS Identificador,
                        P.QT_PEND_XD AS CantidadPendiente
                        FROM V_PENDIENTE_XD_UNA_FASE P 
                        INNER JOIN T_AGENDA_TEMP T ON P.NU_AGENDA = T.NU_AGENDA";

                    resultado = _dapper.Query<DetallePendienteCrossDocking>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual void GenerarDetalleAgendaLpn(int nuAgenda, IEnumerable<Lpn> lpns, IEnumerable<AgendaLpnPlanificacion> planificaciones, long nuTransaccion, bool desasociar = false)
        {
            using (var connection = _dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        if (desasociar)
                        {
                            var query = @"UPDATE T_LPN SET NU_AGENDA = :newNroAgenda, NU_TRANSACCION = :nuTransaccion, DT_UPDROW = :FechaModificacion WHERE NU_AGENDA = :NroAgenda";

                            _dapper.Execute(connection, query, param: new
                            {
                                newNroAgenda = (int?)null,
                                NroAgenda = nuAgenda,
                                FechaModificacion = DateTime.Now,
                                nuTransaccion = nuTransaccion
                            }, transaction: tran);
                        }

                        var sql = @"DELETE FROM T_DET_AGENDA WHERE NU_AGENDA = :nuAgenda";
                        _dapper.Execute(connection, sql, param: new { nuAgenda = nuAgenda }, transaction: tran);

                        sql = @"DELETE FROM T_AGENDA_LPN_PLANIFICACION WHERE NU_AGENDA = :nuAgenda";
                        _dapper.Execute(connection, sql, param: new { nuAgenda = nuAgenda }, transaction: tran);

                        if (lpns.Count() > 0)
                        {
                            var detsAgenda = GetDetalleAgendaLpn(lpns);

                            InsertAgendaDetalleLpn(connection, tran, detsAgenda);
                            UpdateAgendaDetallesLpn(connection, tran, lpns, nuAgenda, nuTransaccion);

                            InsertAgendaLpnPlanificacion(connection, tran, planificaciones);

                            sql = @"UPDATE T_LPN SET NU_AGENDA = :NroAgenda, NU_TRANSACCION = :NumeroTransaccion, DT_UPDROW = :FechaModificacion WHERE NU_LPN = :NumeroLPN";
                            _dapper.Execute(connection, sql, lpns, transaction: tran);
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        this.logger.Error($"GenerarDetalleAgendaLpn: {ex}");
                        throw ex;
                    }
                }
            }
        }

        public virtual void InsertAgendaDetalleLpn(DbConnection connection, DbTransaction tran, IEnumerable<AgendaDetalle> detalles)
        {
            string sql = @$"INSERT INTO T_DET_AGENDA
                            (NU_AGENDA,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            CD_SITUACAO,
                            QT_AGENDADO,
                            QT_RECIBIDA,
                            QT_ACEPTADA,
                            QT_AGENDADO_ORIGINAL,
                            QT_RECIBIDA_FICTICIA,
                            QT_CROSS_DOCKING,
                            DT_FABRICACAO,
                            DT_ADDROW,
                            DT_UPDROW,
                            NU_TRANSACCION) 
                            VALUES (
                            :IdAgenda,
                            :IdEmpresa,
                            :CodigoProducto,
                            :Faixa,
                            :Identificador,
                            {EstadoAgendaDb.Abierta},
                            :CantidadAgendada,
                            :CantidadRecibida,
                            :CantidadAceptada,
                            :CantidadAgendadaOriginal,
                            :CantidadRecibidaFicticia,
                            :CantidadCrossDocking,
                            :Vencimiento,
                            :FechaAlta,
                            :FechaModificacion,
                            :NumeroTransaccion)";

            _dapper.Execute(connection, sql, detalles, transaction: tran);
        }

        public virtual void UpdateAgendaDetallesLpn(DbConnection connection, DbTransaction tran, IEnumerable<Lpn> lpns, int nuAgenda, long nuTransaccion)
        {
            var alias = "DA";
            var from = @"T_DET_AGENDA DA
                INNER JOIN (
	                SELECT 
		                LD.CD_PRODUTO, 
		                LD.NU_IDENTIFICADOR, 
		                LD.CD_FAIXA,
		                LD.CD_EMPRESA,
		                SUM(COALESCE(LD.QT_DECLARADA,0)) QT_DISPONIBLE
                    FROM T_LPN_DET LD
                    WHERE LD.NU_LPN = :NumeroLPN 
                    GROUP BY
                        LD.CD_PRODUTO, 
		                LD.NU_IDENTIFICADOR, 
		                LD.CD_FAIXA,
		                LD.CD_EMPRESA
                ) T ON T.CD_PRODUTO = DA.CD_PRODUTO 
	                AND T.NU_IDENTIFICADOR = DA.NU_IDENTIFICADOR 
                    AND T.CD_FAIXA = DA.CD_FAIXA 
                    AND T.CD_EMPRESA = DA.CD_EMPRESA";

            var set = @" 
                QT_AGENDADO = QT_AGENDADO + QT_DISPONIBLE,
	            NU_TRANSACCION = :NumeroTransaccion";

            var where = "NU_AGENDA = :NroAgenda";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: lpns, transaction: tran);

            var sql = @"UPDATE T_DET_AGENDA
                        SET QT_AGENDADO_ORIGINAL = QT_AGENDADO,
                            NU_TRANSACCION = :nuTransaccion 
                    WHERE NU_AGENDA = :nuAgenda ";

            _dapper.Execute(connection, sql, param: new { nuAgenda = nuAgenda, nuTransaccion = nuTransaccion }, transaction: tran);
        }

        public virtual List<AgendaDetalle> GetDetalleAgendaLpn(IEnumerable<Lpn> lpns)
        {
            var resultado = new List<AgendaDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (NU_LPN) VALUES (:NumeroLPN)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = @$"SELECT 
                            :NroAgenda  as IdAgenda,
                            LD.CD_EMPRESA as IdEmpresa,
                            LD.CD_PRODUTO as CodigoProducto,
                            LD.CD_FAIXA as Faixa,
                            LD.NU_IDENTIFICADOR as Identificador,
                            {EstadoAgendaDb.Abierta} as EstadoId,
                            0 as CantidadAgendada,                            
                            0 as CantidadRecibida,
                            0 as CantidadAceptada,
                            0 as CantidadAgendadaOriginal,
                            0 as CantidadRecibidaFicticia,
                            0 as CantidadCrossDocking,
                            MIN(LD.DT_FABRICACAO) as Vencimiento,
                            :FechaModificacion as FechaAlta,
                            :FechaModificacion as FechaModificacion,
                            :nuTransaccion as NumeroTransaccion
                        FROM T_LPN_DET LD 
                        INNER JOIN T_LPN_TEMP T ON LD.NU_LPN = T.NU_LPN 
                        GROUP BY LD.CD_EMPRESA, LD.CD_PRODUTO, LD.CD_FAIXA, LD.NU_IDENTIFICADOR";

                    resultado = _dapper.Query<AgendaDetalle>(connection, sql, param: new
                    {
                        NroAgenda = lpns.FirstOrDefault().NroAgenda,
                        FechaModificacion = lpns.FirstOrDefault().FechaModificacion,
                        nuTransaccion = lpns.FirstOrDefault().NumeroTransaccion
                    }, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<AgendaLpn> GetFotoLpnsAsociados(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT  
                            NU_AGENDA as NroAgenda, 
                            NU_LPN as NroLPN, 
                            CD_EMPRESA as Empresa, 
                            ID_LPN_EXTERNO as IdExterno, 
                            TP_LPN_TIPO as Tipo, 
                            ID_PACKING as IdPacking,
                            DT_ADDROW as FechaInsercion,
                            DT_UPDROW as FechaModificacion
                        FROM T_AGENDA_LPN 
                        WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<AgendaLpn>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<AgendaLpnDetalle> GetFotoDetalleLpnsAsociados(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT  
                            NU_AGENDA as NroAgenda, 
                            NU_LPN as NroLPN, 
                            ID_LPN_DET as IdLpnDetalle, 
                            CD_PRODUTO as CodigoProducto, 
                            CD_FAIXA as Faixa, 
                            NU_IDENTIFICADOR as Identificador, 
                            QT_RECIBIDA as CantidadRecibida, 
                            DT_FABRICACAO as Vencimiento, 
                            ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno, 
                            DT_ADDROW as FechaInsercion,
                            DT_UPDROW as FechaModificacion
                        FROM T_AGENDA_LPN_DET 
                        WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<AgendaLpnDetalle>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<AgendaLpnAtributo> GetFotoAtributosCabezalLpnsAsociados(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT  
                            NU_AGENDA as NroAgenda, 
                            NU_LPN as NroLPN, 
                            ID_ATRIBUTO as IdAtributo, 
                            NM_ATRIBUTO as Nombre, 
                            VL_LPN_ATRIBUTO as Valor, 
                            DT_ADDROW as FechaInsercion,
                            DT_UPDROW as FechaModificacion
                        FROM T_AGENDA_LPN_ATRIBUTO 
                        WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<AgendaLpnAtributo>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<AgendaLpnDetalleAtributo> GetFotoAtributosDetalleLpnsAsociados(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT  
                            NU_AGENDA as NroAgenda, 
                            NU_LPN as NroLPN, 
                            ID_LPN_DET as IdLpnDetalle, 
                            CD_PRODUTO as CodigoProducto, 
                            CD_FAIXA as Faixa, 
                            NU_IDENTIFICADOR as Identificador, 
                            ID_ATRIBUTO as IdAtributo, 
                            NM_ATRIBUTO as Nombre, 
                            VL_LPN_DET_ATRIBUTO as Valor, 
                            DT_ADDROW as FechaInsercion,
                            DT_UPDROW as FechaModificacion
                        FROM T_AGENDA_LPN_DET_ATRIBUTO 
                        WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<AgendaLpnDetalleAtributo>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void AddFotoAgendaLpn(int nuAgenda)
        {
            var param = new DynamicParameters(new
            {
                nuAgenda = nuAgenda,
                fechaInsercion = DateTime.Now,
            });

            string sql = @" INSERT INTO T_AGENDA_LPN
                            (
                                NU_AGENDA,
                                NU_LPN,
                                CD_EMPRESA,
                                ID_LPN_EXTERNO,
                                TP_LPN_TIPO,
                                ID_PACKING,
                                DT_ADDROW
                            )
                            SELECT
                                :nuAgenda,
                                L.NU_LPN,
                                L.CD_EMPRESA,
                                L.ID_LPN_EXTERNO,
                                L.TP_LPN_TIPO,
                                L.ID_PACKING,
                                :fechaInsercion
                            FROM T_LPN L
                            INNER JOIN T_AGENDA_LPN_PLANIFICACION ALP ON L.NU_LPN = ALP.NU_LPN
                            WHERE L.NU_AGENDA = :nuAgenda AND ALP.FL_RECIBIDO = 'S' ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @" INSERT INTO T_AGENDA_LPN_DET
                    (
                        NU_AGENDA,
                        NU_LPN,
                        ID_LPN_DET,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        QT_RECIBIDA,
                        DT_FABRICACAO,
                        ID_LINEA_SISTEMA_EXTERNO,
                        DT_ADDROW
                    )
                    SELECT
                        :nuAgenda,
                        LD.NU_LPN,
                        LD.ID_LPN_DET,
                        LD.CD_PRODUTO,
                        LD.CD_FAIXA,
                        LD.NU_IDENTIFICADOR,
                        LD.QT_RECIBIDA,
                        LD.DT_FABRICACAO,
                        LD.ID_LINEA_SISTEMA_EXTERNO,
                        :fechaInsercion
                    FROM T_LPN_DET LD
                    INNER JOIN T_LPN L ON LD.NU_LPN = L.NU_LPN
                    INNER JOIN T_AGENDA_LPN_PLANIFICACION ALP ON L.NU_LPN = ALP.NU_LPN
                    WHERE L.NU_AGENDA = :nuAgenda AND ALP.FL_RECIBIDO = 'S' ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @" INSERT INTO T_AGENDA_LPN_ATRIBUTO
                    (
                        NU_AGENDA,
                        NU_LPN,
                        ID_ATRIBUTO,
                        NM_ATRIBUTO,
                        VL_LPN_ATRIBUTO,
                        DT_ADDROW
                    )
                    SELECT
                        :nuAgenda,
                        LA.NU_LPN,
                        LA.ID_ATRIBUTO,
                        A.NM_ATRIBUTO,
                        LA.VL_LPN_ATRIBUTO,
                        :fechaInsercion
                    FROM T_LPN_ATRIBUTO LA
                    INNER JOIN T_ATRIBUTO A ON LA.ID_ATRIBUTO = A.ID_ATRIBUTO
                    INNER JOIN T_LPN L ON LA.NU_LPN = L.NU_LPN
                    INNER JOIN T_AGENDA_LPN_PLANIFICACION ALP ON L.NU_LPN = ALP.NU_LPN
                    WHERE L.NU_AGENDA = :nuAgenda AND ALP.FL_RECIBIDO = 'S' ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @" INSERT INTO T_AGENDA_LPN_DET_ATRIBUTO
                    (
                        NU_AGENDA,
                        NU_LPN,
                        ID_LPN_DET,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        ID_ATRIBUTO,
                        NM_ATRIBUTO,
                        VL_LPN_DET_ATRIBUTO,
                        DT_ADDROW
                    )
                    SELECT
                        :nuAgenda,
                        LDA.NU_LPN,
                        LDA.ID_LPN_DET,
                        LDA.CD_PRODUTO,
                        LDA.CD_FAIXA,
                        LDA.NU_IDENTIFICADOR,
                        LDA.ID_ATRIBUTO,
                        A.NM_ATRIBUTO,
                        LDA.VL_LPN_DET_ATRIBUTO,
                        :fechaInsercion
                    FROM T_LPN_DET_ATRIBUTO LDA
                    INNER JOIN T_LPN_DET LD ON LDA.NU_LPN = LD.NU_LPN AND LDA.ID_LPN_DET = LD.ID_LPN_DET 
                    INNER JOIN T_ATRIBUTO A ON LDA.ID_ATRIBUTO = A.ID_ATRIBUTO
                    INNER JOIN T_LPN L ON LD.NU_LPN = L.NU_LPN
                    INNER JOIN T_AGENDA_LPN_PLANIFICACION ALP ON L.NU_LPN = ALP.NU_LPN
                    WHERE L.NU_AGENDA = :nuAgenda AND ALP.FL_RECIBIDO = 'S' ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void GenerarPlanificacionAgendaLpn(int nuAgenda, IEnumerable<AgendaLpnPlanificacion> planificaciones, long nuTransaccion, bool desasociar = false)
        {
            using (var connection = _dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        if (desasociar)
                        {
                            var query = @"UPDATE T_LPN SET NU_AGENDA = :newNroAgenda, NU_TRANSACCION = :nuTransaccion, DT_UPDROW = :FechaModificacion WHERE NU_AGENDA = :NroAgenda";

                            _dapper.Execute(connection, query, param: new
                            {
                                newNroAgenda = (int?)null,
                                NroAgenda = nuAgenda,
                                FechaModificacion = DateTime.Now,
                                nuTransaccion = nuTransaccion
                            }, transaction: tran);
                        }

                        var sql = @"DELETE FROM T_AGENDA_LPN_PLANIFICACION WHERE NU_AGENDA = :nuAgenda";
                        _dapper.Execute(connection, sql, param: new { nuAgenda = nuAgenda }, transaction: tran);

                        if (planificaciones.Count() > 0)
                        {
                            InsertAgendaLpnPlanificacion(connection, tran, planificaciones);

                            sql = @"UPDATE T_LPN SET NU_AGENDA = :NroAgenda, NU_TRANSACCION = :NumeroTransaccion, DT_UPDROW = :FechaModificacion WHERE NU_LPN = :NroLPN";
                            _dapper.Execute(connection, sql, planificaciones, transaction: tran);
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        this.logger.Error($"GenerarPlanificacionAgendaLpn: {ex}");
                        throw ex;
                    }
                }
            }
        }

        public virtual void InsertAgendaLpnPlanificacion(DbConnection connection, DbTransaction tran, IEnumerable<AgendaLpnPlanificacion> planificaciones)
        {
            var sql = @$"INSERT INTO T_AGENDA_LPN_PLANIFICACION
                            (NU_AGENDA,
                            NU_LPN,
                            FL_PLANIFICADO,
                            FL_RECIBIDO,
                            CD_FUNCIONARIO,
                            CD_FUNCIONARIO_RECEPCION,
                            DT_RECEPCION,
                            DT_ADDROW,
                            DT_UPDROW) 
                            VALUES (
                            :NroAgenda,
                            :NroLPN,
                            :Planificado,
                            :Recibido,
                            :Funcionario,
                            :FuncionarioRecepcion,
                            :FechaRecepcion,
                            :FechaInsercion,
                            :FechaModificacion)";

            _dapper.Execute(connection, sql, planificaciones, transaction: tran);
        }

        public virtual void DesvincularLpnsNoRecibidos(int nuAgenda, long nuTransaccion)
        {
            var sql = @"SELECT  
                            NU_LPN as NumeroLPN, 
                            NULL as NroAgenda,
                            :FechaModificacion as FechaModificacion,
                            :nuTransaccion as NumeroTransaccion
                        FROM T_AGENDA_LPN_PLANIFICACION 
                        WHERE NU_AGENDA = :nuAgenda AND FL_RECIBIDO = 'N' ";

            var lpnsSinRecibir = _dapper.Query<Lpn>(_context.Database.GetDbConnection(), sql, param: new
            {
                nuAgenda = nuAgenda,
                newNroAgenda = (int?)null,
                FechaModificacion = DateTime.Now,
                nuTransaccion = nuTransaccion
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            var query = @"UPDATE T_LPN SET NU_AGENDA = :NroAgenda, NU_TRANSACCION = :NumeroTransaccion, DT_UPDROW = :FechaModificacion WHERE NU_LPN = :NumeroLPN";

            _dapper.Execute(_context.Database.GetDbConnection(), query, lpnsSinRecibir, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion
    }
}