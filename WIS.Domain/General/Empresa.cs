using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class Empresa
    {
        public Empresa()
        {
        }

        public Empresa(short estadoId, string tipoFiscalId, string paisId, string subdivisionId, string municipioId)
        {
            EstadoId = estadoId;
            TipoFiscalId = tipoFiscalId;
            PaisId = paisId;
            SubdivisionId = subdivisionId;
            MunicipioId = municipioId;
        }

        public int Id { get; set; }                             //CD_EMPRESA
        public string Nombre { get; set; }                      //NM_EMPRESA
        public EstadoEmpresa Estado { get; set; }               //CD_SITUACAO
        public DateTime FechaInsercion { get; set; }            //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public string Direccion { get; set; }                   //DS_ENDERECO
        public string Telefono { get; set; }                    //NU_TELEFONE
        public DominioDetalle TipoFiscal { get; set; }          //ND_TIPO_FISCAL
        public DominioDetalle TipoNotificacion { get; set; }    //TP_NOTIFICACION
        public string PayloadUrl { get; set; }                  //VL_PAYLOAD_URL
        public bool IsLocked { get; set; }                      //FL_LOCKED
        public string NumeroFiscal { get; set; }                //CD_CGC_EMPRESA
        public string CodigoPostal { get; set; }                //DS_CP_POSTAL
        public string CdClienteArmadoKit { get; set; }          //CD_CLIENTE_ARMADO_KIT
        public string Anexo1 { get; set; }                      //DS_ANEXO1
        public string Anexo2 { get; set; }                      //DS_ANEXO2
        public string Anexo3 { get; set; }                      //DS_ANEXO3
        public string Anexo4 { get; set; }                      //DS_ANEXO4
        public long? IdLocalidad { get; set; }                  //ID_LOCALIDAD
        public short? cdTipoDeAlmacenajeYSeguro { get; set; }   //TP_ALMACENAJE_Y_SEGURO
        public decimal? ValorMinimoStock { get; set; }          //IM_MINIMO_STOCK
        public int? EmpresaConsolidado { get; set; }            //CD_EMPRESA_DE_CONSOLIDADO
        public int? ProveedorDevolucion { get; set; }           //CD_FORN_DEVOLUCAO
        public int? ListaPrecio { get; set; }                   //CD_LISTA_PRECIO
        public string FG_QUEBRA_PEDIDO { get; set; }            //FG_QUEBRA_PEDIDO - Definir nombre
        public string IdDAP { get; set; }                       //ID_DAP
        public string IdOperativo { get; set; }                 //ID_OPERATIVO
        public string IdUnidadFactura { get; set; }             //ID_UND_FACT_EMPRESA
        public short? CantidadDiasPeriodo { get; set; }         //QT_DIAS_POR_PERIODO
        public decimal? ValorPallet { get; set; }               //VL_POS_PALETE
        public decimal? ValorPalletDia { get; set; }            //VL_POS_PALETE_DIA

        public bool IsNotifiedByWebhook
        {
            get
            {
                var tipoNotificacion = TipoNotificacion?.Id ?? TipoNotificacionId;
                return tipoNotificacion == CodigoDominioDb.TipoNotificacionWebhook;
            }
        }

        #region WMS_API

        public short EstadoId { get; set; }             //CD_SITUACAO
        public string TipoFiscalId { get; set; }        //ND_TIPO_FISCAL
        public string TipoNotificacionId { get; set; }  //TP_NOTIFICACION
        public string PaisId { get; set; }              //CD_PAIS
        public string SubdivisionId { get; set; }       //CD_SUBDIVISION
        public string MunicipioId { get; set; }         //CD_LOCALIDAD
        public string IsLockedId { get; set; }          //FL_LOCKED

        #endregion

        public Agente ClienteArmadoKit { get; set; }
        public PaisSubdivisionLocalidad Localidad { get; set; }
        public TipoDeAlmacenajeYSeguro TipoDeAlmacenajeYSeguro { get; set; }
        public List<Producto> Productos { get; set; }
        public List<Ubicacion> Ubicaciones { get; set; }

        public virtual void Enable()
        {
            this.Estado = EstadoEmpresa.Activo;
        }

        public virtual void Disable()
        {
            this.Estado = EstadoEmpresa.Inactivo;
        }

    }
}
