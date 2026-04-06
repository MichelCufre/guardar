using System.Collections.Generic;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Produccion
{
    public class FormulaMapper : Mapper
    {
        protected readonly FormulaAccionMapper _accionMapper;

        public FormulaMapper()
        {
            this._accionMapper = new FormulaAccionMapper();
        }

        public FormulaMapper(FormulaAccionMapper accionMapper)
        {
            this._accionMapper = accionMapper;
        }

        public virtual Formula MapEntityToObject(T_PRDC_DEFINICION entity)
        {
            var formula = new Formula
            {
                Id = entity.CD_PRDC_DEFINICION,
                Empresa = entity.CD_EMPRESA,
                Descripcion = entity.DS_PRDC_DEFINICION,
                Nombre = entity.NM_PRDC_DEFINICION,
                Estado = entity.CD_SITUACAO,
                Tipo = entity.TP_PRDC_DEFINICION,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                CantidadPasadasPorFormula = entity.QT_PASADAS_POR_FORMULA ?? 0,
            };

            foreach (var detalle in entity.T_PRDC_DET_ENTRADA)
            {
                formula.Entrada.Add(MapLineaEntrada(detalle));
            }

            foreach (var detalle in entity.T_PRDC_DET_SALIDA)
            {
                formula.Salida.Add(MapLineaSalida(detalle));
            }

            foreach (var config in entity.T_PRDC_CONFIGURAR_PASADA)
            {
                formula.Configuracion.Add(MapLineaConfiguracion(config));
            }

            return formula;
        }

        public virtual FormulaEntrada MapLineaEntrada(T_PRDC_DET_ENTRADA entity)
        {
            return new FormulaEntrada
            {
                IdFormula = entity.CD_PRDC_DEFINICION,
                CantidadCompleta = entity.QT_COMPLETA ?? 0,
                CantidadConsumir = entity.QT_CONSUMIDA_LINEA ?? 0,
                CantidadIncompleta = entity.QT_INCOMPLETA ?? 0,
                CantidadPasadas = entity.QT_PASADA_LINEA ?? 0,
                Empresa = entity.CD_EMPRESA,
                Componente = entity.CD_COMPONENTE,
                EmpresaPedido = entity.CD_EMPRESA_PEDIDO,
                Faixa = entity.CD_FAIXA,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Prioridad = entity.NU_PRIORIDAD,
                Producto = entity.CD_PRODUTO,
                //Ojo hacordeado porque se quito la funcionalidad del numero de pasadas 
                NumeroPasada = 1,
            };
        }

        public virtual FormulaSalida MapLineaSalida(T_PRDC_DET_SALIDA entity)
        {
            return new FormulaSalida
            {
                IdFormula = entity.CD_PRDC_DEFINICION,
                CantidadCompleta = entity.QT_COMPLETA ?? 0,
                CantidadProducir = entity.QT_CONSUMIDA_LINEA ?? 0,
                CantidadIncompleta = entity.QT_INCOMPLETA ?? 0,
                CantidadPasadas = entity.QT_PASADA_LINEA ?? 0,
                Empresa = entity.CD_EMPRESA,
                Faixa = entity.CD_FAIXA,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Producto = entity.CD_PRODUTO,
                TipoResultado = MapResultadoTipo(entity.ID_PRODUTO_FINAL),
                //Ojo hacordeado porque se quito la funcionalidad del numero de pasadas 
                NumeroPasada = 1,
            };
        }

        public virtual FormulaConfiguracion MapLineaConfiguracion(T_PRDC_CONFIGURAR_PASADA entity)
        {
            return new FormulaConfiguracion
            {
                Pasada = entity.QT_PASADAS,
                Orden = entity.NU_ORDEN,
                FechaAlta = entity.DT_ADDROW,
                Accion = this._accionMapper.MapEntityToObject(entity.T_PRDC_ACCION_INSTANCIA)
            };
        }

        public virtual T_PRDC_DEFINICION MapObjectToEntity(Formula formula)
        {
            var entity = new T_PRDC_DEFINICION
            {
                CD_PRDC_DEFINICION = formula.Id,
                NM_PRDC_DEFINICION = formula.Nombre,
                DS_PRDC_DEFINICION = formula.Descripcion,
                CD_EMPRESA = formula.Empresa,
                QT_PASADAS_POR_FORMULA = formula.CantidadPasadasPorFormula,
                DT_ADDROW = formula.FechaAlta,
                DT_UPDROW = formula.FechaModificacion,
                CD_SITUACAO = formula.Estado,
                T_PRDC_DET_ENTRADA = new List<T_PRDC_DET_ENTRADA>()
            };

            foreach (var linea in formula.Entrada)
            {
                entity.T_PRDC_DET_ENTRADA.Add(MapLineaEntrada(formula, linea));
            }

            foreach (var linea in formula.Salida)
            {
                entity.T_PRDC_DET_SALIDA.Add(MapLineaSalida(formula, linea));
            }

            foreach (var linea in formula.Configuracion)
            {
                entity.T_PRDC_CONFIGURAR_PASADA.Add(MapLineaConfiguracion(formula, linea));
            }

            return entity;
        }

        public virtual T_PRDC_DET_ENTRADA MapLineaEntrada(Formula formula, FormulaEntrada entrada)
        {
            return new T_PRDC_DET_ENTRADA
            {
                CD_PRDC_DEFINICION = formula.Id,
                CD_EMPRESA = entrada.Empresa,
                CD_EMPRESA_PEDIDO = entrada.EmpresaPedido,
                CD_FAIXA = entrada.Faixa,
                CD_PRODUTO = entrada.Producto,
                CD_COMPONENTE = entrada.Componente,
                DT_ADDROW = entrada.FechaAlta,
                DT_UPDROW = entrada.FechaModificacion,
                NU_PRIORIDAD = entrada.Prioridad,
                QT_PASADA_LINEA = entrada.NumeroPasada,
                QT_COMPLETA = entrada.CantidadCompleta,
                QT_CONSUMIDA_LINEA = entrada.CantidadConsumir,
                QT_INCOMPLETA = entrada.CantidadIncompleta,
            };
        }

        public virtual T_PRDC_DET_SALIDA MapLineaSalida(Formula formula, FormulaSalida salida)
        {
            return new T_PRDC_DET_SALIDA
            {
                CD_PRDC_DEFINICION = formula.Id,
                CD_EMPRESA = salida.Empresa,
                CD_PRODUTO = salida.Producto,
                CD_FAIXA = salida.Faixa,
                DT_ADDROW = salida.FechaAlta,
                DT_UPDROW = salida.FechaModificacion,
                QT_PASADA_LINEA = salida.NumeroPasada,
                QT_COMPLETA = salida.CantidadCompleta,
                QT_INCOMPLETA = salida.CantidadIncompleta,
                QT_CONSUMIDA_LINEA = salida.CantidadProducir,
                ID_PRODUTO_FINAL = this.MapResultadoTipo(salida.TipoResultado)
            };
        }

        public virtual T_PRDC_CONFIGURAR_PASADA MapLineaConfiguracion(Formula formula, FormulaConfiguracion configuracion)
        {
            return new T_PRDC_CONFIGURAR_PASADA
            {
                CD_ACCION_INSTANCIA = configuracion.Accion.Id,
                CD_PRDC_DEFINICION = formula.Id,
                DT_ADDROW = configuracion.FechaAlta,
                DT_UPDROW = configuracion.FechaModificacion,
                NU_ORDEN = configuracion.Orden,
                QT_PASADAS = configuracion.Pasada
            };
        }

        public virtual FormulaResultadoTipo MapResultadoTipo(string tipo)
        {
            switch (tipo)
            {
                case "F": return FormulaResultadoTipo.ProductoFinal;
                case "R": return FormulaResultadoTipo.Residuo;
            }

            return FormulaResultadoTipo.Unknown;
        }

        public virtual string MapResultadoTipo(FormulaResultadoTipo tipo)
        {
            switch (tipo)
            {
                case FormulaResultadoTipo.ProductoFinal: return "F";
                case FormulaResultadoTipo.Residuo: return "R";
            }

            return null;
        }
    }
}
