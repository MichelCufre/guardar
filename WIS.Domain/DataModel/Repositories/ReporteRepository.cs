using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Recepcion;
using WIS.Domain.Reportes;
using WIS.Domain.Reportes.Dtos;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ReporteRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly ReporteMapper _mapper;
        protected readonly IDapper _dapper;

        public ReporteRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new ReporteMapper();
        }

        #region Any

        public virtual bool AnyReporte(string clave, string tabla, string reporte)
        {
            return _context.T_REPORTE_RELACION
                .Any(rr => rr.CD_CLAVE == clave
                    && rr.NM_TABLA == tabla
                    && rr.T_REPORTE.CD_REPORTE == reporte);
        }

        public virtual bool AnyReporteAgenda(int numeroAgenda)
        {
            return _context.T_REPORTE_RELACION
                .AsNoTracking()
                .Any(s => s.NM_TABLA == CReporte.TablaReporteAgenda
                    && s.CD_CLAVE.StartsWith(numeroAgenda.ToString()));
        }

        #endregion

        #region Get

        public virtual List<string> GetReportesIdsAgenda(Agenda agenda)
        {
            return _context.T_RECEPCION_EMP_TIPO_REPORTE
                .AsNoTracking()
                .Where(d => d.TP_RECEPCION_EXTERNO == agenda.TipoRecepcionInterno
                    && d.CD_EMPRESA == agenda.IdEmpresa)
                .Select(s => s.CD_REPORTE)
                .ToList();
        }

        public virtual Reporte GetReporte(long nroReporte)
        {
            var reporte = _context.T_REPORTE
                .Include("T_REPORTE_RELACION")
                .AsNoTracking()
                .FirstOrDefault(r => r.NU_REPORTE == nroReporte);

            return _mapper.MapToObject(reporte);
        }

        public virtual List<Reporte> GetReportePendienteImprimir(List<string> reportesImprimir, List<string> situacionesImprimir)
        {
            return _context.T_REPORTE
                .AsNoTracking()
                .Where(d => situacionesImprimir.Contains(d.ND_SITUACION)
                    && reportesImprimir.Contains(d.CD_REPORTE))
                .Select(r => this._mapper.MapToObject(r))
                .ToList();
        }

        public virtual List<DtoDetallesPackingListSinLpn> GetDetallesPackingListSinLpnReporte(int camion, int empresa, string cliente)
        {
            var listaEmpaque = this._context.V_REPORTE_PACKING_LIST_SIN_LPN
                .Where(x => x.CD_CAMION == camion 
                    && x.CD_EMPRESA == empresa 
                    && x.CD_CLIENTE == cliente)
                .OrderBy(x => x.DS_PRODUTO)
                .ToList();

            var predio = listaEmpaque
                .Select(s => s.NU_PREDIO)
                .FirstOrDefault();

            var agente = this._context.T_CLIENTE
                .FirstOrDefault(i => i.CD_AGENTE == predio && i.TP_AGENTE == TipoAgenteDb.Deposito);

            return _mapper.MapToObjectListaEmpaque(listaEmpaque, agente);
        }

        public virtual List<long> GetReportesPendientes()
        {
            return _context.T_REPORTE
                .AsNoTracking()
                .Where(r => r.ND_SITUACION == CReporte.Pendiente 
                    || r.ND_SITUACION == CReporte.PendienteReprocesar
                    || r.ND_SITUACION == CReporte.PendienteReimprimir)
                .Select(r => r.NU_REPORTE)
                .ToList();
        }

        #endregion

        #region Add

        public virtual void AddReporte(Reporte reporte)
        {
            reporte.Id = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_REPORTE);
            reporte.NombreArchivo = reporte.Id.ToString() + "-" + reporte.Tipo + ".pdf";
            reporte.FechaAlta = DateTime.Now;

            T_REPORTE entity = this._mapper.MapToEntity(reporte);

            T_REPORTE_RELACION relacion = entity.T_REPORTE_RELACION?.FirstOrDefault();

            if (relacion != null)
                relacion.NU_REPORTE_RELACION = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_REPORTE_RELACION);

            this._context.T_REPORTE.Add(entity);
        }

        public virtual DtoReporteConfRecepcion GetAgendaInfoReporte(int nroAgenda)
        {
            var agenda = this._context.V_REPORTE_CONF_RECEPCION.FirstOrDefault(c => c.NU_AGENDA == nroAgenda);
            return this._mapper.MapAgendaToEntity(agenda);
        }

        public virtual List<DtoReporteConfRecepcionDetalle> GetDetallesAgendaReporte(int nroAgenda)
        {
            return _context.V_REPORTE_CONF_RECEPCION_DET
                .Where(x => x.NU_AGENDA == nroAgenda)
                .Select(x => _mapper.MapAgendaDetalleToObject(x))
                .ToList();
        }

        #endregion

        #region Update

        public virtual void UpdateReporte(Reporte reporte)
        {
            var entity = _mapper.MapToEntity(reporte);
            var attachedEntity = _context.T_REPORTE.Local
                .FirstOrDefault(r => r.NU_REPORTE == entity.NU_REPORTE);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_REPORTE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        public virtual List<DtoReporteRelacion> GetReportesPendientes(DbConnection connection, string tabla)
        {
            string sql = $@"SELECT 
                                re.NU_REPORTE_RELACION as IdReporteRelacion,
                                re.NU_REPORTE as Id,
                                re.CD_CLAVE as Clave,
                                re.NM_TABLA as Tabla,
                                r.CD_REPORTE as Tipo,
                                r.CD_USUARIO as Usuario,
                                r.DT_ADDROW as FechaAlta,
                                r.NM_ARCHIVO as NombreArchivo,
                                r.ND_SITUACION as Estado,
                                r.DT_UPDROW as FechaModificacion,
                                r.NU_PREDIO as Predio
                            FROM T_REPORTE_RELACION re 
                            INNER JOIN T_REPORTE r ON re.NU_REPORTE = r.NU_REPORTE  AND 
                                r.ND_SITUACION IN ('{CReporte.Pendiente}','{CReporte.PendienteReprocesar}') 
                            WHERE re.NM_TABLA = :tabla";

            return _dapper.Query<DtoReporteRelacion>(connection, sql, param: new
            {
                tabla = tabla
            }, commandType: CommandType.Text).ToList();
        }

        public virtual DtoReporteContCamion GetInfoContenedoresCamion(DbConnection connection, int cdCamion)
        {
            string sql = $@"SELECT 
                                    CD_CAMION as Camion,
                                    CD_EMPRESA as Empresa,
                                    CD_PLACA_CARRO as Matricula,
                                    CD_ROTA as Ruta,
                                    CD_TRANSPORTADORA as Transportadora,
                                    DS_CAMION as DescripcionCamion,
                                    CD_VEICULO as Vehiculo,
                                    DT_CIERRE as FechaCierre,
                                    DS_TRANSPORTADORA as DescripcionTransportadora,
                                    DS_ROTA as DescripcionRuta,
                                    NM_EMPRESA as NombreEmpresa
                        FROM V_REPORTE_CONT_CAMION  
                        WHERE CD_CAMION = :cdCamion";

            return _dapper.Query<DtoReporteContCamion>(connection, sql, param: new
            {
                cdCamion = cdCamion
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual List<DtoReporteContCamionDet> GetInfoContenedoresCamionDet(DbConnection connection, int cdCamion, int cdEmpresa, string cdCliente)
        {
            string sql = $@"SELECT 
                                CD_CAMION as Camion,
                                CD_CLIENTE as Cliente,
                                CD_EMPRESA as Empresa,
                                TP_CONTENEDOR as TipoContenedor,
                                DS_TIPO_CONTENEDOR as DescripcionTipoContenedor,
                                CD_AGENTE as Agente,
                                DS_CLIENTE as DescripcionCliente,
                                TP_AGENTE as TipoCliente,
                                QT_TIPO_CONTENEDOR as CantidadTipoContenedor
                        FROM V_REPORTE_CONT_CAMION_DET  
                        WHERE CD_CAMION = :cdCamion
                        and CD_EMPRESA = :cdEmpresa and CD_CLIENTE = :cdCliente";

            return _dapper.Query<DtoReporteContCamionDet>(connection, sql, param: new
            {
                cdCamion = cdCamion,
                cdEmpresa = cdEmpresa,
                cdCliente = cdCliente
            }, commandType: CommandType.Text).ToList();
        }

        public virtual List<DtpReporteNotaDevolucionDet> GetInfoFacturasDevolucion(DbConnection connection, int nroAgenda)
        {
            string sql = $@"SELECT 
                                rf.NU_RECEPCION_FACTURA as RecepcionFactura,
                                rf.NU_SERIE  as Serie,
                                rf.NU_FACTURA as Factura,
                                rf.TP_FACTURA as TipoFactura,
                                rf.CD_EMPRESA as Empresa,
                                rf.NU_AGENDA as Agenda,
                                rf.DT_EMISION as FechaEmision,
                                rf.IM_TOTAL_DIGITADO as ImporteTotalDigitado,
                                rf.CD_CLIENTE as Cliente,
                                rf.ID_ORIGEN as Origen,
                                rf.DT_ADDROW as FechaCreacion,
                                rf.DT_UPDROW as FechaModificacion,
                                rf.DT_VENCIMIENTO as Vencimiento,
                                rf.CD_MONEDA as Moneda,
                                rf.CD_SITUACAO as Situacion,
                                rf.NU_PREDIO as Predio,
                                rfd.NU_RECEPCION_FACTURA_DET as RecepcionFacturaDet,
                                rfd.CD_PRODUTO as Producto,
                                rfd.CD_FAIXA as Faixa,
                                rfd.NU_IDENTIFICADOR as Lote,
                                rfd.QT_FACTURADA as CantidadFacturada,
                                rfd.QT_VALIDADA as CantidadValidada,
                                rfd.QT_RECIBIDA as CantidadRecibida,
                                rfd.IM_UNITARIO_DIGITADO as ImporteUnitarioDigitado
                        FROM T_RECEPCION_FACTURA  rf
                        inner join T_RECEPCION_FACTURA_DET rfd on rfd.NU_RECEPCION_FACTURA = rf.NU_RECEPCION_FACTURA
                        WHERE NU_AGENDA = :nroAgenda
                       ";

            return _dapper.Query<DtpReporteNotaDevolucionDet>(connection, sql, param: new
            {
                nroAgenda = nroAgenda
            }, commandType: CommandType.Text).ToList();
        }

        public virtual List<DtpReportePackingListDet> GetInfoCamionPackingListDet(DbConnection connection, int cdCamion, int cdEmpresa, string cdCliente)
        {
            string sql = $@"SELECT 
                                CD_CAMION as Camion,
                                CD_CLIENTE as Cliente,
                                CD_PRODUTO as Producto,
                                CD_FAIXA as Faixa,
                                NU_IDENTIFICADOR as Lote,
                                CD_EMPRESA as Empresa,
                                QT_PRODUTO as CantidadProducto,
                                DT_FABRICACAO_PICKEO as Vencimiento,
                                DS_PRODUTO as DescripcionProducto,
                                DS_CLIENTE as DescripcionCliente,
                                CD_AGENTE as Agente,
                                TP_AGENTE as TipoCliente,
                                NM_EMPRESA as NombreEmpresa
                        FROM V_REPORTE_PACKING_LIST_DET  
                        WHERE CD_CAMION = :cdCamion
                        and CD_EMPRESA = :cdEmpresa and CD_CLIENTE = :cdCliente";

            return _dapper.Query<DtpReportePackingListDet>(connection, sql, param: new
            {
                cdCamion = cdCamion,
                cdEmpresa = cdEmpresa,
                cdCliente = cdCliente
            }, commandType: CommandType.Text).ToList();
        }

        public virtual List<DtpReportePackingListDetLpn> GetInfoCamionPackingListDetLpn(DbConnection connection, int cdCamion, int cdEmpresa)
        {
            string sql = $@"SELECT 
                                CD_CAMION as Camion,
	                            ID_LPN_DET as IdLpnDet,
	                            CD_PRODUTO as Producto,
	                            CD_EMPRESA as Empresa,
	                            CD_FAIXA as Faixa,
	                            NU_IDENTIFICADOR as Lote,
	                            QT_EXPEDIDA as CantidadExpedida,
	                            NM_ATRIBUTO as NombreAtributo,
	                            VL_LPN_DET_ATRIBUTO as ValorAtributo
                        FROM V_REPORTE_PACKING_LIST_DET_LPN 
                        WHERE CD_CAMION = :cdCamion
                        AND CD_EMPRESA = :cdEmpresa";

            return _dapper.Query<DtpReportePackingListDetLpn>(connection, sql, param: new
            {
                cdCamion = cdCamion,
                cdEmpresa = cdEmpresa
            }, commandType: CommandType.Text).ToList();
        }

        public virtual DtpReportePackingList GetInfoCamionPackingList(DbConnection connection, int cdCamion)
        {
            string sql = $@"SELECT 
                                CD_CAMION as Camion,
                                CD_EMPRESA as Empresa,
                                CD_PLACA_CARRO as Matricula,
                                CD_ROTA  as Rota,
                                CD_TRANSPORTADORA as Transportadora,
                                DS_CAMION as DescripcionCamion,
                                CD_VEICULO as Vehiculo,
                                DT_CIERRE as FechaCierre,
                                DS_TRANSPORTADORA as DescripcionTransportadora,
                                DS_ROTA as DescripcionRuta,
                                NM_EMPRESA as NombreEmpresa
                        FROM V_REPORTE_PACKING_LIST  
                        WHERE CD_CAMION = :cdCamion";

            return _dapper.Query<DtpReportePackingList>(connection, sql, param: new
            {
                cdCamion = cdCamion
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual DtoReporteControlCambio GetInfoControlCambio(DbConnection connection, int cdCamion)
        {
            string sql = $@"SELECT 
                                NU_PREPARACION as Preparacion,
                                NU_CONTENEDOR as NroContenedor,
                                CD_CAMION as Camion,
                                QT_BULTO as CantidadBulto,
                                DS_TIPO_CONTENEDOR as DescripcionTipoContenedor,
                                CD_PLACA_CARRO as Matricula,
                                CD_TRANSPORTADORA as Transportadora,
                                DT_CIERRE as FechaCierre,
                                CD_CLIENTE as Cliente,
                                CD_EMPRESA as Empresa,
                                NM_EMPRESA as NombreEmpresa,
                                TP_AGENTE as TipoCliente,
                                CD_AGENTE as Agente,
                                DS_CLIENTE as DescripcionCliente,
                                DS_TRANSPORTADORA as DescripcionTransportadora,
                                ID_EXTERNO_CONTENEDOR as IdExternoContenedor,
                                NU_LPN as NroLpn
                        FROM V_REPORTE_CONTROL_CAMBIO  
                        WHERE CD_CAMION = :cdCamion";

            return _dapper.Query<DtoReporteControlCambio>(connection, sql, param: new
            {
                cdCamion = cdCamion
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual List<DtoReporteControlCambio> GetInfoControlCambioDet(DbConnection connection, int cdCamion, int cdEmpresa, string cdCliente)
        {
            string sql = $@"SELECT 
                                NU_PREPARACION as Preparacion,
                                NU_CONTENEDOR as NroContenedor,
                                CD_CAMION as Camion,
                                QT_BULTO as CantidadBulto,
                                DS_TIPO_CONTENEDOR as DescripcionTipoContenedor,
                                CD_PLACA_CARRO as Matricula,
                                CD_TRANSPORTADORA as Transportadora,
                                DT_CIERRE as FechaCierre,
                                CD_CLIENTE as Cliente,
                                CD_EMPRESA as Empresa,
                                NM_EMPRESA as NombreEmpresa,
                                TP_AGENTE as TipoCliente,
                                CD_AGENTE as Agente,
                                DS_CLIENTE as DescripcionCliente,
                                DS_TRANSPORTADORA as DescripcionTransportadora,
                                ID_EXTERNO_CONTENEDOR as IdExternoContenedor,
                                NU_LPN as NroLpn
                            FROM V_REPORTE_CONTROL_CAMBIO  
                            WHERE CD_CAMION = :cdCamion
                            and CD_EMPRESA = :cdEmpresa and CD_CLIENTE = :cdCliente";

            return _dapper.Query<DtoReporteControlCambio>(connection, sql, param: new
            {
                cdCamion = cdCamion,
                cdEmpresa = cdEmpresa,
                cdCliente = cdCliente
            }, commandType: CommandType.Text).ToList();
        }

        public virtual List<DtoReporteControlCambioLpn> GetInfoControlCambioLpn(DbConnection connection, int cdCamion)
        {
            string sql = $@"SELECT 
		                        CD_CAMION as Camion, 
		                        NU_LPN as NroLpn,
		                        NM_ATRIBUTO as NombreAtributo,
		                        VL_LPN_ATRIBUTO as ValorAtributo
                            FROM V_REPORTE_CONTROL_CAMBIO_LPN  
                            WHERE CD_CAMION = :cdCamion ";

            return _dapper.Query<DtoReporteControlCambioLpn>(connection, sql, param: new
            {
                cdCamion = cdCamion
            }, commandType: CommandType.Text).ToList();
        }

        public virtual void UpdateReporte(DbConnection connection, DtoReporteRelacion relacion)
        {
            string sql = @"UPDATE T_REPORTE 
                    SET NM_ARCHIVO = :NombreArchivo,
                        VL_DATA = :Contenido ,
                        ND_SITUACION = :Estado,
                        DT_UPDROW = :FechaModificacion,
                        CD_ZONA = :Zona 
                    WHERE NU_REPORTE = :Id";

            _dapper.ExecuteAsync(connection, sql, relacion);
        }

        #endregion
    }
}
