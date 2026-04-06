using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion
{
    public abstract class IngresoProduccion : IIngresoPanel
    {
        public string Id { get; set; }
        public int? Empresa { get; set; }
        public Formula Formula { get; set; }
        public int CantidadIteracionesFormula { get; set; }
        public bool GeneraPedido { get; set; }
        public short? Situacion { get; set; }
        public int? Funcionario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string NumeroProduccionOriginal { get; set; }
		public string TipoDeFlujo { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
        public long? EjecucionEntrada { get; set; }
        public string Predio { get; set; }
		public string Tipo { get; set; }
		public TipoProduccionIngreso TipoProduccion { get; set; }
        public ILinea Linea { get; set; }
        public Documento Documento { get; set; }
        public List<Pasada> Pasadas { get; set; }
		public string Anexo5 { get; set; }
		public string IdFormula { get; set; }
		public string Lote { get; set; }
		public int? PosicionEnCola { get; set; }
		public string IdEspacioProducion { get; set; }
		public string IdModalidadLote { get; set; }
		public string PermitirAutoasignarLinea { get; set; }
		public string IngresoDirectoProduccion { get; set; }
		public string IdProduccionExterno { get; set; }
		public long? NuTransaccion { get; set; }
		public string ProductoInsumoAnclaLote { get; set; }
		public string ModalidadTrabajo { get; set; }
        public DateTime? FechaInicioProduccion { get; set; }
        public DateTime? FechaFinProduccion { get; set; }
		public string IdManual { get; set; }
        public long? NroUltInterfazEjecucion { get; set; }

        #region API

        public string GeneraPedidoId { get; set; }
		public bool? LiberarPedido { get; set; }

        #endregion

        public EspacioProduccion EspacioProduccion { get; set; }

		public List<IngresoProduccionDetalleTeorico> Detalles { get; set; }

		public List<IngresoProduccionDetalleReal> Consumidos { get; set; }

		public List<IngresoProduccionDetalleSalida> Producidos { get; set; }

		public IngresoProduccionInstruccion Instrucciones { get; set; }
		public IngresoProduccion()
        {
            this.Consumidos = new List<IngresoProduccionDetalleReal>();
            this.Producidos = new List<IngresoProduccionDetalleSalida>();
            this.Pasadas = new List<Pasada>();
        }

        public abstract void CerrarProduccion();
        public abstract Pasada GetLatestPasada();
        public abstract Pasada GetCurrentPasada();
        public abstract bool IsFinalizado();
        public virtual int GetTotalPasadas()
        {
   
            return this.CantidadIteracionesFormula * this.Formula.CantidadPasadasPorFormula;
        }

		public virtual void RemoveDetalleInsumo(long id)
		{
			var detalle = GetDetalleInsumo(id);

			Consumidos.Remove(detalle);
		}
		public virtual IngresoProduccionDetalleSalida GetDetalleProductoFinal(long Id)
		{
			return Producidos.FirstOrDefault(f => f.NuPrdcIngresoSalida == Id);
		}

		public virtual IngresoProduccionDetalleReal GetDetalleInsumo(long Id)
		{
			return Consumidos.FirstOrDefault(f => f.NuPrdcIngresoReal == Id);
		}

		public virtual IngresoProduccionDetalleReal GetDetalleInsumo(int empresa, string producto, string identificador, decimal faixa)
		{
			return Consumidos.FirstOrDefault(f => f.Empresa == empresa && f.Producto == producto && f.Identificador == identificador && f.Faixa == faixa);
		}

		public virtual long GetMaximoNumeroOrdenInsumosReales()
		{
			return Consumidos.Max(o => o.NuOrden) ?? 0;
		}

		public virtual long GetMaximoNumeroOrdenProductosReales()
		{
			return Producidos.Max(o => o.NuOrden) ?? 0;
		}

		public virtual bool HayPendientesNotificar()
		{
			return Consumidos.Any(w => w.QtDesafectado - w.QtNotificado > 0) || Producidos.Any(w => w.QtProducido - w.QtNotificado > 0);
		}

		public virtual bool EsProductoEsperado(int empresa, string producto, string tipoRegistro)
		{
			return Detalles.Any(a => a.Producto == producto && a.Empresa == empresa && a.Tipo== tipoRegistro);
		}

		public virtual void UpdateDetalles()
		{
			throw new NotImplementedException();
		}

		public virtual void AddDetalleTeorico(IngresoProduccionDetalleTeorico detalle)
		{
			Detalles.Add(detalle);
		}

		public virtual void RemoveDetalleTeorico(IngresoProduccionDetalleTeorico detalle)
		{
			Detalles.Remove(detalle);
		}

		public virtual IngresoProduccionDetalleTeorico GetDetalleTeorico(int Id)
		{
			return Detalles.FirstOrDefault(f => f.Id == Id);
		}

		public virtual void AddDetalleInsumo(IngresoProduccionDetalleReal detalle)
		{
			Consumidos.Add(detalle);
		}

		public virtual void UpdateDetalleInsumo(IngresoProduccionDetalleReal detalle)
		{
			var detalleExistente = GetDetalleInsumo(detalle.NuPrdcIngresoReal);

			detalleExistente.Empresa = detalle.Empresa;
			detalleExistente.Faixa = detalle.Faixa;
			detalleExistente.Producto = detalle.Producto;
			detalleExistente.Identificador = detalle.Identificador;
			detalleExistente.DsAnexo1 = detalle.DsAnexo1;
			detalleExistente.DsAnexo2 = detalle.DsAnexo2;
			detalleExistente.DsAnexo3 = detalle.DsAnexo3;
			detalleExistente.DsAnexo4 = detalle.DsAnexo4;
			detalleExistente.DtAddrow = detalle.DtAddrow;
			detalleExistente.NuOrden = detalle.NuOrden;
			detalleExistente.NuPrdcIngreso = detalle.NuPrdcIngreso;
			detalleExistente.NuPrdcIngresoReal = detalle.NuPrdcIngresoReal;
			detalleExistente.NuTransaccion = detalle.NuTransaccion;
			detalleExistente.QtMerma = detalle.QtMerma;
			detalleExistente.QtNotificado = detalle.QtNotificado;
			detalleExistente.QtReal = detalle.QtReal;
		}
	}
}
