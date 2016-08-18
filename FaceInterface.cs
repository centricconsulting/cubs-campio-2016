using System;
using System.IO;
using System.Net;
using Android.Graphics;
using System.Net.Http;
using Org.Apache.Http.Client.Methods;
using Org.Apache.Http.Entity;
using Android.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Json;
using System.Text;
using System.Collections.Generic;

namespace CarmeraUseRecipe
{
	public class FaceInterface
	{
		public async Task<FacialRecognitionResponse> ReturnFace(Bitmap bitmap, string path)
		{

			byte[] bitmapData;
			var stream = new MemoryStream();
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
			bitmapData = stream.ToArray();
			var fileContent = new ByteArrayContent(bitmapData);

			fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multiform/form-data");
			fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = "file",
				FileName = path
			};

			string boundary = "---8d0f01e6b3b5dafaaadaad";
			MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);
			multipartContent.Add(fileContent);

			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			HttpResponseMessage response = await httpClient.PostAsync("http://face-webapi.azurewebsites.net/api/Face/Upload", multipartContent);

			if (response.IsSuccessStatusCode)
			{

				string content = await response.Content.ReadAsStringAsync();

				var json = JsonValue.Parse(content);

				var data = json["faces"][0];
				var face = new FacialRecognitionResponse();

				foreach (var dataItem in data)
				{
					if (data["candidates"].Count == 0)
						await RegisterFace(json["key"]);
				
					else if (data["candidates"].Count > 0)
					{
						face.IsAMatch = true;

						var candidate = data["candidates"][0];
						face.Name = candidate["personName"];
					}
					else
					{
						face.IsAMatch = false;
					}
				}
				return face;
				//return content;
			}
			return null;
		}




		public async Task<string> RegisterFace(string key)
		{
			using (var client = new HttpClient())
			{
				var formContent = new FormUrlEncodedContent(new[]
				 {
				   new KeyValuePair<string,string>("name", "Jonathan Flatt"),
					new KeyValuePair<string,string>("faceIndex", "0"),
					new KeyValuePair<string,string>("key", key),
		   });

				var postResponse = await client.PostAsync("http://face-webapi.azurewebsites.net/api/Face/Register", formContent);
				postResponse.EnsureSuccessStatusCode();

				string resonse = await postResponse.Content.ReadAsStringAsync();
			}

			return null;
		}
	}
}

