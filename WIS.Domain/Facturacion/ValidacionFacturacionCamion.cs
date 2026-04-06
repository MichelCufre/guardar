using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Exceptions;

namespace WIS.Domain.Facturacion
{
    public class ValidacionFacturacionCamion : IValidacionFacturacionCamion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IConfiguracionValidacionFacturacion _configuracion;

        public ValidacionFacturacionCamion(IUnitOfWork uow, IConfiguracionValidacionFacturacion configuracion)
        {
            this._uow = uow;
            this._configuracion = configuracion;
        }

        public virtual List<ValidacionCamionResultado> Validar(Camion camion)
        {
            if (camion.IsFacturacionEnProceso())
                throw new ValidationFailedException("General_Sec0_Error_Er117_FacturacionEnProceso");

            if (camion.IsFacturado())
                throw new ValidationFailedException("General_Sec0_Error_Er118_CamionFacturado");

            this._configuracion.CargarValidaciones();

            List<long> cargas = camion.Cargas.Select(d => d.Carga).ToList();

            List<DetallePreparacion> lineasPicking = this._uow.PreparacionRepository.GetDetallePreparacionByCarga(cargas);

            if (lineasPicking.Count == 0)
                throw new ValidationFailedException("EXP040_Sec0_Error_FacturarCamionVacio");

            var primeraAnulacion = this._uow.PreparacionRepository.GetPrimeraAnulacionPendienteCamion(camion.Id);

            if (primeraAnulacion != null)
            {
                throw new ValidationFailedException("WEXP010_Sec0_Error_CargaConAnulacionPendiente", new string[] {
                    Convert.ToString(primeraAnulacion.Carga),
                    Convert.ToString(primeraAnulacion.Preparacion),
                    Convert.ToString(primeraAnulacion.Contenedor),
                    primeraAnulacion.Pedido,
                    primeraAnulacion.Producto
                });
            }

            var pedidos = lineasPicking.GroupBy(d => new { d.Empresa, d.Cliente, d.Pedido, d.NumeroPreparacion, NumeroContenedor = d.Contenedor.Numero }).Select(d => d.Key).ToList();

            string grupoExpedicion = null;

            foreach (var linea in pedidos)
            {
                Pedido pedido = this._uow.PedidoRepository.GetPedido(linea.Empresa, linea.Cliente, linea.Pedido);

                if (!this._uow.ContenedorRepository.IsContenedorEmpaque(linea.NumeroContenedor, linea.NumeroPreparacion) && pedido.ConfiguracionExpedicion.DebeEmpaquetarContenedor)
                    throw new ValidationFailedException("EXP040_Sec0_Error_ContenedorRequiereEmpaque");

                if (this._configuracion.IsUnicoGrupoExpedicionRequerido())
                {
                    if (grupoExpedicion == null)
                        grupoExpedicion = pedido.ConfiguracionExpedicion.CodigoGrupoExpedicion;
                    else if (grupoExpedicion != pedido.ConfiguracionExpedicion.CodigoGrupoExpedicion)
                        throw new ValidationFailedException("WEXP040_Sec0_Error_Er013_NoFactPedAsigDifGrupoExp");
                }

                List<IFacturacionValidacion> validacionesEvaluar = this._configuracion.GetValidacionesEvaluar(pedido);

                foreach (var validacion in validacionesEvaluar)
                {
                    validacion.Validate(camion, pedido);
                }
            }

            var contenedoresConProblemas = _uow.CamionRepository.GetContenedoresConProblemas(camion.Id);
            var contedorPreparacion = lineasPicking
                .GroupBy(d => new { d.NroContenedor, d.NumeroPreparacion })
                .Select(d => d.Key)
                .ToList();

            foreach (var i in contedorPreparacion)
            {
                if (contenedoresConProblemas.Any(w => w[0] == i.NumeroPreparacion && w[1] == i.NroContenedor))
                {
                    var cont = _uow.ContenedorRepository.GetContenedor(i.NumeroPreparacion, i.NroContenedor.Value);
                    throw new ValidationFailedException("EXP040_Sec0_Error_FacturarContenedorConProblemas", new string[] { cont.TipoContenedor, cont.IdExterno });
                }
            }

            List<IFacturacionValidacion> validaciones = this._configuracion.GetValidaciones();

            var resultado = new List<ValidacionCamionResultado>();

            if (!ValidarCantidades(lineasPicking, out ValidacionCamionResultado pedidosConProblemas))
                resultado.Add(pedidosConProblemas);

            if (!ValidarPreparaciones(lineasPicking, out ValidacionCamionResultado prepSinFinalizar))
                resultado.Add(prepSinFinalizar);

            foreach (var validacion in validaciones)
            {
                if (!validacion.IsValid())
                    resultado.Add(validacion.GetResult());
            }

            return resultado;
        }

