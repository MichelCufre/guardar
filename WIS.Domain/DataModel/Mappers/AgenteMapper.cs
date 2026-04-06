using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AgenteMapper : Mapper
    {
        protected readonly DominioMapper _dominioMapper;
        protected readonly PaisSubdivisionLocalidadMapper _localidadMapper;
        protected readonly RutaMapper _rutaMapper;

        public AgenteMapper()
        {
            this._dominioMapper = new DominioMapper();
            this._localidadMapper = new PaisSubdivisionLocalidadMapper(new PaisSubdivisionMapper(new PaisMapper()));
            this._rutaMapper = new RutaMapper();
        }

        public virtual Agente MapToObject(T_CLIENTE entity, T_PAIS_SUBDIVISION_LOCALIDAD localidad = null, T_ROTA ruta = null, T_DET_DOMINIO tipoFiscal = null, T_DET_DOMINIO tipoAgente = null)
        {
            if (entity == null)
                return null;

            Agente agente = new Agente();

            agente.Codigo = entity.CD_AGENTE;
            agente.Tipo = entity.TP_AGENTE;
            agente.CodigoInterno = entity.CD_CLIENTE;
            agente.Descripcion = entity.DS_CLIENTE;
            agente.Direccion = entity.DS_ENDERECO;
            agente.Empresa = entity.CD_EMPRESA;
            agente.TipoFiscal = this._dominioMapper.MapToObject(tipoFiscal);
            agente.Anexo1 = entity.DS_ANEXO1;
            agente.Anexo2 = entity.DS_ANEXO2;
            agente.Anexo3 = entity.DS_ANEXO3;
            agente.Anexo4 = entity.DS_ANEXO4;
            agente.Email = entity.DS_EMAIL;
            agente.Barrio = entity.DS_BAIRRO;
            agente.CodigoPostal = entity.CD_CEP;
            agente.FechaAlta = entity.DT_CADASTRAMENTO;
            agente.FechaModificacion = entity.DT_ALTERACAO;
            agente.FechaSituacion = entity.DT_SITUACAO;
            agente.Localidad = this._localidadMapper.MapToObject(localidad);
            agente.IdLocalidad = localidad?.ID_LOCALIDAD;
            agente.NumeroFiscal = entity.CD_CGC_CLIENTE;
            agente.NumeroLocalizacionGlobal = entity.CD_GLN;
            agente.OrdenDeCarga = entity.NU_PRIOR_CARGA;
            agente.OtroDatoFiscal = entity.NU_INSCRICAO;
            agente.PuntoDeEntrega = entity.CD_PUNTO_ENTREGA;
            agente.Ruta = this._rutaMapper.MapToObject(ruta);
            agente.Estado = this.MapEstado(entity.CD_SITUACAO ?? 0);
            agente.TelefonoPrincipal = entity.NU_TELEFONE;
            agente.TelefonoSecundario = entity.NU_FAX;
            agente.TipoAgenteDominio = this._dominioMapper.MapToObject(tipoAgente);
            agente.ValorManejoVidaUtil = entity.VL_PORCENTAJE_VIDA_UTIL;
            agente.Categoria = entity.CD_CATEGORIA;
            agente.TipoActividad = entity.TP_ATIVIDADE;
            agente.NuDvCliente = entity.NU_DV_CLIENTE;
            agente.NuDDD = entity.NU_DDD;
            agente.IdClienteFilial = entity.ID_CLIENTE_FILIAL;
            agente.Fornecedor = entity.CD_FORNECEDOR;
            agente.EmpresaConsolidada = entity.CD_EMPRESA_CONSOLIDADA;
            agente.ClienteConsolidado = entity.CD_CLIENTE_EN_CONSOLIDADO;
            agente.GrupoConsulta = entity.CD_GRUPO_CONSULTA;
            agente.SincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA);
            agente.Email = entity.DS_EMAIL;

            List<AgenteRutaPredio> rutasDefecto = new List<AgenteRutaPredio>();

            if (entity.T_CLIENTE_RUTA_PREDIO != null)
                foreach (var rutaEntity in entity.T_CLIENTE_RUTA_PREDIO)
                {
                    rutasDefecto.Add(this.MapToObject(rutaEntity));
                }

            agente.RutasPorDefecto = rutasDefecto;

            return agente;
        }

        public virtual T_CLIENTE MapToEntity(Agente agente)
        {
            if (agente == null) return null;

            if (agente.CodigoInterno == null)
            {
                agente.CodigoInterno = $"{agente.Tipo}-{agente.Codigo}";
            }

            return new T_CLIENTE
            {
                CD_AGENTE = agente.Codigo,
                CD_CLIENTE = agente.CodigoInterno,
                DS_CLIENTE = agente.Descripcion,
                DS_ENDERECO = agente.Direccion,
                CD_EMPRESA = agente.Empresa,
                TP_AGENTE = agente.Tipo,
                ND_TIPO_FISCAL = agente.TipoFiscal == null ? null : agente.TipoFiscal.Id,
                CD_CEP = agente.CodigoPostal,
                CD_CGC_CLIENTE = agente.NumeroFiscal,
                CD_GLN = agente.NumeroLocalizacionGlobal,
                NU_FAX = agente.TelefonoSecundario,
                ID_LOCALIDAD = agente.Localidad == null ? null : (long?)agente.Localidad.Id,
                CD_PUNTO_ENTREGA = agente.PuntoDeEntrega,
                CD_ROTA = agente.Ruta.Id,
                DS_ANEXO1 = agente.Anexo1,
                DS_ANEXO2 = agente.Anexo2,
                DS_ANEXO3 = agente.Anexo3,
                DS_ANEXO4 = agente.Anexo4,
                DS_BAIRRO = agente.Barrio,
                NU_INSCRICAO = agente.OtroDatoFiscal,
                NU_PRIOR_CARGA = agente.OrdenDeCarga,
                NU_TELEFONE = agente.TelefonoPrincipal,
                CD_SITUACAO = this.MapEstado(agente.Estado),
                DT_ALTERACAO = agente.FechaModificacion,
                DT_CADASTRAMENTO = agente.FechaAlta,
                DT_SITUACAO = agente.FechaSituacion,
                VL_PORCENTAJE_VIDA_UTIL = agente.ValorManejoVidaUtil,
                CD_CATEGORIA = agente.Categoria,
                TP_ATIVIDADE = agente.TipoActividad,
                NU_DV_CLIENTE = agente.NuDvCliente,
                NU_DDD = agente.NuDDD,
                ID_CLIENTE_FILIAL = agente.IdClienteFilial,
                CD_FORNECEDOR = agente.Fornecedor,
                CD_EMPRESA_CONSOLIDADA = agente.EmpresaConsolidada,
                CD_CLIENTE_EN_CONSOLIDADO = agente.ClienteConsolidado,
                CD_GRUPO_CONSULTA = agente.GrupoConsulta,
                FL_SYNC_REALIZADA = this.MapBooleanToString(agente.SincronizacionRealizada),
                DS_EMAIL = agente.Email,
            };
        }

        public virtual string MapEstadoToString(short situacion)
        {
            switch (situacion)
            {
                case SituacionDb.Activo: return "S";
                case SituacionDb.Inactivo: return "N";
            }

            return null;
        }

        public virtual bool MapEstadoShortToBoolean(short value)
        {
            return value == SituacionDb.Activo;
        }

        public virtual short MapEstadoBooleanToShort(bool value)
        {
            return (short)(value ? SituacionDb.Activo : SituacionDb.Inactivo);
        }

        public virtual EstadoAgente MapEstado(short? estado)
        {
            switch (estado)
            {
                case SituacionDb.Activo: return EstadoAgente.Activo;
                case SituacionDb.Inactivo: return EstadoAgente.Inactivo;
            }

            return EstadoAgente.Unknown;
        }

        public virtual short? MapEstado(EstadoAgente estado)
        {
            switch (estado)
            {
                case EstadoAgente.Activo: return SituacionDb.Activo;
                case EstadoAgente.Inactivo: return SituacionDb.Inactivo;
            }

            return null;
        }

        public virtual AgenteRutaPredio MapToObject(T_CLIENTE_RUTA_PREDIO entity)
        {
            if (entity == null)
                return null;

            AgenteRutaPredio rutaPredio = new AgenteRutaPredio();

            rutaPredio.CodigoInternoAgente = entity.CD_CLIENTE;
            rutaPredio.Empresa = entity.CD_EMPRESA;
            rutaPredio.Ruta = entity.CD_ROTA;
            rutaPredio.Predio = entity.NU_PREDIO;

            return rutaPredio;
        }

        public virtual T_CLIENTE_RUTA_PREDIO MapToEntity(AgenteRutaPredio entity)
        {
            if (entity == null)
                return null;

            T_CLIENTE_RUTA_PREDIO rutaPredio = new T_CLIENTE_RUTA_PREDIO()
            {
                CD_CLIENTE = entity.CodigoInternoAgente,
                CD_EMPRESA = entity.Empresa,
                CD_ROTA = entity.Ruta,
                NU_PREDIO = entity.Predio

            };

            return rutaPredio;
        }

        public virtual T_CLIENTE_DIASVALIDEZ_VENTANA MapToEntity(ClienteDiasValidezVentana entity)
        {
            if (entity == null)
                return null;

            T_CLIENTE_DIASVALIDEZ_VENTANA rutaPredio = new T_CLIENTE_DIASVALIDEZ_VENTANA()
            {
                CD_CLIENTE = entity.Cliente,
                CD_EMPRESA = entity.Empresa,
                QT_DIAS_VALIDADE_LIBERACION = entity.CantidadDiasValidezLiberacion,
                CD_VENTANA_LIBERACION = entity.VentanaLiberacion,
                DT_ADDROW = entity.FechaAlta,
                DT_UPDROW = entity.FechaModificacion

            };

            return rutaPredio;
        }

        public virtual ClienteDiasValidezVentana MapToObject(T_CLIENTE_DIASVALIDEZ_VENTANA entity)
        {
            if (entity == null)
                return null;

            ClienteDiasValidezVentana rutaPredio = new ClienteDiasValidezVentana()
            {
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                CantidadDiasValidezLiberacion = entity.QT_DIAS_VALIDADE_LIBERACION,
                VentanaLiberacion = entity.CD_VENTANA_LIBERACION,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW

            };

            return rutaPredio;
        }

        public virtual string MapTipo(AgenteTipo tipo)
        {
            switch (tipo)
            {
                case AgenteTipo.Cliente:
                    return "CLI";
                case AgenteTipo.Proveedor:
                    return "PRO";
            }

            return null;
        }

    }
}
