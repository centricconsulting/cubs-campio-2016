using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

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

			Button loginButton = FindViewById<Button>(Resource.Id.loginButton1);

			loginButton.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(MainActivity));
				StartActivity(intent);
			};

		}
	}
}