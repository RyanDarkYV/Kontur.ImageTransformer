using System;
using System.Drawing;
using System.Drawing.Imaging;
using Kontur.ImageTransformer.Handlers.ImageHandlers;
using Kontur.ImageTransformer.Handlers.Parsers;
using System.Net;

namespace Kontur.ImageTransformer.Handlers.RequestHandlers
{
    internal static class RequestHandler
    {
        #region Properties
        public static int RequestProcessedCount { get; private set; }
        public static int MaxProcessingTime { get; set; }
        #endregion

        #region HandleContext
        internal static void HandleRequest(HttpListenerContext context)
        {
            var operationData = RequestParser.ParseQuery(context.Request.RawUrl);
            var oldImage = ImageHandler.GetImageFromRequest(context.Request);
            var newImage = ImageHandler.TransformImage(oldImage, operationData);

            SendImage(context, newImage);
        }
        #endregion

        #region SendImage
        internal static void SendImage(HttpListenerContext context, Bitmap image)
        {
            context.Response.ContentType = "image/png";
            image.Save(context.Response.OutputStream, ImageFormat.Png);
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.OutputStream.Close();
        }
        #endregion

        #region CloseResponseWithCode
        public static void CloseResponseWithCode(HttpListenerContext context, HttpStatusCode code)
        {
            context.Response.StatusCode = (int) code;
            context.Response.OutputStream.Close();
        }
        #endregion
    }
}