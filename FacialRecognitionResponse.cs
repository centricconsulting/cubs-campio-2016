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
		public bool IsAMatch { get; set; } = true;
		public int ConfidenceLevel { get; set; } = 80;
		public string Name { get; set; } = "Gregor Clegane";
		public string Email { get; set; } = "themountain@gmail.com";
		public string Business { get; set; } = "Westeros";
		public DateTime? LastSeen { get; set; } = DateTime.Now;
	}

}