using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Exceptions;

namespace WIS.Domain.Produccion
{
    public class FormulaEnsamblaje
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IngresoWhiteBox _ingreso;
        protected readonly Pasada _pasada;

        protected List<IdentificadorConsumir> IdentificadoresEntrada { get; set; }
        protected List<IdentificadorProducir> IdentificadoresSalida { get; set; }
        protected EntityChanges<LineaConsumida> LineasConsumidasChanges { get; set; }
        protected EntityChanges<LineaProducida> LineasProducidasChanges { get; set; }
        protected EntityChanges<IdentificadorConsumir> IdentificadorConsumirChanges { get; set; }
        protected EntityChanges<IdentificadorProducir> IdentificadorProducirChanges { get; set; }

        public FormulaEnsamblaje(IUnitOfWork uow, IngresoWhiteBox ingreso, Pasada pasada)
        {
            this._uow = uow;
            this._ingreso = ingreso;
            this._pasada = pasada;

            this.IdentificadoresEntrada = new List<IdentificadorConsumir>();
            this.IdentificadoresSalida = new List<IdentificadorProducir>();

            this.LineasConsumidasChanges = new EntityChanges<LineaConsumida>();
            this.LineasProducidasChanges = new EntityChanges<LineaProducida>();
            this.IdentificadorConsumirChanges = new EntityChanges<IdentificadorConsumir>();
            this.IdentificadorProducirChanges = new EntityChanges<IdentificadorProducir>();
        }

        public virtual void Ensamblar()
        {
            List<string> productos = this._ingreso.Formula.Entrada.Select(d => d.Producto).ToList();
            List<string> productosSalida = this._ingreso.Formula.Salida.Select(d => d.Producto).ToList();

            this.IdentificadoresEntrada = this._uow.IdentificadorConsumirRepository.GetIdentificadores(this._ingreso.Empresa, productos, this._ingreso.Linea.UbicacionEntrada);
            this.IdentificadoresSalida = this._uow.IdentificadorProducirRepository.GetIdentificadores(this._ingreso.Empresa, productosSalida, this._ingreso.Linea.UbicacionSalida);
                        
            this.EnsamblarEntrada();
            this.EnsamblarSalida();

            this.PersistChanges();
        }

        public virtual void EnsamblarEntrada()
        {
            List<FormulaEntrada> entradas = this._ingreso.Formula.GetEntradasByPasada(this._pasada.Numero);

            foreach(var entrada in entradas)
            {
                decimal cantidadConsumir = entrada.CantidadConsumir;

                foreach(var identificador in this.IdentificadoresEntrada.Where(d => d.Empresa == entrada.Empresa && d.Faixa == entrada.Faixa && d.Producto == entrada.Producto).OrderBy(d => d.Orden))
                {
                    cantidadConsumir -= this.UpdateLineasConsumidasAndGetStock(entrada, identificador);

                    if (cantidadConsumir == 0)
                        break;
                }

                if (cantidadConsumir > 0)
                    throw new InvalidOperationException("PRD171_form1_error_SinStockConsumo");
            }
        }

        public virtual void EnsamblarSalida()
        {
            List<FormulaSalida> salidas = this._ingreso.Formula.GetSalidasByPasada(this._pasada.Numero);

            foreach (var salida in salidas)
            {
                decimal cantidadProducida = 0;
                decimal cantidadPendiente = 0;
                decimal cantidadDescontar = 0;

                Producto producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(salida.Empresa, salida.Producto);

                foreach (var identificador in this.IdentificadoresSalida.Where(d => d.Empresa == salida.Empresa && d.Faixa == salida.Faixa && d.Producto == salida.Producto).OrderBy(d => d.Orden))
                {
                    cantidadPendiente = salida.CantidadProducir - cantidadProducida;
                    cantidadDescontar = identificador.Stock > cantidadPendiente ? cantidadPendiente : identificador.Stock;                    
                    cantidadProducida += this.UpdateLineasProducidasAndGetCantidadDescontar(salida, identificador, producto, cantidadDescontar);

                    if (cantidadProducida == salida.CantidadProducir)
                        break;
                }

                if (cantidadProducida < salida.CantidadProducir)
                    this.UpdateLineasProducidasSinLote(salida, producto, cantidadProducida, cantidadPendiente);
            }
        }

        public virtual void PersistChanges()
        {
            this.PersistChangesLineasConsumidas();
            this.PersistChangesLineasProducidas();
            this.PersistChangesIdentificadorConsumir();
            this.PersistChangesIdentificadorProducir();
        }

        public virtual void PersistChangesLineasConsumidas()
        {
            foreach(var linea in this.LineasConsumidasChanges.AddedRecords)
            {
                this._uow.ProduccionRepository.AddConsumido(this._ingreso, linea);
            }
        }

        public virtual void PersistChangesLineasProducidas()
        {
            foreach (var linea in this.LineasProducidasChanges.AddedRecords)
            {
                this._uow.ProduccionRepository.AddProducido(this._ingreso, linea);
            }
        }

        public virtual void PersistChangesIdentificadorConsumir()
        {
            foreach (var linea in this.IdentificadorConsumirChanges.UpdatedRecords)
            {
                this._uow.IdentificadorConsumirRepository.UpdateIdentificador(linea);
            }

            foreach (var linea in this.IdentificadorConsumirChanges.DeletedRecords)
            {
                this._uow.IdentificadorConsumirRepository.DeleteIdentificador(linea);
            }
        }

