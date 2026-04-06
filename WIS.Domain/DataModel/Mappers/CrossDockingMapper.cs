using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CrossDockingMapper : Mapper
    {
        public virtual ICrossDocking MapToObject(T_CROSS_DOCK entity)
        {
            if (entity == null) return null;

            var lineas = new List<LineaCrossDocking>();

            foreach (var linea in entity.T_DET_CROSS_DOCK)
            {
                lineas.Add(this.MapToObject(linea));
            }

            if (entity.TP_CROSS_DOCKING == TipoCrossDockingDb.SegundaFase)
            {
                return new CrossDockingEnDosFases
                {
                    Usuario = entity.CD_FUNCIONARIO ?? -1,
                    Agenda = entity.NU_AGENDA,
                    Preparacion = entity.NU_PREPARACION,
                    FechaAlta = entity.DT_ADDROW,
                    FechaModificacion = entity.DT_UPDROW,
                    Estado = entity.ND_ESTADO,
                    Lineas = lineas
                };
            }
            else if (entity.TP_CROSS_DOCKING == TipoCrossDockingDb.UnaFase)
            {
                return new CrossDockingEnUnaFase
                {
                    Usuario = entity.CD_FUNCIONARIO ?? -1,
                    Agenda = entity.NU_AGENDA,
                    Preparacion = entity.NU_PREPARACION,
                    FechaAlta = entity.DT_ADDROW,
                    FechaModificacion = entity.DT_UPDROW,

                    Estado = entity.ND_ESTADO,
                    Lineas = lineas
                };
            }

            return null;
        }

        public virtual LineaCrossDocking MapToObject(T_DET_CROSS_DOCK entity)
        {
            if (entity == null) return null;

            return new LineaCrossDocking
            {
                Agenda = entity.NU_AGENDA,
                Preparacion = entity.NU_PREPARACION,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Pedido = entity.NU_PEDIDO,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                EspecificaIdentificador = this.MapStringToBoolean(entity.ID_ESPECIFICA_IDENTIFICADOR),
                Carga = entity.NU_CARGA,
                Cantidad = entity.QT_PRODUTO ?? 0,
                CantidadPreparada = entity.QT_PREPARADO ?? 0,
                PreparacionPickeada = entity.NU_PREPARACION_PICKEO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NroTransaccion = entity.NU_TRANSACCION
            };
        }

        public virtual CrossDockingTemp MapToObject(T_CROSS_DOCK_TEMP entity)
        {
            if (entity == null) return null;

            return new CrossDockingTemp
            {
                NU_AGENDA = entity.NU_AGENDA,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_CLIENTE = entity.CD_CLIENTE,
                NU_PEDIDO = entity.NU_PEDIDO,
                CD_PRODUTO = entity.CD_PRODUTO,
                CD_FAIXA = entity.CD_FAIXA,
                NU_IDENTIFICADOR = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = this.MapStringToBoolean(entity.ID_ESPECIFICA_IDENTIFICADOR),
                QT_PRODUTO = entity.QT_PRODUTO ?? 0,
            };
        }

        public virtual CrossDockingTemp MapToObject(V_CROSS_DOCK_TEMP_WREC220 entity)
        {
            return new CrossDockingTemp
            {
                NU_AGENDA = entity.NU_AGENDA,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_CLIENTE = entity.CD_CLIENTE,
                NU_PEDIDO = entity.NU_PEDIDO,
                CD_PRODUTO = entity.CD_PRODUTO,
                CD_FAIXA = entity.CD_FAIXA,
                NU_IDENTIFICADOR = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = this.MapStringToBoolean(entity.ID_ESPECIFICA_IDENTIFICADOR),
                QT_PRODUTO = entity.QT_PRODUTO ?? 0,

                Ruta = entity.CD_ROTA
            };
        }

        public virtual AgendaRec220 MapToObject(V_AGENDAS_WREC270 entity)
        {
            return new AgendaRec220
            {
                Id = entity.NU_AGENDA,
                Nombre = $"Agenda {entity.NU_AGENDA}#Documento#{entity.NU_DOCUMENTO}",
            };
        }

        public virtual EtiquetaPreSep MapToObject(V_ETIQUETA_PRE_SEP_WREC270 entity)
        {
            return new EtiquetaPreSep
            {
                CD_CLIENTE = entity.CD_CLIENTE,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_ENDERECO = entity.CD_ENDERECO,
                CD_SITUACAO = entity.CD_SITUACAO,
                DS_CLIENTE = entity.DS_CLIENTE,
                ID_CTRL_ACEPTADO = entity.ID_CTRL_ACEPTADO,
                MAX_SITUACAO = entity.MAX_SITUACAO,
                MIN_SITUACAO = entity.MIN_SITUACAO,
                NU_AGENDA = entity.NU_AGENDA,
                NU_ETIQUETA_LOTE = entity.NU_ETIQUETA_LOTE,
                NU_EXTERNO_ETIQUETA = entity.NU_EXTERNO_ETIQUETA,
                NU_PREDIO = entity.NU_PREDIO,
                QT_PRODUTO = entity.QT_PRODUTO
            };
        }

        public virtual DetalleDisponibleCrossDocking MapToObject(V_DISPONIBLE_CROSS_DOCKING entity)
        {
            return new DetalleDisponibleCrossDocking
            {
                Agenda = entity.NU_AGENDA,
                Etiqueta = entity.NU_ETIQUETA_LOTE,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Faixa = entity.CD_FAIXA,
                CantidadDisponible = entity.QT_DISPONIBLE,
                ManejoIdentificador = entity.ID_MANEJO_IDENTIFICADOR,
                Predio = entity.NU_PREDIO,
            };
        }


        public virtual T_CROSS_DOCK MapToEntity(ICrossDocking obj)
        {
            var lineas = new List<T_DET_CROSS_DOCK>();

            foreach (var linea in obj.Lineas)
            {
                lineas.Add(this.MapToEntity(linea));
            }

            return new T_CROSS_DOCK
            {
                CD_FUNCIONARIO = obj.Usuario,
                NU_AGENDA = obj.Agenda,
                NU_PREPARACION = obj.Preparacion,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                ND_ESTADO = obj.Estado,
                TP_CROSS_DOCKING = obj.Tipo,
                T_DET_CROSS_DOCK = lineas
            };
        }

        public virtual T_DET_CROSS_DOCK MapToEntity(LineaCrossDocking obj)
        {
            if (obj == null) return null;

            return new T_DET_CROSS_DOCK
            {
                NU_AGENDA = obj.Agenda,
                NU_PREPARACION = obj.Preparacion,
                CD_EMPRESA = obj.Empresa,
                CD_CLIENTE = obj.Cliente,
                NU_PEDIDO = obj.Pedido,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = this.MapBooleanToString(obj.EspecificaIdentificador),
                NU_CARGA = obj.Carga,
                QT_PRODUTO = obj.Cantidad,
                QT_PREPARADO = obj.CantidadPreparada,
                NU_PREPARACION_PICKEO = obj.PreparacionPickeada,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NroTransaccion
            };
        }

        public virtual T_CROSS_DOCK_TEMP MapToEntity(CrossDockingTemp obj)
        {
            return new T_CROSS_DOCK_TEMP()
            {
                NU_AGENDA = obj.NU_AGENDA,
                CD_EMPRESA = obj.CD_EMPRESA,
                CD_CLIENTE = obj.CD_CLIENTE,
                NU_PEDIDO = obj.NU_PEDIDO,
                CD_PRODUTO = obj.CD_PRODUTO,
                CD_FAIXA = obj.CD_FAIXA,
                NU_IDENTIFICADOR = obj.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = this.MapBooleanToString(obj.ID_ESPECIFICA_IDENTIFICADOR),
                QT_PRODUTO = obj.QT_PRODUTO
            };
        }

    }
}
