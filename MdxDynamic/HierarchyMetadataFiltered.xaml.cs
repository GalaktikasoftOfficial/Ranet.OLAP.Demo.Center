using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Types;
using Zaaml.PresentationCore.Utils;

namespace MdxDynamic
{
	[Demo(2)]
	public partial class HierarchyMetadataFiltered
	{

		public HierarchyMetadataFiltered()
		{
			InitializeComponent();
			ControlName = DemoResources.MetadataFiltered_Name;
			About = DemoResources.MetadataFiltered_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/MetadataFilteredDescription.txt")))
				Description = reader.ReadToEnd();

			ConfigureByDefault();
		}

		private void ConfigureByDefault()
		{
			pivotMdxDynamicControl.Connection = CurrentConnection;
			pivotMdxDynamicControl.OlapDataLoader = GetDataLoader();
			pivotMdxDynamicControl.AutoExecuteQuery = true;
			pivotMdxDynamicControl.CubeName = DefaultCubeName;
			pivotMdxDynamicControl.SubCube = "SELECT ({[Product].[Product Categories].[Category].&[1]}, {[Date].[Calendar].[Calendar Year].&[2007], [Date].[Calendar].[Calendar Year].&[2008]}) on COLUMNS FROM [Adventure Works]";
			pivotMdxDynamicControl.Initialize();
		}

		private MetadataFilterList BuildMetadataFilterList(MetadataFilterType filterType)
		{
			var filterMetadata = new MetadataFilterList(filterType);

			var filterMeasureGroup = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.MeasureGroup, MetadataFilterBy.UniqueName), new List<string> { "Internet Sales" });
			filterMetadata.AddMetadataFilter(filterMeasureGroup);

			var filterDimension = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.Dimension, MetadataFilterBy.UniqueName), new List<string> { "[Date]", "[Customer]", "[Product]" });
			filterMetadata.AddMetadataFilter(filterDimension);

			var filterMember = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.Member, MetadataFilterBy.UniqueName), new List<string> { "[Customer].[Customer Geography].[Country].&[United States]", "[Customer].[Customer Geography].[Country].&[Canada]" });
			filterMetadata.AddMetadataFilter(filterMember);

			var filterHierarcy = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.Hierarchy, MetadataFilterBy.UniqueName), new List<string> { "[Promotion].[Promotion Type]", "[Sales Reason].[Sales Reason]" });
			filterMetadata.AddMetadataFilter(filterHierarcy);

			var filterLevel = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.Level, MetadataFilterBy.UniqueName), new List<string> { "[Customer].[Customer Geography].[Country]" });
			filterMetadata.AddMetadataFilter(filterLevel);

			var filterKpi = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.KPI, MetadataFilterBy.UniqueName), new List<string> { "Internet Revenue" });
			filterMetadata.AddMetadataFilter(filterKpi);

			var filterMeasure = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.Measure, MetadataFilterBy.UniqueName), new List<string> { "[Measures].[Internet Order Count]", "[Measures].[Reseller Order Count]" });
			filterMetadata.AddMetadataFilter(filterMeasure);

			var filterSet = new MetadataFilter(
			  new MetadataFilterOptions(OlapTerms.Set, MetadataFilterBy.UniqueName), new List<string> { "Top 50 Customers" });
			filterMetadata.AddMetadataFilter(filterSet);

			return filterMetadata;
		}

		private string ViewFilters()
		{
			var result = string.Empty;
			var filters = new MetadataFilterList();
			filters = BuildMetadataFilterList(MetadataFilterType.Include);
			foreach (var filter in filters.Filters)
			{
				result += filter.Options.Term.ToString() + " | ";
				var info = string.Empty;
				foreach (var item in filter.FilterData)
				{
					info += string.IsNullOrEmpty(info) ? item : ", " + item;
				}
				result += info + "\r\n";
			}
			return result;
		}

		private void ApplyMetadataFilter(MetadataFilterType filterType)
		{
			pivotMdxDynamicControl.MetadataFilterList = BuildMetadataFilterList(filterType);
			pivotMdxDynamicControl.Initialize();
		}

		private void HierarchyMetadataFiltered_OnLoaded(object s, RoutedEventArgs e)
		{
			IncludeButton.IsChecked = true;
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			pivotMdxDynamicControl.ClearMetadataFilterList();
			pivotMdxDynamicControl.Initialize();
		}
		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			pivotMdxDynamicControl.ClearMetadataFilterList();
			pivotMdxDynamicControl.Initialize();
		}

		private void IncludeButton_OnChecked(object sender, RoutedEventArgs e)
		{
			ApplyMetadataFilter(MetadataFilterType.Include);
		}

		private void ExcludeButton_OnChecked(object sender, RoutedEventArgs e)
		{
			ApplyMetadataFilter(MetadataFilterType.Exclude);
		}

		private void NoneButton_OnChecked(object sender, RoutedEventArgs routedEventArgs)
		{
			pivotMdxDynamicControl.ClearMetadataFilterList();
			pivotMdxDynamicControl.Initialize();
		}

		private void ViewFiltersButton_OnClickButton(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(ViewFilters(), "Information", MessageBoxButton.OK);
		}
	}

}