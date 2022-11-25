using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.UI.Controls.Choices;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo(2)]
	public partial class MeasureChoices
	{
		private string _queryTemplate = @"SELECT 
 HIERARCHIZE({<%MeasuresQuery%>}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
 HIERARCHIZE(HIERARCHIZE([Customer].[Customer Geography].[Country].Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}) on COLUMNS FROM [Adventure Works]) 
WHERE ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE
";

		public MeasureChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.MeasureChoices_Name;
			About = ChoicesResource.MeasureChoices_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/MeasureChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}
		private void ConfigureByDefault()
		{
			MeasureChoiceControl.Connection = CurrentConnection;
			MeasureChoiceControl.OlapDataLoader = GetDataLoader();
			MeasureChoiceControl.ACubeName = DefaultCubeName;
			MeasureChoiceControl.SelectedInfo = null;
			MeasureChoiceControl.SelectedMeasures = null;
			MeasureChoiceControl.IsMultiSelect = ckbMultiSelect.IsChecked != null && ckbMultiSelect.IsChecked.Value;
			MeasureChoiceControl.AMeasureGroupName = "Internet Sales";

			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			PivotGridControl.Initialize();
		}

		private void measureChoiceControl_ApplySelectionEvent(object sender, EventArgs e)
		{
			var measureChoicePopUp = sender as MeasureChoicePopUp;
			var resultChoice = string.Empty;
			if (measureChoicePopUp != null)
			{
				if (measureChoicePopUp.IsMultiSelect)
				{
					//used multiple-choice
					foreach (MeasureInfo info in measureChoicePopUp.SelectedMeasures)
					{
						if (!string.IsNullOrEmpty(resultChoice))
						{
							resultChoice = resultChoice + ", ";
						}
						resultChoice = resultChoice + info.UniqueName;
					}
				}
				else
				{
					//used single choice
					resultChoice = measureChoicePopUp.SelectedInfo != null ? measureChoicePopUp.SelectedInfo.UniqueName : string.Empty;
				}
			}
			SelectedSetMeasure.Text = resultChoice;

			PivotGridControl.Query = _queryTemplate.Replace("<%MeasuresQuery%>", resultChoice);
			PivotGridControl.Initialize();
		}

		private void MeasureChoices_OnLoaded(object s, RoutedEventArgs e)
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
			MeasureChoiceControl.IsMultiSelect = ckbMultiSelect.IsChecked != null && ckbMultiSelect.IsChecked.Value;
		}
	}
}