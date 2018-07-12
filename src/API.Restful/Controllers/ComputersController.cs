using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Application.Services.Computer;
using Application.Services.GraphicsCard;

using API.Contracts;
using API.Contracts.Dtos;
using API.Restful.Helpers;

using AutoMapper;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;
using Application.Interfaces.Services;

namespace API.Restful.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/computers")]
    public class ComputersController : Controller
    {
        private readonly IComputersService _computersService;
        private readonly IGraphicsCardsService _graphicsCardsService;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public ComputersController(IComputersService computersService, IGraphicsCardsService graphicsCardsService, IUrlHelper urlHelper, IMapper mapper)
        {
            _computersService = computersService;
            _graphicsCardsService = graphicsCardsService;
            _urlHelper = urlHelper;
            _mapper = mapper;
        }

        #region  GET

        [HttpGet(Name = "GetComputers")]
        [HttpHead]
        public async Task<IActionResult> GetComputers(MetadataResourceParameters metadataResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var result = await _computersService.GetComputers(metadataResourceParameters);

            var previousPageLink = result.Data.HasPrevious ? CreateResourceUri(metadataResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = result.Data.HasNext ? CreateResourceUri(metadataResourceParameters, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = result.Data.TotalCount,
                pageSize = result.Data.PageSize,
                currentPage = result.Data.CurrentPage,
                totalPages = result.Data.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add(new KeyValuePair<string, StringValues>("X-Pagination", JsonConvert.SerializeObject(paginationMetaData)));

            var response = result.Data.Select(x => new ComputerDto
            {
                Name = x.Name,
                Id = x.Id,
                GraphicCards = x.GraphicCards,
                Price = x.Price,
                Rams = x.Rams
            });

            if (mediaType.IsApplicationHal())
            {
                var wrapper = new LinkedCollectionResourceWrapperDto<ComputerDto>(response.Select(CreateLinkForComputer));

                return new ObjectResult(CreateLinkForComputers(wrapper)) { StatusCode = (int)result.HttpStatusCode };
            }

            return new ObjectResult(response) { StatusCode = (int)result.HttpStatusCode };
        }

        [HttpGet("{computerId}", Name = "GetComputer")]
        public async Task<IActionResult> GetComputer(Guid computerId,
            [FromHeader(Name = "Accept")] string mediaType )
        {
            var response = await _computersService.GetComputer(computerId);

            if (response.Data != null && mediaType.IsApplicationHal())
            {
                response.Data = CreateLinkForComputer(response.Data);
            }

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        /// <summary>
        /// Gets the graphics cards for the computer
        /// </summary>
        /// <param name="computerId"></param>
        /// <returns>return all graphic cards for the computer</returns>
        [HttpGet("{computerId}/graphicscards", Name = "GetGraphicsCardsForComputer")]
        public async Task<IActionResult> GetGraphicsCardsForComputer(Guid computerId)
        {
            var response = await _computersService.GetGraphicsCardsForComputer(computerId);

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        /// <summary>
        /// Gets a specific graphicscard for a specified computer
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="graphicsCardId"></param>
        /// <returns>returns a singel graphics card for a specified computer</returns>
        [HttpGet("{computerId}/graphicscards/{graphicsCardId}", Name = "GetGraphicCardForComputer")]
        public async Task<IActionResult> GetGraphicCardForComputer(Guid computerId, Guid graphicsCardId)
        {
            var response = await _computersService.GetGraphicsCardForComputer(computerId, graphicsCardId);

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        /// <summary>
        /// Gets the ram for the computer
        /// </summary>
        /// <param name="computerId"></param>
        /// <returns>returns the ram for the specific computer</returns>
        [HttpGet("{computerId}/rams", Name = "GetRamsForComputer")]
        public async Task<IActionResult> GetRamsForComputer(Guid computerId)
        {
            var response = await _computersService.GetRamsForComputer(computerId);
            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        /// <summary>
        /// Get a specific ram for a specified computer
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="ramId"></param>
        /// <returns>returns the ram for the specific computer</returns>
        [HttpGet("{computerId}/rams/{ramId}", Name = "GetRamForComputer")]
        public async Task<IActionResult> GetRamForComputer(Guid computerId, Guid ramId)
        {
            var response = await _computersService.GetRamForComputer(computerId, ramId);

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        #endregion

        #region POST

        [HttpPost(Name = "CreateComputer")]
        public async Task<IActionResult> CreateComputer([FromBody] CreateComputer request,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var response = await _computersService.CreateComputer(request);

            if (response.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                if (mediaType.IsApplicationHal())
                {
                    response.Data = CreateLinkForComputer(response.Data);
                }

                return CreatedAtRoute("GetComputer", new { computerId = response.Data.Id }, response.Data);
            }

            return new ObjectResult(response.Data)
            {
                StatusCode = (int)response.HttpStatusCode,
                Value = response.Reason
            };
        }

        /// <summary>
        /// Gets a specific graphicscard for a specified computer
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="command"></param>
        /// <returns>returns a singel graphics card for a specified computer</returns>
        [HttpPost("{computerId}/graphicscards", Name = "CreateGraphicCards")]
        public async Task<IActionResult> CreateGraphicCards(Guid computerId, [FromBody] IEnumerable<CreateGraphicsCard> command)
        {
            var response = await _graphicsCardsService.Create(computerId, command);

            if (response.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                var idsAsString = string.Join(",", response.Data.Select(x => x.Id));

                return CreatedAtRoute("GetGraphicsCardCollection", new { graphicsCardIds = idsAsString }, response.Data);
            }

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        #endregion

        #region PUT

        [HttpPut("{computerId}", Name = "UpdateComputer")]
        public async Task<IActionResult> UpdateComputer(Guid computerId, [FromBody] UpdateComputer request,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (request == null)
            {
                return NotFound();
            }

            var response = await _computersService.UpdateComputer(computerId, request);

            if (response.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                if (mediaType.IsApplicationHal())
                {
                    return CreatedAtRoute("GetComputer", new { response.Data.Id }, CreateLinkForComputer(response.Data));
                }

                return CreatedAtRoute("GetComputer", new { response.Data.Id }, response.Data);
            }

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        [HttpPut("{computerId}/graphicscards/{graphicsCardId}", Name = "UpdateGraphicsCard")]
        public async Task<IActionResult> UpdateGraphicsCard(Guid computerId, Guid graphicsCardId, [FromBody] UpdateGraphicsCard updateGraphicsCardCommand)
        {
            if (updateGraphicsCardCommand == null)
            {
                return BadRequest();
            }

            var response = await _graphicsCardsService.Update(computerId, graphicsCardId, updateGraphicsCardCommand);

            if (response.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                return CreatedAtRoute("GetGraphicCardForComputer",
                    new { computerId = response.Data.ComputerId, graphicsCardId = response.Data.Id }, response.Data);
            }

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };

        }

        #endregion

        #region Patch

        [HttpPatch("{computerId}", Name = "PatchComputer")]
        public async Task<IActionResult> PatchComputer(Guid computerId, [FromBody] JsonPatchDocument<ComputerDto> patchoDocument,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (patchoDocument == null)
            {
                return BadRequest();
            }

            var response = await _computersService.Patch(computerId, patchoDocument);

            if (mediaType.IsApplicationHal())
            {
                response.Data = CreateLinkForComputer(response.Data);
            }

            return new ObjectResult(response.Data) { StatusCode = (int)response.HttpStatusCode };
        }

        #endregion

        #region Delete

        [HttpDelete("{computerId}", Name = "DeleteComputer")]
        public async Task<IActionResult> DeleteComputer(Guid computerId)
        {
            if (computerId == Guid.Empty)
            {
                return BadRequest();
            }

            await _computersService.DeleteComputer(computerId);

            return NoContent();
        }

        #endregion

        #region Options

        [HttpOptions(Name = "ComputerOptions")]
        public IActionResult ComputerOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,PUT,PATCH,OPTIONS,HEAD");
            return Ok();
        }

        #endregion

        #region private

        private LinkedCollectionResourceWrapperDto<ComputerDto> CreateLinkForComputers(LinkedCollectionResourceWrapperDto<ComputerDto> computerDto)
        {
            computerDto.Links.Add(new LinkDto(_urlHelper.Link("GetComputers", new { }),
                "self",
                "GET"));

            return computerDto;
        }

        private ComputerDto CreateLinkForComputer(ComputerDto computer)
        {
            computer.Links.Add(
                new LinkDto(_urlHelper.Link("GetComputer",
                new { computerId = computer.Id }),
                "self",
                "GET"));

            computer.Links.Add(
                new LinkDto(_urlHelper.Link("DeleteComputer",
                new { computerId = computer.Id }),
                "delete",
                "DELETE"));

            computer.Links.Add(
                new LinkDto(_urlHelper.Link("UpdateComputer",
                new { computerId = computer.Id }),
                "update",
                "PUT"));

            computer.Links.Add(
                new LinkDto(_urlHelper.Link("PatchComputer",
                new { computerId = computer.Id }),
                "patch",
                "PATCH"));

            return computer;
        }

        private string CreateResourceUri(MetadataResourceParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetComputers",
                        new
                        {
                            searchQuery = parameters.SearchQuery,
                            filter = parameters.Filter,
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetComputers",
                        new
                        {
                            searchQuery = parameters.SearchQuery,
                            filter = parameters.Filter,
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetComputers",
                        new
                        {
                            searchQuery = parameters.SearchQuery,
                            filter = parameters.Filter,
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize
                        });
            }
        }

        private enum ResourceUriType
        {
            PreviousPage,
            NextPage
        }

        #endregion
    }
}
