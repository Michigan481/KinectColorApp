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
        private int color = 1;
        public Canvas drawingCanvas; 

        public DrawController(Canvas canvas)
        {
            drawingCanvas = canvas;
        }

        public void changeColor(int new_color)
        {
            color = new_color;
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

            int colorValue = (int) (255 * (depth / 170.0));
            if (colorValue < 0) colorValue = 0;

            // Change what color we actually make the ellipse:
            // 1: Red, 2: Blue, 3: Green
            if (color == 1)
            {
                mySolidColorBrush.Color = Color.FromArgb(255, (byte)colorValue, 0, 0);
            }
            else if (color == 2)
            {
                mySolidColorBrush.Color = Color.FromArgb(255, 0, (byte)colorValue, 0);
            }
            else
            {
                mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, (byte)colorValue);
            }

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

        public void ClearScreen()
        {
            // Remove everything
            //drawingCanvas.Children.Clear();

            // Remove ellipses only (For testing with debug image)
            var shapes = drawingCanvas.Children.OfType<Ellipse>().ToList();
            foreach (var shape in shapes)
            {
                drawingCanvas.Children.Remove(shape);
            }
        }
    }
}
