using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoReporteConfRecepcion
    {
        public int Id { get; set; }
        public int? Empresa { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public short? Situacion { get; set; }
        public short? Operacion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public short? Puerta { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Placa { get; set; }
        public string DUA { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
        public string IdEnvioDocumento { get; set; }
        public int? FunEnvioDocumento { get; set; }
        public string Averia { get; set; }
        public string IdFechaVencimiento { get; set; }
        public string IdPeso { get; set; }
        public string IdVolumen { get; set; }
        public string Cliente { get; set; }
        public DateTime? FechaFacturacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string TipoRecepcion { get; set; }
        public int? FuncionarioAsignado { get; set; }
        public string TipoRecepcionExterno { get; set; }
        public string Agente { get; set; }
        public string TipoAgente { get; set; }
        public string DescripcionCliente { get; set; }
        public short Ruta { get; set; }
        public string DS_ENDERECO { get; set; }
        public string Barrio { get; set; }
        public long? GLN { get; set; }
        public string Incripcion { get; set; }
        public string Telefono { get; set; }
        public string CEP { get; set; }
        public string FAX { get; set; }
        public string CGCCiente { get; set; }
        public string AnexoCliente1 { get; set; }
        public string AnexoCliente2 { get; set; }
        public string AnexoCliente3 { get; set; }
        public string AnexoCliente4 { get; set; }
        public string DescripcionSituacion { get; set; }
        public string DescripcionRuta { get; set; }
        public long? Localidad { get; set; }
        public string Cuidad { get; set; }
        public string DescripcionLugar { get; set; }
        public string CD_SUBDIV { get; set; }
        public string NM_SUBDIVISION { get; set; }
        public string Pais { get; set; }
        public string DescripcionPais { get; set; }
        public string Lugar { get; set; }
        public string NombreEmpresa { get; set; }
        public string DescripcionRecepcionExterno { get; set; }
    }
}
