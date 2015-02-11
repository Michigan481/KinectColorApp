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
using System.Threading;
using System.Windows.Threading;

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
            backgroundImage.Visibility = Visibility.Hidden;
            colorRect.Visibility = Visibility.Hidden;
            calibrationLabel.Visibility = Visibility.Hidden;

            Image[] codes = new Image[] { _0_code, _1_code, _2_code, _3_code, _4_code };
            foreach (Image i in codes) {
                i.Visibility = Visibility.Hidden;
            }
            _0_code.Visibility = Visibility.Visible;

            calController = new CalibrationController(drawingCanvas, codes, image1);
            drawController = new DrawController(drawingCanvas, backgroundImage, colorRect);
            soundController = new SoundController();
            kinectController = new KinectController(drawController, image1, soundController);
            //galileoController = new GalileoController(drawController, soundController, "COM3", 9600);
        }

        private CalibrationController calController;
        private DrawController drawController;
        private SoundController soundController;
        private KinectController kinectController;
        private GalileoController galileoController;
        private KinectSensor sensor;
        bool has_started_calibrating = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];

                if (this.sensor.Status == KinectStatus.Connected)
                {
                    this.sensor.ColorStream.Enable();
                    this.sensor.DepthStream.Enable();
                    this.sensor.AllFramesReady += calController.CalibrationAllFramesReady;
                    this.sensor.Start();
                }
            }

            this.KeyDown += new KeyEventHandler(OnKeyDown);
            soundController.StartMusic();
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

            drawBorder.Width = drawingGrid.ActualWidth;
            drawBorder.Height = drawBorder.Width * (3.0 / 4.0);

            backgroundImage.Width = drawingGrid.ActualWidth - 40;
            backgroundImage.Height = drawingGrid.ActualHeight - 40;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
            if (!has_started_calibrating)
            {
                calibrationLabel.Content = "Calibrating...";
                has_started_calibrating = true;
            }

            if (e.Key.ToString() == "R") {
                drawController.ClearScreen();
            }
            else if (e.Key.ToString() == "B")
            {
                soundController.TriggerBackgroundEffect();
                drawController.CycleBackgrounds();
            }
            else if (e.Key.ToString() == "Q")
            {
                Application.Current.Shutdown();
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D3)
            {
                Colors c = (Colors)(e.Key - Key.D0);
                soundController.TriggerColorEffect((int)c);
                drawController.ChangeColor(c);
            }
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
            //galileoController.closePort();
        }
    }
}
