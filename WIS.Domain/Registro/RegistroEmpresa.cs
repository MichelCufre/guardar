using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using NLog;
using NLog.LayoutRenderers;
using WIS.Domain.Security;
using WIS.Domain.Security.Enums;
using WIS.Domain.DataModel.Mappers;

namespace WIS.Domain.Registro
{
    public class RegistroEmpresa
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _userId;
        protected readonly string _aplicacion;

        public RegistroEmpresa(IUnitOfWork uow, int userId, string aplicacion)
        {
            this._uow = uow;
            this._userId = userId;
            this._aplicacion = aplicacion;
        }

        public virtual Empresa RegistrarEmpresa(Empresa empresa)
        {
            return RegistrarEmpresa(empresa, null);
        }

        public virtual Empresa RegistrarEmpresa(Empresa empresa, string secret)
        {
            empresa.CdClienteArmadoKit = AgentesPorDefectoEmpresaDb.ClienteArmadoKitCodigo;
            empresa.FechaInsercion = DateTime.Now;
            empresa.FechaModificacion = DateTime.Now;

            var secretInfoCompleta = GetSecretInfoCompleta(secret);

            _uow.EmpresaRepository.AddEmpresa(empresa, secretInfoCompleta);

            _uow.DominioRepository.AddDetalleDominioEmpresa(empresa.Id);

            _uow.SecurityRepository.AsignarEmpresaAUsuario(this._userId, empresa.Id);

            _uow.SecurityRepository.AsignarEmpresaAUsuariosConPermiso(this._userId, empresa.Id);

            CrearClienteMuestraDefecto(empresa);
            CrearProveedorMuestraDefecto(empresa);
            CrearClienteEnsambladoKit(empresa);
            AsociarTiposDeRecepcionYReportesPorDefecto(empresa);
            AgregarParametrosInterfaces(empresa);

            return empresa;
        }

        public virtual Empresa ActualizarEmpresa(Empresa empresa)
        {
            return ActualizarEmpresa(empresa, null);
        }

        public virtual Empresa ActualizarEmpresa(Empresa empresa, string secret)
        {
            empresa.FechaModificacion = DateTime.Now;

            var secretInfoCompleta = GetSecretInfoCompleta(secret);

            _uow.EmpresaRepository.UpdateEmpresa(empresa, secretInfoCompleta);

            return empresa;
        }

        public virtual string[] GetSecretInfoCompleta(string secret)
        {
            string[] secretInfoCompleta = null;

            if (!string.IsNullOrEmpty(secret))
            {
                string cipherText = Encrypter.Encrypt(secret, out string salt, out int format);
                secretInfoCompleta = new string[] { cipherText, salt, format.ToString() };
            }

            return secretInfoCompleta;
        }

        public virtual void CrearClienteMuestraDefecto(Empresa empresa)
        {
            this._uow.AgenteRepository.AddAgente(new Agente()
            {
                Empresa = empresa.Id,
                Codigo = AgentesPorDefectoEmpresaDb.ClienteMuestrasCodigo,
                Tipo = TipoAgenteDb.Cliente,
                Ruta = this._uow.RutaRepository.GetRutaGenerica(),
                Descripcion = AgentesPorDefectoEmpresaDb.ClienteMuestrasDescripcion,
                Estado = EstadoAgente.Activo,
            });
        }

        public virtual void CrearProveedorMuestraDefecto(Empresa empresa)
        {
            this._uow.AgenteRepository.AddAgente(new Agente()
            {
                Empresa = empresa.Id,
                Tipo = TipoAgenteDb.Proveedor,
                Codigo = AgentesPorDefectoEmpresaDb.ProveedorMuestrasCodigo,
                Ruta = this._uow.RutaRepository.GetRutaGenerica(),
                Descripcion = AgentesPorDefectoEmpresaDb.ProveedorMuestrasDescripcion,
                Estado = EstadoAgente.Activo,
            });
        }

        public virtual void CrearClienteEnsambladoKit(Empresa empresa)
        {
            Agente clienteensambladoKit = new Agente()
            {
                Empresa = empresa.Id,
                Codigo = AgentesPorDefectoEmpresaDb.ClienteArmadoKitCodigo,
                Tipo = TipoAgenteDb.Cliente,
                Ruta = this._uow.RutaRepository.GetRutaProcesosInternos(),
                Descripcion = AgentesPorDefectoEmpresaDb.ClienteArmadoKitDescripcion,
                Estado = EstadoAgente.Activo,
            };

            this._uow.AgenteRepository.AddAgente(clienteensambladoKit);

            empresa.ClienteArmadoKit = clienteensambladoKit;
        }

        public virtual void AsociarTiposDeRecepcionYReportesPorDefecto(Empresa empresa)
        {
            var tiposRecepcion = this._uow.RecepcionTipoRepository.GetRecepcionTiposConReportes();

            foreach (var tipoInterno in tiposRecepcion)
            {
                this._uow.RecepcionTipoRepository.AddEmpresaRecepcionTipo(new EmpresaRecepcionTipo()
                {
                    IdEmpresa = empresa.Id,
                    TipoExterno = tipoInterno.Tipo,
                    RecepcionTipoInterno = tipoInterno,
                    ManejoDeInterfaz = ManejoInterfazDb.Definida,
                    DescripcionExterna = tipoInterno.Descripcion,
                    Habilitado = tipoInterno.HabilitaAsociacionAlCrearEmpresa,
                    InterfazExterna = null,
                });

                foreach (var reporte in tipoInterno.Reportes)
                {
                    this._uow.RecepcionTipoRepository.AddEmpresaRecepcionTipoReporte(new EmpresaRecepcionTipoReporte()
                    {
                        IdEmpresa = empresa.Id,
                        CodigoReporte = reporte.Id,
                        TipoRecepcion = tipoInterno.Tipo,
                    });
                }
            }
        }

        public virtual void AgregarParametrosInterfaces(Empresa empresa)
        {
            var permiteGenerar = (_uow.ParametroRepository.GetParameter(ParamManager.GENERAR_PARAMETROS_INTERFACES) ?? "N") == "S";
            if (permiteGenerar)
            {
                var parametros = ParamManager.GetParametrosInterfaces(_uow)
                    .Select(p => new ParametroConfiguracion()
                    {
                        CodigoParametro = p,
                        TipoParametro = ParamManager.PARAM_EMPR,
                        Clave = $"{ParamManager.PARAM_EMPR}_{empresa.Id}",
                        Valor = "S"
                    });

                _uow.ParametroRepository.AddParametroConfiguracion(parametros);
            }
        }
    }
}
