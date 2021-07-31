using System.Drawing;
using System.Runtime.InteropServices;

namespace AutoPaint
{
    public static class DllImports
    {
        private static readonly int _leftMouseDown = 0x02;
        private static readonly int _leftMouseUp = 0x04;

        public static Point GetCursorPosition()
        {
            GetCursorPos(out POINT lpPoint);
            return lpPoint;
        }

        public static void LeftMouseClick(Bitmap bitmap, int x, int y, int xOffset, int yOffset, float brightnessTrigger)
        {
            Color nextPixel = x < bitmap.Width && y < bitmap.Height ? bitmap.GetPixel(x, y + 1) : default;

            SetCursorPos(x + xOffset, y + yOffset);
            mouse_event(_leftMouseDown, x, y, 0, 0);
            SetCursorPos(x + xOffset, y + yOffset);

            if (nextPixel == default || (nextPixel.GetBrightness() is float brightness && (invert ? brightness <= brightnessTrigger : brightness > brightnessTrigger)) || nextPixel.A < 225)
                mouse_event(_leftMouseUp, x + xOffset, y + yOffset, 0, 0);
        }

        public static void LeftMouseUp(int x, int y, int xOffset, int yOffset)
            => mouse_event(_leftMouseUp, x + xOffset, y + yOffset, 0, 0);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

    }
}
