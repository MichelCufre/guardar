using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Documento;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Liberacion;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class CrossDockingRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly CrossDockingMapper _mapper;
        protected readonly EtiquetaLoteMapper _etiquetaLoteMapper;
        protected readonly OndaMapper _ondaMapper;
        protected readonly IDapper _dapper;
        protected ParametroRepository _paramRepo;

        public CrossDockingRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new CrossDockingMapper();
            this._paramRepo = new ParametroRepository(_context, cdAplicacion, userId, dapper);
            this._ondaMapper = new OndaMapper();
            this._dapper = dapper;
            this._etiquetaLoteMapper = new EtiquetaLoteMapper();
        }

        #region Any

        public virtual bool AnyCrossDocking(int agenda, string tipo, string estado, bool inverso = false)
        {
            return _context.T_CROSS_DOCK.AsNoTracking()
                .Any(c => c.NU_AGENDA == agenda && c.TP_CROSS_DOCKING == tipo
                && (inverso ? (c.ND_ESTADO != estado) : c.ND_ESTADO == estado));
        }

        public virtual bool CargaFacturada(string cliente, int empresa, long? carga)
        {
            return _context.V_FACTU_SIN_EXPEDIR_REC270.AsNoTracking().Any(x => x.CD_CLIENTE == cliente && x.CD_EMPRESA == empresa && x.NU_CARGA == carga);
        }

        public virtual bool PermiteEditarCargaDocumental(string cliente, int empresa, long? carga)
        {
            return _context.V_CARGAS_EGRESO_DOCUMENTAL.AsNoTracking().Any(x => x.CD_CLIENTE == cliente && x.CD_EMPRESA == empresa && x.NU_CARGA == carga);
        }

        public virtual bool CargaPedidoExpedida(string pedido, string cliente, int empresa, long? carga)
        {
            return _context.V_PED_CAR_EXP_REC270.AsNoTracking().Any(x => x.NU_PEDIDO == pedido && x.CD_CLIENTE == cliente && x.CD_EMPRESA == empresa && x.NU_CARGA == carga);
        }

        public virtual bool AnyAgendaEnCrossDock(int numeroAgenda)
        {
            return _context.T_CROSS_DOCK
                .AsNoTracking()
                .Any(d => d.NU_AGENDA == numeroAgenda);
        }

        public virtual bool AnyAgendaEnCrossDockConDetalles(int numeroAgenda)
        {
            return _context.T_CROSS_DOCK
                .AsNoTracking()
                .Any(d => d.NU_AGENDA == numeroAgenda && d.ND_ESTADO != EstadoCrossDockingDb.EnEdicion);
        }

        public virtual bool AnyAgendaEnCrossDockActivo(int numeroAgenda)
        {
            return _context.T_CROSS_DOCK
                .AsNoTracking()
                .Any(d => d.NU_AGENDA == numeroAgenda
                    && (d.ND_ESTADO == EstadoCrossDockingDb.Iniciado || d.ND_ESTADO == EstadoCrossDockingDb.Finalizado));
        }

        public virtual bool AnyAgendaEnCrossDockIniciado(int numeroAgenda)
        {
            return _context.T_CROSS_DOCK
                .AsNoTracking()
                .Any(d => d.NU_AGENDA == numeroAgenda && d.ND_ESTADO == EstadoCrossDockingDb.Iniciado);
        }

        public virtual bool HayTiposCrossDockDisponibles(EstadoAgenda estado)
        {
            bool ok = _context.T_TIPO_CROSS_DOCK
                .AsNoTracking()
                .Any(w => ((estado == EstadoAgenda.Cerrada && w.FL_REQUIERE_CIERRE_AGENDA == "S")
                    || (estado == EstadoAgenda.AguardandoDesembarque && w.FL_REQUIERE_LIBERACION_AGENDA == "S" && w.FL_REQUIERE_CIERRE_AGENDA == "N")
                    || ((estado == EstadoAgenda.Abierta || estado == EstadoAgenda.DocumentoAsociado) && w.FL_REQUIERE_LIBERACION_AGENDA == "N" && w.FL_REQUIERE_CIERRE_AGENDA == "N"))
                 && w.FL_ACTIVO == "S");

            return ok;
        }

        public virtual bool PuedeEditarTpCrossDocking(int empresa, int agenda, string predio)
        {
            return !_context.V_REC200_SELECCION_CROSS_DOCK.AsNoTracking().Any(d => d.CD_EMPRESA == empresa && d.NU_AGENDA == agenda && d.NU_PREDIO == predio);
        }

        #endregion

        #region Get

        public virtual ICrossDocking GetCrossDockingByAgenda(int nroAgenda)
        {
            T_CROSS_DOCK entity = this._context.T_CROSS_DOCK
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_AGENDA == nroAgenda);

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual ICrossDocking GetCrossDockingActivoByAgenda(int nroAgenda)
        {
            T_CROSS_DOCK entity = this._context.T_CROSS_DOCK
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_AGENDA == nroAgenda && d.ND_ESTADO != EstadoCrossDockingDb.Finalizado);

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual ICrossDocking GetCrossDockingActivoByAgendaTipo(int nroAgenda, string tipo)
        {
            var entity = this._context.T_CROSS_DOCK
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_AGENDA == nroAgenda && d.TP_CROSS_DOCKING == tipo);

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual Onda GetOndaCrossDocking()
        {
            T_ONDA entity = null;

            if (short.TryParse(this._paramRepo.GetParameter("ONDA_DEFAULT_CROSSDOCKING"), out short onda))
                entity = this._context.T_ONDA.AsNoTracking().FirstOrDefault(d => d.CD_ONDA == onda);

            if (entity == null)
                entity = this._context.T_ONDA.AsNoTracking().OrderBy(d => d.CD_ONDA).FirstOrDefault();

            if (entity == null)
                return null;

            return this._ondaMapper.MapToObject(entity);
        }

        public virtual ICrossDocking GetCrossDockingIniciadoByAgenda(int nroAgenda)
        {
            T_CROSS_DOCK entity = this._context.T_CROSS_DOCK
                .AsNoTracking()
                .Where(d => d.NU_AGENDA == nroAgenda && d.ND_ESTADO == EstadoCrossDockingDb.Iniciado)
                .FirstOrDefault();

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual List<LineaCrossDocking> GetDetalleCrossDock(string P_Produto, decimal P_Faixa, int P_Agenda, int P_Prep, long P_Carga, string P_Pedido, string P_Cliente, string P_Identif, string P_Espec_Identif)
        {
            List<LineaCrossDocking> lista = new List<LineaCrossDocking>();
            List<T_DET_CROSS_DOCK> List = _context.T_DET_CROSS_DOCK
                .AsNoTracking()
                .Where(x => x.CD_PRODUTO == P_Produto
                    && x.CD_FAIXA == P_Faixa
                    && x.NU_AGENDA == P_Agenda
                    && x.NU_PREPARACION == P_Prep
                    && x.NU_CARGA == P_Carga
                    && x.NU_PEDIDO == P_Pedido
                    && x.CD_CLIENTE == P_Cliente
                    && x.NU_IDENTIFICADOR == P_Identif
                    && x.ID_ESPECIFICA_IDENTIFICADOR == P_Espec_Identif)
                .ToList();

            foreach (var det in List)
            {
                lista.Add(this._mapper.MapToObject(det));
            }

            return lista;
        }

        public virtual List<LineaCrossDocking> GetDetalleCrossDock(int pNroAgenda, int nU_PREPARACION, string nroPedido, string pCdCliente, int pCdEmpresa, long? nroCarga, string cD_PRODUTO, decimal cD_FAIXA, string nU_IDENTIFICADOR, string especificaId, decimal? cantPedido)
        {
            List<LineaCrossDocking> lista = null;
            List<T_DET_CROSS_DOCK> listaLineaCrossDock = _context.T_DET_CROSS_DOCK.AsNoTracking()
                 .Where(dcd => dcd.NU_AGENDA == pNroAgenda
                            && dcd.NU_PREPARACION == nU_PREPARACION
                           && dcd.NU_PEDIDO == nroPedido
                           && dcd.CD_CLIENTE == pCdCliente
                           && dcd.CD_EMPRESA == pCdEmpresa
                           && dcd.NU_CARGA == (nroCarga ?? 0)
                           && dcd.CD_PRODUTO == cD_PRODUTO
                           && dcd.CD_FAIXA == cD_FAIXA
                           && dcd.NU_IDENTIFICADOR == nU_IDENTIFICADOR
                           && dcd.ID_ESPECIFICA_IDENTIFICADOR == especificaId
                           && dcd.QT_PRODUTO == cantPedido)
                 .ToList();
            if (listaLineaCrossDock == null)
            {
                return null;
            }
            else
            {
                lista = new List<LineaCrossDocking>();
                foreach (var reg in listaLineaCrossDock)
                {
                    LineaCrossDocking det = this._mapper.MapToObject(reg);
                    lista.Add(det);
                }
                return lista;
            }
        }

        public virtual List<LineaCrossDocking> GetDetalleCrossDock(string cdProducto, decimal cdFaixa, string nroIdentificador, int nroAgenda, string cdCliente)
        {
            List<LineaCrossDocking> lista = null;
            List<T_DET_CROSS_DOCK> listaLineaCrossDock = _context.T_DET_CROSS_DOCK.AsNoTracking()
                 .Where(dcd => dcd.CD_PRODUTO == cdProducto
                     && dcd.CD_FAIXA == cdFaixa
                     && (dcd.NU_IDENTIFICADOR == nroIdentificador || dcd.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                     && dcd.NU_AGENDA == nroAgenda
                     && dcd.CD_CLIENTE == cdCliente)
                 .ToList();
            if (listaLineaCrossDock == null)
            {
                return null;
            }
            else
            {
                lista = new List<LineaCrossDocking>();
                foreach (var reg in listaLineaCrossDock)
                {
                    LineaCrossDocking det = this._mapper.MapToObject(reg);
                    lista.Add(det);
                }
                return lista;
            }

        }

        public virtual IEnumerable<LineaCrossDocking> GetLineasCrossDockingParaFinalizar(int nroAgenda, out int prep)
        {
            prep = -1;

            var crossDocking = this._context.T_CROSS_DOCK
                .Include("T_DET_CROSS_DOCK")
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_AGENDA == nroAgenda &&
                    d.ND_ESTADO == EstadoCrossDockingDb.Iniciado);

            var lineasCrossDocking = crossDocking.T_DET_CROSS_DOCK
                .Select(w => this._mapper.MapToObject(w));

            prep = crossDocking.NU_PREPARACION;

            return lineasCrossDocking;
        }

        public virtual DetalleDisponibleCrossDocking GetDetalleDisponible(int nroAgenda, int empresa, string producto, decimal faixa, string identificador)
        {
            var entity = this._context.V_DISPONIBLE_CROSS_DOCKING
                .FirstOrDefault(x => x.NU_AGENDA == nroAgenda
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.QT_DISPONIBLE > 0);

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual List<CrossDockingTemp> GetCrossDockTempWrec220(int empresa, int agenda)
        {
            List<CrossDockingTemp> list = new List<CrossDockingTemp>();
            List<V_CROSS_DOCK_TEMP_WREC220> listaCD = _context.V_CROSS_DOCK_TEMP_WREC220.AsNoTracking().Where(e => e.CD_EMPRESA == empresa && e.NU_AGENDA == agenda).OrderBy(o => o.CD_ROTA).ThenBy(tb => tb.NU_AGENDA).ThenBy(b => b.CD_CLIENTE).ToList();
            foreach (var det in listaCD)
            {
                list.Add(_mapper.MapToObject(det));
            }

            return list;
        }

        public virtual CrossDockingTemp GetCrosDockingTemp(int nroAgenda, DetallePedido dpsps, AgendaDetalle daa)
        {
            string especificaindent = "N";
            if (dpsps.EspecificaIdentificador)
            {
                especificaindent = "S";
            }
            T_CROSS_DOCK_TEMP entity = _context.T_CROSS_DOCK_TEMP.AsNoTracking().FirstOrDefault(dct => dct.NU_AGENDA == nroAgenda
                          && dct.NU_PEDIDO == dpsps.Id
                          && dct.CD_CLIENTE == dpsps.Cliente
                          && dct.CD_EMPRESA == daa.IdEmpresa
                          && dct.CD_PRODUTO == daa.CodigoProducto
                          && dct.CD_FAIXA == daa.Faixa
                          && dct.NU_IDENTIFICADOR == daa.Identificador
                          && dct.ID_ESPECIFICA_IDENTIFICADOR == especificaindent)
                          ?? _context.T_CROSS_DOCK_TEMP.Local.FirstOrDefault(dct => dct.NU_AGENDA == nroAgenda
                          && dct.NU_PEDIDO == dpsps.Id
                          && dct.CD_CLIENTE == dpsps.Cliente
                          && dct.CD_EMPRESA == daa.IdEmpresa
                          && dct.CD_PRODUTO == daa.CodigoProducto
                          && dct.CD_FAIXA == daa.Faixa
                          && dct.NU_IDENTIFICADOR == daa.Identificador
                          && dct.ID_ESPECIFICA_IDENTIFICADOR == especificaindent)
                        ;
            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual CrossDockingTemp GetCrossDockTemp(string pedid, int emp, string cliente, string ident, string prod, int age, decimal faixa, bool ID_ESPECIFICA_IDENTIFICADOR)
        {
            string s_ID_ESPECIFICA_IDENTIFICADOR = ID_ESPECIFICA_IDENTIFICADOR ? "S" : "N";
            T_CROSS_DOCK_TEMP attachedEntity = _context.T_CROSS_DOCK_TEMP.AsNoTracking().Where(cdt => cdt.CD_PRODUTO == prod
              && cdt.CD_FAIXA == faixa
              && cdt.NU_IDENTIFICADOR == ident
              && cdt.NU_AGENDA == age
              && cdt.CD_CLIENTE == cliente
              && cdt.CD_EMPRESA == emp
              && cdt.NU_PEDIDO == pedid
              && cdt.ID_ESPECIFICA_IDENTIFICADOR == s_ID_ESPECIFICA_IDENTIFICADOR).FirstOrDefault();

            return this._mapper.MapToObject(attachedEntity);
        }

        public virtual List<SelectOption> GetTpCrossDockingDisponiblesForSelect(EstadoAgenda estado)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            List<T_TIPO_CROSS_DOCK> tiposDisponibles = _context.T_TIPO_CROSS_DOCK
                .AsNoTracking()
                .Where(w => ((estado == EstadoAgenda.Cerrada && w.FL_REQUIERE_CIERRE_AGENDA == "S")
                    || (estado == EstadoAgenda.AguardandoDesembarque && w.FL_REQUIERE_LIBERACION_AGENDA == "S" && w.FL_REQUIERE_CIERRE_AGENDA == "N")
                    || ((estado == EstadoAgenda.Abierta || estado == EstadoAgenda.DocumentoAsociado) && w.FL_REQUIERE_LIBERACION_AGENDA == "N" && w.FL_REQUIERE_CIERRE_AGENDA == "N"))
                    && w.FL_ACTIVO == "S")
                .ToList();

            foreach (T_TIPO_CROSS_DOCK tipo in tiposDisponibles)
            {
                opciones.Add(new SelectOption()
                {
                    Label = tipo.DS_TIPO_CROSS_DOCK,
                    Value = tipo.CD_TIPO_CROSS_DOCK
                });
            }

            return opciones;
        }

        public virtual List<AgendaRec220> GetAgendaCrossDockingREC270(string value)
        {
            int agenda;
            if (int.TryParse(value, out agenda))
            {
                return _context.V_AGENDAS_WREC270
                    .AsNoTracking()
                    .Where(d => (d.NU_DOCUMENTO.ToLower().Contains(value.ToLower()) || d.NU_AGENDA.ToString().Contains(agenda.ToString())) && d.CD_SITUACAO == 4)
                    .Take(10)
                    .ToList()
                    .Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
            else
            {
                return _context.V_AGENDAS_WREC270
                    .AsNoTracking()
                    .Where(d => d.NU_DOCUMENTO.ToLower().Contains(value.ToLower()) && d.CD_SITUACAO == 4)
                    .Take(10)
                    .ToList()
                    .Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
        }

        public virtual List<EtiquetaPreSep> GetListaDeEtiquetasAConvertir(int nroAgenda)
        {
            List<EtiquetaPreSep> list = new List<EtiquetaPreSep>();
            List<V_ETIQUETA_PRE_SEP_WREC270> listaEtiquetas = _context.V_ETIQUETA_PRE_SEP_WREC270
                          .AsNoTracking().Where(el => el.MAX_SITUACAO == SituacionDb.AgendaConferidaSinDiferencia
                              && el.ID_CTRL_ACEPTADO == "S"
                              && el.CD_CLIENTE != null
                              && el.QT_PRODUTO > 0
                              && el.CD_SITUACAO == SituacionDb.PalletConferido
                              && el.NU_AGENDA == nroAgenda)
                          .ToList();
            foreach (var det in listaEtiquetas)
            {
                EtiquetaPreSep new_reg = this._mapper.MapToObject(det);
                list.Add(new_reg);
            }
            return list;
        }

        public virtual List<EtiquetaLoteDetalle> GetDetalleEtiqueta(int pNroEtiqueta)
        {
            List<EtiquetaLoteDetalle> list = new List<EtiquetaLoteDetalle>();
            List<T_DET_ETIQUETA_LOTE> detalles = _context.T_DET_ETIQUETA_LOTE.AsNoTracking()
                      .Where(del => del.NU_ETIQUETA_LOTE == pNroEtiqueta && del.QT_PRODUTO > 0)
                      .ToList();
            foreach (var det in detalles)
            {
                EtiquetaLoteDetalle new_reg = this._etiquetaLoteMapper.MapToObject(det);
                list.Add(new_reg);
            }
            return list;

        }

        public virtual EtiquetaLoteDetalle GetDetalleEtiqueta(int pNroEtiqueta, string CD_PRODUTO, decimal CD_FAIXA, int pCdEmpresa, string NU_IDENTIFICADOR)
        {
            T_DET_ETIQUETA_LOTE etiquetaDisminuir = _context.T_DET_ETIQUETA_LOTE
                         .AsNoTracking().Where(del => del.NU_ETIQUETA_LOTE == pNroEtiqueta
                             && del.CD_PRODUTO == CD_PRODUTO
                             && del.CD_FAIXA == CD_FAIXA
                             && del.CD_EMPRESA == pCdEmpresa
                             && del.NU_IDENTIFICADOR == NU_IDENTIFICADOR)
                         .FirstOrDefault();

            return this._etiquetaLoteMapper.MapToObject(etiquetaDisminuir);
        }

        public virtual List<EtiquetaLote> GetListaDeEtiquetas(int pNroEtiqueta)
        {
            List<EtiquetaLote> listadet = new List<EtiquetaLote>();
            List<T_ETIQUETA_LOTE> lista = _context.T_ETIQUETA_LOTE.AsNoTracking().Where(el => el.NU_ETIQUETA_LOTE == pNroEtiqueta).ToList();
            foreach (var det in lista)
            {
                listadet.Add(this._etiquetaLoteMapper.MapToObject(det));
            }
            return listadet;
        }

        public virtual short GetLastNumeroOrdenLiberacion(int nroPreparacion)
        {
            return this._context.T_PEDIDO_SAIDA
                 .AsNoTracking()
                 .Where(d => d.NU_PREPARACION_PROGRAMADA == nroPreparacion)
                 .DefaultIfEmpty()
                 .Max(d => d.NU_ORDEN_LIBERACION ?? 0);
        }

        public virtual LineaCrossDocking GetLineaCrossDocking(int agenda, int numeroPreparacion, string cliente, string producto, string pedido, decimal faixa, string lote, int empresa, int numeroPreparacionPickeo)
        {

            return this._mapper.MapToObject(
                 _context.T_DET_CROSS_DOCK.FirstOrDefault(w =>
                    w.NU_AGENDA == agenda &&
                    w.NU_PREPARACION == numeroPreparacion &&
                    w.CD_CLIENTE == cliente &&
                    w.CD_PRODUTO == producto &&
                    w.NU_PEDIDO == pedido &&
                    w.CD_FAIXA == faixa &&
                    w.NU_IDENTIFICADOR == lote &&
                    w.CD_EMPRESA == empresa &&
                    w.NU_PREPARACION_PICKEO == numeroPreparacionPickeo
                ));
        }

        public virtual decimal? GetCantidadPreparada(string cdProducto, decimal cdFaixa, string nroIdentificador, int cdEmpresa, string cdCliente, string NU_PEDIDO, long NU_CARGA, int nroPreparacion)
        {
            return _context.T_DET_PICKING
                    .Where(dp => dp.CD_PRODUTO == cdProducto
                        && dp.CD_FAIXA == cdFaixa
                        && dp.NU_IDENTIFICADOR == nroIdentificador
                        && dp.CD_EMPRESA == cdEmpresa
                        && dp.CD_CLIENTE == cdCliente
                        && dp.NU_PEDIDO == NU_PEDIDO
                        && dp.NU_CARGA == NU_CARGA
                        && dp.NU_PREPARACION == nroPreparacion)
                    .Sum(dp => dp.QT_PREPARADO);
        }

        #endregion

        #region Add

        public virtual void AddCrossDocking(ICrossDocking crossDock)
        {
            T_CROSS_DOCK entity = this._mapper.MapToEntity(crossDock);

            this._context.T_CROSS_DOCK.Add(entity);
        }

        public virtual void AddTempCrossDockingData(CrossDockingTemp detCrossDockTemp)
        {
            T_CROSS_DOCK_TEMP entity = _mapper.MapToEntity(detCrossDockTemp);
            _context.T_CROSS_DOCK_TEMP.Add(entity);
        }

        public virtual void AddLineaCrossDocking(LineaCrossDocking linea)
        {
            T_DET_CROSS_DOCK entity = this._mapper.MapToEntity(linea);

            this._context.T_DET_CROSS_DOCK.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateCrossDocking(ICrossDocking crossDock)
        {
            crossDock.FechaModificacion = DateTime.Now;

            T_CROSS_DOCK entity = this._mapper.MapToEntity(crossDock);
            T_CROSS_DOCK attachedEntity = _context.T_CROSS_DOCK.Local
                .FirstOrDefault(x => x.NU_AGENDA == entity.NU_AGENDA && x.NU_PREPARACION == entity.NU_PREPARACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_CROSS_DOCK.Attach(entity);
                _context.Entry<T_CROSS_DOCK>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateCrossDockingTemporal(CrossDockingTemp obj)
        {

            T_CROSS_DOCK_TEMP entity = this._mapper.MapToEntity(obj);

            T_CROSS_DOCK_TEMP attachedEntity = _context.T_CROSS_DOCK_TEMP.Local.FirstOrDefault(x =>
                        x.NU_AGENDA == entity.NU_AGENDA
                        && x.CD_CLIENTE == entity.CD_CLIENTE
                        && x.NU_PEDIDO == entity.NU_PEDIDO
                        && x.CD_PRODUTO == entity.CD_PRODUTO
                        && x.CD_FAIXA == entity.CD_FAIXA
                        && x.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                        && x.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                        && x.CD_EMPRESA == entity.CD_EMPRESA
            );

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                //attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_CROSS_DOCK_TEMP.Attach(entity);
                _context.Entry<T_CROSS_DOCK_TEMP>(entity).State = EntityState.Modified;
            }

        }

        public virtual void UpdateDetalleCrossDocking(LineaCrossDocking det)
        {
            T_DET_CROSS_DOCK entity = this._mapper.MapToEntity(det);

            T_DET_CROSS_DOCK attachedEntity = _context.T_DET_CROSS_DOCK.Local.FirstOrDefault(w =>
                            w.NU_AGENDA == entity.NU_AGENDA &&
                w.NU_PREPARACION == entity.NU_PREPARACION &&
                w.CD_CLIENTE == entity.CD_CLIENTE &&
                w.CD_PRODUTO == entity.CD_PRODUTO &&
                w.NU_PEDIDO == entity.NU_PEDIDO &&
                w.CD_FAIXA == entity.CD_FAIXA &&
                w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR &&
                w.CD_EMPRESA == entity.CD_EMPRESA &&
                w.NU_PREPARACION_PICKEO == entity.NU_PREPARACION_PICKEO
            );

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_CROSS_DOCK.Attach(entity);
                _context.Entry<T_DET_CROSS_DOCK>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveCrossDocking(ICrossDocking crossDocking)
        {
            T_CROSS_DOCK entity = this._mapper.MapToEntity(crossDocking);
            T_CROSS_DOCK attachedEntity = _context.T_CROSS_DOCK.Local
                .FirstOrDefault(x => x.NU_AGENDA == entity.NU_AGENDA && x.NU_PREPARACION == entity.NU_PREPARACION);

            if (attachedEntity != null)
            {
                this._context.T_CROSS_DOCK.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CROSS_DOCK.Attach(entity);
                this._context.T_CROSS_DOCK.Remove(entity);
            }
        }

        public virtual void EliminarTemporal(CrossDockingTemp fila)
        {
            T_CROSS_DOCK_TEMP entity = this._mapper.MapToEntity(fila);

            T_CROSS_DOCK_TEMP attachedEntity = _context.T_CROSS_DOCK_TEMP.Local.Where(cdt => cdt.CD_PRODUTO == fila.CD_PRODUTO
                 && cdt.CD_FAIXA == fila.CD_FAIXA
                 && cdt.NU_IDENTIFICADOR == fila.NU_IDENTIFICADOR
                 && cdt.NU_AGENDA == fila.NU_AGENDA
                 && cdt.CD_CLIENTE == fila.CD_CLIENTE
                 && cdt.NU_PEDIDO == fila.NU_PEDIDO
                 && cdt.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR).FirstOrDefault();

            if (attachedEntity != null)
            {
                _context.T_CROSS_DOCK_TEMP.Remove(attachedEntity);
            }
            else
            {
                _context.T_CROSS_DOCK_TEMP.Attach(entity);
                _context.T_CROSS_DOCK_TEMP.Remove(entity);
            }
        }

        #endregion

        #region Dapper 

        #region API CrossDocking 

        public virtual IEnumerable<CrossDockingEnUnaFase> GetCrossDockingAgendaActivos(IEnumerable<Agenda> agendas)
        {
            IEnumerable<CrossDockingEnUnaFase> resultado = new List<CrossDockingEnUnaFase>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_AGENDA_TEMP (NU_AGENDA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, agendas, transaction: tran);

                    sql = @"SELECT 
                        P.NU_AGENDA AS Agenda,
                        P.NU_PREPARACION AS Preparacion,
                        P.TP_CROSS_DOCKING AS IdTipo
                        FROM T_CROSS_DOCK P 
                        INNER JOIN T_AGENDA_TEMP T ON P.NU_AGENDA = T.NU_AGENDA
                        WHERE P.ND_ESTADO = :Estado";

                    resultado = _dapper.Query<CrossDockingEnUnaFase>(connection, sql, param: new { Estado = EstadoCrossDockingDb.Iniciado }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task ProcesarCrossDockingUnaFase(List<CrossDockingUnaFase> detalles, ICrossDockingServiceContext context, long nuTransaccion, int userId, IBarcodeService barcodeService, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                var bulkContext = GetBulkOperationContext(detalles, context, connection, nuTransaccion, userId, barcodeService);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertEtiquetaLote(connection, tran, bulkContext.EtiquetaLote);
                    await BulkInsertEtiquetaLoteEnUso(connection, tran, bulkContext.EtiquetaLote);
                    await BulkInsertDetalleEtiqueta(connection, tran, bulkContext.InsertDetallesEtiquetaLote);
                    await BulkUpdateDetalleEtiqueta(connection, tran, bulkContext.UpdateDetallesEtiquetaLote);

                    await BulkInsertLogDetalleEtiqueta(connection, tran, bulkContext.InsertDetallesEtiquetaLote);
                    await BulkInsertLogDetalleEtiqueta(connection, tran, bulkContext.UpdateDetallesEtiquetaLote);

                    await BulkInsertContenedores(connection, tran, bulkContext.Contenedores);
                    await BulkInsertStock(connection, tran, bulkContext.NewStock);
                    await BulkUpdateStock(connection, tran, bulkContext.UpdateStock);
                    await BulkUpdateDetalle(connection, tran, bulkContext.UpdateDetalleAgenda);
                    await BulkRemovePropblemaDetalleAgenda(connection, tran, bulkContext.UpdateDetalleAgenda);
                    await BulkInsertPropblemaDeMenosDetalleAgenda(connection, tran, bulkContext.UpdateDetalleAgenda, userId);
                    await BulkUpdateSituacionDetalle(connection, tran, bulkContext.UpdateDetalleAgenda);
                    await BulkUpdateAgenda(connection, tran, bulkContext.AgendasUpdate);
                    tran.Commit();
                }
            }

        }

        public virtual async Task BulkInsertPropblemaDeMenosDetalleAgenda(DbConnection connection, DbTransaction tran, List<AgendaDetalle> agendasUpdate, int userId)
        {
            List<object> detalles = new List<object>();
            foreach (var det in agendasUpdate)
            {
                detalles.Add(new
                {
                    IdAgenda = det.IdAgenda,
                    IdEmpresa = det.IdEmpresa,
                    CodigoProducto = det.CodigoProducto,
                    Identificador = det.Identificador,
                    faixa = det.Faixa,
                    FechaAlta = DateTime.Now,
                    userId = userId
                });
            }
            string sql = @"INSERT  INTO T_RECEPCION_AGENDA_PROBLEMA (NU_AGENDA,
                                                                    CD_PRODUTO,
                                                                    NU_IDENTIFICADOR,
                                                                    ND_TIPO,
                                                                    ND_PROBLEMA,
                                                                    FL_ACEPTADO,
                                                                    CD_FUNCIONARIO,
                                                                    DT_ADDROR,
                                                                    CD_FAIXA,
                                                                    VL_DIFERENCIA) 
                                                    SELECT DET.NU_AGENDA,
                                                            DET.CD_PRODUTO,
                                                            DET.NU_IDENTIFICADOR,
                                                            'PRO',        
                                                            CASE WHEN((DET.QT_AGENDADO - DET.QT_RECIBIDA) > 0) 
                                                                    THEN 'RMA'
                                                                    ELSE 'REA' END,
                                                            'N',
                                                            :userId,
                                                            :FechaAlta,
                                                            CD_FAIXA,
                                                            (DET.QT_AGENDADO - DET.QT_RECIBIDA)
                                                        FROM (SELECT NU_AGENDA,
                                                                            CD_PRODUTO,
                                                                            NU_IDENTIFICADOR,
                                                                            CD_FAIXA,
                                                                            CD_EMPRESA,
                                                                            SUM(COALESCE(QT_RECIBIDA,
                                                                            0)) QT_RECIBIDA ,
                                                                            SUM(COALESCE(QT_AGENDADO,
                                                                            0)) QT_AGENDADO
                                                                FROM T_DET_AGENDA
                                                                GROUP BY  NU_AGENDA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA,CD_EMPRESA) DET
                                                                WHERE DET.QT_AGENDADO - DET.QT_RECIBIDA != 0
                                                               AND  DET.NU_AGENDA = :IdAgenda 
                                                               AND DET.CD_PRODUTO = :CodigoProducto
                                                               AND DET.CD_FAIXA = :Faixa
                                                               AND DET.CD_EMPRESA = :IdEmpresa
                                                               AND DET.NU_IDENTIFICADOR = :Identificador
                                                                  ";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual async Task BulkInsertPropblemaDeMasDetalleAgenda(DbConnection connection, DbTransaction tran, List<AgendaDetalle> agendasUpdate)
        {
            string sql = @"INSERT T_RECEPCION_AGENDA_PROBLEMA INTO (NU_AGENDA,CD_PRODUTO,NU_IDENTIFICADOR,ND_TIPO,ND_PROBLEMA
                                ,FL_ACEPTADO,CD_FUNCIONARIO,DT_ADDROR,CD_FAIXA,VL_DIFERENCIA) 
                                    SELECT NU_AGENDA,CD_PRODUTO,NU_IDENTIFICADOR,:FechaAlta, SUM(QT_RECIBIDA) QT_RECIBIDA ,SUM(QT_AGENDADO) QT_AGENDADO FROM T_DET_AGENDA
                                                    VL_DIFERENCIA = VL_DIFERENCIA - :CantidadRecibida,
                                                    DT_UPDROW = :FechaModificacion
                                                                  WHERE NU_AGENDA = :Numero 
                                                                  AND CD_PRODUTO = :Producto
                                                                  AND CD_FAIXA = :Faixa
                                                                  AND CD_EMPRESA = :Empresa
                                                                  AND NU_IDENTIFICADOR = :Identificador
                                                                  ";

            await _dapper.ExecuteAsync(connection, sql, agendasUpdate, transaction: tran);
        }

        public virtual async Task BulkRemovePropblemaDetalleAgenda(DbConnection connection, DbTransaction tran, List<AgendaDetalle> agendasUpdate)
        {
            string sql = @"DELETE T_RECEPCION_AGENDA_PROBLEMA  WHERE NU_AGENDA = :IdAgenda 
                                                                  AND CD_PRODUTO = :CodigoProducto
                                                                  AND CD_FAIXA = :Faixa
                                                                  AND NU_IDENTIFICADOR = :Identificador
                                                                  AND FL_ACEPTADO = 'N'
                                                                  ";

            await _dapper.ExecuteAsync(connection, sql, agendasUpdate, transaction: tran);
        }

        public virtual async Task BulkUpdateDetalle(DbConnection connection, DbTransaction tran, List<AgendaDetalle> updateDetalleAutoAgenda)
        {
            string sql = @"UPDATE T_DET_AGENDA SET NU_TRANSACCION = :NumeroTransaccion, 
                                                   QT_RECIBIDA = QT_RECIBIDA + :CantidadRecibida,
                                                    DT_UPDROW = :FechaModificacion
                                                                  WHERE NU_AGENDA = :IdAgenda 
                                                                  AND CD_PRODUTO = :CodigoProducto
                                                                  AND CD_FAIXA = :Faixa
                                                                  AND CD_EMPRESA = :IdEmpresa
                                                                  AND NU_IDENTIFICADOR = :Identificador
                                                                  ";

            await _dapper.ExecuteAsync(connection, sql, updateDetalleAutoAgenda, transaction: tran);
        }

        public virtual async Task BulkUpdateSituacionDetalle(DbConnection connection, DbTransaction tran, List<AgendaDetalle> updateDetalleAutoAgenda)
        {
            var alias = "a";
            var from = @"
                T_DET_AGENDA a
                LEFT JOIN T_RECEPCION_AGENDA_PROBLEMA cc ON 
                     cc.NU_AGENDA = a.NU_AGENDA
                 AND cc.CD_PRODUTO = a.CD_PRODUTO
                 AND cc.CD_FAIXA = a.CD_FAIXA
                 AND cc.NU_IDENTIFICADOR = a.NU_IDENTIFICADOR
                 AND cc.FL_ACEPTADO = 'N'";
            var set = @"
                NU_TRANSACCION = :NumeroTransaccion, 
                CD_SITUACAO = CASE WHEN cc.CD_PRODUTO is null THEN 9 ELSE 10 END,
                DT_UPDROW = :FechaModificacion";
            var where = @"NU_AGENDA = :IdAgenda
                AND CD_PRODUTO = :CodigoProducto
                AND CD_FAIXA = :Faixa
                AND CD_EMPRESA = :IdEmpresa";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, updateDetalleAutoAgenda, commandType: CommandType.Text, transaction: tran);
        }

        public virtual async Task BulkUpdateAgenda(DbConnection connection, DbTransaction tran, List<Agenda> agendasUpdate)
        {

            var alias = "a";
            var from = @"
                T_AGENDA a
                INNER JOIN (select max(CD_SITUACAO) CD_SITUACAO ,NU_AGENDA FROM T_DET_AGENDA group by NU_AGENDA) cc ON 
                cc.NU_AGENDA = a.NU_AGENDA";
            var set = @"
                CD_SITUACAO = cc.CD_SITUACAO,
                NU_TRANSACCION = :NumeroTransaccion,
                DT_UPDROW = :FechaModificacion,
                CD_PORTA = :CodigoPuerta";
            var where = "NU_AGENDA = :Id";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, agendasUpdate, commandType: CommandType.Text, transaction: tran);

        }

        public virtual async Task BulkUpdateDetalleEtiqueta(DbConnection connection, DbTransaction tran, List<object> updateStock)
        {
            string sql = @"UPDATE T_DET_ETIQUETA_LOTE SET NU_TRANSACCION = :NuTransaccion,
                                                            QT_PRODUTO_RECIBIDO = QT_PRODUTO_RECIBIDO + :Cantidad,
                                                            QT_PRODUTO = QT_PRODUTO + :Cantidad,
                                                            DT_UPDROW = :FechaRegistro
                                                                  WHERE NU_ETIQUETA_LOTE = :Numero 
                                                                  AND CD_PRODUTO = :Producto
                                                                  AND CD_FAIXA = :Faixa
                                                                  AND CD_EMPRESA = :Empresa
                                                                  AND NU_IDENTIFICADOR = :Identificador
                                                                  ";

            await _dapper.ExecuteAsync(connection, sql, updateStock, transaction: tran);

        }

        public virtual async Task BulkInsertDetalleEtiqueta(DbConnection connection, DbTransaction tran, List<object> insertDetallesEtiquetaLote)
        {
            string sql = @" INSERT INTO T_DET_ETIQUETA_LOTE (NU_ETIQUETA_LOTE,CD_PRODUTO,CD_FAIXA,CD_EMPRESA,NU_IDENTIFICADOR,
                                                            QT_PRODUTO_RECIBIDO,QT_PRODUTO,DT_ADDROW,DT_ENTRADA,NU_TRANSACCION) values 
                                                            ( :Numero,:Producto,:Faixa,:Empresa,:Identificador,
                                                            :Cantidad,:Cantidad,:FechaRegistro,:FechaRegistro,:NumeroTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, insertDetallesEtiquetaLote, transaction: tran);
        }

        public virtual async Task BulkInsertLogDetalleEtiqueta(DbConnection connection, DbTransaction tran, List<object> insertDetallesEtiquetaLote)
        {
            string sql = @" INSERT INTO T_LOG_ETIQUETA (NU_AGENDA,CD_APLICACAO,NU_ETIQUETA,CD_PRODUTO,CD_FAIXA,CD_EMPRESA,
                                                        NU_IDENTIFICADOR,QT_MOVIMIENTO,CD_ENDERECO,DT_OPERACION,
                                                        CD_FUNCIONARIO,NU_TRANSACCION) values 
                                                        ( :idAgenda,:Aplicacion,:Numero,:Producto,:Faixa,:Empresa,
                                                          :Identificador,:CantidadRecibida,:Ubicacion,:FechaRegistro,
                                                        :IdUsuario,:NumeroTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, insertDetallesEtiquetaLote, transaction: tran);
        }

        public virtual async Task BulkInsertEtiquetaLote(DbConnection connection, DbTransaction tran, List<object> etiquetaLote)
        {
            string sql = @" INSERT INTO T_ETIQUETA_LOTE (NU_ETIQUETA_LOTE,NU_AGENDA,CD_ENDERECO,CD_SITUACAO,DT_RECEPCION,CD_CLIENTE
                                                        ,CD_BARRAS,NU_EXTERNO_ETIQUETA,TP_ETIQUETA,NU_TRANSACCION,CD_FUNC_RECEPCION) values 
                                                    ( :Numero,:NumeroAgenda,:IdUbicacion,:Estado,:FechaRecepcion,:Cliente,
                                                        :CodigoBarras,:NumeroExterno,:TipoEtiqueta,:NumeroTransaccion,:FuncionarioRecepcion)";

            await _dapper.ExecuteAsync(connection, sql, etiquetaLote, transaction: tran);
        }

        public virtual async Task BulkInsertEtiquetaLoteEnUso(DbConnection connection, DbTransaction tran, List<object> etiquetaLote)
        {
            string sql = @" INSERT INTO T_ETIQUETAS_EN_USO (NU_ETIQUETA_LOTE,NU_EXTERNO_ETIQUETA,TP_ETIQUETA) values 
                                                    (:Numero,:NumeroExterno,:TipoEtiqueta)";

            await _dapper.ExecuteAsync(connection, sql, etiquetaLote, transaction: tran);
        }

        public virtual async Task BulkInsertContenedores(DbConnection connection, DbTransaction tran, List<object> contenedor)
        {
            string sql = @"INSERT INTO T_CONTENEDOR 
                               (NU_PREPARACION,
                                NU_CONTENEDOR,
                                TP_CONTENEDOR,
                                CD_SITUACAO,
                                CD_ENDERECO,
                                DT_ADDROW,
                                FL_HABILITADO,
                                NU_TRANSACCION,
                                ID_EXTERNO_CONTENEDOR,
                                CD_BARRAS) 
                            VALUES 
                                (:Preparacion,
                                :Contenedor,
                                :TipoEtiquetaDestino,
                                :SituacionDestino,
                                :Ubicacion,
                                :Fecha,
                                'S',
                                :NuTransaccion,
                                :IdExternoContenedor,
                                :CodigoBarras)";

            await _dapper.ExecuteAsync(connection, sql, contenedor, transaction: tran);
        }

        public virtual async Task BulkInsertStock(DbConnection connection, DbTransaction tran, List<object> updateStock)
        {
            string sql = @" INSERT INTO T_STOCK (CD_ENDERECO,CD_EMPRESA,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,QT_ESTOQUE,QT_RESERVA_SAIDA,
                                                    QT_TRANSITO_ENTRADA,ID_AVERIA,ID_INVENTARIO,DT_INVENTARIO,ID_CTRL_CALIDAD,NU_TRANSACCION) values 
                                                    ( :Ubicacion,:Empresa,:Producto,:Faixa,:Identificador,:Cantidad,:Cantidad
                                                    ,0,'N','R',:Fecha,'C',:NuTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, updateStock, transaction: tran);
        }

        public virtual async Task BulkUpdateStock(DbConnection connection, DbTransaction tran, List<object> updateStock)
        {
            string sql = @" UPDATE T_STOCK 
                            SET NU_TRANSACCION = :NuTransaccion, 
                                QT_ESTOQUE = QT_ESTOQUE + :Cantidad, 
                                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + :Cantidad
                            WHERE CD_ENDERECO = :Ubicacion
                                AND CD_PRODUTO = :Producto
                                AND CD_FAIXA = :Faixa
                                AND NU_IDENTIFICADOR = :Identificador
                                AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, updateStock, transaction: tran);

        }

        public virtual CrossDockingBulkOperationContext GetBulkOperationContext(List<CrossDockingUnaFase> detalles, ICrossDockingServiceContext serviceContext, DbConnection connection, long nuTransaccion, int userId, IBarcodeService barcodeService)
        {
            var context = new CrossDockingBulkOperationContext();
            foreach (var detalle in detalles)
            {
                var agenda = serviceContext.GetAgenda(detalle.Agenda, detalle.Empresa);
                if (agenda.CodigoPuerta == null && !context.AgendasUpdate.Any(x => x.Id == agenda.Id))
                {
                    agenda.CodigoPuerta = serviceContext.GetPuerta(detalle.Ubicacion).Id;
                }

                if (!context.AgendasUpdate.Any(x => x.Id == agenda.Id))
                {
                    agenda.NumeroTransaccion = nuTransaccion;
                    agenda.FechaModificacion = DateTime.Now;
                    context.AgendasUpdate.Add(agenda);
                }
            }


            var contenedores = detalles.GroupBy(x => new { x.Preparacion, x.IdExternoContenedor, x.TipoContenedor })
                .Select(x => new CrossDockingUnaFase { Preparacion = x.Key.Preparacion, IdExternoContenedor = x.Key.IdExternoContenedor, TipoContenedor = x.Key.TipoContenedor })
                .ToList();

            int cantidadContenedores = 0;
            foreach (var detalle in contenedores)
            {
                var etiquetaUso = serviceContext.GetEtiquetaEnUso(detalle.IdExternoContenedor, detalle.TipoContenedor);
                if (etiquetaUso == null)
                {
                    cantidadContenedores++;
                }
            }

            var secuenciasContenedores = GetNewContenedores(cantidadContenedores, connection);
            foreach (var detalle in contenedores)
            {
                CrossDockingUnaFase detalleCross = detalles
                    .FirstOrDefault(x => x.IdExternoContenedor == detalle.IdExternoContenedor
                        && x.TipoContenedor == detalle.TipoContenedor
                        && x.Preparacion == detalle.Preparacion);

                var etiquetaUso = serviceContext.GetEtiquetaEnUso(detalle.IdExternoContenedor, detalleCross.TipoContenedor);
                if (etiquetaUso == null)
                {

                    var nuCont = secuenciasContenedores.FirstOrDefault();
                    detalle.TipoContenedor = detalleCross.TipoContenedor;
                    detalle.Ubicacion = detalleCross.Ubicacion;
                    detalle.SituacionDestino = detalleCross.SituacionDestino;
                    detalle.Fecha = DateTime.Now;
                    detalle.NuTransaccion = nuTransaccion;
                    detalle.Contenedor = nuCont;
                    detalle.CodigoBarras = barcodeService.GenerateBarcode(detalle.IdExternoContenedor, detalle.TipoContenedor);

                    secuenciasContenedores.Remove(nuCont);
                    context.Contenedores.Add(detalle);
                }
            }

            var detallesAgrupadosStock = detalles.GroupBy(x => new { x.Empresa, x.Producto, x.Identificador, x.Ubicacion, x.Faixa })
               .Select(x => new CrossDockingUnaFase
               {
                   Empresa = x.Key.Empresa,
                   Producto = x.Key.Producto,
                   Identificador = x.Key.Identificador,
                   Ubicacion = x.Key.Ubicacion,
                   Cantidad = x.Sum(d => d.Cantidad),
                   Faixa = x.Key.Faixa
               });

            foreach (var detalle in detallesAgrupadosStock)
            {
                var stock = serviceContext.GetStock(detalle.Ubicacion, detalle.Producto, detalle.Empresa, detalle.Identificador, detalle.Faixa ?? 1);
                if (stock == null)
                {
                    detalle.NuTransaccion = nuTransaccion;
                    detalle.Fecha = DateTime.Now;
                    context.NewStock.Add(detalle);
                }
                else
                {
                    detalle.Fecha = DateTime.Now;
                    detalle.NuTransaccion = nuTransaccion;
                    context.UpdateStock.Add(detalle);
                }
            }

            var detallesAgrupadosEtiquetaLote = detalles.GroupBy(x => new { x.Agenda, x.Cliente, x.Empresa, x.Producto, x.Identificador, x.IdExternoContenedor, x.TipoContenedor, x.Faixa })
             .Select(x => new CrossDockingUnaFase
             {
                 Agenda = x.Key.Agenda,
                 Cliente = x.Key.Cliente,
                 Empresa = x.Key.Empresa,
                 Producto = x.Key.Producto,
                 Identificador = x.Key.Identificador,
                 IdExternoContenedor = x.Key.IdExternoContenedor,
                 TipoContenedor = x.Key.TipoContenedor,
                 Cantidad = x.Sum(d => d.Cantidad),
                 Faixa = x.Key.Faixa
             }).ToList();

            var detallesAgrupados = detalles.GroupBy(x => new { x.Agenda, x.Cliente, x.IdExternoContenedor, x.TipoContenedor, x.Ubicacion })
             .Select(x => new EtiquetaLote
             {
                 NumeroAgenda = x.Key.Agenda,
                 Cliente = x.Key.Cliente,
                 TipoEtiqueta = x.Key.TipoContenedor,
                 NumeroExterno = x.Key.IdExternoContenedor,
                 IdUbicacion = x.Key.Ubicacion
             });

            int cantidadEtiqueta = 0;
            foreach (var detalle in detallesAgrupados)
            {
                var etiquetaUso = serviceContext.GetEtiquetaEnUso(detalle.NumeroExterno, detalle.TipoEtiqueta);
                if (etiquetaUso == null)
                {
                    cantidadEtiqueta = cantidadEtiqueta + 1;
                }
            }

            var etiquetaLoteIds = GetNewEtiquetaLoteIds(cantidadEtiqueta, connection);
            etiquetaLoteIds = etiquetaLoteIds.OrderBy(x => x).ToList();
            foreach (var detalle in detallesAgrupados)
            {
                var etiquetaUso = serviceContext.GetEtiquetaEnUso(detalle.NumeroExterno, detalle.TipoEtiqueta);
                int etiquetaLote;
                if (etiquetaUso == null)
                {
                    etiquetaLote = etiquetaLoteIds.FirstOrDefault();
                    detalle.Numero = etiquetaLote;
                    detalle.NumeroTransaccion = nuTransaccion;
                    detalle.FechaRecepcion = DateTime.Now;
                    detalle.Estado = SituacionDb.PalletConferido;
                    detalle.FuncionarioRecepcion = userId;
                    detalle.CodigoBarras = barcodeService.GenerateBarcode(detalle.NumeroExterno, detalle.TipoEtiqueta);
                    etiquetaLoteIds.Remove(etiquetaLote);
                    context.EtiquetaLote.Add(detalle);
                }
                else
                {
                    etiquetaLote = etiquetaUso.Numero;
                }

                var detallesEtiquetaLote = detallesAgrupadosEtiquetaLote.Where(x => x.Agenda == detalle.NumeroAgenda && x.Cliente == detalle.Cliente
                && x.TipoContenedor == detalle.TipoEtiqueta && x.IdExternoContenedor == detalle.NumeroExterno).ToList();

                foreach (var detalleEtiqueta in detallesEtiquetaLote)
                {
                    var contextEtiquetaLoteDetalle = serviceContext.GetDetallesEtiquetaLote(etiquetaLote, detalleEtiqueta.Producto, detalleEtiqueta.Empresa, detalleEtiqueta.Faixa ?? 1, detalleEtiqueta.Identificador);
                    if (contextEtiquetaLoteDetalle == null)
                    {
                        context.InsertDetallesEtiquetaLote.Add(new
                        {
                            Ubicacion = detalleEtiqueta.Ubicacion,
                            IdAgenda = detalleEtiqueta.Agenda,
                            Numero = etiquetaLote,
                            Producto = detalleEtiqueta.Producto,
                            Faixa = detalleEtiqueta.Faixa,
                            Empresa = detalleEtiqueta.Empresa,
                            Identificador = detalleEtiqueta.Identificador,
                            CantidadRecibida = detalleEtiqueta.Cantidad,
                            Cantidad = detalleEtiqueta.Cantidad,
                            FechaRegistro = DateTime.Now,
                            NumeroTransaccion = nuTransaccion,
                            Aplicacion = "API",
                            IdUsuario = userId
                        });
                    }
                    else
                    {
                        context.UpdateDetallesEtiquetaLote.Add(new
                        {
                            Ubicacion = detalleEtiqueta.Ubicacion,
                            IdAgenda = detalleEtiqueta.Agenda,
                            Numero = etiquetaLote,
                            Producto = detalleEtiqueta.Producto,
                            Faixa = detalleEtiqueta.Faixa,
                            Empresa = detalleEtiqueta.Empresa,
                            Identificador = detalleEtiqueta.Identificador,
                            CantidadRecibida = detalleEtiqueta.Cantidad,
                            Cantidad = detalleEtiqueta.Cantidad,
                            FechaRegistro = DateTime.Now,
                            NumeroTransaccion = nuTransaccion,
                            Aplicacion = "API",
                            IdUsuario = userId
                        });
                    }
                }
            }

            var detallesAgrupadosAgenda = detalles
                .GroupBy(x => new { x.Agenda, x.Empresa, x.Producto, x.Identificador, x.Faixa })
                .Select(x => new CrossDockingUnaFase
                {
                    Agenda = x.Key.Agenda,
                    Empresa = x.Key.Empresa,
                    Producto = x.Key.Producto,
                    Faixa = x.Key.Faixa,
                    Identificador = x.Key.Identificador,
                    Cantidad = x.Sum(d => d.Cantidad)
                }).ToList();

            foreach (var detalleEtiqueta in detallesAgrupadosAgenda)
            {
                AgendaDetalle detalleAgenda = serviceContext.GetDetalle(detalleEtiqueta.Agenda, detalleEtiqueta.Producto, detalleEtiqueta.Empresa, detalleEtiqueta.Faixa ?? 1, detalleEtiqueta.Identificador);
                detalleAgenda.FechaModificacion = DateTime.Now;
                detalleAgenda.NumeroTransaccion = nuTransaccion;
                detalleAgenda.CantidadRecibida = detalleEtiqueta.Cantidad;
                context.UpdateDetalleAgenda.Add(detalleAgenda);
            }

            return context;
        }

        public virtual List<int> GetNewEtiquetaLoteIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_ETIQUETA_LOTE, count).ToList();
        }

        public virtual List<int> GetNewContenedores(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_NU_CONTENEDOR, count).ToList();
        }

        #endregion

        #region Panel CrossDocking

        public virtual void MarcarPedidos(IEnumerable<Pedido> keys)
        {
            var sql = @"UPDATE T_PEDIDO_SAIDA
                        SET NU_PREPARACION_PROGRAMADA = :PreparacionProgramada,
                            NU_ORDEN_LIBERACION = :NumeroOrdenLiberacion,
                            DT_UPDROW = :FechaModificacion,
                            NU_TRANSACCION = :Transaccion                          
                        WHERE NU_PEDIDO = :Id 
                            AND CD_EMPRESA = :Empresa
                            AND CD_CLIENTE = :Cliente ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void DesmarcarPedidos(IEnumerable<Pedido> keys)
        {
            var sql = @"UPDATE T_PEDIDO_SAIDA
                        SET NU_PREPARACION_PROGRAMADA = NULL,
                            NU_ORDEN_LIBERACION = NULL,
                            DT_UPDROW = :FechaModificacion,
                            NU_TRANSACCION = :Transaccion                          
                        WHERE NU_PEDIDO = :Id 
                            AND CD_EMPRESA = :Empresa
                            AND CD_CLIENTE = :Cliente ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void EliminarPreparacionCrossDocking(int nuPreparacion, int nuAgenda)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            var sql = @"SELECT 
                            ps.NU_PEDIDO                            
	                    FROM T_PEDIDO_SAIDA ps		                
	                    WHERE ps.NU_PREPARACION_PROGRAMADA = :preparacionProgramada";

            var pedido = _dapper.Query<string>(connection, sql, param: new { preparacionProgramada = nuPreparacion }, transaction: tran).FirstOrDefault();

            if (string.IsNullOrEmpty(pedido))
            {
                sql = @"UPDATE T_PICKING
                        SET NU_TRANSACCION = :Transaccion,
                            NU_TRANSACCION_DELETE = :Transaccion
                        WHERE NU_PREPARACION = :Preparacion ";

                _dapper.Execute(connection, sql, param: new
                {
                    Preparacion = nuPreparacion,
                    FechaModificacion = DateTime.Now,
                    Transaccion = _context.GetTransactionNumber()
                }, transaction: tran);

                sql = @"DELETE FROM T_PICKING WHERE NU_PREPARACION = :Preparacion ";

                _dapper.Execute(connection, sql, param: new { Preparacion = nuPreparacion }, transaction: tran);

                sql = @"DELETE FROM T_CROSS_DOCK WHERE NU_PREPARACION = :Preparacion AND NU_AGENDA = :Agenda";

                _dapper.Execute(connection, sql, param: new { Preparacion = nuPreparacion, Agenda = nuAgenda }, transaction: tran);
            }
        }

        public virtual bool ValidarPedidos(int preparacion, out DetallePedido detalleInvalido)
        {
            var sql = @"SELECT 
                            dps.NU_PEDIDO as Id,
                            dps.CD_CLIENTE as Cliente,
                            dps.CD_EMPRESA as Empresa,
                            dps.CD_PRODUTO as Producto
	                    FROM T_PEDIDO_SAIDA ps
		                INNER JOIN T_DET_PEDIDO_SAIDA dps ON ps.NU_PEDIDO = dps.NU_PEDIDO and ps.CD_CLIENTE  = dps.CD_CLIENTE and ps.CD_EMPRESA  = dps.CD_EMPRESA
	                    WHERE ps.NU_PREPARACION_PROGRAMADA = :preparacionProgramada
	                    GROUP BY 
                            dps.NU_PEDIDO,
                            dps.CD_CLIENTE,
                            dps.CD_EMPRESA,
                            dps.CD_PRODUTO
	                    HAVING MAX(dps.ID_ESPECIFICA_IDENTIFICADOR) != MIN(dps.ID_ESPECIFICA_IDENTIFICADOR)";

            detalleInvalido = _dapper.Query<DetallePedido>(_context.Database.GetDbConnection(), sql, param: new { preparacionProgramada = preparacion }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();

            return (detalleInvalido == null);
        }

        public virtual void ProcesarPedidosCrossDocking(AtenderPedidoCrossDockingBulkOperationContext context)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkInsert(connection, tran, context.NewCrossDockingTemp, "T_CROSS_DOCK_TEMP", new Dictionary<string, Func<CrossDockingTemp, ColumnInfo>>
            {
                { "NU_AGENDA" , x => new ColumnInfo(x.NU_AGENDA)},
                { "CD_CLIENTE" , x => new ColumnInfo(x.CD_CLIENTE)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.CD_EMPRESA)},
                { "NU_PEDIDO" , x => new ColumnInfo(x.NU_PEDIDO)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.CD_PRODUTO)},
                { "CD_FAIXA" , x => new ColumnInfo(x.CD_FAIXA)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.NU_IDENTIFICADOR)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.ID_ESPECIFICA_IDENTIFICADOR ? "S" : "N")},
                { "QT_PRODUTO" , x => new ColumnInfo(x.QT_PRODUTO)},
            });

            _dapper.BulkInsert(connection, tran, context.NewDetallePedido, "T_DET_PEDIDO_SAIDA", new Dictionary<string, Func<DetallePedido, ColumnInfo>>
            {
                { "NU_PEDIDO" , x => new ColumnInfo(x.Id)},
                { "CD_CLIENTE" , x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA" , x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
                { "ID_AGRUPACION" , x => new ColumnInfo(x.Agrupacion, DbType.String)},
                { "QT_PEDIDO" , x => new ColumnInfo(x.Cantidad)},
                { "QT_LIBERADO" , x => new ColumnInfo(x.CantidadLiberada)},
                { "QT_ANULADO" , x => new ColumnInfo(x.CantidadAnulada)},
                { "DT_ADDROW" , x => new ColumnInfo(x.FechaAlta, DbType.DateTime)},
                { "NU_TRANSACCION" , x => new ColumnInfo(x.Transaccion)},
            });

            _dapper.BulkUpdate(connection, tran, context.UpdateDetallePedido, "T_DET_PEDIDO_SAIDA", new Dictionary<string, Func<DetallePedido, ColumnInfo>>
            {
                { "QT_PEDIDO", x => new ColumnInfo(x.Cantidad)},
                { "QT_LIBERADO", x => new ColumnInfo(x.CantidadLiberada)},
                { "DT_UPDROW", x => new ColumnInfo(x.FechaModificacion, DbType.DateTime)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.Transaccion)},
            }, new Dictionary<string, Func<DetallePedido, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Id)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
            });

            _dapper.BulkUpdate(connection, tran, context.UpdateDetalleAgenda, "T_DET_AGENDA", new Dictionary<string, Func<AgendaDetalle, ColumnInfo>>
            {
                { "QT_CROSS_DOCKING", x => new ColumnInfo(x.CantidadCrossDocking)},
                { "DT_UPDROW", x => new ColumnInfo(x.FechaModificacion, DbType.DateTime)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.NumeroTransaccion)},
            }, new Dictionary<string, Func<AgendaDetalle, ColumnInfo>>
            {
                { "NU_AGENDA", x => new ColumnInfo(x.IdAgenda)},
                { "CD_EMPRESA", x => new ColumnInfo(x.IdEmpresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.CodigoProducto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
            });
        }

        public virtual void ProcesarInicioCrossDockingDF(IniciarCrossDockingBulkOperationContext context)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkInsert(connection, tran, context.NewDetalleCrossDocking, "T_DET_CROSS_DOCK", new Dictionary<string, Func<LineaCrossDocking, ColumnInfo>>
            {
                { "NU_AGENDA" , x => new ColumnInfo(x.Agenda)},
                { "NU_PREPARACION" , x => new ColumnInfo(x.Preparacion)},
                { "NU_PEDIDO" , x => new ColumnInfo(x.Pedido)},
                { "CD_CLIENTE" , x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA" , x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
                { "NU_CARGA" , x => new ColumnInfo(x.Carga)},
                { "QT_PRODUTO" , x => new ColumnInfo(x.Cantidad)},
                { "QT_PREPARADO" , x => new ColumnInfo(x.CantidadPreparada)},
                { "NU_PREPARACION_PICKEO" , x => new ColumnInfo(x.PreparacionPickeada, DbType.Int32)},
                { "DT_ADDROW" , x => new ColumnInfo((x.FechaAlta ?? DateTime.Now))},
                { "NU_TRANSACCION" , x => new ColumnInfo(x.NroTransaccion, DbType.Int64)},
            });

            _dapper.BulkUpdate(connection, tran, context.UpdateDetallePedido, "T_DET_PEDIDO_SAIDA", new Dictionary<string, Func<DetallePedido, ColumnInfo>>
            {
                { "QT_LIBERADO", x => new ColumnInfo(x.CantidadLiberada)},
                { "DT_UPDROW", x => new ColumnInfo(x.FechaModificacion, DbType.DateTime)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.Transaccion)},
            }, new Dictionary<string, Func<DetallePedido, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Id)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
            });

            _dapper.BulkUpdate(connection, tran, context.UpdateDetalleAgenda, "T_DET_AGENDA", new Dictionary<string, Func<AgendaDetalle, ColumnInfo>>
            {
                { "QT_CROSS_DOCKING", x => new ColumnInfo(x.CantidadCrossDocking)},
                { "DT_UPDROW", x => new ColumnInfo(x.FechaModificacion, DbType.DateTime)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.NumeroTransaccion)},
            }, new Dictionary<string, Func<AgendaDetalle, ColumnInfo>>
            {
                { "NU_AGENDA", x => new ColumnInfo(x.IdAgenda)},
                { "CD_EMPRESA", x => new ColumnInfo(x.IdEmpresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.CodigoProducto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
            });

            _dapper.BulkUpdate(connection, tran, context.UpdateDocumentoLineaDesafectada, "T_DET_DOCUMENTO", new Dictionary<string, Func<DocumentoLineaDesafectada, ColumnInfo>>
            {
                { "QT_RESERVADA", x => new ColumnInfo(x.LineaModificada.CantidadReservada)},
                { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
                { "VL_DATO_AUDITORIA", x => new ColumnInfo(_context.GetTransactionNumber(), DbType.Int64)},
            }, new Dictionary<string, Func<DocumentoLineaDesafectada, ColumnInfo>>
            {
                { "NU_DOCUMENTO", x => new ColumnInfo(x.NroDocumento)},
                { "TP_DOCUMENTO", x => new ColumnInfo(x.TipoDocumento)},
                { "CD_EMPRESA", x => new ColumnInfo(x.LineaModificada.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.LineaModificada.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.LineaModificada.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.LineaModificada.Identificador)},
            });

            _dapper.BulkInsert(connection, tran, context.NewDocumentoPreparacionReserva, "T_DOCUMENTO_PREPARACION_RESERV", new Dictionary<string, Func<DocumentoPreparacionReserva, ColumnInfo>>
            {
                { "NU_DOCUMENTO" , x => new ColumnInfo(x.NroDocumento)},
                { "TP_DOCUMENTO" , x => new ColumnInfo(x.TipoDocumento)},
                { "NU_PREPARACION" , x => new ColumnInfo(x.Preparacion)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA" , x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
                { "QT_PRODUTO" , x => new ColumnInfo(x.CantidadProducto, DbType.Decimal)},
                { "QT_PREPARADO" , x => new ColumnInfo(x.CantidadPreparada, DbType.Decimal)},
                { "QT_ANULAR" , x => new ColumnInfo(x.CantidadAnular, DbType.Decimal)},
                { "NU_IDENTIFICADOR_PICKING_DET" , x => new ColumnInfo(x.NroIdentificadorPicking)},
                { "VL_DATO_AUDITORIA" , x => new ColumnInfo(_context.GetTransactionNumber(), DbType.Int64)},
                { "DT_ADDROW" , x => new ColumnInfo(DateTime.Now)},
            });
        }

        public virtual void ProcesarInicioCrossDockingUF(IniciarCrossDockingBulkOperationContext context)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkInsert(connection, tran, context.NewDetalleCrossDocking, "T_DET_CROSS_DOCK", new Dictionary<string, Func<LineaCrossDocking, ColumnInfo>>
            {
                { "NU_AGENDA" , x => new ColumnInfo(x.Agenda)},
                { "NU_PREPARACION" , x => new ColumnInfo(x.Preparacion)},
                { "NU_PEDIDO" , x => new ColumnInfo(x.Pedido)},
                { "CD_CLIENTE" , x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA" , x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
                { "NU_CARGA" , x => new ColumnInfo(x.Carga)},
                { "QT_PRODUTO" , x => new ColumnInfo(x.Cantidad)},
                { "QT_PREPARADO" , x => new ColumnInfo(x.CantidadPreparada)},
                { "NU_PREPARACION_PICKEO" , x => new ColumnInfo(x.PreparacionPickeada, DbType.Int32)},
                { "DT_ADDROW" , x => new ColumnInfo((x.FechaAlta ?? DateTime.Now))},
                { "NU_TRANSACCION" , x => new ColumnInfo(x.NroTransaccion, DbType.Int64)},
            });

            _dapper.BulkDelete(connection, tran, context.RemoveDetalleCrossDockingTemporal, "T_CROSS_DOCK_TEMP", new Dictionary<string, Func<CrossDockingTemp, object>>
            {
                { "NU_AGENDA", x => x.NU_AGENDA},
                { "NU_PEDIDO", x => x.NU_PEDIDO},
                { "CD_CLIENTE", x => x.CD_CLIENTE},
                { "CD_EMPRESA", x => x.CD_EMPRESA},
                { "CD_PRODUTO", x => x.CD_PRODUTO},
                { "CD_FAIXA", x => x.CD_FAIXA},
                { "NU_IDENTIFICADOR", x => x.NU_IDENTIFICADOR},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x=>(x.ID_ESPECIFICA_IDENTIFICADOR ? "S" : "N")},
            });

            if (context.IsUpdateDetallesDocumentoValid())
            {
                _dapper.BulkUpdate(connection, tran, context.UpdateDetallesDocumento.DetallesDocumento, "T_DET_DOCUMENTO", new Dictionary<string, Func<DocumentoLinea, ColumnInfo>>
                {
                    { "QT_RESERVADA", x => new ColumnInfo(x.CantidadReservada)},
                    { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
                    { "VL_DATO_AUDITORIA", x => new ColumnInfo(_context.GetTransactionNumber(), DbType.Int64)},
                }, new Dictionary<string, Func<DocumentoLinea, ColumnInfo>>
                {
                    { "NU_DOCUMENTO", x => new ColumnInfo(context.UpdateDetallesDocumento.NuDocumento)},
                    { "TP_DOCUMENTO", x => new ColumnInfo(context.UpdateDetallesDocumento.TipoDocumento)},
                    { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                    { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                    { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                    { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                });
            }

            _dapper.BulkInsert(connection, tran, context.NewDocumentoPreparacionReserva, "T_DOCUMENTO_PREPARACION_RESERV", new Dictionary<string, Func<DocumentoPreparacionReserva, ColumnInfo>>
            {
                { "NU_DOCUMENTO" , x => new ColumnInfo(x.NroDocumento)},
                { "TP_DOCUMENTO" , x => new ColumnInfo(x.TipoDocumento)},
                { "NU_PREPARACION" , x => new ColumnInfo(x.Preparacion)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA" , x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.Identificador)},
                { "ID_ESPECIFICA_IDENTIFICADOR" , x => new ColumnInfo(x.EspecificaIdentificador ? "S" : "N")},
                { "QT_PRODUTO" , x => new ColumnInfo(x.CantidadProducto, DbType.Decimal)},
                { "QT_PREPARADO" , x => new ColumnInfo(x.CantidadPreparada, DbType.Decimal)},
                { "QT_ANULAR" , x => new ColumnInfo(x.CantidadAnular, DbType.Decimal)},
                { "NU_IDENTIFICADOR_PICKING_DET" , x => new ColumnInfo(x.NroIdentificadorPicking)},
                { "VL_DATO_AUDITORIA" , x => new ColumnInfo(_context.GetTransactionNumber(), DbType.Int64)},
                { "DT_ADDROW" , x => new ColumnInfo(DateTime.Now)},
            });
        }
        #endregion

        #endregion 

    }
}
