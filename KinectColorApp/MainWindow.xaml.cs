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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            drawController = new DrawController(drawingCanvas);
            kinectController = new KinectController(drawController, image1);
            //galileoController = new GalileoController(drawController, "COM3", 9600);
        }

        private DrawController drawController;
        private KinectController kinectController;
        private GalileoController galileoController;
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
                    this.sensor.SkeletonStream.Enable();
                    this.sensor.AllFramesReady += kinectController.SensorAllFramesReady;
                    this.sensor.Start();
                }
            }

            this.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        private void Window_Size_Did_Change(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Width = drawingGrid.ActualWidth;
            drawingCanvas.Height = drawingGrid.ActualHeight;

            calibrationBorder.Width = drawingGrid.ActualWidth;
            calibrationBorder.Height = drawingGrid.ActualHeight;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
            if (e.Key.ToString() == "R") {
                drawController.ClearScreen();
            }
        }

        // Calibration
        private Point startPoint;
        private Rectangle rect;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(drawingCanvas);
            rect = new Rectangle
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.X);
            drawingCanvas.Children.Add(rect);
        }

        private void Canvas_MouseMoved(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || rect == null)
                return;

            var pos = e.GetPosition(drawingCanvas);

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point endPoint = e.GetPosition(drawingCanvas);
            this.kinectController.Calibrate((int)startPoint.X - 10, (int)startPoint.Y - 10, (int)endPoint.X - 10, (int)endPoint.Y - 10);

            this.calibrationBorder.Visibility = Visibility.Hidden;
            //this.image1.Visibility = Visibility.Hidden;
            //rect.Visibility = Visibility.Hidden;
            rect = null;
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
            galileoController.closePort();
        }
    }
}
