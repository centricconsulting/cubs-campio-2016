using System;
using System.Collections.Generic;
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

        public async Task Acknowledge(string key)
        {
            using (var client = new HttpClient())
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

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return;
                else
                    throw new HttpRequestException(result.StatusCode.ToString());
            }
        }

    }
}
