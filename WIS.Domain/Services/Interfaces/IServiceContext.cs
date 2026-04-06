using System.Collections.Generic;
using System.Threading.Tasks;

namespace WIS.Domain.Services.Interfaces
{
    public interface IServiceContext
    {
        public int Empresa { get; }
        public int UserId { get; }
        public bool PermiteProductoInactivos { get; }

        Task Load();

        void AddParametro(string codigo, string entidad = "PARAM_GRAL", string entidadValor = "PARAM_GRAL");
        
        void AddParametroEmpresa(string codigo, int empresa);
        
        void AddParametroPredio(string codigo, string predio);
        
        HashSet<string> GetCamposInmutables();
        
        string GetCaracteresNoPermitidosIdentificador();
        
        string GetParametro(string codigo);
                
        void SetParametroCamposInmutables(string paramCamposInmutables);
    }
}