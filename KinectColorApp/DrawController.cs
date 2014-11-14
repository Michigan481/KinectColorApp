using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectColorApp
{
    class DrawController
    {
        public Canvas drawingCanvas; 

        public DrawController(Canvas canvas)
        {
            drawingCanvas = canvas;
        }

        public void drawEllipseAtPoint(int x, int y, int depth)
        {
            // Create a red Ellipse.
            Ellipse myEllipse = new Ellipse();

            // Create a SolidColorBrush with a red color to fill the  
            // Ellipse with.
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.  
            // Each value has a range of 0-255.
            //Console.WriteLine(depth);

            int blueValue = (int) (255 * (depth / 170.0));
            if (blueValue < 0) blueValue = 0;
            mySolidColorBrush.Color = Color.FromArgb(255, (byte)blueValue, 0, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 0;
            myEllipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            myEllipse.Width = 40;
            myEllipse.Height = 40;

            Canvas.SetTop(myEllipse, y);
            Canvas.SetLeft(myEllipse, x);

            // Add the Ellipse to the StackPanel.
            drawingCanvas.Children.Add(myEllipse);
        }
    }
}
