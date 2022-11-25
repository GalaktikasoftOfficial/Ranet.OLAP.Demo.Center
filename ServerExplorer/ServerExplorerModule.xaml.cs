using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.UI.Controls.General;
using Zaaml.PresentationCore.Utils;

namespace ServerExplorer
{
	[Demo]
	public partial class ServerExplorerModule
	{
		private string _currentCubeName;

		public ServerExplorerModule()
		{
			InitializeComponent();
			ControlName = ServerExplorerResource.ServerExplorer_Name;
			About = ServerExplorerResource.ServerExplorer_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/Description.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitializeServerExplorer()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;

			serverExplorerControl.Connection = CurrentConnection;
			serverExplorerControl.OlapDataLoader = GetDataLoader();
			serverExplorerControl.CanSelectCube = true;
			serverExplorerControl.ShowAllCubes = false;
			serverExplorerControl.Initialize();
			serverExplorerControl.CubeSelected -= serverExplorerControl_CubeSelected;
			serverExplorerControl.CubeSelected += serverExplorerControl_CubeSelected;

			btnInitFiltredServerExplorer.Click -= btnInitFiltredServerExplorer_Click;
			btnInitFiltredServerExplorer.Click += btnInitFiltredServerExplorer_Click;
			btnResetMetaFilters.IsEnabled = true;
			btnInitFiltredServerExplorer.IsEnabled = true;
			btnResetMetaFilters.Click -= btnResetMetaFilters_Click;
			btnResetMetaFilters.Click += btnResetMetaFilters_Click;

			cbMetaFilterType.SelectionChanged -= cbMetaFilterType_SelectionChanged;
			cbMetaFilterType.SelectionChanged += cbMetaFilterType_SelectionChanged;
		}

		private void serverExplorerControl_CubeSelected(object sender, CustomEventArgs<string> e)
		{
			_currentCubeName = e.Args;
			ResetFilters();
			InitServerExplorerControlFiltred();
		}

		private void btnInitFiltredServerExplorer_Click(object sender, RoutedEventArgs e)
		{
			InitServerExplorerControlFiltred();
		}

		private void InitServerExplorerControlFiltred()
		{
			if (string.IsNullOrEmpty(_currentCubeName))
				_currentCubeName = serverExplorerControl.CurrentCubeName;
			if (string.IsNullOrEmpty(CurrentConnection)) return;

			serverExplorerControlFiltred.CubeName = _currentCubeName;
			serverExplorerControlFiltred.OlapDataLoader = GetDataLoader();
			serverExplorerControlFiltred.Connection = CurrentConnection;
			serverExplorerControlFiltred.CanSelectCube = false;
			serverExplorerControlFiltred.ShowAllCubes = true;

			serverExplorerControlFiltred.MetadataFilterList = metafilterDataGrid.GetMetadataFilterList();
			serverExplorerControlFiltred.Initialize();
		}

		private void cbMetaFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbMetaFilterType.SelectedIndex == 0)
				metafilterDataGrid.FilterType = MetadataFilterType.Exclude;

			if (cbMetaFilterType.SelectedIndex == 1)
				metafilterDataGrid.FilterType = MetadataFilterType.Include;
		}

		private void btnResetMetaFilters_Click(object sender, RoutedEventArgs e)
		{
			ResetFilters();
		}

		private void ResetFilters()
		{
			metafilterDataGrid.ClearMetadataFilters();
			serverExplorerControlFiltred.MetadataFilterList = metafilterDataGrid.GetMetadataFilterList();
		}

		private void ServerExplorerControl_OnLoaded(object s, RoutedEventArgs e)
		{
			InitializeServerExplorer();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			InitializeServerExplorer();
		}
	}
}