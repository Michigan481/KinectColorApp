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
        private Canvas canvas;
        private Image debugImage;

        Image[] codes;
        Point[] code_points = new Point[5];

        public CalibrationController(Canvas canvas, Image[] codes, Image debugImage)
        {
            this.canvas = canvas;
            this.codes = codes;
            this.debugImage = debugImage;
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
                    double center_x = result.ResultPoints[0].X + 0.5 * (result.ResultPoints[2].X - result.ResultPoints[0].X);
                    double center_y = result.ResultPoints[0].Y + 0.5 * (result.ResultPoints[2].Y - result.ResultPoints[0].Y);
                    Point p = new Point(center_x, center_y);
                    code_points[code_num] = p;

                    Console.WriteLine("Found code " + code_num + " at (" + center_x + ", " + center_y + ")");
                    return code_num;
                }
            }

            return -1;
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
                    debugImage.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    int code_num = find_code(colorFrame);
                    if (code_num >= 0)
                    {
                        // Make the next code visible.
                        if (code_num < 4)
                        {
                            codes[code_num].Visibility = Visibility.Hidden;
                            codes[code_num + 1].Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
    }
}

