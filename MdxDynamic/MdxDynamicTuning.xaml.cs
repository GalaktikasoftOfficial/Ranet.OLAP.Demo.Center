using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Types;
using Ranet.Olap.Core.Wrappers;
using Ranet.Olap.UI.Controls.DynamicPivotGrid;
using Ranet.Olap.UI.Controls.ToolBar;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Menu;
using Zaaml.UI.Controls.Primitives;
using Localization = Ranet.Olap.UI.Localization;

namespace MdxDynamic
{
	[Demo(3)]
	public partial class MdxDynamicTuning
	{
		public QueryDesignerControl CurrentDesigner = null;
		private CellSetData _dataPivotTable;
		private Button _navigateBack;
		private Button _navigateForward;
		private Button _navigateToBegin;
		private Button _navigateToEnd;

		public MdxDynamicTuning()
		{
			InitializeComponent();
			ControlName = DemoResources.Tuning_Name;
			About = DemoResources.Tuning_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/MdxDynamicTuning.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitializeMdxDynamic()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;

			pivotMdxDynamicControl.Connection = CurrentConnection;
			pivotMdxDynamicControl.OlapDataLoader = GetDataLoader();
			pivotMdxDynamicControl.AutoExecuteQuery = true;
			pivotMdxDynamicControl.CubeName = DefaultCubeName;
			pivotMdxDynamicControl.CubeCaption = DefaultCubeName;
			pivotMdxDynamicControl.UseCellConditionsDesigner = true;
			pivotMdxDynamicControl.ShowStateBar = true;
			pivotMdxDynamicControl.ShowActiveFiltersArea = false;
			pivotMdxDynamicControl.IsUpdateable = false;
			pivotMdxDynamicControl.IsAutomaticUpdateScript = true;
			pivotMdxDynamicControl.IsEnabledProcessCube = true;

			pivotMdxDynamicControl.IsAdvancedMode = true;
			pivotMdxDynamicControl.ShowStateBar = true;

			pivotMdxDynamicControl.CurrentDesignerChanged -= OnCurrentDesignerChanged;
			pivotMdxDynamicControl.CurrentDesignerChanged += OnCurrentDesignerChanged;

			pivotMdxDynamicControl.Initialized -= pivotMdxDynamicControlIntegration_Initialized;
			pivotMdxDynamicControl.Initialized += pivotMdxDynamicControlIntegration_Initialized;

			pivotMdxDynamicControl.Initialize();
		}

		private void MdxDynamicByTuple_OnLoaded(object sender, RoutedEventArgs e)
		{
			InitializeMdxDynamic();
		}

		private void OnCurrentDesignerChanged(object sender, EventArgs e)
		{
			CurrentDesigner = pivotMdxDynamicControl.CurrentDesigner;
		}

		private void UpdateablePivotGrid_LoadedCellSetData(object sender, EventArgs e)
		{
		}

		private void InitPivotGridExtension(CellSetData data)
		{
		}

		private void pivotMdxDynamicControlIntegration_Initialized(object sender, EventArgs e)
		{
			pivotMdxDynamicControl.CurrentDesigner.UpdateablePivotGrid.LoadedCellSetData -=
				UpdateablePivotGrid_LoadedCellSetData;
			pivotMdxDynamicControl.CurrentDesigner.UpdateablePivotGrid.LoadedCellSetData +=
				UpdateablePivotGrid_LoadedCellSetData;
		}

		private void ChangeDock(DockPivotGridExtension dock, bool isSetComboDock)
		{
			var cmd = pivotMdxDynamicControl.ChangeDockDesignerExtensionCommand;
			if (cmd != null)
				cmd.Execute(dock);
		}

		private void cdmMetadataArea_Click(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ToggleMetadataAreaCommand.Execute(null);
		}

		private void cdmDesigner_Click(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ToggleDesignerAreaCommand.Execute(null);
		}

		private void cdmActiveFilters_Click(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ToggleActiveFiltersAreaCommand.Execute(null);
		}

		private void MdxQueryArea_OnClick(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ToggleMdxQueryAreaCommand.Execute(null);
		}

