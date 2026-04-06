using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion.Enums;
using WIS.Domain.Facturacion.Validaciones;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion
{
    //TODO: Redisenar, esto es un desastre
    public class ConfiguracionValidacionFacturacion : IConfiguracionValidacionFacturacion

    {
        protected readonly IUnitOfWork _uow;
        protected readonly IValidacionFacturacionResultFormatResolver _resultResolver;
        protected Dictionary<FacturacionValidacionTipo, IFacturacionValidacion> Validaciones { get; set; }
        protected List<IFacturacionValidacion> ValidacionesGenerales { get; set; }
        protected bool ValidacionesGeneralesCargadas { get; set; }
        protected bool RequiereUnicoGrupoExpedicion { get; set; }

        public ConfiguracionValidacionFacturacion(IUnitOfWork uow, IValidacionFacturacionResultFormatResolver resultResolver)
        {
            this._uow = uow;
            this._resultResolver = resultResolver;
            this.ValidacionesGenerales = new List<IFacturacionValidacion>();
            this.Validaciones = new Dictionary<FacturacionValidacionTipo, IFacturacionValidacion>
            {
                [FacturacionValidacionTipo.EmpaquetadoCompleto] = new ValidacionFacturacionEmpaquetadoCompleto(uow, this._resultResolver),
                [FacturacionValidacionTipo.RequierePrecinto] = new ValidacionFacturacionPrecinto(uow, this._resultResolver),
                [FacturacionValidacionTipo.PickingCompleto] = new ValidacionFacturacionPickingCompleto(uow, this._resultResolver)
            };
        }

        public virtual void CargarValidaciones()
        {
            Dictionary<string, string> parameters = this._uow.ParametroRepository.GetParameters(new List<string> {
                "WEXP040_VALIDAR_LIB_COMPLETA",
                "WEXP040_VALIDAR_TOTAL_ASIGNADO",
                "PRECINTO_CONTROL_PARCIAL",
                "VL_DIF_CD_GRUPO_EXPEDICION_PED"
            });

            if (parameters["WEXP040_VALIDAR_TOTAL_ASIGNADO"] == "S")
            {
                this.Validaciones[FacturacionValidacionTipo.AsignacionCompleta] = new ValidacionFacturacionAsignacionCompleta(this._uow, this._resultResolver);
                //this.ValidacionesGenerales.Add(this.Validaciones[FacturacionValidacionTipo.AsignacionCompleta]);
            }

            if (parameters["WEXP040_VALIDAR_LIB_COMPLETA"] == "S")
                this.Validaciones[FacturacionValidacionTipo.LiberacionCompleta] = new ValidacionFacturacionLiberacionCompleta(this._uow, this._resultResolver);

            if (parameters["PRECINTO_CONTROL_PARCIAL"] == "S")
                this.Validaciones[FacturacionValidacionTipo.RequierePrecinto] = new ValidacionFacturacionPrecintoParcial(this._uow, this._resultResolver);
            else
                this.Validaciones[FacturacionValidacionTipo.RequierePrecinto] = new ValidacionFacturacionPrecinto(this._uow, this._resultResolver);

            this.RequiereUnicoGrupoExpedicion = parameters["VL_DIF_CD_GRUPO_EXPEDICION_PED"] == "S";

            this.ValidacionesGeneralesCargadas = true;
        }

        public virtual List<IFacturacionValidacion> GetValidacionesEvaluar(Pedido pedido)
        {
            if (!this.ValidacionesGeneralesCargadas)
                throw new InvalidOperationException("No se cargaron las validaciones generales");

            var validacionesResultado = new List<IFacturacionValidacion>();

            if (this.ValidacionesGenerales.Any())
                validacionesResultado.AddRange(this.ValidacionesGenerales);

            if (pedido.ConfiguracionExpedicion.DebeEmpaquetarContenedor)
                validacionesResultado.Add(this.Validaciones[FacturacionValidacionTipo.EmpaquetadoCompleto]);

            if (!pedido.ConfiguracionExpedicion.PermiteFacturacionSinPrecinto)
                validacionesResultado.Add(this.Validaciones[FacturacionValidacionTipo.RequierePrecinto]);

            if (!pedido.ConfiguracionExpedicion.PermiteFacturacionParcial)
            {
                if (this.Validaciones.ContainsKey(FacturacionValidacionTipo.LiberacionCompleta))
                    validacionesResultado.Add(this.Validaciones[FacturacionValidacionTipo.LiberacionCompleta]);

                if (this.Validaciones.ContainsKey(FacturacionValidacionTipo.AsignacionCompleta))
                    validacionesResultado.Add(this.Validaciones[FacturacionValidacionTipo.AsignacionCompleta]);

                validacionesResultado.Add(this.Validaciones[FacturacionValidacionTipo.PickingCompleto]);
            }


            return validacionesResultado;
        }

        public virtual List<IFacturacionValidacion> GetValidaciones()
        {
            return this.Validaciones.Values.ToList();
        }

        public virtual bool IsUnicoGrupoExpedicionRequerido()
        {
            if (!this.ValidacionesGeneralesCargadas)
                throw new InvalidOperationException("No se cargaron las validaciones generales");

            return this.RequiereUnicoGrupoExpedicion;
        }
    }
}
