using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using webapi.Controllers;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.Abstractions;
using System.IO;
using Moq;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using webapi.Models;
using webapi.Services;

namespace webapi.test
{
    [Collection("use image data file")]
    public class FaceControllerUnitTest
    {
        private readonly ITestOutputHelper _output;
        public IServiceProvider Services => TestApplicationEnvironment.Services;
        private IStorageService _storageService;

        public FaceControllerUnitTest(ITestOutputHelper output)
        {
            _output = output;
            _storageService = Services.GetRequiredService<IStorageService>();
            _storageService.Clear();
        }

        [Fact]
        public async Task upload_single_face_single_candidate_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            var fileMock = new Mock<IFormFile>();
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            using (var fs = new FileStream(sampleFile, FileMode.Open))
            {
                fs.Position = 0;
                fileMock.Setup(m => m.OpenReadStream()).Returns(fs);
                fileMock.SetupAllProperties();
                fileMock.SetupGet(p => p.FileName).Returns(sampleFile);

                // mocking Face Service
                Guid faceId = Guid.NewGuid();
                Guid personId = Guid.NewGuid();
                var faceServiceMock = new Mock<IFaceServiceClient>();
                faceServiceMock.Setup(f => f.DetectAsync(It.IsAny<Stream>(), true, false, null)).Returns(
                    Task.FromResult(new[] {
                    new Face { FaceId = faceId }
                    }));
                faceServiceMock.Setup(f => f.IdentifyAsync(It.IsAny<string>(), It.IsAny<Guid[]>(), 1)).Returns(
                    Task.FromResult(new[] {
                    new IdentifyResult {
                        FaceId = faceId,
                        Candidates = new [] { new Candidate { Confidence = 0.8, PersonId = personId } }
                    }
                    }));
                faceServiceMock.Setup(f => f.GetPersonAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(
                    Task.FromResult(
                        new Person { PersonId = personId, Name = "Johannes" }
                        ));

                var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), faceServiceMock.Object, _storageService);

                // act 
                var result = await obj.Upload(fileMock.Object);

                // assert
                Assert.IsAssignableFrom<IActionResult>(result);
                Assert.IsType(typeof(OkObjectResult), result);
                Assert.IsType(typeof(ResponseModel), ((OkObjectResult)result).Value);
                Assert.True((((OkObjectResult)result).Value as ResponseModel).Faces.Count == 1);
                Assert.True((((OkObjectResult)result).Value as ResponseModel).Faces[0].Candidates.Count == 1);
                Assert.Equal("Johannes", (((OkObjectResult)result).Value as ResponseModel).Faces[0].Candidates[0].PersonName);
            }
        }

        
        [Fact]
        public void acknowledge_RESULT_OK()
        {
            // arrange
            var key = "test_key";
            _storageService.Add(key, "whatever object", null);
            Guid faceId = Guid.NewGuid();
            Guid personId = Guid.NewGuid();
            var faceServiceMock = new Mock<IFaceServiceClient>();
            var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), faceServiceMock.Object, _storageService);

            // act
            var result = obj.Acknowledge(key);

            // assert
            var storageObject = _storageService.Get(key);
            Assert.Null(storageObject);
        }
    }

}
