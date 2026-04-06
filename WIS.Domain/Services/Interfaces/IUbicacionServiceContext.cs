using System.Threading.Tasks;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Recorridos;

namespace WIS.Domain.Services.Interfaces
{
    public interface IUbicacionServiceContext : IServiceContext
    {
        Recorrido RecorridoPorDefecto { get; set; }
        UbicacionConfiguracion UbicacionConfiguracion { get; set; }

        bool AreaUbicacionEsMantenible(short cdArea);
        bool AreaUbicacionEsRecorrible(short cdArea);
        bool CodigoBarrasContienePrefijosWis(string cdBarras);
        bool CodigoDeBarrasEsUnicoParaPredio(string codigoBarras, string predio);
        bool ExisteAreaUbicacion(short cdArea);
        bool ExisteClase(string cdClase);
        bool ExisteEmpresa(int cdEmpresa);
        bool ExisteFamilia(int cdFamilia);
        bool ExisteNumeroOrden(long nuOrden);
        bool ExisteOrdenPorDefecto(long nuOrden);
        bool ExistePredio(string nuPredio);
        bool ExisteRotatividad(short cdRotatividad);
        bool ExisteTipoUbicacion(short cdTipoUbicacion);
        bool ExisteUbicacion(string cdUbicacion);
        bool ExisteZona(string cdZona);
        bool IdContienePrefijosWis(string ubicacion, string numeroPredio);
        Task Load();
    }
}