        public virtual void PersistChangesIdentificadorProducir()
        {
            foreach (var linea in this.IdentificadorProducirChanges.UpdatedRecords)
            {
                this._uow.IdentificadorProducirRepository.UpdateIdentificador(linea);
            }

            foreach (var linea in this.IdentificadorProducirChanges.DeletedRecords)
            {
                this._uow.IdentificadorProducirRepository.DeleteIdentificador(linea);
            }
        }

        public virtual decimal UpdateLineasConsumidasAndGetStock(FormulaEntrada entrada, IdentificadorConsumir identificador)
        {
            decimal stock = identificador.Stock > entrada.CantidadConsumir ? entrada.CantidadConsumir : identificador.Stock;
            LineaConsumida linea = this.GetLineaConsumida(entrada.Producto, entrada.Empresa, entrada.Faixa, identificador.Identificador);

            linea.Cantidad += stock;

            if (identificador.Stock < stock)
                throw new InvalidOperationException("PRD171_form1_error_IdentificadorEntradaInsuficiente");

            identificador.Stock -= stock;

            if (identificador.Stock == 0)
                this.IdentificadorConsumirChanges.DeletedRecords.Add(identificador);
            else
                this.IdentificadorConsumirChanges.UpdatedRecords.Add(identificador);

            return stock;
        }

        public virtual decimal UpdateLineasProducidasAndGetCantidadDescontar(FormulaSalida salida, IdentificadorProducir identificador, Producto producto, decimal cantidadDescontar)
        {
            DateTime? vencimiento = null;
            
            if(producto.IsFefo() || producto.IsFifo())
                vencimiento = identificador.Vencimiento ?? DateTime.Now;

            LineaProducida linea = this.GetLineaProducida(salida.Producto, salida.Empresa, salida.Faixa, identificador.Identificador, vencimiento);

            linea.Cantidad += cantidadDescontar;

            this.LineasProducidasChanges.UpdatedRecords.Add(linea);

            if (identificador.Stock < cantidadDescontar)
                throw new ValidationFailedException("PRD171_form1_error_IdentificadorSalidaInsuficiente", new string[] { identificador.Identificador });

            identificador.Stock -= cantidadDescontar;

            if (identificador.Stock == 0)
                this.IdentificadorProducirChanges.DeletedRecords.Add(identificador);
            else
                this.IdentificadorProducirChanges.UpdatedRecords.Add(identificador);

            return cantidadDescontar;
        }

        public virtual void UpdateLineasProducidasSinLote(FormulaSalida salida, Producto producto, decimal cantidadProducida, decimal cantidadPendiente)
        {
            DateTime? vencimientoSinLote = null;

            string identificadorSinLote = ManejoIdentificadorDb.IdentificadorProducto;

            if (producto.IsFefo() || producto.IsFifo())
                vencimientoSinLote = DateTime.Now;

            if (producto.IsIdentifiedByLote() || producto.IsIdentifiedBySerie())
                identificadorSinLote = this._ingreso.Id;

            cantidadPendiente = salida.CantidadProducir - cantidadProducida;

            LineaProducida linea = this.GetLineaProducida(salida.Producto, salida.Empresa, salida.Faixa, identificadorSinLote, vencimientoSinLote);

            linea.Cantidad += cantidadPendiente;

            this.LineasProducidasChanges.UpdatedRecords.Add(linea);
        }

        public virtual LineaConsumida GetLineaConsumida(string producto, int empresa, decimal faixa, string identificador)
        {
            //var linea = this._ingreso.Consumidos.Where(d => d.Iteracion == this._pasada.NumeroFormula && d.Pasada == this._pasada.Numero
            //            && d.Producto == producto && d.Empresa == empresa && d.Faixa == faixa && d.Identificador == identificador).FirstOrDefault();

            //if (linea == null)
            //{
            //    linea = new LineaConsumida
            //    {
            //        Iteracion = this._pasada.NumeroFormula,
            //        Pasada = this._pasada.Numero,
            //        Producto = producto,
            //        Faixa = faixa,
            //        Empresa = empresa,
            //        Identificador = identificador,
            //        FechaAlta = DateTime.Now,
            //        Cantidad = 0
            //    };

            //    this._ingreso.Consumidos.Add(linea);

            //    this.LineasConsumidasChanges.AddedRecords.Add(linea);
            //}

            //return linea;

            return new LineaConsumida();

		}

        public virtual LineaProducida GetLineaProducida(string producto, int empresa, decimal faixa, string identificador, DateTime? vencimiento)
        {
            //var linea = this._ingreso.Producidos
            //    .Where(d => d.Iteracion == this._pasada.NumeroFormula 
            //        && d.Pasada == this._pasada.Numero
            //        && d.Producto == producto 
            //        && d.Empresa == empresa 
            //        && d.Faixa == faixa 
            //        && d.Identificador == identificador)
            //    .FirstOrDefault();

            //if (linea == null)
            //{
            //    linea = new LineaProducida
            //    {
            //        Iteracion = this._pasada.NumeroFormula,
            //        Pasada = this._pasada.Numero,
            //        Producto = producto,
            //        Faixa = faixa,
            //        Empresa = empresa,
            //        Identificador = identificador,
            //        FechaAlta = DateTime.Now,
            //        Vencimiento = vencimiento,
            //        Cantidad = 0
            //    };

            //    this._ingreso.Producidos.Add(linea);
            //    this.LineasProducidasChanges.AddedRecords.Add(linea);
            //}

            //return linea;

            return new LineaProducida();

		}
    }
}
