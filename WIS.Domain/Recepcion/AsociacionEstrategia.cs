using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.General;

namespace WIS.Domain.Recepcion
{
    public class AsociacionEstrategia
    {
        public string Clase { get; set; } //CD_CLASSE
        public int? Empresa { get; set; } //CD_EMPRESA
        public string Grupo { get; set; } //CD_GRUPO
        public string Producto { get; set; } //CD_PRODUTO
        public DateTime FechaRegistro { get; set; } //DT_ADDROW
        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
        public int Id { get; set; } //NU_ALM_ASOCIACION
        public int Estrategia { get; set; } //NU_ALM_ESTRATEGIA
        public string Predio { get; set; } //NU_PREDIO
        public string Tipo { get; set; } //TP_ALM_ASOCIACION
        public string DatoAuditoria { get; set; } //VL_AUDITORIA
        public OperativaAlmacenaje Operativa { get; set; }

        public AsociacionEstrategia()
        {
            Operativa = new OperativaAlmacenaje();
        }
    }
}
