using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.UI.Controls;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	/// <summary>
	/// Interaction logic for DatePickerChoices.xaml
	/// </summary>
	[Demo(3)]
	public partial class DatePickerChoices
	{
		private const string QueryTemplate = @"SELECT { Crossjoin( <%dateChoice%>, {[Measures].[Internet Average Sales Amount], [Measures].[Internet Sales Amount], [Measures].[Internet Standard Product Cost]})} ON Columns
, { {[Customer].[Customer Geography].[Country].Members} } ON Rows FROM [Adventure Works]";

		public DatePickerChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.DatePicker_Name;
			About = ChoicesResource.DatePicker_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/DatePickerChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitDataPicker()
		{
			DatePickerChoice.Connection = CurrentConnection;
			DatePickerChoice.OlapDataLoader = GetDataLoader();
			DatePickerChoice.CubeName = DefaultCubeName;
			DatePickerChoice.DayLevelUniqueName = "[Date].[Calendar].[Date]";
			// DateToUniqueNameTemplate describes how control should convert selected date time to time-dimension memeber.
			//  <D> - day;
			DatePickerChoice.DateToUniqueNameTemplate = "[Date].[Calendar].[Date].&[<YYYY><MM><DD>]";
			DatePickerChoice.SelectedDate = new DateTime(2007, 09, 25);

			DatePickerChoice.Initialize();
		}

		private void DatePickerChoices_OnLoaded(object s, RoutedEventArgs e)
		{
			InitDataPicker();

			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;
			DatePickerChoice.DatesLoaded -= DatePickerChoiceOnDatesLoaded;

			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
			DatePickerChoice.DatesLoaded += DatePickerChoiceOnDatesLoaded;
		}

		private void DatePickerChoiceOnDatesLoaded(object sender, EventArgs eventArgs)
		{
			InitializePivotGrid(ParseQuery(DatePickerChoice.SelectedDateUniqueName));
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			InitDataPicker();
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			InitDataPicker();
		}

		private void InitializePivotGrid(string query)
		{
			PivotGridChoice.Connection = CurrentConnection;
			PivotGridChoice.Query = query;
			PivotGridChoice.OlapDataLoader = GetDataLoader();
			PivotGridChoice.Initialize();
		}

		private string ParseQuery(string uniqueName)
		{
			return !string.IsNullOrEmpty(uniqueName) ? QueryTemplate.Replace("<%dateChoice%>", uniqueName) : string.Empty;
		}

		private void DatePickerChoice_OnDateSelectionChanged(object sender, DateSelectionChangedEventArgs e)
		{
			InitializePivotGrid(ParseQuery(e.DateUniqueName));
		}

	}
}