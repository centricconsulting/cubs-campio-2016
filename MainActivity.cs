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
using Android.Util;
using System.Threading.Tasks;

namespace CarmeraUseRecipe
{
	[Activity(Label = "CarmeraUseRecipe", Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		private ImageView _imageView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);


			if (!IsThereAnAppToTakePictures()) return;

			CreateDirectoryForPictures();

			_imageView = FindViewById<ImageView>(Resource.Id.imageView1);
			Button myButton = FindViewById<Button>(Resource.Id.myButton);

			OpenCameraAndTakePicture();
			myButton.Click += TakeAPicture;

		}

		protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
			mediaScanIntent.SetData(contentUri);
			SendBroadcast(mediaScanIntent);

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = Resources.DisplayMetrics.WidthPixels;
			App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);

			if (App.bitmap != null)
			{
				var match = await GetFacialResponse();
				_imageView.SetImageBitmap(App.bitmap);
				if (match.IsAMatch == true)
					populateFacialMatch(match);
			}

			GC.Collect();
		}

		private async Task<FacialRecognitionResponse> GetFacialResponse()
		{
			var x = new FaceInterface();
			var response = await x.ReturnFace(App.bitmap, App._file.AbsolutePath);
			return response;
		}


		private void CreateDirectoryForPictures()
		{
			App._dir = new File(
				Android.OS.Environment.GetExternalStoragePublicDirectory(
					Android.OS.Environment.DirectoryPictures), "CameraAppDemo");

			if (!App._dir.Exists())
			{
				App._dir.Mkdirs();
			}
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities =
				PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}


		private void OpenCameraAndTakePicture()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);


			App._file = new File(App._dir, string.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
			StartActivityForResult(intent, 0);

		}

		// Yeah, its not great. still figuring out how to do it without the same method two different ways. 
		private void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);


			App._file = new File(App._dir, string.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
			intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
			StartActivityForResult(intent, 0);
		}

		private void populateFacialMatch(FacialRecognitionResponse response)
		{
			TextView NameOfResponse = FindViewById<TextView>(Resource.Id.NameOfResponse);
			string responseString =  "Match: " + response.Name;

			NameOfResponse.Text = responseString;

			NameOfResponse.SetTextSize(ComplexUnitType.Dip, 21f);
		}
	}
}