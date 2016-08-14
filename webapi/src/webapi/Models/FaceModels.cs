using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapi.Models
{
    public class FaceModel : Face
    {
        public FaceModel(Face face)
        {
            Candidates = new List<CandidateModel>();
            FaceAttributes = face.FaceAttributes;
            FaceId = face.FaceId;
            FaceLandmarks = face.FaceLandmarks;
            FaceRectangle = face.FaceRectangle;
        }

        public List<CandidateModel> Candidates { get; set; }
    }

    public class CandidateModel
    {
        public string PersonId { get; set; }

        public string PersonName { get; set; }

        public double Confidence { get; set; }
    }
}
