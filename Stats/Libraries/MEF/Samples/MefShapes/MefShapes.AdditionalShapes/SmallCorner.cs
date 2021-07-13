﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.Samples.MefShapes.Shapes.Library;
using Microsoft.Samples.MefShapes.Shapes;

namespace Microsoft.Samples.MefShapes.AdditionalShapes
{
    [Export(typeof(IShape))]
    [Shape(ShapeType.GameShape, "2 by 2 corner shape", 0)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SmallCorner : RegularShape
    {
        protected override Cell[,] CreateMatrix(Color color, CellFactory cellFactory)
        {
            Cell[,] matrix = new Cell[3, 3];
            cellFactory.PositionNewCell(color, matrix, Cells, 0, 0);
            cellFactory.PositionNewCell(color, matrix, Cells, 0, 1);
            cellFactory.PositionNewCell(color, matrix, Cells, 1, 1);
            return matrix;
        }
    }
}
