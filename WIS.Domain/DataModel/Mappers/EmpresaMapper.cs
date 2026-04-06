using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EmpresaMapper : Mapper
    {
        protected readonly DominioMapper _dominioMapper;
        protected readonly PaisSubdivisionLocalidadMapper _localidadMapper;

        public EmpresaMapper()
        {
            this._dominioMapper = new DominioMapper();
            this._localidadMapper = new PaisSubdivisionLocalidadMapper(new PaisSubdivisionMapper(new PaisMapper()));
        }

        public virtual Empresa MapToObject(T_EMPRESA entity)
        {
            if (entity == null) return null;

            return new Empresa
            {
                Id = entity.CD_EMPRESA,
                Nombre = entity.NM_EMPRESA,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Anexo4 = entity.DS_ANEXO4,
                CdClienteArmadoKit = entity.CD_CLIENTE_ARMADO_KIT,
                CodigoPostal = entity.DS_CP_POSTAL,
                Direccion = entity.DS_ENDERECO,
                Estado = this.MapEstado(entity.CD_SITUACAO),
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                IdLocalidad = entity.ID_LOCALIDAD,
                NumeroFiscal = entity.CD_CGC_EMPRESA,
                TipoFiscalId = entity.ND_TIPO_FISCAL,
                Telefono = entity.NU_TELEFONE,
                cdTipoDeAlmacenajeYSeguro = entity.TP_ALMACENAJE_Y_SEGURO,
                ValorMinimoStock = entity.IM_MINIMO_STOCK,
                EmpresaConsolidado = entity.CD_EMPRESA_DE_CONSOLIDADO,
                ProveedorDevolucion = entity.CD_FORN_DEVOLUCAO,
                ListaPrecio = entity.CD_LISTA_PRECIO,
                FG_QUEBRA_PEDIDO = entity.FG_QUEBRA_PEDIDO,
                IdDAP = entity.ID_DAP,
                IdOperativo = entity.ID_OPERATIVO,
                IdUnidadFactura = entity.ID_UND_FACT_EMPRESA,
                CantidadDiasPeriodo = entity.QT_DIAS_POR_PERIODO,
                ValorPallet = entity.VL_POS_PALETE,
                ValorPalletDia = entity.VL_POS_PALETE_DIA,
                TipoNotificacionId = entity.TP_NOTIFICACION,
                PayloadUrl = entity.VL_PAYLOAD_URL,
                IsLocked = MapStringToBoolean(entity.FL_LOCKED)
            };
        }

        public virtual Empresa MapToObjectWithRelations(T_EMPRESA entity, T_PAIS_SUBDIVISION_LOCALIDAD localidad, T_DET_DOMINIO tipoFiscal)
        {
            return MapToObjectWithRelations(entity, localidad, tipoFiscal, null);
        }

        public virtual Empresa MapToObjectWithRelations(T_EMPRESA entity, T_PAIS_SUBDIVISION_LOCALIDAD localidad, T_DET_DOMINIO tipoFiscal, T_DET_DOMINIO tipoNotificacion)
        {
            Empresa empresa = this.MapToObject(entity);
            empresa.TipoFiscal = this._dominioMapper.MapToObject(tipoFiscal);
            empresa.TipoNotificacion = tipoNotificacion != null ? this._dominioMapper.MapToObject(tipoNotificacion) : null;
            empresa.Localidad = this._localidadMapper.MapToObject(localidad);

            return empresa;
        }

        public virtual T_EMPRESA MapToEntity(Empresa empresa)
        {
            if (empresa == null) return null;

            T_EMPRESA entity = new T_EMPRESA
            {
                CD_EMPRESA = empresa.Id,
                NM_EMPRESA = empresa.Nombre,
                CD_SITUACAO = this.MapEstado(empresa.Estado),
                DT_UPDROW = empresa.FechaModificacion,
                DT_ADDROW = empresa.FechaInsercion,
                DS_ENDERECO = empresa.Direccion,
                NU_TELEFONE = empresa.Telefono,
                ND_TIPO_FISCAL = empresa.TipoFiscal?.Id ?? empresa.TipoFiscalId,
                CD_CGC_EMPRESA = empresa.NumeroFiscal,
                DS_CP_POSTAL = empresa.CodigoPostal,
                CD_CLIENTE_ARMADO_KIT = empresa.CdClienteArmadoKit,
                DS_ANEXO1 = empresa.Anexo1,
                DS_ANEXO2 = empresa.Anexo2,
                DS_ANEXO3 = empresa.Anexo3,
                DS_ANEXO4 = empresa.Anexo4,
                TP_ALMACENAJE_Y_SEGURO = empresa.cdTipoDeAlmacenajeYSeguro,
                ID_LOCALIDAD = empresa.IdLocalidad,
                IM_MINIMO_STOCK = empresa.ValorMinimoStock,
                CD_EMPRESA_DE_CONSOLIDADO = empresa.EmpresaConsolidado,
                CD_FORN_DEVOLUCAO = empresa.ProveedorDevolucion,
                CD_LISTA_PRECIO = empresa.ListaPrecio,
                FG_QUEBRA_PEDIDO = empresa.FG_QUEBRA_PEDIDO,
                ID_DAP = empresa.IdDAP,
                ID_OPERATIVO = empresa.IdOperativo,
                ID_UND_FACT_EMPRESA = empresa.IdUnidadFactura,
                QT_DIAS_POR_PERIODO = empresa.CantidadDiasPeriodo,
                VL_POS_PALETE = empresa.ValorPallet,
                VL_POS_PALETE_DIA = empresa.ValorPalletDia,
                TP_NOTIFICACION = empresa.TipoNotificacion?.Id ?? empresa.TipoNotificacionId,
                VL_PAYLOAD_URL = empresa.PayloadUrl,
                FL_LOCKED = MapBooleanToString(empresa.IsLocked)
            };

            if (empresa.IdLocalidad == null && empresa.Localidad != null)
            {
                entity.ID_LOCALIDAD = empresa.Localidad.Id;
            }

            return entity;
        }

        public virtual EstadoEmpresa MapEstado(short estado)
        {
            switch (estado)
            {
                case SituacionDb.Activo: return EstadoEmpresa.Activo;
                case SituacionDb.Inactivo: return EstadoEmpresa.Inactivo;
            }

            return EstadoEmpresa.Unknown;
        }
        public virtual short MapEstado(EstadoEmpresa estado)
        {
            switch (estado)
            {
                case EstadoEmpresa.Activo: return SituacionDb.Activo;
                case EstadoEmpresa.Inactivo: return SituacionDb.Inactivo;
            }

            return 0;
        }






    }
}
