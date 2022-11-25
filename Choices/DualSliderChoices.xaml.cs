using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo(5)]
	public partial class DualSliderChoices
	{

		private string queryTemplate = @"SELECT 
NON EMPTY HIERARCHIZE({<%DataCalendarDayName%>}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
NON EMPTY HIERARCHIZE(CrossJoin(HIERARCHIZE({<%DateCalendarYear%>}), HIERARCHIZE({<%DataCalendarMonthYear%>}))) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Sales Channel].[Sales Channel].&[Internet]}}) on COLUMNS FROM [Adventure Works]) 
WHERE ([Measures].[Internet Sales Amount], {{[Sales Channel].[Sales Channel].&[Internet]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE
";

		public DualSliderChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.DualSlider_Name;
			About = ChoicesResource.DualSlider_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/DualSliderChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}
		private void ConfigureByDefault()
		{
			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();

			//Initialize Sliders
			SliderDateCalendarYear.CubeName = DefaultCubeName;
			SliderDateCalendarYear.HierarchyUniqueName = "[Date].[Calendar]";
			SliderDateCalendarYear.LevelUniqueName = "[Date].[Calendar].[Calendar Year]";
			SliderDateCalendarYear.Connection = CurrentConnection;
			SliderDateCalendarYear.ShowTickBar = true;
			SliderDateCalendarYear.OlapDataLoader = GetDataLoader();
			//SliderDateCalendarYear.ResultSetChanged += SliderDateCalendarYearControlResultSetChanged;
			SliderDateCalendarYear.Initialize();

			SliderDataCalendarMonthYear.CubeName = DefaultCubeName;
			SliderDataCalendarMonthYear.HierarchyUniqueName = "[Date].[Month of Year]";
			SliderDataCalendarMonthYear.LevelUniqueName = "[Date].[Month of Year].[Month of Year]";
			SliderDataCalendarMonthYear.Connection = CurrentConnection;
			SliderDataCalendarMonthYear.ShowTickBar = true;
			SliderDataCalendarMonthYear.OlapDataLoader = GetDataLoader();
			//SliderDataCalendarMonthYear.ResultSetChanged += SliderDataCalendarMonthYearControlResultSetChanged;
			SliderDataCalendarMonthYear.Initialize();

			SliderDataCalendarDayName.CubeName = DefaultCubeName;
			SliderDataCalendarDayName.HierarchyUniqueName = "[Date].[Day Name]";
			SliderDataCalendarDayName.LevelUniqueName = "[Date].[Day Name].[Day Name]";
			SliderDataCalendarDayName.Connection = CurrentConnection;
			SliderDataCalendarDayName.ShowTickBar = true;
			SliderDataCalendarDayName.OlapDataLoader = GetDataLoader();
			//SliderDataCalendarDayName.ResultSetChanged += SliderDataCalendarDayNameControlOnResultSetChanged;
			SliderDataCalendarDayName.Initialize();

			//Replace Mdx Query parameters 
			PivotGridControl.Query = ReplaceQueryParameters();

			PivotGridControl.Initialize();
		}
		private string ReplaceQueryParameters()
		{
			var result = string.Empty;
			var query = queryTemplate;

			result = string.IsNullOrEmpty(SliderDateCalendarYear.ResultSet) ? "[Date].[Calendar].[All Periods]" : SliderDateCalendarYear.ResultSet;
			query = query.Replace("<%DateCalendarYear%>", result);

			result = string.IsNullOrEmpty(SliderDataCalendarMonthYear.ResultSet) ? "[Date].[Month of Year].[All Periods]" : SliderDataCalendarMonthYear.ResultSet;
			query = query.Replace("<%DataCalendarMonthYear%>", result);

			result = string.IsNullOrEmpty(SliderDataCalendarDayName.ResultSet) ? "[Date].[Day Name].[All Periods]" : SliderDataCalendarDayName.ResultSet;
			query = query.Replace("<%DataCalendarDayName%>", result);

			return query;
		}

		private void ApplyMemberSliderResultSetClick(object sender, RoutedEventArgs e)
		{
			PivotGridControl.Query = ReplaceQueryParameters();
			PivotGridControl.Initialize();
		}

		private void SliderDateCalendarYearControlResultSetChanged(object sender, EventArgs e)
		{
			PivotGridControl.Query = ReplaceQueryParameters();
			PivotGridControl.Initialize();
		}

		private void SliderDataCalendarMonthYearControlResultSetChanged(object sender, EventArgs eventArgs)
		{
			PivotGridControl.Query = ReplaceQueryParameters();
			PivotGridControl.Initialize();
		}

		private void SliderDataCalendarDayNameControlOnResultSetChanged(object sender, EventArgs e)
		{
			PivotGridControl.Query = ReplaceQueryParameters();
			PivotGridControl.Initialize();
		}

		private void SliderPromotionTypeControlOnResultSetChanged(object sender, EventArgs eventArgs)
		{
			PivotGridControl.Query = ReplaceQueryParameters();
			PivotGridControl.Initialize();
		}

		private void DualSliderChoices_OnLoaded(object s, RoutedEventArgs e)
		{
			ConfigureByDefault();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;

			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			ConfigureByDefault();
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			ConfigureByDefault();
		}
	}
}