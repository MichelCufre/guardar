using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion
{
    public class ValidacionCamionResultado
    {
        public string Message { get; set; }
        public List<string> Datos { get; set; }

        public ValidacionCamionResultado(string message, List<string> datos)
        {
            this.Message = message;
            this.Datos = datos;
        }
    }
}
