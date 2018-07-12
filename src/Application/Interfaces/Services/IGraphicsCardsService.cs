using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Helpers;

using API.Contracts;
using API.Contracts.Dtos;

namespace Application.Interfaces.Services
{
    public interface IGraphicsCardsService
    {
        Task<BaseResponse<IEnumerable<GraphicsCardDto>>> Get(MetadataResourceParameters metadataParameters);

        Task<BaseResponse<GraphicsCardDto>> Get(Guid id);

        Task<BaseResponse<GraphicsCardDto>> Update(Guid computerId, Guid id, UpdateGraphicsCard command);

        Task<BaseResponse<IEnumerable<GraphicsCardDto>>> Create(Guid computerId, IEnumerable<CreateGraphicsCard> command);

        Task<BaseResponse<IEnumerable<GraphicsCardDto>>> Get(IEnumerable<Guid> graphicsCardId);
    }
}
