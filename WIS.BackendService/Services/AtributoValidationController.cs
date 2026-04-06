using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Domain.Validation;

namespace WIS.BackendService.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtributoValidationController : ControllerBase
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public AtributoValidationController(IUnitOfWorkFactory uowFactory) : base()
        {
            _uowFactory = uowFactory;
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Validation([FromBody] AtributoData data)
        {
            try
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                AtributoValidation.ValidacionAsociadaAtributos(uow, data.IdAtributo, data.Valor, CultureInfo.InvariantCulture, invocarAPICustom: true, out List<Error> errores);

                data.Errores = errores;
            }
            catch (Exception ex)
            {
                if (data.Errores == null)
                    data.Errores = new List<Error>();

                data.Errores.Add(new Error(ex.Message));
            }

            return Ok(data);
        }
    }

    public class AtributoData
    {
        public string Valor { get; set; }
        public int IdAtributo { get; set; }
        public List<Error> Errores { get; set; }
    }
}
