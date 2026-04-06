using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ContenedorMapper : Mapper
    {
        public ContenedorMapper()
        {
        }

        public virtual Contenedor MapToObject(T_CONTENEDOR entity)
        {
            if (entity == null) return null;

            return new Contenedor
            {
                NumeroPreparacion = entity.NU_PREPARACION,
                Numero = entity.NU_CONTENEDOR,
                TipoContenedor = entity.TP_CONTENEDOR,
                Estado = this.MapEstadoContenedor(entity.CD_SITUACAO),
                Ubicacion = entity.CD_ENDERECO,
                CodigoSubClase = entity.CD_SUB_CLASSE,
                CodigoPuerta = entity.CD_PORTA,
                CodigoCamion = entity.CD_CAMION,
                FechaPulmon = entity.DT_PULMON,
                FechaExpedido = entity.DT_EXPEDIDO,
                FechaAgregado = entity.DT_ADDROW,
                FechaModificado = entity.DT_UPDROW,
                CodigoFuncionarioExpedicion = entity.CD_FUNCIONARIO_EXPEDICION,
                PesoReal = entity.PS_REAL,
                Altura = entity.VL_ALTURA,
                Largo = entity.VL_LARGURA,
                Profundidad = entity.VL_PROFUNDIDADE,
                CodigoUnidadBulto = entity.CD_UNIDAD_BULTO,
                CantidadBulto = entity.QT_BULTO,
                DescripcionContenedor = entity.DS_CONTENEDOR,
                CodigoCamionCongelado = entity.CD_CAMION_CONGELADO,
                NumeroUnidadTransporte = entity.NU_UNIDAD_TRANSPORTE,
                CodigoAgrupador = entity.CD_AGRUPADOR,
                NumeroViaje = entity.NU_VIAJE,
                CodigoCanal = entity.CD_CANAL,
                IdContenedorEmpaque = entity.ID_CONTENEDOR_EMPAQUE,
                CamionFacturado = entity.CD_CAMION_FACTURADO,
                TipoControl = entity.TP_CONTROL,
                ValorCubagem = entity.VL_CUBAGEM,
                Precinto1 = entity.ID_PRECINTO_1,
                Precinto2 = entity.ID_PRECINTO_2,
                Habilitado = entity.FL_HABILITADO,
                ValorControl = entity.VL_CONTROL,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                SegundaFase = entity.FL_SEPARADO_DOS_FASES,
                IdExterno = entity.ID_EXTERNO_CONTENEDOR,
                NroLpn = entity.NU_LPN,
                IdExternoTracking = entity.ID_EXTERNO_TRACKING,
                CodigoBarras = entity.CD_BARRAS
            };
        }

        public virtual T_CONTENEDOR MapToEntity(Contenedor contenedor)
        {
            return new T_CONTENEDOR
            {
                NU_PREPARACION = contenedor.NumeroPreparacion,
                NU_CONTENEDOR = contenedor.Numero,
                TP_CONTENEDOR = contenedor.TipoContenedor,
                CD_SITUACAO = this.MapEstadoContenedor(contenedor.Estado),
                CD_ENDERECO = contenedor.Ubicacion,
                CD_SUB_CLASSE = contenedor.CodigoSubClase,
                CD_PORTA = contenedor.CodigoPuerta,
                CD_CAMION = contenedor.CodigoCamion,
                DT_PULMON = contenedor.FechaPulmon,
                DT_EXPEDIDO = contenedor.FechaExpedido,
                DT_ADDROW = contenedor.FechaAgregado,
                DT_UPDROW = contenedor.FechaModificado,
                CD_FUNCIONARIO_EXPEDICION = contenedor.CodigoFuncionarioExpedicion,
                PS_REAL = contenedor.PesoReal,
                VL_ALTURA = contenedor.Altura,
                VL_LARGURA = contenedor.Largo,
                VL_PROFUNDIDADE = contenedor.Profundidad,
                CD_UNIDAD_BULTO = contenedor.CodigoUnidadBulto,
                QT_BULTO = contenedor.CantidadBulto,
                DS_CONTENEDOR = contenedor.DescripcionContenedor,
                CD_CAMION_CONGELADO = contenedor.CodigoCamionCongelado,
                NU_UNIDAD_TRANSPORTE = contenedor.NumeroUnidadTransporte,
                CD_AGRUPADOR = contenedor.CodigoAgrupador,
                NU_VIAJE = contenedor.NumeroViaje,
                CD_CANAL = contenedor.CodigoCanal,
                ID_CONTENEDOR_EMPAQUE = contenedor.IdContenedorEmpaque,
                CD_CAMION_FACTURADO = contenedor.CamionFacturado,
                TP_CONTROL = contenedor.TipoControl,
                VL_CUBAGEM = contenedor.ValorCubagem,
                ID_PRECINTO_1 = contenedor.Precinto1,
                ID_PRECINTO_2 = contenedor.Precinto2,
                FL_HABILITADO = contenedor.Habilitado,
                VL_CONTROL = contenedor.ValorControl,
                NU_TRANSACCION = contenedor.NumeroTransaccion,
                NU_TRANSACCION_DELETE = contenedor.NumeroTransaccionDelete,
                FL_SEPARADO_DOS_FASES = contenedor.SegundaFase,
                ID_EXTERNO_CONTENEDOR = contenedor.IdExterno,
                NU_LPN = contenedor.NroLpn,
                ID_EXTERNO_TRACKING = contenedor.IdExternoTracking,
                CD_BARRAS = contenedor.CodigoBarras
            };
        }

        public virtual TipoContenedor MapToObject(T_TIPO_CONTENEDOR entity)
        {
            if (entity == null) return null;

            return new TipoContenedor
            {
                Id = entity.TP_CONTENEDOR,
                Descripcion = entity.DS_TIPO_CONTENEDOR,
                RangoInicial = entity.VL_RANGO_INICIAL,
                RangoFinal = entity.VL_RANGO_FINAL,
                UltimaSecuencia = entity.VL_ULTIMA_SECUENCIA,
                ClientePredefinido = MapStringToBoolean(entity.ID_CLIENTE_PREDEFINIDO),
                TipoEnvase = entity.ND_TP_ENVASE,
                Retornable = MapStringToBoolean(entity.FL_RETORNABLE),
                TpObjetoTracking = entity.TP_OBJETO_TRACKING,
                Habilitado = MapStringToBoolean(entity.FL_HABILITADO),
                NombreSecuencia = entity.NM_SECUENCIA
            };
        }

        public virtual T_TIPO_CONTENEDOR MapToEntity(TipoContenedor tipo)
        {
            return new T_TIPO_CONTENEDOR
            {
                TP_CONTENEDOR = tipo.Id,
                DS_TIPO_CONTENEDOR = tipo.Descripcion,
                VL_RANGO_INICIAL = tipo.RangoInicial,
                VL_RANGO_FINAL = tipo.RangoFinal,
                VL_ULTIMA_SECUENCIA = tipo.UltimaSecuencia,
                ID_CLIENTE_PREDEFINIDO = MapBooleanToString(tipo.ClientePredefinido),
                ND_TP_ENVASE = tipo.TipoEnvase,
                FL_RETORNABLE = MapBooleanToString(tipo.Retornable),
                TP_OBJETO_TRACKING = tipo.TpObjetoTracking,
                FL_HABILITADO = MapBooleanToString(tipo.Habilitado),
                NM_SECUENCIA = NullIfEmpty(tipo.NombreSecuencia),
            };
        }

        public virtual ContenedorPredefinido MapToObject(T_CONTENEDORES_PREDEFINIDOS entity)
        {
            if (entity == null) return null;
            return new ContenedorPredefinido
            {
                NumeroContenedor = entity.NU_CONTENEDOR,
                TipoContenedor = entity.TP_CONTENEDOR,
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoCliente = entity.CD_CLIENTE,
                IdExternoContenedor = entity.ID_EXTERNO_CONTENEDOR
            };
        }

        public virtual T_CONTENEDORES_PREDEFINIDOS MapToEntity(ContenedorPredefinido obj)
        {
            return new T_CONTENEDORES_PREDEFINIDOS
            {
                NU_CONTENEDOR = obj.NumeroContenedor,
                TP_CONTENEDOR = obj.TipoContenedor,
                CD_EMPRESA = obj.CodigoEmpresa,
                CD_CLIENTE = obj.CodigoCliente,
                ID_EXTERNO_CONTENEDOR = obj.IdExternoContenedor,
            };
        }

        public virtual EstadoContenedor MapEstadoContenedor(short? estado)
        {
            switch (estado)
            {
                case SituacionDb.ContenedorVacio: return EstadoContenedor.Vacio;
                case SituacionDb.ContenedorEnPreparacion: return EstadoContenedor.EnPreparacion;
                case SituacionDb.ContenedorEnCamion: return EstadoContenedor.EnCamion;
                case SituacionDb.ContenedorEnviado: return EstadoContenedor.Enviado;
                case SituacionDb.ContenedorContabilizado: return EstadoContenedor.Contabilizado;
            }

            return EstadoContenedor.Unknown;
        }

        public virtual short? MapEstadoContenedor(EstadoContenedor estado)
        {
            switch (estado)
            {
                case EstadoContenedor.Vacio: return SituacionDb.ContenedorVacio;
                case EstadoContenedor.EnPreparacion: return SituacionDb.ContenedorEnPreparacion;
                case EstadoContenedor.EnCamion: return SituacionDb.ContenedorEnCamion;
                case EstadoContenedor.Enviado: return SituacionDb.ContenedorEnviado;
                case EstadoContenedor.Contabilizado: return SituacionDb.ContenedorContabilizado;
            }

            return null;
        }
    }
}
