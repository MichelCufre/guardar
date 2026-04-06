using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class AgenteMapper : Mapper, IAgenteMapper
    {
        public AgenteMapper()
        {
        }

        public virtual List<Agente> Map(AgentesRequest request)
        {
            List<Agente> models = new List<Agente>();

            foreach (var a in request.Agentes)
            {
                Agente model = new Agente(a.Ruta, a.Estado, a.TipoFiscal, a.AceptaDevolucion, a.Pais, a.Subdivision, a.Localidad);
                model.Codigo = a.CodigoAgente;
                model.Tipo = a.Tipo;
                model.Descripcion = a.Descripcion;
                model.Anexo1 = a.Anexo1;
                model.Anexo2 = a.Anexo2;
                model.Anexo3 = a.Anexo3;
                model.Anexo4 = a.Anexo4;
                model.Barrio = a.Barrio;
                model.Direccion = a.Direccion;
                model.Empresa = request.Empresa;
                model.TelefonoPrincipal = a.TelefonoPrincipal;
                model.TelefonoSecundario = a.TelefonoSecundario;
                model.ValorManejoVidaUtil = a.ValorManejoVidaUtil;
                model.Categoria = a.Categoria;
                model.CodigoPostal = a.CodigoPostal;
                model.NumeroFiscal = a.NumeroFiscal;
                model.NumeroLocalizacionGlobal = a.NumeroLocalizacionGlobal;
                model.GrupoConsulta = a.GrupoConsulta;
                model.PuntoDeEntrega = a.PuntoDeEntrega;
                model.IdClienteFilial = a.IdClienteFilial;
                model.NuDDD = a.CaracteristicaTelefonica;
                model.OtroDatoFiscal = a.OtroDatoFiscal;
                model.OrdenDeCarga = a.OrdenDeCarga;
                model.Email = a.Email;

                models.Add(model);
            }
            return models;
        }
        public virtual AgenteResponse MapToResponse(Agente agente)
        {
            return new AgenteResponse()
            {
                CodigoAgente = agente.Codigo,
                Categoria = agente.Categoria,
                CodigoPostal = agente.CodigoPostal,
                NumeroFiscal = agente.NumeroFiscal,
                CodigoCliente = agente.CodigoInterno,
                ClienteConsolidado = agente.ClienteConsolidado,
                Empresa = agente.Empresa,
                EmpresaConsolidada = agente.EmpresaConsolidada,
                NumeroLocalizacionGlobal = agente.NumeroLocalizacionGlobal,
                GrupoConsulta = agente.GrupoConsulta,
                PuntoDeEntrega = agente.PuntoDeEntrega,
                Ruta = agente.Ruta.Id,
                Situacion = this.MapEstado(agente.Estado) ?? -1,
                Anexo1 = agente.Anexo1,
                Anexo2 = agente.Anexo2,
                Anexo3 = agente.Anexo3,
                Anexo4 = agente.Anexo4,
                Barrio = agente.Barrio,
                Descripcion = agente.Descripcion,
                Direccion = agente.Direccion,
                FechaAlta = agente.FechaAlta.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = agente.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                FechaSituacion = agente.FechaSituacion.ToString(CDateFormats.DATE_ONLY),
                AceptaDevolucion = this.MapBooleanToString(agente.AceptaDevolucion),
                IdClienteFilial = agente.IdClienteFilial,
                IdLocalidad = agente.IdLocalidad,
                TipoFiscal = agente.TipoFiscal?.Id,
                CaracteristicaTelefonica = agente.NuDDD,
                TelefonoSecundario = agente.TelefonoSecundario,
                OtroDatoFiscal = agente.OtroDatoFiscal,
                OrdenDeCarga = agente.OrdenDeCarga,
                TelefonoPrincipal = agente.TelefonoPrincipal,
                Tipo = agente.Tipo,
                TipoActividad = agente.TipoActividad,
                ValorManejoVidaUtil = agente.ValorManejoVidaUtil,
                SincronizacionRealizada = this.MapBooleanToString(agente.SincronizacionRealizada),
                Email = agente.Email,
            };
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
    }
}
