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
using System.Windows.Media.Animation;

namespace KinectColorApp
{
    enum Colors {Red, Green, Blue, White};
    //enum Backgrounds {Farm, Pokemon, Turtle, Planets, Pony, Car, AlreadySet};

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

            buttons = new Ellipse[] { red_selector, blue_selector, green_selector };
            drawController = new DrawController(drawingCanvas, backgroundImage, colorRect, image1);
            soundController = new SoundController();
            kinectController = new KinectController(drawController, image1, soundController, buttons);
            //galileoController = new GalileoController(drawController, soundController, "COM3", 9600);
        }

        private CalibrationController calController;
        private DrawController drawController;
        private SoundController soundController;
        private KinectController kinectController;
        //private GalileoController galileoController;
        private KinectSensor sensor;
        bool has_started_calibrating = false;
        Ellipse[] buttons;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];

				if (this.sensor.Status == KinectStatus.Connected)
				{
					Image[] codes = new Image[] { _0_code, _1_code, _2_code, _3_code, _4_code, };
					foreach (Image i in codes)
					{
						i.Visibility = Visibility.Hidden;
					}
					calController = new CalibrationController(sensor, kinectController, drawingCanvas, codes, image1);
					calController.CalibrationDidComplete += new CalibrationController.calibrationDidCompleteHandler(calibrationCompleted);
					//sensor.AllFramesReady += kinectController.SensorAllFramesReady;//
					//calibrationCompleted();//
					this.sensor.ColorStream.Enable();
					this.sensor.DepthStream.Enable();
				}
            }

            this.KeyDown += new KeyEventHandler(OnKeyDown);
            soundController.StartMusic();
            drawController.ChangeBackground();
            drawController.ChangeColor(Colors.Red);

            // Faded colored rectangle:
            colorRect.Width = drawingCanvas.ActualWidth;
            Canvas.SetZIndex(colorRect, 11);

            foreach (Ellipse ellipse in buttons)
            {
                ellipse.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Size_Did_Change(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Width = drawingGrid.ActualWidth;
            drawingCanvas.Height = drawingCanvas.Width * (3.0 / 4.0);

            image1.Width = drawingGrid.ActualWidth;
            image1.Height = drawingCanvas.Width * (3.0 / 4.0);

            drawBorder.Width = drawingGrid.ActualWidth;
            drawBorder.Height = drawBorder.Width * (3.0 / 4.0);

            backgroundImage.Width = drawingGrid.ActualWidth - 40;
            backgroundImage.Height = drawingGrid.ActualHeight - 40;
            Canvas.SetLeft(backgroundImage, 0);
        }

        private void calibrationCompleted()
        {
            calibrationLabel.Content = "Done!";
            DoubleAnimation newAnimation = new DoubleAnimation();
            newAnimation.From = calibrationLabel.Opacity;
            newAnimation.To = 0.0;
            newAnimation.Duration = new System.Windows.Duration(TimeSpan.FromSeconds(2));
            newAnimation.AutoReverse = false;

            calibrationLabel.BeginAnimation(MediaElement.OpacityProperty, newAnimation, HandoffBehavior.SnapshotAndReplace);

            colorRect.Visibility = Visibility.Visible;
            backgroundImage.Visibility = Visibility.Visible;
            Canvas.SetZIndex(calibrationLabel, 2);
            Canvas.SetZIndex(backgroundImage, 1);

            foreach (Ellipse ellipse in buttons)
            {
                Canvas.SetLeft(ellipse, drawingCanvas.Width - ellipse.Width - 5);
                ellipse.Visibility = Visibility.Visible;
                Canvas.SetZIndex(ellipse, 2);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
			if (!has_started_calibrating)
			{
				_0_code.Visibility = Visibility.Visible;
				this.sensor.AllFramesReady += calController.CalibrationAllFramesReady;
				this.sensor.Start();
				calibrationLabel.Content = "Calibrating...";
				has_started_calibrating = true;
			}

            if (e.Key.ToString() == "R" || e.Key.ToString() == "F") {
                drawController.ClearScreen();
            }
            else if (e.Key.ToString() == "B" || e.Key.ToString() == "G")
            {
                soundController.TriggerBackgroundEffect();
                drawController.CycleBackgrounds();
            }
            else if (e.Key.ToString() == "Q" || e.Key.ToString() == "Space")
            {
                Application.Current.Shutdown();
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D3) || e.Key == Key.W || e.Key == Key.A || e.Key == Key.S)
            {
                if (e.Key == Key.W)
                {
                    HandleColorChange(0);
                }
                else if (e.Key == Key.A)
                {
                    HandleColorChange(1);
                }
                else if (e.Key == Key.S)
                {
                    HandleColorChange(2);
                }
                else
                {
                    HandleColorChange(e.Key - Key.D0);
                }

            }
        }

        void HandleColorChange(int inColor)
        {
            Colors c = (Colors)(inColor);
            soundController.TriggerColorEffect((int)c);
            drawController.ChangeColor(c);
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
