using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace PivotGrid
{
	[Demo(3)]
	public partial class PivotGridUpdateable
	{
		private string mdxQuery = @"SELECT 
HIERARCHIZE(HIERARCHIZE([Date].[Calendar].Levels(0).Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
HIERARCHIZE(HIERARCHIZE([Sales Territory].[Sales Territory].Levels(0).Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Date].[Calendar Year].&[2007]}}, {{[Employee].[Employee Department].[Department].&[Facilities and Maintenance]}}) on COLUMNS FROM [Adventure Works]) 
WHERE ([Measures].[Sales Amount Quota], {{[Date].[Calendar Year].&[2007]}}, {{[Employee].[Employee Department].[Department].&[Facilities and Maintenance]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE";

		private string updateScript = @"UPDATE CUBE [Sales Targets]
SET
-- first Quarter (a Measures group is related to the first Quarter - the Quarter before the granulation)
([Measures].[Sales Amount Quota], Descendants(<%[Date].[Calendar]%>, [Date].[Calendar].[Calendar Quarter]).Item(0), <%[Employee].[Employees]%>, <%[Sales Territory].[Sales Territory]%>) 
= 
-- cell in which the write data in the cube
( [Measures].[Sales Amount Quota], Descendants(<%[Date].[Calendar]%>, [Date].[Calendar].[Calendar Quarter]).Item(0), <%[Employee].[Employees]%>, <%[Sales Territory].[Sales Territory]%> )
+
<%newValue%>
-
-- cell which we see and in which we write data on a form
( [Measures].[Sales Amount Quota], <%[Date].[Calendar]%>, <%[Employee].[Employees]%>, <%[Sales Territory].[Sales Territory]%> )";

		public PivotGridUpdateable()
		{
			InitializeComponent();
			ControlName = PivotGridResource.PivotGridUpdateable_Name;
			About = PivotGridResource.PivotGridUpdateable_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/GridUpdateabeDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitPivotGrid()
		{
			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			PivotGridControl.Query = mdxQuery;
			PivotGridControl.UpdateScript = updateScript;
			PivotGridControl.UseCellConditionsDesigner = true;
			PivotGridControl.IsUpdateable = true;
			PivotGridControl.ShowWritebackSettingMenuItem = false;


			PivotGridControl.Initialize();

			var wb = Resources["wbCommandsPivotGrid"] as WBCommandsPivotGrid;
			if (wb != null)
				wb.SetControl(PivotGridControl);
		}

		private void PivotGridUpdateable_OnLoaded(object s, RoutedEventArgs e)
		{
			InitPivotGrid();

			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;

			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			PivotGridUpdateable_OnLoaded(null, null);
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			PivotGridUpdateable_OnLoaded(null, null);
		}
	}
}