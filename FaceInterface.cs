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
					if (json["candidates"].Count > 0)
					{
						face.IsAMatch = true;
						face.Name = data["Name"];
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

		}

	}

