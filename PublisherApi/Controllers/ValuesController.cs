using Microsoft.AspNetCore.Mvc;
using PublisherApi.Services;
using System;
using System.Collections.Generic;

namespace PublisherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ValuesController(IMessageService messageService)
        {
            this._messageService = messageService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public IActionResult Post(string data)
        {
            Console.WriteLine("received a Post: " + data);
            _messageService.Enqueue(data);
            return this.Ok("{\"success\": \"true\"}");
        }

        [HttpPut("{id}")]
        public void Put(int id, string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
