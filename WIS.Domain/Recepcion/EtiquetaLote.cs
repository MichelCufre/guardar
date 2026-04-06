using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Impresiones;

namespace WIS.Domain.Recepcion
{
    public class EtiquetaLote
    {
        public int Numero { get; set; }                         //NU_ETIQUETA_LOTE
        public string Cliente { get; set; }                     //CD_CLIENTE
        public string IdUbicacion { get; set; }                 //CD_ENDERECO
        public string UbicacionMovimiento { get; set; }         //CD_ENDERECO_MOVTO_PARCIAL
        public string IdUbicacionSugerida { get; set; }         //CD_ENDERECO_SUGERIDO
        public int? FuncionarioAlmacenamiento { get; set; }     //CD_FUNC_ALMACENAMIENTO
        public int? FuncionarioRecepcion { get; set; }          //CD_FUNC_RECEPCION
        public string CodigoGrupo { get; set; }                 //CD_GRUPO
        public short? CodigoPallet { get; set; }                //CD_PALLET
        public short? Estado { get; set; }                      //CD_SITUACAO
        public short? EstadoPallet { get; set; }                //CD_SITUACAO_PALLET
        public DateTime? FechaAlmacenamiento { get; set; }      //DT_ALMACENAMIENTO
        public DateTime? FechaRecepcion { get; set; }           //DT_RECEPCION
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public int NumeroAgenda { get; set; }                   //NU_AGENDA
        public string NumeroExterno { get; set; }               //NU_EXTERNO_ETIQUETA
        public int? CodigoUnidadTransporte { get; set; }        //NU_UNIDAD_TRANSPORTE
        public string TipoEtiqueta { get; set; }                //TP_ETIQUETA
        public string CodigoBarras { get; set; }                //CD_BARRAS
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }      //NU_TRANSACCION_DELETE
        public long? NroLpn { get; set; }                       //NU_LPN

        public string Predio { get; set; }

        public Agenda Agenda { get; set; }
        public List<EtiquetaLoteDetalle> Detalles { get; set; }
        public Ubicacion Ubicacion { get; set; }
        public Ubicacion UbicacionSugerida { get; set; }

        public EtiquetaLote()
        {
            Detalles = new List<EtiquetaLoteDetalle>();
        }

    }
}
