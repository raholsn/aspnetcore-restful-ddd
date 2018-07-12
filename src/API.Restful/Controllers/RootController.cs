using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using API.Contracts.Dtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Restful.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class RootController : Controller
    {
        private readonly IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")]string mediaType)
        {
            if (mediaType == "application/hal+json")
            {
                var links = new List<LinkDto>
                {
                    new LinkDto(_urlHelper.Link("GetRoot", new { }), "self", "GET"),
                    new LinkDto(_urlHelper.Link("GetComputers", new { }), "computers", "GET"),
                    new LinkDto(_urlHelper.Link("CreateComputer", new { }), "create_computer", "POST")
                };


                return Ok(links);
            }

            return NoContent();
        }
    }
}
