﻿using System;
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
        const int TextileSpacing = 7; // How deep do we have to push in to start drawing?
        public double[] calibration_coefficients;

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
            // Check if we need to change background
            if (drawController.background != Backgrounds.AlreadySet)
            {
                drawController.ChangeBackground(drawController.background);
            }
            
            // Check if we need to change color
            if (drawController.shouldChangeColor != -1)
            {
                drawController.ChangeColor((Colors)drawController.shouldChangeColor);
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null) return;

                if (this.isCalibrated) {
                    this.ParseDepthFrame(depthFrame);

                    //byte[] pixels = this.GenerateColoredBytes(depthFrame);
                    //int stride = depthFrame.Width * 4;
                    //debugImage.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                }
                else
                {
                    // Display depth visualization in debug image by uncommenting below:
                    //byte[] pixels = this.GenerateColoredBytes(depthFrame);
                    //int stride = depthFrame.Width * 4;
                    //debugImage.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    // Display color video in debug image
                    using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                    {
                        if (colorFrame != null)
                        {
                            //byte[] pixels = new byte[colorFrame.PixelDataLength];
                            //colorFrame.CopyPixelDataTo(pixels);
                            //int stride = colorFrame.Width * 4;
                            //debugImage.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                            debugImage.Visibility = Visibility.Hidden;

                            Console.WriteLine("A: " + calibration_coefficients[0]);
                            Console.WriteLine("B: " + calibration_coefficients[1]);
                            Console.WriteLine("C: " + calibration_coefficients[2]);
                            Console.WriteLine("D: " + calibration_coefficients[3]);
                            Console.WriteLine("E: " + calibration_coefficients[4]);
                            Console.WriteLine("F: " + calibration_coefficients[5]);
                            isCalibrated = true;

                            //drawController.backgroundImage.Visibility = Visibility.Visible;
                            //Canvas.SetZIndex(drawController.backgroundImage, 2);
                        }
                    }
                }
                
            }
        }

        #region Getting textile touches

        bool gotTouch = false;

        private void ParseDepthFrame(DepthImageFrame depthFrame)
        {
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);

            int minDepth = DepthThreshold;
            int bestDepthIndex = -1;

            // Loop through data and set colors for each pixel
            int minDepthIndex = (int)this.topLeft.Y * depthFrame.Width;
            int maxDepthIndex = (int)this.bottomRight.Y * depthFrame.Width;

            //Console.WriteLine("Depth Threshold: " + DepthThreshold);

            for (int depthIndex = minDepthIndex; depthIndex < maxDepthIndex; depthIndex++)
            {
                // Skip this depth index if it's horizontally outside of our textile
                int x_kinect = (int)((depthIndex) % depthFrame.Width);
                if (x_kinect < topLeft.X)
                {
                    continue;
                }
                else if (x_kinect > bottomRight.X)
                {
                    depthIndex += (depthFrame.Width - (int)(bottomRight.X - topLeft.X - 1));
                    //Console.WriteLine(depthFrame.Width - (int)(bottomRight.X - topLeft.X - 1));
                    continue;
                }

                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // Ignore invalid depth values
                if (depth == -1 || depth == 0) continue;

                if (depth < minDepth)
                {
                    minDepth = depth;
                    bestDepthIndex = depthIndex;
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
                    soundController.StartMusic();
                    DrawPoint(depthFrame, bestDepthIndex, minDepth);
                    gotTouch = true;
                } 
            }
            else
            {
                if (gotTouch == true) soundController.StopMusic();
                gotTouch = false;
            }
        }

        private void DrawPoint(DepthImageFrame depthFrame, int depthIndex, int minDepth)
        {
            double x_kinect = (depthIndex % depthFrame.Width);
            double y_kinect = (depthIndex / depthFrame.Width);

            double x = x_kinect * calibration_coefficients[0] + y_kinect * calibration_coefficients[1] + calibration_coefficients[2] - 0;
            double y = x_kinect * calibration_coefficients[3] + y_kinect * calibration_coefficients[4] + calibration_coefficients[5];

            x = drawController.drawingCanvas.Width - x;
            //Console.WriteLine("Drawing at " + x + ", " + y);

            drawController.DrawEllipseAtPoint(x, y, (DepthThreshold - minDepth));
        }

        #endregion

        #region Image creation

        public byte[] GenerateColoredBytes(DepthImageFrame depthFrame)
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
