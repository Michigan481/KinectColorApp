using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

namespace KinectColorApp
{
    class DrawController
    {
        private Colors color = Colors.Red;
        public Backgrounds background = Backgrounds.AlreadySet;

        int prevBackground = 0;
        public Canvas drawingCanvas;
        public Image backgroundImage;

        public DrawController(Canvas canvas, Image image)
        {
            drawingCanvas = canvas;
            backgroundImage = image;
        }

        public void CycleBackgrounds()
        {
            int currBackground = prevBackground + 1;
            if (currBackground == (int)Backgrounds.AlreadySet)
            {
                currBackground = 0;
            }

            prevBackground = currBackground;
            background = (Backgrounds)currBackground;
        }


        public void ChangeBackground(Backgrounds new_background)
        {
            Console.WriteLine("Changing background to " + new_background);

            background = Backgrounds.AlreadySet;
            
            switch (new_background)
            {
                case Backgrounds.Farm:
                    backgroundImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/animal.png"));
                    break;
                case Backgrounds.Pokemon:
                    backgroundImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pokemon.png"));
                    break;
                case Backgrounds.Turtle:
                    backgroundImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/turtle.png"));
                    break;

                default:
                    break;
            }

            // And, in any case, clear screen:
            ClearScreen();

        }

        public void ChangeColor(Colors new_color)
        {
            color = new_color;
        }

        public void DrawEllipseAtPoint(double x, double y, int depth)
        {
            // Create an ellipse with a gradient brush
            Ellipse myEllipse = new Ellipse();
            RadialGradientBrush brush = new RadialGradientBrush();

            int colorValue = (int)(255 * (depth / 100.0));
            if (colorValue < 0) colorValue = 0;
            if (colorValue > 255) colorValue = 255;

            // Set the color based on depth data
            if (color == Colors.Red)
            {
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(200, 255, (byte)colorValue, 0), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(200, 255, (byte)colorValue, 0), 0.4));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, (byte)colorValue, 0), 1.0));
            }
            else if (color == Colors.Green)
            {
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(200, 0, 255, (byte)colorValue), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(200, 0, 255, (byte)colorValue), 0.4));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 255, (byte)colorValue), 1.0));
            }
            else if (color == Colors.Blue)
            {
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(200, (byte)colorValue, 0, 255), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(200, (byte)colorValue, 0, 255), 0.4));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, (byte)colorValue, 0, 255), 1.0));
            }
            else if (color == Colors.White) 
            {
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 255), 0.0));
                brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 255), 1.0));
            }

            myEllipse.Fill = brush;
            myEllipse.StrokeThickness = 0;

            // Set the width and height of the Ellipse.
            myEllipse.Width = 25;
            myEllipse.Height = 25;

            Canvas.SetTop(myEllipse, y - myEllipse.Height/2);
            Canvas.SetLeft(myEllipse, x - myEllipse.Width/2);
            Canvas.SetZIndex(myEllipse, 0);

            // Add the Ellipse to the drawingCanvas
            drawingCanvas.Children.Add(myEllipse);
        }

        public void ClearScreen()
        {
            // Remove ellipses only
            var shapes = drawingCanvas.Children.OfType<Ellipse>().ToList();
            foreach (var shape in shapes)
            {
                drawingCanvas.Children.Remove(shape);
            }
        }
    }
}
