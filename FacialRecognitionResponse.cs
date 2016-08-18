
using System;


namespace CarmeraUseRecipe
{

	public class FacialRecognitionResponse
	{
		public bool IsAMatch { get; set; }
		public int ConfidenceLevel { get; set;}
		public string Name { get; set; } 
		public string Email { get; set; } 
		public string Business { get; set; }
		public DateTime? LastSeen { get; set; }
	}

}