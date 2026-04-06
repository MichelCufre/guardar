using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Reportes.Especificaciones;
using WIS.Domain.Reportes.Setups;

namespace WIS.Domain.Reportes
{
    public class Reporte
    {
        public long Id { get; set; }
        public string Tipo { get; set; }
        public byte[] Contenido { get; set; }
        public int? Usuario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string NombreArchivo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Predio { get; set; }
        public string Zona { get; set; }
        public List<ReporteRelacion> RelacionEntidad { get; set; }


        public Reporte()
        {
            this.RelacionEntidad = new List<ReporteRelacion>();
        }

        public virtual void AddRelacion(string tabla, string clave)
        {
            this.RelacionEntidad.Add(new ReporteRelacion
            {
                Clave = clave,
                Tabla = tabla
            });
        }

        public virtual bool PuedeReimprimir()
        {
            var situacionesNoReimprimir = new List<string>
            {
                CReporte.Pendiente,
                CReporte.PendienteReprocesar,
                CReporte.PendienteReimprimir,
                CReporte.Error,
                CReporte.Anulado,
            };

            return !situacionesNoReimprimir.Contains(this.Estado);
        }

        public virtual void PrepararReproceso()
        {
            this.Estado = CReporte.PendienteReprocesar;
        }

        public virtual void PrepararReimpresion()
        {
            this.Estado = CReporte.PendienteReimprimir;
        }

        public virtual IReportSetup GetTipoReporte(IUnitOfWork uow, string tipoReporte)
        {
            IReportSetup tipo = null;
            switch (tipoReporte)
            {
                case CReporte.NOTA_DEVOLUCION:
                    tipo = new NotaDevolucionReportSetup(uow);
                    break;
                case CReporte.CONFIRMACION_RECEPCION:
                    tipo = new ConfirmacionRecepcionReportSetup(uow);
                    break;
                case CReporte.PACKING_LIST:
                    tipo = new PackingListReportSetup(uow);
                    break;
                case CReporte.CONTENEDORES_CAMION:
                    tipo = new ContenedoresCamionReportSetup(uow);
                    break;
                case CReporte.CONTROL_CAMBIO:
                    tipo = new ControlCambioReportSetup(uow);
                    break;
                case CReporte.PACKING_LIST_SIN_LPN:
                    tipo = new PackingListSinLpnReportSetup(uow);
                    break;

            }
            return tipo;
        }

        public static string[] SplitCompositeId(string original)
        {
            return original.Split(GeneralDb.SEPARADOR_CLAVE_REPO_ID);
        }
    }
}
