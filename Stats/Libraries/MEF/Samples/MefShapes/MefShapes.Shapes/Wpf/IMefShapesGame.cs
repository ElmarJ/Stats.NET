//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using Microsoft.Samples.MefShapes.Shapes;
using Microsoft.Samples.MefShapes.Shapes.Library;

namespace Microsoft.Samples.MefShapes
{
    public interface IMefShapesGame
    {
        ObservableCollection<IShape> Shapes { get; }
        ObservableCollection<Export<IShape, IShapeMetadata>> SelectionShapes { get; }
        IShape ActiveShape { get; }
        void StartGame(Dispatcher dispatcher, int timerTickInterval);
        void StopGame();
        bool IsRunning { get; }
    }
}