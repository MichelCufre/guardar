using System;
using System.Collections.Generic;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Produccion.Enums;

namespace WIS.Domain.Produccion
{
    public interface IIngreso
    {
        string Id { get; set; }
        int? Empresa { get; set; }
        Formula Formula { get; set; }
        int CantidadIteracionesFormula { get; set; }
        bool GeneraPedido { get; set; }
        short? Situacion { get; set; }
        int? Funcionario { get; set; }
        DateTime? FechaAlta { get; set; }
        DateTime? FechaActualizacion { get; set; }
        string NumeroProduccionOriginal { get; set; }
        string Anexo1 { get; set; }
        string Anexo2 { get; set; }
        string Anexo3 { get; set; }
        string Anexo4 { get; set; }
        long? EjecucionEntrada { get; set; }
        string Predio { get; set; }
        string IdManual { get; set; }

        DateTime? FechaInicioProduccion { get; set; }
        DateTime? FechaFinProduccion { get; set; }
        long? NroUltInterfazEjecucion { get; set; }
        
        TipoProduccionIngreso TipoProduccion { get; set; }
		List<IngresoProduccionDetalleReal> Consumidos { get; set; }
		List<IngresoProduccionDetalleSalida> Producidos { get; set; }
		void CerrarProduccion();
        int GetTotalPasadas(); 
        bool IsFinalizado();
    }
}
