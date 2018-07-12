using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Application.Helpers;

using API.Contracts;
using API.Contracts.Dtos;

using AutoMapper;

using Domain.Builder;

using Application.Interfaces.Services;
using Application.Interfaces.Repositories;

namespace Application.Services.GraphicsCard
{
    public class GraphicsCardsService : IGraphicsCardsService
    {
        private readonly IGraphicsCardRepository _graphicsCardRepository;
        private readonly IMapper _mapper;
        private readonly IGraphicsCardBuilder _builder;


        public GraphicsCardsService(IGraphicsCardRepository graphicsCardRepository, IMapper mapper, IGraphicsCardBuilder builder)
        {
            _graphicsCardRepository = graphicsCardRepository;
            _mapper = mapper;
            _builder = builder;
        }

        public async Task<BaseResponse<IEnumerable<GraphicsCardDto>>> Create(Guid computerId, IEnumerable<CreateGraphicsCard> command)
        {
            var createGraphicsCards = command as CreateGraphicsCard[] ?? command.ToArray();

            foreach (var createGraphicsCard in createGraphicsCards)
            {
                var (errors, isValid) = EntityValidator.Validate(createGraphicsCard);

                if (!isValid)
                {
                    return new BaseResponse<IEnumerable<GraphicsCardDto>>((int)CustomHttpStatusCode.UnprocessableEntity, string.Join("", errors));
                }
            }

            var graphicsCards = createGraphicsCards.Select(createGraphicsCard => _builder.CreateGraphicsCard(createGraphicsCard.Price, createGraphicsCard.Name, computerId)
                                                                             .Build()).ToList();

            await _graphicsCardRepository.Create(graphicsCards);
            return new BaseResponse<IEnumerable<GraphicsCardDto>>((int)HttpStatusCode.Created, _mapper.Map<IEnumerable<GraphicsCardDto>>(graphicsCards));

        }

        public async Task<BaseResponse<IEnumerable<GraphicsCardDto>>> Get(IEnumerable<Guid> graphicsCardId)
        {
            var cardId = graphicsCardId as Guid[] ?? graphicsCardId.ToArray();

            var graphicsCards = await _graphicsCardRepository.Get(cardId);

            if (graphicsCards.Count() != cardId.Count())
            {
                return new BaseResponse<IEnumerable<GraphicsCardDto>>((int)HttpStatusCode.NotFound);
            }

            return new BaseResponse<IEnumerable<GraphicsCardDto>>((int)HttpStatusCode.OK, _mapper.Map<IEnumerable<GraphicsCardDto>>(graphicsCards));
        }

        public async Task<BaseResponse<IEnumerable<GraphicsCardDto>>> Get(MetadataResourceParameters parameters)
        {
            var graphicsCards = await _graphicsCardRepository.Get(parameters.PageNumber, parameters.PageSize, parameters.Filter, parameters.SearchQuery);
            return new BaseResponse<IEnumerable<GraphicsCardDto>>((int)HttpStatusCode.OK, _mapper.Map<IEnumerable<GraphicsCardDto>>(graphicsCards));
        }

        public async Task<BaseResponse<GraphicsCardDto>> Get(Guid id)
        {
            if (!await _graphicsCardRepository.Exists(id))
            {
                return new BaseResponse<GraphicsCardDto>((int)HttpStatusCode.NotFound);
            }

            var graphicsCard = await _graphicsCardRepository.Get(id);
            return new BaseResponse<GraphicsCardDto>((int)HttpStatusCode.OK, _mapper.Map<GraphicsCardDto>(graphicsCard));
        }

        public async Task<BaseResponse<GraphicsCardDto>> Update(Guid computerId, Guid graphicsCardId, UpdateGraphicsCard command)
        {
            var (errors, isValid) = EntityValidator.Validate(command);

            if (!isValid)
            {
                return new BaseResponse<GraphicsCardDto>((int)CustomHttpStatusCode.UnprocessableEntity, string.Join("", errors));
            }

            if (!await _graphicsCardRepository.Exists(computerId, graphicsCardId))
            {
                return new BaseResponse<GraphicsCardDto>((int)HttpStatusCode.NotFound);
            }

            var graphicsCard = _builder.UpdateGraphicsCard(graphicsCardId, command.Price, command.Name, computerId)
                                       .Build();

            graphicsCard.Update(command.Name, command.Price);

            await _graphicsCardRepository.Update(graphicsCard);

            return new BaseResponse<GraphicsCardDto>((int)HttpStatusCode.Created, _mapper.Map<GraphicsCardDto>(graphicsCard));
        }
    }
}
