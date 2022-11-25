using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ranet.Demo.Core;
using Ranet.Olap.UI.Controls.Choices;
using Zaaml.PresentationCore.Utils;

namespace Integrations
{
	[Demo]
	public partial class UseTwoConnections
	{
		private double _mFirstGridRow = 0.0;
		private double _mSecondGridRow = 0.0;

		public UseTwoConnections()
		{
			InitializeComponent();
			ControlName = IntegrationsResource.UseTwoConnections_Name;
			About = IntegrationsResource.UseTwoConnections_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/TwoConnectionsDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void UpdateConnectionInformation()
		{
			var firstIndex = ComboBoxConnection1.SelectedIndex;
			var secondIndex = ComboBoxConnection2.SelectedIndex;

			ComboBoxConnection1.ItemsSource = ConfigurationData.Storage.ListConnections;
			ComboBoxConnection2.ItemsSource = ConfigurationData.Storage.ListConnections;

			if (firstIndex >= 0 && firstIndex < ComboBoxConnection1.Items.Count) ComboBoxConnection1.SelectedIndex = firstIndex;
			if (secondIndex >= 0 && secondIndex < ComboBoxConnection2.Items.Count)
				ComboBoxConnection2.SelectedIndex = secondIndex;
		}

		private void PopUpCubeChoice1Conn_OnCubeSelected(object sender, CubeEventArgs e)
		{
			FirstPivotGrid.CubeName = e.CubeName;
			FirstPivotGrid.OlapDataLoader = GetDataLoader();
			FirstPivotGrid.Initialize();
		}

		private void PopUpCubeChoice2Conn_OnCubeSelected(object sender, CubeEventArgs e)
		{
			SecondPivotGrid.CubeName = e.CubeName;
			SecondPivotGrid.OlapDataLoader = GetDataLoader();
			SecondPivotGrid.Initialize();
		}

		private void ComboBoxFirstConnection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ComboBoxConnection1.SelectedIndex < 0) return;
			var connectionStr = ConfigurationData.Storage.ListConnections[ComboBoxConnection1.SelectedIndex].ConnectionString;
			CubeChoiceFirstCon.Connection = connectionStr;
			CubeChoiceFirstCon.NeedReload = true;
			FirstPivotGrid.Connection = connectionStr;
		}

		private void ComboBoxSecondConnection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ComboBoxConnection2.SelectedIndex < 0) return;
			var connectionStr = ConfigurationData.Storage.ListConnections[ComboBoxConnection2.SelectedIndex].ConnectionString;
			CubeChoiceSecondCon.Connection = connectionStr;
			CubeChoiceSecondCon.NeedReload = true;
			SecondPivotGrid.Connection = connectionStr;
		}

		private void UseTwoConnectionsModule_OnLoaded(object s, RoutedEventArgs e)
		{
			_mFirstGridRow = FirstPivotGridRow.ActualHeight;
			_mSecondGridRow = SecondPivotGridRow.ActualHeight;

			UpdateConnectionInformation();
			ConfigurationDataChanged -= OnConfigurationDataChanged;
			ConfigurationDataChanged += OnConfigurationDataChanged;
		}

		private void OnConfigurationDataChanged(object sender, EventArgs eventArgs)
		{
			UpdateConnectionInformation();
		}

		private void ViewModeControls_OnChecked(object sender, RoutedEventArgs e)
		{
			var button = sender as RadioButton;
		}

	}
}