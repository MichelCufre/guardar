using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG040ImportExcel")]
    [ApiController]
    public class REG040ImportarExcelModal : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REG040ImportarExcel _controller;

        public REG040ImportarExcelModal(IFormControllerInvocation formInvoker, REG040ImportarExcel controller)
        {
            _formInvoker = formInvoker;
            _controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(_formInvoker.Invoke(data, _controller));
        }
    }
}
