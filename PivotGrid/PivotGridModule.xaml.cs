using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.CommonUtilities.ColorBrush;
using Ranet.Olap.Core.Types;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Windows;

namespace PivotGrid
{
	[Demo(1)]
	public partial class PivotGridModule
	{
		private string mdxQuery = @"SELECT 
HIERARCHIZE(HIERARCHIZE([Date].[Calendar].Levels(0).Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
HIERARCHIZE(HIERARCHIZE([Sales Territory].[Sales Territory].Levels(0).Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Date].[Calendar Year].&[2007]}}, {{[Employee].[Employee Department].[Department].&[Facilities and Maintenance]}}) on COLUMNS FROM [Adventure Works]) 
WHERE ([Measures].[Sales Amount Quota], {{[Date].[Calendar Year].&[2007]}}, {{[Employee].[Employee Department].[Department].&[Facilities and Maintenance]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE";

		private string updateScript = @"UPDATE CUBE [Adventure Works] SET ([Date].[Calendar Year].&[2007]
 ,[Employee].[Employee Department].[Department].&[Facilities and Maintenance]
 ,<%[Sales Territory].[Sales Territory]%>
 ,<%[Date].[Calendar]%>
 ,[Measures].[Sales Amount Quota]) = <%newValue%> USE_EQUAL_ALLOCATION";

		public PivotGridModule()
		{
			InitializeComponent();
			ControlName = PivotGridResource.PivotGridCustomization_Name;
			About = PivotGridResource.PivotGridCustomization_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/Description.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitPivotGrid()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			PivotGridControl.IsAutoOpenExportedExcelFile = true;
			PivotGridControl.Query = mdxQuery;
			PivotGridControl.UpdateScript = updateScript;

			PivotGridChangeOptions();

			PivotGridControl.ShowWritebackSettingMenuItem = false;

			if (!PivotGridControl.UseCellConditionsDesigner)
			{
				var conds = new CellConditionsDescriptorBase("[Measures].[Sales Amount Quota]");
				var cellApp = new CellAppearanceObject(Colors.Cyan, Colors.Black, Colors.Black)
				{
					Options = { UseBackColor = true }
				};
				var cond = new CellConditionBase(CellConditionType.GreaterOrEqual, 1000000.0, 1000000.0, cellApp);
				conds.Conditions.Add(cond);
				PivotGridControl.CustomCellsConditions = new List<CellConditionsDescriptorBase> { conds };
			}

			PivotGridControl.Initialize();
		}

		private void PivotGridChangeOptions()
		{
			PivotGridControl.MembersViewMode = (ViewModeTypes)cbMembersViewMode.SelectedIndex;
			PivotGridControl.MemberVisualizationType = (MemberVisualizationTypes)cbMemberVisualizationType.SelectedIndex;
			PivotGridControl.DataReorganizationType = (DataReorganizationTypes)cbDataReorganizationType.SelectedIndex;
			PivotGridControl.RowsDefaultMemberAction = (MemberClickBehaviorTypes)cbDefaultMemberAction.SelectedIndex;
			PivotGridControl.ColumnsDefaultMemberAction = (MemberClickBehaviorTypes)cbColumnTitleClickBehavior.SelectedIndex;
			PivotGridControl.ColumnsDefaultSortMode = PivotTableSortTypes.SortByContext;
			PivotGridControl.DrillDownMode = (DrillDownMode)cbDrillDownMode.SelectedIndex;
			PivotGridControl.IsUpdateable = ckbIsUpdateable.IsChecked.Value;
			PivotGridControl.AutoWidthColumns = ckbAutoWidthColumns.IsChecked.Value;
			PivotGridControl.ColumnsIsInteractive = ckbColumnsIsInteractive.IsChecked.Value;
			PivotGridControl.RowsIsInteractive = ckbRowsIsInteractive.IsChecked.Value;
			PivotGridControl.UseColumnsAreaHint = ckbUseColumnsAreaHint.IsChecked.Value;
			PivotGridControl.UseRowsAreaHint = ckbUseRowsAreaHint.IsChecked.Value;
			PivotGridControl.UseCellsAreaHint = ckbUseCellsAreaHint.IsChecked.Value;
			PivotGridControl.UseCellConditionsDesigner = ckbUseCellConditionsDesigner.IsChecked.Value;
			PivotGridControl.PivotGrid.ShowMdxMenuItem = ckbShowMdx.IsChecked.Value;
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			PivotGridModule_OnLoaded(null, null);
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			PivotGridModule_OnLoaded(null, null);
		}

		private void PivotGridModule_OnLoaded(object sender, RoutedEventArgs e)
		{
			InitPivotGrid();

			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;

			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void ApplyOptionsButton_OnClickButton(object sender, RoutedEventArgs e)
		{
			PivotGridChangeOptions();
			PivotGridControl.Initialize(true);
		}

		private void QueryDesignerButton_OnClickButton(object sender, RoutedEventArgs e)
		{
			var queryScript = new MdxQueryAndUpdateScriptControl(CurrentConnection, GetDataLoader());

			var window = new FloatingDialog
			{
				Content = queryScript,
				Title = "Generate MDX Query and Update Script",
				Buttons = FloatingDialogButtons.Ok,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				Width = 1000,
				Height = 600
			};

			window.Closed += WindowOnClosed;
			window.ShowDialog();
		}
		private void WindowOnClosed(object sender, EventArgs eventArgs)
		{
			var window = (FloatingDialog)sender;
			var control = (MdxQueryAndUpdateScriptControl)window.Content;

			PivotGridControl.Query = control.Designer.PivotGrid.TransformedQuery;
			PivotGridControl.UpdateScript = string.IsNullOrEmpty(control.Script.Text) ? control.Designer.UpdateScript : control.Script.Text;

			PivotGridChangeOptions();

			PivotGridControl.Initialize();
		}
	}
}