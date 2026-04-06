using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtpReportePackingList
    {
        public int Camion { get; set; }
        public int? Empresa { get; set; }
        public string Matricula { get; set; }
        public short? Rota { get; set; }
        public int Transportadora { get; set; }
        public string DescripcionCamion { get; set; }
        public int? Vehiculo { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string DescripcionTransportadora { get; set; }
        public string DescripcionRuta { get; set; }
        public string NombreEmpresa { get; set; }
    }
}
