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
using webapi.Services;
using System;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class FaceController : Controller
    {
        private IHostingEnvironment _environment;
        private IFaceServiceClient _faceService;
        private IStorageService _storageService;
        private string _personGroupId = "campio";

        public FaceController(IHostingEnvironment environment, IFaceServiceClient faceService, IStorageService storageService)
        {
            _environment = environment;
            _faceService = faceService;
            _storageService = storageService;
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
                _storageService.Add("file_" + DateTime.UtcNow.ToString().ToLowerInvariant(), response, s);
            }

            return Ok(response);
        }

        // POST api/face/register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(string key, int faceIndex, string name)
        {
            // get the file from Storage
            var storageObject = _storageService.Get(key) as object[];
            var faces = storageObject[0] as List<FaceModel>;
            var fileStream = storageObject[1] as Stream;

            // create new person
            CreatePersonResult person = await _faceService.CreatePersonAsync(_personGroupId, name);

            // register face - person
            await _faceService.AddPersonFaceAsync(_personGroupId, person.PersonId, fileStream, null, faces[faceIndex].FaceRectangle);

            // train the person group
            await _faceService.TrainPersonGroupAsync(_personGroupId);

            // waith until training is done
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await _faceService.GetPersonGroupTrainingStatusAsync(_personGroupId);
                if (trainingStatus.Status != Status.Running) { break; }
                await Task.Delay(1000);
            }

            // remove from storage
            _storageService.Remove(key);
            return Ok();
        }

        // POST api/face/acknowledge
        [HttpPost("Acknowledge")]
        public IActionResult Acknowledge(string key)
        {
            _storageService.Remove(key);
            return Ok();
        }
    }
}