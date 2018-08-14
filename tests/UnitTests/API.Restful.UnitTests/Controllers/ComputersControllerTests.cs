using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interfaces.Services;
using API.Contracts.Dtos;
using API.Restful.Controllers;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace API.Restful.UnitTests.Controllers
{
    public class ComputersControllerTests
    {
        private readonly ComputersController _controller;
        private readonly Mock<IComputersService> _computersServiceMock;
        private readonly Mock<IGraphicsCardsService> _graphicsCardsServiceMock;
        private readonly Mock<IUrlHelper> _urlHelperMock;
        private readonly Mock<IMapper> _mapperMock;

        public ComputersControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _computersServiceMock = new Mock<IComputersService>();
            _graphicsCardsServiceMock = new Mock<IGraphicsCardsService>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _controller = new ComputersController(_computersServiceMock.Object,_graphicsCardsServiceMock.Object,_urlHelperMock.Object,_mapperMock.Object);
        }

        [Theory]
        [AutoData]
        public async Task Get_Computers(BaseResponse<ComputerDto> _response)
        {

        }
    }
}
