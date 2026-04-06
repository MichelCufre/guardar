using System;

namespace WIS.Domain.Picking
{
    public class ColaDeTrabajo
    {
        public int Numero { get; set; }
        public string Predio { get; set; }
        public string Descripcion { get; set; }
        public DateTime? dtAddrow { get; set; }
        public DateTime? dtUpdrow { get; set; }
        public string flOrdenCalendario { get; set; }
    }
}
