using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;

namespace WIS.Domain.Services
{
    public class AnularPickingPedidoPendienteAutomatismoContext : ServiceContext
    {
        protected int _userId = 0;

        protected List<AnularPickingPedidoPendienteAutomatismo> _detalles = new List<AnularPickingPedidoPendienteAutomatismo>();
        protected HashSet<string> _predios = new HashSet<string>();
        protected Dictionary<string, Producto> _productos = new Dictionary<string, Producto>();
        protected Dictionary<string, Agente> _cliente = new Dictionary<string, Agente>();
        protected List<Pedido> _pedidos = new List<Pedido>();
        protected List<DetallePreparacion> _detallesPreparacion = new List<DetallePreparacion>();
        protected List<AnulacionPreparacion> _anulacionesPreparacionPendiente = new List<AnulacionPreparacion>();
        protected List<int> _empresasDocumentales = new List<int>();

        public AnularPickingPedidoPendienteAutomatismoContext(IUnitOfWork uow,
            List<AnularPickingPedidoPendienteAutomatismo> detalles,
            int userId, int empresa) : base(uow, userId ,empresa)
        {
            _userId = userId;
            _detalles = detalles;
        }

        public virtual bool ExistePredio(string predio)
        {
            return _predios.Contains(predio);
        }

        public async override Task Load()
        {
            var keysPedido = new Dictionary<string, Pedido>();
            var keysCliente = new Dictionary<string, Agente>();
            var keysPreparacionAnulacion = new Dictionary<int, AnulacionPreparacion>();
            var keysPreparacion = new Dictionary<string, DetallePreparacion>();

            await base.Load();

            _empresasDocumentales = _uow.EmpresaRepository.GetEmpresasDocumentales().ToList();

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(_userId))
            {
                _predios.Add(p.Numero);
            }

            foreach (var p in _detalles)
            {
                var keyCliente = $"{p.CodigoAgente}.{p.TipoAgente}.{p.Empresa}";
                if (!keysCliente.ContainsKey(keyCliente))
                    keysCliente[keyCliente] = new Agente() { Codigo = p.CodigoAgente, Tipo = p.TipoAgente, Empresa = p.Empresa };
            }

            foreach (var p in _uow.AgenteRepository.GetAgentes(keysCliente.Values))
            {
                var keyCliente = $"{p.Codigo}.{p.Tipo}.{p.Empresa}";
                _cliente[keyCliente] = p;
            }

            foreach (var det in _detalles)
            {
                var cliente = "";
                var agente = GetAgente(det.Empresa, det.CodigoAgente, det.TipoAgente);

                if (agente != null)
                {
                    cliente = agente.Codigo;
                }

                var keyPedido = $"{det.Pedido}.{cliente}.{det.Empresa}";

                if (!keysPedido.ContainsKey(keyPedido))
                    keysPedido[keyPedido] = new Pedido() { Id = det.Pedido, Cliente = cliente, Empresa = det.Empresa };

                var keyPreparacion = $"{det.Preparacion}.{det.Pedido}.{cliente}.{det.Empresa}";

                if (!keysPreparacion.ContainsKey(keyPreparacion))
                    keysPreparacion[keyPreparacion] = new DetallePreparacion() { NumeroPreparacion = det.Preparacion, Pedido = det.Pedido, Cliente = cliente, Empresa = det.Empresa };

                var preparacionAnulacion = det.Preparacion;

                if (!keysPreparacionAnulacion.ContainsKey(preparacionAnulacion))
                    keysPreparacionAnulacion[preparacionAnulacion] = new AnulacionPreparacion() { Preparacion = det.Preparacion };
            }

            foreach (var p in _uow.PedidoRepository.GetPedidos(keysPedido.Values))
            {
                _pedidos.Add(p);
            }

            foreach (var d in _uow.PreparacionRepository.GetDetallesPreparacionPedidos(keysPreparacion.Values))
            {
                _detallesPreparacion.Add(d);
            }

            foreach (var a in _uow.PreparacionRepository.GetAnulacionPreparacionPendiente(keysPreparacionAnulacion.Values))
            {
                _anulacionesPreparacionPendiente.Add(a);
            }
        }

        public virtual Agente GetAgente(int empresa, string codigo, string tipo)
        {
            var keyCliente = $"{codigo}.{tipo}.{empresa}";
            return _cliente.GetValueOrDefault(keyCliente, null);
        }

        public virtual Pedido GetPedido(string pedido, int empresa, string cliente)
        {
            return _pedidos.FirstOrDefault(x => x.Id == pedido && x.Empresa == empresa && x.Cliente == cliente);
        }

        public virtual bool AnyPreparacionPedido(int preparacion, string pedido, int empresa, string cliente)
        {
            return _detallesPreparacion
                .Any(x => x.NumeroPreparacion == preparacion
                    && x.Pedido == pedido
                    && x.Empresa == empresa
                    && x.Cliente == cliente);
        }

        public virtual List<DetallePreparacion> GetDetallesPreparacionPedido(int preparacion, string pedido, int empresa, string cliente, string estadoPicking)
        {
            return _detallesPreparacion
                .Where(x => x.NumeroPreparacion == preparacion
                    && x.Pedido == pedido
                    && x.Empresa == empresa
                    && x.Cliente == cliente
                    && x.Estado == estadoPicking)
                .ToList();
        }

        public virtual bool AnyAnulacionPreparacionPendiente(int preparacion)
        {
            var estados = new List<string> { EstadoAnulacion.EnProceso, EstadoAnulacion.AnulacionPendiente };
            return _anulacionesPreparacionPendiente.Any(x => x.Preparacion == preparacion && estados.Contains(x.Estado));
        }

        public virtual bool IsEmpresaDocumental(int empresa)
        {
            return _empresasDocumentales.Any(x => x == empresa);
        }
    }
}
