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

        // POST api/v1/NomenclatureContent/1
        [HttpPost("{type}/{filename}/{fileNameChilde?}")]
        public async Task<IActionResult> Post(string type, string fileName, string fileNameChilde, [FromHeader] string author)
        {
            var input = new StreamReader(Request.Body).BaseStream;
            var r = await _contentStore.SetContentAsync(input,
                new Content {
                    FileName = fileNameChilde == null ? fileName : fileNameChilde,
                    ContentType = Request.ContentType,
                    Author = author,
                    FileNameParent = fileName
                }
            );
            if (!r)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
