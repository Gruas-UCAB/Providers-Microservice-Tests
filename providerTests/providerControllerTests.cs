using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using ProvidersMicroservice.src.crane.application.commands.create_crane;
using ProvidersMicroservice.src.crane.application.commands.create_crane.types;
using ProvidersMicroservice.src.crane.application.commands.create_provider;
using ProvidersMicroservice.src.crane.application.commands.create_provider.types;
using ProvidersMicroservice.src.provider.application.commands.update_conductor.types;
using ProvidersMicroservice.src.provider.application.commands.update_conductor;
using ProvidersMicroservice.src.provider.application.repositories;
using ProvidersMicroservice.src.provider.application.repositories.dto;
using ProvidersMicroservice.src.provider.domain;
using ProvidersMicroservice.src.provider.domain.entities.conductor;
using ProvidersMicroservice.src.provider.domain.entities.conductor.value_objects;
using ProvidersMicroservice.src.provider.domain.entities.crane;
using ProvidersMicroservice.src.provider.domain.entities.crane.value_objects;
using ProvidersMicroservice.src.provider.domain.value_objects;
using ProvidersMicroservice.src.provider.infrastructure;
using ProvidersMicroservice.src.provider.infrastructure.dto;
using ProvidersMicroservice.src.provider.infrastructure.validators;
using ProvidersMicroservice.src.providers.application.commands.create_provider.types;
using RestSharp;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UsersMicroservice.core.Application;
using UsersMicroservice.core.Common;
using UsersMicroservice.Core.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using ProvidersMicroservice.src.provider.application.commands.update_crane.types;
using ProvidersMicroservice.src.provider.application.commands.create_conductor;
using ProvidersMicroservice.src.provider.application.commands.create_conductor.types;
using FluentValidation;
using MongoDB.Bson;
using ProvidersMicroservice.src.provider.infrastructure.repositories;
using ProvidersMicroservice.src.provider.application.repositories.exceptions;

namespace TestProvidersMicroservice.providerTests
{
    public class ProviderControllerTests
    {
        private readonly Mock<IProviderRepository> _providerRepositoryMock;
        private readonly Mock<IIdGenerator<string>> _idGeneratorMock;
        private readonly Mock<IRestClient> _restClientMock;
        private readonly ProviderController _controller;
        private readonly Mock<IMongoCollection<BsonDocument>> _providerCollectionMock;
       

        public ProviderControllerTests()
        {
            _providerRepositoryMock = new Mock<IProviderRepository>();
            _idGeneratorMock = new Mock<IIdGenerator<string>>();
            _restClientMock = new Mock<IRestClient>();
            _providerCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            _controller = new ProviderController(_providerRepositoryMock.Object, _idGeneratorMock.Object, _restClientMock.Object);
        }

