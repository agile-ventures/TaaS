using System;
using System.IO;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Pusher.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Pusher.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _log;
        private readonly IPushService _pushService;

        public MessageController(ILogger<MessageController> log, IPushService pushService)
        {
            _log = log;
            _pushService = pushService;
        }

        // POST api/message
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            try
            {
                string requestBody;
                using (var reader = new StreamReader(Request.Body))
                {
                    requestBody = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(requestBody))
                {
                    _log.LogError("Payload was null or empty!");
                    return BadRequest("Payload was null or empty!");
                }
                _log.LogTrace($"Message with payload {requestBody}");

                var blockHeader = JsonConvert.DeserializeObject<HeadModel>(requestBody);
                _log.LogInformation($"Block {blockHeader.level} received.");
                await _pushService.PushBlockHeader(blockHeader);
                
                return Ok();
            }
            catch (Exception e)
            {
                _log.LogCritical(e, "Error during processing api/message.", Request.Body);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}