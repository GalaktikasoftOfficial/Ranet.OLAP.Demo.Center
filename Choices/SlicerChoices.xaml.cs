using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ranet.Demo.Core;
using Ranet.Olap.UI.Controls.Slicer;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo(4)]
	public partial class SlicerChoices
	{
		private readonly string _queryTemplate = ChoicesResource.SlicerQuery;

		public SlicerChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.Slicer_Name;
			About = ChoicesResource.Slicer_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/SlicerChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void ConfigureByDefault()
		{
			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();

			//Initialize Slicers
			var configSlicerData = new SlicerOlapConfiguration(null,
			  CurrentConnection,
			  DefaultCubeName,
			  "[Date].[Calendar]",
			  "[Date].[Calendar].[Calendar Year]");
			SlicerDateCalendarYear.OlapConfiguration = configSlicerData;
			SlicerDateCalendarYear.Orientation = Orientation.Horizontal;
			SlicerDateCalendarYear.OlapDataLoader = GetDataLoader();
			SlicerDateCalendarYear.ResultSetChanged += SlicerOnResultSetChanged;
			SlicerDateCalendarYear.Initialize(configSlicerData, "{[Date].[Calendar].[Calendar Year].&[2007], [Date].[Calendar].[Calendar Year].&[2008]}");

			var configSlicerSalesReason = new SlicerOlapConfiguration(null,
			  CurrentConnection,
			  DefaultCubeName,
			  "[Sales Reason].[Sales Reason Type]",
			  "[Sales Reason].[Sales Reason Type].[Sales Reason Type]");
			SlicerSalesReason.OlapConfiguration = configSlicerSalesReason;
			SlicerSalesReason.OlapDataLoader = GetDataLoader();
			SlicerSalesReason.BorderThickness = new Thickness(2);
			SlicerSalesReason.ResultSetChanged += SlicerOnResultSetChanged;
			SlicerSalesReason.Initialize(configSlicerSalesReason, "{[Sales Reason].[Sales Reason Type].&[Marketing], [Sales Reason].[Sales Reason Type].&[Promotion]}");

			var configSlicerProduct = new SlicerOlapConfiguration(null,
			  CurrentConnection,
			  DefaultCubeName,
			  "[Product].[Category]",
			  "[Product].[Category].[Category]");
			SlicerProductCategory.OlapConfiguration = configSlicerProduct;
			SlicerProductCategory.OlapDataLoader = GetDataLoader();
			SlicerProductCategory.ResultSetChanged += SlicerOnResultSetChanged;
			//var subCubeProduct = "SELECT ({[Promotion].[Promotion Type].&[Discontinued Product], [Promotion].[Promotion Type].&[Excess Inventory], [Promotion].[Promotion Type].&[Seasonal Discount]}) ON COLUMNS FROM [Adventure Works]";
			SlicerProductCategory.Initialize(configSlicerProduct, "");

			var configSlicerPromotion = new SlicerOlapConfiguration(null,
			  CurrentConnection,
			  DefaultCubeName,
			  "[Promotion].[Promotion Type]",
			  "[Promotion].[Promotion Type].[Promotion Type]");
			SlicerPromotionType.OlapConfiguration = configSlicerPromotion;
			SlicerPromotionType.LineSize = 2;
			SlicerPromotionType.OlapDataLoader = GetDataLoader();
			SlicerPromotionType.ResultSetChanged += SlicerOnResultSetChanged;
			SlicerPromotionType.Initialize(configSlicerPromotion, "");

			//Replace Mdx Query parameters 
			PivotGridControl.Query = ReplaceQueryParameters();

			PivotGridControl.Initialize();
		}

		private string ReplaceQueryParameters()
		{
			var query = _queryTemplate;

			var result = string.IsNullOrEmpty(SlicerDateCalendarYear.ResultSet) ? "[Date].[Calendar].[All Periods]" : SlicerDateCalendarYear.ResultSet;
			query = query.Replace("<%DateCalendarYear%>", result);

			result = string.IsNullOrEmpty(SlicerSalesReason.ResultSet) ? "[Sales Reason].[Sales Reason Type].[All Sales Reasons]" : SlicerSalesReason.ResultSet;
			query = query.Replace("<%SalesReason%>", result);

			result = string.IsNullOrEmpty(SlicerProductCategory.ResultSet) ? "[Product].[Category].[All Products]" : SlicerProductCategory.ResultSet;
			query = query.Replace("<%ProductCategory%>", result);

			result = string.IsNullOrEmpty(SlicerPromotionType.ResultSet) ? "[Promotion].[Promotion Type].[All Promotions]" : SlicerPromotionType.ResultSet;
			query = query.Replace("<%PromotionType%>", result);

			return query;
		}

		private void SlicerOnResultSetChanged(object sender, EventArgs eventArgs)
		{
			PivotGridControl.Query = ReplaceQueryParameters();
			PivotGridControl.Initialize(true);
		}

		private void SlicerChoices_OnLoaded(object s, RoutedEventArgs e)
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