        [Fact(DisplayName = "Test When CreateProvider Method Resturns IsSuccessful")]
        public async Task CreateProvider_ValidCommand_IsSuccessful()
        {

            // Arrange 

            var proviId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var command = new CreateProviderCommand(proviId, "J123456789", "ProviderName", "urldelaimagen");
            var validator = new CreateProviderCommandValidator();
            var token = "Bearer token";
            var providerTest = Provider.Create(
                  new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                  new ProviderName("ProviderName"),
                  new ProviderRif("J123456789"),
                  new ProviderImage("urldelaimagen"),
                  new List<Conductor>(),
                  new List<Crane>()
                  );
            var userExistsRequestp = new RestRequest($"https://localhost:5350/user/{command.Id}", Method.Get);
            userExistsRequestp.AddHeader("Authorization", token);
           var userExistsRequest = _restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), default))
                         .ReturnsAsync(new RestResponse
                         {
                             StatusCode = HttpStatusCode.OK,
                             Content = @"{
                                       ""id"": ""0fdceb8f-217f-4cdc-b983-7e9815187dce"",
                                       ""name"": ""test user"",
                                       ""phone"": ""+584242374797"",
                                       ""role"": ""provider"",
                                       ""department"": ""5fb9be1e-37a6-457b-8718-6a832185b5d3"",
                                        ""isActive"": true
                                        }"

                         });
            

            _providerRepositoryMock.Setup(repo => repo.SaveProvider(It.IsAny<Provider>())).ReturnsAsync(providerTest);
            var service = new CreateProviderCommandHandler(_providerRepositoryMock.Object);

            // Act

            var result = await service.Execute(command);
            var oKresult = await _controller.CreateProvider(command, token);

            //Arrange
            Assert.True(result.IsSuccessful);
            Assert.Equal(proviId, result.Unwrap().Id);
          


        }

        [Fact(DisplayName = "Test When CreateProvider Method Resturns IsFailure")]
        public async Task CreateProvider_ValidCommand_IsFailure()
        {

            // Arrange 
            var command = new CreateProviderCommand("invalid", "J123456789", "ProviderName", "urldelaimagen");
            var validator = new CreateProviderCommandValidator();
            var token = "Bearer token";
            var providerTest = Provider.Create(
                  new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                  new ProviderName("ProviderName"),
                  new ProviderRif("J123456789"),
                  new ProviderImage("urldelaimagen"),
                  new List<Conductor>(),
                  new List<Crane>()
                  );
            var userExistsRequestp = new RestRequest($"https://localhost:5350/user/{command.Id}", Method.Get);
            userExistsRequestp.AddHeader("Authorization", token);
            var userExistsRequest = _restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), default))
                          .ReturnsAsync(new RestResponse
                          {
                              StatusCode = HttpStatusCode.OK,
                              Content = @"{
                                       ""id"": ""0fdceb8f-217f-4cdc-b983-7e9815187dce"",
                                       ""name"": ""test user"",
                                       ""phone"": ""+584242374797"",
                                       ""role"": ""provider"",
                                       ""department"": ""5fb9be1e-37a6-457b-8718-6a832185b5d3"",
                                        ""isActive"": true
                                        }"

                          });


            _providerRepositoryMock.Setup(repo => repo.SaveProvider(It.IsAny<Provider>())).ReturnsAsync(providerTest);
            var service = new CreateProviderCommandHandler(_providerRepositoryMock.Object);

            // Act

            var result = await service.Execute(command);
            var oKresult = await _controller.CreateProvider(command, token);

            //Arrange
            Assert.False(result.IsSuccessful);

        }

        [Fact(DisplayName = "Test When CreateConductor Method Resturns IsSuccessful")]
        public async Task CreateConductor_ValidCommand_IsSuccessful()
        {

            // Arrange 

            var proviId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var condId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var craneId = "98c9326e-9ab1-47b8-a8d1-ddb6359859f7";
            var conductors = new List<Conductor>();
            var cranes = new List<Crane>();

            var command = new CreateConductorCommand(proviId, condId, 28052110 , "urldelafoto", "39.7128,-71.0060", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7");
            var validator = new CreateConductorCommandValidator(); ;
            var token = "Bearer token";
            var conductorTest = new Conductor(
                                new ConductorId(condId),
                                new ConductorDni(28052110),
                                new ConductorName("urldelafoto"),
                                new ConductorLocation("39.7128,-71.0060"),
                                new ConductorImage("Image1"),
                                new CraneId(craneId)
                                );
   
            
            var providerTest = Provider.Create(
                   new ProviderId(proviId),
                   new ProviderName("ProviderName"),
                   new ProviderRif("J123456789"),
                   new ProviderImage("urldelaimagen"),
                   conductors = new List<Conductor>
                           {
                           new Conductor(
                               new ConductorId(proviId),
                               new ConductorDni(28052110),
                               new ConductorName("urldelafoto"),
                               new ConductorLocation("39.7128,-71.0060"),
                               new ConductorImage("Image1"),
                               new CraneId(craneId))
                            },
                   cranes = new List<Crane>
                            {
                            new Crane(
                                new CraneId(craneId),
                                new CraneBrand("brand"),
                                new CraneModel("model"),
                                new CranePlate("V24C11G"),
                                new CraneType("medium"),
                                new CraneYear(2023))
                            }
                    );
            var saveCondDto = new SaveConductorDto(new ProviderId(proviId),conductorTest);
            var userExistsRequestp = new RestRequest($"https://localhost:5350/user/{command.ConductorId}", Method.Get);
            userExistsRequestp.AddHeader("Authorization", token);
            var userExistsRequest = _restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), default))
                          .ReturnsAsync(new RestResponse
                          {
                              StatusCode = HttpStatusCode.OK,
                              Content = @"{
                                       ""id"": ""0fdceb8f-217f-4cdc-b983-7e9815187dce"",
                                       ""name"": ""test user"",
                                       ""phone"": ""+584242374797"",
                                       ""role"": ""conductor"",
                                       ""department"": ""5fb9be1e-37a6-457b-8718-6a832185b5d3"",
                                        ""isActive"": true
                                        }"

                          });
           
            _providerRepositoryMock.Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<Provider>.Of(providerTest));
            _providerRepositoryMock.Setup(repo => repo.SaveConductor(It.IsAny<SaveConductorDto>()))
                .ReturnsAsync(conductorTest);
            var service = new CreateConductorCommandHandler(_providerRepositoryMock.Object);
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            // Act

            var result = await service.Execute(command);
            var oKresult = await _controller.CreateConductor(command, token);

            //Arrange
           Assert.True(result.IsSuccessful);
           Assert.Equal(condId, result.Unwrap().Id);

        }

        [Fact(DisplayName = "Test When CreateConductor Method Resturns IsFailure")]
        public async Task CreateConductor_ValidCommand_IsFailure()
        {

            // Arrange 

            var proviId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var condId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var craneId = "98c9326e-9ab1-47b8-a8d1-ddb6359859f7";
            var conductors = new List<Conductor>();
            var cranes = new List<Crane>();
            var command = new CreateConductorCommand(proviId, condId, 28052110, "urldelafoto", "39.7128,-71.0060", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7");
            var validator = new CreateConductorCommandValidator(); 
            var token = "Bearer token";
            var conductorTest = new Conductor(
                                new ConductorId(condId),
                                new ConductorDni(28052110),
                                new ConductorName("urldelafoto"),
                                new ConductorLocation("39.7128,-71.0060"),
                                new ConductorImage("Image1"),
                                new CraneId(craneId)
                                );
            var saveCondDto = new SaveConductorDto(new ProviderId(proviId), conductorTest);
            var userExistsRequestp = new RestRequest($"https://localhost:5350/user/{command.ConductorId}", Method.Get);
            userExistsRequestp.AddHeader("Authorization", token);
            var userExistsRequest = _restClientMock.Setup(client => client.ExecuteAsync(It.IsAny<RestRequest>(), default))
                          .ReturnsAsync(new RestResponse
                          {
                              StatusCode = HttpStatusCode.OK,
                              Content = @"{
                                       ""id"": ""0fdceb8f-217f-4cdc-b983-7e9815187dce"",
                                       ""name"": ""test user"",
                                       ""phone"": ""+584242374797"",
                                       ""role"": ""conductor"",
                                       ""department"": ""5fb9be1e-37a6-457b-8718-6a832185b5d3"",
                                        ""isActive"": true
                                        }"

                          });

            _providerRepositoryMock.Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<Provider>.Empty);
            var service = new CreateConductorCommandHandler(_providerRepositoryMock.Object);
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            // Act

            var result = await service.Execute(command);
            var oKresult = await _controller.CreateConductor(command, token);

            //Arrange
            Assert.False(result.IsSuccessful);

        }

        [Fact(DisplayName = "Test When CreateConductor CommandValidator Returns True ")]
        public void CreateConductor_ReturnsTrue_WhenCommandValidatorIsValid()
        {

            // Arrange 

            var proviId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var condId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var command = new CreateConductorCommand(proviId, condId, 28052110, "urldelafoto", "39.7128,-71.0060", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7");
            var validator = new CreateConductorCommandValidator();

            // Act
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();


            // Assert
            Assert.True(validationResult.IsValid);
            Assert.True(validationResult.Errors.Count == 0);


        }

        [Theory(DisplayName = "Test When CreateConductort CommandValidator Returns False ")]
        [InlineData("", "cfdceb8f-217f-4cdc-b983-7e9815187dce", 28052110, "urldelafoto", 
            "39.7128,-71.0060", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "cfdceb8f-217f-4cdc-b983-7e9815187dce", 0, "urldelafoto",
            "39.7128,-71.0060", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "cfdceb8f-217f-4cdc-b983-7e9815187dce", 28052110, "",
            "39.7128,-71.0060", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "cfdceb8f-217f-4cdc-b983-7e9815187dce", 28052110, "urldelafoto",
            "", "Image1", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "cfdceb8f-217f-4cdc-b983-7e9815187dce", 28052110, "urldelafoto",
            "39.7128,-71.0060", "", "98c9326e-9ab1-47b8-a8d1-ddb6359859f7")]
        public void CreateConductort_ReturnsFalse_WhenCommandValidatorIsNotValid(string proId, string condId, int rif, string foto,
            string location, string img, string craId  )
        {
            //Arrange
            var command = new CreateConductorCommand(proId, condId, rif, foto, location, img, craId);
            var validator = new CreateConductorCommandValidator();

            // Act
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            // Assert
            Assert.False(validationResult.IsValid);
        }


        [Fact(DisplayName = "Test When CreateProvider CommandValidator Returns True ")]
        public void  CreateProvider_ReturnsTrue_WhenCommandValidatorIsValid()
        {

            // Arrange 

            var proviId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var command = new CreateProviderCommand(proviId, "J123456789", "ProviderName", "urldelaimagen");
            var validator = new CreateProviderCommandValidator();

            // Act
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();


            // Assert
            Assert.True(validationResult.IsValid);
            Assert.True(validationResult.Errors.Count == 0);


        }

        [Theory(DisplayName = "Test When CreateProvider CommandValidator Returns False ")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "", "ProviderName", "urldelaimagen")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "J123456789", "", "urldelaimagen")]
        [InlineData("0fdceb8f-217f-4cdc-b983-7e9815187dce", "J123456789", "ProviderName", "")]
        public void CreateProvidert_ReturnsFalse_WhenCommandValidatorIsNotValid(string proviId, string proRif, string proName, string proImg )
        {
            // Arrange
            var command = new CreateProviderCommand(proviId, proRif, proName, proImg);
            var validator = new CreateProviderCommandValidator();

            // Act
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            // Assert
            Assert.False(validationResult.IsValid);
        }


        [Fact(DisplayName = "Test When CreateCrane Returns CreatedResult ")]
        public async Task CreateCrane_ReturnsCreated_WhenSuccessful()
        {
            // Arrange
            var command = new CreateCraneCommand("0fdceb8f-217f-4cdc-b983-7e9815187dce", "brand", "model", "V24C11G", "medium", 2023);
            var validator = new CreateCraneCommandValidator();
            var crane = new Crane(
                new CraneId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                new CraneBrand("brand"),
                new CraneModel("model"),
                new CranePlate("V24C11G"),
                new CraneType("medium"),
                new CraneYear(2023));
            var providerTest = Provider.Create(
                   new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ProviderName("ProviderName"),
                   new ProviderRif("J123456789"),
                   new ProviderImage("urldelaimagen"),
                   new List<Conductor>(),
                   new List<Crane>()
                   );

            _providerRepositoryMock.Setup(repo => repo.SaveCrane(It.IsAny<SaveCraneDto>())).ReturnsAsync(crane);
            _providerRepositoryMock.Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>())).ReturnsAsync(_Optional<Provider>.Of(providerTest));
            _idGeneratorMock.Setup(gen => gen.GenerateId()).Returns("cfdceb8f-217f-4cdc-b983-7e9815187dce");
            _providerRepositoryMock.Setup(repo => repo.SaveProvider(It.IsAny<Provider>())).ReturnsAsync(providerTest);
            
            var service = new CreateCraneCommandHandler(_idGeneratorMock.Object, _providerRepositoryMock.Object);

            // Act
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            var response = await service.Execute(command);
            var oKresult = await _controller.CreateCrane(command);

            // Assert
            Assert.True(oKresult is CreatedResult);
        }


        [Fact(DisplayName = "Test When CreateCrane Returns BadRequestObjectResult ")]
        public async Task CreateCrane_ReturnsCreated_BadRequestObjectResult()
        {
            // Arrange
            var command = new CreateCraneCommand("", "brand", "model", "V24C11G", "medium", 2023);
            var validator = new CreateCraneCommandValidator();
            var crane = new Crane(
                new CraneId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                new CraneBrand("brand"),
                new CraneModel("model"),
                new CranePlate("V24C11G"),
                new CraneType("medium"),
                new CraneYear(2023));
            var providerTest = Provider.Create(
                   new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ProviderName("ProviderName"),
                   new ProviderRif("J123456789"),
                   new ProviderImage("urldelaimagen"),
                   new List<Conductor>(),
                   new List<Crane>()
                   );

            _providerRepositoryMock.Setup(repo => repo.SaveCrane(It.IsAny<SaveCraneDto>()))
                .ReturnsAsync(crane);
            _providerRepositoryMock.Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<Provider>.Empty);
            _idGeneratorMock.Setup(gen => gen.GenerateId())
                .Returns("cfdceb8f-217f-4cdc-b983-7e9815187dce");
            _providerRepositoryMock.Setup(repo => repo.SaveProvider(It.IsAny<Provider>()))
                .ReturnsAsync(providerTest);

            var service = new CreateCraneCommandHandler(_idGeneratorMock.Object, _providerRepositoryMock.Object);

            // Act
            var validationResult = validator.Validate(command);
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            var response = await service.Execute(command);
            var oKresult = await _controller.CreateCrane(command);

            // Assert
            Assert.True(oKresult is BadRequestObjectResult);
        }

        [Fact(DisplayName = "Test When GetAllProviders Returns OkObjectResult ")]
        public async Task GetAllProviders_ProvidersFound_ReturnsOk()
        {
            // Arrange
            var data = new GetAllProvidersDto();
            var providers = new List<Provider>
        {
            Provider.Create(
                  new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                  new ProviderName("ProviderName"),
                  new ProviderRif("J123456789"),
                  new ProviderImage("urldelaimagen"),
                  new List<Conductor>(),
                  new List<Crane>()

            )
        };

            _providerRepositoryMock
                .Setup(repo => repo.GetAllProviders(data))
                .ReturnsAsync(_Optional<List<Provider>>.Of(providers));

            // Act
            var result = await _controller.GetAllProviders(data);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);
        }

        [Fact(DisplayName = "Test When GetAllProviders Returns NotFoundObjectResult ")]
        public async Task GetAllProviders_NoProvidersFound_ReturnsNotFound()
        {
            // Arrange
            var data = new GetAllProvidersDto();
            _providerRepositoryMock
                .Setup(repo => repo.GetAllProviders(data))
                .ReturnsAsync(_Optional<List<Provider>>.Empty);

            // Act
            var result = await _controller.GetAllProviders(data);

            // Assert
            Assert.True(result is NotFoundObjectResult);
        }


        [Fact(DisplayName = "Test When GetAllActiveConductors Returns OKbjectResult ")]
        public async Task GetAllActiveConductors_ReturnsOk_WhenConductorsFound()
        {
            // Arrange
            var data = new GetAllConductorsDto();
            var conductors = new List<Conductor>
                            {
                           new Conductor(
                               new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"), 
                               new ConductorDni(28052110), 
                               new ConductorName("urldelafoto"), 
                               new ConductorLocation("39.7128,-71.0060"), 
                               new ConductorImage("Image1"), 
                               new CraneId("98c9326e-9ab1-47b8-a8d1-ddb6359859f7"))
                            };
            _providerRepositoryMock.Setup(repo => repo.GetAllActiveConductors(data))
                .ReturnsAsync(_Optional<List<Conductor>>.Of(conductors));

            // Act
            var result = await _controller.GetAllActiveConductors(data);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);

        }

        [Fact(DisplayName = "Test When GetAllActiveConductors Returns NotFoundObjectResult ")]
        public async Task GetAllActiveConductors_ReturnsNotFound_WhenNoConductorsFound()
        {
            // Arrange
            var data = new GetAllConductorsDto();
            _providerRepositoryMock.Setup(repo => repo.GetAllActiveConductors(data))
                .ReturnsAsync(_Optional<List<Conductor>>.Empty());

            // Act
            var result = await _controller.GetAllActiveConductors(data);

            // Assert

            Assert.True(result is NotFoundObjectResult);
        }

        [Fact(DisplayName = "Test When GetAllConductors Returns  OkObjectResult ")]
        public async Task GetAllConductors_ReturnsOk_WhenConductorsFound()
        {
            // Arrange
            var data = new GetAllConductorsDto();
            var proviId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var command = new CreateProviderCommand(proviId, "J123456789", "ProviderName", "urldelaimagen");
            var conductors = new List<Conductor>
                            {
                           new Conductor(
                           new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"), new ConductorDni(28052110),
                           new ConductorName("urldelafoto"), new ConductorLocation("39.7128,-71.0060"),
                           new ConductorImage("Image1"), new CraneId("0c651cbc-7df2-4882-ba3b-8f469f6bc3ae"))
                            };

            _providerRepositoryMock.Setup(repo => repo.GetAllConductors(data, It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<List<Conductor>>.Of(conductors));

            // Act
            var result = await _controller.GetAllConductors(data, proviId);

            // Assert
            
            Assert.True(result is OkObjectResult);

        }

        [Fact(DisplayName = "Test When  GetAllConductors Returns NotFoundObjectResult ")]
        public async Task GetAllConductors_ReturnsNotFound_WhenNoConductorsFound()
        {
            // Arrange
            var data = new GetAllConductorsDto();
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            _providerRepositoryMock
                .Setup(repo => repo.GetAllConductors(data, It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<List<Conductor>>.Empty());

            // Act
            var result = await _controller.GetAllConductors(data, providerId);

            // Assert
            Assert.True(result is NotFoundObjectResult);
        }

        [Fact(DisplayName = "Test When GetAllCranes Returns  OkObjectResult ")]
        public async Task GetAllCranes_ReturnsOk_WhenCranesFound()
        {
            // Arrange
            var data = new GetAllCranesDto();
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var cranes = new List<Crane>
        {
                new Crane(
                new CraneId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                new CraneBrand("brand"),
                new CraneModel("model"),
                new CranePlate("V24C11G"),
                new CraneType("medium"),
                new CraneYear(2023))

        };
            _providerRepositoryMock
                .Setup(repo => repo.GetAllCranes(data, It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<List<Crane>>.Of(cranes));

            // Act
            var result = await _controller.GetAllCranes(data, providerId);

            // Assert
            Assert.True(result is OkObjectResult);
        }

        [Fact(DisplayName = "Test When  GetAllCranes Returns NotFoundObjectResult ")]

        public async Task GetAllCranes_ReturnsNotFound_WhenNoCranesFound()
        {
            // Arrange
            var data = new GetAllCranesDto();
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            _providerRepositoryMock
                .Setup(repo => repo.GetAllCranes(data, It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<List<Crane>>.Empty());

            // Act
            var result = await _controller.GetAllCranes(data, providerId);

            // Assert
            Assert.True(result is NotFoundObjectResult);
        }



        [Fact(DisplayName = "Test When GetProviderById Returns OkObjectResult ")]

        public async Task GetProviderById_ReturnsOk_WhenProviderFound()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var conductors = new List<Conductor>
        {
            new Conductor(
                        
                   new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ConductorDni(28052110),
                   new ConductorName("urldelafoto"), 
                   new ConductorLocation("39.7128,-71.0060"),
                   new ConductorImage("Image1"), 
                   new CraneId("0c651cbc-7df2-4882-ba3b-8f469f6bc3ae"))

        };
            var cranes = new List<Crane>
        {
                new Crane(
                new CraneId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                new CraneBrand("brand"),
                new CraneModel("model"),
                new CranePlate("V24C11G"),
                new CraneType("medium"),
                new CraneYear(2023))
        };
            var provider = Provider.Create(
                  new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                  new ProviderName("ProviderName"),
                  new ProviderRif("J123456789"),
                  new ProviderImage("urldelaimagen"),
                conductors, cranes);

            _providerRepositoryMock
                .Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<Provider>.Of(provider));

            // Act
            var result = await _controller.GetProviderById(providerId);

            // Assert
            Assert.True(result is OkObjectResult); 
        }

        [Fact(DisplayName = "Test When GetProviderById Returns  NotFoundObjectResult ")]
        public async Task GetProviderById_ReturnsNotFound_WhenProviderNotFound()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            _providerRepositoryMock
                .Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<Provider>.Empty());

            // Act
            var result = await _controller.GetProviderById(providerId);

            // Assert
            Assert.True(result is NotFoundObjectResult);
        }

        [Fact]
        public async Task GetConductorById_ReturnsOk_WhenConductorExists()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var conductor = new Conductor(
                   new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ConductorDni(28052110),
                   new ConductorName("urldelafoto"),
                   new ConductorLocation("39.7128,-71.0060"),
                   new ConductorImage("Image1"),
                   new CraneId("0c651cbc-7df2-4882-ba3b-8f469f6bc3ae"));

            _providerRepositoryMock.Setup(repo => repo.GetConductorById(It.IsAny<ConductorId>()))
                .ReturnsAsync(_Optional<Conductor>.Of(conductor));

            // Act
            var result = await _controller.GetConductorById(conductorId);

            // Assert
            Assert.True(result is OkObjectResult);
        }

        [Fact(DisplayName = "Test When GetConductorById Returns  NotFoundObjectResult ")]
        public async Task GetConductorById_ReturnsNotFound_WhenConductorDoesNotExist()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            _providerRepositoryMock.Setup(repo => repo.GetConductorById(It.IsAny<ConductorId>()))
                .ReturnsAsync(_Optional<Conductor>.Empty());

            // Act
            var result = await _controller.GetConductorById(conductorId);

            // Assert
            Assert.True(result is NotFoundObjectResult);
        }


        [Fact]
        public async Task GetConductorById_ReturnsBadRequest_OnException()
        {
            // Arrange
            var conductorId = "nonexistent-id";
            _providerRepositoryMock.Setup(repo => repo.GetConductorById(It.IsAny<ConductorId>()))
                .ThrowsAsync(new System.Exception(" Is BadRequestObjectResult"));

            // Act
            var result = await _controller.GetConductorById(conductorId);

            // Assert
            Assert.True(result is BadRequestObjectResult);
        }

        [Fact]
        public async Task GetCraneById_ReturnsNotFound_WhenCraneDoesNotExist()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var query = new GetCraneByIdDto("cfdceb8f-217f-4cdc-b983-7e9815187dce");
            _providerRepositoryMock.Setup(repo => repo.GetCraneById(It.IsAny<ProviderId>(), It.IsAny<CraneId>()))
                .ReturnsAsync(_Optional<Crane>.Empty());

            // Act
            var result = await _controller.GetCraneById(query, providerId);

            // Assert
            Assert.True(result is NotFoundObjectResult);
        }

        [Fact]
        public async Task GetCraneById_ReturnsOk_WhenCraneExists()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var query = new GetCraneByIdDto("cfdceb8f-217f-4cdc-b983-7e9815187dce");
            var crane = new Crane(
                new CraneId(query.CraneId),
                new CraneBrand("brand"),
                new CraneModel("model"),
                new CranePlate("V24C11G"),
                new CraneType("medium"),
                new CraneYear(2023));

            _providerRepositoryMock.Setup(repo => repo.GetCraneById(It.IsAny<ProviderId>(), It.IsAny<CraneId>()))
                .ReturnsAsync(_Optional<Crane>.Of(crane));

            // Act

            var result = await _controller.GetCraneById(query, providerId);

            // Assert
            Assert.True(result is OkObjectResult); ;
        }

        [Fact]
        public async Task GetCraneById_ReturnsBadRequest_OnException()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var query = new GetCraneByIdDto("cfdceb8f-217f-4cdc-b983-7e9815187dce");
            _providerRepositoryMock.Setup(repo => repo.GetCraneById(It.IsAny<ProviderId>(), It.IsAny<CraneId>()))
                .ThrowsAsync(new System.Exception("is BadRequestObjectResult"));

            // Act


            var result = await _controller.GetCraneById(query, providerId);

            // Assert
            Assert.True(result is BadRequestObjectResult);
        }

        [Fact]
        public async Task UpdateConductorLocation_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var data = new UpdateConductorDto("39.7128,-71.0060");
            var command = new UpdateConductorCommand(conductorId, data.Location);

            var conductor = new Conductor(
                   new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ConductorDni(28052110),
                   new ConductorName("urldelafoto"),
                   new ConductorLocation("39.7128,-71.0060"),
                   new ConductorImage("Image1"),
                   new CraneId("0c651cbc-7df2-4882-ba3b-8f469f6bc3ae"));

            _providerRepositoryMock.Setup(repo => repo.GetConductorById(It.IsAny<ConductorId>()))
                .ReturnsAsync(_Optional<Conductor>.Of(conductor));
            _providerRepositoryMock.Setup(repo => repo.UpdateConductorLocationById(It.IsAny<Conductor>())).ReturnsAsync(new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"));
           
            var service = new UpdateConductorCommandHandler( _providerRepositoryMock.Object);
            // Act
            var result = await service.Execute(command);
            var oKresult = await _controller.UpdateConductorLocation(data, conductorId);

            // Assert
            Assert.True(oKresult is OkObjectResult);
        }

        [Fact]
        public async Task UpdateConductorLocation_ReturnsBadRequest_WhenLocationIsNullOrEmpty()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var data = new UpdateConductorDto(null);

            // Act
           
            var result = await _controller.UpdateConductorLocation(data, conductorId);

            // Assert
            Assert.True(result is BadRequestObjectResult);
        }

        [Fact]
        public async Task UpdateConductorLocation_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var data = new UpdateConductorDto("InvalidLocation");
            var command = new UpdateConductorCommand(conductorId, data.Location);
            var validator = new UpdateConductorCommandValidator();
            var validationResult = validator.Validate(command);
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Location", "Invalid location"));

            // Act
            var result = await _controller.UpdateConductorLocation(data, conductorId);

            // Assert
            Assert.True(result is BadRequestObjectResult);

        }
        
        [Fact]
        public async Task ToggleConductorActivity_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var data = new UpdateConductorDto(null);
            var command = new UpdateConductorCommand(conductorId, "39.7128,-71.0060");

            var conductor = new Conductor(
                   new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ConductorDni(28052110),
                   new ConductorName("urldelafoto"),
                   new ConductorLocation("39.7128,-71.0060"),
                   new ConductorImage("Image1"),
                   new CraneId("0c651cbc-7df2-4882-ba3b-8f469f6bc3ae"));

            _providerRepositoryMock.Setup(repo => repo.GetConductorById(It.IsAny<ConductorId>()))
                .ReturnsAsync(_Optional<Conductor>.Of(conductor));
            _providerRepositoryMock.Setup(repo => repo.ToggleActivityConductorById(It.IsAny<ConductorId>()))
                 .ReturnsAsync(new ConductorId(conductorId));
            _providerRepositoryMock.Setup(repo => repo.UpdateConductorLocationById(It.IsAny<Conductor>()))
                .ReturnsAsync(new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"));

            // Act
            var result = await _controller.ToggleConductorActivity(data, conductorId);

            // Assert
            Assert.True(result is OkObjectResult);
        }

        [Fact]
        public async Task ToggleConductorActivity_ReturnsBadRequest_WhenLocationIsNotEmpty()
        {
            // Arrange
            var conductorId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var data = new UpdateConductorDto("39.7128,-71.0060");

            // Act
            var result = await _controller.ToggleConductorActivity(data, conductorId);

            // Assert
            Assert.True(result is BadRequestObjectResult);
        }

        [Fact]
        public async Task ToggleCraneActivity_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var craneId = "cfdceb8f-217f-4cdc-b983-7e9815187dce";
            var updateCraneDto = new UpdateCraneDto(craneId);
            var command = new UpdateCraneCommand(providerId, craneId);
            
            
            var conductors = new List<Conductor>
        {
            new Conductor(

                   new ConductorId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                   new ConductorDni(28052110),
                   new ConductorName("urldelafoto"),
                   new ConductorLocation("39.7128,-71.0060"),
                   new ConductorImage("Image1"),
                   new CraneId("0c651cbc-7df2-4882-ba3b-8f469f6bc3ae"))

        };
            var cranes = new List<Crane>
        {
                new Crane(
                new CraneId("cfdceb8f-217f-4cdc-b983-7e9815187dce"),
                new CraneBrand("brand"),
                new CraneModel("model"),
                new CranePlate("V24C11G"),
                new CraneType("medium"),
                new CraneYear(2023))
        };
            var provider = Provider.Create(
                  new ProviderId("0fdceb8f-217f-4cdc-b983-7e9815187dce"),
                  new ProviderName("ProviderName"),
                  new ProviderRif("J123456789"),
                  new ProviderImage("urldelaimagen"),
                conductors, cranes);

            _providerRepositoryMock
                .Setup(repo => repo.GetProviderById(It.IsAny<ProviderId>()))
                .ReturnsAsync(_Optional<Provider>.Of(provider));

            _providerRepositoryMock
               .Setup(repo => repo.ToggleActivityCraneById(It.IsAny<ProviderId>(), It.IsAny<CraneId>()))
                .ReturnsAsync(new CraneId(craneId));

            // Act
            var result = await _controller.ToggleCraneActivity(updateCraneDto, providerId);

            // Assert
            Assert.True(result is OkObjectResult);
        }


        [Fact]
        public async Task ToggleCraneActivity_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var providerId = "0fdceb8f-217f-4cdc-b983-7e9815187dce";
            var craneId = "invalidCraneId";
            var updateCraneDto = new UpdateCraneDto(craneId);
            var command = new UpdateCraneCommand(providerId, craneId);
            var validator = new UpdateCraneCommandValidator();

            // Act
            var result = await _controller.ToggleCraneActivity(updateCraneDto, providerId);

            // Assert
            Assert.True(result is BadRequestObjectResult);
        }



    }
}
