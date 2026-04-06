using System.Collections.Generic;
using System.Linq;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ControlDeCalidadMapper : Mapper
    {
        protected readonly StockMapper _stockMapper;

        public ControlDeCalidadMapper()
        {
            this._stockMapper = new StockMapper();

        }

        public virtual List<ControlDeCalidad> MapToObject(List<T_TIPO_CTR_CALIDAD> entity)
        {
            List<ControlDeCalidad> tiposDeControles = new List<ControlDeCalidad>();
            if (entity == null)
                return null;

            foreach (var item in entity)
            {
                ControlDeCalidad tipo = this.MapToObject(item);
                tiposDeControles.Add(tipo);

            }
            return tiposDeControles;          
        }
        public virtual List <T_CTR_CALIDAD_PENDIENTE> MapToEntity (List <ControlDeCalidadPendiente> toAddControlesPendientes) =>
            toAddControlesPendientes.Select (this.MapToEntity).ToList ();
        
        
        public virtual ControlDeCalidad MapToObject(T_TIPO_CTR_CALIDAD entity)
        {
            if (entity == null)
                return null;

            return new ControlDeCalidad
            {
                Id = entity.CD_CONTROL,
                Descripcion = entity.DS_CONTROL,
                Sigla = entity.SG_CONTROL,
                EsBloqueante = MapStringToBoolean(entity.ID_BLOQUEIO),
                FechaInsercion = entity.DT_ADDROW,
                FechaActualizacion = entity.DT_UPDROW
            };
        }
        public virtual T_TIPO_CTR_CALIDAD MapToEntity(ControlDeCalidad obj)
        {
            return new T_TIPO_CTR_CALIDAD
            {
                CD_CONTROL = obj.Id,
                DS_CONTROL = obj.Descripcion,
                SG_CONTROL = obj.Sigla,
                ID_BLOQUEIO = MapBooleanToString(obj.EsBloqueante),
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaActualizacion
            };
        }

        public virtual ControlDeCalidadPendiente MapToObject(T_CTR_CALIDAD_PENDIENTE entity)
        {
            if (entity == null) return null;

            return new ControlDeCalidadPendiente
            {
                Id = entity.NU_CTR_CALIDAD_PENDIENTE,
                Codigo = entity.CD_CONTROL,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Etiqueta = entity.NU_ETIQUETA,
                Ubicacion = entity.CD_ENDERECO,
                FuncionarioAceptacion = entity.CD_FUNCIONARIO_ACEPTO,
                Predio = entity.NU_PREDIO,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Faixa = entity.CD_FAIXA,
                Aceptado = this.MapStringToBoolean(entity.ID_ACEPTADO),
                NroLPN = entity.NU_LPN,
                IdLpnDet = entity.ID_LPN_DET,
                Descripcion = entity.DS_CONTROL,
                Instancia = entity.NU_INSTANCIA_CONTROL
            };
        }
        public virtual T_CTR_CALIDAD_PENDIENTE MapToEntity(ControlDeCalidadPendiente obj)
        {
            if (obj == null) return null;

            var ctrlCalidad = new T_CTR_CALIDAD_PENDIENTE
            {
                NU_CTR_CALIDAD_PENDIENTE = obj.Id,
                CD_CONTROL = obj.Codigo,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                CD_EMPRESA = obj.Empresa,
                CD_ENDERECO = obj.Ubicacion,
                CD_FAIXA = obj.Faixa,
                CD_PRODUTO = obj.Producto,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ACEPTADO = this.MapBooleanToString(obj.Aceptado),
                NU_ETIQUETA = obj.Etiqueta,
                NU_PREDIO = obj.Predio,
                CD_FUNCIONARIO_ACEPTO = obj.FuncionarioAceptacion,
                NU_LPN = obj.NroLPN,
                ID_LPN_DET = obj.IdLpnDet,
                NU_INSTANCIA_CONTROL = obj.Instancia,
                DS_CONTROL = obj.Descripcion
            };

            if (obj.Stock != null)
            {
                ctrlCalidad.CD_EMPRESA = obj.Stock.Empresa;
                ctrlCalidad.CD_PRODUTO = obj.Stock.Producto;
                ctrlCalidad.CD_FAIXA = obj.Stock.Faixa;
                ctrlCalidad.NU_IDENTIFICADOR = obj.Stock.Identificador?.Trim()?.ToUpper();
                ctrlCalidad.CD_ENDERECO = obj.Stock.Ubicacion;
            }

            return ctrlCalidad;
        }
        public virtual ControlDeCalidadPendiente MapToObject(T_CTR_CALIDAD_PENDIENTE entity, T_STOCK entityStock)
        {
            var control = this.MapToObject(entity);

            if (entityStock != null)
                control.Stock = this._stockMapper.MapToStock(entityStock);

            return control;
        }

        public virtual ControlDeCalidadProducto MapToObject(T_CTR_CALIDAD_NECESARIO entity)
        {
            if (entity == null)
                return null;

            return new ControlDeCalidadProducto
            {
                Codigo = entity.CD_CONTROL,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }
        public virtual T_CTR_CALIDAD_NECESARIO MapToEntity(ControlDeCalidadProducto obj)
        {
            return new T_CTR_CALIDAD_NECESARIO
            {
                CD_CONTROL = obj.Codigo,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion
            };
        }
    }
}



