using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;
using WIS.Domain.Impresiones;
using WIS.Domain.Picking;

namespace WIS.Domain.General
{
    public class Contenedor
    {
        public int NumeroPreparacion { get; set; }                      //NU_PREPARACION
        public int Numero { get; set; }                                 //NU_CONTENEDOR
        public string TipoContenedor { get; set; }                      //TP_CONTENEDOR
        public EstadoContenedor Estado { get; set; }                    //CD_SITUACAO
        public string Ubicacion { get; set; }                           //CD_ENDERECO
        public string CodigoSubClase { get; set; }                      //CD_SUB_CLASSE
        public short? CodigoPuerta { get; set; }                        //CD_PORTA
        public int? CodigoCamion { get; set; }                          //CD_CAMION
        public DateTime? FechaPulmon { get; set; }                      //DT_PULMON
        public DateTime? FechaExpedido { get; set; }                    //DT_EXPEDIDO
        public DateTime? FechaAgregado { get; set; }                    //DT_ADDROW
        public DateTime? FechaModificado { get; set; }                  //DT_UPDROW
        public int? CodigoFuncionarioExpedicion { get; set; }           //CD_FUNCIONARIO_EXPEDICION
        public decimal? PesoReal { get; set; }                          //PS_REAL
        public decimal? Altura { get; set; }                            //VL_ALTURA
        public decimal? Largo { get; set; }                             //VL_LARGURA
        public decimal? Profundidad { get; set; }                       //VL_PROFUNDIDADE
        public string CodigoUnidadBulto { get; set; }                   //CD_UNIDAD_BULTO
        public int? CantidadBulto { get; set; }                         //QT_BULTO
        public string DescripcionContenedor { get; set; }               //DS_CONTENEDOR
        public int? CodigoCamionCongelado { get; set; }                 //CD_CAMION_CONGELADO
        public int? NumeroUnidadTransporte { get; set; }                //NU_UNIDAD_TRANSPORTE
        public string CodigoAgrupador { get; set; }                     //CD_AGRUPADOR
        public int? NumeroViaje { get; set; }                           //NU_VIAJE
        public short? CodigoCanal { get; set; }                         //CD_CANAL
        public string IdContenedorEmpaque { get; set; }                 //ID_CONTENEDOR_EMPAQUE
        public int? CamionFacturado { get; set; }                       //CD_CAMION_FACTURADO
        public string TipoControl { get; set; }                         //TP_CONTROL
        public decimal? ValorCubagem { get; set; }                      //VL_CUBAGEM
        public string Precinto1 { get; set; }                           //ID_PRECINTO_1
        public string Precinto2 { get; set; }                           //ID_PRECINTO_2
        public string Habilitado { get; set; }                          //FL_HABILITADO
        public string ValorControl { get; set; }                        //VL_CONTROL
        public long? NumeroTransaccion { get; set; }                    //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }              //NU_TRANSACCION_DELETE
        public string SegundaFase { get; set; }                         //FL_SEPARADO_DOS_FASES        
        public string IdExterno { get; set; }                           //ID_EXTERNO_CONTENEDOR
        public long? NroLpn { get; set; }                               //NU_LPN
        public long? IdExternoTracking { get; set; }                    //ID_EXTERNO_TRACKING
        public string CodigoBarras { get; set; }                        //CD_BARRAS

        #region WMS_API
        public short? EstadoId { get; set; }                            //CD_SITUACAO
        public int? Empresa { get; set; }                               //CD_EMPRESA
        public short? Ruta { get; set; }                                //CD_ROTA
        #endregion

        public Preparacion Preparacion { get; set; }

        public virtual bool PuedeAsignarseACamion()
        {
            return this.Estado == EstadoContenedor.EnPreparacion;
        }

        public virtual bool IsFacturado()
        {
            return this.CamionFacturado != null;
        }

        public virtual void PrepararFacturacion(int camion)
        {
            this.CamionFacturado = camion;
        }

        public virtual void MarcarComoNoFacturable()
        {
            this.CamionFacturado = 0;
        }
    }
}
