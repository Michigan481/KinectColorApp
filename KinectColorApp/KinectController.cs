using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KinectColorApp
{
    class KinectController
    {
        private Image debugImage;
        private DrawController drawController;
        private SoundController soundController;

        private bool isCalibrated = false;
        private bool hasSetDepthThreshold = false;
        private int DepthThreshold = 9000000;
        const int TextileSpacing = 10;

        // Store the location and size of the textile in Kinect coordinates
        private Point topLeft;
        private Point bottomRight;
        private int textileWidth;
        private int textileHeight;

        public KinectController(DrawController dController, Image image, SoundController sController)
        {
            debugImage = image;
            drawController = dController;
            soundController = sController;
        }

        public void Calibrate(int top_left_x, int top_left_y, int bottom_right_x, int bottom_right_y)
        {
            topLeft = new Point(top_left_x, top_left_y);
            bottomRight = new Point(bottom_right_x, bottom_right_y);
            textileWidth = bottom_right_x - top_left_x;
            textileHeight = bottom_right_y - top_left_y;
            this.isCalibrated = true;
        }

        public void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    Console.WriteLine("No depth frame");
                    return;
                }

                if (this.isCalibrated) {
                    this.ParseDepthFrame(depthFrame);

                    byte[] pixels = this.GenerateColoredBytes(depthFrame);
                    int stride = depthFrame.Width * 4;
                    debugImage.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                }
                else
                {
                    // Display depth visualization in debug image by uncommenting below:
                    //byte[] pixels = this.GenerateColoredBytes(depthFrame);
                    //int stride = depthFrame.Width * 4;
                    //debugImage.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    // Display color video in debug image
                    ColorImageFrame colorFrame = e.OpenColorImageFrame();
                    byte[] pixels = new byte[colorFrame.PixelDataLength];
                    colorFrame.CopyPixelDataTo(pixels);
                    int stride = colorFrame.Width * 4;
                    debugImage.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                }
                
            }
        }

        #region Getting textile touches

        private void ParseDepthFrame(DepthImageFrame depthFrame)
        {
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            int minDepth = DepthThreshold;
            int bestDepthIndex = -1;

            // Loop through data and set colors for each pixel
            int minDepthIndex = (int)this.topLeft.Y * depthFrame.Width;
            int maxDepthIndex = (int)this.bottomRight.Y * depthFrame.Width;

            for (int depthIndex = minDepthIndex; depthIndex < maxDepthIndex; depthIndex++)
            {
                // Skip this depth index if it's horizontally outside of our textile
                int x_kinect = (int)((depthIndex) % depthFrame.Width);
                if (x_kinect < topLeft.X || x_kinect > bottomRight.X) continue;

                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // Ignore invalid depth values
                if (depth == -1 || depth == 0) continue;

                if (depth <= this.DepthThreshold)
                {
                    if (depth < minDepth)
                    {
                        minDepth = depth;
                        bestDepthIndex = depthIndex;
                    }
                }
            }

            // Draw if a touch was found
            if (bestDepthIndex >= 0)
            {
                if (!this.hasSetDepthThreshold) {
                    this.DepthThreshold = minDepth - TextileSpacing;
                    this.hasSetDepthThreshold = true;
                }
                else
                {
                    Console.WriteLine(this.DepthThreshold);
                    drawPoint(depthFrame, bestDepthIndex, minDepth);
                } 
            }
        }

        private void drawPoint(DepthImageFrame depthFrame, int depthIndex, int minDepth)
        {
            int x_kinect = (int)((depthIndex) % depthFrame.Width);
            int y_kinect = (int)((depthIndex) / depthFrame.Width);

            double x_ratio = (x_kinect - topLeft.X) / (double)textileWidth;
            double y_ratio = (y_kinect - topLeft.Y) / (double)textileHeight;

            int x = (int)(x_ratio * drawController.drawingCanvas.Width);
            int y = (int)(y_ratio * drawController.drawingCanvas.Height);

            drawController.drawEllipseAtPoint(x, y, (DepthThreshold - minDepth));
        }

        #endregion

        #region Image creation

        private byte[] GenerateColoredBytes(DepthImageFrame depthFrame)
        {
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            // Create the RGB pixel array
            Byte[] pixels = new byte[depthFrame.Height * depthFrame.Width * 4];

            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;

            // Loop through data and set colors for each pixel
            for (int depthIndex = 0, colorIndex = 0;
                 depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                 depthIndex++, colorIndex += 4)
            {
                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                if (depth == -1 || depth == 0) continue;

                byte intensity = CalculateIntensityFromDepth(depth);
                pixels[colorIndex + BlueIndex] = intensity;
                pixels[colorIndex + GreenIndex] = intensity;
                pixels[colorIndex + RedIndex] = intensity;
            }

            return pixels;
        }

        private static byte CalculateIntensityFromDepth(int distance)
        {
            return (byte)(255 - (255 * Math.Max(distance - 800, 0) / 2000));
        }

        #endregion
    }
}
