using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class Agente
    {
        public Agente()
        {
        }

        public Agente(short? rutaId, short? estadoId, string tipoFiscalId, string aceptaDevolucionId, string paisId, string subdivisionId, string municipioId)
        {
            RutaId = rutaId;
            EstadoId = estadoId;
            TipoFiscalId = tipoFiscalId;
            AceptaDevolucionId = aceptaDevolucionId;
            PaisId = paisId;
            SubdivisionId = subdivisionId;
            MunicipioId = municipioId;
        }

        public string CodigoInterno { get; set; }           //CD_CLIENTE
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string Codigo { get; set; }                  //CD_AGENTE
        public string Tipo { get; set; }                    //TP_AGENTE
        public string Descripcion { get; set; }             //DS_CLIENTE
        public string Direccion { get; set; }               //DS_ENDERECO
        public string Barrio { get; set; }                  //DS_BAIRRO
        public string CodigoPostal { get; set; }            //CD_CEP
        public string TelefonoPrincipal { get; set; }       //NU_TELEFONE
        public string TelefonoSecundario { get; set; }      //NU_FAX
        public string NumeroFiscal { get; set; }            //CD_CGC_CLIENTE
        public string OtroDatoFiscal { get; set; }          //NU_INSCRICAO
        public EstadoAgente Estado { get; set; }            //CD_SITUACAO
        public DateTime? FechaSituacion { get; set; }       //DT_SITUACAO
        public DateTime? FechaAlta { get; set; }            //DT_CADASTRAMENTO
        public DateTime? FechaModificacion { get; set; }    //DT_ALTERACAO
        public string Anexo1 { get; set; }                  //DS_ANEXO1
        public string Anexo2 { get; set; }                  //DS_ANEXO2
        public string Anexo3 { get; set; }                  //DS_ANEXO3
        public string Anexo4 { get; set; }                  //DS_ANEXO4
        public bool AceptaDevolucion { get; set; }          //FL_ACEPTA_DEVOLUCION
        public string PuntoDeEntrega { get; set; }          //CD_PUNTO_ENTREGA
        public short? OrdenDeCarga { get; set; }            //NU_PRIOR_CARGA
        public long? NumeroLocalizacionGlobal { get; set; } //CD_GLN
        public long? IdLocalidad { get; set; }              //ID_LOCALIDAD
        public decimal? ValorManejoVidaUtil { get; set; }   //VL_PORCENTAJE_VIDA_UTIL
        public string Categoria { get; set; }               //CD_CATEGORIA
        public string TipoActividad { get; set; }           //TP_ATIVIDADE
        public short? NuDvCliente { get; set; }             //NU_DV_CLIENTE
        public string NuDDD { get; set; }                   //NU_DDD
        public string IdClienteFilial { get; set; }         //ID_CLIENTE_FILIAL
        public int? Fornecedor { get; set; }                //CD_FORNECEDOR
        public int? EmpresaConsolidada { get; set; }        //CD_EMPRESA_CONSOLIDADA
        public string ClienteConsolidado { get; set; }      //CD_CLIENTE_EN_CONSOLIDADO
        public string GrupoConsulta { get; set; }           //CD_GRUPO_CONSULTA
        public bool SincronizacionRealizada { get; set; }   //FL_SYNC_REALIZADA
        public string Email { get; set; }                   //DS_EMAIL
        public long? Transaccion { get; set; }

        #region WMS_API
        public short? RutaId { get; set; }                      //CD_ROTA
        public short? EstadoId { get; set; }                    //CD_SITUACAO
        public string TipoFiscalId { get; set; }                //ND_TIPO_FISCAL
        public string AceptaDevolucionId { get; set; }          //FL_ACEPTA_DEVOLUCION
        public string SincronizacionRealizadaId { get; set; }   //FL_SYNC_REALIZADA
        public string PaisId { get; set; }                      //CD_PAIS
        public string SubdivisionId { get; set; }               //CD_SUBDIVISION
        public string MunicipioId { get; set; }                 //CD_LOCALIDAD
        #endregion

        public Ruta Ruta { get; set; }
        public PaisSubdivisionLocalidad Localidad { get; set; }
        public DominioDetalle TipoFiscal { get; set; }
        public DominioDetalle TipoAgenteDominio { get; set; }
        public List<AgenteRutaPredio> RutasPorDefecto { get; set; }

        public virtual void Enable()
        {
            this.Estado = EstadoAgente.Activo;
            this.FechaSituacion = DateTime.Now;
        }

        public virtual void Disable()
        {
            this.Estado = EstadoAgente.Inactivo;
            this.FechaSituacion = DateTime.Now;
        }
    }
}
