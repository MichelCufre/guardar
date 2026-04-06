using Newtonsoft.Json;
using System;

namespace WIS.Domain.Automatismo
{
    public class PtlPosicionEnUso
    {
        public string Key { get; set; }
        public string Referencia { get; set; }
        public long Id { get; set; }
        public int Ptl { get; set; }
        public int Ubicacion { get; set; }
        public long Orden { get; set; }

        public string Color { get; set; }
        public string Display { get; set; }
        public string DisplayFn { get; set; }

        public DateTime FechaRegistro { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }

        public int UserId { get; set; }

        public short Estado { get; set; }

        public string Detalle { get; set; }
        public long Transaccion { get; set; }
        public string Agrupacion { get; set; }

        public virtual T GetDetail<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Detalle);
        }
        public virtual void SetSerielizado<T>(T data)
        {
            this.Detalle = JsonConvert.SerializeObject(data);
        }

        public virtual void Clonar(PtlPosicionEnUso data)
        {
            this.Detalle = data.Detalle;
            this.Id = data.Id;
            this.Ptl = data.Ptl;
            this.Ubicacion = data.Ubicacion;
            this.Orden = data.Orden;
            this.Color = data.Color;
            this.Display = data.Display;
            this.DisplayFn = data.DisplayFn;
            this.FechaRegistro = data.FechaRegistro;
            this.Empresa = data.Empresa;
            this.Producto = data.Producto;
            this.Estado = data.Estado;
            this.UserId = data.UserId;
            this.Key = data.Key;
            this.Referencia = data.Referencia;
            this.Agrupacion = data.Agrupacion;
        }

        public virtual void SetOperacion(short estado, string display, int? nuPosicion = null)
        {
            if (nuPosicion != null)
            {
                this.Ubicacion = (int)nuPosicion;
            }

            this.Estado = estado;
            this.Display = display;
        }
    }
}
