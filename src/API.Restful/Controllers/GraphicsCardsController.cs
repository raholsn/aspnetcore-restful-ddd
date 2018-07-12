using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Application.Services;
using Application.Services.GraphicsCard;

using API.Contracts;
using API.Restful.Helpers;

using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;

namespace API.Restful.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/graphicscards")]
    public class GraphicsCardsController : Controller
    {
        private readonly IGraphicsCardsService _graphicsCardsService;
        public GraphicsCardsController(IGraphicsCardsService graphicsCardsService)
        {
            _graphicsCardsService = graphicsCardsService;
        }

        /// <summary>
        ///  Gets the graphics cards
        /// </summary>
        /// <returns>All graphics cards</returns>
        [HttpGet]
        public async Task<IActionResult> Get(MetadataResourceParameters metadataParameters)
        {
            var responses = await _graphicsCardsService.Get(metadataParameters);

            return new ObjectResult(responses.Data) { StatusCode = (int)responses.HttpStatusCode };
        }

        /// <summary>
        /// Gets a specific graphics cards
        /// </summary>
        /// <param name="graphicsCardId"></param>
        /// <returns>Specific graphics card</returns>
        [HttpGet("{graphicsCardId}")]
        public async Task<IActionResult> Get(Guid graphicsCardId)
        {
            var response = await _graphicsCardsService.Get(graphicsCardId);

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        /// <summary>
        /// Gets a specific graphics cards
        /// </summary>
        /// <param name="graphicsCardIds"></param>
        /// <returns>Specific graphics card</returns>
        [HttpGet("({graphicsCardIds})", Name = "GetGraphicsCardCollection")]
        public async Task<IActionResult> Get([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> graphicsCardIds)
        {
            if (graphicsCardIds == null)
            {
                return BadRequest();
            }

            var graphicsCardId = graphicsCardIds as Guid[] ?? graphicsCardIds.ToArray();

            var response = await _graphicsCardsService.Get(graphicsCardId);

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }
    }
}
