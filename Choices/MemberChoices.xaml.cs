using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo("Member Choice Widget", 1)]
	public partial class MemberChoices
	{

		private string _queryTemplate = @"SELECT 
NON EMPTY HIERARCHIZE({[Measures].[Internet Sales Amount], [Measures].[Internet Gross Profit]}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
NON EMPTY HIERARCHIZE(HIERARCHIZE(<%MemberQuery%>)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}) on COLUMNS FROM <%CubeName%>
WHERE ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}})) 
WHERE ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE";

		private const string HierarchyMdxQueryBuilder = "[Customer].[Customer Geography]";
		private const string LevelMdxQueryBuilder = "[Customer].[Customer Geography].[Customer]";

		public MemberChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.MemberChoices_Name;
			About = ChoicesResource.MemberChoices_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/MemberChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void ConfigureByDefault()
		{
			if (!MemberChoice.SelectedSet.Contains(HierarchyMdxQueryBuilder))
			{
				MemberChoice.SelectedInfo.Clear();
				MemberChoice.SelectedSet = string.Empty;
			}
			MemberChoice.Connection = CurrentConnection;
			MemberChoice.OlapDataLoader = GetDataLoader();
			MemberChoice.CubeName = DefaultCubeName;
			MemberChoice.HierarchyUniqueName = HierarchyMdxQueryBuilder;
			MemberChoice.StartLevelUniqueName = LevelMdxQueryBuilder;
			MemberChoice.Step = (int)nudStep.Value;
			MemberChoice.MultiSelect = ckbMultiSelect.IsChecked != null && ckbMultiSelect.IsChecked.Value;
			MemberChoice.Initialize();

			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			PivotGridControl.Initialize();
		}

		private void MemberChoice_OnSelectedInfoChanged(object sender, EventArgs e)
		{
			if (MemberChoice.SelectedSet == null) return;
			var query = _queryTemplate.Replace("<%MemberQuery%>", MemberChoice.SelectedSet);
			PivotGridControl.Query = query.Replace("<%CubeName%>", DefaultCubeName);
			PivotGridControl.Initialize();
		}

		private void MemberChoices_OnLoaded(object s, RoutedEventArgs e)
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

		private void ApplyOptionsButton_OnClickButton(object sender, RoutedEventArgs e)
		{
			MemberChoice.Step = (int)nudStep.Value;
			MemberChoice.MultiSelect = ckbMultiSelect.IsChecked != null && ckbMultiSelect.IsChecked.Value;
			MemberChoice.Initialize();
		}
	}
}