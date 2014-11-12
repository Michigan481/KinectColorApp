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
        }

        private DrawController drawController;
        private KinectController kinectController;
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
        }

        private void Window_Size_Did_Change(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Width = drawingGrid.ActualWidth;
            drawingCanvas.Height = drawingGrid.ActualHeight;
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
