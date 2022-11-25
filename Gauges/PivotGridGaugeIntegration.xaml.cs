using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Data;
using Zaaml.PresentationCore.Utils;

namespace Gauges
{
	[Demo]
	public partial class PivotGridGaugeIntegration
	{
		#region Private Fields

		private readonly string _gaugeQuery = @"
		WITH 
			MEMBER [Min] AS 0 
			MEMBER [Max] AS 100 
			MEMBER [Low] AS 65 
			MEMBER [High] AS 95 
			MEMBER [Value] AS 85 
		SELECT 
			{ [Min], [Max], [Low], [High], [Value] } 
			DIMENSION PROPERTIES KEY0, MEMBER_TYPE ON 0 
		FROM 
			{CubeName}";

		#endregion

		#region Constructors

		public PivotGridGaugeIntegration()
		{
			ControlName = GaugeResource.GaugeDataIntegration_Name;

			About = GaugeResource.GaugeDataIntegration_Caption;

			using (var reader =
				new StreamReader(GetType().Assembly.GetResourceStream("Resources/GaugeDataIntegrationDescription.txt")))
			{
				Description = reader.ReadToEnd();
			}

			InitializeComponent();
		}

		#endregion

		#region Private Helpers

		private void PivotGridControl_LoadedCellSetData(object sender, EventArgs e)
		{
			var cellSetData = sender as CellSetData;

			if (cellSetData == null) return;

			if(cellSetData.Cells.Count < 5) return;

			var min = double.Parse(cellSetData.Cells[0].Value.Value.ToString());
			var max = double.Parse(cellSetData.Cells[1].Value.Value.ToString());
			var low = double.Parse(cellSetData.Cells[2].Value.Value.ToString());
			var high = double.Parse(cellSetData.Cells[3].Value.Value.ToString());
			var value = double.Parse(cellSetData.Cells[4].Value.Value.ToString());

			InitializeGauge(min, max, low, high, value);
		}

		private void InitializePivotGrid()
		{
			var query = _gaugeQuery.Replace("{CubeName}", DefaultCubeName);
			QueryTextBlock.Text = query;

			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			PivotGridControl.IsShowToolBar = false;
			PivotGridControl.Query = query;
			PivotGridControl.Initialize();
		}

		private void InitializeGauge(double min, double max, double lowSector, double highSector, double value)
		{
			GaugeControl.Value = value;
			GaugeControl.Max = max;
			GaugeControl.Min = min;
			LowSector.Length = lowSector/(max - min);
			MiddleSector.Length = (highSector - lowSector)/(max - min);
			HighSector.Length = (max - highSector)/(max - min);
		}

		private void PivotGridGaugeIntegration_OnLoaded(object s, RoutedEventArgs e)
		{
			PivotGridControl.LoadedCellSetData += PivotGridControl_LoadedCellSetData;

			InitializePivotGrid();
		}

		#endregion
	}
}