using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Kontur.ImageTransformer.Handlers.RequestHandlers;

namespace Kontur.ImageTransformer.Handlers.ImageHandlers
{
    internal static class ImageHandler
    {
        #region Properties
        public static int MaxImageSize { get; set; } = 102400;

        private static Dictionary<string, RotateFlipType> _possibleOperations = new Dictionary<string, RotateFlipType>()
        {
            {"rotate-cw", RotateFlipType.Rotate90FlipNone},
            {"rotate-ccw", RotateFlipType.Rotate270FlipNone},
            {"flip-h", RotateFlipType.RotateNoneFlipX},
            {"flip-v", RotateFlipType.RotateNoneFlipY}
        };
        #endregion
        
        #region GetImage
        internal static Bitmap GetImageFromRequest(HttpListenerRequest request)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                request.InputStream.CopyTo(stream);

                if (stream == null || stream.Length > MaxImageSize)
                {
                    throw new ArgumentException("Stream is empty.");
                }
                var image = new Bitmap(stream);
                if (image.Width> 1000 || image.Height > 1000)
                {
                    throw new ArgumentException("Width and height can't be more than 1000px");
                }
                return new Bitmap(stream);
            }
        }
        #endregion

        #region TransformImage
        internal static Bitmap TransformImage(Bitmap image, RequestTransform transform)
        {
            image.RotateFlip(transform.Operation);

            var rectangular = transform.Rectangle;

            var imageRectangular = new Rectangle(0,0,image.Width,image.Height);
            rectangular.Intersect(imageRectangular);

            if (rectangular.Width == 0 || rectangular.Height == 0)
            {
                throw new ArgumentException("Width and height can't be equal to 0.");
            }

            var cuttedImage = image.Clone(rectangular, PixelFormat.Format32bppArgb);
            return cuttedImage;
        }
        #endregion

    }
}