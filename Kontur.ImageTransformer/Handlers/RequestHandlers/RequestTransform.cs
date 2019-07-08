using System.Drawing;

namespace Kontur.ImageTransformer.Handlers.RequestHandlers
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