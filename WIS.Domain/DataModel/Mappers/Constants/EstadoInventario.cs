using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class EstadoInventario
    {
        public const string Dominio = "SINV";

        public const string EnEdicion = "SINVEDI";//en edicion
        public const string EnProceso = "SINVPRO";//en proceso
        public const string Cancelado = "SINVCAN";//cancelado
        public const string Actualizado = "SINVACT";//stock actualizado
        public const string Cerrado = "SINVCER";//cerrado
    }
}
