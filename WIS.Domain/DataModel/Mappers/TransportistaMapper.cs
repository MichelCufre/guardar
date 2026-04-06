using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TransportistaMapper : Mapper
    {
        public virtual Transportista MapToObject(T_TRANSPORTADORA entity)
        {
            if (entity == null)
                return null;

            return new Transportista
            {
                Id = entity.CD_TRANSPORTADORA,
                Descripcion = entity.DS_TRANSPORTADORA,
                Estado = entity.CD_SITUACAO,
                DireccionFiscal = entity.DS_ENDERECO,
                CodigoCiudad = entity.CD_UF,
                CodigoDepartamento = entity.CD_MUNICIPIO,
                FechaSituacion = entity.DT_SITUACAO,
                FechaAlta = entity.DT_CADASTRAMENTO,
                FechaModificacion = entity.DT_ALTERACAO,
                NumeroFiscal = entity.CD_CGC_TRANSP,
                OtroDatoFiscal = entity.CD_INSCRICAO_TRANSP,
                Contacto = entity.NM_CONTACTO,
                TelefonoPrincipal = entity.NU_TELEFONE,
                TelefonoSecundario = entity.NU_FAX,
            };
        }

        public virtual T_TRANSPORTADORA MapToEntity(Transportista transportista)
        {
            return new T_TRANSPORTADORA
            {
                CD_TRANSPORTADORA = transportista.Id,
                DS_TRANSPORTADORA = transportista.Descripcion,

                CD_SITUACAO = transportista.Estado,
                DS_ENDERECO = transportista.DireccionFiscal,
                CD_UF = transportista.CodigoCiudad,
                CD_MUNICIPIO = transportista.CodigoDepartamento,
                DT_SITUACAO = transportista.FechaSituacion,
                DT_CADASTRAMENTO = transportista.FechaAlta,
                DT_ALTERACAO = transportista.FechaModificacion,
                CD_CGC_TRANSP = transportista.NumeroFiscal,
                CD_INSCRICAO_TRANSP = transportista.OtroDatoFiscal,
                NM_CONTACTO = transportista.Contacto,
                NU_TELEFONE = transportista.TelefonoPrincipal,
                NU_FAX = transportista.TelefonoSecundario,
            };
        }
    }
}
