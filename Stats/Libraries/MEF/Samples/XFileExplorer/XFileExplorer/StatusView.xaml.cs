//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Samples.XFileExplorer
{
    /// <summary>
    /// Interaction logic for StatusView.xaml
    /// </summary>
    [Export("Microsoft.Samples.XFileExplorer.FileExplorerViewContract", typeof(UserControl))]
    [ExportMetadata("Name", "Status Pane")]
    [ExportMetadata("Docking", Dock.Bottom)]
    public partial class StatusView : UserControl
    {
        [ImportMany("Microsoft.Samples.XFileExplorer.StatusServiceContract")]
        public ExportCollection<UserControl, IStatusServiceMetadata> StatusCollection { get; set; }

        public StatusView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (StatusCollection.Count == 0)
                StatusInfo.Content = "No status service available";

            foreach (var status in StatusCollection.OrderBy(i => i.MetadataView.Index))
            {
                StatusPane.Children.Add(status.GetExportedObject());
            }
        }
    }

    public interface IStatusServiceMetadata
    {
        int Index { get; }
    }
}
