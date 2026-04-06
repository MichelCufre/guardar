namespace WIS.Domain.General
{
	public class UbicacionArea
    {

        public short Id { get; set; } //CD_AREA_ARMAZ
        public string Descripcion { get; set; } //DS_AREA_ARMAZ

        /// <summary>
        /// Campo en base ID_AREA_PROBLEMA (S/N)
        /// </summary>
        public bool EsAreaProblema { get; set; }

        /// <summary>
        /// Campo en base ID_AREA_EMBARQUE (S/N)
        /// </summary>
        public bool EsAreaEmbarque { get; set; }
        /// <summary>
        /// Campo en base ID_AREA_AVARIA (S/N)
        /// </summary>
        public bool EsAreaAveria { get; set; }

        /// <summary>
        /// Campo en base ID_VEICULO (S/N)
        /// </summary>
        public bool PermiteVehiculo { get; set; }
        /// <summary>
        /// Campo en base ID_AREA_ESPERA (S/N)
        /// </summary>
        public bool EsAreaEspera { get; set; }
        /// <summary>
        /// Campo en base ID_AREA_PICKING (S/N)
        /// </summary>
        public bool EsAreaPicking { get; set; }
        /// <summary>
        /// Campo en base ID_ESTOQUE_GERAL (S/N)
        /// </summary>
        public bool EsAreaStockGeneral { get; set; }
        /// <summary>
        /// Campo en base ID_DISP_ESTOQUE (S/N)
        /// </summary>
        public bool DisponibilizaStock { get; set; }
        /// <summary>
        /// Campo en base FL_INVENTARIABLE (S/N)
        /// </summary>
        public bool EsAreaInventariable { get; set; }

        /// <summary>
        /// Campo en base ID_PERMITE_MANTENIMIENTO (S/N)
        /// </summary>
        public bool EsAreaMantenible { get; set; }


    }
}
