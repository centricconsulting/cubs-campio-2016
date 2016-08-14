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

namespace webapi.test
{
    [Collection("use image data file")]
    public class FaceControllerUnitTest
    {
        private readonly ITestOutputHelper _output;
        public IServiceProvider Services => TestApplicationEnvironment.Services;

        public FaceControllerUnitTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task upload_single_face_single_candidate_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            var fileMock = new Mock<IFormFile>();
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            var fs = new FileStream(sampleFile, FileMode.Open);
            fs.Position = 0;
            fileMock.Setup(m => m.OpenReadStream()).Returns(fs);

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

            var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), faceServiceMock.Object);

            // act 
            var result = await obj.Upload(fileMock.Object);

            // assert
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.IsType(typeof(List<FaceModel>), ((OkObjectResult)result).Value);
            Assert.True((((OkObjectResult)result).Value as List<FaceModel>).Count == 1);
            Assert.True((((OkObjectResult)result).Value as List<FaceModel>)[0].Candidates.Count == 1);
            Assert.Equal("Johannes", (((OkObjectResult)result).Value as List<FaceModel>)[0].Candidates[0].PersonName);
        }
    }

}
