using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using webapi.Controllers;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.Abstractions;
using Microsoft.AspNet.Http;
using System.IO;
using Moq;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace webapi.test
{
    public class FaceControllerTest
    {
        private readonly ITestOutputHelper _output;
        public IServiceProvider Services => TestApplicationEnvironment.Services;

        public FaceControllerTest(ITestOutputHelper output)
        {
            _output = output;

        }

        [Fact]
        public async Task Get_Index_VIEW_RESULT_OK()
        {
            //arrange           

            // mocking IFormFile using a memory stream
            var fileMock = new Mock<IFormFile>();
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "joe_family.jpg");
            var fs = new FileStream(sampleFile, FileMode.Open);
            fs.Position = 0;
            fileMock.Setup(m => m.OpenReadStream()).Returns(fs);

            // mocking Face Service
            var faceServiceMock = new Mock<IFaceServiceClient>();
            faceServiceMock.Setup(f => f.DetectAsync(It.IsAny<Stream>(), true, false, null)).Returns(
                Task.FromResult(new[] {
                    new Face { FaceId = Guid.NewGuid() }
                }
                )
                );

            var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), faceServiceMock.Object);

            //act - call Index from Home Controller
            var result = await obj.Upload(fileMock.Object);

            //assert
            Assert.NotNull(result);

            //_targetInstanceRepoMock.Verify();
        }
    }

}
