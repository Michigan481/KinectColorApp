using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Kinect;

namespace KinectColorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private KinectSensor sensor;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];

                if (this.sensor.Status == KinectStatus.Connected)
                {
                    this.sensor.ColorStream.Enable();
                    this.sensor.DepthStream.Enable();
                    this.sensor.SkeletonStream.Enable(); //-- Disabled for now, because we probably won't use skeletal tracking
                    this.sensor.AllFramesReady += this.SensorAllFramesReady;
                    //this.sensor.DepthFrameReady += sensor_DepthFrameReady;
                    this.sensor.Start();
                }
            }
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("depth frame ready");
        }

        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Console.WriteLine("here");
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    Console.WriteLine("No depth frame");
                    return;
                }
              
                byte[] pixels = this.GenerateColoredBytes(depthFrame);

                int stride = depthFrame.Width * 4;
                image1.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }
        }

        private byte[] GenerateColoredBytes(DepthImageFrame depthFrame)
        {
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            // Create the RGB pixel array
            Byte[] pixels = new byte[depthFrame.Height * depthFrame.Width * 4];

            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;

            bool didDrawPoint = false;

            // Loop through data and set colors for each pixel
            for (int depthIndex = 0, colorIndex = 0;
                 depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                 depthIndex++, colorIndex += 4)
            {
                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                //Console.WriteLine(depth);

                if (depth == -1 || depth == 0) continue;

                byte intensity = CalculateIntensityFromDepth(depth);
                pixels[colorIndex + BlueIndex] = intensity;
                pixels[colorIndex + GreenIndex] = intensity;
                pixels[colorIndex + RedIndex] = intensity;

                if (depth <= 900)
                {
                   // Console.WriteLine(depth);
                    pixels[colorIndex + BlueIndex] = Convert.ToByte(255 * (1 - (depth / 900.0)));
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 255;


                    int x_kinect = (int) ((depthIndex) % depthFrame.Width);
                    int y_kinect = (int)((depthIndex) / depthFrame.Width);

                    double x_ratio = x_kinect / (double)depthFrame.Width;
                    double y_ratio = y_kinect / (double)depthFrame.Height;

                    int x = (int)(x_ratio*drawingCanvas.Width);
                    int y = (int)(y_ratio*drawingCanvas.Height);

                    if (!didDrawPoint)
                    {
                        Console.WriteLine(x_ratio);

                        var images = drawingCanvas.Children.OfType<Ellipse>().ToList();
                        //foreach (var image in images)
                        //{
                        //    drawingCanvas.Children.Remove(image);
                        //}
                        drawEllipseAtPoint(x, y);
                        didDrawPoint = true;
                    }
                }
            }

            return pixels;
        }

        void drawEllipseAtPoint(int x, int y)
        {
            // Create a red Ellipse.
            Ellipse myEllipse = new Ellipse();

            // Create a SolidColorBrush with a red color to fill the  
            // Ellipse with.
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.  
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 0;
            myEllipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            myEllipse.Width = 20;
            myEllipse.Height = 20;

            Canvas.SetTop(myEllipse, y);
            Canvas.SetLeft(myEllipse, x);

            // Add the Ellipse to the StackPanel.
            drawingCanvas.Children.Add(myEllipse);
        }

        public static byte CalculateIntensityFromDepth(int distance)
        {
            return (byte)(255 - (255 * Math.Max(distance - 800, 0) / 2000));
        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.AudioSource.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(this.sensor);
        }
    }
}
