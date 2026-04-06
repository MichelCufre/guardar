using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.Persistence.Database;

namespace WIS.Domain.Produccion.Mappers
{
    public class EspacioProduccionMapper
    {
        protected readonly UbicacionMapper _ubicacionMapper;

        public EspacioProduccionMapper()
        {
            _ubicacionMapper = new UbicacionMapper();
        }

        public virtual T_PRDC_LINEA MapObjectToEntity(EspacioProduccion espacio)
        {
            var entity = new T_PRDC_LINEA
            {
                CD_PRDC_LINEA = espacio.Id,
                DS_PRDC_LINEA = espacio.Descripcion,
                ND_TIPO_LINEA = espacio.Tipo,
                CD_ENDERECO_ENTRADA = espacio.IdUbicacionEntrada,
                CD_ENDERECO_SALIDA = espacio.IdUbicacionSalida,
                NU_PREDIO = espacio.Predio,
                NU_PRDC_INGRESO = espacio.NumeroIngreso,
                DT_ADDROW = espacio.FechaAlta,
                DT_UPDROW = espacio.FechaModificacion,
                CD_ENDERECO_PRODUCCION = espacio.IdUbicacionProduccion,
                CD_ENDERECO_SALIDA_TRAN = espacio.IdUbicacionSalidaTran,
                FL_CONF_MAN = espacio.FlConfirmacionManual,
                FL_STOCK_CONSUMIBLE = espacio.FlStockConsumible,
                NU_TRANSACCION = espacio.NumeroTransaccion,
                T_ENDERECO_ESTOQUE_ENTRADA = _ubicacionMapper.MapToEntity(espacio.UbicacionEntrada),
                T_ENDERECO_ESTOQUE_PRODUCCION = _ubicacionMapper.MapToEntity(espacio.UbicacionProduccion),
                T_ENDERECO_ESTOQUE_SALIDA = _ubicacionMapper.MapToEntity(espacio.UbicacionSalida),
                T_ENDERECO_ESTOQUE_SALIDA_TRAN = _ubicacionMapper.MapToEntity(espacio.UbicacionSalidaTran)
            };

            return entity;
        }

        public virtual EspacioProduccion MapToObjet(T_PRDC_LINEA entity)
        {
            var _types = new List<string> { CEspacioProduccion.TipoEspacioaBlackBox, CEspacioProduccion.TipoEspacioWhiteBox };

            EspacioProduccion espacio;

            switch (entity.ND_TIPO_LINEA)
            {
                case CEspacioProduccion.TipoEspacioaBlackBox:
                    espacio = new EspacioBlackBox(CEspacioProduccion.TipoEspacioaBlackBox);
                    break;

                case CEspacioProduccion.TipoEspacioWhiteBox:
                    espacio = new EspacioWhiteBox(CEspacioProduccion.TipoEspacioWhiteBox);
                    break;

                default:
                    espacio = new EspacioBlackBox(CEspacioProduccion.TipoEspacioaBlackBox);
                    break;
            }

            espacio.IdUbicacionEntrada = entity.CD_ENDERECO_ENTRADA;
            espacio.IdUbicacionProduccion = entity.CD_ENDERECO_PRODUCCION;
            espacio.IdUbicacionSalida = entity.CD_ENDERECO_SALIDA;
            espacio.IdUbicacionSalidaTran = entity.CD_ENDERECO_SALIDA_TRAN;
            espacio.Id = entity.CD_PRDC_LINEA;
            espacio.Descripcion = entity.DS_PRDC_LINEA;
            espacio.Predio = entity.NU_PREDIO;
            espacio.NumeroIngreso = entity.NU_PRDC_INGRESO;
            espacio.FechaAlta = entity.DT_ADDROW;
            espacio.FechaModificacion = entity.DT_UPDROW;
            espacio.FlConfirmacionManual = entity.FL_CONF_MAN;
            espacio.FlStockConsumible = entity.FL_STOCK_CONSUMIBLE;
            espacio.NumeroTransaccion = entity.NU_TRANSACCION;
            espacio.Tipo = entity.ND_TIPO_LINEA;

            return espacio;
        }
    }
}
