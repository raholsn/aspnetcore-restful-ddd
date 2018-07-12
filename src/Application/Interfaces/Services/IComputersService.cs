using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Helpers;

using API.Contracts;
using API.Contracts.Dtos;


using Microsoft.AspNetCore.JsonPatch;

using Shared;

namespace Application.Interfaces.Services
{
    public interface IComputersService
    {
        Task<BaseResponse<ComputerDto>> GetComputer(Guid id);

        Task<BaseResponse<PagedList<ComputerDto>>> GetComputers(MetadataResourceParameters metadataParameters);

        Task<BaseResponse<IEnumerable<GraphicsCardDto>>> GetGraphicsCardsForComputer(Guid computerId);

        Task<BaseResponse<GraphicsCardDto>> GetGraphicsCardForComputer(Guid computerId,Guid graphicCardId);

        Task<BaseResponse<IEnumerable<RamDto>>> GetRamsForComputer(Guid computerId);

        Task<BaseResponse<RamDto>> GetRamForComputer(Guid computerId,Guid ramId);

        Task<BaseResponse<ComputerDto>> CreateComputer(CreateComputer request);

        Task<BaseResponse> DeleteComputer(Guid computerId);

        Task<BaseResponse<ComputerDto>> UpdateComputer(Guid id,UpdateComputer request);

        Task<BaseResponse<ComputerDto>> Patch(Guid computerId, JsonPatchDocument<ComputerDto> patchoDocument);
    }
}
