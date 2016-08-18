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

namespace CarmeraUseRecipe
{
	public class FaceInterface
	{




		public async Task<string> ReturnFace(Bitmap bitmap, string path)
		{
			using (HttpClient httpClient = new HttpClient())
			{
				var fileStream = File.Open(path, FileMode.Open);
				var fileInfo = new FileInfo(path);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var content = new MultipartFormDataContent();
				content.Add(new StreamContent(fileStream), "\"file\"", string.Format("\"{0}\"", fileInfo.Name));
                
				var result = await httpClient.PostAsync("http://localhost:51241/api/face/Upload?file=", content);


						if (result.IsSuccessStatusCode)
						{
							return "SUCCESS";
						}
						else
						{
							return "FAILURE";
						}
					}

				

			}




			//var httpClient = new HttpClient();

			////httpClient.DefaultRequestHeaders.TransferEncodingChunked = true;

			//httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			//var content = new MultipartFormDataContent();
			//using (var imageContent = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
			//{
			//	imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
			//	content.Add(imageContent);

			//	return await httpClient.PostAsync("http://face-webapi.azurewebsites.net/api/Face/Upload?file=", content);
			//}	
		}

		//public async Task<String>  ReturnFaceFromPicture(Bitmap bitmap, string path)
		//{

		//	byte[] bitmapData;
		//	var stream = new MemoryStream();
		//	bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
		//	bitmapData = stream.ToArray();
		//	var fileContent = new ByteArrayContent(bitmapData);


		//	fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
		//	fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		//	{
		//		Name = "file",
		//		FileName = path

		//	};

		//	string boundary = "---8d0f01e6b3b5dafaaadaada";
		//	MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);
		//	multipartContent.Add(fileContent);
		//	HttpClient httpClient = new HttpClient();

		//	httpClient.DefaultRequestHeaders.
		//	httpClient.DefaultRequestHeaders.Accept.Clear();
		//	httpClient.DefaultRequestHeaders.Accept.Add(
		//		new MediaTypeWithQualityHeaderValue("application/json"));
		//	//httpClient.R
		//	HttpResponseMessage response =  await httpClient.PostAsync("http://face-webapi.azurewebsites.net/api/Face/Upload", multipartContent);
		//	if (response.IsSuccessStatusCode)
		//	{
		//		//string content = await response.Content.;
		//		return "SUCCESS";

		//		//return content;
		//	}
		//	return null;
		//}



	}

