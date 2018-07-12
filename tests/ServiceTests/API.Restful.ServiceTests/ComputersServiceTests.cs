using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using API.Contracts;
using API.Contracts.Dtos;
using API.Contracts.Headers;

using AutoFixture;
using AutoFixture.Xunit2;

using FluentAssertions;

using Microsoft.AspNetCore.JsonPatch;

using Newtonsoft.Json;

using Shared;

using Xunit;

namespace API.Restful.ServiceTests
{
    public class ComputersServiceTests : ServiceTestsBase
    {
        private string ResourceKey = "/api/v1/computers";

        public class Get : ComputersServiceTests
        {
            private IFixture _fixture;
            public Get()
            {
                _fixture = new Fixture();
            }

            [Fact]
            public async Task Get_Computers()
            {
                var response = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await response.Content.ReadAsStringAsync());

                result.Should().NotBeNullOrEmpty();

                int defaultPageSize = 10;

                Assert.Equal(result.Count(), defaultPageSize);
            }

            [Theory]
            [InlineData(2, 5)]
            public async Task Get_Computers_Paging_Should_Return_Correct_Amount(int pageNumber, int pageSize)
            {
                var response = await base.HttpClient.GetAsync(ResourceKey + "?pageNumber=" + pageNumber + "&pageSize=" + pageSize);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await response.Content.ReadAsStringAsync());

                result.Should().NotBeNullOrEmpty();

                Assert.Equal(result.Count(), pageSize);
            }

