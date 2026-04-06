using Dapper;
using System.Data;
using System.Threading.Tasks;
using WIS.Domain.Reportes;
using WIS.Domain.Services.Interfaces;

namespace WIS.TicketPrintingProcess.DataModel.Repositories
{
    public class ReporteRepository
    {
        private readonly IDapper _dapper;

        public ReporteRepository(IDapper dapper)
        {
            this._dapper = dapper;
        }

        public Reporte GetReporte(long nroReporte)
        {
            var parameters = new DynamicParameters(new
            {
                nroReporte = nroReporte
            });

            var task = Task.FromResult(_dapper.Get<Reporte>(
                @"SELECT 
                    NM_ARCHIVO as NombreArchivo,
                    VL_DATA as Contenido
                FROM T_REPORTE
                WHERE NU_REPORTE = :nroReporte",
                parameters,
                commandType: CommandType.Text));

            task.Wait();

            return task.Result;
        }
    }
}
