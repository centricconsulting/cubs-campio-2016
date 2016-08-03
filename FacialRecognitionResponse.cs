using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CarmeraUseRecipe
{

    public class FacialRecognitionResponse
        {
            public bool IsAMatch { get; set; }
            public int ConfidenceLevel { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Business { get; set; }
            public DateTime? LastSeen { get; set; }
        }
}