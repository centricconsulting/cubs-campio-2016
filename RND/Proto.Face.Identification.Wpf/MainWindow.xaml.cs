using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proto.Face.Identification.Wpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("6eca31c3a55a4968bfae16fc35fb54df");

        public MainWindow() {
            InitializeComponent();
        }

        private async void RegisterFaces() {
            this.BrowseButton.IsEnabled = false;
            Title = "Please wait ...";

            // Create an empty person group
            string personGroupId = "mykids";
            var pg = await faceServiceClient.GetPersonGroupAsync(personGroupId);
            if (pg != null)
                await faceServiceClient.DeletePersonGroupAsync(personGroupId);

            await faceServiceClient.CreatePersonGroupAsync(personGroupId, "My Kids");

            // Define Micah
            CreatePersonResult micah = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Micah"
            );

            // Define Jon
            //CreatePersonResult jon = await faceServiceClient.CreatePersonAsync(
            //    // Id of the person group that the person belonged to
            //    personGroupId,
            //    // Name of the person
            //    "Jon"
            //);

            // Directory contains image files of Micah
            string micahImageDir = @"C:\temp\samples\micah";
            var counter = 0;
            foreach (string imagePath in Directory.GetFiles(micahImageDir, "*.png")) {
                if (counter == 15) {
                    counter = 0;
                    Thread.Sleep(61000);
                }

                using (Stream s = File.OpenRead(imagePath)) {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, micah.PersonId, s);
                }
                counter++;
            }

            // Do the same for Jon
            //string jonImageDir = @"C:\Users\Johannes\Pictures\jon";

            //foreach (string imagePath in Directory.GetFiles(jonImageDir, "*.jpg")) {
            //    using (Stream s = File.OpenRead(imagePath)) {
            //        // Detect faces in the image and add to Anna
            //        await faceServiceClient.AddPersonFaceAsync(
            //            personGroupId, jon.PersonId, s);
            //    }
            //}

            await faceServiceClient.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true) {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running) {
                    break;
                }

                await Task.Delay(1000);
            }

            this.BrowseButton.IsEnabled = true;
            Title = "Ready...";
        }


        private async void BrowseButton_Click(object sender, RoutedEventArgs e) {
            string personGroupId = "mykids";
            var openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
            bool? result = openDlg.ShowDialog(this);

            if (!(bool)result) {
                return;
            }

            string filePath = openDlg.FileName;

            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            FacePhoto.Source = bitmapSource;

            Title = "Identifying...";

            using (Stream s = File.OpenRead(filePath)) {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.OrderBy(f => f.FaceId).Select(face => face.FaceId).ToArray();
                var identifyResults = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);

                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;

                foreach (var face in faces) {
                    DrawRectangle(drawingContext, face.FaceRectangle, resizeFactor);

                    if (identifyResults.Any(f => f.FaceId == face.FaceId)) {
                        var identifyResult = identifyResults.FirstOrDefault(f => f.FaceId == face.FaceId);
                        var name = "UNKNOWN";
                        if (identifyResult.Candidates.Any()) {
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            name = person.Name;
                        }
                        drawingContext.DrawText(new FormattedText(name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 32, Brushes.Black),
                            new Point(face.FaceRectangle.Left * resizeFactor, face.FaceRectangle.Top * resizeFactor));
                    }

                }

                drawingContext.Close();
                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96, 96, PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);
                FacePhoto.Source = faceWithRectBitmap;
            }
            Title = "Identifying... Done";
        }

        private void DrawRectangle(DrawingContext drawingContext, FaceRectangle faceRect, double resizeFactor) {
            drawingContext.DrawRectangle(
                Brushes.Transparent,
                new Pen(Brushes.Red, 2),
                new Rect(
                    faceRect.Left * resizeFactor,
                    faceRect.Top * resizeFactor,
                    faceRect.Width * resizeFactor,
                    faceRect.Height * resizeFactor
                    )
            );
        }

        private void LearnButton_Click(object sender, RoutedEventArgs e) {
            RegisterFaces();
        }
    }
}
