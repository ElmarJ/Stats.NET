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
    [Export(typeof(IMefShapesGame))]
    public class MefShapesGame : IMefShapesGame
    {
        private DispatcherTimer timer;
        private bool isInitialized;

        public MefShapesGame()
        {
            Shapes = new ObservableCollection<IShape>();
            SelectionShapes = new ObservableCollection<Export<IShape, IShapeMetadata>>();
        }

        [Import]
        private IAccelerationStrategy AccelerationStrategy { get; set; }

        [Import]
        private ShapeFactory ShapeFactory { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public ObservableCollection<Export<IShape, IShapeMetadata>> SelectionShapes { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
        Import("Microsoft.Samples.MefShapes.Shapes.EnvironmentShapeContract")]
        private IShape EnvironmentShape
        {
            set { Shapes.Add(value); }
        }

        public IShape ActiveShape { get; private set; }

        public ObservableCollection<IShape> Shapes { get; private set; }

        public bool IsRunning { get { return timer != null && timer.IsEnabled; } }

        public void StartGame(Dispatcher dispatcher, int timerTickInterval)
        {
            if (!isInitialized)
            {
                SendNextShape();
                isInitialized = true;
            }

            if (timer == null || !timer.IsEnabled)
            {
                timer = new DispatcherTimer(TimeSpan.FromMilliseconds(timerTickInterval), DispatcherPriority.Normal, TimerTick, dispatcher);
            }
        }

        public void StopGame()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ActiveShape.Move(Direction.Down);
        }

        private void SendNextShape()
        {
            IShape shape = ShapeFactory.GetRandomShape();
            shape.ReachedBottom += shape_ReachedBottom;
            Shapes.Add(shape);
            ActiveShape = shape;
        }

        private void shape_ReachedBottom(object sender, ReachedBottomEventArgs e)
        {
            ((Shape)sender).ReachedBottom -= shape_ReachedBottom;

            if (e.LayersRemoved > 0 && timer != null)
            {
                if (AccelerationStrategy.Acceleration != 0)
                {
                    TimeSpan newInterval = TimeSpan.FromMilliseconds(timer.Interval.TotalMilliseconds / AccelerationStrategy.Acceleration);
                    timer.Interval = newInterval;
                }
            }

            SendNextShape();
        }
    }
}
