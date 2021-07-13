//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.MefShapes.Shapes.Library
{
    public class PictureShape : RegularShape
    {
        private string Picture;
        public PictureShape(string picture)
        {
            Picture = picture;
        }

        protected override Cell[,] CreateMatrix(System.Windows.Media.Color color, CellFactory cellFactory)
        {
            string[] lines = Picture.Split('/');
            int height = lines.Length;
            int width = lines[0].Length;
            Cell[,] matrix = new Cell[width, height];
            for (int row = 0; row < height; row++)
            {
                if (lines[row].Length != width)
                {
                    throw new FormatException(string.Format("Each line must be the same length ({0})", width));
                }
                for (int col = 0; col < width; col++)
                {
                    char ch = lines[row][col];
                    if (ch == '1')
                    {
                        cellFactory.PositionNewCell(color, matrix, Cells, col, row);
                    }
                }
            }
            return matrix;
        }
    }

    public class PictureShapeAdapter
    {
        [Import]
        public ICompositionService CompositionService { get; set; }

        [Export(CompositionConstants.AdapterContractName)]
        [ExportMetadata(CompositionConstants.AdapterFromContractMetadataName, "MefShapesShapePicture")]
        [ExportMetadata(CompositionConstants.AdapterToContractMetadataName, typeof(IShape))]
        public Export Adapt(Export export)
        {
            string contractName = AttributedModelServices.GetContractName(typeof(IShape));
            var metadata = new Dictionary<string, object>(export.Metadata);
            metadata[CompositionConstants.ExportTypeIdentityMetadataName] = AttributedModelServices.GetTypeIdentity(typeof(IShape));

            return new Export(contractName, metadata, () =>
            {
                string picture = (string)export.GetExportedObject();
                var shape = new PictureShape(picture);
                CompositionService.SatisfyImports(shape);

                return shape;
            });
        }
    }

    public static class StandardShapePictures
    {
        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "T shape", 0)]
        public const string TShape = "010/111/000";

        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "L shape", 0)]
        public const string LShape = "010/010/011";

        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "J shape", 0)]
        public const string JShape = "010/010/110";

        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "I shape", 0)]
        public const string IShape = "1000/1000/1000/1000";

        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "Square shape", 0)]
        public const string SquareShape = "11/11";

        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "S shape", 0)]
        public const string SShape = "011/110/000";

        [Export("MefShapesShapePicture")]
        [Shape(ShapeType.GameShape, "Z shape", 0)]
        public const string ZShape = "110/011/000";
    }
}
