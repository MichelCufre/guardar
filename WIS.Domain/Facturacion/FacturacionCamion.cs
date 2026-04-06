using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Exceptions;

namespace WIS.Domain.Facturacion
{
    //TODO: Redisenar todo esto, es un asco, sentarse en GRUPO y pensar una verdadera solucion
    public class FacturacionCamion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IValidacionFacturacionCamion _validacion;
        protected readonly Camion _camion;

        public FacturacionCamion(IUnitOfWork uow, IValidacionFacturacionCamion validacion, Camion camion)
        {
            this._uow = uow;
            this._validacion = validacion;
            this._camion = camion;
        }

        public virtual List<ValidacionCamionResultado> Facturar()
        {
            if (_camion.IsControlContenedoresHabilitado && _uow.PreparacionRepository.AnyContenedorSinControl(_camion, out int cantCont))
                throw new ValidationFailedException("General_Sec0_Error_ContenedoreSinControlar", new string[] { cantCont.ToString() });
            else if (_uow.PreparacionRepository.AnyContenedorSinFinalizarControl(_camion, out cantCont))
                throw new ValidationFailedException("General_Sec0_Error_ContenedoreSinFinalizarControl", new string[] { cantCont.ToString() });

            List<ValidacionCamionResultado> resultadoValidacion = this._validacion.Validar(this._camion);

            if (resultadoValidacion.Any())
                return resultadoValidacion;

            if (this._uow.PreparacionRepository.AnyContenedorNoFacturado(this._camion))
            {
                this.FacturarContenedores();
                this.GenerarCargasPredefinidas();
            }
            else
            {
                this._camion.MarcarComoNoFacturable();
                this._uow.CamionRepository.UpdateCamion(this._camion);
            }

            return resultadoValidacion;
        }

        public virtual void FacturarContenedores()
        {
            List<Contenedor> contenedores = this._uow.PreparacionRepository.GetContenedores(this._camion);

            foreach (var contenedor in contenedores)
            {
                if (this._uow.PreparacionRepository.IsFacturacionRequeridaContenedor(contenedor))
                {
                    this._camion.PrepararFacturacion();
                    contenedor.PrepararFacturacion(this._camion.Id);
                    this._uow.ContenedorRepository.UpdateContenedor(contenedor);
                    continue;
                }

                contenedor.MarcarComoNoFacturable();

                this._uow.ContenedorRepository.UpdateContenedor(contenedor);
            }

            this._uow.CamionRepository.UpdateCamion(this._camion);
        }

        public virtual void GenerarCargasPredefinidas()
        {
            //Nota: Esta lista de pedidos se carga automaticamente con los nuevos número de cargas, no refleja el dato real existente.
            var pedidos = _uow.PedidoRepository.GetPedidosNuevasCargas(_camion.Id);

            foreach (var p in pedidos)
            {
                var carga = new Carga
                {
                    Id = (long)p.NuCarga,
                    Ruta = (short)p.Ruta,
                    Descripcion = $"Generada por la facturación de egreso para el Pedido {p.Id} - Cliente: {p.Cliente} - Empresa: {p.Empresa}",
                    Preparacion = null,
                    FechaAlta = DateTime.Now
                };

                p.Transaccion = _uow.GetTransactionNumber();

                _uow.CargaRepository.AddCarga(carga, false);
                _uow.PedidoRepository.UpdatePedido(p);
            }

        }
    }
}
