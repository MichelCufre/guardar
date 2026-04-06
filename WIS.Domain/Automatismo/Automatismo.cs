using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;

namespace WIS.Domain.Automatismo
{
    public abstract class Automatismo : IAutomatismo
    {
        protected int _interfazEnUso;

        public int Numero { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string Predio { get; set; }
        public string ZonaUbicacion { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? Transaccion { get; set; }

        public List<AutomatismoPosicion> Posiciones { get; set; }
        public List<AutomatismoCaracteristica> Caracteristicas { get; set; }
        public List<AutomatismoEjecucion> Ejecuciones { get; set; }
        public List<AutomatismoInterfaz> Interfaces { get; set; }
        public List<AutomatismoPuesto> Puestos { get; set; }

        public virtual AutomatismoInterfaz GetInterfaz(int cdInterfaz)
        {
            return this.Interfaces.FirstOrDefault(f => f.Interfaz == cdInterfaz || f.InterfazExterna == cdInterfaz);
        }

        public virtual List<AutomatismoPosicion> GetPosiciones(string tipo)
        {
            return this.Posiciones
                .Where(p => p.TipoUbicacion == tipo)
                .OrderBy(p => p.IdUbicacion)
                .ToList();
        }

        public virtual void SetInterfazEnUso(int interfaz)
        {
            this._interfazEnUso = interfaz;
        }

        public virtual AutomatismoInterfaz GetInterfazEnUso()
        {
            return this.GetInterfaz(this._interfazEnUso);
        }

        public virtual AutomatismoCaracteristica GetCaracteristicaByCodigo(string codigo)
        {
            return this.Caracteristicas?.FirstOrDefault(w => w.Codigo == codigo);
        }

        /// <summary>
        /// Función cuyo comportamiento de generación de ubicaciones es definido en cada subclase de Automatismo
        /// </summary>

        public abstract void GenerarUbicaciones(AutomatismoCantidades cantidades);


        public abstract void GetConfiguracionUbicacionDefault(string tipoPosicion, out short area, out short tipo, out int? tipoAgrupacion);

        /// <summary>
        /// Método que genera tantas posiciones de automatismo del tipo especificado como la cantidad especificada
        /// </summary>
        public virtual void GenerarUbicacionesByTipoUbicacion(int cantidadUbicaciones, string tipoUbicacion)
        {
            for (int i = 0; i < cantidadUbicaciones; i++)
            {
                var ubic = this.GetUbicacionPorDefault(tipoUbicacion, out int? tipoAgrupacion);

                this.Posiciones.Add(new AutomatismoPosicion
                {
                    TipoUbicacion = tipoUbicacion,
                    IdAutomatismo = Numero,
                    Orden = (short)i,
                    Ubicacion = ubic,
                    TipoAgrupacion = tipoAgrupacion
                });
            }
        }

        public virtual Ubicacion GetUbicacionPorDefault(string tipoPosicion, out int? tipoAgrupacion)
        {
            Ubicacion ubicacion = new Ubicacion();

            switch (tipoPosicion)
            {
                case AutomatismoDb.TP_UBIC_PICKING:
                    ubicacion.Calle = TipoUbicacionDb.AutomatismoPicking;
                    break;
                case AutomatismoDb.TP_UBIC_ENTRADA:
                    ubicacion.Calle = TipoUbicacionDb.AutomatismoEntrada;
                    break;
                case AutomatismoDb.TP_UBIC_TRANSITO:
                    ubicacion.Calle = TipoUbicacionDb.AutomatismoTransferencia;
                    break;
                case AutomatismoDb.TP_UBIC_SALIDA:
                    ubicacion.Calle = TipoUbicacionDb.AutomatismoSalida;
                    break;
                case AutomatismoDb.TP_UBIC_AJUSTES:
                    ubicacion.Calle = TipoUbicacionDb.AutomatismoAjuste;
                    break;
                case AutomatismoDb.TP_UBIC_RECHAZOS:
                    ubicacion.Calle = TipoUbicacionDb.AutomatismoRechazo;
                    break;
                default:
                    break;
            }

            this.GetConfiguracionUbicacionDefault(tipoPosicion, out short area, out short tipo, out tipoAgrupacion);

            ubicacion.IdUbicacionArea = area;
            ubicacion.IdUbicacionTipo = tipo;

            return ubicacion;
        }


        /// <summary>
        /// Método que modifican valores del sistema dependiendo de la característica ingresada.
        /// </summary>
        public virtual void ActualizarValoresSegunCaracteristica(AutomatismoCaracteristica caracteristica, List<AutomatismoCaracteristicaConfiguracion> configDefault)
        {
            if (caracteristica.Codigo == AutomatismoDb.CARACTERISTICA_AGRUPACION_UBIC)
            {
                foreach (var pos in Posiciones.Where(w => w.TipoAgrupacion != 0))
                {
                    pos.TipoAgrupacion = int.TryParse(caracteristica.Valor, out int tipoAgrup) ? tipoAgrup : -1;
                    pos.Transaccion = 1;
                }
            }
        }

        public virtual AutomatismoPosicion GetPosicion(int id)
        {
            return this.Posiciones.FirstOrDefault(w => w.Id == id);
        }

        public virtual AutomatismoPosicion GetPosicionByUbicacion(string ubicacion)
        {
            return this.Posiciones.FirstOrDefault(w => w.IdUbicacion == ubicacion);
        }

        public virtual bool AllowEditionEjecucion()
        {
            return true;
        }
    }
}