            [Theory]
            [InlineData(1, 2)]
            public async Task Get_Computers_With_QueryStringParameter_Pagination_Should_Work(int pageNumber, int pageSize)
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey + "?pagenumber=" + pageNumber + "&pageSize=" + pageSize);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var pagination = JsonConvert.DeserializeObject<Pagination>(responses.Headers.GetValues("x-Pagination").First());

                responses = await base.HttpClient.GetAsync(pagination.NextPageLink);

                pagination = JsonConvert.DeserializeObject<Pagination>(responses.Headers.GetValues("x-Pagination").First());

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);
                Assert.Equal(pageSize, pagination.PageSize);
                Assert.Equal((pageNumber + 1), pagination.CurrentPage);
                Assert.NotNull(pagination.PreviousPageLink);
            }

            [Theory]
            [AutoData]
            public async Task Get_Computers_With_QueryStringParameter_SearchQuery_Should_Return_Correct_Amount(string computerName)
            {
                var computers = _fixture.CreateMany<CreateComputer>();

                foreach (var createComputer in computers)
                {
                    createComputer.Name = computerName;
                    await HttpClient.PostAsync(ResourceKey, new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));
                }

                var responses = await base.HttpClient.GetAsync(ResourceKey + "?searchQuery=" + computerName);
                var result = JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                Assert.Equal(computers.Count(), result.Count());
                Assert.True(computers.All(x => x.Name == computerName));
            }

            [Theory]
            [InlineData(1, 2)]
            public async Task Get_Computers_With_QueryStringParameter_Pagination_And_Filter_Should_Work(int pageNumber, int pageSize)
            {
                var computers = _fixture.CreateMany<CreateComputer>();
                var computerName = _fixture.Create<string>();

                foreach (var createComputer in computers)
                {
                    createComputer.Name = computerName;
                    await HttpClient.PostAsync(ResourceKey, new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));
                }

                var responses = await base.HttpClient.GetAsync(ResourceKey + "?filter=" + computerName + "&pagenumber=" + pageNumber + "&pageSize=" + pageSize);

                var result = JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                Assert.Equal(pageSize, result.Count());

                var pagination = JsonConvert.DeserializeObject<Pagination>(responses.Headers.GetValues("x-Pagination").First());

                responses = await base.HttpClient.GetAsync(pagination.NextPageLink);

                result = JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());
                Assert.Single(result);

                pagination = JsonConvert.DeserializeObject<Pagination>(responses.Headers.GetValues("x-Pagination").First());
                Assert.Null(pagination.NextPageLink);
            }

            [Fact]
            public async Task Get_Computer()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                computers.Should().NotBeNullOrEmpty();

                var response = await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                computer.Should().NotBeNull();
            }

            [Fact]
            public async Task Get_Computers_GraphicsCards()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                computers.Should().NotBeNullOrEmpty();

                var response = await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id + "/graphicscards");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var graphicsCards =
                    JsonConvert.DeserializeObject<IEnumerable<GraphicsCardDto>>(await response.Content.ReadAsStringAsync());

                graphicsCards.Should().NotBeNull();
            }

            [Fact]
            public async Task Get_Computer_GraphicsCard()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                computers.Should().NotBeNullOrEmpty();

                var response = await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id + "/graphicscards/" +
                                                              computers.First().GraphicCards.First().Id);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var graphicsCards = JsonConvert.DeserializeObject<GraphicsCardDto>(await response.Content.ReadAsStringAsync());

                graphicsCards.Should().NotBeNull();
            }

            [Fact]
            public async Task Get_Computers_Rams()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                computers.Should().NotBeNullOrEmpty();

                var response = await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id + "/rams");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var rams = JsonConvert.DeserializeObject<IEnumerable<RamDto>>(await response.Content.ReadAsStringAsync());

                rams.Should().NotBeNull();
            }

            [Fact]
            public async Task Get_Computer_Ram()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                computers.Should().NotBeNullOrEmpty();

                var response =
                    await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id + "/rams/" +
                                                   computers.First().Rams.First().Id);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var rams = JsonConvert.DeserializeObject<RamDto>(await response.Content.ReadAsStringAsync());

                rams.Should().NotBeNull();
            }

            [Fact]
            public async Task Get_Computer_Not_Found()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey + "/" + Guid.NewGuid());

                Assert.Equal(HttpStatusCode.NotFound, responses.StatusCode);
            }

            [Fact]
            public async Task Get_Computers_GraphicsCard_Not_Found()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                var response =
                    await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id + "/graphicscards/" + Guid.NewGuid());

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task Get_Computers_Ram_Not_Found()
            {
                var responses = await base.HttpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);

                var computers =
                    JsonConvert.DeserializeObject<IEnumerable<ComputerDto>>(await responses.Content.ReadAsStringAsync());

                var response =
                    await base.HttpClient.GetAsync(ResourceKey + "/" + computers.First().Id + "/rams/" + Guid.NewGuid());

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
           
        }

        public class Get_HAL : ComputersServiceTests
        {
            private readonly HttpClient _httpClient;
            public Get_HAL()
            {
                _httpClient = base.HttpClient;
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/hal+json"));
            }

            [Fact]
            public async Task Get_Computers_With_Hal_Should_Have_Valid_Links()
            {
                var response = await _httpClient.GetAsync(ResourceKey);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<LinkedCollectionResourceWrapperDto<ComputerDto>>(await response.Content.ReadAsStringAsync());

                result.Value.Should().NotBeNullOrEmpty();

                var self = result.Links.FirstOrDefault(x => x.Rel == "self");

                Assert.NotNull(self);
                Assert.Equal(HttpMethod.Get.ToString(), self.Method, ignoreCase: true);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1), self.Href);
            }

            [Fact]
            public async Task Get_Computer_With_Hal_Should_Have_Valid_Links()
            {
                var response = await _httpClient.GetAsync(ResourceKey);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<LinkedCollectionResourceWrapperDto<ComputerDto>>(await response.Content.ReadAsStringAsync());
                result.Value.Should().NotBeNullOrEmpty();

                var self = result.Value.First().Links.FirstOrDefault(x => x.Rel == "self");

                response = await _httpClient.GetAsync(self.Href);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                self = computer.Links.FirstOrDefault(x => x.Rel == "self");
                var delete = computer.Links.FirstOrDefault(x => x.Rel == "delete");
                var put = computer.Links.FirstOrDefault(x => x.Rel == "update");
                var patch = computer.Links.FirstOrDefault(x => x.Rel == "patch");

                Assert.NotNull(self);
                Assert.Equal(HttpMethod.Get.ToString(), self.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, self.Href);

                Assert.NotNull(delete);
                Assert.Equal(HttpMethod.Delete.ToString(), delete.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, delete.Href);

                Assert.NotNull(put);
                Assert.Equal(HttpMethod.Put.ToString(), put.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, put.Href);

                Assert.NotNull(patch);
                Assert.Equal("PATCH", patch.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, patch.Href);
            }

            [Theory]
            [AutoData]
            public async Task Update_Computer_With_Hal_Should_Have_Valid_Links(UpdateComputer updateComputer)
            {
                var response = await _httpClient.GetAsync(ResourceKey);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<LinkedCollectionResourceWrapperDto<ComputerDto>>(await response.Content.ReadAsStringAsync());

                response = await _httpClient.GetAsync(result.Value.First().Links.First(x => x.Rel == "self").Href);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                response = await _httpClient.PutAsync(computer.Links.First(x => x.Rel == "update").Href,
                    new StringContent(JsonConvert.SerializeObject(updateComputer), Encoding.UTF8, "application/json"));

                var updatedComputer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                var self = computer.Links.FirstOrDefault(x => x.Rel == "self");
                var delete = computer.Links.FirstOrDefault(x => x.Rel == "delete");
                var put = computer.Links.FirstOrDefault(x => x.Rel == "update");
                var patch = computer.Links.FirstOrDefault(x => x.Rel == "patch");

                Assert.NotNull(self);
                Assert.Equal(HttpMethod.Get.ToString(), self.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, self.Href);

                Assert.NotNull(delete);
                Assert.Equal(HttpMethod.Delete.ToString(), delete.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, delete.Href);

                Assert.NotNull(put);
                Assert.Equal(HttpMethod.Put.ToString(), put.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, put.Href);

                Assert.NotNull(patch);
                Assert.Equal("PATCH", patch.Method);
                Assert.Equal(_httpClient.BaseAddress + ResourceKey.Substring(1) + "/" + computer.Id, patch.Href);

            }
        }

        public class Post : ComputersServiceTests
        {
            [Theory]
            [AutoData]
            public async Task Create_Computer(CreateComputer createComputer)
            {
                var response = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                computer.Should().NotBeNull();

                response = await HttpClient.GetAsync(response.Headers.Location);

                computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                computer.Name.Should().Be(createComputer.Name);
                computer.Price.Should().Be(createComputer.Price);
                foreach (var graphicsCard in createComputer.GraphicsCard)
                {
                    Assert.Contains(createComputer.GraphicsCard, x => x.Name == graphicsCard.Name);
                    Assert.Contains(createComputer.GraphicsCard, x => x.Price.Equals(graphicsCard.Price));
                }

                foreach (var ram in createComputer.Ram)
                {
                    Assert.Contains(createComputer.Ram, x => x.Name == ram.Name);
                    Assert.Contains(createComputer.Ram, x => x.Price.Equals(ram.Price));
                }
            }

            [Theory]
            [AutoData]
            public async Task Create_Computer_Should_Return_UnProcessableEntity(CreateComputer createComputer)
            {
                createComputer.Name = new String('a', 256);
                var response = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(422, (int)response.StatusCode);
            }

            [Theory]
            [AutoData]
            public async Task Create_GraphicsCard(CreateComputer createComputer,
                IEnumerable<CreateGraphicsCard> createGraphicsCard)
            {
                var response = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());
                computer.Should().NotBeNull();

                var graphicsCardResponse = await HttpClient.PostAsync(ResourceKey + "/" + computer.Id + "/graphicscards",
                    new StringContent(JsonConvert.SerializeObject(createGraphicsCard), Encoding.UTF8, "application/json"));

                var graphicsCards =
                    JsonConvert.DeserializeObject<IEnumerable<GraphicsCardDto>>(
                        await graphicsCardResponse.Content.ReadAsStringAsync());

                Assert.NotNull(graphicsCards);

                response = await HttpClient.GetAsync(graphicsCardResponse.Headers.Location);

                graphicsCards =
                    JsonConvert.DeserializeObject<IEnumerable<GraphicsCardDto>>(await response.Content.ReadAsStringAsync());

                foreach (var card in graphicsCards)
                {
                    Assert.Contains(createGraphicsCard, x => x.Name == card.Name);
                }
            }

            [Theory]
            [AutoData]
            public async Task Create_GraphicsCard_Should_Return_UnProcessableEntity(CreateComputer createComputer, IEnumerable<CreateGraphicsCard> createGraphicsCard)
            {
                var response = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());
                computer.Should().NotBeNull();

                createGraphicsCard.First().Name = new String('a', 256);

                var graphicsCardResponse = await HttpClient.PostAsync(ResourceKey + "/" + computer.Id + "/graphicscards",
                    new StringContent(JsonConvert.SerializeObject(createGraphicsCard), Encoding.UTF8, "application/json"));

                Assert.Equal(422, (int)graphicsCardResponse.StatusCode);
            }
        }

        public class Put : ComputersServiceTests
        {
            [Theory]
            [AutoData]
            public async Task Update_Computer(CreateComputer createComputer, UpdateComputer updateComputer)
            {
                var postResponse = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await postResponse.Content.ReadAsStringAsync());

                var putResult = await HttpClient.PutAsync(ResourceKey + "/" + computer.Id,
                    new StringContent(JsonConvert.SerializeObject(updateComputer), Encoding.UTF8, "application/json"));
                putResult = await HttpClient.GetAsync(putResult.Headers.Location);

                computer = JsonConvert.DeserializeObject<ComputerDto>(await putResult.Content.ReadAsStringAsync());

                computer.Name.Should().Be(updateComputer.Name);
                computer.Price.Should().Be(updateComputer.Price);
                foreach (var graphicsCard in createComputer.GraphicsCard)
                {
                    Assert.Contains(createComputer.GraphicsCard, x => x.Name == graphicsCard.Name);
                    Assert.Contains(createComputer.GraphicsCard, x => x.Price.Equals(graphicsCard.Price));
                }

                foreach (var ram in createComputer.Ram)
                {
                    Assert.Contains(createComputer.Ram, x => x.Name == ram.Name);
                    Assert.Contains(createComputer.Ram, x => x.Price.Equals(ram.Price));
                }
            }

            [Theory]
            [AutoData]
            public async Task Update_Computer_Should_Return_UnProcessableEntity(CreateComputer createComputer,
                UpdateComputer updateComputer)
            {
                var postResponse = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await postResponse.Content.ReadAsStringAsync());

                updateComputer.Name = new String('a', 256);

                var putResult = await HttpClient.PutAsync(ResourceKey + "/" + computer.Id,
                    new StringContent(JsonConvert.SerializeObject(updateComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(422, (int)putResult.StatusCode);
            }

            [Theory]
            [AutoData]
            public async Task Update_GraphicsCard(CreateComputer createComputer, UpdateGraphicsCard updateGraphicsCardCommand)
            {
                var postResponse = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await postResponse.Content.ReadAsStringAsync());

                var putResult =
                    await HttpClient.PutAsync(
                        ResourceKey + "/" + computer.Id + "/graphicscards/" + computer.GraphicCards.First().Id,
                        new StringContent(JsonConvert.SerializeObject(updateGraphicsCardCommand), Encoding.UTF8, "application/json"));
                putResult = await HttpClient.GetAsync(putResult.Headers.Location);

                var updatedGraphicsCard =
                    JsonConvert.DeserializeObject<GraphicsCardDto>(await putResult.Content.ReadAsStringAsync());

                updatedGraphicsCard.Should().NotBeNull();

                Assert.Equal(updatedGraphicsCard.Name, updatedGraphicsCard.Name);
                Assert.Equal(updatedGraphicsCard.Price, updatedGraphicsCard.Price);
            }

            [Theory]
            [AutoData]
            public async Task Update_GraphicsCard_Should_Return_UnProcessableEntity(CreateComputer createComputer,
                UpdateGraphicsCard updateGraphicsCardCommand)
            {
                var postResponse = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

                updateGraphicsCardCommand.Name = new String('a', 256);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await postResponse.Content.ReadAsStringAsync());

                var putResult = await HttpClient.PutAsync(
                    ResourceKey + "/" + computer.Id + "/graphicscards/" + computer.GraphicCards.First().Id,
                    new StringContent(JsonConvert.SerializeObject(updateGraphicsCardCommand), Encoding.UTF8, "application/json"));

                Assert.Equal(422, (int)putResult.StatusCode);
            }
        }

        public class Patch : ComputersServiceTests
        {
            [Theory]
            [AutoData]
            public async Task Patch_Computer_Name_Replace(CreateComputer createComputer, string newName)
            {
                var response = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());
                computer.Should().NotBeNull();

                var jsonPatchDocument = new JsonPatchDocument<ComputerDto>();
                jsonPatchDocument.Replace(dto => dto.Name, newName);

                var patchResponse = await HttpClient.PatchAsync(ResourceKey + "/" + computer.Id,
                    new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8,
                        "application/json-patch+json"));
                computer = JsonConvert.DeserializeObject<ComputerDto>(await patchResponse.Content.ReadAsStringAsync());

                Assert.Equal(computer.Name, newName);
            }
        }

        public class Delete : ComputersServiceTests
        {
            [Theory]
            [AutoData]
            public async Task Delete_Computer(CreateComputer createComputer)
            {
                var response = await HttpClient.PostAsync(ResourceKey,
                    new StringContent(JsonConvert.SerializeObject(createComputer), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var computer = JsonConvert.DeserializeObject<ComputerDto>(await response.Content.ReadAsStringAsync());

                computer.Should().NotBeNull();

                response = await HttpClient.DeleteAsync(ResourceKey + "/" + computer.Id);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        public class HEAD : ComputersServiceTests
        {
            [Theory]
            [InlineData("api-supported-versions")]
            public async Task Head_Should_Return_Correct_Headers(string expectedHeader)
            {
                var response = await HttpClient.HeadAsync(ResourceKey, null);

                Assert.True(response.Headers.Contains("api-supported-versions"));
            }
        }

        public class Options : ComputersServiceTests
        {
            [Theory]
            [InlineData("GET")]
            [InlineData("POST")]
            [InlineData("PUT")]
            [InlineData("PATCH")]
            [InlineData("OPTIONS")]
            [InlineData("HEAD")]
            public async Task Options_Contains_GET_POST_PUT_PATCH_OPTIONS_HEAD(string expectedAllowHeader)
            {
                var response = await HttpClient.OptionsAsync(ResourceKey, null);

                Assert.True(response.Content.Headers.Allow.Contains(expectedAllowHeader));
            }
        }
    }
}
