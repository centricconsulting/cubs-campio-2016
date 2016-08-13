using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNet.Http;
using Microsoft.ProjectOxford.Face;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class FaceController : Controller
    {
        private IHostingEnvironment _environment;
        private IFaceServiceClient _faceService;
        private string _personGroupId = "campio";

        public FaceController(IHostingEnvironment environment, IFaceServiceClient faceService)
        {
            _environment = environment;
            _faceService = faceService;
        }

        // POST api/face/upload
        [HttpPost]
        public async Task<IActionResult> Upload(Microsoft.AspNet.Http.IFormFile file)
        {
            var response = new List<FaceModel>();
            using (Stream s = file.OpenReadStream())
            {
                // DETECT faces in photo
                var faces = await _faceService.DetectAsync(s);
                // sort faces DETECTED by face id
                var faceIds = faces.OrderBy(f => f.FaceId).Select(face => face.FaceId).ToArray();
                // IDENTIFY faces DETECED
                var identifyResults = await _faceService.IdentifyAsync(_personGroupId, faceIds);

                foreach (var face in faces)
                {
                    var faceModel = new FaceModel();
                    faceModel.FaceId = face.FaceId.ToString();

                    // for each face DETECTED in photo, if any is IDENTIFIED
                    if (identifyResults.Any(f => f.FaceId == face.FaceId))
                    {
                        // get the IDENTIFIED face
                        var identifyResult = identifyResults.FirstOrDefault(f => f.FaceId == face.FaceId);

                        // if the IDENTIFIED face has any possible CANDIDATES
                        for (int i = 0; i < identifyResult.Candidates.Length; i++)
                        {
                            // get the CANDIDATE's name
                            var candidateId = identifyResult.Candidates[i].PersonId;
                            var person = await _faceService.GetPersonAsync(_personGroupId, candidateId);

                            faceModel.Candidates.Add(new CandidateModel { PersonId = candidateId.ToString(), Confidence = identifyResult.Candidates[i].Confidence, PersonName = person.Name });
                        }
                    }
                    response.Add(faceModel);
                }
            }

            return Ok(response);
        }

        // POST api/face/register
        [HttpPost]
        public async Task<IActionResult> Register()
        {
            return Ok();
        }
    }

    public class FaceModel
    {
        public string FaceId { get; set; }

        public List<CandidateModel> Candidates { get; set; }
    }

    public class CandidateModel
    {
        public string PersonId { get; set; }

        public string PersonName { get; set; }

        public double Confidence { get; set; }
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
