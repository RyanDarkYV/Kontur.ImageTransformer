using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Kontur.ImageTransformer.Handlers.RequestHandlers;

namespace Kontur.ImageTransformer.Handlers.Parsers
{
    internal static class RequestParser
    {
        #region Properties
        private static readonly string NumberRegex = @"(?:-?\d{1,})";
        private static readonly string QueryRegex = $"^/process/[\\w-]+/(?:{NumberRegex},){{3}}{NumberRegex}";

        
        private static Dictionary<string, RotateFlipType> _possibleOperations = new Dictionary<string, RotateFlipType>()
        {
            {"rotate-cw", RotateFlipType.Rotate90FlipNone},
            {"rotate-ccw", RotateFlipType.Rotate270FlipNone},
            {"flip-h", RotateFlipType.RotateNoneFlipX},
            {"flip-v", RotateFlipType.RotateNoneFlipY}
        };
        #endregion

        #region ParseQuery
        internal static RequestTransform ParseQuery(string request)
        {
            if (!Regex.IsMatch(request, QueryRegex))
            {
                throw new ArgumentException("Wrong query. You need to specify operation type and coordinates.");
            }


            var queryParameters = request.Split('/', '?');
            var operationName = queryParameters[2];

            // Possible null exception. Rework to List<int> and int.TryParse.
            var rectangleCoords = queryParameters[3].Split(',').Select(coordinates => int.Parse(coordinates)).ToArray();


            if (!_possibleOperations.ContainsKey(operationName))
            {
                throw new ArgumentException(
                    "Wrong query. You need to specify operation type. Possible parameters: rotate-cw, rotate-ccw, flip-h, flip-v");
            }

            if (rectangleCoords.Length != 4)
            {
                throw new ArgumentException("Wrong query. You need to enter 4 numbers for coordinates.");
            }

            var axisX = rectangleCoords[0];
            var axisY = rectangleCoords[1];
            var rectangleWidth = rectangleCoords[2];
            var rectangleHeight = rectangleCoords[3];

            var rectangle = new Rectangle(Math.Min(axisX, axisX + rectangleWidth),
                Math.Min(axisY, axisY + rectangleHeight), Math.Abs(rectangleWidth), Math.Abs(rectangleHeight));

            var requestTransform = new RequestTransform(_possibleOperations[operationName], rectangle);

            return requestTransform;
        }
        #endregion
    }
}