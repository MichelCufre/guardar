using System;
using WIS.Domain.General;

namespace WIS.Domain.Picking
{
    public class DetallePreparacion
    {
        public int NumeroPreparacion { get; set; }              //NU_PREPARACION
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Lote { get; set; }                        //NU_IDENTIFICADOR
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Ubicacion { get; set; }                   //CD_ENDERECO
        public string Pedido { get; set; }                      //NU_PEDIDO
        public string Cliente { get; set; }                     //CD_CLIENTE
        public long? Carga { get; set; }                        //NU_CARGA
        public int NumeroSecuencia { get; set; }                //NU_SEQ_PREPARACION
        public string EspecificaLote { get; set; }              //ID_ESPECIFICA_IDENTIFICADOR
        public string Agrupacion { get; set; }                  //ID_AGRUPACION
        public decimal Cantidad { get; set; }                   //QT_PRODUTO
        public decimal? CantidadPreparada { get; set; }         //QT_PREPARADO
        public decimal? CantidadPickeo { get; set; }            //QT_PICKEO
        public decimal? CantidadControlada { get; set; }        //QT_CONTROLADO
        public decimal? CantidadControl { get; set; }           //QT_CONTROL
        public int? Usuario { get; set; }                       //CD_FUNCIONARIO
        public int? NumeroContenedorSys { get; set; }           //NU_CONTENEDOR_SYS
        public DateTime? FechaPickeo { get; set; }              //DT_PICKEO
        public int? NumeroContenedorPickeo { get; set; }        //NU_CONTENEDOR_PICKEO
        public int? UsuarioPickeo { get; set; }                 //CD_FUNC_PICKEO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public DateTime? VencimientoPickeo { get; set; }        //DT_FABRICACAO_PICKEO
        public string AveriaPickeo { get; set; }                //ID_AVERIA_PICKEO
        public int? Proveedor { get; set; }                     //CD_FORNECEDOR
        public string AreaAveria { get; set; }                  //ID_AREA_AVERIA
        public bool Cancelado { get; set; }                     //FL_CANCELADO
        public string ErrorControl { get; set; }                //FL_ERROR_CONTROL
        public long? Transaccion { get; set; }                  //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }            //NU_TRANSACCION_DELETE
        public Contenedor Contenedor { get; set; }              //NU_CONTENEDOR
        public string Estado { get; set; }                      //ND_ESTADO
        public string ReferenciaEstado { get; set; }            //VL_ESTADO_REFERENCIA
        public DateTime? FechaSeparacion { get; set; }          //DT_SEPARACION
        public long? IdDetallePickingLpn { get; set; }          //ID_DET_PICKING_LPN

        #region WMS_API
        public string CanceladoId { get; set; }                 //FL_CANCELADO
        public int? NroContenedor { get; set; }                 //NU_CONTENEDOR
        public string CodigoAgente { get; set; }                //CD_AGENTE
        public string TipoAgente { get; set; }                  //TP_AGENTE
        public string ComparteContenedorPicking { get; set; }   //VL_COMPARTE_CONTENEDOR_PICKING
        public string IdExternoContenedor { get; set; }         //ID_EXTERNO_CONTENEDOR
        public string TipoContenedor { get; set; }              //TP_CONTENEDOR
        public bool ExisteContenedor { get; set; }

        public decimal? CantidadAnular { get; set; }            //PRE120
        public string UbicacionPicking { get; set; }
        public long? NroLpn { get; internal set; }
        public int? NroContenedorDestino { get; set; }
        public int? UnidadTransporte { get; internal set; }

        #endregion

        public virtual bool DebeComprobarContenedoresCompartidos()
        {
            return this.Agrupacion == WIS.Domain.DataModel.Mappers.Constants.Agrupacion.Cliente || this.Agrupacion == WIS.Domain.DataModel.Mappers.Constants.Agrupacion.Pedido;
        }

        public virtual DetallePreparacion CopiarLinea()
        {
            return new DetallePreparacion()
            {
                NumeroPreparacion = this.NumeroPreparacion,
                Producto = this.Producto,
                Faixa = this.Faixa,
                Lote = this.Lote,
                Empresa = this.Empresa,
                Ubicacion = this.Ubicacion,
                Pedido = this.Pedido,
                Cliente = this.Cliente,
                Carga = this.Carga,
                NumeroSecuencia = this.NumeroSecuencia,
                EspecificaLote = this.EspecificaLote,
                Agrupacion = this.Agrupacion,
                Cantidad = this.Cantidad,
                CantidadPreparada = this.CantidadPreparada,
                CantidadPickeo = this.CantidadPickeo,
                CantidadControlada = this.CantidadControlada,
                CantidadControl = this.CantidadControl,
                Usuario = this.Usuario,
                NumeroContenedorSys = this.NumeroContenedorSys,
                FechaPickeo = this.FechaPickeo,
                NumeroContenedorPickeo = this.NumeroContenedorPickeo,
                UsuarioPickeo = this.UsuarioPickeo,
                FechaAlta = this.FechaAlta,
                FechaModificacion = this.FechaModificacion,
                VencimientoPickeo = this.VencimientoPickeo,
                AveriaPickeo = this.AveriaPickeo,
                Proveedor = this.Proveedor,
                AreaAveria = this.AreaAveria,
                Cancelado = this.Cancelado,
                ErrorControl = this.ErrorControl,
                Transaccion = this.Transaccion,
                TransaccionDelete = this.TransaccionDelete,
                Contenedor = this.Contenedor,
                Estado = this.Estado,
                ReferenciaEstado = this.ReferenciaEstado,
                FechaSeparacion = this.FechaSeparacion,
                IdDetallePickingLpn = this.IdDetallePickingLpn,
                CanceladoId = this.CanceladoId,
                NroContenedor = this.NroContenedor,
                CodigoAgente = this.CodigoAgente,
                TipoAgente = this.TipoAgente,
                ComparteContenedorPicking = this.ComparteContenedorPicking,
                IdExternoContenedor = this.IdExternoContenedor,
                ExisteContenedor = this.ExisteContenedor,
                CantidadAnular = this.CantidadAnular
            };
        }
    }
}
