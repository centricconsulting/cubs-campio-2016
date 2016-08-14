using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.ProjectOxford.Face;
using Microsoft.AspNetCore.Http;
using Microsoft.ProjectOxford.Face.Contract;
using webapi.Models;

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
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var response = new List<FaceModel>();
            using (Stream s = file.OpenReadStream())
            {
                // DETECT faces in photo
                var faces = await _faceService.DetectAsync(s);

                // if no face detected, return with NotFound status
                if (!faces.Any()) { return NotFound(); }

                // sort faces DETECTED by face id
                var faceIds = faces.OrderBy(f => f.FaceId).Select(face => face.FaceId).ToArray();
                // IDENTIFY faces DETECED
                var identifyResults = await _faceService.IdentifyAsync(_personGroupId, faceIds);

                foreach (var face in faces)
                {
                    var faceModel = new FaceModel(face);

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
        [HttpPost("Register")]
        public async Task<IActionResult> Register()
        {
            return Ok();
        }
    }
}