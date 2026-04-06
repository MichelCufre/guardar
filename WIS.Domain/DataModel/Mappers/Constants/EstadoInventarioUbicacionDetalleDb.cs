using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class EstadoInventarioUbicacionDetalleDb
    {
        public const string ND_ESTADO_ENDERECO_DET = "SINVED";                      //CD_DOMINIO
        public const string ND_ESTADO_ENDERECO_DET_CANCELADO = "SINVEDCAN";         //cancelado
        public const string ND_ESTADO_ENDERECO_DET_RECONTAR = "SINVEDREC";          //recontar
        public const string ND_ESTADO_ENDERECO_DET_CONTADO = "SINVEDCON";           //contado
        public const string ND_ESTADO_ENDERECO_DET_FINALIZADO_OK = "SINVEDFINOK";   //ok
        public const string ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF = "SINVEDFINDIF"; //con dif
        public const string ND_ESTADO_ENDERECO_DET_FINALIZADO_REC = "SINVEDFINREC"; //rechazado
        public const string ND_ESTADO_ENDERECO_DET_ACTUALIZADO = "SINVEDACT";       //actualizado 
    }
}