		private void HeaderGrid_OnClick(object sender, RoutedEventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.ToggleHeaderGridCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void FooterGrid_OnClick(object sender, RoutedEventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.ToggleFooterGridCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CallCommand_CustomCalculationsClick(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ShowCalculatedMemberEditorCommand.Execute(null);
		}

		private void CallCommand_CellsStyleDesignerClick(object sender, RoutedEventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.ConditionsDesignerCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CallCommand_ResetDesignerClick(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ExecuteResetCommand();
		}

		private void CallCommand_ExecuteQueryClick(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ExecuteQueryCommand.Execute(null);
		}

		private void CallCommand_RefreshClick(object sender, RoutedEventArgs e)
		{
			pivotMdxDynamicControl.ExecuteQueryCommand.Execute(null);
		}

		private void CdmNavigateToBegin_OnLoaded(object sender, RoutedEventArgs e)
		{
			_navigateToBegin = sender as Button;
		}

		private void CdmNavigateBack_OnLoaded(object sender, RoutedEventArgs e)
		{
			_navigateBack = sender as Button;
		}

		private void CdmNavigateForward_OnLoaded(object sender, RoutedEventArgs e)
		{
			_navigateForward = sender as Button;
		}

		private void CdmNavigateToEnd_OnLoaded(object sender, RoutedEventArgs e)
		{
			_navigateToEnd = sender as Button;
		}

		private void CmdEmptyColumns_OnClick(object sender, RoutedEventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.ToggleEmptyColumnsCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CmdEmptyRows_OnClick(object sender, RoutedEventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.ToggleEmptyRowsCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CallCommand_RotateAxesClick(object sender, RoutedEventArgs routedEventArgs)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(d => d.UpdateablePivotGrid.RotateAxisCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CmdGoToFocusedCell_OnClick(object sender, RoutedEventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.GoToFocusedCellCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CmdZoom_OnValueChanged(object sender, EventArgs e)
		{
			ZoomingToolBarControl zoomingToolBarControl = sender as ZoomingToolBarControl;

			var upg = pivotMdxDynamicControl.CurrentDesigner.Return(d => d.UpdateablePivotGrid);
			if (upg != null)
				upg.ZoomValue = zoomingToolBarControl.Value;
		}

		private void CallCommand_OpenReportClick(object sender, RoutedEventArgs routedEventArgs)
		{
			pivotMdxDynamicControl.ShowImportLayoutDialogCommand.Execute(null);
		}

		private void CallCommand_SaveReportClick(object sender, RoutedEventArgs routedEventArgs)
		{
			pivotMdxDynamicControl.ShowExportLayoutDialogCommand.Execute(null);
		}

		private void CallCommand_ExcelExportClick(object sender, EventArgs e)
		{
			var cd = pivotMdxDynamicControl.CurrentDesigner;
			if (cd != null)
			{
#if SILVERLIGHT
				//cd.UpdateablePivotGrid.OverrideExportOptionsEvent -= UpdateablePivotGridOnOverrideExportOptionsEvent;
				//cd.UpdateablePivotGrid.OverrideExportOptionsEvent += UpdateablePivotGridOnOverrideExportOptionsEvent;
#endif
			}
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(d => d.UpdateablePivotGrid.ExportToExcelCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CallCommand_XMLExportClick(object sender, EventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(d => d.UpdateablePivotGrid.ExportToXmlCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CallCommand_ExportLayoutToExcelClick(object sender, EventArgs e)
		{
			if (CurrentDesigner == null)
				return;

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = Localization.SaveDialog_Filter_XlsFiles,
				DefaultExt = ".xlsx",
				FilterIndex = 1
			};

			var result = saveFileDialog.ShowDialog();
			if(!result.HasValue || !result.Value)
				return;

			MdxLayoutWrapper mdxLayoutWrapper = CurrentDesigner.ExportMdxLayout();

			ExportService exportService = new ExportService();
			exportService.ExportLayoutToExcelInterop(saveFileDialog.FileName, CurrentDesigner.Connection, mdxLayoutWrapper);
		}
		

		private void RunQueryAutomatic_OnClick(object sender, EventArgs e)
		{
			CheckBoxMenuItem autoToggleMenuItem = sender as CheckBoxMenuItem;
			pivotMdxDynamicControl.AutoExecuteQuery = autoToggleMenuItem.IsChecked.Value;
		}

		private void HideHints_OnClick(object sender, EventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.ToggleHideHintsCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void Normalize_OnClick(object sender, EventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(d => d.UpdateablePivotGrid.NormalizeSizeCommand);
			if (cmd != null)
				cmd.Execute(null);
		}

		private void CallCommand_SettingDesignerClick(object sender, EventArgs routedEventArgs)
		{
			pivotMdxDynamicControl.ShowMdxDesignerSettings();
		}

		private void Restore_OnClick(object sender, EventArgs e)
		{
			var cmd =
				pivotMdxDynamicControl.CurrentDesigner.Return(
					d => d.UpdateablePivotGrid.RestoreDefaultSizeCommand);
			if (cmd != null)
				cmd.Execute(null);
		}


		#region Group commands for the extension

		private void CallCommand_ChangeDockOnlyPivotGrid(object sender, EventArgs e)
		{
			ChangeDock(DockPivotGridExtension.OnlyPivotGrid, true);
		}

		private void CallCommand_ChangeDockTop(object sender, EventArgs e)
		{
			ChangeDock(DockPivotGridExtension.Top, true);
		}

		private void CallCommand_ChangeDockBottom(object sender, EventArgs e)
		{
			ChangeDock(DockPivotGridExtension.Bottom, true);
		}

		private void CallCommand_ChangeDockRight(object sender, EventArgs e)
		{
			ChangeDock(DockPivotGridExtension.Right, true);
		}

		private void CallCommand_ChangeDockLeft(object sender, EventArgs e)
		{
			ChangeDock(DockPivotGridExtension.Left, true);
		}

		private void CallCommand_ChangeDockOnlyExtension(object sender, EventArgs e)
		{
			ChangeDock(DockPivotGridExtension.OnlyExtension, true);
		}

		#endregion

	}
}
