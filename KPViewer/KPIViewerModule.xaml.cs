using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace KPIViewer
{
	[Demo]
	public partial class KPIViewerModule
	{
		public KPIViewerModule()
		{
			InitializeComponent();
			ControlName = KPIViewerResources.Overview_Name;
			About = KPIViewerResources.Overview_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/Description.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitializeKpi()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			kpiViewerControl.Connection = CurrentConnection;
			kpiViewerControl.OlapDataLoader = GetDataLoader();
			kpiViewerControl.ShowMetadataArea(true);
			kpiViewerControl.Initialize();
		}

		private void KPIViewerModule_OnLoaded(object s, RoutedEventArgs e)
		{
			InitializeKpi();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			InitializeKpi();
		}
	}
}