using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;

namespace WIS.Domain.StockEntities
{
    public class RegistroLPN
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroLPN(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        public virtual Lpn RegistrarLPN(Lpn nuevoLPN)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.ManejoLpnRepository.AddLPN(nuevoLPN);
                
                LpnTipo tipoLpn = _uow.ManejoLpnRepository.GetTipoLpn(nuevoLPN.Tipo);
                LpnBarras lpnBarra = new LpnBarras();
                
                lpnBarra.NumeroLpn = nuevoLPN.NumeroLPN;
                lpnBarra.Tipo = BarcodeDb.TIPO_LPN_CB;

                var codigoBarras = nuevoLPN.IdExterno.Substring(tipoLpn.Prefijo.Length).ToString().PadLeft(15, '0');
                int sumaCaracteres = 0;

				foreach (var digito in codigoBarras)
				{                    
                   sumaCaracteres += int.Parse(digito.ToString());
                }

                var mod = sumaCaracteres % DefaultDb.ModuloDigitoVerificacion;

                lpnBarra.CodigoBarras = tipoLpn.Prefijo + codigoBarras + mod.ToString();
                lpnBarra.Orden = 0; 
                this._uow.ManejoLpnRepository.AddLPNBarras(lpnBarra);
                
                return nuevoLPN;
            }
        }

        public virtual LpnAtributo RegistrarAtributoAsociado(LpnAtributo atributoAsociado)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.ManejoLpnRepository.AddAtributoAsociado(atributoAsociado);

                return atributoAsociado;
            }
        }

        public virtual LpnTipo UpdateTipoLpn (LpnTipo tipo)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {
                this._uow.ManejoLpnRepository.UpdateTipoLpn(tipo);

                return tipo;
            }
        }

    }
}
