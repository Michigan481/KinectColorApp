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
    enum Colors {Red, Green, Blue, White};
    enum Backgrounds {Farm, Pokemon, Turtle, Planets, Pony, Car, AlreadySet};

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            drawingCanvas.Width = drawingGrid.ActualWidth;
            drawingCanvas.Height = drawingCanvas.Width * (3.0 / 4.0);
            backgroundImage.Width = drawingGrid.ActualWidth;
            backgroundImage.Height = drawingGrid.ActualHeight;
           
            drawController = new DrawController(drawingCanvas, backgroundImage, colorRect);
            soundController = new SoundController();

            kinectController = new KinectController(drawController, image1, soundController);
            //galileoController = new GalileoController(drawController, "COM3", 9600);
        }

        private DrawController drawController;
        private SoundController soundController;
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
            soundController.PlayMusic(Backgrounds.Farm);
            drawController.ChangeBackground(Backgrounds.Farm);
            drawController.ChangeColor(Colors.Red);

            // Faded colored rectangle:
            colorRect.Width = drawingCanvas.ActualWidth;
            Canvas.SetZIndex(colorRect, 11);
        }

        private void Window_Size_Did_Change(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Width = drawingGrid.ActualWidth;
            drawingCanvas.Height = drawingCanvas.Width * (3.0 / 4.0);

            calibrationBorder.Width = drawingGrid.ActualWidth;
            calibrationBorder.Height = calibrationBorder.Width*(3.0/4.0);

            drawBorder.Width = drawingGrid.ActualWidth;
            drawBorder.Height = drawBorder.Width * (3.0 / 4.0);

            backgroundImage.Width = drawingGrid.ActualWidth - 40;
            backgroundImage.Height = drawingGrid.ActualHeight - 40;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
            if (e.Key.ToString() == "R") {
                drawController.ClearScreen();
            }
            else if (e.Key.ToString() == "B")
            {
                drawController.CycleBackgrounds();
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D3)
            {
                drawController.ChangeColor((Colors)(e.Key - Key.D0));
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
            var h = 0.75 * w;
            //var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point endPoint = e.GetPosition(drawingCanvas);
            Console.WriteLine(rect.Height);
            this.kinectController.Calibrate((int)startPoint.X - 10, (int)startPoint.Y - 10, (int)(startPoint.X + rect.Width - 10), (int)(startPoint.Y + rect.Height - 10));

            this.calibrationBorder.Visibility = Visibility.Hidden;
            this.image1.Visibility = Visibility.Hidden;
            rect.Visibility = Visibility.Hidden;
            rect = null;

            Canvas.SetZIndex(backgroundImage, 2);
            Canvas.SetZIndex(drawBorder, 1);
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
