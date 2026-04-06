using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.Automatismo.Interfaces;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoEjecucion
    {
        public AutomatismoEjecucion()
        {
            this.AutomatismoData = new List<AutomatismoData>();
        }

        public int Id { get; set; }

        public int? IdAutomatismo { get; set; }

        public int? AutomatismoInterfaz { get; set; }

        public int InterfazExterna { get; set; }

        public EstadoEjecucion Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? Transaccion { get; set; }

        public string Referencia { get; set; }

        public string IdentityUser { get; set; }

        public virtual IAutomatismo Automatismo { get; set; }

        public virtual List<AutomatismoData> AutomatismoData { get; set; }
        public int UserId { get; set; }

        public virtual void SetAutomatismo(IAutomatismo automatismo)
        {
            var interfaz = automatismo.GetInterfaz(this.InterfazExterna);

            if (interfaz != null)
            {
                this.InterfazExterna = interfaz.InterfazExterna;
                this.AutomatismoInterfaz = interfaz.Id;
            }


            automatismo.SetInterfazEnUso(this.InterfazExterna);

            this.Automatismo = automatismo;
            this.IdAutomatismo = automatismo.Numero;
        }

        public virtual void AddData(object request, object result)
        {
            var data = new AutomatismoData();

            data.IdAutomatismoEjecucion = this.Id;

            if (request != null)
                data.RequestData = JsonConvert.SerializeObject(request);

            if (result != null)
            {
                data.ResponseData = JsonConvert.SerializeObject(result);
            }

            this.AutomatismoData.Add(data);
        }
    }
}
