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
        // File paths for backgrounds
        const String busPath = @"../../../Assets\Backgrounds\bus.png";
        const String farmPath = @"";

        private Colors color = Colors.Red;
        public Backgrounds background = Backgrounds.AlreadySet;
        public Canvas drawingCanvas;
        public Image backgroundImage;


        public DrawController(Canvas canvas, Image image)
        {
            drawingCanvas = canvas;
            backgroundImage = image;
        }

        public void setBackgroundFlag(Backgrounds new_background)
        {
            background = new_background;
        }


        public void ChangeBackground(Backgrounds new_background)
        {
            Console.WriteLine("Changing background to " + new_background);

            background = Backgrounds.AlreadySet;
            
            switch (new_background)
            {
                case Backgrounds.Farm:
                    //bitmap.UriSource = new Uri("pack://application:,,,/Resources/bus.jpg");
                    backgroundImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/animal.png"));
                    break;
                case Backgrounds.Pokemon:
                    Console.WriteLine("Got into pokemon case");
                    //bitmap.UriSource = new Uri("pack://application:,,,/Resources/animal.jpg");
                    backgroundImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pokemon.png"));
                    break;
                case Backgrounds.Turtle:
                    Console.WriteLine("Got into turtle case");
                    //bitmap.UriSource = new Uri("pack://application:,,,/Resources/animal.jpg");
                    backgroundImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/turtle.png"));
                    break;

                default:
                    break;
            }

            // And, in any case, clear screen:
            ClearScreen();

        }

        public void changeColor(Colors new_color)
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

            int colorValue = (int) (255 * (1 - (depth / 100.0)));
            if (colorValue < 0) colorValue = 0;
            if (colorValue > 255) colorValue = 255;

            // Change what color we actually make the ellipse:
            // 1: Red, 2: Blue, 3: Green
            if (color == Colors.Red)
            {
                mySolidColorBrush.Color = Color.FromArgb(100, (byte)colorValue, 0, 0);
            }
            else if (color == Colors.Green)
            {
                mySolidColorBrush.Color = Color.FromArgb(100, 0, (byte)colorValue, 0);
            }
            else if (color == Colors.Blue)
            {
                mySolidColorBrush.Color = Color.FromArgb(100, 0, 0, (byte)colorValue);
            }
            else if (color == Colors.White) 
            {
                mySolidColorBrush.Color = Color.FromArgb(100, 255, 255, 255);
            }

            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 0;
            myEllipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            myEllipse.Width = 20;
            myEllipse.Height = 20;

            Canvas.SetTop(myEllipse, y - myEllipse.Height/2);
            Canvas.SetLeft(myEllipse, x - myEllipse.Width/2);
            Canvas.SetZIndex(myEllipse, 0);

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