        public virtual List<Contenedor> GetContenedoresConProblemas(List<DetallePreparacion> lineasPicking)
        {
            var contenedoresConProblemas = new List<Contenedor>();

            var grupos = lineasPicking.Where(w => w.Contenedor != null).GroupBy(d => new { d.Contenedor }).ToList();

            foreach (var grupo in grupos)
            {
                //TODO: Ver si incluir control de asignacion

                var preparacion = grupo.FirstOrDefault(d => d.Agrupacion == Agrupacion.Pedido || d.Agrupacion == Agrupacion.Cliente);

                if (preparacion != null && this._uow.CamionRepository.ExistenContenedoresCompartidos(grupo.Key.Contenedor.NumeroPreparacion, grupo.Key.Contenedor.Numero))
                    contenedoresConProblemas.Add(grupo.Key.Contenedor);
            }

            return contenedoresConProblemas;
        }

        public virtual bool ValidarCantidades(List<DetallePreparacion> lineasPicking, out ValidacionCamionResultado result)
        {
            string msg = "Pedidos con cantidad preparada mayor a la cantidad disponible:";
            var datos = new List<string>();

            var picking = lineasPicking.GroupBy(d => new { d.Empresa, d.Cliente, d.Pedido, d.Producto, d.Lote, d.Faixa, d.EspecificaLote })
            .Select(w => new { w.Key.Empresa, w.Key.Cliente, w.Key.Pedido, w.Key.Producto, w.Key.Lote, w.Key.Faixa, w.Key.EspecificaLote, CantidadPreparada = w.Sum(s => (s.CantidadPreparada ?? 0)) }).ToList();

            foreach (var pick in picking)
            {
                var detPedido = this._uow.PedidoRepository.GetDetallePedido(pick.Pedido, pick.Empresa, pick.Cliente, pick.Producto, pick.Lote, pick.Faixa, pick.EspecificaLote);
                var qt = pick.CantidadPreparada;// + detPedido.CantidadFacturada;

                if ((qt > detPedido.CantidadLiberada) || (qt > (detPedido.Cantidad - detPedido.CantidadAnulada)))
                    datos.Add($"Pedido: {pick.Pedido} - Cliente:{pick.Cliente} - Empresa: {pick.Empresa}");
            }

            result = new ValidacionCamionResultado(msg, datos);

            if (datos.Any())
                return false;
            else
                return true;
        }

        public virtual bool ValidarPreparaciones(List<DetallePreparacion> lineasPicking, out ValidacionCamionResultado result)
        {
            string msg = "Las siguientes preparaciones no estan finalizadas:";
            var datos = new List<string>();

            var preps = lineasPicking.GroupBy(p => p.NumeroPreparacion).Distinct().Select(p => p.Key).ToList();
            var prepNofinalizadas = _uow.PreparacionRepository.GetPrepManualSinFinalizar(preps);

            foreach (var pick in prepNofinalizadas)
            {
                datos.Add($"Nro: {pick.Id} - {pick.Descripcion}");
            }
            result = new ValidacionCamionResultado(msg, datos);

            if (datos.Any())
                return false;
            else
                return true;
        }
    }
}
