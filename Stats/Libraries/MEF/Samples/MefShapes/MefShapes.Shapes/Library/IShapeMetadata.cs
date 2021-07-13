//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;

namespace Microsoft.Samples.MefShapes.Shapes.Library
{
    public enum ShapeType
    {
        /// <summary>
        /// Game shape, e.g. stick or square or corner
        /// </summary>
        GameShape,

        /// <summary>
        /// Box shape, i.e. the the box where the shapes get dropped to
        /// </summary>
        BoxShape,

    }

    public interface IShapeMetadata
    {
        ShapeType ShapeType { get; }

        string ShapeDescription { get; }
        
        /// <summary>
        /// This value can be used in shape frequency algorithms
        /// </summary>
        int ShapePriority { get; }
    }

    [MetadataAttribute]
    public sealed class ShapeAttribute:Attribute, IShapeMetadata
    {
        public ShapeAttribute(ShapeType shapeType, string shapeDescription, int shapePriority)
        {
            this.ShapeType = shapeType;
            this.ShapeDescription = shapeDescription;
            this.ShapePriority = shapePriority;
        }

        public ShapeType ShapeType { get; private set; }
        public string ShapeDescription { get; private set; }
        public int ShapePriority { get; private set; }
    }
}
