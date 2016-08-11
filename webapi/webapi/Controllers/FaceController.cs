using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace webapi.Controllers
{
    public class FaceController : Controller
    {
        // GET: Face
        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var x = GetContactIdForUser();

            var provider = new BlobStorageMultipartStreamProvider()
            {
                ConnectionString = ConfigurationManager.AppSettings[DomainConstants.ACCOUNT_STORAGE_CONNECTION_STRING_KEY],
                ContainerName = ConfigurationManager.AppSettings[DomainConstants.AVATAR_IMAGE_INPUT_CONTAINER_NAME_KEY],
                FileName = string.Format("{0}.jpg", x)
            };

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            };
        }
    }

    public class BlobStorageMultipartStreamProvider : MultipartStreamProvider
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string FileName { get; set; }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            Stream stream = null;
            var contentDisposition = headers.ContentDisposition;

            if (contentDisposition == null) return null;
            if (String.IsNullOrWhiteSpace(contentDisposition.FileName)) return null;

            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(ContainerName);

            blobContainer.CreateIfNotExists();
            blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var blob = blobContainer.GetBlockBlobReference(FileName);
            stream = blob.OpenWrite();
            return stream;
        }
    }
}