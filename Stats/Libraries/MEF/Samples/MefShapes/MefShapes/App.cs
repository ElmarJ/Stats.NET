//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace Microsoft.Samples.MefShapes
{
    public partial class App : Application
    {
        private CompositionContainer _container;

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThreadAttribute()]
        public static void Main()
        {
            new App().Run();
        }

        [Import(typeof(MainWindow))]
        public new Window MainWindow
        {
            get { return base.MainWindow; }
            set { base.MainWindow = value; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Compose())
            {
                MainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (_container != null)
            {
                _container.Dispose();
            }
        }

        private bool Compose()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMefShapesGame).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(DefaultDimensions).Assembly));

            _container = new CompositionContainer(catalog);

            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(this);
            batch.AddExportedObject<ICompositionService>(_container);
            batch.AddExportedObject<AggregateCatalog>(catalog);

            try
            {
                _container.Compose(batch);
            }
            catch (CompositionException compositionException)
            {
                MessageBox.Show(compositionException.ToString());
                Shutdown(1);
                return false;
            }
            return true;
        }
    }
}
