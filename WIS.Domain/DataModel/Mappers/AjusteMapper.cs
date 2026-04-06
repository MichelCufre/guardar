using WIS.Domain.General;
using WIS.Domain.ManejoStock;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AjusteMapper : Mapper
    {
        public AjusteMapper()
        {
        }

        public virtual AjusteStock MapToAjusteStock(T_AJUSTE_STOCK ajusteEntity)
        {
            return new AjusteStock
            {
                Producto = ajusteEntity.CD_PRODUTO,
                Faixa = ajusteEntity.CD_FAIXA,
                Identificador = ajusteEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Empresa = ajusteEntity.CD_EMPRESA,
                FechaRealizado = ajusteEntity.DT_REALIZADO,
                TipoAjuste = ajusteEntity.TP_AJUSTE,
                QtMovimiento = ajusteEntity.QT_MOVIMIENTO,
                DescMotivo = ajusteEntity.DS_MOTIVO,
                TpDocumento = ajusteEntity.TP_DOCUMENTO,
                NuDocumento = ajusteEntity.NU_DOCUMENTO,
                FechaModificacion = ajusteEntity.DT_UPDROW,
                IdProcesar = ajusteEntity.ID_PROCESAR,
                CdMotivoAjuste = ajusteEntity.CD_MOTIVO_AJUSTE,
                NuAjusteStock = ajusteEntity.NU_AJUSTE_STOCK,
                NuLogInventario = ajusteEntity.NU_LOG_INVENTARIO,
                NuTransaccion = ajusteEntity.NU_TRANSACCION,
                Predio = ajusteEntity.NU_PREDIO,
                FuncionarioMotivo = ajusteEntity.CD_FUNC_MOTIVO,
                FechaMotivo = ajusteEntity.DT_MOTIVO,
                Aplicacion = ajusteEntity.CD_APLICACAO,
                Funcionario = ajusteEntity.CD_FUNCIONARIO,
                IdAreaAveria = ajusteEntity.ID_AREA_AVERIA,
                Ubicacion = ajusteEntity.CD_ENDERECO,
                NuInventarioEnderecoDet = ajusteEntity.NU_INVENTARIO_ENDERECO_DET,
                IdProcesado = ajusteEntity.ID_PROCESADO,
                NuInterfazEjecucion = ajusteEntity.NU_INTERFAZ_EJECUCION,
                Serializado = ajusteEntity.VL_SERIALIZADO,
                FechaVencimiento = ajusteEntity.DT_FABRICACAO,
                Metadata = ajusteEntity.VL_METADATA,
                Atributos = ajusteEntity.VL_ATRIBUTOS_LPN,
            };
        }

        public virtual T_AJUSTE_STOCK MapFromAjusteStock(AjusteStock ajuste)
        {
            return new T_AJUSTE_STOCK
            {
                CD_PRODUTO = ajuste.Producto,
                CD_FAIXA = ajuste.Faixa,
                NU_IDENTIFICADOR = ajuste.Identificador?.Trim()?.ToUpper(),
                CD_EMPRESA = ajuste.Empresa,
                DT_REALIZADO = ajuste.FechaRealizado,
                TP_AJUSTE = ajuste.TipoAjuste,
                QT_MOVIMIENTO = ajuste.QtMovimiento,
                DS_MOTIVO = ajuste.DescMotivo,
                TP_DOCUMENTO = ajuste.TpDocumento,
                NU_DOCUMENTO = ajuste.NuDocumento,
                DT_UPDROW = ajuste.FechaModificacion,
                ID_PROCESAR = ajuste.IdProcesar,
                CD_MOTIVO_AJUSTE = ajuste.CdMotivoAjuste,
                NU_AJUSTE_STOCK = ajuste.NuAjusteStock,
                NU_LOG_INVENTARIO = ajuste.NuLogInventario,
                NU_TRANSACCION = ajuste.NuTransaccion,
                NU_PREDIO = ajuste.Predio,
                CD_FUNC_MOTIVO = ajuste.FuncionarioMotivo,
                DT_MOTIVO = ajuste.FechaMotivo,
                CD_APLICACAO = ajuste.Aplicacion,
                CD_FUNCIONARIO = ajuste.Funcionario,
                ID_AREA_AVERIA = ajuste.IdAreaAveria,
                CD_ENDERECO = ajuste.Ubicacion,
                NU_INVENTARIO_ENDERECO_DET = ajuste.NuInventarioEnderecoDet,
                ID_PROCESADO = ajuste.IdProcesado,
                NU_INTERFAZ_EJECUCION = ajuste.NuInterfazEjecucion,
                VL_SERIALIZADO = ajuste.Serializado,
                DT_FABRICACAO = ajuste.FechaVencimiento,
                VL_METADATA = ajuste.Metadata,
                VL_ATRIBUTOS_LPN = ajuste.Atributos
            };
        }

        public virtual MotivoAjuste MapToMotivoAjuste(T_MOTIVO_AJUSTE entity)
        {
            return new MotivoAjuste
            {
                Codigo = entity.CD_MOTIVO_AJUSTE,
                Descripcion = entity.DS_MOTIVO_AJUSTE,
                FechaCreacion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual T_MOTIVO_AJUSTE MapFromMotivoAjuste(MotivoAjuste obj)
        {
            return new T_MOTIVO_AJUSTE
            {
                CD_MOTIVO_AJUSTE = obj.Codigo,
                DS_MOTIVO_AJUSTE = obj.Descripcion,
                DT_ADDROW = obj.FechaCreacion,
                DT_UPDROW = obj.FechaModificacion
            };
        }

        public virtual DocumentoAjusteStock MapToAjusteDocumental(T_DOCUMENTO_AJUSTE_STOCK ajusteDocumentoEntity)
        {
            if (ajusteDocumentoEntity == null)
                return null;

            return this.SetPropertiesAjusteStockDocumento(ajusteDocumentoEntity);
        }

        public virtual DocumentoAjusteStock SetPropertiesAjusteStockDocumento(T_DOCUMENTO_AJUSTE_STOCK ajusteDocumentoEntity)
        {
            return new DocumentoAjusteStock()
            {
                Aplicacion = ajusteDocumentoEntity.CD_APLICACAO,
                CantidadMovimiento = ajusteDocumentoEntity.QT_MOVIMIENTO,
                CodigoEmpresa = ajusteDocumentoEntity.CD_EMPRESA,
                CodigoFuncionario = ajusteDocumentoEntity.CD_FUNCIONARIO,
                CodigoMotivoAjuste = ajusteDocumentoEntity.CD_MOTIVO_AJUSTE,
                DescripcionMotivo = ajusteDocumentoEntity.DS_MOTIVO,
                Endereco = ajusteDocumentoEntity.CD_ENDERECO,
                Faixa = ajusteDocumentoEntity.CD_FAIXA,
                FechaActualizacion = ajusteDocumentoEntity.DT_UPDROW,
                FechaCreacion = ajusteDocumentoEntity.DT_ADDROW,
                FechaMotivo = ajusteDocumentoEntity.DT_MOTIVO,
                FuncionarioMotivo = ajusteDocumentoEntity.CD_FUNC_MOTIVO,
                Identificador = ajusteDocumentoEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NumeroAjuste = ajusteDocumentoEntity.NU_AJUSTE_STOCK,
                NumeroDocumento = ajusteDocumentoEntity.NU_DOCUMENTO,
                NumeroTransaccion = ajusteDocumentoEntity.NU_TRANSACCION,
                Producto = ajusteDocumentoEntity.CD_PRODUTO,
                Predio = ajusteDocumentoEntity.NU_PREDIO,
                TipoDocumento = ajusteDocumentoEntity.TP_DOCUMENTO
            };
        }

        public virtual DocumentoAjusteStock MapFromAjusteToAjusteDocumental(AjusteStock ajuste)
        {
            if (ajuste == null)
                return null;

            return new DocumentoAjusteStock()
            {
                Aplicacion = ajuste.Aplicacion,
                CantidadMovimiento = ajuste.QtMovimiento,
                CodigoEmpresa = ajuste.Empresa,
                CodigoFuncionario = ajuste.Funcionario,
                CodigoMotivoAjuste = ajuste.CdMotivoAjuste,
                DescripcionMotivo = ajuste.DescMotivo,
                Endereco = ajuste.Ubicacion,
                Faixa = ajuste.Faixa,
                FechaActualizacion = ajuste.FechaModificacion,
                FechaCreacion = ajuste.FechaRealizado,
                FechaMotivo = ajuste.FechaMotivo,
                FuncionarioMotivo = ajuste.FuncionarioMotivo,
                Identificador = ajuste.Identificador?.Trim()?.ToUpper(),
                NumeroAjuste = ajuste.NuAjusteStock,
                NumeroDocumento = ajuste.NuDocumento,
                NumeroTransaccion = ajuste.NuTransaccion,
                Producto = ajuste.Producto,
                Predio = ajuste.Predio,
                TipoDocumento = ajuste.TpDocumento
            };
        }

        public virtual T_DOCUMENTO_AJUSTE_STOCK MapFromAjusteDocumental(DocumentoAjusteStock ajuste)
        {
            if (ajuste == null)
                return null;

            return this.CreateEntityAjusteStock(ajuste);
        }

        public virtual T_DOCUMENTO_AJUSTE_STOCK_HIST MapFromAjusteDocumentalHistorico(DocumentoAjusteStockHistorico ajuste)
        {
            if (ajuste == null)
                return null;

            return this.CreateEntityAjusteStockHistorico(ajuste);
        }

        public virtual T_DOCUMENTO_AJUSTE_STOCK CreateEntityAjusteStock(DocumentoAjusteStock ajuste)
        {
            return new T_DOCUMENTO_AJUSTE_STOCK()
            {
                TP_DOCUMENTO = ajuste.TipoDocumento,
                NU_PREDIO = ajuste.Predio,
                CD_APLICACAO = ajuste.Aplicacion,
                CD_EMPRESA = ajuste.CodigoEmpresa,
                CD_ENDERECO = ajuste.Endereco,
                CD_FAIXA = ajuste.Faixa,
                CD_FUNCIONARIO = ajuste.CodigoFuncionario,
                CD_FUNC_MOTIVO = ajuste.FuncionarioMotivo,
                CD_MOTIVO_AJUSTE = ajuste.CodigoMotivoAjuste,
                CD_PRODUTO = ajuste.Producto,
                DS_MOTIVO = ajuste.DescripcionMotivo,
                DT_ADDROW = ajuste.FechaCreacion,
                DT_MOTIVO = ajuste.FechaMotivo,
                DT_UPDROW = ajuste.FechaActualizacion,
                NU_AJUSTE_STOCK = ajuste.NumeroAjuste,
                NU_DOCUMENTO = ajuste.NumeroDocumento,
                NU_IDENTIFICADOR = ajuste.Identificador?.Trim()?.ToUpper(),
                NU_TRANSACCION = ajuste.NumeroTransaccion,
                QT_MOVIMIENTO = ajuste.CantidadMovimiento
            };
        }

        public virtual T_DOCUMENTO_AJUSTE_STOCK_HIST CreateEntityAjusteStockHistorico(DocumentoAjusteStockHistorico ajuste)
        {
            return new T_DOCUMENTO_AJUSTE_STOCK_HIST()
            {
                TP_DOCUMENTO = ajuste.TipoDocumento,
                NU_PREDIO = ajuste.Predio,
                CD_APLICACAO = ajuste.Aplicacion,
                CD_EMPRESA = ajuste.CodigoEmpresa,
                CD_ENDERECO = ajuste.Endereco,
                CD_FAIXA = ajuste.Faixa,
                CD_FUNCIONARIO = ajuste.CodigoFuncionario,
                CD_FUNC_MOTIVO = ajuste.FuncionarioMotivo,
                CD_MOTIVO_AJUSTE = ajuste.CodigoMotivoAjuste,
                CD_PRODUTO = ajuste.Producto,
                DS_MOTIVO = ajuste.DescripcionMotivo,
                DT_ADDROW = ajuste.FechaCreacion,
                DT_MOTIVO = ajuste.FechaMotivo,
                DT_UPDROW = ajuste.FechaActualizacion,
                NU_AJUSTE_STOCK = ajuste.NumeroAjuste,
                NU_DOCUMENTO = ajuste.NumeroDocumento,
                NU_IDENTIFICADOR = ajuste.Identificador?.Trim()?.ToUpper(),
                NU_TRANSACCION = ajuste.NumeroTransaccion,
                QT_MOVIMIENTO = ajuste.CantidadMovimiento,
                TP_OPERACION = ajuste.TipoOperacion,
                NU_OPERACION = ajuste.NumeroOperacion
            };
        }

        public virtual DocumentoAjusteStockHistorico MapToAjusteDocumentalHistorico(T_DOCUMENTO_AJUSTE_STOCK_HIST ajusteDocumentoHistoricoEntity)
        {
            if (ajusteDocumentoHistoricoEntity == null)
                return null;

            return this.SetPropertiesAjusteStockDocumentoHistorico(ajusteDocumentoHistoricoEntity);
        }

        public virtual DocumentoAjusteStockHistorico SetPropertiesAjusteStockDocumentoHistorico(T_DOCUMENTO_AJUSTE_STOCK_HIST ajusteDocumentoHistoricoEntity)
        {
            return new DocumentoAjusteStockHistorico()
            {
                Aplicacion = ajusteDocumentoHistoricoEntity.CD_APLICACAO,
                CantidadMovimiento = ajusteDocumentoHistoricoEntity.QT_MOVIMIENTO,
                CodigoEmpresa = ajusteDocumentoHistoricoEntity.CD_EMPRESA,
                CodigoFuncionario = ajusteDocumentoHistoricoEntity.CD_FUNCIONARIO,
                CodigoMotivoAjuste = ajusteDocumentoHistoricoEntity.CD_MOTIVO_AJUSTE,
                DescripcionMotivo = ajusteDocumentoHistoricoEntity.DS_MOTIVO,
                Endereco = ajusteDocumentoHistoricoEntity.CD_ENDERECO,
                Faixa = ajusteDocumentoHistoricoEntity.CD_FAIXA,
                FechaActualizacion = ajusteDocumentoHistoricoEntity.DT_UPDROW,
                FechaCreacion = ajusteDocumentoHistoricoEntity.DT_ADDROW,
                FechaMotivo = ajusteDocumentoHistoricoEntity.DT_MOTIVO,
                FuncionarioMotivo = ajusteDocumentoHistoricoEntity.CD_FUNC_MOTIVO,
                Identificador = ajusteDocumentoHistoricoEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NumeroAjuste = ajusteDocumentoHistoricoEntity.NU_AJUSTE_STOCK,
                NumeroDocumento = ajusteDocumentoHistoricoEntity.NU_DOCUMENTO,
                NumeroTransaccion = ajusteDocumentoHistoricoEntity.NU_TRANSACCION,
                Producto = ajusteDocumentoHistoricoEntity.CD_PRODUTO,
                Predio = ajusteDocumentoHistoricoEntity.NU_PREDIO,
                TipoDocumento = ajusteDocumentoHistoricoEntity.TP_DOCUMENTO,
                TipoOperacion = ajusteDocumentoHistoricoEntity.TP_OPERACION,
                NumeroOperacion = ajusteDocumentoHistoricoEntity.NU_OPERACION
            };
        }
    }
}
