using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class EstadoControlContenedor
    {
        public const string Iniciado = "P";                 //Parcialmente controlado (iniciado)
        public const string Finalizado = "C";               //Control cerrado sin diferencias (completo)
        public const string FinalizadoConDiferencias = "D"; //Control cerrado con diferencias (Finalizado con diferencias)
    }
}
