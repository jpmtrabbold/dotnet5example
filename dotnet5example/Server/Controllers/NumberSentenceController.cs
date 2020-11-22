using dotnet5example.Service;
using dotnet5example.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet5example.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NumberSentenceController : ControllerBase
    {
        private readonly NumberSentenceService _service;
        public NumberSentenceController(NumberSentenceService service)
        {
            _service = service;
        }

        [HttpGet("{number}")]
        public NumberSentenceResponse Get(decimal number, [FromQuery] string name)
        {
            string numberSentence = _service.GenerateNumberSentence(number);
            return new NumberSentenceResponse { Name = name, NumberSentence = numberSentence };
        }
    }
}
