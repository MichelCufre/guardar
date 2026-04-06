using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProduccionServiceContext : IServiceContext
    {
        Agente Agente { get; set; }
        Empresa EmpresaEjecucion { get; set; }

        HashSet<string> Predios { get; set; }
        HashSet<string> TiposModalidadLote { get; set; }
        HashSet<string> TiposIngresoProduccion { get; set; }
        HashSet<IngresoBlackBox> IdsProduccionExterno { get; set; }

        IEnumerable<FormulaSalida> FormulaDetallesSalida { get; set; }
        IEnumerable<FormulaEntrada> FormulaDetallesEntrada { get; set; }

        Dictionary<string, Formula> Formulas { get; set; } 
        Dictionary<string, Producto> Productos { get; set; } 
        Dictionary<string, EstacionDeTrabajo> Espacios { get; set; } 
        Dictionary<string, string> Parameters { get; set; } 

        Task Load();

        bool ExisteIdProduccionExternoEmpresa(string idExterno, int empresa);
        bool ExisteModalidadLote(string id);
        bool ExistePredio(string predio);
        bool ExisteTipoIngreso(string tipo);
        int GetCantidadDetallesTeoricos();
        IEnumerable<FormulaEntrada> GetDetallesEntrada(string idFormula);
        IEnumerable<FormulaSalida> GetDetallesSalida(string idFormula);
        EstacionDeTrabajo GetEspacioProduccion(string id);
        Formula GetFormula(string id);
        string GetParamValue(string id);
        Producto GetProducto(string codigo);
        Dictionary<string, string> LoadParamDictionary();
    }
}