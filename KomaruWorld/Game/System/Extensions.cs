using Microsoft.Xna.Framework;

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
}