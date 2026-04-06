using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using WIS.TaskQueue.Dtos;
using WIS.TaskQueue.Extensions;
using WIS.TaskQueue.Services;

namespace WIS.TaskQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class TaskQueueController : ControllerBaseExtension
    {
        private readonly ITaskQueue _taskQueue;

        public TaskQueueController(TaskQueue.Services.TaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        /// <summary>
        ///     swagger_summary_taskqueue_enqueue
        /// </summary>
        /// <remarks>swagger_remarks_taskqueue_enqueue</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">swagger_response_200_taskqueue_validate</response>
        /// <response code="400">swagger_response_400_taskqueue_validate</response>
        [HttpPost("Enqueue")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult Enqueue([FromBody] TasksRequest request)
        {
            if (_taskQueue.IsPreloadCompleted())
            {
                foreach (var task in request.Tasks)
                {
                    _taskQueue.Add(new Models.Task()
                    {
                        Category = task.Category,
                        Data = task.Data
                    });
                }
            }

            return Ok();
        }

        /// <summary>
        ///     swagger_summary_taskqueue_restart
        /// </summary>
        /// <remarks>swagger_remarks_taskqueue_restart</remarks>
        /// /// <response code="200">swagger_response_200_taskqueue_restart</response>
        /// <response code="400">swagger_response_400_taskqueue_restart</response>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Restart")]
        public IActionResult Restart()
        {
            _taskQueue.Stop();
            _taskQueue.Start();

            return Ok();
        }
    }
}
