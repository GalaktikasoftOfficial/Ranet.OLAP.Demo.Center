using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.UI.Controls.General;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo("Metadata Choice Widget", 0)]
	public partial class ChoiceControls
	{
		public ChoiceControls()
		{
			InitializeComponent();
			ControlName = ChoicesResource.OverviewChoice_Name;
			About = ChoicesResource.OverviewChoice_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/ChoiceControlsDescription.txt")))
				Description = reader.ReadToEnd();
		}
		private void UpdateConnectionInformation()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;

			CubeChoiceControl.Connection = CurrentConnection;
			CubeChoiceControl.Loader = GetDataLoader();

			CubeChoiceControl.Initialize();

			DimensionChoiceControl.Clear();
			DimensionChoiceControl.ClearTree();
			HierarchyChoiceControl.Clear();
			HierarchyChoiceControl.ClearTree();
			LevelChoiceControl.Clear();
			LevelChoiceControl.ClearTree();
		}

		private void cubeChoiceControl_SelectedItemChanged(object sender, ItemEventArgs e)
		{
			if (CubeChoiceControl.SelectedInfo == null || string.IsNullOrEmpty(CurrentConnection))
				return;

			DimensionChoiceControl.Connection = CurrentConnection;
			DimensionChoiceControl.CubeName = CubeChoiceControl.SelectedInfo.Name;
			DimensionChoiceControl.OlapDataLoader = GetDataLoader();
			DimensionChoiceControl.Initialize();
			HierarchyChoiceControl.Clear();
			HierarchyChoiceControl.ClearTree();
			LevelChoiceControl.Clear();
			LevelChoiceControl.ClearTree();

		}

		private void dimensionChoiceControl_SelectedItemChanged(object sender, ItemEventArgs e)
		{
			if (CubeChoiceControl.SelectedInfo == null || DimensionChoiceControl.SelectedInfo == null ||
				string.IsNullOrEmpty(CurrentConnection))
				return;

			HierarchyChoiceControl.Connection = CurrentConnection;
			HierarchyChoiceControl.CubeName = CubeChoiceControl.SelectedInfo.Name;
			HierarchyChoiceControl.DimensionUniqueName = DimensionChoiceControl.SelectedInfo.UniqueName;
			HierarchyChoiceControl.OlapDataLoader = GetDataLoader();
			HierarchyChoiceControl.Initialize();
			LevelChoiceControl.Clear();
			LevelChoiceControl.ClearTree();
		}

		private void hierarchyChoiceControl_SelectedItemChanged(object sender, ItemEventArgs e)
		{
			if (CubeChoiceControl.SelectedInfo == null || HierarchyChoiceControl.SelectedInfo == null ||
				string.IsNullOrEmpty(CurrentConnection))
				return;

			LevelChoiceControl.Connection = CurrentConnection;
			LevelChoiceControl.CubeName = CubeChoiceControl.SelectedInfo.Name;
			LevelChoiceControl.DimensionUniqueName = DimensionChoiceControl.SelectedInfo.UniqueName;
			LevelChoiceControl.HierarchyUniqueName = HierarchyChoiceControl.SelectedInfo.UniqueName;
			LevelChoiceControl.OlapDataLoader = GetDataLoader();
			LevelChoiceControl.Initialize();
		}

		private void ChoiceControls_OnLoaded(object s, RoutedEventArgs e)
		{
			UpdateConnectionInformation();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			UpdateConnectionInformation();
		}
	}
}