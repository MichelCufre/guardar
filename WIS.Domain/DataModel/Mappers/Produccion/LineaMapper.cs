using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Produccion
{
	public class LineaMapper : Mapper
    {
        protected readonly LineaFactory _factory;

        public LineaMapper()
        {
            this._factory = new LineaFactory();
        }

        public virtual ILinea MapLineaEntityToObject(T_PRDC_LINEA linea)
        {
            if (linea == null)
                return null;

            TipoProduccionLinea tipo = this.MapStringToTipoLinea(linea.ND_TIPO_LINEA);
            ILinea lineaObject = this._factory.Create(tipo);

            this.SetPropertiesLinea(tipo, lineaObject, linea);

            return lineaObject;
        }

        public virtual LineaWhiteBox MapLineaWhiteBoxEntityToObject(T_PRDC_LINEA linea)
        {
            if (linea == null)
                return null;

            LineaWhiteBox lineaObject = new LineaWhiteBox();

            this.SetPropertiesLineaWhiteBox(TipoProduccionLinea.WhiteBox, lineaObject, linea);

            return lineaObject;
        }
        public virtual LineaBlackBox MapLineaBlackBoxEntityToObject(T_PRDC_LINEA linea)
        {
            if (linea == null)
                return null;

            LineaBlackBox lineaObject = new LineaBlackBox();

            this.SetPropertiesLineaBlackBox(TipoProduccionLinea.WhiteBox, lineaObject, linea);

            return lineaObject;
        }


        public virtual void SetPropertiesLinea(TipoProduccionLinea tipo, ILinea linea, T_PRDC_LINEA lineaEntity)
        {
            linea.Id = lineaEntity.CD_PRDC_LINEA;
            linea.Descripcion = lineaEntity.DS_PRDC_LINEA;
            linea.Tipo = tipo;
            linea.UbicacionEntrada = lineaEntity.CD_ENDERECO_ENTRADA;
            linea.UbicacionSalida = lineaEntity.CD_ENDERECO_SALIDA;
            linea.Predio = lineaEntity.NU_PREDIO;
            linea.NumeroIngreso = lineaEntity.NU_PRDC_INGRESO;
            linea.FechaAlta = lineaEntity.DT_ADDROW;
            linea.FechaModificacion = lineaEntity.DT_UPDROW;
        }

        public virtual void SetPropertiesLineaWhiteBox(TipoProduccionLinea tipo, LineaWhiteBox linea, T_PRDC_LINEA lineaEntity)
        {
            linea.Id = lineaEntity.CD_PRDC_LINEA;
            linea.Descripcion = lineaEntity.DS_PRDC_LINEA;
            linea.Tipo = tipo;
            linea.UbicacionEntrada = lineaEntity.CD_ENDERECO_ENTRADA;
            linea.UbicacionSalida = lineaEntity.CD_ENDERECO_SALIDA;
            linea.Predio = lineaEntity.NU_PREDIO;
            linea.NumeroIngreso = lineaEntity.NU_PRDC_INGRESO;
        }

        public virtual void SetPropertiesLineaBlackBox(TipoProduccionLinea tipo, LineaBlackBox linea, T_PRDC_LINEA lineaEntity)
        {
            linea.Id = lineaEntity.CD_PRDC_LINEA;
            linea.Descripcion = lineaEntity.DS_PRDC_LINEA;
            linea.Tipo = tipo;
            linea.UbicacionEntrada = lineaEntity.CD_ENDERECO_ENTRADA;
            linea.UbicacionSalida = lineaEntity.CD_ENDERECO_SALIDA;
            linea.Predio = lineaEntity.NU_PREDIO;
            linea.NumeroIngreso = lineaEntity.NU_PRDC_INGRESO;
        }

        public virtual T_PRDC_LINEA MapObjectToEntity(ILinea linea)
        {
            var entity = new T_PRDC_LINEA
            {
                CD_PRDC_LINEA = linea.Id,
                DS_PRDC_LINEA = linea.Descripcion,
                ND_TIPO_LINEA = this.MapTipoLineaToString(linea.Tipo),
                CD_ENDERECO_ENTRADA = NullIfEmpty(linea.UbicacionEntrada),
                CD_ENDERECO_SALIDA = NullIfEmpty(linea.UbicacionSalida),
                NU_PREDIO = linea.Predio,
                NU_PRDC_INGRESO = linea.NumeroIngreso,
                DT_ADDROW = linea.FechaAlta,
                DT_UPDROW = linea.FechaModificacion,
            };

            if (linea is LineaBlackBox)
            {
                entity.CD_ENDERECO_PRODUCCION = ((LineaBlackBox)linea).UbicacionBlackBox;
            }

            return entity;
        }

        public virtual TipoProduccionLinea MapStringToTipoLinea(string tipo)
        {
            switch (tipo)
            {
                case TipoLineaProduccion.TipoLineaBlackBox:
                    return TipoProduccionLinea.BlackBox;
                case TipoLineaProduccion.TipoLineaWhiteBox:
                    return TipoProduccionLinea.WhiteBox;
            }

            return TipoProduccionLinea.Unknown;
        }

        public virtual string MapTipoLineaToString(TipoProduccionLinea tipo)
        {
            switch (tipo)
            {
                case TipoProduccionLinea.BlackBox:
                    return TipoLineaProduccion.TipoLineaBlackBox;
                case TipoProduccionLinea.WhiteBox:
                    return TipoLineaProduccion.TipoLineaWhiteBox;
            }

            return "";
        }

        public virtual DetallePedidoE MapEntityToObject(V_KIT200_DET_PEDIDO_SAIDA linea)
        {
            return new DetallePedidoE
            {
                Pedido = linea.NU_PEDIDO,
                Cliente = linea.CD_CLIENTE,
                Empresa = linea.CD_EMPRESA,
                Producto = linea.CD_PRODUTO,
                Identificador = linea.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Faixa = linea.CD_FAIXA,
                Semiacabado = linea.FL_SEMIACABADO,
                Consumible = linea.FL_CONSUMIBLE,
                Predio = linea.NU_PREDIO
            };
        }
    }
}
