using System.Drawing;

namespace Kontur.ImageTransformer.Handler
{
    internal class RequestTransform
    {
        public RequestTransform(RotateFlipType operation, Rectangle rectangle)
        {
            Operation = operation;
            Rectangle = rectangle;
        }

        public RotateFlipType Operation { get; }
        public Rectangle Rectangle { get; }

    }
}