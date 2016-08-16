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
using Microsoft.AspNetCore.Http.Internal;

namespace webapi.test
{
    [Collection("use image data file")]
    public class FaceControllerIntegrationTest
    {
        private readonly ITestOutputHelper _output;
        public IServiceProvider Services => TestApplicationEnvironment.Services;
        private IStorageService _storageService;

        public FaceControllerIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
            _storageService = Services.GetRequiredService<IStorageService>();
            _storageService.Clear();
        }

        [Fact]
        public async Task upload_single_face_single_candidate_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            using (var fs = new FileStream(sampleFile, FileMode.Open))
            {
                FormFile f = new FormFile(fs, 0, fs.Length, "file", fs.Name);
                var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), Services.GetRequiredService<IFaceServiceClient>(), _storageService);

                // act 
                var result = await obj.Upload(f);

                // assert
                Assert.IsAssignableFrom<IActionResult>(result);
                Assert.IsType(typeof(OkObjectResult), result);
                Assert.IsType(typeof(ResponseModel), ((OkObjectResult)result).Value);
                Assert.True((((OkObjectResult)result).Value as ResponseModel).Faces.Count == 4);
            }
        }

        [Fact]
        public async Task register_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), Services.GetRequiredService<IFaceServiceClient>(), _storageService);
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            ResponseModel uploadResult;
            using (var fs = new FileStream(sampleFile, FileMode.Open))
            {
                FormFile f = new FormFile(fs, 0, fs.Length, "file", fs.Name);
                var uploadResultTemp = await obj.Upload(f);
                uploadResult = ((OkObjectResult)uploadResultTemp).Value as ResponseModel;
            }

            // act
            var registerResult = await obj.Register(uploadResult.Key, 1, "Helen");

            // assert
            var storageObject = _storageService.Get(uploadResult.Key);
            Assert.Null(storageObject);

            // redetect & reidentify
            string sampleFileNext = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            using (var fsNext = new FileStream(sampleFileNext, FileMode.Open))
            {
                FormFile fNext = new FormFile(fsNext, 0, fsNext.Length, "file", fsNext.Name);
                var uploadResultTemp = await obj.Upload(fNext);
                Assert.Equal("Helen", (((OkObjectResult)uploadResultTemp).Value as ResponseModel).Faces[1].Candidates[0].PersonName);
            }
        }

    }

}
