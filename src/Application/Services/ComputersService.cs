using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Application.Helpers;

using API.Contracts;
using API.Contracts.Dtos;

using AutoMapper;

using Domain;
using Domain.Builder;


using Microsoft.AspNetCore.JsonPatch;

using Shared;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services.Computer
{
    public class ComputersService : IComputersService
    {
        private readonly IComputerRepository _computerRepository;
        private readonly IGraphicsCardRepository _graphicsCardRepository;
        private readonly IMapper _mapper;
        public ComputersService(IComputerRepository computerRepository, IGraphicsCardRepository graphicsCardRepository, IMapper mapper)
        {
            _mapper = mapper;
            _computerRepository = computerRepository;
            _graphicsCardRepository = graphicsCardRepository;
        }

        public async Task<BaseResponse<ComputerDto>> GetComputer(Guid id)
        {
            if (!(await _computerRepository.Exists(id)))
            {
                return BaseResponse<ComputerDto>.NotFound("Computer id not found");
            }

            var result = await _computerRepository.Get(id);

            return new BaseResponse<ComputerDto>((int)HttpStatusCode.OK, _mapper.Map<ComputerDto>(result));
        }

        public async Task<BaseResponse<PagedList<ComputerDto>>> GetComputers(MetadataResourceParameters parameters)
        {
            var result = await _computerRepository.Get(parameters.PageNumber, parameters.PageSize, parameters.Filter, parameters.SearchQuery);

            if (!result.Any())
            {
                return BaseResponse<PagedList<ComputerDto>>.NotFound("No computers found");
            }


            return new BaseResponse<PagedList<ComputerDto>>((int)HttpStatusCode.OK,
                    new PagedList<ComputerDto>(result.Select(x => _mapper.Map<ComputerDto>(x)).ToList(), result.TotalCount,
                        parameters.PageNumber, parameters.PageSize));
        }

        public async Task<BaseResponse<IEnumerable<GraphicsCardDto>>> GetGraphicsCardsForComputer(Guid computerId)
        {
            if (!await _computerRepository.Exists(computerId))
            {
                return BaseResponse<IEnumerable<GraphicsCardDto>>.NotFound("Computer not found");
            }

            var result = await _computerRepository.GetGraphicsCardsForComputer(computerId);

            return new BaseResponse<IEnumerable<GraphicsCardDto>>((int)HttpStatusCode.OK, _mapper.Map<IEnumerable<GraphicsCardDto>>(result));
        }

        public async Task<BaseResponse<GraphicsCardDto>> GetGraphicsCardForComputer(Guid computerId, Guid graphicCardId)
        {
            if (!await _graphicsCardRepository.Exists(computerId, graphicCardId))
            {
                return BaseResponse<GraphicsCardDto>.NotFound("Computer not found");
            }

            var result = await _computerRepository.GetGraphicsCardForComputer(computerId, graphicCardId);

            var graphicsCardDto = _mapper.Map<GraphicsCardDto>(result);

            return new BaseResponse<GraphicsCardDto>((int)HttpStatusCode.OK, graphicsCardDto);
        }

        public async Task<BaseResponse<IEnumerable<RamDto>>> GetRamsForComputer(Guid computerId)
        {
            if (!await _computerRepository.Exists(computerId))
            {
                return BaseResponse<IEnumerable<RamDto>>.NotFound("Computer not found");
            }

            var result = await _computerRepository.GetRamsForComputer(computerId);

            var ramDtos = _mapper.Map<IEnumerable<RamDto>>(result);

            return new BaseResponse<IEnumerable<RamDto>>((int)HttpStatusCode.OK, ramDtos);
        }

        public async Task<BaseResponse<RamDto>> GetRamForComputer(Guid computerId, Guid ramId)
        {
            if (!await _computerRepository.ComputerRamExists(computerId, ramId))
            {
                return BaseResponse<RamDto>.NotFound("Computer not found");
            }

            var result = await _computerRepository.GetRamForComputer(computerId, ramId);

            var ramDto = _mapper.Map<RamDto>(result);

            return new BaseResponse<RamDto>((int)HttpStatusCode.OK, ramDto);
        }

        public async Task<BaseResponse<ComputerDto>> CreateComputer(CreateComputer request)
        {
            var (errors, isValid) = EntityValidator.Validate(request);

            if (!isValid)
            {
                return new BaseResponse<ComputerDto>((int)CustomHttpStatusCode.UnprocessableEntity, string.Join("", errors));
            }

            var computer = new ComputerBuilder().CreateComputer(request.Name, request.Price)
                                        .WithGraphicsCards(_mapper.Map<IEnumerable<Domain.GraphicsCard>>(request.GraphicsCard))
                                        .WithRams(_mapper.Map<IEnumerable<Ram>>(request.Ram))
                                        .Build();

            await _computerRepository.Create(computer);

            var computerDto = _mapper.Map<ComputerDto>(computer);

            return new BaseResponse<ComputerDto>((int)HttpStatusCode.Created, computerDto);
        }

        public async Task<BaseResponse> DeleteComputer(Guid computerId)
        {
            if (!await _computerRepository.Exists(computerId))
            {
                return new BaseResponse((int)HttpStatusCode.NotFound);
            }

            var computer = await _computerRepository.Get(computerId);

            await _computerRepository.Delete(computer);

            return new BaseResponse((int)HttpStatusCode.NoContent);
        }

        public async Task<BaseResponse<ComputerDto>> UpdateComputer(Guid computerId, UpdateComputer request)
        {
            var (errors, isValid) = EntityValidator.Validate(request);

            if (!isValid)
            {
                return new BaseResponse<ComputerDto>((int)CustomHttpStatusCode.UnprocessableEntity, string.Join("", errors));
            }

            if (!await _computerRepository.Exists(computerId))
            {
                return new BaseResponse<ComputerDto>((int)HttpStatusCode.NotFound);
            }

            var computer = await _computerRepository.Get(computerId);
            computer.Update(request.Name, request.Price);
            await _computerRepository.Update(computer);

            return new BaseResponse<ComputerDto>((int)HttpStatusCode.Created, _mapper.Map<ComputerDto>(computer));
        }

        public async Task<BaseResponse<ComputerDto>> Patch(Guid computerId, JsonPatchDocument<ComputerDto> patchDocument)
        {
            if (!await _computerRepository.Exists(computerId))
            {
                return new BaseResponse<ComputerDto>((int)HttpStatusCode.NotFound);
            }

            var computer = await _computerRepository.Get(computerId);

            var computerToPatch = _mapper.Map<ComputerDto>(computer);

            patchDocument.ApplyTo(computerToPatch);

            computer.Update(computerToPatch.Name, computerToPatch.Price);

            await _computerRepository.Update(computer);

            return new BaseResponse<ComputerDto>((int)HttpStatusCode.Created, _mapper.Map<ComputerDto>(computer));
        }
    }

  
}
