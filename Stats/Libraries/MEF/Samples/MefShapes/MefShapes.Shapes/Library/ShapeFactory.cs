//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Samples.MefShapes.Shapes;
using Microsoft.Samples.MefShapes.Shapes.Library;

namespace Microsoft.Samples.MefShapes
{
    [Export]
    public class ShapeFactory
    {
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        [Import]
        private ICompositionService CompositionService { get; set; }

        public IShape GetRandomShape()
        {
            var shapeRetriever = new ShapeRetriever();

            CompositionService.SatisfyImports(shapeRetriever);

            int randomIndex = random.Next(shapeRetriever.PossibleShapes.Length);

            return shapeRetriever.PossibleShapes[randomIndex].GetExportedObject();
        }

        private class ShapeRetriever
        {
            [ImportMany(RequiredCreationPolicy = CreationPolicy.NonShared)]
            public Export<IShape, IShapeMetadata>[] PossibleShapes { get; set; }
        }
    }
}