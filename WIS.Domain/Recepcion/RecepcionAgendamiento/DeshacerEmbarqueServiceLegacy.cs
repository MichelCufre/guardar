using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class DeshacerEmbarqueServiceLegacy : IDeshacerEmbarqueServiceLegacy
    {
        protected readonly ILogger<DeshacerEmbarqueServiceLegacy> _logger;
        protected readonly IDapper _dapper;

        public DeshacerEmbarqueServiceLegacy(
            ILogger<DeshacerEmbarqueServiceLegacy> logger,
            IDapper dapper)
        {
            this._logger = logger;
            this._dapper = dapper;
        }

        public virtual void DeshacerEmbarque(int nuAgenda, long nuTransaccion)
        {
            using (var connection = _dapper.GetDbConnection())
            {
                connection.Open();

                using (DbTransaction tran = connection.BeginTransaction(this._dapper.GetSnapshotIsolationLevel()))
                {
                    try
                    {
                        var sqlAgenda = @"SELECT 1 FROM T_AGENDA WHERE NU_AGENDA = :NU_AGENDA";
                        var existeAgenda = _dapper.Query<int?>(connection, sqlAgenda, param: new { NU_AGENDA = nuAgenda }, transaction: tran).FirstOrDefault() != null;

                        if (existeAgenda)
                        {
                            short newSit = SituacionDb.AgendaAbierta;

                            EliminarEtiquetas(connection, tran, nuAgenda, nuTransaccion);

                            this.UpdateAgendaSituacion(connection, tran, nuAgenda, newSit, nuTransaccion);

                            this.UpdateDetalleAgendaSituacion(connection, tran, nuAgenda, newSit, nuTransaccion);

                            EliminarProblemasRecepcion(connection, tran, nuAgenda);

                            tran.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public virtual void EliminarEtiquetas(DbConnection connection, DbTransaction tran, int nuAgenda, long nuTransacccion)
        {
            try
            {
                var sqlEtiqueta = @"SELECT * from V_ETIQUETA_LOTE_DET_WREC170 WHERE NU_AGENDA = :NU_AGENDA ORDER BY CD_ENDERECO_SUGERIDO ASC";
                var colEtiquetaAndDet = _dapper.Query<V_ETIQUETA_LOTE_DET_WREC170>(connection, sqlEtiqueta, param: new
                {
                    NU_AGENDA = nuAgenda,
                }, transaction: tran);

                //K_STOCK kStock = new K_STOCK();
                string cdEndereco = string.Empty;
                if (colEtiquetaAndDet != null)
                {
                    colEtiquetaAndDet.ToList().ForEach(e =>
                    {
                        this.DeleteEtiquetaLoteDetalle(connection, tran, e.NU_ETIQUETA_LOTE);
                        this.DeleteEtiquetaLote(connection, tran, e.NU_ETIQUETA_LOTE, nuTransacccion);
                        this.DeleteEtiquetasEnUso(connection, tran, e.NU_ETIQUETA_LOTE);
                    });
                }

            }
            catch (Exception ex)
            {
                this._logger.LogInformation($"DESHACER EMBARQUE: " + ex.ToString());
                throw;
            }
        }

        public virtual void EliminarProblemasRecepcion(DbConnection connection, DbTransaction tran, int nuAgenda)
        {
            try
            {
                var sqlProblemas = @"Delete from T_RECEPCION_AGENDA_PROBLEMA WHERE NU_AGENDA = :NU_AGENDA";
                _dapper.Execute(connection, sqlProblemas, param: new { NU_AGENDA = nuAgenda }, transaction: tran);
            }
            catch (Exception ex)
            {
                this._logger.LogInformation($"DESHACER EMBARQUE: " + ex.ToString());
                throw;
            }
        }

        public virtual void UpdateAgendaSituacion(DbConnection connection, DbTransaction tran, int agenda, short situacion, long nuTransaccion)
        {
            var sqlUpdateAgenda = @"UPDATE T_AGENDA SET CD_SITUACAO = :CD_SITUACAO, NU_TRANSACCION = :NU_TRANSACCION WHERE NU_AGENDA = :NU_AGENDA";
            _dapper.Execute(connection, sqlUpdateAgenda, param: new
            {
                CD_SITUACAO = situacion,
                NU_TRANSACCION = nuTransaccion,
                NU_AGENDA = agenda,
            }, transaction: tran);
        }

        public virtual void UpdateDetalleAgendaSituacion(DbConnection connection, DbTransaction tran, int agenda, short situacion, long nuTransaccion)
        {
            var sqlUpdateDetAgenda = @"UPDATE T_DET_AGENDA SET CD_SITUACAO = :CD_SITUACAO, QT_RECIBIDA = 0, QT_ACEPTADA = 0, NU_TRANSACCION = :NU_TRANSACCION WHERE NU_AGENDA = :NU_AGENDA";
            _dapper.Execute(connection, sqlUpdateDetAgenda, param: new
            {
                CD_SITUACAO = situacion,
                NU_TRANSACCION = nuTransaccion,
                NU_AGENDA = agenda,
            }, transaction: tran);
        }

        public virtual void DeleteEtiquetaLoteDetalle(DbConnection connection, DbTransaction tran, int etiquetaLote)
        {
            var sqlDelete = @"Delete from T_DET_ETIQUETA_LOTE WHERE NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE";
            _dapper.Execute(connection, sqlDelete, param: new
            {
                NU_ETIQUETA_LOTE = etiquetaLote,
            }, transaction: tran);
        }

        public virtual void DeleteEtiquetaLote(DbConnection connection, DbTransaction tran, int etiquetaLote, long nuTransaccion)
        {
            var sqlUpdate = @"UPDATE T_ETIQUETA_LOTE SET 
                    NU_TRANSACCION = :NU_TRANSACCION, 
                    NU_TRANSACCION_DELETE = :NU_TRANSACCION, 
                    DT_UPDROW = :DT_UPDROW 
                WHERE NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE ";

            _dapper.Execute(connection, sqlUpdate, param: new
            {
                NU_ETIQUETA_LOTE = etiquetaLote,
                NU_TRANSACCION = nuTransaccion,
                DT_UPDROW = DateTime.Now,
            }, transaction: tran);

            var sqlDelete = @"DELETE FROM T_ETIQUETA_LOTE 
                WHERE NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE ";

            _dapper.Execute(connection, sqlDelete, param: new
            {
                NU_ETIQUETA_LOTE = etiquetaLote,
            }, transaction: tran);
        }

        public virtual void DeleteEtiquetasEnUso(DbConnection connection, DbTransaction tran, int etiquetaLote)
        {
            var sqlDelete = @"Delete from T_ETIQUETAS_EN_USO WHERE NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE ";

            _dapper.Execute(connection, sqlDelete, param: new
            {
                NU_ETIQUETA_LOTE = etiquetaLote,
            }, transaction: tran);
        }
    }
}
