using System.Linq;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class InterfazEjecDataRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;

        public InterfazEjecDataRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
        }

        public virtual bool EstanConfPedido(long nuInt)
        {
            return _context.I_S_ESTAN_CONF_PEDI_PEDIDO.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanRefRecepcion(long nuInt)
        {
            return _context.I_E_ESTAN_REF_RECEPCION.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanPedidoAnulado(long nuInt)
        {
            return _context.I_S_ESTAN_PEDIDO_ANULADO.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanAjusteStock(long nuInt)
        {
            return _context.I_S_ESTAN_AJUSTE_STOCK.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanAgente(long nuInt)
        {
            return _context.I_E_ESTAN_AGENTE.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanProducto(long nuInt)
        {
            return _context.I_E_ESTAN_PRODUTO.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanPedidoSalida(long nuInt)
        {
            return _context.I_E_ESTAN_PEDIDO_SAIDA.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanCodigoBarras(long nuInt)
        {
            return _context.I_E_ESTAN_CODIGO_BARRAS.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanConvertedor(long nuInt)
        {
            return _context.I_E_ESTAN_CONVERTEDOR.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }

        public virtual bool EstanFacturaRec(long nuInt)
        {
            return _context.I_E_ESTAN_FACTURA_REC.Any(i => i.NU_INTERFAZ_EJECUCION == nuInt);
        }
    }
}