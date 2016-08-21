using common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace webapi.client
{
    public class FaceServiceClient
    {
        public FaceServiceClient()
        { }

        public async Task<ResponseModel> Upload(Stream imageStream)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://face=webapi.azurewebsites.net");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new MultipartFormDataContent();

                // initialize file/stream content
                var fileContent = new StreamContent(imageStream);

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

                if (result.IsSuccessStatusCode)
                {
                    ResponseModel response = JsonConvert.DeserializeObject<ResponseModel>(result.Content.ReadAsStringAsync().Result);
                    return response;
                }
                else
                {
                    throw new HttpRequestException(result.StatusCode.ToString());
                }
            }
        }

        public async Task Acknowledge(string key)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://face=webapi.azurewebsites.net");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new MultipartFormDataContent();

                // initialize string content
                var stringContent = new StringContent(key);

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

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return;
                else
                    throw new HttpRequestException(result.StatusCode.ToString());
            }
        }
    }
}
