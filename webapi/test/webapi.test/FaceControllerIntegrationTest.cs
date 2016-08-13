﻿using Microsoft.AspNetCore.Hosting;
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

namespace webapi.test
{
    [Collection("use image data file")]
    public class FaceControllerIntegrationTest
    {
        private readonly ITestOutputHelper _output;
        public IServiceProvider Services => TestApplicationEnvironment.Services;

        public FaceControllerIntegrationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task upload_single_face_single_candidate_RESULT_OK()
        {
            // arrange - mocking IFormFile using a memory stream
            var fileMock = new Mock<IFormFile>();
            string sampleFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "joe_family.jpg");
            var fs = new FileStream(sampleFile, FileMode.Open); fs.Position = 0;
            fileMock.Setup(m => m.OpenReadStream()).Returns(fs);

            var obj = new FaceController(Services.GetRequiredService<IHostingEnvironment>(), Services.GetRequiredService<IFaceServiceClient>());

            // act 
            var result = await obj.Upload(fileMock.Object);

            // assert
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType(typeof(OkObjectResult), result);
            Assert.IsType(typeof(List<FaceModel>), ((OkObjectResult)result).Value);
            Assert.True((((OkObjectResult)result).Value as List<FaceModel>).Count == 4);
        }
    }

}