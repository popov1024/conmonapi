using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using conmonapi.Models;
using conmonapi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace conmonapi.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : Controller
    {
        private readonly IContentSore _contentStore;
        private IHostingEnvironment _environment;
        public ContentController(IHostingEnvironment environment, IContentSore contentStore)
        {
            _contentStore = contentStore;
            _environment = environment;
        }

       [HttpGet("{author}/{type}/{filename}/{fileNameChilde?}")]
        public async Task<IActionResult> Get(string author, string type, string fileName, string fileNameChilde)
        {
            var tuple = await _contentStore.GetContentAsync(
                new Content(author, type, fileName, fileNameChilde)
            );
            if (tuple.Item1 == null)
            {
                return NotFound();
            }
            var file = new FileContentResult(tuple.Item1, tuple.Item2.ContentType);
            return file;
        }

        // POST api/v1/NomenclatureContent/1
        [HttpPost("{author}/{type}/{filename}/{fileNameChilde?}")]
        public async Task<IActionResult> Post(string author, string type, string fileName, string fileNameChilde)
        {
            var input = new StreamReader(Request.Body).BaseStream;
            var r = await _contentStore.SetContentAsync(input,
                new Content(author, type, fileName, fileNameChilde, Request.ContentType)
            );
            if (!r)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
