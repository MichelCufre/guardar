using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;

namespace WIS.Application.Controllers.REG
{
    public class REG009GrupoProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGrupoService _grupoService;

        public REG009GrupoProducto(IUnitOfWorkFactory uowFactory, IGrupoService grupoService)
        {
            this._uowFactory = uowFactory;
            this._grupoService = grupoService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idEmpresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "idProducto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(x => x.Id == "idProducto")?.Value;

                Producto producto = uow.ProductoRepository.GetProducto(idEmpresa, idProducto);
                Grupo grupo = this._grupoService.GetGrupo(producto);

                form.GetField("empresa").Value = $"{producto.CodigoEmpresa} - {producto.Empresa?.Nombre}";
                form.GetField("producto").Value = $"{producto.Codigo} - {producto.Descripcion}";
                form.GetField("grupo").Value = $"{grupo.Id} - {grupo.Descripcion}";

                form.GetField("empresa").ReadOnly = true;
                form.GetField("producto").ReadOnly = true;
                form.GetField("grupo").ReadOnly = true;
            }

            return form;
        }
    }
}
