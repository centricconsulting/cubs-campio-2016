using Android.App;
using Android.Widget;
using Android.OS;
using Java.IO;
using Android.Graphics;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using System;
using Android.Content.Res;

namespace CarmeraUseRecipe
{
	[Activity(Label = "CarmeraUseRecipe", MainLauncher = true, Icon = "@mipmap/icon")]
	public class LoginActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			// Create your application here
			SetContentView(Resource.Layout.Login);

			Button loginButton = FindViewById<Button>(Resource.Id.myButton1);

			loginButton.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(MainActivity));
				StartActivity(intent);
			};

		}
	}
}