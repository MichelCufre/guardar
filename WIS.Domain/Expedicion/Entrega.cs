using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion
{
    public class Entrega
    {
        public string Id { get; set; } //NU_ENTREGA
        public string TipoEntrega { get; set; } //TP_ENTREGA

        public string Agencia { get; set; } //CD_AGENCIA
        public string CodigoBarras { get; set; } //CD_BARRAS
        public int? Camion { get; set; } //CD_CAMION
        public int? CamionFacturado { get; set; } //CD_CAMION_FACTURADO
        public string Cliente { get; set; } //CD_CLIENTE
        public int? Empresa { get; set; } //CD_EMPRESA
        public string PuntoEntrega { get; set; } //CD_PUNTO_ENTREGA
        public short? Ruta { get; set; } //CD_ROTA
        public int? Transportadora { get; set; } //CD_TRANSPORTADORA
        public string Anexo { get; set; } //DS_ANEXO
        public string DescripcionEntrega { get; set; } //DS_ENTREGA
        public DateTime? FechaAlta { get; set; } //DT_ADDROW
        public DateTime? FechaAnulacion { get; set; } //DT_ANULADA
        public DateTime? FechaCarga { get; set; } //DT_CARGADO
        public DateTime? FechaExpedicion { get; set; } //DT_EXPEDIDO
        public string IdReenvio { get; set; } //FL_REENVIO
        public int? Contenedor { get; set; } //NU_CONTENEDOR
        public long? NumeroInterfazEjecucion { get; set; } //NU_INTERFAZ_EJECUCION
        public long? NumeroInterfazEjecucionAnulacion { get; set; } //NU_INTERFAZ_EJECUCION_ANULA
        public int? NroOrdenEntrega { get; set; } //NU_ORDEN_ENTREGA
        public int? Preparacion { get; set; } //NU_PREPARACION
        public string AgrupacionEntrega { get; set; } //VL_AGRUPACION_ENTREGA
        public string Serializado { get; set; } //VL_SERIALIZADO
    }
}
