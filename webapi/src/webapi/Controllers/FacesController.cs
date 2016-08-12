using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class FacesController : Controller
    {
        //private IHostingEnvironment _environment;

        //public FacesController(IHostingEnvironment environment)
        //{
        //    _environment = environment;
        //}

        //public FacesController()
        //{
        //}

        // GET api/faces
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "face1", "face2" };
        }

        // POST api/faces/upload
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload()
        {
            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            //foreach (var file in files)
            //{
            //    if (file.Length > 0)
            //    {
            //        using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
            //        {
            //            await file.CopyToAsync(fileStream);
            //        }
            //    }
            //}



            return Ok();
        }

        // POST api/faces/register
        [HttpPost("Register")]
        public async Task<IActionResult> Register()
        {
            return Ok();
        }
    }
}




//// GET: Face
//public async Task<HttpResponseMessage> Post()
//{
//    if (!Request.Content.IsMimeMultipartContent())
//    {
//        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
//    }

//    var x = GetContactIdForUser();

//    var provider = new BlobStorageMultipartStreamProvider()
//    {
//        ConnectionString = ConfigurationManager.AppSettings[DomainConstants.ACCOUNT_STORAGE_CONNECTION_STRING_KEY],
//        ContainerName = ConfigurationManager.AppSettings[DomainConstants.AVATAR_IMAGE_INPUT_CONTAINER_NAME_KEY],
//        FileName = string.Format("{0}.jpg", x)
//    };

//    try
//    {
//        await Request.Content.ReadAsMultipartAsync(provider);
//        return Request.CreateResponse(HttpStatusCode.OK);
//    }
//    catch (Exception e)
//    {
//        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
//    };
//}

//public class BlobStorageMultipartStreamProvider : MultipartStreamProvider
//{
//    public string ConnectionString { get; set; }
//    public string ContainerName { get; set; }
//    public string FileName { get; set; }

//    public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
//    {
//        Stream stream = null;
//        var contentDisposition = headers.ContentDisposition;

//        if (contentDisposition == null) return null;
//        if (String.IsNullOrWhiteSpace(contentDisposition.FileName)) return null;

//        var storageAccount = CloudStorageAccount.Parse(ConnectionString);
//        var blobClient = storageAccount.CreateCloudBlobClient();
//        var blobContainer = blobClient.GetContainerReference(ContainerName);

//        blobContainer.CreateIfNotExists();
//        blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

//        var blob = blobContainer.GetBlockBlobReference(FileName);
//        stream = blob.OpenWrite();
//        return stream;
//    }
//}
