using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;

namespace WIS.Domain.Facturacion
{
    public class FacturacionLogic
    {
        protected readonly IDapper _dapper;
        protected readonly IUnitOfWork _uow;
        protected readonly IOptions<PluginSettings> _settings;
        protected readonly ILogger<FacturacionService> _logger;

        public FacturacionLogic(IUnitOfWork uow,
            IDapper dapper,
            ILogger<FacturacionService> logger,
            IOptions<PluginSettings> settings)
        {
            _uow = uow;
            _logger = logger;
            _dapper = dapper;
            _settings = settings;
        }

        public virtual void EjecutarProcesoFacturacion(int? nroFactEjecucion = null)
        {
            try
            {
                long? nuTransaccion = null;

                _logger.LogDebug($"EjecutarProcesoFacturacion");

                using (var connection = this._dapper.GetDbConnection())
                {
                    connection.Open();

                    _logger.LogDebug($"Destrucción de pallets sin productos");

                    if (_uow.FacturacionRepository.AnyDestruccionPalletSinProductos(connection))
                    {
                        nuTransaccion = GetTransactionNumber(_uow, nuTransaccion);
                        _uow.FacturacionRepository.DestruccionPalletSinProductos(connection, nuTransaccion.Value);
                    }

                    var ejecuciones = _uow.FacturacionRepository.GetEjecucionesPendientes(connection, nroFactEjecucion);

                    foreach (var ejecucion in ejecuciones.GroupBy(e => e.NumeroEjecucion).Select(d => d.Key))
                    {
                        var procesos = ejecuciones.Where(e => e.NumeroEjecucion == ejecucion && e.TipoProceso != FacturacionDb.TIPO_DE_CALCULO_MANUAL).ToList();
                        foreach (var proceso in procesos)
                        {
                            var factEmpProceso = _uow.FacturacionRepository.GetFactEmpresaProceso(connection, proceso.CodigoEmpresa, proceso.CodigoProceso);
                            if (factEmpProceso != null)
                            {
                                DateTime? fechaDesde = GetFechaDesde(proceso, factEmpProceso);
                                DateTime fechaHasta = GetFechaHasta(proceso);

                                nuTransaccion = GetTransactionNumber(_uow, nuTransaccion);
                                factEmpProceso.NumeroTransaccion = nuTransaccion;

                                using (var tran = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        ProcesarEjecuciones(connection, ejecucion, nuTransaccion.Value, tran, proceso, fechaDesde, fechaHasta);
                                        tran.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        tran.Rollback();
                                        string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                                        _uow.FacturacionRepository.GuardarError(proceso, SituacionDb.CALCULO_CON_ERRORES, msg, connection);
                                    }
                                }

                                UpdateFactEmpresaProceso(connection, proceso, factEmpProceso, fechaHasta);
                                UpdateFactEjecEmpresa(connection, proceso, fechaDesde, fechaHasta);
                            }
                            else
                            {
                                string msg = $"No existe registro de facturación empresa proceso. Empresa: {proceso.CodigoEmpresa} - Proceso: {proceso.CodigoProceso}";
                                _uow.FacturacionRepository.GuardarError(proceso, FacturacionDb.EMP_PROCESO_ERROR, msg, connection);
                            }
                        }

                        _uow.FacturacionRepository.UpdateSituacionEjecucion(ejecucion, SituacionDb.EJECUCION_REALIZADA, connection);
                        _uow.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public virtual long? GetTransactionNumber(IUnitOfWork uow, long? nuTransaccion)
        {
            if (!nuTransaccion.HasValue)
            {
                _uow.CreateTransactionNumber("EjecutarProcesoFacturacion");
                nuTransaccion = _uow.GetTransactionNumber();
            }

            return nuTransaccion;
        }

        public virtual void ProcesarEjecuciones(DbConnection connection, int ejecucion, long nuTransaccion, DbTransaction tran, FacturacionEjecEmpProceso proceso, DateTime? fechaDesde, DateTime fechaHasta)
        {
            switch (proceso.TipoProceso)
            {
                case FacturacionDb.TIPO_DE_CALCULO_PROGRAMABLE:
                    ProcesarProgramables(connection, ejecucion, nuTransaccion, tran, proceso);
                    break;
                case FacturacionDb.TIPO_DE_CALCULO_CALCULADO:
                    ProcesarCalculados(connection, tran, proceso, nuTransaccion, ref fechaDesde, ref fechaHasta);
                    break;
            }
        }

        #region Procesamientos
        public virtual void ProcesarProgramables(DbConnection connection, int ejecucion, long nuTransaccion, DbTransaction tran, FacturacionEjecEmpProceso proceso)
        {
            _uow.FacturacionRepository.ProcesarProgramables(ejecucion, proceso.CodigoEmpresa, proceso.CodigoProceso, nuTransaccion, connection, tran);
        }

        public virtual void ProcesarCalculados(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, ref DateTime? fechaDesde, ref DateTime fechaHasta)
        {
            var cuentaContable = _uow.FacturacionRepository.GetCuentaContableResultado(connection, tran, proceso.CodigoFacturacion, proceso.NumeroComponente, proceso.NumeroCuentaContable);

            switch (proceso.CodigoFacturacion)
            {
                case FacturacionDb.CD_FACT_WST001:
                    ProcesarWST001(connection, tran, proceso, nuTransaccion, fechaDesde, fechaHasta, cuentaContable);
                    break;
                case FacturacionDb.CD_FACT_WST002:
                    ProcesarWST002(connection, tran, proceso, nuTransaccion, fechaDesde, fechaHasta, cuentaContable);
                    break;
                case FacturacionDb.CD_FACT_WST003:
                    ProcesarWST003(connection, tran, proceso, nuTransaccion, fechaDesde, fechaHasta, cuentaContable);
                    break;
                case FacturacionDb.CD_FACT_WST004:
                    ProcesarWST004(connection, tran, proceso, nuTransaccion, fechaDesde, fechaHasta, cuentaContable);
                    break;
                case FacturacionDb.CD_FACT_WST005:
                    ProcesarWST005(connection, tran, proceso, nuTransaccion, fechaDesde, fechaHasta, cuentaContable);
                    break;
                default:
                    ProcesarDefault(connection, tran, proceso, nuTransaccion, fechaDesde, fechaHasta, cuentaContable);
                    break;
            }
        }

        public virtual void ProcesarWST001(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable)
        {
            var resultado = _uow.FacturacionRepository.ExisteResultadoWST001(connection, tran, proceso.CodigoEmpresa, fechaDesde, fechaHasta);
            if (resultado)
            {
                _uow.FacturacionRepository.ProcesarWST001(proceso, cuentaContable, fechaDesde, fechaHasta, nuTransaccion, connection, tran);
                _uow.FacturacionRepository.LogProcesarWST001(proceso, cuentaContable, fechaDesde, fechaHasta, connection, tran);

                var result = _uow.FacturacionRepository.ValidarProceso(proceso, connection, tran);
                if (!result)
                {
                    string msg = $"El total de las cantidades del resultado y los detalles no coinciden.";
                    throw new Exception(msg);
                }
            }
            else
                _uow.FacturacionRepository.SetearResultadoVacio(proceso, cuentaContable, nuTransaccion, connection, tran);
        }

        public virtual void ProcesarWST002(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable)
        {
            var resultado = _uow.FacturacionRepository.ExisteResultadoWST002(connection, tran, proceso.CodigoEmpresa, fechaDesde, fechaHasta);
            if (resultado)
            {
                _uow.FacturacionRepository.ProcesarWST002(proceso, cuentaContable, fechaDesde, fechaHasta, nuTransaccion, connection, tran);
                _uow.FacturacionRepository.LogProcesarWST002(proceso, cuentaContable, fechaDesde, fechaHasta, connection, tran);

                //No se puede hacer esta comparativa ya que el resultado guarda un promedio, y el detalle es un desglose por dia/producto/cantidad
                /*var result = _uow.FacturacionRepository.ValidarProceso(proceso, connection, tran);
                if (!result)
                {
                    string msg = $"El total de las cantidades del resultado y los detalles no coinciden.";
                    throw new Exception(msg);
                }*/
            }
            else
                _uow.FacturacionRepository.SetearResultadoVacio(proceso, cuentaContable, nuTransaccion, connection, tran);
        }

        public virtual void ProcesarWST003(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable)
        {
            var resultado = _uow.FacturacionRepository.ExisteResultadoWST003(connection, tran, proceso.CodigoEmpresa, fechaDesde, fechaHasta);
            if (resultado)
            {
                _uow.FacturacionRepository.ProcesarWST003(proceso, cuentaContable, fechaDesde, fechaHasta, nuTransaccion, connection, tran);
                _uow.FacturacionRepository.LogProcesarWST003(proceso, cuentaContable, fechaDesde, fechaHasta, connection, tran);

                var result = _uow.FacturacionRepository.ValidarProceso(proceso, connection, tran);
                if (!result)
                {
                    string msg = $"El total de las cantidades del resultado y los detalles no coinciden.";
                    throw new Exception(msg);
                }
            }
            else
                _uow.FacturacionRepository.SetearResultadoVacio(proceso, cuentaContable, nuTransaccion, connection, tran);
        }

        public virtual void ProcesarWST004(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable)
        {
            var resultado = _uow.FacturacionRepository.ExisteResultadoWST004(connection, tran, proceso.CodigoEmpresa, fechaDesde, fechaHasta);
            if (resultado)
            {
                _uow.FacturacionRepository.ProcesarWST004(proceso, cuentaContable, fechaDesde, fechaHasta, nuTransaccion, connection, tran);
                _uow.FacturacionRepository.LogProcesarWST004(proceso, cuentaContable, fechaDesde, fechaHasta, connection, tran);

                var result = _uow.FacturacionRepository.ValidarProceso(proceso, connection, tran);
                if (!result)
                {
                    string msg = $"El total de las cantidades del resultado y los detalles no coinciden.";
                    throw new Exception(msg);
                }
            }
            else
                _uow.FacturacionRepository.SetearResultadoVacio(proceso, cuentaContable, nuTransaccion, connection, tran);
        }

        public virtual void ProcesarWST005(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable)
        {
            var resultado = _uow.FacturacionRepository.ExisteResultadoWST005(connection, tran, proceso.CodigoEmpresa, fechaDesde, fechaHasta);
            if (resultado)
            {
                _uow.FacturacionRepository.ProcesarWST005(proceso, cuentaContable, fechaDesde, fechaHasta, nuTransaccion, connection, tran);
                _uow.FacturacionRepository.LogProcesarWST005(proceso, cuentaContable, fechaDesde, fechaHasta, connection, tran);

                var result = _uow.FacturacionRepository.ValidarProceso(proceso, connection, tran);
                if (!result)
                {
                    string msg = $"El total de las cantidades del resultado y los detalles no coinciden.";
                    throw new Exception(msg);
                }
            }
            else
                _uow.FacturacionRepository.SetearResultadoVacio(proceso, cuentaContable, nuTransaccion, connection, tran);
        }

        public virtual void ProcesarDefault(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, long nuTransaccion, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable)
        {
            _logger.LogDebug("ProcesarDefault");

            var assemblyPaths = Directory.GetFiles(this._settings.Value.DirectoryPath, "*.dll");
            Type type = null;

            foreach (var path in assemblyPaths)
            {
                var assembly = Assembly.LoadFrom(path);
                type = assembly
                    .GetTypes()
                    .FirstOrDefault(p => typeof(ICodigoFacturacionManager).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);

                if (type != null)
                    break;
            }

            if (type != null)
            {
                var manager = (ICodigoFacturacionManager)Activator.CreateInstance(type);
                var result = manager.Calcular(connection, tran, proceso, fechaDesde, fechaHasta, cuentaContable, nuTransaccion);
                _logger.LogDebug($"Ejecucion: {proceso.NumeroEjecucion} - Transaccion: {nuTransaccion} - CodigoFacturacion: {proceso.CodigoFacturacion} - Empresa: {proceso.CodigoEmpresa} - Proceso: {proceso.CodigoProceso} - Resultado: {result?.ToString()}");
            }
        }

        #endregion

        public virtual void UpdateFactEjecEmpresa(DbConnection connection, FacturacionEjecEmpProceso proceso, DateTime? fechaDesde, DateTime fechaHasta)
        {
            proceso.FechaDesdeEjecEmp = fechaDesde;
            proceso.FechaHastaEjecEmp = fechaHasta;
            proceso.Estado = FacturacionDb.ESTADO_HAB;
            _uow.FacturacionRepository.UpdateFactEjecEmpresa(proceso, connection);
        }

        public virtual void UpdateFactEmpresaProceso(DbConnection connection, FacturacionEjecEmpProceso proceso, FacturacionEmpresaProceso factEmpProceso, DateTime fechaHasta)
        {
            factEmpProceso.UltimoProceso = fechaHasta;
            factEmpProceso.TipoUltimoProceso = proceso.EjecucionPorHora;
            _uow.FacturacionRepository.UpdateFactEmpresaProceso(factEmpProceso, connection);
        }

        public virtual DateTime GetFechaHasta(FacturacionEjecEmpProceso proceso)
        {
            DateTime fechaHasta = (DateTime)proceso.FechaHastaEjecucion;

            if (proceso.EjecucionPorHora == "N")
                fechaHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);

            return fechaHasta;
        }

        public virtual DateTime? GetFechaDesde(FacturacionEjecEmpProceso proceso, FacturacionEmpresaProceso factEmpProceso)
        {
            DateTime? fechaDesde = proceso.FechaDesdeEjecucion ?? factEmpProceso.UltimoProceso;

            if (fechaDesde != null)
            {
                if (proceso.EjecucionPorHoraProceso == "N")
                {
                    if (proceso.EjecucionPorHora == "N")
                        fechaDesde = ((DateTime)fechaDesde).Date;
                    else
                        fechaDesde = ((DateTime)fechaDesde).AddMinutes(1);
                }
                else
                    fechaDesde = ((DateTime)fechaDesde).AddMinutes(1);
            }

            return fechaDesde;
        }


        public static void CancelarFacturacion(IUnitOfWork uow, int nroEjecucion, int empresa, string CodigoProceso)
        {
            var proceso = uow.FacturacionRepository.GetFacturacionProceso(CodigoProceso);
            var resultados = uow.FacturacionRepository.GetResultadosEjecucion(nroEjecucion, empresa, proceso.CodigoFacturacion);

            if (resultados.Any(r => !string.IsNullOrEmpty(r.NumeroFactura) && !string.IsNullOrEmpty(r.NumeroTicketInterfazFacturacion)))
                throw new ValidationFailedException("FAC004_Sec0_Error_ResultadosEnviadosAFacturar");

            var ultNroEjec = uow.FacturacionRepository.UltimoNroEjecucionProceso(empresa, proceso.CodigoProceso, FacturacionDb.ESTADO_HAB);
            if (nroEjecucion != ultNroEjec)
                throw new ValidationFailedException("FAC004_Sec0_Error_ExistenEjecucionesProEmpresa");

            uow.CreateTransactionNumber("CancelarFacturacion");

            foreach (var resultado in resultados)
            {
                resultado.CodigoSituacion = SituacionDb.CALCULO_RECHAZADO;
                resultado.NumeroTransaccion = uow.GetTransactionNumber();
                uow.FacturacionRepository.UpdateFacturacionResultado(resultado);
            }

            var ejeEmpresaActual = uow.FacturacionRepository.GetFacturacionEjecucionEmpresa(nroEjecucion, empresa, proceso.CodigoProceso);
            if (ejeEmpresaActual != null)
            {
                ejeEmpresaActual.Estado = FacturacionDb.ESTADO_CAN;
                uow.FacturacionRepository.UpdateFacturacionEjecucionEmpresa(ejeEmpresaActual);
            }

            FacturacionEjecucion ejecucionAnterior = null;
            var ejeEmpresaAnterior = uow.FacturacionRepository.GetFacturacionEjecucionEmpresaAnterior(nroEjecucion, empresa, proceso.CodigoProceso);
            if (ejeEmpresaAnterior != null)
                ejecucionAnterior = uow.FacturacionRepository.GetFacturacionEjecucion(ejeEmpresaAnterior.NumeroEjecucion);

            var empresaProceso = uow.FacturacionRepository.GetFacturacionEmpresaProceso(empresa, proceso.CodigoProceso);
            if (empresaProceso != null)
            {
                empresaProceso.UltimoProceso = ejeEmpresaAnterior?.FechaHasta;
                empresaProceso.TipoUltimoProceso = ejecucionAnterior?.EjecucionPorHora;
                empresaProceso.NumeroTransaccion = uow.GetTransactionNumber();
                uow.FacturacionRepository.UpdateFacturacionEmpresaProceso(empresaProceso);
            }
        }
    }
}
