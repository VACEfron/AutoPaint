using System;
using System.Drawing;
using System.Threading;

namespace AutoPaint
{
    public class AutoPaint
    {
        private readonly int _xOffset = 30;
        private readonly int _yOffset = 200;
        
        // Float between 0-1 for at what brightness per pixel a mouse click will be triggered. 0 is 0%, 1 is 100%.
        private readonly float _brightnessTrigger = 0.5f;

        // The amount of time the program will wait before taking control of your mouse.
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(3);

        private Point _previousPos;

        public void Start()
        {
            Console.WriteLine("Press any key to start countdown.");
            Console.ReadKey(true);

            ActivateDelay(false);

            Console.WriteLine("Starting now...");
            Draw();            
        }

        private void Draw()
        {
            using var bitmap = new Bitmap("assets/image.jpg");

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Point mousePos = DllImports.GetCursorPosition();

                    if (GetDistance(_previousPos, new Point(mousePos.X - _xOffset, mousePos.Y - _yOffset)) is double distance
                        && distance > 1 && distance < 5)
                    {
                        Console.WriteLine("Cursor moved by user. Press any key to continue drawing.");
                        DllImports.LeftMouseUp(x, y, _xOffset, _yOffset);
                        Console.ReadKey(true);
                        ActivateDelay(true);
                    }

                    Color pixel = bitmap.GetPixel(x, y);

                    if (pixel.GetBrightness() is float brightness && (_invert ? brightness > _brightnessTrigger : brightness <= _brightnessTrigger) && pixel.A >= 225)
                        DllImports.LeftMouseClick(bitmap, x, y, _xOffset, _yOffset, _brightnessTrigger, _invert);

                    DllImports.SetCursorPos(x + _xOffset, y + _yOffset);

                    _previousPos = new Point(x, y);

                    Thread.Sleep(1);
                }
            }

            Console.WriteLine("Done. How did it turn out?");
        }

        private void ActivateDelay(bool continuing)
        {
            Console.WriteLine($"{(continuing ? "Continuing" : "Starting")} in {_delay.TotalSeconds} {(_delay.TotalSeconds == 1 ? "second" : "seconds")}. Move your cursor to stop drawing.");
            Thread.Sleep((int)_delay.TotalMilliseconds);
        }

        private static double GetDistance(Point pos1, Point pos2)
            => Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
    }
}
