using WIS.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Domain.Logic;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class ModificarDetalleReferenciaServiceContext : ReferenciaRecepcionServiceContext, IModificarDetalleReferenciaServiceContext
    {
        public Dictionary<string, ReferenciaRecepcionDetalle> Detalles { get; set; } = new Dictionary<string, ReferenciaRecepcionDetalle>();
        public HashSet<int> ReferenciaIds { get; set; } = new HashSet<int>();
        public HashSet<int> ReferenciasEnUso { get; set; } = new HashSet<int>();

        public ModificarDetalleReferenciaServiceContext(IUnitOfWork uow, List<ReferenciaRecepcion> referencias, int userId, int empresa)
            : base(uow, referencias, userId, empresa)
        {
        }

        public async override Task Load()
        {
            await base.Load();

            var keysDetalles = new List<ReferenciaRecepcionDetalle>();

            foreach (var r in _referencias)
            {
                foreach (var d in r.Detalles)
                {
                    var idReferencia = GetReferencia(r.Numero, Empresa, r.TipoReferencia, r.CodigoCliente)?.Id;

                    if (idReferencia.HasValue)
                    {
                        d.IdReferencia = idReferencia.Value;
                        d.Identificador = GetIdentificador(d.Identificador, d.CodigoProducto);
                        keysDetalles.Add(new ReferenciaRecepcionDetalle()
                        {
                            IdReferencia = d.IdReferencia,
                            IdLineaSistemaExterno = d.IdLineaSistemaExterno.Truncate(40),
                            IdEmpresa = d.IdEmpresa,
                            CodigoProducto = d.CodigoProducto.Truncate(40),
                            Identificador = d.Identificador.Truncate(40)
                        });
                        ReferenciaIds.Add(idReferencia.Value);
                    }
                }
            }

            var detalles = _uow.ReferenciaRecepcionRepository.GetDetallesReferencias(keysDetalles);

            foreach (var d in detalles)
            {
                var identificador = GetIdentificador(d.Identificador, d.CodigoProducto);
                var key = $"{d.IdReferencia}.{d.IdLineaSistemaExterno}.{Empresa}.{d.CodigoProducto}.{identificador}";
                Detalles[key] = d;
            }

            foreach (var id in _uow.ReferenciaRecepcionRepository.GetReferenciasEnUso(ReferenciaIds))
            {
                ReferenciasEnUso.Add(id);
            }
        }

        public virtual ReferenciaRecepcionDetalle GetDetalleReferencia(int referencia, string idLinea, int empresa, string producto, string identificador)
        {
            var key = $"{referencia}.{idLinea}.{empresa}.{producto}.{identificador}";
            return Detalles.GetValueOrDefault(key, null);
        }

        public virtual string GetIdentificador(string identificador, string producto)
        {
            var prod = GetProducto(Empresa, producto);

            if (string.IsNullOrEmpty(identificador))
            {
                if (prod != null)
                {
                    if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                        return ManejoIdentificadorDb.IdentificadorProducto;
                    else
                        return ManejoIdentificadorDb.IdentificadorAuto;
                }
                else
                {
                    return string.Empty;
                }
            }

            return identificador;
        }

        public virtual bool ReferenciaEnUso(int referencia)
        {
            return ReferenciasEnUso.Contains(referencia);
        }
    }
}