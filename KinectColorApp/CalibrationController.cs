using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using ZXing.Kinect;

namespace KinectColorApp
{
    public delegate void MethodCaller();

    class CalibrationController
    {
        private KinectSensor sensor;
        private KinectController kinectController;
        private Canvas canvas;
        private Image debugImage;
        DrawController dc;

        Image[] codes;
        Point[] code_points = new Point[9];

        public CalibrationController(KinectSensor sensor, KinectController kinectController, Canvas canvas, Image[] codes, Image debugImage, DrawController dc)
        {
            this.canvas = canvas;
            this.codes = codes;
            this.debugImage = debugImage;
            this.kinectController = kinectController;
            this.sensor = sensor;
            this.dc = dc;
        }

        public void CalibrationAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    byte[] pixels = new byte[colorFrame.PixelDataLength];
                    colorFrame.CopyPixelDataTo(pixels);
                    int stride = colorFrame.Width * 4;
                    //debugImage.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    int code_num = find_code(colorFrame);
                    if (code_num >= 0)
                    {
                        // Make the next code visible.
                        if (code_num < 8)
                        {
                            codes[code_num].Visibility = Visibility.Hidden;
                            codes[code_num + 1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // We are done. Calculate the coefficients.
                            sensor.AllFramesReady -= this.CalibrationAllFramesReady;
                            codes[8].Visibility = Visibility.Hidden;
                            kinectController.calibration_coefficients = get_calibration_coeffs();
                            Point center_top_left = code_points[0];//codes[0].TransformToAncestor(canvas).Transform(new Point(codes[0].ActualWidth / 2, codes[0].ActualHeight / 2));
                            Point center_bot_right = code_points[4];//codes[4].TransformToAncestor(canvas).Transform(new Point(codes[4].ActualWidth / 2, codes[4].ActualHeight / 2));
                            kinectController.Calibrate((int)center_top_left.X, (int)center_top_left.Y, (int)center_bot_right.X, (int)center_bot_right.Y);
                            sensor.AllFramesReady += kinectController.SensorAllFramesReady;

                        }
                    }
                }
            }
        }

        int find_code(ColorImageFrame colorFrame)
        {
            ZXing.Kinect.BarcodeReader reader = new ZXing.Kinect.BarcodeReader();
            if (colorFrame != null)
            {
                //Decode the colorFrame
                var result = reader.Decode(colorFrame);
                if (result != null)
                {
                    string val = result.Text;
                    int code_num = Convert.ToInt32(val);
                    double center_x = result.ResultPoints[0].X +0.5 * (result.ResultPoints[2].X - result.ResultPoints[0].X);
                    double center_y = result.ResultPoints[0].Y +0.5 * (result.ResultPoints[2].Y - result.ResultPoints[0].Y);
                    Point p = new Point(center_x, center_y);
                    code_points[code_num] = p;

                    Console.WriteLine("Found code " + code_num + " at (" + center_x + ", " + center_y + ")");
                    return code_num;
                }
            }

            return -1;
        }

        double[] get_calibration_coeffs()
        {
            double[] coeffs = new double[6];

            // Make the Z matrix
            Matrix Z = new Matrix(9, 3);
            for (int i = 0; i < 9; i++)
            {
                Z[i, 0] = code_points[i].X;
                Z[i, 1] = code_points[i].Y;
                Z[i, 2] = 1;
            }

            // Make the display_x and display_y matrices
            Matrix D_x = new Matrix(9, 1);
            Matrix D_y = new Matrix(9, 1);
            for (int i = 0; i < 9; i++)
            {
                // Get the position of the code
                Point center = codes[i].TransformToAncestor(canvas).Transform(new Point(codes[i].ActualWidth / 2, codes[i].ActualHeight / 2));
                //dc.DrawEllipseAtPoint(center.X, center.Y, 50);
                D_x[i, 0] = center.X;
                D_y[i, 0] = center.Y;
            }

            // Calculate ABC, DEF
            Matrix ABC = (Matrix.Transpose(Z) * Z).Invert() * Matrix.Transpose(Z) * D_x;
            Matrix DEF = (Matrix.Transpose(Z) * Z).Invert() * Matrix.Transpose(Z) * D_y;

            coeffs[0] = ABC[0, 0];
            coeffs[1] = ABC[1, 0];
            coeffs[2] = ABC[2, 0];
            coeffs[3] = DEF[0, 0];
            coeffs[4] = DEF[1, 0];
            coeffs[5] = DEF[2, 0];

            return coeffs;
        }
    }
}

