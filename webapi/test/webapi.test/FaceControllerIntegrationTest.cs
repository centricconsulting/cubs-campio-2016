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
using common.Models;
using webapi.Services;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace webapi.test
{
    [Collection("use image data file")]
    public class FaceControllerIntegrationTest
    {
        private readonly ITestOutputHelper _output;
        public IServiceProvider Services => TestApplicationEnvironment.Services;
        public TestServer TestServer => TestApplicationEnvironment.Server;
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
        public async Task http_upload_single_face_single_candidate_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            using (var fs = new FileStream(sampleFile, FileMode.Open))
            {
                using (var client = TestServer.CreateClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new MultipartFormDataContent();

                    // initialize file/stream content
                    var fileContent = new StreamContent(fs);

                    // initialize content disposition for the stream content
                    var contentDisposition = new ContentDispositionHeaderValue("form-data"); // must "form-data" since we are posting as form
                    contentDisposition.Name = "file"; // must match with parameter name, which in this case is "file"
                    contentDisposition.FileName = "joe_family.jpg"; // file name or path

                    // set content disposition 
                    fileContent.Headers.ContentDisposition = contentDisposition;

                    // set content type to "multipart/form-data"
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

                    // add stream content into multipart content
                    content.Add(fileContent);

                    // act
                    var result = await client.PostAsync("/api/face/upload", content);

                    // assert
                    Assert.NotNull(result);
                }
            }
        }

        [Fact]
        public async Task http_acknowledge_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            using (var client = TestServer.CreateClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new MultipartFormDataContent();

                // initialize string content
                var stringContent = new StringContent("abc");

                // initialize content disposition for the string content
                var contentDisposition = new ContentDispositionHeaderValue("form-data"); // must "form-data" since we are posting as form
                contentDisposition.Name = "key"; // must match with parameter name, which in this case is "key"

                // set content disposition 
                stringContent.Headers.ContentDisposition = contentDisposition;

                // set content type to "multipart/form-data"
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

                // add stream content into multipart content
                content.Add(stringContent);

                // act
                var result = await client.PostAsync("/api/face/acknowledge", content);

                // assert
                Assert.NotNull(result);
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
