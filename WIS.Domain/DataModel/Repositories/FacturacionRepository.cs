using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.GridComponent.Excel;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class FacturacionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly FacturacionMapper _mapper;
        protected readonly DominioMapper _mapperDominio;
        protected readonly ListaPrecioMapper _mapperListaPrecio;
        protected readonly CotizacionListasMapper _mapperListaCotizacion;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IDapper _dapper;

        public FacturacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new FacturacionMapper();
            this._mapperDominio = new DominioMapper();
            this._mapperListaPrecio = new ListaPrecioMapper();
            this._mapperListaCotizacion = new CotizacionListasMapper();
            this._dapper = dapper;
        }

        #region Add

        public virtual void AddFacturacion(FacturacionCodigoComponente facturacionCodigoComponente)
        {
            T_FACTURACION_CODIGO_COMPONEN entity = this._mapper.MapToEntity(facturacionCodigoComponente);
            this._context.T_FACTURACION_CODIGO_COMPONEN.Add(entity);
        }

        public virtual void AddFacturacionProceso(FacturacionProceso facturacionProceso)
        {
            T_FACTURACION_PROCESO entity = this._mapper.MapToEntity(facturacionProceso);
            this._context.T_FACTURACION_PROCESO.Add(entity);
        }

        public virtual void AddFacturacionEjecucion(FacturacionEjecucion facturacionEjecucion)
        {
            facturacionEjecucion.NumeroEjecucion = this._context.GetNextSequenceValueInt(_dapper, "S_FACTURACION_EJECUCION");
            T_FACTURACION_EJECUCION entity = this._mapper.MapToEntity(facturacionEjecucion);
            this._context.T_FACTURACION_EJECUCION.Add(entity);
        }

        public virtual void AddFacturacionEjecucionEmpresa(FacturacionEjecucionEmpresa facturacionEjecucionEmpresa)
        {
            T_FACTURACION_EJEC_EMPRESA entity = this._mapper.MapToEntity(facturacionEjecucionEmpresa);
            this._context.T_FACTURACION_EJEC_EMPRESA.Add(entity);
        }

        public virtual void AddFacturacionResultado(FacturacionResultado facturacionResultado)
        {
            T_FACTURACION_RESULTADO entity = this._mapper.MapToEntity(facturacionResultado);
            this._context.T_FACTURACION_RESULTADO.Add(entity);

        }

        public virtual void AddFacturacionEmpresaProceso(FacturacionEmpresaProceso facturacionEmpresaProceso)
        {
            T_FACTURACION_EMPRESA_PROCESO entity = this._mapper.MapToEntity(facturacionEmpresaProceso);
            this._context.T_FACTURACION_EMPRESA_PROCESO.Add(entity);
        }

        public virtual void AddFacturacionUnidadMedida(FacturacionUnidadMedida facturacionUnidadMedida)
        {
            T_FACTURACION_UNIDAD_MEDIDA entity = this._mapper.MapToEntity(facturacionUnidadMedida);
            this._context.T_FACTURACION_UNIDAD_MEDIDA.Add(entity);
        }

        public virtual void AddFacturacionUnidadMedidaEmpresa(FacturacionUnidadMedidaEmpresa facturacionUnidadMedidaEmpresa)
        {
            T_FACTURACION_UND_MEDIDA_EMP entity = this._mapper.MapToEntity(facturacionUnidadMedidaEmpresa);
            this._context.T_FACTURACION_UND_MEDIDA_EMP.Add(entity);
        }

        public virtual void AddCodigoFacturacion(FacturacionCodigo codigoFacturacion)
        {
            T_FACTURACION_CODIGO entity = this._mapper.MapToEntity(codigoFacturacion);
            this._context.T_FACTURACION_CODIGO.Add(entity);
        }

        public virtual void AddCodigoFacturacionComponente(FacturacionCodigoComponente codigoFacturacionComponente)
        {
            T_FACTURACION_CODIGO_COMPONEN entity = this._mapper.MapToEntity(codigoFacturacionComponente);
            this._context.T_FACTURACION_CODIGO_COMPONEN.Add(entity);
        }

        #endregion

        #region Update 

        public virtual void UpdateFacturacionEjecucion(FacturacionEjecucion facturacionEjecucion)
        {
            T_FACTURACION_EJECUCION entity = this._mapper.MapToEntity(facturacionEjecucion);
            T_FACTURACION_EJECUCION attachedEntity = _context.T_FACTURACION_EJECUCION.Local
                .FirstOrDefault(w => w.NU_EJECUCION == entity.NU_EJECUCION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_EJECUCION.Attach(entity);
                _context.Entry<T_FACTURACION_EJECUCION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturacionEjecucionEmpresa(FacturacionEjecucionEmpresa factEjecucionEmpresa)
        {
            T_FACTURACION_EJEC_EMPRESA entity = this._mapper.MapToEntity(factEjecucionEmpresa);
            T_FACTURACION_EJEC_EMPRESA attachedEntity = _context.T_FACTURACION_EJEC_EMPRESA.Local
                .FirstOrDefault(w => w.NU_EJECUCION == entity.NU_EJECUCION
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PROCESO.ToUpper().Trim() == entity.CD_PROCESO.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_EJEC_EMPRESA.Attach(entity);
                _context.Entry<T_FACTURACION_EJEC_EMPRESA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateCodigoFacturacionComponente(FacturacionCodigoComponente codigoFacturacionComponente)
        {
            T_FACTURACION_CODIGO_COMPONEN entity = this._mapper.MapToEntity(codigoFacturacionComponente);
            T_FACTURACION_CODIGO_COMPONEN attachedEntity = _context.T_FACTURACION_CODIGO_COMPONEN.Local.
                FirstOrDefault(c => c.CD_FACTURACION.ToUpper().Trim() == entity.CD_FACTURACION.ToUpper().Trim()
                    && c.NU_COMPONENTE.ToUpper().Trim() == entity.NU_COMPONENTE.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_CODIGO_COMPONEN.Attach(entity);
                _context.Entry<T_FACTURACION_CODIGO_COMPONEN>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturacionProceso(FacturacionProceso facturacionProceso)
        {
            T_FACTURACION_PROCESO entity = this._mapper.MapToEntity(facturacionProceso);
            T_FACTURACION_PROCESO attachedEntity = _context.T_FACTURACION_PROCESO.Local
                .FirstOrDefault(c => c.CD_FACTURACION.ToUpper().Trim() == entity.CD_FACTURACION.ToUpper().Trim()
                    && c.CD_PROCESO.ToUpper().Trim() == entity.CD_PROCESO.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_PROCESO.Attach(entity);
                _context.Entry<T_FACTURACION_PROCESO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateCodigoFacturacion(FacturacionCodigo codigoFacturacion)
        {
            T_FACTURACION_CODIGO entity = this._mapper.MapToEntity(codigoFacturacion);
            T_FACTURACION_CODIGO attachedEntity = _context.T_FACTURACION_CODIGO.Local
                .FirstOrDefault(c => c.CD_FACTURACION.ToUpper().Trim() == entity.CD_FACTURACION.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_CODIGO.Attach(entity);
                _context.Entry<T_FACTURACION_CODIGO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturacionResultado(FacturacionResultado facturacionResultado)
        {
            T_FACTURACION_RESULTADO entity = this._mapper.MapToEntity(facturacionResultado);
            T_FACTURACION_RESULTADO attachedEntity = _context.T_FACTURACION_RESULTADO.Local
                .FirstOrDefault(f => f.CD_EMPRESA == entity.CD_EMPRESA
                    && f.CD_FACTURACION.ToUpper().Trim() == entity.CD_FACTURACION.ToUpper().Trim()
                    && f.NU_EJECUCION == entity.NU_EJECUCION
                    && f.NU_COMPONENTE.ToUpper().Trim() == entity.NU_COMPONENTE.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_RESULTADO.Attach(entity);
                _context.Entry<T_FACTURACION_RESULTADO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturacionPalletDet(FacturacionPalletDet facturacionPalletDet)
        {
            T_FACTURACION_PALLET_DET entity = this._mapper.MapToEntity(facturacionPalletDet);
            T_FACTURACION_PALLET_DET attachedEntity = _context.T_FACTURACION_PALLET_DET.Local
                .FirstOrDefault(f => f.NU_PALLET_DET == entity.NU_PALLET_DET);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_PALLET_DET.Attach(entity);
                _context.Entry<T_FACTURACION_PALLET_DET>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturacionEmpresaProceso(FacturacionEmpresaProceso facturacionEmpresaProceso)
        {
            T_FACTURACION_EMPRESA_PROCESO entity = this._mapper.MapToEntity(facturacionEmpresaProceso);
            T_FACTURACION_EMPRESA_PROCESO attachedEntity = _context.T_FACTURACION_EMPRESA_PROCESO.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PROCESO.ToUpper().Trim() == entity.CD_PROCESO.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_EMPRESA_PROCESO.Attach(entity);
                _context.Entry<T_FACTURACION_EMPRESA_PROCESO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturacionUnidadMedida(FacturacionUnidadMedida facturacionUnidadMedida)
        {
            T_FACTURACION_UNIDAD_MEDIDA entity = this._mapper.MapToEntity(facturacionUnidadMedida);
            T_FACTURACION_UNIDAD_MEDIDA attachedEntity = _context.T_FACTURACION_UNIDAD_MEDIDA.Local
                .FirstOrDefault(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == entity.CD_UNIDADE_MEDIDA.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_UNIDAD_MEDIDA.Attach(entity);
                _context.Entry<T_FACTURACION_UNIDAD_MEDIDA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void HabilitarFacturacion(int nuEjecucion, short?[] arrSit)
        {
            this._context.T_FACTURACION_RESULTADO
                .Where(s => s.NU_EJECUCION == nuEjecucion)
                .Where(x => arrSit.Contains(x.CD_SITUACAO))
                .ToList()
                .ForEach(fres =>
                {
                    fres.CD_SITUACAO = SituacionDb.CALCULO_ENVIADO;
                });

            this._context.T_FACTURACION_PALLET_DET
                .Where(s => s.NU_EJECUCION_FACTURACION == nuEjecucion)
                .Where(s => s.ID_ESTADO == FacturacionDb.ESTADO_HAB)
                .ToList()
                .ForEach(fres =>
                {
                    fres.ID_ESTADO = FacturacionDb.ESTADO_FAC;
                });
        }

        public virtual void UpdateFacturacionEjecucionEmpresa(FacturacionEjecucion ejecucion)
        {
            T_FACTURACION_EJEC_EMPRESA entity = this._context.T_FACTURACION_EJEC_EMPRESA.FirstOrDefault(x => x.NU_EJECUCION == ejecucion.NumeroEjecucion);
            T_FACTURACION_EJEC_EMPRESA attachedEntity = _context.T_FACTURACION_EJEC_EMPRESA.Local
                .FirstOrDefault(e => e.NU_EJECUCION == entity.NU_EJECUCION);

            if (entity == null)
                return;

            entity.DT_DESDE = ejecucion.FechaDesde;
            entity.DT_HASTA = ejecucion.FechaHasta;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_EJEC_EMPRESA.Attach(entity);
                _context.Entry<T_FACTURACION_EJEC_EMPRESA>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteFacturacionEjecucionEmpresa(int nuEjecucion, int cdEmpresa, string cdProceso)
        {
            T_FACTURACION_EJEC_EMPRESA entity = this._context.T_FACTURACION_EJEC_EMPRESA.FirstOrDefault(x => x.NU_EJECUCION == nuEjecucion && x.CD_EMPRESA == cdEmpresa && x.CD_PROCESO == cdProceso);
            T_FACTURACION_EJEC_EMPRESA attachedEntity = _context.T_FACTURACION_EJEC_EMPRESA.Local
                .FirstOrDefault(w => w.NU_EJECUCION == entity.NU_EJECUCION
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PROCESO.ToUpper().Trim() == entity.CD_PROCESO.ToUpper().Trim());

            if (attachedEntity != null)
            {
                _context.T_FACTURACION_EJEC_EMPRESA.Remove(attachedEntity);
            }
            else
            {
                _context.T_FACTURACION_EJEC_EMPRESA.Attach(entity);
                _context.T_FACTURACION_EJEC_EMPRESA.Remove(entity);
            }
        }

        public virtual void DeleteFacturacionResultado(int cdEmpresa, string cdFacturacion, int nuEjecucion, string nuComponente)
        {
            T_FACTURACION_RESULTADO entity = this._context.T_FACTURACION_RESULTADO
                .FirstOrDefault(f => f.CD_EMPRESA == cdEmpresa
                    && f.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()
                    && f.NU_EJECUCION == nuEjecucion
                    && f.NU_COMPONENTE.ToUpper().Trim() == nuComponente.ToUpper().Trim());

            T_FACTURACION_RESULTADO attachedEntity = _context.T_FACTURACION_RESULTADO.Local
                .FirstOrDefault(f => f.CD_EMPRESA == entity.CD_EMPRESA
                    && f.CD_FACTURACION.ToUpper().Trim() == entity.CD_FACTURACION.ToUpper().Trim()
                    && f.NU_EJECUCION == entity.NU_EJECUCION
                    && f.NU_COMPONENTE.ToUpper().Trim() == entity.NU_COMPONENTE.ToUpper().Trim());

            if (attachedEntity != null)
            {
                _context.T_FACTURACION_RESULTADO.Remove(attachedEntity);
            }
            else
            {
                _context.T_FACTURACION_RESULTADO.Attach(entity);
                _context.T_FACTURACION_RESULTADO.Remove(entity);
            }
        }

        public virtual void DeleteFacturacionEmpresaProceso(int cdEmpresa, string cdProceso)
        {
            T_FACTURACION_EMPRESA_PROCESO entity = this._context.T_FACTURACION_EMPRESA_PROCESO
                .FirstOrDefault(x => x.CD_EMPRESA == cdEmpresa
                    && x.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim());
            T_FACTURACION_EMPRESA_PROCESO attachedEntity = _context.T_FACTURACION_EMPRESA_PROCESO.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PROCESO.ToUpper().Trim() == entity.CD_PROCESO.ToUpper().Trim());

            if (attachedEntity != null)
            {
                _context.T_FACTURACION_EMPRESA_PROCESO.Remove(attachedEntity);
            }
            else
            {
                _context.T_FACTURACION_EMPRESA_PROCESO.Attach(entity);
                _context.T_FACTURACION_EMPRESA_PROCESO.Remove(entity);
            }
        }

        public virtual void DeleteFacturacionUnidadMedidaEmpresa(string unidadMedida, int cdEmpresa)
        {
            T_FACTURACION_UND_MEDIDA_EMP entity = this._context.T_FACTURACION_UND_MEDIDA_EMP
                .FirstOrDefault(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == unidadMedida.ToUpper().Trim()
                    && x.CD_EMPRESA == cdEmpresa);
            T_FACTURACION_UND_MEDIDA_EMP attachedEntity = _context.T_FACTURACION_UND_MEDIDA_EMP.Local
                .FirstOrDefault(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == entity.CD_UNIDADE_MEDIDA.ToUpper().Trim()
                && x.CD_EMPRESA == entity.CD_EMPRESA);

            if (attachedEntity != null)
            {
                _context.T_FACTURACION_UND_MEDIDA_EMP.Remove(attachedEntity);
            }
            else
            {
                _context.T_FACTURACION_UND_MEDIDA_EMP.Attach(entity);
                _context.T_FACTURACION_UND_MEDIDA_EMP.Remove(entity);
            }
        }

        #endregion

        #region Any 

        public virtual bool ExisteCodigoPallet(short codigoPallet)
        {
            return this._context.V_REG605_PALLETS
                .AsNoTracking()
                .Any(x => x.CD_PALLET == codigoPallet);
        }

        public virtual bool AnyFacturacion(string componente)
        {
            return this._context.T_FACTURACION_CODIGO_COMPONEN
                .AsNoTracking()
                .Any(x => x.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim());
        }

        public virtual bool AnyFacturacionBycdFacturacion(string cdFacturacion)
        {
            return this._context.T_FACTURACION_CODIGO
                .AsNoTracking()
                .Any(x => x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim());
        }

        public virtual bool AnyFacturacionComponente(string nuComponente)
        {
            return this._context.T_FACTURACION_CODIGO_COMPONEN
                .AsNoTracking()
                .Any(x => x.NU_COMPONENTE.ToUpper().Trim() == nuComponente.ToUpper().Trim());
        }

        public virtual bool AnyFacturacion(string componente, string cdFacturacion)
        {
            return this._context.T_FACTURACION_CODIGO_COMPONEN
                .AsNoTracking()
                .Any(x => x.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim()
                    && x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim());
        }

        public virtual bool AnyProceso(string componente, string cdFacturacion)
        {
            return this._context.V_FACTURACION_PROC_FAC_WFAC251
                .AsNoTracking()
                .Any(x => x.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim()
                    && x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim());
        }

        public virtual bool AnyComponenteByFactura(string componente, string cdFacturacion)
        {
            return this._context.V_FACTURAC_CODIGO_COMP_WFAC250
                .AsNoTracking()
                .Any(x => x.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim()
                    && x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim());
        }

        public virtual bool AnyFacturacionEjecucion(int nuEjecucion)
        {
            return this._context.T_FACTURACION_EJEC_EMPRESA
                .Include("T_FACTURACION_EJECUCION")
                .AsNoTracking()
                .Where(s => s.T_FACTURACION_EJECUCION.NU_EJECUCION == nuEjecucion)
                .Any();
        }

        public virtual bool AnyFacturacionAnulada(int nuEjecucion)
        {
            return this._context.T_FACTURACION_EJECUCION
                .AsNoTracking()
                .Where(e => e.NU_EJECUCION == nuEjecucion
                    && e.CD_SITUACAO == SituacionDb.EJECUCION_ANULADA)
                .Any();
        }

        public virtual bool AnyFacturacionResultado(int cdEmpresa, string cdFacturacion, int nuEjecucion, string nuComponente)
        {
            return this._context.T_FACTURACION_RESULTADO
                .AsNoTracking()
                .Where(f => f.CD_EMPRESA == cdEmpresa
                    && f.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()
                    && f.NU_EJECUCION == nuEjecucion
                    && f.NU_COMPONENTE.ToUpper().Trim() == nuComponente.ToUpper().Trim())
                .Any();
        }

        public virtual bool AnyFacturacionResultadoNoRechazado(int nuEjecucion)
        {
            return this._context.T_FACTURACION_RESULTADO
                .AsNoTracking()
                .Where(f => f.NU_EJECUCION == nuEjecucion
                    && f.CD_SITUACAO != SituacionDb.CALCULO_RECHAZADO)
                .Any();
        }

        public virtual bool AnyResultadoSituacion(int nuEjecucion, short situacion)
        {
            return this._context.T_FACTURACION_RESULTADO
                .AsNoTracking()
                .Any(f => f.NU_EJECUCION == nuEjecucion && f.CD_SITUACAO == situacion);
        }

        public virtual bool AnyResultadoError(int nuEjecucion)
        {
            return this._context.V_FACTURA_ERROR_RESULT_WFAC003
                .AsNoTracking()
                .Any(f => f.NU_EJECUCION == nuEjecucion);
        }

        public virtual bool AnyResultadoSinPrecioUnitario(int nuEjecucion, List<short?> situaciones)
        {
            return _context.T_FACTURACION_RESULTADO
                .AsNoTracking()
                .GroupJoin(
                    _context.V_FACTURACION_PRECIO_EMPRESA,
                    fr => new { fr.CD_EMPRESA, fr.CD_FACTURACION, fr.NU_COMPONENTE },
                    fpe => new { fpe.CD_EMPRESA, fpe.CD_FACTURACION, fpe.NU_COMPONENTE },
                    (fr, fpe) => new { Resultado = fr, Cotizacion = fpe }
                )
                .SelectMany(frfpe => frfpe.Cotizacion.DefaultIfEmpty(), (fr, fpe) => new { Resultado = fr.Resultado, Cotizacion = fpe })
                .Any(d => d.Resultado.NU_EJECUCION == nuEjecucion
                    && situaciones.Contains(d.Resultado.CD_SITUACAO)
                    && (d.Cotizacion == null || !d.Cotizacion.VL_PRECIO_UNITARIO.HasValue));
        }

        public virtual bool AnyFacturacionEjecucionEmpresa(int nuEjecucion, int cdEmpresa, string cdProceso)
        {
            return this._context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .Where(s => s.NU_EJECUCION == nuEjecucion
                    && s.CD_EMPRESA == cdEmpresa
                    && s.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim())
                .Any();
        }

        public virtual bool AnyFacturacionHabilitadaCalculo(int nuEjecucion)
        {
            return this._context.V_FACTURA_HABILIT_CALCULO
                .AsNoTracking()
                .Any(s => s.NU_EJECUCION == nuEjecucion);
        }

        public virtual bool ExisteTipoCalculo(string tpCalculo)
        {
            return _context.T_FACTURACION_CODIGO
                .AsNoTracking()
                .Any(x => x.TP_CALCULO.ToUpper().Trim() == tpCalculo.ToUpper().Trim());
        }

        public virtual bool AnyCuentaContable(string cuentaContable)
        {
            return _context.T_FACTURACION_CUENTA_CONTABLE
                .AsNoTracking()
                .Any(x => x.NU_CUENTA_CONTABLE.ToUpper().Trim() == cuentaContable.ToUpper().Trim());
        }

        public virtual bool AnyCodigoProceso(string codigoProceso)
        {
            return _context.T_FACTURACION_PROCESO
                .AsNoTracking()
                .Any(x => x.CD_PROCESO.ToUpper().Trim() == codigoProceso.ToUpper().Trim());
        }

        public virtual bool AnyFacturacionEmpresaProceso(int cdEmpresa, string cdProceso)
        {
            return this._context.T_FACTURACION_EMPRESA_PROCESO
                .AsNoTracking()
                .Any(x => x.CD_EMPRESA == cdEmpresa
                    && x.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim());
        }

        public virtual bool AnyFacturacionUnidadMedida(string unidadMedida)
        {
            return this._context.T_FACTURACION_UNIDAD_MEDIDA
                .AsNoTracking()
                .Any(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == unidadMedida.ToUpper().Trim());
        }

        public virtual bool AnyFacturacionUnidadMedidaEmpresa(string unidadMedida, int cdEmpresa)
        {
            return this._context.T_FACTURACION_UND_MEDIDA_EMP
                .AsNoTracking()
                .Any(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == unidadMedida.ToUpper().Trim()
                    && x.CD_EMPRESA == cdEmpresa);
        }

        #endregion

        #region Get

        public virtual int GetNextNumeroOrden()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_ORT_ORDEN");
        }

        public virtual List<FacturacionCodigo> GetFacturacionesCodigoByEjecucionEmpresa(int nuEjecucion, int empresa)
        {
            var resultado = this._context.T_FACTURACION_EJEC_EMPRESA
                .Include("T_FACTURACION_PROCESO")
                .Include("T_FACTURACION_PROCESO.T_FACTURACION_CODIGO_COMPONEN")
                .Include("T_FACTURACION_PROCESO.T_FACTURACION_CODIGO_COMPONEN.T_FACTURACION_CODIGO")
                .Where(x => x.NU_EJECUCION == nuEjecucion
                    && x.CD_EMPRESA == empresa
                    && x.T_FACTURACION_PROCESO.TP_PROCESO == "M"
                    && x.T_FACTURACION_PROCESO.T_FACTURACION_CODIGO_COMPONEN.T_FACTURACION_CODIGO.TP_CALCULO == "M")
                .GroupBy(x => new
                {
                    x.T_FACTURACION_PROCESO.T_FACTURACION_CODIGO_COMPONEN.T_FACTURACION_CODIGO.CD_FACTURACION,
                    x.T_FACTURACION_PROCESO.T_FACTURACION_CODIGO_COMPONEN.T_FACTURACION_CODIGO.DS_FACTURACION
                })
                .Select(x => new FacturacionCodigo()
                {
                    CodigoFacturacion = x.Key.CD_FACTURACION,
                    DescripcionFacturacion = x.Key.DS_FACTURACION
                })
                .AsNoTracking()
                .ToList();

            return resultado;
        }

        public virtual List<FacturacionCodigoComponente> GetFacturacionesCodigoComponente()
        {
            var facturacionCodigoComponente = new List<FacturacionCodigoComponente>();
            var entities = this._context.T_FACTURACION_CODIGO_COMPONEN
                .Include("T_FACTURACION_CODIGO")
                .AsNoTracking()
                .Where(x => x.T_FACTURACION_CODIGO.TP_CALCULO == "M");

            foreach (var entity in entities)
            {
                facturacionCodigoComponente.Add(this._mapper.MapToObject(entity));
            }

            return facturacionCodigoComponente;
        }

        public virtual FacturacionCodigoComponente GetCodigoFacturacionComponente(string facturacion, string componente)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_CODIGO_COMPONEN
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_FACTURACION.ToUpper().Trim() == facturacion.ToUpper().Trim()
                    && x.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim()));
        }

        public virtual List<FacturacionUnidadMedidaEmpresa> GetUnidadMedidaByEmpresa(int codigoEmpresa)
        {
            var unidadesDeMedida = new List<FacturacionUnidadMedidaEmpresa>();
            var entities = this._context.T_FACTURACION_UND_MEDIDA_EMP
                .AsNoTracking()
                .Where(x => x.CD_EMPRESA == codigoEmpresa);

            foreach (var entity in entities)
            {
                unidadesDeMedida.Add(this._mapper.MapToObject(entity));
            }

            return unidadesDeMedida;
        }

        public virtual FacturacionUnidadMedidaEmpresa GetUnidadMedida(string unidadDeMedida)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_UND_MEDIDA_EMP
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == unidadDeMedida.ToUpper().Trim()));
        }

        public virtual List<FacturacionCodigoComponente> GetFacturacionesCodigoComponente(string cdFacturacion)
        {
            var facturacionCodigoComponente = new List<FacturacionCodigoComponente>();
            var entities = this._context.T_FACTURACION_CODIGO_COMPONEN
                .Include("T_FACTURACION_CODIGO")
                .AsNoTracking()
                .Where(x => x.T_FACTURACION_CODIGO.TP_CALCULO == "M"
                    && x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim());

            foreach (var entity in entities)
            {
                facturacionCodigoComponente.Add(this._mapper.MapToObject(entity));
            }

            return facturacionCodigoComponente;
        }

        public virtual FacturacionUnidadMedida GetFacturacionUnidadMedida(string unidadMedida)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_UNIDAD_MEDIDA
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_UNIDADE_MEDIDA.ToUpper().Trim() == unidadMedida.ToUpper().Trim()));
        }

        public virtual List<FacturacionUnidadMedida> GetFacturacionesUnidadMedida()
        {
            var facturacionUnidadMedida = new List<FacturacionUnidadMedida>();
            var entities = this._context.T_FACTURACION_UNIDAD_MEDIDA.AsNoTracking();

            foreach (var entity in entities)
            {
                facturacionUnidadMedida.Add(this._mapper.MapToObject(entity));
            }

            return facturacionUnidadMedida;
        }

        public virtual FacturacionEmpresaProceso GetFacturacionEmpresaProceso(int cdEmpresa, string cdProceso)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_EMPRESA_PROCESO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_EMPRESA == cdEmpresa
                    && x.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim()));
        }

        public virtual FacturacionCodigo GetFacturacionCodigo(string cdFacturacion)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_CODIGO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()));
        }

        public virtual List<FacturacionCodigo> GetAllFacturacionCodigos()
        {
            var entities = this._context.T_FACTURACION_CODIGO.AsNoTracking();
            var codigosFacturacion = new List<FacturacionCodigo>();

            foreach (var entity in entities)
            {
                codigosFacturacion.Add(this._mapper.MapToObject(entity));
            }

            return codigosFacturacion;
        }

        public virtual List<FacturacionCodigo> GetAllFacturacionCodigosByTpCalculo(string tpCalculo)
        {
            var entities = this._context.T_FACTURACION_CODIGO
                .AsNoTracking()
                .Where(w => w.TP_CALCULO.ToUpper().Trim() == tpCalculo.ToUpper().Trim());

            var codigosFacturacion = new List<FacturacionCodigo>();

            foreach (var entity in entities)
            {
                codigosFacturacion.Add(this._mapper.MapToObject(entity));
            }

            return codigosFacturacion;
        }

        public virtual List<CuentaContable> GetAllCuentaContables()
        {
            var entities = this._context.T_FACTURACION_CUENTA_CONTABLE.AsNoTracking();
            var cuentasContables = new List<CuentaContable>();

            foreach (var entity in entities)
            {
                cuentasContables.Add(this._mapper.MapToObject(entity));
            }

            return cuentasContables;
        }

        public virtual FacturacionCodigoComponente GetFacturacionCodigoComponente(string componente, string cdFacturacion)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_CODIGO_COMPONEN
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim()
                    && x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()));
        }

        public virtual List<FacturacionCodigoComponente> GetAllFacturacionCodigoComponente()
        {
            var codigosComponentes = new List<FacturacionCodigoComponente>();
            var entites = this._context.T_FACTURACION_CODIGO_COMPONEN.AsNoTracking();

            foreach (T_FACTURACION_CODIGO_COMPONEN entity in entites)
            {
                codigosComponentes.Add(this._mapper.MapToObject(entity));
            }

            return codigosComponentes;
        }

        public virtual List<Componente> GetAllComponentes()
        {
            var componentes = new List<Componente>();
            var entites = this._context.V_ORT_FUNC_COMP_COR18.AsNoTracking();

            foreach (V_ORT_FUNC_COMP_COR18 entity in entites)
            {
                componentes.Add(this._mapper.MapToObject(entity));
            }

            return componentes;
        }

        public virtual List<FacturacionCodigoComponente> GetComponentesByFacturacion(string cdFacturacion)
        {
            var codigosComponentes = new List<FacturacionCodigoComponente>();
            var entites = this._context.T_FACTURACION_CODIGO_COMPONEN
                .AsNoTracking()
                .Where(x => x.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim());

            foreach (T_FACTURACION_CODIGO_COMPONEN entity in entites)
            {
                codigosComponentes.Add(this._mapper.MapToObject(entity));
            }

            return codigosComponentes;
        }

        public virtual List<FacturacionPalletDet> GetFacturacionPalletDet(int nroEjecucion, int cdEmpresa, string cdFacturacion, string nroComponente, string estado)
        {
            var facturacionPalletDets = new List<FacturacionPalletDet>();
            var entites = this._context.T_FACTURACION_PALLET_DET
                .AsNoTracking()
                .Where(s => s.NU_EJECUCION_FACTURACION == nroEjecucion
                    && s.CD_EMPRESA == cdEmpresa
                    && s.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()
                    && s.NU_COMPONENTE.ToUpper().Trim() == nroComponente.ToUpper().Trim()
                    && s.ID_ESTADO.ToUpper().Trim() == estado.ToUpper().Trim());

            foreach (T_FACTURACION_PALLET_DET entity in entites)
            {
                facturacionPalletDets.Add(this._mapper.MapToObject(entity));
            }

            return facturacionPalletDets;
        }

        public virtual FacturacionResultado GetFacturacionResultado(int cdEmpresa, string cdFacturacion, int nuEjecucion, string nuComponente)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_RESULTADO
                .AsNoTracking()
                .Where(f => f.CD_EMPRESA == cdEmpresa
                    && f.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()
                    && f.NU_EJECUCION == nuEjecucion
                    && f.NU_COMPONENTE.ToUpper().Trim() == nuComponente.ToUpper().Trim())
                .FirstOrDefault());
        }

        public virtual List<Pallet> GetPallets()
        {
            var pallets = new List<Pallet>();
            var entities = this._context.V_REG605_PALLETS
                .AsNoTracking();

            foreach (var entity in entities)
            {
                pallets.Add(this._mapper.MapToObject(entity));
            }

            return pallets;
        }

        public virtual List<Pallet> GetPalletByNombreOrCodePartial(string value)
        {
            short codigoPallet;
            if (short.TryParse(value, out codigoPallet))
            {
                return this._context.V_REG605_PALLETS.AsNoTracking()
                    .Where(d => d.DS_PALLET.ToLower().Contains(value.ToLower()) || d.CD_PALLET == codigoPallet)
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
            else
            {
                return this._context.V_REG605_PALLETS.AsNoTracking()
                    .Where(d => d.DS_PALLET.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }

        }

        public virtual Pallet GetPallet(short codigoPallet)
        {
            return this._mapper.MapToObject(this._context.V_REG605_PALLETS
                .AsNoTracking()
                .Where(p => p.CD_PALLET == codigoPallet)
                .FirstOrDefault());
        }

        public virtual FacturacionEjecucion GetFacturacionEjecucion(int nuEjecucion)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_EJECUCION
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_EJECUCION == nuEjecucion));
        }

        public virtual FacturacionEjecucion GetAnyFacturacionEjecucionSolapada(int nuEjecucion, int? cdEmpresa = null, string cdProceso = null, bool incluirProgramaciones = false, bool sobreProcesosAsignados = true)
        {
            var procesosEjecucion = null as IQueryable<T_FACTURACION_EMPRESA_PROCESO>;

            var ejecucion = this._context.T_FACTURACION_EJECUCION
                .FirstOrDefault(e => e.NU_EJECUCION == nuEjecucion);

            if (sobreProcesosAsignados)
            {
                procesosEjecucion = this._context.T_FACTURACION_EJEC_EMPRESA
                        .Where(fee => fee.NU_EJECUCION == nuEjecucion
                            && (!cdEmpresa.HasValue || fee.CD_EMPRESA == cdEmpresa.Value)
                            && (string.IsNullOrEmpty(cdProceso) || fee.CD_PROCESO == cdProceso))
                        .Join(this._context.T_FACTURACION_EMPRESA_PROCESO
                                .Include("T_FACTURACION_PROCESO"),
                            fee => new { fee.CD_EMPRESA, fee.CD_PROCESO },
                            ep => new { ep.CD_EMPRESA, ep.CD_PROCESO },
                            (fee, fep) => fep);
            }
            else
            {
                var procesosNoAsignados = this._context.V_FACTURACION_PROC_WFAC010
                    .GroupJoin(this._context.T_FACTURACION_EJEC_EMPRESA
                        .Where(fee => fee.NU_EJECUCION == nuEjecucion
                            && (!cdEmpresa.HasValue || fee.CD_EMPRESA == cdEmpresa.Value)
                            && (string.IsNullOrEmpty(cdProceso) || fee.CD_PROCESO == cdProceso)),
                        fp => new { fp.CD_EMPRESA, fp.CD_PROCESO },
                        fee => new { fee.CD_EMPRESA, fee.CD_PROCESO },
                        (fp, fees) => new { Fp = fp, Fees = fees })
                    .SelectMany(fpfees => fpfees.Fees.DefaultIfEmpty(), (fpfees, fee) => new { Fp = fpfees.Fp, Fee = fee })
                    .Where(fpfee => fpfee.Fee == null
                        && (ejecucion.FL_EJEC_POR_HORA != "S" || fpfee.Fp.FL_EJEC_POR_HORA == "S"))
                    .Select(fpfee => fpfee.Fp);

                procesosEjecucion = procesosNoAsignados
                    .Join(this._context.T_FACTURACION_EMPRESA_PROCESO
                        .Include("T_FACTURACION_PROCESO"),
                        pna => new { pna.CD_EMPRESA, pna.CD_PROCESO },
                        fep => new { fep.CD_EMPRESA, fep.CD_PROCESO },
                        (fee, fep) => fep);
            }

            var procesosSolapados = this._context.T_FACTURACION_EJEC_EMPRESA
                .Include("T_FACTURACION_EJECUCION")
                .Where(fee => fee.NU_EJECUCION != nuEjecucion
                    && (fee.T_FACTURACION_EJECUCION.CD_SITUACAO != SituacionDb.EJECUCION_ANULADA && fee.T_FACTURACION_EJECUCION.CD_SITUACAO != SituacionDb.CALCULO_RECHAZADO)
                    && (incluirProgramaciones || fee.T_FACTURACION_EJECUCION.CD_SITUACAO != SituacionDb.EJECUCION_EN_PROGRAMACION)
                    && (!cdEmpresa.HasValue || fee.CD_EMPRESA == cdEmpresa.Value)
                    && (string.IsNullOrEmpty(cdProceso) || fee.CD_PROCESO == cdProceso));

            var ejecucionSolapada = procesosSolapados
                .Join(procesosEjecucion,
                    s => new { s.CD_EMPRESA, s.CD_PROCESO },
                    e => new { e.CD_EMPRESA, e.CD_PROCESO },
                    (s, e) => new
                    {
                        Solapado = s.T_FACTURACION_EJECUCION,
                        SolapadoEjecPorHora = s.T_FACTURACION_EJECUCION.FL_EJEC_POR_HORA,
                        SolapadoDesde = s.DT_DESDE,
                        SolapadoHasta = s.DT_HASTA,
                        EjecucionEjecPorHora = ejecucion.FL_EJEC_POR_HORA,
                        EjecucionDesde = ejecucion.DT_DESDE,
                        EjecucionHasta = ejecucion.DT_HASTA,
                        EjecPorHoraProceso = e.T_FACTURACION_PROCESO.FL_EJEC_POR_HORA,
                        UltimoProceso = e.HR_ULTIMO_PROCESO,
                    }
                )
                .AsNoTracking()
                .Select(j => new
                {
                    Solapado = j.Solapado,
                    SolapadoEjecPorHora = j.SolapadoEjecPorHora != "N",
                    SolapadoDesde = (j.SolapadoDesde ?? j.UltimoProceso) ?? DateTime.MinValue,
                    SolapadoHasta = j.SolapadoHasta ?? DateTime.MaxValue,
                    EjecucionEjecPorHora = j.EjecucionEjecPorHora != "N",
                    EjecucionDesde = (j.EjecucionDesde ?? j.UltimoProceso) ?? DateTime.MinValue,
                    EjecucionHasta = j.EjecucionHasta ?? DateTime.MaxValue,
                    EjecPorHoraProceso = j.EjecPorHoraProceso != "N"
                })
                .Select(j => new
                {
                    Solapado = j.Solapado,
                    SolapadoDesde = j.SolapadoEjecPorHora || j.EjecPorHoraProceso ? j.SolapadoDesde.AddDays(1 / (24 * 60)) : j.SolapadoDesde.Date,
                    SolapadoHasta = j.SolapadoEjecPorHora ? j.SolapadoHasta : j.SolapadoHasta.Date.AddDays(1 - (1 / (24 * 60))),
                    EjecucionDesde = j.EjecucionEjecPorHora || j.EjecPorHoraProceso ? j.EjecucionDesde.AddDays(1 / (24 * 60)) : j.EjecucionDesde.Date,
                    EjecucionHasta = j.EjecucionEjecPorHora ? j.EjecucionHasta : j.EjecucionHasta.Date.AddDays(1 - (1 / (24 * 60))),
                })
                .Where(j => !(j.SolapadoHasta < j.EjecucionDesde
                    || j.SolapadoDesde > j.EjecucionHasta))
                .Select(j => j.Solapado)
                .OrderBy(s => s.NU_EJECUCION)
                .FirstOrDefault();

            return ejecucionSolapada == null ? null : this._mapper.MapToObject(ejecucionSolapada);
        }

        protected virtual DateTime? GetFechaDesde(T_FACTURACION_EJEC_EMPRESA fee, T_FACTURACION_EMPRESA_PROCESO fep)
        {
            return GetFechaDesde(fee.DT_DESDE ?? fep.HR_ULTIMO_PROCESO, !(fee.T_FACTURACION_EJECUCION.FL_EJEC_POR_HORA == "N" && fee.T_FACTURACION_PROCESO.FL_EJEC_POR_HORA == "N"));
        }

        protected virtual DateTime? GetFechaDesde(DateTime? fechaDesde, bool ejecPorHora)
        {
            return ejecPorHora ? fechaDesde?.AddMinutes(1) : fechaDesde?.Date;
        }

        protected virtual DateTime? GetFechaHasta(T_FACTURACION_EJEC_EMPRESA fee)
        {
            return fee.DT_HASTA.HasValue ? (fee.T_FACTURACION_EJECUCION.FL_EJEC_POR_HORA == "S" ? fee.DT_HASTA : new DateTime(fee.DT_HASTA.Value.Year, fee.DT_HASTA.Value.Month, fee.DT_HASTA.Value.Day, 23, 59, 59)) : null;
        }

        public virtual List<FacturacionEjecucionEmpresa> GetFacturacionEjecucionEmpresa(int nuEjecucion)
        {
            var facturacionEjecucionEmpresa = new List<FacturacionEjecucionEmpresa>();
            var entities = this._context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .Where(w => w.NU_EJECUCION == nuEjecucion);

            foreach (var entity in entities)
            {
                facturacionEjecucionEmpresa.Add(this._mapper.MapToObject(entity));
            }

            return facturacionEjecucionEmpresa;
        }

        public virtual List<string> GetProcesosFacturacionEjecucionEmpresa(int nuEjecucion, int empresa)
        {
            var procesos = new List<string>();
            var entities = this._context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .Where(w => w.NU_EJECUCION == nuEjecucion
                    && w.CD_EMPRESA == empresa);

            foreach (var entity in entities)
            {
                procesos.Add(entity.CD_PROCESO);
            }

            return procesos;
        }

        public virtual List<string> GetFacturacionProceso(int nuEjecucion, int empresa, string cdFacturacion)
        {
            return this._context.T_FACTURACION_EJEC_EMPRESA
                .Include("T_FACTURACION_PROCESO")
                .AsNoTracking()
                .Where(w => w.NU_EJECUCION == nuEjecucion
                    && w.CD_EMPRESA == empresa
                    && w.T_FACTURACION_PROCESO.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim()
                    && w.T_FACTURACION_PROCESO.TP_PROCESO == "M")
                .GroupBy(g => g.CD_PROCESO)
                .Select(s => s.Key)
                .ToList();
        }

        public virtual List<string> GetFacturacionProcesoComponente(string cdProceso, string cdFacturacion)
        {
            return this._context.T_FACTURACION_PROCESO
                    .AsNoTracking()
                    .Where(w => w.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim()
                        && w.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim())
                    .Select(s => s.CD_PROCESO)
                    .ToList();
        }

        public virtual FacturacionEjecucionEmpresa GetFacturacionEjecucionEmpresa(int nuEjecucion, int cdEmpresa, string cdProceso)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_EJECUCION == nuEjecucion
                    && w.CD_EMPRESA == cdEmpresa
                    && w.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim()));
        }

        public virtual FacturacionEjecucionEmpresa GetFacturacionEjecucionEmpresaAnterior(int nuEjecucion, int cdEmpresa, string cdProceso)
        {
            var entity = _context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .Where(r => r.CD_EMPRESA == cdEmpresa
                    && r.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim()
                    && r.ID_ESTADO == FacturacionDb.ESTADO_HAB
                    && r.NU_EJECUCION != nuEjecucion)
                .OrderByDescending(r => r.NU_EJECUCION).FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual List<FacturacionEjecucionEmpresa> GetFacturacionEjecucionEmpresaByNuEjecucion(int nuEjecucion)
        {
            var facturacionEjecucionEmpresa = new List<FacturacionEjecucionEmpresa>();
            var entites = this._context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .Where(w => w.NU_EJECUCION == nuEjecucion);

            foreach (T_FACTURACION_EJEC_EMPRESA entity in entites)
            {
                facturacionEjecucionEmpresa.Add(this._mapper.MapToObject(entity));
            }

            return facturacionEjecucionEmpresa;
        }

        public virtual FacturacionProceso GetFacturacionProceso(string cdProceso)
        {
            var entity = _context.T_FACTURACION_PROCESO
                .AsNoTracking()
                .FirstOrDefault(w => w.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim());

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual List<FacturacionProceso> GetFacturacionesProceso()
        {
            var facturacionProceso = new List<FacturacionProceso>();
            var entites = this._context.T_FACTURACION_PROCESO.AsNoTracking();

            foreach (T_FACTURACION_PROCESO entity in entites)
            {
                facturacionProceso.Add(this._mapper.MapToObject(entity));
            }

            return facturacionProceso;
        }

        public virtual T_PRODUTO GetProduto(List<T_PRODUTO> memoryProd, int cd_empresa, string cd_produto, string cd_facturacion, string componente)
        {
            T_PRODUTO producto = memoryProd.FirstOrDefault(s => s.CD_EMPRESA == cd_empresa && s.CD_PRODUTO == cd_produto);

            if (producto == null)
            {
                producto = this._context.T_PRODUTO
                    .Include("T_UNIDADE_MEDIDA")
                    .Include("T_EMPRESA")
                    .AsNoTracking()
                    .FirstOrDefault(s => s.CD_EMPRESA == cd_empresa
                    && s.CD_PRODUTO.ToUpper().Trim() == cd_produto.ToUpper().Trim());

                V_FACTURACION_PRECIO_EMPRESA precioEmp = this._context.V_FACTURACION_PRECIO_EMPRESA
                    .AsNoTracking()
                    .FirstOrDefault(s => s.CD_EMPRESA == cd_empresa
                        //&& s.CD_LISTA_PRECIO == producto.T_EMPRESA.CD_LISTA_PRECIO
                        && s.CD_FACTURACION.ToUpper().Trim() == cd_facturacion.ToUpper().Trim()
                        && s.NU_COMPONENTE.ToUpper().Trim() == componente.ToUpper().Trim());

                decimal? precioUnit = 0;

                if (precioEmp != null)
                    precioUnit = precioEmp.VL_PRECIO_UNITARIO;

                producto.AUX_VL_PRECIO_UNITARIO = precioUnit;

                memoryProd.Add(producto);
            }

            return producto;
        }

        public virtual int GetDays(DateTime fIni, DateTime fFin)
        {
            int estadia = 1;
            TimeSpan ts = (DateTime)fFin - fIni;
            ts = ts.Add(new TimeSpan(0, 0, 1));

            estadia += ts.Days;

            return estadia;
        }

        public virtual byte[] GetExcel<T>(List<T> data, string name, string[] ColumnsToRemove)
        {
            byte[] bt;
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(name);
                List<MemberInfo> lstmi = new List<MemberInfo>();

                PropertyInfo[] lst = typeof(T).GetProperties();

                lst.ToList().ForEach(col =>
                {
                    PropertyInfo pi = typeof(T).GetProperty(col.Name);
                    if (!ColumnsToRemove.Contains(col.Name))
                        lstmi.Add(pi);
                });

                MemberInfo[] miar = lstmi.ToArray();
                var range = LoadFromCollectionInternal(worksheet, data, true, XLWorkbook.DefaultStyle, miar);
                var dateList = new List<int>();
                var dupedList = new List<string>();
                int index = 0;

                List<IXLCell> cellsPendingTranslation = new List<IXLCell>();

                foreach (var column in lstmi)
                {
                    this.AlterColumn(column, range, worksheet, dupedList, lstmi, dateList, index);

                    index++;
                }

                worksheet.Columns().AdjustToContents();
                FreezePanes(worksheet, 2, 0);

                bt = GetAsByteArray(workbook);
            }
            return bt;
        }
        
        public virtual void AlterColumn(MemberInfo column, IXLRange range, IXLWorksheet worksheet, List<string> dupedList, List<MemberInfo> infoList, List<int> dateList, int index)
        {
            string columnName = column.Name.Replace("\n", "").Trim();

            range.Cell(1, index + 1).Value = columnName;

            int count = range.Cells(c => c.WorksheetRow().RowNumber() == 1 && c.Value.ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase)).Count();

            if (count > 1)
            {
                dupedList.Add(columnName);

                range.Cell(1, index + 1).Value = columnName + "~" + dupedList.Count(s => s == columnName);
            }

            var columnInfo = (PropertyInfo)infoList[index];

            if (columnInfo.PropertyType.Equals(typeof(DateTime)) || columnInfo.PropertyType.Equals(typeof(DateTime?)))
            {
                worksheet.Column(index + 1).Style.NumberFormat.Format = "dd/MM/yyyy HH:mm:ss";

                dateList.Add(index + 1);
            }
        }
        
        public virtual IXLRange LoadFromCollectionInternal<T>(IXLWorksheet worksheet, IList<T> collection, bool printHeaders, IXLStyle style, MemberInfo[] memberInfos)
        {

            var defaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

            LoadFromCollectionParams parameters = new LoadFromCollectionParams
            {
                PrintHeaders = printHeaders,
                TableStyle = style,
                BindingFlags = defaultBindingFlags,
                Members = memberInfos
            };

            return new LoadFromCollection<T>(worksheet, collection, parameters).Load();

        }
        
        public virtual void FreezePanes(IXLWorksheet worksheet, int rows, int columnns)
        {
            worksheet.SheetView.Freeze(rows, columnns);
        }
        
        public virtual byte[] GetAsByteArray(XLWorkbook workbook)
        {
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }
        
        public virtual List<V_FOTO_STOCK_PALLET> GetFOTOSTOCK(List<V_FOTO_STOCK_PALLET> ListFOTO, List<V_FOTO_STOCK_PALLET> memoryPallet, string NU_PALLET, DateTime? fechaInicioP, DateTime? fechaFINP)
        {
            String Pallet = "@" + NU_PALLET;
            DateTime? EJ_F_DESDE = fechaInicioP;
            DateTime EJ_F_HASTA = fechaInicioP == null ? DateTime.Now : fechaInicioP.Value.AddDays(1);
            List<V_FOTO_STOCK_PALLET> Pallet1 = null;

            Pallet1 = memoryPallet.Where(s => s.NU_IDENTIFICADOR.EndsWith(Pallet)).ToList();

            if (Pallet1.Count == 0)
            {
                Pallet1 = ListFOTO.Where(x => x.NU_IDENTIFICADOR.EndsWith(Pallet)).ToList();
                foreach (V_FOTO_STOCK_PALLET resultado in Pallet1)
                {
                    memoryPallet.Add(resultado);
                }
            }

            return Pallet1;
        }

        public virtual List<V_FOTO_STOCK_PALLET> GetFOTOSTOCK(WISDB wisdb, List<V_FOTO_STOCK_PALLET> ListFOTO, List<V_FOTO_STOCK_PALLET> memoryPallet, string NU_PALLET, DateTime? fechaInicioP, DateTime? fechaFINP)
        {
            String Pallet = "@" + NU_PALLET;
            DateTime? EJ_F_DESDE = fechaInicioP;
            DateTime EJ_F_HASTA = fechaInicioP == null ? DateTime.Now : fechaInicioP.Value.AddDays(1);
            List<V_FOTO_STOCK_PALLET> Pallet1 = null;

            Pallet1 = memoryPallet.Where(s => s.NU_IDENTIFICADOR.EndsWith(Pallet)).ToList();

            if (Pallet1.Count == 0)
            {
                Pallet1 = ListFOTO.Where(x => x.NU_IDENTIFICADOR.EndsWith(Pallet)).ToList();
                foreach (V_FOTO_STOCK_PALLET resultado in Pallet1)
                {
                    memoryPallet.Add(resultado);
                }
            }

            return Pallet1;
        }

        protected virtual int Cantidad_de_prod(List<V_FOTO_STOCK_PALLET> PalletProd, String NU_PALLET)
        {
            int cont = 0;
            List<V_FOTO_STOCK_PALLET> lista = new List<V_FOTO_STOCK_PALLET>();
            List<V_FOTO_STOCK_PALLET> ListaPallet = PalletProd.Where(x => x.NU_IDENTIFICADOR.EndsWith(NU_PALLET)).ToList();

            foreach (V_FOTO_STOCK_PALLET resultado in ListaPallet)
            {
                String Pallet = "@" + NU_PALLET;
                if (lista.Count == 0)
                {
                    if (resultado.NU_IDENTIFICADOR.EndsWith(Pallet))
                    {
                        lista.Add(resultado);
                    }
                }
                else
                {
                    Boolean existe = true;
                    foreach (V_FOTO_STOCK_PALLET resultado1 in lista)
                    {
                        if (resultado.NU_IDENTIFICADOR.EndsWith(Pallet))
                        {
                            if (!resultado.CD_PRODUTO.Equals(resultado1.CD_PRODUTO))
                            {
                                existe = false;
                            }
                        }
                    }
                    if (existe == false)
                    {
                        lista.Add(resultado);
                    }
                }
            }

            cont = lista.Count;
            return cont;
        }

        public virtual List<DominioDetalle> GetTpCalculo()
        {
            return this._context.T_DET_DOMINIO
                .AsNoTracking()
                .Where(w => w.CD_DOMINIO == "TP_CALCULO")
                .Select(w => this._mapperDominio.MapToObject(w))
                .ToList();
        }

        public virtual int UltimoNroEjecucionProceso(int emp, string cdProceso, string estado)
        {
            return _context.T_FACTURACION_EJEC_EMPRESA
                .AsNoTracking()
                .Where(r => r.CD_EMPRESA == emp
                    && r.CD_PROCESO.ToUpper().Trim() == cdProceso.ToUpper().Trim()
                    && r.ID_ESTADO.ToUpper().Trim() == estado.ToUpper().Trim())
                .Max(s => s.NU_EJECUCION);
        }

        public virtual List<FacturacionResultado> GetResultadosEjecucion(int nroEjecucion, int empresa, string cdFacturacion)
        {
            return _context.T_FACTURACION_RESULTADO
                .AsNoTracking()
                .Where(r => r.NU_EJECUCION == nroEjecucion
                    && r.CD_EMPRESA == empresa
                    && r.CD_FACTURACION.ToUpper().Trim() == cdFacturacion.ToUpper().Trim())
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual List<FacturacionResultado> GetResultadosEjecucionSituacion(int nroEjecucion, List<short?> situaciones)
        {
            return _context.T_FACTURACION_RESULTADO
                .Where(r => r.NU_EJECUCION == nroEjecucion && situaciones.Contains(r.CD_SITUACAO))
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual List<FacturacionPalletDet> GetDetallesPalletsHabilitados(int nroEjecucion)
        {
            return _context.T_FACTURACION_PALLET_DET
                .Where(s => s.NU_EJECUCION_FACTURACION == nroEjecucion && s.ID_ESTADO == FacturacionDb.ESTADO_HAB)
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual Dictionary<int, int?> GetEmpresaListasPrecios(int nuEjecucion, List<short?> situaciones)
        {
            return _context.T_FACTURACION_RESULTADO
                .Where(fr => fr.NU_EJECUCION == nuEjecucion && situaciones.Contains(fr.CD_SITUACAO))
                .Join(_context.T_EMPRESA.Where(e => e.CD_LISTA_PRECIO.HasValue),
                fr => fr.CD_EMPRESA,
                e => e.CD_EMPRESA,
                (fr, e) => e)
                .GroupBy(e => new { e.CD_EMPRESA, e.CD_LISTA_PRECIO })
                .AsNoTracking()
                .Select(e => e.Key)
                .ToDictionary(e => e.CD_EMPRESA, e => e.CD_LISTA_PRECIO) ?? new Dictionary<int, int?>();
        }

        public virtual Dictionary<int, ListaPrecio> GetListasDePrecios(int nuEjecucion, List<short?> situaciones)
        {
            var listasPrecios = _context.T_FACTURACION_RESULTADO
                .Where(fr => fr.NU_EJECUCION == nuEjecucion && situaciones.Contains(fr.CD_SITUACAO))
                .Join(_context.T_EMPRESA.Where(e => e.CD_LISTA_PRECIO.HasValue),
                fr => fr.CD_EMPRESA,
                e => e.CD_EMPRESA,
                (fr, e) => e)
                .GroupBy(e => e.CD_LISTA_PRECIO)
                .AsNoTracking()
                .Select(g => new { CD_LISTA_PRECIO = g.Key });

            return _context.T_FACTURACION_LISTA_PRECIO
                .Join(listasPrecios,
                flp => flp.CD_LISTA_PRECIO,
                lp => lp.CD_LISTA_PRECIO,
                (flp, lp) => flp)
                .AsNoTracking()
                .Select(flp => _mapperListaPrecio.MapToObject(flp))
                .ToDictionary(lp => lp.Id, lp => lp) ?? new Dictionary<int, ListaPrecio>();
        }

        public virtual Dictionary<string, CotizacionListas> GetListasDeCotizacion(int nuEjecucion, List<short?> situaciones)
        {
            var listasPrecios = _context.T_FACTURACION_RESULTADO
                .Where(fr => fr.NU_EJECUCION == nuEjecucion && situaciones.Contains(fr.CD_SITUACAO))
                .Join(_context.T_EMPRESA.Where(e => e.CD_LISTA_PRECIO.HasValue),
                fr => fr.CD_EMPRESA,
                e => e.CD_EMPRESA,
                (fr, e) => e)
                .GroupBy(e => e.CD_LISTA_PRECIO)
                .AsNoTracking()
                .Select(g => new { CD_LISTA_PRECIO = g.Key });

            return _context.T_FACTURACION_LISTA_COTIZACION
                .Join(listasPrecios,
                flc => flc.CD_LISTA_PRECIO,
                lp => lp.CD_LISTA_PRECIO,
                (flc, lp) => flc)
                .AsNoTracking()
                .Select(flc => _mapperListaCotizacion.MapToObject(flc))
                .ToDictionary(lc => $"{lc.CodigoListaPrecio}.{lc.CodigoFacturacion}.{lc.NumeroComponente}", lc => lc) ?? new Dictionary<string, CotizacionListas>();
        }

        #endregion

        #region ExcelCOR
       
        public virtual byte[] DescargarExcel(int nuEjecucion, int cdEmpresa, string cdFacturacion, string nameFile)
        {
            byte[] bt = null;
            switch (cdFacturacion)
            {
                //COR
                case "FAC001":
                    string[] ColumnsToRemove17 = new string[] { "NU_RESULTADO_DETALLE", "CD_FACTURACION", "DT_ADDROW" };
                    bt = ExcelFAC001(nuEjecucion, nameFile, ColumnsToRemove17);
                    break;
                case "COR_02":
                    string[] ColumnsToRemove1 = new string[] { "PRUEBA", "NU_IDENTIFICADOR", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_02(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove1);
                    break;
                case "COR_03":
                    string[] ColumnsToRemove = new string[] { "PRUEBA", "NU_PALLET", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "PS_BULTO" };
                    bt = ExcelCOR_03(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove);
                    break;
                case "COR_04":
                    string[] ColumnsToRemove11 = new string[] { "PRUEBA", "NU_PALLET", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA" };
                    bt = ExcelCOR_04(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove11);
                    break;
                case "COR_05":
                    string[] ColumnsToRemove4 = new string[] { "PRUEBA", "QT_ESTADIA", "DT_RETIRO", "NU_PALLET", "NU_COMPONENTE", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_05(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove4);
                    break;
                case "COR_06":
                    string[] ColumnsToRemove5 = new string[] { "PRUEBA", "NU_IDENTIFICADOR", "QT_ESTADIA", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "DT_RETIRO", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_06(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove5);
                    break;
                case "COR_08":
                    string[] ColumnsToRemove7 = new string[] { "PRUEBA", "NU_IDENTIFICADOR", "QT_ESTADIA", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "DT_RETIRO", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_08(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove7);
                    break;
                case "COR_09":
                    string[] ColumnsToRemove8 = new string[] { "PRUEBA", "QT_ESTADIA", "DT_RETIRO", "NU_PALLET", "NU_COMPONENTE", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_09(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove8);
                    break;
                case "COR_10":
                    string[] ColumnsToRemove2 = new string[] { "PRUEBA", "NU_IDENTIFICADOR", "DS_PRODUTO", "QT_ESTADIA", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_10(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove2);
                    break;
                case "COR_11":
                    string[] ColumnsToRemove3 = new string[] { "PRUEBA", "QT_ESTADIA", "DT_RETIRO", "NU_PALLET", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "NU_COMPONENTE", "TP_RESULTADO", "PS_BULTO" };
                    bt = ExcelCOR_11(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove3);
                    break;
                case "COR_12":
                    string[] ColumnsToRemove6 = new string[] { "PRUEBA", "QT_ESTADIA", "DT_RETIRO", "NU_PALLET", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "NU_COMPONENTE", "TP_RESULTADO", "PS_BULTO" };
                    bt = ExcelCOR_12(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove6);
                    break;
                case "COR_14":
                    string[] ColumnsToRemove9 = new string[] { "PRUEBA", "QT_ESTADIA", "DT_INGRESO", "NU_PALLET", "NU_COMPONENTE", "TP_RESULTADO" };
                    bt = ExcelCOR_14(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove9);
                    break;
                case "COR_21":
                    string[] ColumnsToRemove10 = new string[] { "PRUEBA", "NU_IDENTIFICADOR", "QT_ESTADIA", "QT_UND_BULTO", "CD_UNIDADE_MEDIDA", "DT_INGRESO", "PS_BULTO", "TP_RESULTADO" };
                    bt = ExcelCOR_21(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove10);
                    break;
                //COD
                case "COD_04":
                    string[] ColumnsToRemove12 = new string[] { "PRUEBA","CD_CLIENTE",
                        "CD_FAIXA","DT_RETIRO","NU_COMPONENTE","NU_FOTO","NU_PALLET","NU_PEDIDO","NU_PREDIO","PRUEBA","PS_BULTO","QT_ESTADIA",
                        "QT_PRODUTO","QT_UND_BULTO","TP_RESULTADO",};
                    bt = ExcelCOD_04(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove12);
                    break;
                case "COD_05":
                    string[] ColumnsToRemove13 = new string[] { "PRUEBA" ,"CD_CLIENTE","DS_ENVASE","DT_INGRESO","DT_RETIRO",
                        "NU_AGENDA","NU_PALLET","NU_PEDIDO","PS_BULTO","QT_ESTADIA","QT_PRODUTO","TP_RESULTADO", };
                    bt = ExcelCOD_05(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove13);
                    break;
                case "COD_06":
                    string[] ColumnsToRemove14 = new string[] {"PRUEBA","CD_FAIXA","CD_PRODUTO","CD_UNID_EMB","CD_UNIDADE_MEDIDA",
                        "DT_INGRESO","NU_AGENDA","NU_COMPONENTE","NU_ETIQUETA_LOTE","NU_FOTO","NU_IDENTIFICADOR","NU_PALLET","NU_PREDIO",
                        "PRUEBA","PS_BULTO","QT_ESTADIA","QT_PRODUTO","QT_UND_BULTO","TP_RESULTADO",  };
                    bt = ExcelCOD_06(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove14);
                    break;
                case "COD_07":
                    string[] ColumnsToRemove15 = new string[] { "PRUEBA",  "CD_FAIXA","DT_INGRESO","NU_AGENDA","NU_COMPONENTE",
                        "NU_ETIQUETA_LOTE","NU_FOTO","NU_PALLET","NU_PREDIO","PRUEBA","PS_BULTO","QT_ESTADIA","TP_RESULTADO",       };
                    bt = ExcelCOD_07(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove15);
                    break;
                case "COD_08":
                    string[] ColumnsToRemove16 = new string[] { "PRUEBA",  "CD_FAIXA","DT_INGRESO","NU_AGENDA","NU_COMPONENTE",
                        "NU_ETIQUETA_LOTE","NU_FOTO","NU_PALLET","NU_PREDIO","PRUEBA","PS_BULTO","QT_ESTADIA","TP_RESULTADO",};
                    bt = ExcelCOD_08(nuEjecucion, cdEmpresa, cdFacturacion, nameFile, ColumnsToRemove16);
                    break;
            }
            return bt;
        }

        public byte[] ExcelFAC001(int nu_ejecucion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                List<V_FAC008_RESULTADO_DETALLE> lst = 
                    this._context.V_FAC008_RESULTADO_DETALLE.AsNoTracking().Where(s =>
                        s.NU_EJECUCION == nu_ejecucion
                    ).ToList().OrderBy(w => w.CD_EMPRESA).ThenBy(w => w.CD_FACTURACION).ThenBy(w => w.NU_COMPONENTE).ToList();

                byte[] bt = GetExcel<V_FAC008_RESULTADO_DETALLE>(lst, nameFile, ColumnsToRemove);

                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual byte[] ExcelCOR_02(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<V_FOTO_STOCK_PALLET> memoryPallet = new List<V_FOTO_STOCK_PALLET>();
                List<V_FACTURACION_COR_DET_02> logs = this._context.V_FACTURACION_COR_DET_02.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION_FACTURACION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();

                T_FACTURACION_EJECUCION eje = this._context.T_FACTURACION_EJECUCION.Where(s =>
                                                                s.NU_EJECUCION == nu_ejecucion).FirstOrDefault();
                List<V_FOTO_STOCK_PALLET> LISTPalletProd = this._context.V_FOTO_STOCK_PALLET.Where(x => x.DT_FOTO > eje.DT_DESDE && x.DT_FOTO < eje.DT_HASTA && x.CD_EMPRESA == cd_empresa).ToList();

                List<V_FOTO_STOCK_PALLET> ListPalletProdAgrupado = (List<V_FOTO_STOCK_PALLET>)LISTPalletProd.GroupBy(x => new { x.NU_IDENTIFICADOR, x.CD_PRODUTO, x.NU_PALLET, x.CD_EMPRESA, x.DS_PRODUTO })
                                                                    .Select(y => new V_FOTO_STOCK_PALLET()
                                                                    {
                                                                        CD_PRODUTO = y.Key.CD_PRODUTO,
                                                                        NU_IDENTIFICADOR = y.Key.NU_IDENTIFICADOR,
                                                                        NU_PALLET = y.Key.NU_PALLET,
                                                                        CD_EMPRESA = y.Key.CD_EMPRESA,
                                                                        DS_PRODUTO = y.Key.DS_PRODUTO
                                                                    }
                                                                    ).ToList();

                foreach (V_FACTURACION_COR_DET_02 resultado in logs)
                {
                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    if (resultado.FL_APLICO_MINIMO != null)
                    {
                        if (resultado.FL_APLICO_MINIMO.Equals("S"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }
                    DateTime? DT_RETIRO = null;
                    if (resultado.DT_DESTRUCCION_PALLET <= resultado.DT_HASTA)
                    {
                        DT_RETIRO = resultado.DT_DESTRUCCION_PALLET;

                    }
                    String DS_PRODUTO;
                    List<V_FOTO_STOCK_PALLET> PalletProd = GetFOTOSTOCK(this._context, ListPalletProdAgrupado, memoryPallet, resultado.NU_PALLET, resultado.DT_DESDE, resultado.DT_HASTA);
                    int cont = Cantidad_de_prod(PalletProd, resultado.NU_PALLET);
                    if (cont == 1)
                    {
                        DS_PRODUTO = PalletProd[0].DS_PRODUTO;
                    }
                    else
                    {
                        DS_PRODUTO = "VARIOS";
                    }
                    lst.Add(new AUXCOR()
                    {
                        NU_PALLET = resultado.NU_PALLET,
                        DS_ENVASE = "PALLET",
                        NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_ADDROW,
                        DT_RETIRO = DT_RETIRO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA_INGRESO,
                        QT_CANTIDAD = 1,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        QT_ESTADIA = (int)resultado.QT_RESULTADO,
                        TP_RESULTADO = tp_resultado,
                        //poner el tipo
                        DS_PRODUTO = DS_PRODUTO,
                    });

                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                //this._logger.Info("LOG_EXCEL ", ts2 + " -  GENERAR EXCEL - TIEMPO: -");
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                //this._logger.Info("LOG_EXCEL ", ts3 + " - AFTER GENERAR EXCEL - TIEMPO: -");
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_03(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                //DateTime tiempo = DateTime.Now;
                //DateTime tiempo2 = tiempo;
                //System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<T_PRODUTO> memoryProd = new List<T_PRODUTO>();
                //T_FACTURACION_EJECUCION ejecucion = this._context.T_FACTURACION_EJECUCION.FirstOrDefault(s => s.NU_EJECUCION == nu_ejecucion);
                T_FACTURACION_EJEC_EMPRESA ejeEmp = this._context.T_FACTURACION_EJEC_EMPRESA.FirstOrDefault(s => s.NU_EJECUCION == nu_ejecucion
                                                                                                        && s.CD_EMPRESA == cd_empresa
                                                                                                        && s.CD_PROCESO == cd_facturacion);



                List<T_LOG_FACTURACION_ALM_BULTO> logs = this._context.T_LOG_FACTURACION_ALM_BULTO.AsNoTracking().Where(s =>
                                                                        s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).OrderBy(s => s.DT_INGRESO).ToList();

                string[] componentes = logs.GroupBy(s => s.NU_COMPONENTE).Select(s => s.Key).ToArray();

                for (int i = 0; i < componentes.Length; i++)
                {
                    string comp = componentes[i];

                    string[] lotes = logs.Where(s => s.NU_COMPONENTE == comp).GroupBy(s => s.NU_IDENTIFICADOR).Select(s => s.Key).ToArray();
                    for (int j = 0; j < lotes.Length; j++)
                    {
                        string lot = lotes[j];

                        T_LOG_FACTURACION_ALM_BULTO stockInicial = logs.Where(s => s.NU_IDENTIFICADOR == lot).OrderBy(s => s.DT_FECHA).FirstOrDefault();

                        // COMPLEMENTOS
                        T_LOG_FACTURACION_ALM_BULTO[] egresos = logs.Where(s => s.DT_RETIRO != null && s.NU_IDENTIFICADOR == lot && s.TP_RESULTADO == "FCM").ToArray();

                        for (int h = 0; h < egresos.Length; h++)
                        {
                            T_LOG_FACTURACION_ALM_BULTO egreso = egresos[h];
                            T_PRODUTO producto = GetProduto(memoryProd, cd_empresa, egreso.CD_PRODUTO, cd_facturacion, comp);
                            int ESTADIA = GetDays(stockInicial.DT_FECHA, (DateTime)egreso.DT_RETIRO) + (int)(egreso.QT_RESULTADO / egreso.QT_BULTOS);
                            lst.Add(new AUXCOR()
                            {
                                DS_ENVASE = producto.T_UNIDADE_MEDIDA.DS_UNIDADE_MEDIDA,
                                DS_PRODUTO = producto.DS_PRODUTO,
                                DT_INGRESO = egreso.DT_INGRESO,
                                DT_RETIRO = egreso.DT_RETIRO,
                                NU_COMPONENTE = comp,
                                IM_TARIFADIA = producto.AUX_VL_PRECIO_UNITARIO,
                                NU_AGENDA = egreso.NU_AGENDA,
                                NU_IDENTIFICADOR = egreso.NU_IDENTIFICADOR,
                                QT_CANTIDAD = egreso.QT_BULTOS,
                                QT_ESTADIA = ESTADIA,//GetDays(stockInicial.DT_FECHA, (DateTime)egreso.DT_RETIRO) + (int)(egreso.QT_RESULTADO / egreso.QT_BULTOS),
                                IM_IMPORTE = (egreso.QT_BULTOS * ESTADIA * producto.AUX_VL_PRECIO_UNITARIO),
                                TP_RESULTADO = (egreso.TP_RESULTADO == "FCM" ? "COMP" : "NORM")
                            });
                        }

                        T_LOG_FACTURACION_ALM_BULTO egresoFinal = egresos.OrderBy(s => s.DT_RETIRO).LastOrDefault();

                        // NORMAL
                        T_LOG_FACTURACION_ALM_BULTO[] stocksLot = logs.Where(s => s.NU_IDENTIFICADOR == lot && s.TP_RESULTADO == "FAC").OrderBy(s => s.DT_FECHA).ToArray();
                        for (int h = 0; h < stocksLot.Length; h++)
                        {
                            if (h == stocksLot.LongLength - 1)//ultimo (Stock al final de periodo)
                            {
                                T_LOG_FACTURACION_ALM_BULTO sto = stocksLot[h];
                                decimal QT_BUL = 0;
                                bool addStock = true;
                                if (egresoFinal != null)
                                {
                                    if (egresoFinal.DT_RETIRO >= sto.DT_FECHA)

                                        if (egresoFinal.QT_BULTOS == sto.QT_BULTOS)
                                            addStock = false;
                                        else
                                            QT_BUL = egresoFinal.QT_BULTOS;
                                }
                                else
                                {
                                    QT_BUL = 0;
                                }
                                if (addStock)
                                {

                                    T_PRODUTO producto = GetProduto(memoryProd, cd_empresa, sto.CD_PRODUTO, cd_facturacion, comp);
                                    AUXCOR aux = new AUXCOR()
                                    {
                                        DS_ENVASE = producto.T_UNIDADE_MEDIDA.DS_UNIDADE_MEDIDA,
                                        DS_PRODUTO = producto.DS_PRODUTO,
                                        NU_COMPONENTE = comp,
                                        DT_INGRESO = sto.DT_INGRESO,
                                        QT_ESTADIA = GetDays(stocksLot[0].DT_FECHA, sto.DT_FECHA),

                                        IM_TARIFADIA = producto.AUX_VL_PRECIO_UNITARIO,
                                        NU_AGENDA = sto.NU_AGENDA,
                                        NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR,
                                        QT_CANTIDAD = sto.QT_RESULTADO - QT_BUL,
                                        TP_RESULTADO = (sto.TP_RESULTADO == "FCM" ? "COMP" : "NORM")
                                    };
                                    aux.IM_IMPORTE = ((sto.QT_RESULTADO - QT_BUL) * producto.AUX_VL_PRECIO_UNITARIO) * aux.QT_ESTADIA;

                                    lst.Add(aux);

                                    if (sto.DT_FECHA.Date < ejeEmp.DT_HASTA.Value.Date)
                                    {
                                        aux.DT_RETIRO = sto.DT_FECHA;
                                    }
                                    // this._logger.Info("DETALLE_EXCEL_CORS, string.Format("COR_03> ADD FINAL: NU_COMPONENTE {0} | NU_IDENTIFICADOR {1} | QT_CANTIDAD {2} | TP_RESULTADO {3}", aux.NU_COMPONENTE, aux.NU_IDENTIFICADOR, aux.QT_CANTIDAD, aux.TP_RESULTADO));
                                }
                            }
                            else
                            {
                                T_LOG_FACTURACION_ALM_BULTO sto = stocksLot[h];
                                T_LOG_FACTURACION_ALM_BULTO stoProx = stocksLot[h + 1];

                                if (sto.QT_RESULTADO > stoProx.QT_RESULTADO)
                                {
                                    bool insert = !egresos.Any(s => s.DT_RETIRO.Value.Day == sto.DT_FECHA.Day
                                                                && s.DT_RETIRO.Value.Month == sto.DT_FECHA.Month
                                                                && s.DT_RETIRO.Value.Year == sto.DT_FECHA.Year);
                                    if (insert)
                                    {
                                        T_PRODUTO producto = GetProduto(memoryProd, cd_empresa, sto.CD_PRODUTO, cd_facturacion, comp);
                                        AUXCOR aux2 = new AUXCOR()
                                        {
                                            DS_ENVASE = producto.T_UNIDADE_MEDIDA.DS_UNIDADE_MEDIDA,
                                            DS_PRODUTO = producto.DS_PRODUTO,
                                            NU_COMPONENTE = comp,
                                            DT_INGRESO = sto.DT_INGRESO,
                                            DT_RETIRO = sto.DT_FECHA,

                                            IM_TARIFADIA = producto.AUX_VL_PRECIO_UNITARIO,
                                            NU_AGENDA = sto.NU_AGENDA,
                                            NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR,
                                            QT_CANTIDAD = (sto.QT_RESULTADO - stoProx.QT_RESULTADO),
                                            QT_ESTADIA = GetDays(stocksLot[0].DT_FECHA, sto.DT_FECHA),
                                            TP_RESULTADO = (sto.TP_RESULTADO == "FCM" ? "COMP" : "NORM")
                                        };
                                        lst.Add(aux2);
                                        aux2.IM_IMPORTE = (((sto.QT_RESULTADO - stoProx.QT_RESULTADO) * producto.AUX_VL_PRECIO_UNITARIO)) * aux2.QT_ESTADIA;

                                    }
                                }
                            }
                        }
                    }
                    //TimeSpan ts = DateTime.Now - tiempo2;
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0} - COMPONENTE: {1} - TIEMPO: {2}", DateTime.Now, comp, ts.TotalSeconds));
                    //tiempo2 = DateTime.Now;
                }

                //TimeSpan ts2 = DateTime.Now - tiempo;
                //System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);

                //DateTime tiempo3 = DateTime.Now;
                //TimeSpan ts3 = DateTime.Now - tiempo3;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                //System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_04(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                //DateTime tiempo = DateTime.Now;
                //DateTime tiempo2 = tiempo;
                //System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<T_PRODUTO> memoryProd = new List<T_PRODUTO>();
                //T_FACTURACION_EJECUCION ejecucion = facDB.T_FACTURACION_EJECUCION.FirstOrDefault(s => s.NU_EJECUCION == nu_ejecucion);
                T_FACTURACION_EJEC_EMPRESA ejeEmp = this._context.T_FACTURACION_EJEC_EMPRESA.FirstOrDefault(s => s.NU_EJECUCION == nu_ejecucion
                                                                                                        && s.CD_EMPRESA == cd_empresa
                                                                                                        && s.CD_PROCESO == cd_facturacion);

                List<T_LOG_FACTURACION_ALM_COR_04> logs = this._context.T_LOG_FACTURACION_ALM_COR_04.AsNoTracking().Where(s =>
                                                                        s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).OrderBy(s => s.DT_INGRESO).ToList();

                string[] componentes = logs.GroupBy(s => s.NU_COMPONENTE).Select(s => s.Key).ToArray();

                for (int i = 0; i < componentes.Length; i++)
                {
                    string comp = componentes[i];

                    string[] lotes = logs.Where(s => s.NU_COMPONENTE == comp).GroupBy(s => s.NU_IDENTIFICADOR).Select(s => s.Key).ToArray();
                    for (int j = 0; j < lotes.Length; j++)
                    {
                        string lot = lotes[j];

                        T_LOG_FACTURACION_ALM_COR_04 stockInicial = logs.Where(s => s.NU_IDENTIFICADOR == lot).OrderBy(s => s.DT_FECHA).FirstOrDefault();

                        // COMPLEMENTOS
                        T_LOG_FACTURACION_ALM_COR_04[] egresos = logs.Where(s => s.DT_RETIRO != null && s.NU_IDENTIFICADOR == lot && s.TP_RESULTADO == "FCM").ToArray();

                        for (int h = 0; h < egresos.Length; h++)
                        {
                            T_LOG_FACTURACION_ALM_COR_04 egreso = egresos[h];
                            T_PRODUTO producto = GetProduto(memoryProd, cd_empresa, egreso.CD_PRODUTO, cd_facturacion, comp);
                            lst.Add(new AUXCOR()
                            {
                                PS_BULTO = (decimal)producto.PS_BRUTO,
                                DS_ENVASE = producto.T_UNIDADE_MEDIDA.DS_UNIDADE_MEDIDA,
                                DS_PRODUTO = producto.DS_PRODUTO,
                                DT_INGRESO = egreso.DT_INGRESO,
                                DT_RETIRO = egreso.DT_RETIRO,
                                NU_COMPONENTE = comp,
                                IM_TARIFADIA = producto.AUX_VL_PRECIO_UNITARIO,
                                NU_AGENDA = egreso.NU_AGENDA,
                                NU_IDENTIFICADOR = egreso.NU_IDENTIFICADOR,
                                QT_CANTIDAD = egreso.PS_BRUTO / (decimal)producto.PS_BRUTO,
                                QT_ESTADIA = GetDays(stockInicial.DT_FECHA, (DateTime)egreso.DT_RETIRO) + (int)(egreso.QT_RESULTADO / egreso.PS_BRUTO),
                                IM_IMPORTE = (egreso.QT_RESULTADO * producto.AUX_VL_PRECIO_UNITARIO),
                                TP_RESULTADO = (egreso.TP_RESULTADO == "FCM" ? "COMP" : "NORM")
                            });
                        }

                        T_LOG_FACTURACION_ALM_COR_04 egresoFinal = egresos.OrderBy(s => s.DT_RETIRO).LastOrDefault();

                        // NORMAL
                        T_LOG_FACTURACION_ALM_COR_04[] stocksLot = logs.Where(s => s.NU_IDENTIFICADOR == lot && s.TP_RESULTADO == "FAC").OrderBy(s => s.DT_FECHA).ToArray();
                        for (int h = 0; h < stocksLot.Length; h++)
                        {
                            if (h == stocksLot.LongLength - 1)//ultimo (Stock al final de periodo)
                            {
                                T_LOG_FACTURACION_ALM_COR_04 sto = stocksLot[h];

                                bool addStock = true;
                                if (egresoFinal != null)
                                    if (egresoFinal.DT_RETIRO >= sto.DT_FECHA)
                                        addStock = false;
                                if (addStock)
                                {
                                    T_PRODUTO producto = GetProduto(memoryProd, cd_empresa, sto.CD_PRODUTO, cd_facturacion, comp);
                                    AUXCOR aux = new AUXCOR()
                                    {
                                        PS_BULTO = (decimal)producto.PS_BRUTO,
                                        DS_ENVASE = producto.T_UNIDADE_MEDIDA.DS_UNIDADE_MEDIDA,
                                        DS_PRODUTO = producto.DS_PRODUTO,
                                        NU_COMPONENTE = comp,
                                        DT_INGRESO = sto.DT_INGRESO,
                                        IM_TARIFADIA = producto.AUX_VL_PRECIO_UNITARIO,
                                        NU_AGENDA = sto.NU_AGENDA,
                                        NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR,
                                        QT_CANTIDAD = (sto.PS_BRUTO / (decimal)producto.PS_BRUTO),
                                        QT_ESTADIA = GetDays(stocksLot[0].DT_FECHA, sto.DT_FECHA),
                                        TP_RESULTADO = (sto.TP_RESULTADO == "FCM" ? "COMP" : "NORM")
                                    };
                                    aux.IM_IMPORTE = ((sto.QT_RESULTADO) * producto.AUX_VL_PRECIO_UNITARIO) * aux.QT_ESTADIA;
                                    lst.Add(aux);

                                    if (sto.DT_FECHA.Date < ejeEmp.DT_HASTA.Value.Date)
                                    {
                                        aux.DT_RETIRO = sto.DT_FECHA;
                                    }
                                }
                            }
                            else
                            {
                                T_LOG_FACTURACION_ALM_COR_04 sto = stocksLot[h];
                                T_LOG_FACTURACION_ALM_COR_04 stoProx = stocksLot[h + 1];

                                if (sto.QT_RESULTADO > stoProx.QT_RESULTADO)
                                {
                                    bool insert = !egresos.Any(s => s.DT_RETIRO.Value.Day == sto.DT_FECHA.Day
                                                                && s.DT_RETIRO.Value.Month == sto.DT_FECHA.Month
                                                                && s.DT_RETIRO.Value.Year == sto.DT_FECHA.Year);
                                    if (insert)
                                    {
                                        T_PRODUTO producto = GetProduto(memoryProd, cd_empresa, sto.CD_PRODUTO, cd_facturacion, comp);
                                        AUXCOR aux2 = new AUXCOR()
                                        {
                                            PS_BULTO = (decimal)producto.PS_BRUTO,
                                            DS_ENVASE = producto.T_UNIDADE_MEDIDA.DS_UNIDADE_MEDIDA,
                                            DS_PRODUTO = producto.DS_PRODUTO,
                                            NU_COMPONENTE = comp,
                                            DT_INGRESO = sto.DT_INGRESO,
                                            DT_RETIRO = sto.DT_FECHA,
                                            // IM_IMPORTE = ((sto.QT_RESULTADO - stoProx.QT_RESULTADO) * producto.AUX_VL_PRECIO_UNITARIO),
                                            IM_TARIFADIA = producto.AUX_VL_PRECIO_UNITARIO,
                                            NU_AGENDA = sto.NU_AGENDA,
                                            NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR,
                                            QT_CANTIDAD = (sto.QT_RESULTADO - stoProx.PS_BRUTO) / (decimal)producto.PS_BRUTO,
                                            QT_ESTADIA = GetDays(stocksLot[0].DT_FECHA, sto.DT_FECHA),
                                            TP_RESULTADO = (sto.TP_RESULTADO == "FCM" ? "COMP" : "NORM")
                                        };
                                        aux2.IM_IMPORTE = (((sto.QT_RESULTADO - stoProx.QT_RESULTADO) * producto.AUX_VL_PRECIO_UNITARIO)) * aux2.QT_ESTADIA;
                                        lst.Add(aux2);
                                    }
                                }
                            }
                        }
                    }
                    //TimeSpan ts = DateTime.Now - tiempo2;
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0} - COMPONENTE: {1} - TIEMPO: {2}", DateTime.Now, comp, ts.TotalSeconds));
                    //tiempo2 = DateTime.Now;
                }

                //TimeSpan ts2 = DateTime.Now - tiempo;
                //System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);

                //DateTime tiempo3 = DateTime.Now;
                //TimeSpan ts3 = DateTime.Now - tiempo3;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                //System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_05(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_COR_DET_05> logs = this._context.V_FACTURACION_COR_DET_05.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_FACTURACION_COR_DET_05 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }
                    lst.Add(new AUXCOR()
                    {

                        DS_ENVASE = resultado.NU_COMPONENTE,
                        //QT_UND_BULTO =  resultado.QT_UND_BULTO,
                        //CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        // NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_CIERRE,
                        //   DT_RETIRO = resultado.DT_RETIRO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        TP_RESULTADO = tp_resultado,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public virtual byte[] ExcelCOR_06(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<V_FOTO_STOCK_PALLET> memoryPallet = new List<V_FOTO_STOCK_PALLET>();
                List<V_FACTURACION_COR_DET_06> logs = this._context.V_FACTURACION_COR_DET_06.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                T_FACTURACION_EJECUCION eje = this._context.T_FACTURACION_EJECUCION.Where(s =>
                                                                s.NU_EJECUCION == nu_ejecucion).FirstOrDefault();
                List<V_FOTO_STOCK_PALLET> LISTPalletProd = this._context.V_FOTO_STOCK_PALLET.Where(x => x.DT_FOTO > eje.DT_DESDE && x.DT_FOTO < eje.DT_HASTA && x.CD_EMPRESA == cd_empresa).ToList();

                List<V_FOTO_STOCK_PALLET> ListPalletProdAgrupado = (List<V_FOTO_STOCK_PALLET>)LISTPalletProd.GroupBy(x => new { x.NU_IDENTIFICADOR, x.CD_PRODUTO, x.NU_PALLET, x.CD_EMPRESA, x.DS_PRODUTO })
            .Select(y => new V_FOTO_STOCK_PALLET()
            {
                CD_PRODUTO = y.Key.CD_PRODUTO,
                NU_IDENTIFICADOR = y.Key.NU_IDENTIFICADOR,
                NU_PALLET = y.Key.NU_PALLET,
                CD_EMPRESA = y.Key.CD_EMPRESA,
                DS_PRODUTO = y.Key.DS_PRODUTO
            }
            ).ToList();
                foreach (V_FACTURACION_COR_DET_06 resultado in logs)
                {
                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }
                    String DS_PRODUTO;
                    List<V_FOTO_STOCK_PALLET> PalletProd = GetFOTOSTOCK(ListPalletProdAgrupado, memoryPallet, resultado.NU_PALLET, resultado.DT_DESDE, resultado.DT_HASTA);
                    int cont = Cantidad_de_prod(PalletProd, resultado.NU_PALLET);
                    if (cont == 1)
                    {
                        DS_PRODUTO = PalletProd[0].DS_PRODUTO;
                    }
                    else
                    {
                        DS_PRODUTO = "VARIOS";
                    }


                    lst.Add(new AUXCOR()
                    {
                        NU_PALLET = resultado.NU_PALLET,
                        DS_ENVASE = "PALLET",
                        NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_FECHA,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        TP_RESULTADO = tp_resultado,
                        DS_PRODUTO = DS_PRODUTO,
                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_08(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<V_FOTO_STOCK_PALLET> memoryPallet = new List<V_FOTO_STOCK_PALLET>();
                List<V_FACTURACION_COR_DET_08> logs = this._context.V_FACTURACION_COR_DET_08.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                T_FACTURACION_EJECUCION eje = this._context.T_FACTURACION_EJECUCION.Where(s =>
                                                                s.NU_EJECUCION == nu_ejecucion).FirstOrDefault();
                List<V_FOTO_STOCK_PALLET> LISTPalletProd = this._context.V_FOTO_STOCK_PALLET.Where(x => x.DT_FOTO > eje.DT_DESDE && x.DT_FOTO < eje.DT_HASTA && x.CD_EMPRESA == cd_empresa).ToList();

                List<V_FOTO_STOCK_PALLET> ListPalletProdAgrupado = (List<V_FOTO_STOCK_PALLET>)LISTPalletProd.GroupBy(x => new { x.NU_IDENTIFICADOR, x.CD_PRODUTO, x.NU_PALLET, x.CD_EMPRESA, x.DS_PRODUTO })
            .Select(y => new V_FOTO_STOCK_PALLET()
            {
                CD_PRODUTO = y.Key.CD_PRODUTO,
                NU_IDENTIFICADOR = y.Key.NU_IDENTIFICADOR,
                NU_PALLET = y.Key.NU_PALLET,
                CD_EMPRESA = y.Key.CD_EMPRESA,
                DS_PRODUTO = y.Key.DS_PRODUTO
            }
            ).ToList();
                foreach (V_FACTURACION_COR_DET_08 resultado in logs)
                {
                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;
                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }
                    String DS_PRODUTO;
                    List<V_FOTO_STOCK_PALLET> PalletProd = GetFOTOSTOCK(ListPalletProdAgrupado, memoryPallet, resultado.NU_PALLET, resultado.DT_DESDE, resultado.DT_HASTA);
                    int cont = Cantidad_de_prod(PalletProd, resultado.NU_PALLET);
                    if (cont == 1)
                    {
                        DS_PRODUTO = PalletProd[0].DS_PRODUTO;
                    }
                    else
                    {
                        DS_PRODUTO = "VARIOS";
                    }

                    lst.Add(new AUXCOR()
                    {
                        NU_PALLET = resultado.NU_PALLET,
                        DS_ENVASE = "PALLET",
                        NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_FECHA,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        TP_RESULTADO = tp_resultado,
                        DS_PRODUTO = DS_PRODUTO,
                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_09(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_COR_DET_09> logs = this._context.V_FACTURACION_COR_DET_09.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_FACTURACION_COR_DET_09 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }



                    lst.Add(new AUXCOR()
                    {

                        DS_ENVASE = resultado.NU_COMPONENTE,
                        //QT_UND_BULTO = resultado.QT_UND_BULTO,
                        //CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        // NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_CIERRE,
                        //   DT_RETIRO = resultado.DT_RETIRO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        TP_RESULTADO = tp_resultado,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_10(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<T_FACTURACION_PALLET> memoryPallet = new List<T_FACTURACION_PALLET>();
                List<V_FACTURACION_COR_DET_10> logs = this._context.V_FACTURACION_COR_DET_10.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_FACTURACION_COR_DET_10 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.QT_IMPORTE != null)
                    {
                        precioemp = (decimal)resultado.QT_IMPORTE;

                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }



                    lst.Add(new AUXCOR()
                    {
                        NU_PALLET = resultado.NU_PALLET,
                        DS_ENVASE = "PALLET",
                        NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_INGRESO,
                        DT_RETIRO = resultado.DT_RETIRO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        TP_RESULTADO = tp_resultado,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public virtual byte[] ExcelCOR_11(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<T_FACTURACION_PALLET> memoryPallet = new List<T_FACTURACION_PALLET>();
                List<V_FACTURACION_COR_DET_11> logs = this._context.V_FACTURACION_COR_DET_11.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();

                var logs1 = logs.GroupBy(s => new { s.NU_EJECUCION, s.NU_COMPONENTE, s.CD_EMPRESA, s.CD_PROCESO, s.CD_PRODUTO, s.NU_IDENTIFICADOR, s.NU_AGENDA, s.DT_CIERRE, s.DS_PRODUTO, s.VL_PRECIO_UNITARIO })
                    .Select(s => new V_FACTURACION_COR_DET_11
                    {
                        CD_EMPRESA = s.Key.CD_EMPRESA,
                        CD_PROCESO = s.Key.CD_PROCESO,
                        NU_AGENDA = s.Key.NU_AGENDA,
                        NU_COMPONENTE = s.Key.NU_COMPONENTE,
                        DT_CIERRE = s.Key.DT_CIERRE,
                        DS_PRODUTO = s.Key.DS_PRODUTO,
                        VL_PRECIO_UNITARIO = s.Key.VL_PRECIO_UNITARIO,
                        // NU_CUENTA_CONTABLE = cuentaContable,
                        NU_IDENTIFICADOR = s.Key.NU_IDENTIFICADOR,
                        NU_EJECUCION = nu_ejecucion,
                        QT_RESULTADO = s.Sum(a => a.QT_RESULTADO)
                    }); ;
                List<V_FACTURACION_COR_DET_11> logsf = logs1.ToList();

                foreach (V_FACTURACION_COR_DET_11 resultado in logsf)
                {

                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;
                    lst.Add(new AUXCOR()
                    {


                        DS_ENVASE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_CIERRE,
                        //   DT_RETIRO = resultado.DT_RETIRO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        // TP_RESULTADO = tp_resultado,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_12(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<T_FACTURACION_PALLET> memoryPallet = new List<T_FACTURACION_PALLET>();
                List<V_FACTURACION_COR_DET_12> logs = this._context.V_FACTURACION_COR_DET_12.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_FACTURACION_COR_DET_12 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    lst.Add(new AUXCOR()
                    {
                        DS_ENVASE = resultado.NU_COMPONENTE,
                        DT_INGRESO = resultado.DT_FECHA,
                        //   DT_RETIRO = resultado.DT_RETIRO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_14(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_COR_DET_14> logs = this._context.V_FACTURACION_COR_DET_14.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_FACTURACION_COR_DET_14 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }
                    lst.Add(new AUXCOR()
                    {
                        PS_BULTO = resultado.PS_BRUTO ?? 1,
                        DS_ENVASE = resultado.NU_COMPONENTE,

                        //QT_UND_BULTO =  resultado.QT_UND_BULTO,
                        //CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        // NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_RETIRO = resultado.DT_FECHA,
                        //   DT_RETIRO = resultado.DT_RETIRO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = resultado.QT_RESULTADO / resultado.PS_BRUTO ?? 1,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        //  TP_RESULTADO = tp_resultado,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOR_21(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_FACTURACION_PRECIO_EMPRESA> memoryPrecio_Empresa = new List<V_FACTURACION_PRECIO_EMPRESA>();
                List<V_FOTO_STOCK_PALLET> memoryPallet = new List<V_FOTO_STOCK_PALLET>();
                List<V_FACTURACION_COR_DET_21> logs = this._context.V_FACTURACION_COR_DET_21.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();

                T_FACTURACION_EJECUCION eje = this._context.T_FACTURACION_EJECUCION.Where(s =>
                                                                                        s.NU_EJECUCION == nu_ejecucion).FirstOrDefault();
                List<V_FOTO_STOCK_PALLET> LISTPalletProd = this._context.V_FOTO_STOCK_PALLET.Where(x => x.DT_FOTO > eje.DT_DESDE && x.DT_FOTO < eje.DT_HASTA && x.CD_EMPRESA == cd_empresa).ToList();

                List<V_FOTO_STOCK_PALLET> ListPalletProdAgrupado = (List<V_FOTO_STOCK_PALLET>)LISTPalletProd.GroupBy(x => new { x.NU_IDENTIFICADOR, x.CD_PRODUTO, x.NU_PALLET, x.CD_EMPRESA, x.DS_PRODUTO })
            .Select(y => new V_FOTO_STOCK_PALLET()
            {
                CD_PRODUTO = y.Key.CD_PRODUTO,
                NU_IDENTIFICADOR = y.Key.NU_IDENTIFICADOR,
                NU_PALLET = y.Key.NU_PALLET,
                CD_EMPRESA = y.Key.CD_EMPRESA,
                DS_PRODUTO = y.Key.DS_PRODUTO
            }
            ).ToList();


                foreach (V_FACTURACION_COR_DET_21 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    if (resultado.TP_RESULTADO != null)
                    {
                        if (resultado.TP_RESULTADO.Equals("FCM"))
                        {
                            tp_resultado = "COMP";
                        }
                        else
                        {
                            tp_resultado = "NORM";
                        }
                    }
                    else
                    {
                        tp_resultado = "";
                    }
                    String DS_PRODUTO;
                    List<V_FOTO_STOCK_PALLET> PalletProd = GetFOTOSTOCK(ListPalletProdAgrupado, memoryPallet, resultado.NU_PALLET, resultado.DT_DESDE, resultado.DT_HASTA);
                    int cont = Cantidad_de_prod(PalletProd, resultado.NU_PALLET);
                    if (cont == 1)
                    {
                        DS_PRODUTO = PalletProd[0].DS_PRODUTO;
                    }
                    else
                    {
                        DS_PRODUTO = "VARIOS";
                    }


                    lst.Add(new AUXCOR()
                    {

                        NU_PALLET = resultado.NU_PALLET,
                        DS_ENVASE = "PALLET",
                        NU_COMPONENTE = resultado.NU_COMPONENTE,
                        DT_RETIRO = resultado.DT_FECHA,
                        IM_TARIFADIA = precioemp,
                        NU_AGENDA = resultado.NU_AGENDA,
                        QT_CANTIDAD = 1,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        TP_RESULTADO = tp_resultado,
                        DS_PRODUTO = DS_PRODUTO,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        #endregion

        #region ExcelCOD
       
        public virtual byte[] ExcelCOD_04(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_COD_04> logs = this._context.V_COD_04.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_COD_04 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    lst.Add(new AUXCOR()
                    {
                        NU_AGENDA = resultado.NU_AGENDA,
                        NU_ETIQUETA_LOTE = resultado.NU_ETIQUETA_LOTE,
                        CD_PRODUTO = resultado.CD_PRODUTO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_ENVASE = resultado.NU_COMPONENTE,
                        CD_EMPRESA = resultado.CD_EMPRESA,
                        CD_PROCESO = resultado.CD_PROCESO,
                        VL_PRECIO_UNITARIO = resultado.VL_PRECIO_UNITARIO,
                        QT_CANTIDAD = resultado.QT_RESULTADO ?? 0,
                        IM_IMPORTE = (resultado.QT_RESULTADO ?? 0 * precioemp),
                        IM_TARIFADIA = precioemp,
                        QT_TOTAL = resultado.QT_TOTAL,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        CD_UNID_EMB = resultado.CD_UNID_EMB,
                        DT_INGRESO = resultado.DT_CIERRE_AGENDA

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual byte[] ExcelCOD_05(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_COD_05> logs = this._context.V_COD_05.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_COD_05 resultado in logs)
                {
                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;
                    }
                    string tp_resultado;

                    lst.Add(new AUXCOR()
                    {
                        NU_FOTO = resultado.NU_FOTO,
                        CD_PRODUTO = resultado.CD_PRODUTO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        NU_COMPONENTE = resultado.NU_COMPONENTE,
                        CD_FAIXA = resultado.CD_FAIXA,
                        NU_PREDIO = resultado.NU_PREDIO,
                        CD_EMPRESA = resultado.CD_EMPRESA,
                        CD_PROCESO = resultado.CD_PROCESO,
                        VL_PRECIO_UNITARIO = resultado.VL_PRECIO_UNITARIO,
                        IM_TARIFADIA = precioemp,
                        QT_CANTIDAD = resultado.QT_RESULTADO ?? 0,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        QT_TOTAL = resultado.QT_TOTAL,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        QT_UND_BULTO = resultado.QT_UND_BULTO,
                        CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        CD_UNID_EMB = resultado.CD_UNID_EMB,
                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public virtual byte[] ExcelCOD_06(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {
                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_COD_06> logs = this._context.V_COD_06.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_COD_06 resultado in logs)
                {
                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    lst.Add(new AUXCOR()
                    {
                        NU_PEDIDO = resultado.NU_PEDIDO,
                        DS_ENVASE = resultado.NU_COMPONENTE,
                        CD_EMPRESA = resultado.CD_EMPRESA,
                        CD_PROCESO = resultado.CD_PROCESO,
                        VL_PRECIO_UNITARIO = resultado.VL_PRECIO_UNITARIO,
                        QT_CANTIDAD = resultado.QT_RESULTADO ?? 0,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        IM_TARIFADIA = precioemp,
                        QT_TOTAL = resultado.QT_TOTAL,
                        CD_CLIENTE = resultado.CD_CLIENTE,
                        DT_RETIRO = resultado.DT_EXPEDIDO,


                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public virtual byte[] ExcelCOD_07(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_COD_07> logs = this._context.V_COD_07.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_COD_07 resultado in logs)
                {
                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;


                    lst.Add(new AUXCOR()
                    {
                        NU_PEDIDO = resultado.NU_PEDIDO,
                        CD_EMPRESA = resultado.CD_EMPRESA,
                        CD_CLIENTE = resultado.CD_CLIENTE,
                        CD_PRODUTO = resultado.CD_PRODUTO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_ENVASE = resultado.NU_COMPONENTE,
                        VL_PRECIO_UNITARIO = resultado.VL_PRECIO_UNITARIO,
                        QT_PRODUTO = resultado.QT_PRODUTO,
                        QT_CANTIDAD = resultado.QT_RESULTADO ?? 0,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        IM_TARIFADIA = precioemp,
                        QT_TOTAL = resultado.QT_TOTAL,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        QT_UND_BULTO = resultado.QT_UND_BULTO,
                        CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        CD_UNID_EMB = resultado.CD_UNID_EMB,
                        DT_RETIRO = resultado.DT_EXPEDIDO,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public virtual byte[] ExcelCOD_08(int nu_ejecucion, int cd_empresa, string cd_facturacion, string nameFile, string[] ColumnsToRemove)
        {
            try
            {

                DateTime tiempo = DateTime.Now;
                DateTime tiempo2 = tiempo;

                System.Diagnostics.Debug.WriteLine(tiempo + " - ARRANCA PROCESO -");
                List<AUXCOR> lst = new List<AUXCOR>();
                List<V_COD_08> logs = this._context.V_COD_08.AsNoTracking().Where(s =>
                                                                            s.NU_EJECUCION == nu_ejecucion
                                                                        && s.CD_EMPRESA == cd_empresa
                                                                        ).ToList();
                foreach (V_COD_08 resultado in logs)
                {



                    decimal precioemp = 0;
                    if (resultado.VL_PRECIO_UNITARIO != null)
                    {
                        precioemp = (decimal)resultado.VL_PRECIO_UNITARIO;

                    }
                    string tp_resultado;

                    lst.Add(new AUXCOR()
                    {
                        NU_PEDIDO = resultado.NU_PEDIDO,
                        CD_CLIENTE = resultado.CD_CLIENTE,
                        CD_EMPRESA = resultado.CD_EMPRESA,
                        CD_PRODUTO = resultado.CD_PRODUTO,
                        NU_IDENTIFICADOR = resultado.NU_IDENTIFICADOR,
                        DS_ENVASE = resultado.NU_COMPONENTE,
                        CD_PROCESO = resultado.CD_PROCESO,
                        VL_PRECIO_UNITARIO = resultado.VL_PRECIO_UNITARIO,
                        QT_CANTIDAD = resultado.QT_RESULTADO ?? 0,
                        IM_IMPORTE = (resultado.QT_RESULTADO * precioemp),
                        IM_TARIFADIA = precioemp,
                        QT_TOTAL = resultado.QT_TOTAL,
                        DS_PRODUTO = resultado.DS_PRODUTO,
                        QT_UND_BULTO = resultado.QT_UND_BULTO,
                        CD_UNIDADE_MEDIDA = resultado.CD_UNIDADE_MEDIDA,
                        CD_UNID_EMB = resultado.CD_UNID_EMB,
                        DT_RETIRO = resultado.DT_EXPEDIDO,

                    });
                }

                TimeSpan ts2 = DateTime.Now - tiempo;
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - GENERAR EXCEL - TIEMPO: " + ts2.TotalSeconds);
                DateTime tiempo3 = DateTime.Now;
                byte[] bt = GetExcel<AUXCOR>(lst, nameFile, ColumnsToRemove);
                TimeSpan ts3 = DateTime.Now - tiempo3;

                System.Diagnostics.Debug.WriteLine(DateTime.Now + " - AFTER GENERAR EXCEL - TIEMPO: " + ts3.TotalSeconds);
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion

        #region ELIMINAR PROCESO EMPRESA
       
        public virtual void EliminarProcesoEmpresa(int NU_EJECUCION, int CD_EMPRESA, string CD_PROCESO)
        {
            try
            {
                this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">>>> INICIO CANCELACION --- EJ: {0} EMP: {1} PRO: {2}", NU_EJECUCION, CD_EMPRESA, CD_PROCESO));

                T_FACTURACION_EJECUCION ejecucion = this._context.T_FACTURACION_EJECUCION
                    .FirstOrDefault(s => s.NU_EJECUCION == NU_EJECUCION);

                if (!new short?[] { SituacionDb.EJECUCION_REALIZADA }.Contains(ejecucion.CD_SITUACAO)) //cambiar
                    throw new ValidationFailedException("General_Sec0_Error_Er128_SitEjecNoPermiteCancelar");

                T_FACTURACION_PROCESO proceso = this._context.T_FACTURACION_PROCESO
                    .FirstOrDefault(s => s.CD_PROCESO.ToUpper().Trim() == CD_PROCESO.ToUpper().Trim());

                List<T_FACTURACION_RESULTADO> resultados = this._context.T_FACTURACION_RESULTADO
                    .Where(s => s.NU_EJECUCION == ejecucion.NU_EJECUCION
                        && s.CD_EMPRESA == CD_EMPRESA
                        && s.CD_FACTURACION.ToUpper().Trim() == proceso.CD_FACTURACION.ToUpper().Trim())
                    .ToList();

                if (resultados.Any(s => s.NU_FACTURA != null && s.NU_TICKET_INTERFAZ_FACTURACION != null))
                    throw new ValidationFailedException("General_Sec0_Error_Er129_ResultadosEnviadosAFacturar");

                List<T_FACTURACION_EJEC_EMPRESA> lstEjecEmpresa = this._context.T_FACTURACION_EJEC_EMPRESA
                    .Where(s => s.CD_EMPRESA == CD_EMPRESA
                        && s.CD_PROCESO.ToUpper().Trim() == proceso.CD_PROCESO.ToUpper().Trim()
                        && s.ID_ESTADO == "HAB")
                    .OrderBy(s => s.NU_EJECUCION)
                    .ToList();

                T_FACTURACION_EJEC_EMPRESA ejeEmpresaActual = lstEjecEmpresa.FirstOrDefault(s => s.NU_EJECUCION == ejecucion.NU_EJECUCION);
                this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> EJECUCION EMPRESA -- EJ: {0} EMP: {1} PRO: {2} DESDE: {3} HASTA: {4} EST: {5}"
                    , ejeEmpresaActual.NU_EJECUCION, ejeEmpresaActual.CD_EMPRESA, ejeEmpresaActual.CD_PROCESO, ejeEmpresaActual.DT_DESDE, ejeEmpresaActual.DT_HASTA, ejeEmpresaActual.ID_ESTADO));

                int count = lstEjecEmpresa.Count - 1;
                int posActual = lstEjecEmpresa.IndexOf(ejeEmpresaActual);

                if (count != posActual)
                    throw new ValidationFailedException("General_Sec0_Error_Er130_ExistenEjecucionesProEmpresa");

                // --- Modifico datos ---

                //Rechazo los resultados
                foreach (T_FACTURACION_RESULTADO res in resultados)
                {
                    this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">>> RECHAZO RESULTADO -- EJ: {0} EMP: {1} CD: {2} COMP: {3} RES: {4} SIT: {5} ", res.NU_EJECUCION, res.CD_EMPRESA, res.CD_FACTURACION, res.NU_COMPONENTE, res.QT_RESULTADO, res.CD_SITUACAO));
                    res.CD_SITUACAO = SituacionDb.CALCULO_RECHAZADO;
                }
                // Cancelo ejecucion empresa
                ejeEmpresaActual.ID_ESTADO = FacturacionDb.ESTADO_CAN;

                // Actualizo empresa proceso
                T_FACTURACION_EMPRESA_PROCESO empProceso = this._context.T_FACTURACION_EMPRESA_PROCESO
                    .FirstOrDefault(s => s.CD_EMPRESA == CD_EMPRESA
                    && s.CD_PROCESO.ToUpper().Trim() == proceso.CD_PROCESO.ToUpper().Trim());

                this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> EMPRESA PROCESO ORI -- EMP: {0} PRO: {1} HR ULT: {2} TP ULT: {3}", empProceso.CD_EMPRESA, empProceso.CD_PROCESO, empProceso.HR_ULTIMO_PROCESO, empProceso.TP_ULTIMO_PROCESO));
                DateTime? hrUltimaEjecucion = null;
                string tpUltimoProceso = string.Empty;

                if (count >= 1)
                {
                    T_FACTURACION_EJEC_EMPRESA ejeEmpAnterior = lstEjecEmpresa.ElementAt(posActual - 1);

                    hrUltimaEjecucion = ejeEmpAnterior.DT_HASTA;

                    T_FACTURACION_EJECUCION ejecucionAnterior = this._context.T_FACTURACION_EJECUCION
                        .FirstOrDefault(s => s.NU_EJECUCION == ejeEmpAnterior.NU_EJECUCION);

                    tpUltimoProceso = ejecucionAnterior.FL_EJEC_POR_HORA;
                }
                empProceso.HR_ULTIMO_PROCESO = hrUltimaEjecucion;
                empProceso.TP_ULTIMO_PROCESO = tpUltimoProceso;

                // --- Ajuste de CORS Parciales ---
                switch (proceso.CD_FACTURACION)
                {
                    case "COR_02":
                        List<T_FACTURACION_PALLET_DET> lstLogCOR_02 = this._context.T_FACTURACION_PALLET_DET.Where(s => s.NU_EJECUCION_FACTURACION == NU_EJECUCION && s.CD_EMPRESA == CD_EMPRESA && s.CD_FACTURACION == proceso.CD_FACTURACION && s.ID_ESTADO == "HAB").ToList();
                        foreach (T_FACTURACION_PALLET_DET log in lstLogCOR_02)
                        {
                            this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> LOG COR_02 -- EJ: {0} EMP: {1} COD: {2} EST: {3} COMP: {4} RES: {5}", log.NU_EJECUCION_FACTURACION, log.CD_EMPRESA, log.CD_FACTURACION, log.ID_ESTADO, log.NU_COMPONENTE, log.QT_RESULTADO));
                            log.ID_ESTADO = FacturacionDb.ESTADO_CAN;
                        }
                        break;
                    case "COR_03":
                        List<T_LOG_FACTURACION_ALM_BULTO> lstLogCOR_03 = this._context.T_LOG_FACTURACION_ALM_BULTO.Where(s => s.NU_EJECUCION == NU_EJECUCION && s.CD_EMPRESA == CD_EMPRESA && s.CD_PROCESO == proceso.CD_PROCESO && s.ID_ESTADO == "HAB").ToList(); ;
                        foreach (T_LOG_FACTURACION_ALM_BULTO log in lstLogCOR_03)
                        {
                            this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> LOG COR_03 -- EJ: {0} EMP: {1} PRO: {2} COMP: {3} PROD: {4} LOTE: {5} FECHA: {6} RES: {7} TP RES {8} EST:{9}", log.NU_EJECUCION, log.CD_EMPRESA, log.CD_PROCESO, log.NU_COMPONENTE, log.CD_PRODUTO, log.NU_IDENTIFICADOR, log.DT_FECHA, log.QT_RESULTADO, log.TP_RESULTADO, log.ID_ESTADO));
                            log.ID_ESTADO = FacturacionDb.ESTADO_CAN;
                        }
                        break;
                    case "COR_04":
                        List<T_LOG_FACTURACION_ALM_COR_04> lstLogCOR_04 = this._context.T_LOG_FACTURACION_ALM_COR_04.Where(s => s.NU_EJECUCION == NU_EJECUCION && s.CD_EMPRESA == CD_EMPRESA && s.CD_PROCESO == proceso.CD_PROCESO && s.ID_ESTADO == "HAB").ToList(); ;
                        foreach (T_LOG_FACTURACION_ALM_COR_04 log in lstLogCOR_04)
                        {
                            this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> LOG COR_04 -- EJ: {0} EMP: {1} PRO: {2} COMP: {3} PROD: {4} LOTE: {5} FECHA: {6} RES: {7} TP RES {8} EST:{9}", log.NU_EJECUCION, log.CD_EMPRESA, log.CD_PROCESO, log.NU_COMPONENTE, log.CD_PRODUTO, log.NU_IDENTIFICADOR, log.DT_FECHA, log.QT_RESULTADO, log.TP_RESULTADO, log.ID_ESTADO));
                            log.ID_ESTADO = FacturacionDb.ESTADO_CAN;
                        }
                        break;
                    case "COR_10":
                        List<T_LOG_FACTURACION_COR_10> lstLogCOR_10 = this._context.T_LOG_FACTURACION_COR_10.Where(s => s.NU_EJECUCION == NU_EJECUCION && s.CD_EMPRESA == CD_EMPRESA && s.CD_PROCESO == proceso.CD_PROCESO && s.ID_ESTADO == "HAB").ToList(); ;
                        foreach (T_LOG_FACTURACION_COR_10 log in lstLogCOR_10)
                        {
                            this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> LOG COR_10 -- EJ: {0} EMP: {1} PRO: {2} COMP: {3} PALLET: {4} FECHA: {5} RES: {6} TP RES {7} EST:{8}", log.NU_EJECUCION, log.CD_EMPRESA, log.CD_PROCESO, log.NU_COMPONENTE, log.NU_PALLET, log.DT_FECHA, log.QT_RESULTADO, log.TP_RESULTADO, log.ID_ESTADO));
                            log.ID_ESTADO = FacturacionDb.ESTADO_CAN;
                        }
                        break;
                    case "COR_11":
                        List<T_LOG_FACTURACION_MOV_EG_BULTO> lstLogCOR_11 = this._context.T_LOG_FACTURACION_MOV_EG_BULTO.Where(s => s.NU_EJECUCION == NU_EJECUCION && s.CD_EMPRESA == CD_EMPRESA && s.CD_PROCESO == proceso.CD_PROCESO && s.ID_ESTADO == "HAB").ToList(); ;
                        foreach (T_LOG_FACTURACION_MOV_EG_BULTO log in lstLogCOR_11)
                        {
                            this._logger.Info("FAC_CANCELAR_EJEC ", string.Format(">> LOG COR_11 -- EJ: {0} EMP: {1} PRO: {2} COMP: {3} PROD: {4} LOTE: {5} FECHA: {6} RES: {7} TP RES {8} EST:{9}", log.NU_EJECUCION, log.CD_EMPRESA, log.CD_PROCESO, log.NU_COMPONENTE, log.CD_PRODUTO, log.NU_IDENTIFICADOR, log.DT_FECHA, log.QT_RESULTADO, log.TP_RESULTADO, log.ID_ESTADO));
                            log.ID_ESTADO = FacturacionDb.ESTADO_CAN;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        #endregion

        #region Dapper

        public virtual bool AnyDestruccionPalletSinProductos(DbConnection connection)
        {
            string sql =
                $@"SELECT 1 
                FROM T_FACTURACION_PALLET fp 
                WHERE EXISTS ( SELECT 1 FROM V_FACTURACION_PALLET_A_DESTRU fpd WHERE fp.NU_ETIQUETA_LOTE = fpd.NU_ETIQUETA_LOTE )
                AND ( SELECT COUNT(*) FROM T_FACTURACION_EJECUCION  WHERE CD_SITUACAO = {SituacionDb.EJECUCION_PENDIENTE} ) > 0";

            var result = _dapper.Query<string>(connection, sql, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result);
        }

        public virtual void DestruccionPalletSinProductos(DbConnection connection, long nuTransaccion)
        {
            string sql =
                $@"UPDATE T_FACTURACION_PALLET fp
                SET (DT_DESTRUCCION_PALLET, FL_DEST_SIN_PRODUCTO, NU_TRANSACCION) = 
                (SELECT fpd.DT_ALMACENAMIENTO, 'S', :nuTransaccion 
                FROM V_FACTURACION_PALLET_A_DESTRU fpd
                WHERE fp.NU_ETIQUETA_LOTE = fpd.NU_ETIQUETA_LOTE)
                WHERE EXISTS ( SELECT 1 FROM V_FACTURACION_PALLET_A_DESTRU fpd WHERE fp.NU_ETIQUETA_LOTE = fpd.NU_ETIQUETA_LOTE )
                AND ( SELECT COUNT(*) FROM T_FACTURACION_EJECUCION  WHERE CD_SITUACAO = {SituacionDb.EJECUCION_PENDIENTE} ) > 0";

            _dapper.Execute(connection, sql, new { nuTransaccion = nuTransaccion });
        }

        public virtual List<FacturacionEjecEmpProceso> GetEjecucionesPendientes(DbConnection connection, int? nroFactEjecucion = null)
        {
            string sql = GetSqlSelect(nroFactEjecucion);
            return _dapper.Query<FacturacionEjecEmpProceso>(connection, sql, commandType: CommandType.Text).ToList();
        }

        public static string GetSqlSelect(int? nroFactEjecucion = null)
        {
            var sql = $@"SELECT 
                        CD_EMPRESA as CodigoEmpresa,
						CD_FACTURACION as CodigoFacturacion,
						CD_FUNC_ANULACION as CodigoFuncAnulacion,
						CD_FUNC_APROBACION as CodigoFuncAprobacion,
						CD_FUNC_EJECUCION as CodigoFuncEjecucion,
						CD_FUNC_ENVIADA as CodigoFuncEnviada,
						CD_FUNC_PROGRAMACION as CodigoFuncProgramacion,
						CD_PROCESO as CodigoProceso,
						CD_SITUACAO_EJEC_EMP as SituacionEjecEmp,
						CD_SITUACAO_EJECUCION as SituacionEjecucion,
						CD_SITUACAO_ERROR_PROCESO as SituacionErrorProceso,
						DS_PROCESO as DescripcionProceso,
						DT_ADDROW as FechaIngresado,
						DT_ANULACION as FechaAnulacion,
						DT_APROBACION as FechaAprobacion,
						DT_CORTE_QUINCENA as CorteQuincena,
						DT_DESDE_EJEC_EMP as FechaDesdeEjecEmp,
						DT_DESDE_EJECUCION as FechaDesdeEjecucion,
						DT_EJECUCION as FechaEjecucion,
						DT_ENVIADA as FechaEnviada,
						DT_HASTA_EJEC_EMP as FechaHastaEjecEmp,
						DT_HASTA_EJECUCION as FechaHastaEjecucion,
						DT_PROGRAMACION as FechaProgramacion,
						FL_EJEC_POR_HORA_EJEC as EjecucionPorHora,
						FL_EJEC_POR_HORA_EJEC_EMP as EjecucionPorHoraProceso,
						ID_ESTADO as Estado,
						NM_EJECUCION as Nombre,
						NM_PROCEDIMIENTO as NombreProcedimiento,
						NU_COMPONENTE as NumeroComponente,
						NU_CUENTA_CONTABLE as NumeroCuentaContable,
						NU_EJECUCION as NumeroEjecucion,
						TP_PROCESO as TipoProceso
                    FROM V_FACT_EJEC_EMP_PROCESO";

            if (nroFactEjecucion != null)
                sql += $@" WHERE NU_EJECUCION = {nroFactEjecucion}";

            return sql;
        }

        public virtual FacturacionEmpresaProceso GetFactEmpresaProceso(DbConnection connection, int empresa, string cdProceso)
        {
            string sql = @"SELECT 
                            CD_EMPRESA as CodigoEmpresa,
                            CD_PROCESO as CodigoProceso,
                            CD_SITUACAO_ERROR as SituacionError,
                            DT_UPDROW as FechaModificacion,
                            HR_ULTIMO_PROCESO as UltimoProceso,
                            QT_RESULTADO as Resultado,
                            TP_ULTIMO_PROCESO as TipoUltimoProceso
                        FROM T_FACTURACION_EMPRESA_PROCESO 
                        WHERE CD_EMPRESA = :empresa 
                        AND CD_PROCESO = :cdProceso";

            return _dapper.Query<FacturacionEmpresaProceso>(connection, sql, new { empresa = empresa, cdProceso = cdProceso }, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void ProcesarProgramables(int numeroEjecucion, int empresa, string cdProceso, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, QT_RESULTADO, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, NU_TRANSACCION)
            SELECT :NumeroEjecucion, fep.CD_EMPRESA, fep.CD_PROCESO, fp.CD_FACTURACION, fcc.NU_COMPONENTE, COALESCE(fcc.NU_CUENTA_CONTABLE,0), COALESCE(fep.QT_RESULTADO,0), 'UND', 301, :fechaModificacion, :fechaModificacion, :nuTransaccion
            FROM T_FACTURACION_EMPRESA_PROCESO fep 
            LEFT JOIN T_FACTURACION_PROCESO fp on fep.CD_PROCESO = fp.CD_PROCESO
            LEFT JOIN V_FACTURAC_CODIGO_COMP_WFAC250 fcc on fp.CD_FACTURACION = fcc.CD_FACTURACION
            WHERE fep.CD_EMPRESA = :empresa AND fep.CD_PROCESO = :cdProceso";

            _dapper.Execute(connection, sql, new
            {
                NumeroEjecucion = numeroEjecucion,
                empresa = empresa,
                cdProceso = cdProceso,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
            }, transaction: tran);
        }

        public virtual string GetCuentaContableResultado(DbConnection connection, DbTransaction tran, string cdFacturacion, string nuComponente, string nuCuentaContable)
        {
            string sql = $@"SELECT NU_CUENTA_CONTABLE FROM T_FACTURACION_CODIGO_COMPONEN WHERE CD_FACTURACION=:cdFacturacion AND NU_COMPONENTE= :nuComponente";
            var result = _dapper.Query<string>(connection, sql, new { cdFacturacion = cdFacturacion, nuComponente = nuComponente }, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? result : nuCuentaContable;
        }

        public virtual void SetearResultadoVacio(FacturacionEjecEmpProceso proceso, string nuCuentaContable, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                nuComponente = proceso.NumeroComponente,
                cdProceso = proceso.CodigoProceso,
                nuCuentaContable = nuCuentaContable,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
            };

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, QT_RESULTADO, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, NU_TRANSACCION)
            VALUES(:NumeroEjecucion, :empresa, :cdProceso, :cdFacturacion, :nuComponente, :nuCuentaContable, 0, 'UND', 301, :fechaModificacion, :fechaModificacion, :nuTransaccion )";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }

        #region WST001
        
        public virtual void ProcesarWST001(FacturacionEjecEmpProceso proceso, string nuCuentaContable, DateTime? fechaDesde, DateTime fechaHasta, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                cdProceso = proceso.CodigoProceso,
                cdFacturacion = proceso.CodigoFacturacion,
                nuComponente = proceso.NumeroComponente,
                nuCuentaContable = nuCuentaContable,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = proceso.CodigoEmpresa,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ag.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, QT_RESULTADO, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, NU_TRANSACCION)
            SELECT :NumeroEjecucion, ag.CD_EMPRESA, :cdProceso, :cdFacturacion, COALESCE(p.nd_facturacion_comp1,:nuComponente), :nuCuentaContable, SUM(dtl.QT_PRODUTO_RECIBIDO), COALESCE(MAX(p.cd_unidade_medida),'UND'), 301, :fechaModificacion, :fechaModificacion, :nuTransaccion
            FROM T_DET_ETIQUETA_LOTE dtl 
            INNER JOIN T_ETIQUETA_LOTE el ON dtl.NU_ETIQUETA_LOTE = el.NU_ETIQUETA_LOTE
            INNER JOIN T_AGENDA ag ON el.NU_AGENDA = ag.NU_AGENDA
            INNER JOIN T_PRODUTO p ON dtl.CD_PRODUTO = p.CD_PRODUTO AND dtl.CD_EMPRESA = p.CD_EMPRESA
            WHERE ag.CD_SITUACAO = 4 AND ag.CD_EMPRESA = :empresa  AND ag.DT_CIERRE <= :fechaHasta {sqlAux}
            GROUP BY ag.CD_EMPRESA, COALESCE(p.nd_facturacion_comp1,:nuComponente)";
            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
        
        public virtual bool ExisteResultadoWST001(DbConnection connection, DbTransaction tran, int empresa, DateTime? fechaDesde, DateTime fechaHasta)
        {
            var model = new
            {
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = empresa
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ag.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"SELECT 1 
            FROM T_DET_ETIQUETA_LOTE dtl 
            INNER JOIN T_ETIQUETA_LOTE el ON dtl.NU_ETIQUETA_LOTE= el.NU_ETIQUETA_LOTE
            INNER JOIN T_AGENDA ag ON el.NU_AGENDA = ag.NU_AGENDA
            INNER JOIN T_PRODUTO p ON dtl.CD_PRODUTO = p.CD_PRODUTO AND dtl.CD_EMPRESA = p.CD_EMPRESA
            WHERE ag.CD_SITUACAO = 4 AND ag.CD_EMPRESA = :empresa  AND ag.DT_CIERRE <= :fechaHasta {sqlAux}";

            var result = _dapper.Query<string>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? true : false;
        }
        
        public virtual void LogProcesarWST001(FacturacionEjecEmpProceso proceso, string cuentaContable, DateTime? fechaDesde, DateTime fechaHasta, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                cdProceso = proceso.CodigoProceso,
                nuComponente = proceso.NumeroComponente,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ag.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO_DETALLE (NU_EJECUCION, CD_EMPRESA, CD_FACTURACION, CD_PROCESO, DT_GENERICO, VL_SERIALIZADO, NU_COMPONENTE, QT_RESULTADO, DT_ADDROW)
            SELECT :NumeroEjecucion, ag.CD_EMPRESA, :cdFacturacion, :cdProceso, ag.DT_CIERRE, 'Agenda ' || ag.NU_AGENDA || '(' || count(dtl.cd_produto)||' Producto/s)',
            COALESCE(p.nd_facturacion_comp1,:nuComponente), SUM(dtl.QT_PRODUTO_RECIBIDO), :fechaModificacion 
            FROM T_DET_ETIQUETA_LOTE dtl 
            INNER JOIN T_ETIQUETA_LOTE el ON dtl.NU_ETIQUETA_LOTE= el.NU_ETIQUETA_LOTE
            INNER JOIN T_AGENDA ag ON el.NU_AGENDA = ag.NU_AGENDA
            INNER JOIN T_PRODUTO p ON dtl.CD_PRODUTO=p.CD_PRODUTO AND dtl.CD_EMPRESA=p.CD_EMPRESA
            WHERE ag.CD_SITUACAO = 4 AND ag.CD_EMPRESA = :empresa  AND ag.DT_CIERRE <= :fechaHasta {sqlAux}
            GROUP BY ag.CD_EMPRESA, ag.DT_CIERRE, ag.NU_AGENDA, COALESCE(p.nd_facturacion_comp1,:nuComponente)
            ORDER BY ag.DT_CIERRE";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
        
        #endregion

        #region WST002
        public virtual void ProcesarWST002(FacturacionEjecEmpProceso proceso, string nuCuentaContable, DateTime? fechaDesde, DateTime fechaHasta, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var sql = $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, QT_RESULTADO, NU_TRANSACCION)
            SELECT :nuEjecucion, 
                S2.CD_EMPRESA, 
                :cdProceso, 
                :cdFacturacion, 
                S2.NU_COMPONENTE, 
                :nuCuentaContable, 
                COALESCE(MAX(S2.CD_UNIDADE_MEDIDA),'UND'), 
                301, 
                :fechaActual, 
                :fechaActual, 
                SUM(S2.QT_ESTOQUE) / :totalDias,
                :nuTransaccion 
            FROM (
                SELECT 
                    S1.CD_EMPRESA, 
                    S1.NU_COMPONENTE, 
                    S1.DT_ADDROW, 
                    SUM(S1.QT_ESTOQUE) AS QT_ESTOQUE,
                    COALESCE(MAX(S1.CD_UNIDADE_MEDIDA),'UND') AS CD_UNIDADE_MEDIDA
                FROM ({GetSqlStockDiarioProductosPorFotoStock(fechaDesde)}) S1
                GROUP BY S1.CD_EMPRESA, S1.NU_COMPONENTE, S1.DT_ADDROW
            ) S2
            GROUP BY S2.CD_EMPRESA, S2.NU_COMPONENTE";

            var cantFotoStock = GetCountFotoStock(connection, tran, proceso.CodigoEmpresa, fechaDesde, fechaHasta);

            if (fechaDesde == null)
            {
                fechaDesde = GetMinFechaFotoStock(connection, tran, proceso.CodigoEmpresa);

                if (fechaDesde != null && proceso.EjecucionPorHora == "N")
                {
                    var fechaDesdeaux = ((DateTime)fechaDesde).AddDays(-1);
                    fechaDesde = new DateTime(fechaDesdeaux.Year, fechaDesdeaux.Month, fechaDesdeaux.Day, 23, 59, 59);
                }
            }

            if (fechaDesde == null)
                fechaDesde = fechaHasta.Date;

            var totalDias = Math.Max(1, (fechaHasta - fechaDesde.Value).Days);

            if (cantFotoStock != totalDias)
                throw new ValidationFailedException("Cantidad de fotos de stock incorrecta");

            var model = new
            {
                nuEjecucion = proceso.NumeroEjecucion,
                cdProceso = proceso.CodigoProceso,
                cdFacturacion = proceso.CodigoFacturacion,
                nuComponente = proceso.NumeroComponente,
                nuCuentaContable = nuCuentaContable,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = proceso.CodigoEmpresa,
                totalDias = totalDias,
                fechaActual = DateTime.Now,
                nuTransaccion = nuTransaccion,
            };

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }

        protected virtual DateTime? GetMinFechaFotoStock(DbConnection connection, DbTransaction tran, int empresa)
        {
            var sql = $@"SELECT MIN(DT_ADDROW) FROM T_FOTO_STOCK_UBIC WHERE CD_EMPRESA = :empresa";
            var model = new
            {
                empresa = empresa
            };

            return _dapper.Query<DateTime?>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)
                .FirstOrDefault();
        }

        protected virtual long GetCountFotoStock(DbConnection connection, DbTransaction tran, int empresa, DateTime? fechaDesde, DateTime fechaHasta)
        {
            var sqlAux = string.Empty;

            if (fechaDesde != null)
                sqlAux = $" AND DT_ADDROW >= :fechaDesde ";

            var sql = $@"SELECT COUNT(*)
                FROM (
                    SELECT trunc(DT_ADDROW) AS DT_ADDROW
                    FROM T_FOTO_STOCK_UBIC
                    WHERE CD_EMPRESA = :empresa AND DT_ADDROW <= :fechaHasta {sqlAux}
                    GROUP BY trunc(DT_ADDROW)
            ) T";

            var model = new
            {
                empresa = empresa,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta
            };

            return _dapper.Query<long>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)
                .FirstOrDefault();
        }

        public virtual bool ExisteResultadoWST002(DbConnection connection, DbTransaction tran, int empresa, DateTime? fechaDesde, DateTime fechaHasta)
        {
            var model = new
            {
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = empresa
            };

            var sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND fsu.DT_ADDROW >= :fechaDesde ";

            var sql =
            $@"SELECT 1 FROM T_FOTO_STOCK_UBIC fsu  
            INNER JOIN T_ENDERECO_ESTOQUE ee ON fsu.CD_ENDERECO = ee.CD_ENDERECO 
            INNER JOIN T_PRODUTO p ON fsu.CD_PRODUTO = p.CD_PRODUTO AND fsu.CD_EMPRESA = p.CD_EMPRESA
            WHERE fsu.CD_EMPRESA = :empresa  AND fsu.DT_ADDROW <= :fechaHasta {sqlAux}";

            var result = _dapper.Query<string>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? true : false;
        }
        
        public virtual void LogProcesarWST002(FacturacionEjecEmpProceso proceso, string cuentaContable, DateTime? fechaDesde, DateTime fechaHasta, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                nuEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                cdProceso = proceso.CodigoProceso,
                nuComponente = proceso.NumeroComponente,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                fechaActual = DateTime.Now
            };

            var sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO_DETALLE (NU_EJECUCION, CD_EMPRESA, CD_FACTURACION, CD_PROCESO, DT_GENERICO, VL_SERIALIZADO, NU_COMPONENTE, QT_RESULTADO, DT_ADDROW) 
            SELECT S1.NU_EJECUCION, 
                S1.CD_EMPRESA, 
                S1.CD_FACTURACION, 
                S1.CD_PROCESO, 
                S1.DT_ADDROW, 
                S1.VL_SERIALIZADO, 
                S1.NU_COMPONENTE, 
                S1.QT_ESTOQUE, 
                :fechaActual
            FROM ({GetSqlStockDiarioProductosPorFotoStock(fechaDesde)}) S1
            ORDER BY S1.DT_ADDROW";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }

        protected virtual string GetSqlStockDiarioProductosPorFotoStock(DateTime? fechaDesde)
        {
            var sqlAux = string.Empty;

            if (fechaDesde != null)
                sqlAux = $" AND fsu.DT_ADDROW >= :fechaDesde ";

            return $@"SELECT :nuEjecucion AS NU_EJECUCION, 
                fsu.CD_EMPRESA, 
                :cdFacturacion AS CD_FACTURACION, 
                :cdProceso AS CD_PROCESO, 
                trunc(fsu.DT_ADDROW) AS DT_ADDROW, 
                'Producto ' || fsu.CD_PRODUTO || ' - ' || p.ds_produto AS VL_SERIALIZADO,
                COALESCE(p.ND_FACTURACION_COMP1, :nuComponente) AS NU_COMPONENTE, 
                SUM(fsu.QT_ESTOQUE) AS QT_ESTOQUE, 
                COALESCE(MAX(p.CD_UNIDADE_MEDIDA),'UND') AS CD_UNIDADE_MEDIDA
            FROM T_FOTO_STOCK_UBIC fsu  
            INNER JOIN T_ENDERECO_ESTOQUE ee ON fsu.CD_ENDERECO = ee.CD_ENDERECO 
            INNER JOIN T_PRODUTO p ON fsu.CD_PRODUTO = p.CD_PRODUTO AND fsu.CD_EMPRESA = p.CD_EMPRESA
            WHERE fsu.CD_EMPRESA = :empresa  AND fsu.DT_ADDROW <= :fechaHasta {sqlAux}
            GROUP BY fsu.CD_EMPRESA, trunc(fsu.DT_ADDROW), fsu.CD_PRODUTO, p.DS_PRODUTO, COALESCE(p.ND_FACTURACION_COMP1, :nuComponente)";
        }

        #endregion

        #region WST003
        
        public virtual void ProcesarWST003(FacturacionEjecEmpProceso proceso, string nuCuentaContable, DateTime? fechaDesde, DateTime fechaHasta, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                cdProceso = proceso.CodigoProceso,
                cdFacturacion = proceso.CodigoFacturacion,
                nuComponente = proceso.NumeroComponente,
                nuCuentaContable = nuCuentaContable,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = proceso.CodigoEmpresa,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, QT_RESULTADO, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, NU_TRANSACCION)
            SELECT :NumeroEjecucion, :empresa, :cdProceso, :cdFacturacion, :nuComponente, :nuCuentaContable, count(*), 'UND', 301, :fechaModificacion, :fechaModificacion, :nuTransaccion
            FROM (SELECT dpe.CD_CAMION, dpe.NU_PEDIDO, dpe.CD_CLIENTE, dpe.CD_EMPRESA 
                  FROM T_CAMION ca
                  INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA  
                  WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
                  GROUP BY dpe.CD_CAMION, dpe.NU_PEDIDO, dpe.CD_CLIENTE, dpe.CD_EMPRESA) d";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
        
        public virtual bool ExisteResultadoWST003(DbConnection connection, DbTransaction tran, int empresa, DateTime? fechaDesde, DateTime fechaHasta)
        {
            var model = new
            {
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = empresa
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"SELECT 1 FROM 
            (SELECT dpe.CD_CAMION, dpe.NU_PEDIDO, dpe.CD_CLIENTE, dpe.CD_EMPRESA 
            FROM T_CAMION ca
            INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA  
            WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
            GROUP BY dpe.CD_CAMION, dpe.NU_PEDIDO, dpe.CD_CLIENTE, dpe.CD_EMPRESA) d";

            var result = _dapper.Query<string>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? true : false;
        }
       
        public virtual void LogProcesarWST003(FacturacionEjecEmpProceso proceso, string cuentaContable, DateTime? fechaDesde, DateTime fechaHasta, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                cdProceso = proceso.CodigoProceso,
                nuComponente = proceso.NumeroComponente,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO_DETALLE (NU_EJECUCION, CD_EMPRESA, CD_FACTURACION, CD_PROCESO, DT_GENERICO, VL_SERIALIZADO, NU_COMPONENTE, QT_RESULTADO, DT_ADDROW)
            SELECT :NumeroEjecucion, :empresa, :cdFacturacion, :cdProceso, d.DT_CIERRE, 'Camión ' || d.CD_CAMION, :nuComponente, COUNT(d.NU_PEDIDO), :fechaModificacion 
            FROM (SELECT dpe.CD_CAMION, ca.DT_CIERRE, dpe.NU_PEDIDO, dpe.CD_CLIENTE, dpe.CD_EMPRESA 
            FROM T_CAMION ca
            INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
            WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
            GROUP BY dpe.CD_CAMION, ca.DT_CIERRE, dpe.NU_PEDIDO, dpe.CD_CLIENTE, dpe.CD_EMPRESA) d
            GROUP BY d.CD_CAMION, d.DT_CIERRE
            ORDER BY d.DT_CIERRE, d.CD_CAMION";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
       
        #endregion

        #region WST004

        public virtual void ProcesarWST004(FacturacionEjecEmpProceso proceso, string nuCuentaContable, DateTime? fechaDesde, DateTime fechaHasta, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                cdProceso = proceso.CodigoProceso,
                cdFacturacion = proceso.CodigoFacturacion,
                nuComponente = proceso.NumeroComponente,
                nuCuentaContable = nuCuentaContable,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = proceso.CodigoEmpresa,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, QT_RESULTADO, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, NU_TRANSACCION)
            SELECT :NumeroEjecucion, dpe.CD_EMPRESA, :cdProceso, :cdFacturacion, COALESCE(p.nd_facturacion_comp1,:nuComponente), :nuCuentaContable, SUM(dpe.qt_produto/p.qt_und_bulto), COALESCE(MAX(p.CD_UNIDADE_MEDIDA),'UND'), 301, :fechaModificacion, :fechaModificacion, :nuTransaccion
            FROM T_CAMION ca 
            INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
            INNER JOIN T_PRODUTO p ON dpe.CD_PRODUTO = p.CD_PRODUTO AND dpe.CD_EMPRESA = p.CD_EMPRESA
            WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
            GROUP BY dpe.CD_EMPRESA, COALESCE(p.nd_facturacion_comp1, :nuComponente)";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
        
        public virtual bool ExisteResultadoWST004(DbConnection connection, DbTransaction tran, int empresa, DateTime? fechaDesde, DateTime fechaHasta)
        {
            var model = new
            {
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = empresa
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"SELECT 1 FROM T_CAMION ca 
            INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
            INNER JOIN T_PRODUTO p ON dpe.CD_PRODUTO = p.CD_PRODUTO AND dpe.CD_EMPRESA = p.CD_EMPRESA
            WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}";

            var result = _dapper.Query<string>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? true : false;
        }
       
        public virtual void LogProcesarWST004(FacturacionEjecEmpProceso proceso, string cuentaContable, DateTime? fechaDesde, DateTime fechaHasta, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                cdProceso = proceso.CodigoProceso,
                nuComponente = proceso.NumeroComponente,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO_DETALLE (NU_EJECUCION, CD_EMPRESA, CD_FACTURACION, CD_PROCESO, DT_GENERICO, VL_SERIALIZADO, NU_COMPONENTE, QT_RESULTADO, DT_ADDROW)
            SELECT :NumeroEjecucion, d.CD_EMPRESA, :cdFacturacion, :cdProceso, trunc(d.DT_GENERICO), count(d.cd_produto) || ' Producto/s', d.NU_COMPONENTE, SUM(d.QT_RESULTADO), :fechaModificacion 
            FROM (  SELECT ca.CD_EMPRESA,
                        trunc(ca.DT_CIERRE) DT_GENERICO,
                        dpe.cd_produto,
                        SUM(dpe.qt_produto/p.qt_und_bulto) QT_RESULTADO,
                        COALESCE(p.nd_facturacion_comp1,:nuComponente) NU_COMPONENTE
                    FROM T_CAMION ca 
                    INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
                    INNER JOIN T_PRODUTO p ON dpe.CD_PRODUTO=p.CD_PRODUTO AND dpe.CD_EMPRESA=p.CD_EMPRESA
                    WHERE ca.CD_SITUACAO=652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
                    GROUP BY ca.CD_EMPRESA, trunc(ca.DT_CIERRE),dpe.cd_produto, COALESCE(p.nd_facturacion_comp1,:nuComponente)) d
            GROUP BY d.CD_EMPRESA, trunc(d.DT_GENERICO), d.NU_COMPONENTE 
            ORDER BY trunc(d.DT_GENERICO), d.NU_COMPONENTE";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
        
        #endregion

        #region WST005

        public virtual void ProcesarWST005(FacturacionEjecEmpProceso proceso, string nuCuentaContable, DateTime? fechaDesde, DateTime fechaHasta, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                cdProceso = proceso.CodigoProceso,
                cdFacturacion = proceso.CodigoFacturacion,
                nuComponente = proceso.NumeroComponente,
                nuCuentaContable = nuCuentaContable,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = proceso.CodigoEmpresa,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO (NU_EJECUCION, CD_EMPRESA, CD_PROCESO, CD_FACTURACION, NU_COMPONENTE, NU_CUENTA_CONTABLE, QT_RESULTADO, CD_UNIDADE_MEDIDA, CD_SITUACAO, DT_ADDROW, DT_UPDROW, NU_TRANSACCION)
            SELECT :NumeroEjecucion, dpe.CD_EMPRESA, :cdProceso, :cdFacturacion, COALESCE(p.nd_facturacion_comp1,:nuComponente), :nuCuentaContable, SUM(dpe.qt_produto), COALESCE(MAX(p.CD_UNIDADE_MEDIDA),'UND'), 301, :fechaModificacion, :fechaModificacion, :nuTransaccion 
            FROM T_CAMION ca 
            INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
            INNER JOIN T_PRODUTO p ON dpe.CD_PRODUTO = p.CD_PRODUTO AND dpe.CD_EMPRESA = p.CD_EMPRESA
            WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
            GROUP BY dpe.CD_EMPRESA, COALESCE(p.nd_facturacion_comp1, :nuComponente)";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
        
        public virtual bool ExisteResultadoWST005(DbConnection connection, DbTransaction tran, int empresa, DateTime? fechaDesde, DateTime fechaHasta)
        {
            var model = new
            {
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                empresa = empresa
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"SELECT 1 FROM T_CAMION ca 
            INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
            INNER JOIN T_PRODUTO p ON dpe.CD_PRODUTO = p.CD_PRODUTO AND dpe.CD_EMPRESA = p.CD_EMPRESA
            WHERE ca.CD_SITUACAO = 652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}";

            var result = _dapper.Query<string>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? true : false;
        }
       
        public virtual void LogProcesarWST005(FacturacionEjecEmpProceso proceso, string cuentaContable, DateTime? fechaDesde, DateTime fechaHasta, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                cdProceso = proceso.CodigoProceso,
                nuComponente = proceso.NumeroComponente,
                fechaDesde = fechaDesde,
                fechaHasta = fechaHasta,
                fechaModificacion = DateTime.Now,
            };

            string sqlAux = string.Empty;
            if (fechaDesde != null)
                sqlAux = $" AND ca.DT_CIERRE >= :fechaDesde ";

            string sql =
            $@"INSERT INTO T_FACTURACION_RESULTADO_DETALLE (NU_EJECUCION, CD_EMPRESA, CD_FACTURACION, CD_PROCESO, DT_GENERICO, VL_SERIALIZADO, NU_COMPONENTE, QT_RESULTADO, DT_ADDROW)
            SELECT :NumeroEjecucion, d.CD_EMPRESA, :cdFacturacion, :cdProceso, trunc(d.DT_GENERICO), count(d.cd_produto) || ' Producto/s', d.NU_COMPONENTE, SUM(d.QT_RESULTADO), :fechaModificacion 
            FROM (  SELECT ca.CD_EMPRESA,
                        trunc(ca.DT_CIERRE) DT_GENERICO,
                        dpe.cd_produto,
                        SUM(dpe.qt_produto) QT_RESULTADO,
                        COALESCE(p.nd_facturacion_comp1,:nuComponente) NU_COMPONENTE
                    FROM T_CAMION ca 
                    INNER JOIN T_DET_PEDIDO_EXPEDIDO dpe ON ca.CD_CAMION = dpe.CD_CAMION AND ca.CD_EMPRESA = dpe.CD_EMPRESA
                    INNER JOIN T_PRODUTO p ON dpe.CD_PRODUTO=p.CD_PRODUTO AND dpe.CD_EMPRESA=p.CD_EMPRESA
                    WHERE ca.CD_SITUACAO=652 AND ca.CD_EMPRESA = :empresa AND ca.DT_CIERRE IS NOT NULL AND ca.DT_CIERRE <= :fechaHasta {sqlAux}
                    GROUP BY ca.CD_EMPRESA, trunc(ca.DT_CIERRE),dpe.cd_produto, COALESCE(p.nd_facturacion_comp1,:nuComponente)) d
            GROUP BY d.CD_EMPRESA, trunc(d.DT_GENERICO), d.NU_COMPONENTE 
            ORDER BY trunc(d.DT_GENERICO), d.NU_COMPONENTE";

            _dapper.Execute(connection, sql, param: model, transaction: tran);
        }
       
        #endregion

        public virtual bool ValidarProceso(FacturacionEjecEmpProceso proceso, DbConnection connection, DbTransaction tran)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                empresa = proceso.CodigoEmpresa,
                cdFacturacion = proceso.CodigoFacturacion,
                cdProceso = proceso.CodigoProceso,
            };

            string sql = $@"
            SELECT CASE WHEN fr.RESULTADO= frd.RESULTADO_DETALLE THEN 1 END RESULTADO
            FROM (SELECT SUM(QT_RESULTADO) RESULTADO FROM T_FACTURACION_RESULTADO
            WHERE NU_EJECUCION=:NumeroEjecucion AND CD_EMPRESA = :empresa AND CD_FACTURACION = :cdFacturacion AND CD_PROCESO= :cdProceso) fr,
            (SELECT SUM(QT_RESULTADO) RESULTADO_DETALLE FROM T_FACTURACION_RESULTADO_DETALLE
            WHERE NU_EJECUCION=:NumeroEjecucion AND CD_EMPRESA = :empresa AND CD_FACTURACION = :cdFacturacion AND CD_PROCESO= :cdProceso) frd";
            var result = _dapper.Query<string>(connection, sql, param: model, transaction: tran, commandType: CommandType.Text)?.FirstOrDefault();
            return !string.IsNullOrEmpty(result) ? true : false;
        }

        public virtual void UpdateFactEmpresaProceso(FacturacionEmpresaProceso fep, DbConnection connection)
        {
            string sql = @"UPDATE T_FACTURACION_EMPRESA_PROCESO SET HR_ULTIMO_PROCESO = :UltimoProceso, TP_ULTIMO_PROCESO = :TipoUltimoProceso, NU_TRANSACCION = :NumeroTransaccion, DT_UPDROW = :FechaModificacion
                         WHERE CD_EMPRESA = :CodigoEmpresa AND CD_PROCESO = :CodigoProceso";

            fep.FechaModificacion = DateTime.Now;

            _dapper.Execute(connection, sql, fep);
        }

        public virtual void UpdateFactEjecEmpresa(FacturacionEjecEmpProceso feep, DbConnection connection)
        {
            string sql = @"UPDATE T_FACTURACION_EJEC_EMPRESA SET DT_DESDE = :FechaDesdeEjecEmp, DT_HASTA=:FechaHastaEjecEmp, ID_ESTADO=:Estado, DT_UPDROW = :FechaModificacion
                         WHERE NU_EJECUCION = :NumeroEjecucion AND CD_EMPRESA = :CodigoEmpresa AND CD_PROCESO = :CodigoProceso ";

            _dapper.Execute(connection, sql, param: new
            {
                FechaDesdeEjecEmp = feep.FechaDesdeEjecEmp,
                FechaHastaEjecEmp = feep.FechaHastaEjecEmp,
                Estado = feep.Estado,
                FechaModificacion = DateTime.Now,
                NumeroEjecucion = feep.NumeroEjecucion,
                CodigoEmpresa = feep.CodigoEmpresa,
                CodigoProceso = feep.CodigoProceso,
            });
        }

        public virtual void UpdateSituacionEjecucion(int nroFactEjecucion, short situacion, DbConnection connection)
        {
            string sql = @"UPDATE T_FACTURACION_EJECUCION SET CD_SITUACAO = :situacion, DT_UPDROW = :fechaModificacion
                         WHERE NU_EJECUCION = :nroFactEjecucion ";

            _dapper.Execute(connection, sql, new
            {
                nroFactEjecucion = nroFactEjecucion,
                situacion = situacion,
                fechaModificacion = DateTime.Now,
            });
        }

        public virtual void GuardarError(FacturacionEjecEmpProceso proceso, short cdError, string error, DbConnection connection)
        {
            var model = new
            {
                NumeroEjecucion = proceso.NumeroEjecucion,
                CodigoFacturacion = proceso.CodigoFacturacion,
                NumeroComponente = proceso.NumeroComponente,
                CodigoEmpresa = proceso.CodigoEmpresa,
                NroLinea = 1,
                cdError = cdError,
                error = error,
            };

            string sql = @"INSERT INTO T_FACTURACION_RESULTADO_ERROR (NU_EJECUCION, CD_FACTURACION, NU_COMPONENTE, CD_EMPRESA, NU_LINEA, CD_ERROR, DS_PROBLEMA)
             VALUES( :NumeroEjecucion, :CodigoFacturacion, :NumeroComponente, :CodigoEmpresa, :NroLinea, :cdError, :error)";
            _dapper.Execute(connection, sql, model);
        }
        
        #endregion
    }
}
