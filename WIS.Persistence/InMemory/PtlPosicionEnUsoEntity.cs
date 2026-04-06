using Newtonsoft.Json;
using System;

namespace WIS.Persistence.InMemory
{
    public class PtlPosicionEnUsoEntity
    {
        public string Key { get; set; }
        public string Referencia { get; set; }
        public long Id { get; set; }
        public int NU_PTL { get; set; }
        public int NU_ADDRESS { get; set; }
        public long NU_ORDEN { get; set; }


        public string NU_COLOR { get; set; }
        public string Display { get; set; }
        public string DisplayFn { get; set; }

        public DateTime DT_ADDROW { get; set; }
        public int CD_EMPRESA { get; set; }
        public string CD_PRODUCTO { get; set; }

        public int UserId { get; set; }

        public short CD_ESTADO { get; set; }

        public string Detail { get; set; }
        public long Transaccion { get; set; }
        public string Agrupacion { get; set; }

        public T GetDetail<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.Detail);
        }
        public void SetSerielizado<T>(T data)
        {
            this.Detail = JsonConvert.SerializeObject(data);
        }

        public void Clonar(PtlPosicionEnUsoEntity data)
        {
            this.Detail = data.Detail;
            this.Id = data.Id;
            this.NU_PTL = data.NU_PTL;
            this.NU_ADDRESS = data.NU_ADDRESS;
            this.NU_ORDEN = data.NU_ORDEN;
            this.NU_COLOR = data.NU_COLOR;
            this.Display = data.Display;
            this.DisplayFn = data.DisplayFn;
            this.DT_ADDROW = data.DT_ADDROW;
            this.CD_EMPRESA = data.CD_EMPRESA;
            this.CD_PRODUCTO = data.CD_PRODUCTO;
            this.CD_ESTADO = data.CD_ESTADO;
            this.UserId = data.UserId;
            this.Key = data.Key;
            this.Referencia = data.Referencia;
            this.Agrupacion = data.Agrupacion;
        }

        public void SetOperacion(short estado, string display, int? nuPosicion = null)
        {
            if (nuPosicion != null)
            {
                this.NU_ADDRESS = (int)nuPosicion;
            }

            this.CD_ESTADO = estado;
            this.Display = display;
        }


    }
}
