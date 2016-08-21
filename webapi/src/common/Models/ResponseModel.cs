using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace common.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            Faces = new List<FaceModel>();
        }

        public string Key { get; set; }

        public List<FaceModel> Faces { get; set; }
    }

    [DebuggerDisplay("{FaceAttributes.Gender}, Age {FaceAttributes.Age}")]
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

    //[DebuggerDisplay("PersonName {PersonName}")]
    public class CandidateModel
    {
        public string PersonId { get; set; }

        public string PersonName { get; set; }

        public double Confidence { get; set; }
    }
}
