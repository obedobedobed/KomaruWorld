using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class Extensions
{
    public static bool IntersectsNonInclusive(this Rectangle rectangle, Rectangle intersectRectangle)
    {
        return rectangle.Right > intersectRectangle.Left &&
               rectangle.Left < intersectRectangle.Right &&
               rectangle.Bottom > intersectRectangle.Top &&
               rectangle.Top < intersectRectangle.Bottom;
    }

    public static bool IntersectsX(this Rectangle rectangle, Rectangle intersectRectangle)
    {
        return rectangle.Right > intersectRectangle.Left &&
               rectangle.Left < intersectRectangle.Right;
    }

    public static Point NormalizeForWindow(this MouseState mouse)
    {
        return new Point
        (
            (int)(mouse.X / (Game1.Instance.Window.ClientBounds.Width / (float)VIRTUAL_WIDTH)),
            (int)(mouse.Y / (Game1.Instance.Window.ClientBounds.Height / (float)VIRTUAL_HEIGHT))
        );
    }
}