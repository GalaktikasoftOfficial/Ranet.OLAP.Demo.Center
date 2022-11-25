using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.Types;
using Ranet.Olap.Core.Wrappers;

namespace MdxDynamic
{
	public class ExportService
	{
		#region Fields

		private readonly Application _application = new Application();
		private PivotCache _pivotCache;
		private PivotTable _pivotTable;
		private Workbook _workbook;
		private Worksheet _workSheet;

		#endregion

		#region  Methods

		private void ApplyMembersFilter(PivotTable pivotTable, Members_FilterWrapper membersFilter)
		{
			//список всех выбранных элементов и иерархий
			List<MemberChoiceSettings> memberChoiceSettings = MemberChoiceSettings.ToList(membersFilter.SelectedInfo);
			if (!memberChoiceSettings.Any())
				return;
			//список выбранных элементов для каждой иерархии
			Dictionary<string, List<string>> selectedGroups = new Dictionary<string, List<string>>();
			//группируем по LevelName:
			foreach (var mcc in memberChoiceSettings)
			{
				string levelName = mcc.Info.LevelName;

				if (!selectedGroups.ContainsKey(levelName))
					selectedGroups.Add(levelName, new List<string>());

				if (mcc.SelectState == SelectStates.Selected_Self)
					selectedGroups[levelName].Add(mcc.UniqueName);
			}

			foreach (string level in selectedGroups.Keys)
			{
				if (selectedGroups[level].Any())
				{
					var pf = (PivotField) pivotTable.VisibleFields[level];
					pf.VisibleItemsList = selectedGroups[level].ToArray();
				}
			}
		}

		private void ExportLayoutToExcel(string fileName, string connectionString, MdxLayoutWrapper mdxLayoutWrapper)
		{
			var missing = Missing.Value;

			_application.Visible = false;
			_application.EnableEvents = false;
			_workbook = _application.Workbooks.Add(missing);
			_workSheet = (Worksheet) _workbook.ActiveSheet;
			var range = _workSheet.Range["A3", missing];
			string connection = @"OLEDB;" + connectionString;
			string connectionName = "connName";

			string commandText = mdxLayoutWrapper.CubeName.Replace("[", "").Replace("]", "");

			_workbook.Connections.Add(connectionName, "", connection, commandText, 1);
			_pivotCache = _workbook.PivotCaches().Create(XlPivotTableSourceType.xlExternal,
				_workbook.Connections[connectionName],
				XlPivotTableVersionList.xlPivotTableVersion15);

			// ************* Local Cube
			if (connection.Contains(@":\") && connection.Contains(@".cub"))
			{
				_pivotCache.LocalConnection = connection;
				_pivotCache.UseLocalConnection = true;
				_pivotCache.Refresh();
			}
			// ************* Local Cube

			_pivotTable = _pivotCache.CreatePivotTable(
				range.Address[missing, missing, XlReferenceStyle.xlR1C1, missing, missing],
				"OLAPPivot", missing, XlPivotTableVersionList.xlPivotTableVersion15);

			_pivotTable.EnableDrilldown = true;
			_pivotTable.ManualUpdate = true;


			var cf = _pivotTable.CubeFields;


			//filters area
			int filterPosition = 1;
			foreach (var hierWrapper in mdxLayoutWrapper.Filters.OfType<Filtered_AreaItemWrapper>())
			{
				string filterCaption = hierWrapper.GetHierarchyUniqueName();
				var pvfilter = cf.Item[filterCaption];
				pvfilter.Orientation = XlPivotFieldOrientation.xlPageField;
				pvfilter.Position = filterPosition;

				pvfilter.EnableMultiplePageItems = true;

				filterPosition++;
			}

			//rows:
			int rowPosition = 1;
			foreach (var hierWrapper in mdxLayoutWrapper.Rows.OfType<Filtered_AreaItemWrapper>())
			{
				string rowCaption = hierWrapper.GetHierarchyUniqueName();

				var pvRow = cf.Item[rowCaption];
				pvRow.Orientation = XlPivotFieldOrientation.xlRowField;
				pvRow.Position = rowPosition;

				rowPosition++;
			}

			//columns
			int columnPosition = 1;
			foreach (var hierWrapper in mdxLayoutWrapper.Columns.OfType<Filtered_AreaItemWrapper>())
			{
				string columnCaption = hierWrapper.GetHierarchyUniqueName();
				var pvColumn = cf.Item[columnCaption];
				pvColumn.Orientation = XlPivotFieldOrientation.xlColumnField;
				pvColumn.Position = columnPosition;

				columnPosition++;
			}

			//data(measure)
			int dataPosition = 1;
			foreach (var hierWrapper in mdxLayoutWrapper.Data.OfType<Measure_AreaItemWrapper>())
			{
				string dataCaption = hierWrapper.UniqueName;
				var pvdata = cf.Item[dataCaption];
				pvdata.Orientation = XlPivotFieldOrientation.xlDataField;
				pvdata.Position = dataPosition;

				dataPosition++;
			}

			//Применение фильтров:
			foreach (var hierWrapper in mdxLayoutWrapper.Filters.OfType<Filtered_AreaItemWrapper>())
				ApplyMembersFilter(_pivotTable, hierWrapper.CompositeFilter.MembersFilter);
			foreach (var hierWrapper in mdxLayoutWrapper.Rows.OfType<Filtered_AreaItemWrapper>())
				ApplyMembersFilter(_pivotTable, hierWrapper.CompositeFilter.MembersFilter);
			foreach (var hierWrapper in mdxLayoutWrapper.Columns.OfType<Filtered_AreaItemWrapper>())
				ApplyMembersFilter(_pivotTable, hierWrapper.CompositeFilter.MembersFilter);

			_workbook.SaveAs(fileName, XlFileFormat.xlWorkbookDefault, null, null, false, false);
			_pivotTable.ManualUpdate = false;
			_application.ActiveWorkbook.Saved = true;

			Marshal.FinalReleaseComObject(_pivotTable);
			Marshal.FinalReleaseComObject(_pivotCache);
			Marshal.FinalReleaseComObject(range);
			Marshal.FinalReleaseComObject(_application.Worksheets);
			Marshal.FinalReleaseComObject(_application.Workbooks);

			_workbook.Close(false);
			Marshal.FinalReleaseComObject(_workbook);
			_application.Quit();
			Marshal.FinalReleaseComObject(_application);

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public void ExportLayoutToExcelInterop(string fileName, string connectionString,
			MdxLayoutWrapper mdxLayoutWrapper)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException(nameof(fileName));
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException(nameof(connectionString));
			if (mdxLayoutWrapper == null)
				throw new ArgumentNullException(nameof(mdxLayoutWrapper));

			ExportLayoutToExcel(fileName, connectionString, mdxLayoutWrapper);
		}

		#endregion
	}
}