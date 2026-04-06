using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Reportes
{
    //TODO: Revisar nombre, podria ser PreparacionReporte para no usar un verbo
    public class PrepararReporte
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _usuario;
        protected string _predio;
        protected string _zona;

        public PrepararReporte(IUnitOfWork uow, int usuario, string aplicacion, string predio, string zona)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._predio = predio;
            this._zona = zona;
        }

        public virtual long CrearReporte(string tabla, string idReporteDefinicion, string nombreArchivo, string claveReferencia)
        {
            //Guardar datos en tabla de reporte
            var nuevoReporte = new Reporte
            {
                Id = -1,
                Tipo = idReporteDefinicion,
                Usuario = _usuario,
                NombreArchivo = nombreArchivo,
                Estado = CReporte.Pendiente,
                Predio = _predio,
                Zona = _zona //Se utiliza para determinar donde se imprime el reporte (Antes venia de la puerta de la agenda, ahora la agenda no tiene puerta al momento de la creacion, entonces se setea el predio del usuario)
            };

            nuevoReporte.AddRelacion(tabla, claveReferencia);

            _uow.ReporteRepository.AddReporte(nuevoReporte);

            return nuevoReporte.Id;
        }
    }
}