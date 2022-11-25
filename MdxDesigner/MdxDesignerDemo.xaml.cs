using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ranet.Demo.Core;
using Ranet.Olap.Core.CommonUtilities.ColorBrush;
using Ranet.Olap.Core.CommonUtilities.Serialization;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Types;
using Ranet.Olap.Core.Wrappers;
using Ranet.Olap.UI.Controls;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Windows;

namespace MdxDesigner
{
	[Demo]
	public partial class MdxDesignerDemo
	{
		public MdxDesignerDemo()
		{
			InitializeComponent();

			ControlName = MdxDesignerResource.Overview_Name;
			About = MdxDesignerResource.Overview_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/Description.txt")))
				Description = reader.ReadToEnd();
		}

		private void DefaultSettingsPivotMdxDesigner()
		{
			pivotMdxDesignerControl.Connection = CurrentConnection;
			pivotMdxDesignerControl.OlapDataLoader = GetDataLoader();
			pivotMdxDesignerControl.CubeName = DefaultCubeName;
			pivotMdxDesignerControl.CanSelectCube = true;
			pivotMdxDesignerControl.AutoExecuteQuery = true;
			pivotMdxDesignerControl.ShowHints = false;
			pivotMdxDesignerControl.UseCellConditionsDesigner = true;
			pivotMdxDesignerControl.IsUpdateable = false;
			pivotMdxDesignerControl.ShowStateBar = true;
			pivotMdxDesignerControl.IsEnabledProcessCube = true;
			pivotMdxDesignerControl.SubCube = "";
		}

		private void InitializePivotMdxDesignerControl()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			DefaultSettingsPivotMdxDesigner();

			//Standard mode
			pivotMdxDesignerControl.Initialize();
		}

		private List<List<ShortMemberInfo>> GreateTuple()
		{
			var initByDefaultTuples = new List<List<ShortMemberInfo>>();
			var tuples = new List<ShortMemberInfo>();

			var smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Product].[Product Categories]",
				UniqueName = "[Product].[Product Categories].[Category].&[1]"
			};
			tuples.Add(smi);
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Date].[Calendar]",
				UniqueName = "[Date].[Calendar].[Calendar Year].&[2007]"
			};
			tuples.Add(smi);
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Measures]",
				UniqueName = "[Measures].[Internet Sales Amount]"
			};
			tuples.Add(smi);

			initByDefaultTuples.Add(tuples);

			return initByDefaultTuples;
		}

		private void MdxDesignerDemo_OnLoaded(object s, RoutedEventArgs e)
		{
			InitializePivotMdxDesignerControl();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;

			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			InitializePivotMdxDesignerControl();
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			InitializePivotMdxDesignerControl();
		}

		private void ShowMdxQuery_OnClick(object sender, RoutedEventArgs e)
		{
			ShowDialog("Mdx Query", pivotMdxDesignerControl.GetCurrentMdxQuery(), "Mdx query is empty.");
		}

		private void ShowUpdateScript_OnClick(object sender, RoutedEventArgs e)
		{
			ShowDialog("Update Script", pivotMdxDesignerControl.UpdateScript, "Update Script is empty.");
		}

		private void ShowLayout_OnClick(object sender, RoutedEventArgs e)
		{
			if (pivotMdxDesignerControl.FilterAreaContainer.GetItemWrappers().Any()
			    | pivotMdxDesignerControl.ColumnsAreaContainer.GetItemWrappers().Any()
			    | pivotMdxDesignerControl.RowsAreaContainer.GetItemWrappers().Any()
			    | pivotMdxDesignerControl.DataAreaContainer.GetItemWrappers().Any()
				)
			{
				ShowDialog("Report Layout", pivotMdxDesignerControl.ExportMdxLayoutInfo(), "");
			}
			else
			{
				MessageWindow.Show("Report Layout is empty.", Ranet.Olap.UI.Localization.MessageBox_Information, MessageWindowButtons.OK);
			}
		}

		private void ShowDialog(string header, string text, string messageError)
		{
			if (string.IsNullOrEmpty(text))
			{
				MessageWindow.Show(messageError, Ranet.Olap.UI.Localization.MessageBox_Information, MessageWindowButtons.OK);
				return;
			}
			var mdxQueryWindow = new FloatingDialog()
			{
				Title = header,
				Buttons = FloatingDialogButtons.Ok,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				MinHeight = 150,
				MinWidth = 400,
				Height = 400,
				Width = 600,
				Content = new TextBox
				{
					Text = text,
					TextWrapping = TextWrapping.Wrap,
					IsReadOnly = true,
					VerticalAlignment = VerticalAlignment.Stretch,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
					HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
				}
			}.InitWindow(this);
			mdxQueryWindow.ShowDialog();
		}

		/// <summary>
		///     Use Custom Calculations
		/// </summary>
		private List<CalcMemberInfo> CreateCustomMembers()
		{
			var customMember = new List<CalcMemberInfo>();
			//create Calc Member
			var itemCalcMember1 = new CalcMemberInfo
			{
				Name = "[Measures].[Calculated Member 1]",
				Caption = "Expample Calculated Member",
				SolveOrder = 1,
				ParentHierarchyUniqueName = "[Measures]",
				DisplayFolder = "My Folder",
				Expression = "[Measures].[Reseller Sales Amount]/[Measures].[Reseller Order Quantity]",
				NonEmptyBehavior = new List<string> {"Reseller Order Quantity"},
				FormatString = "#,#.00",
				BackColor = new Color(0, 255, 255, 224),
				ForeColor = new Color(0, 165, 42, 42)
			};
			customMember.Add(itemCalcMember1);
			return customMember;
		}

		private List<CalculatedNamedSetInfo> CreateCustomSet()
		{
			var customSet = new List<CalculatedNamedSetInfo>();
			//create Named Set
			var itemCustomSet1 = new CalculatedNamedSetInfo
			{
				Name = "[Bike_Set]",
				Hierarchies = new List<string> {"[Product].[Product Categories]"},
				Expression =
					"{[Product].[Product Categories].[Subcategory].&[1], [Product].[Product Categories].[Subcategory].&[2], [Product].[Product Categories].[Subcategory].&[3]}"
			};
			customSet.Add(itemCustomSet1);
			return customSet;
		}

		private void InitBased_XmlFileLayout_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			DefaultSettingsPivotMdxDesigner();

			pivotMdxDesignerControl.Initialize(MdxDesignerResource.AW_Sparkline_2007year);
		}

		private void InitBased_ObjectWrapperLayout_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			DefaultSettingsPivotMdxDesigner();

			var layoutXmlFile = MdxDesignerResource.AW_Sparkline_2007year;
			if (string.IsNullOrEmpty(MdxDesignerResource.AW_Sparkline_2007year))
				throw new ArgumentNullException("layoutXmlFile");

			var layoutObject = XmlSerializationUtility.XmlStr2Obj<MdxLayoutWrapper>(layoutXmlFile);
			if (layoutObject == null)
				throw new ArgumentException(string.Format(Ranet.Olap.UI.Localization.ImportXmlLayout_DeserializeType_Error,
					StorageContentTypes.MdxDesignerLayout));

			//Transform loaded report layout...
			//Add new Custom Calculations
			layoutObject.CalculatedMembers.AddRange(CreateCustomMembers());
			layoutObject.CalculatedNamedSets.AddRange(CreateCustomSet());
			//Modify fields of report 
			layoutObject.Data.Clear();
			layoutObject.Rows.Clear();
			layoutObject.Columns.Clear();
			var data1 = new CalculatedMember_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.CalculatedMember_AreaItemWrapper,
				Caption = CreateCustomMembers()[0].Caption,
				Name = CreateCustomMembers()[0].Name
			};
			layoutObject.Data.Add(data1);
			var row1 = new CalculatedNamedSet_AreaItemWrapper(CreateCustomSet()[0])
			{
				AreaItemType = AreaItemWrapperType.CalculatedNamedSet_AreaItemWrapper
			};
			layoutObject.Rows.Add(row1);

			pivotMdxDesignerControl.CubeName = layoutObject.CubeName;

			pivotMdxDesignerControl.Initialize(layoutObject);
		}

		private void InitBased_MetaAndDataFilter_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			DefaultSettingsPivotMdxDesigner();

			//Initialization using a Subcube that restricts the available data
			pivotMdxDesignerControl.SubCube =
				"SELECT ({[Product].[Product Categories].[Category].&[1]}, {[Date].[Calendar].[Calendar Year].&[2007], [Date].[Calendar].[Calendar Year].&[2008]}) on COLUMNS FROM [Adventure Works]";

			pivotMdxDesignerControl.Initialize();
		}

		/// <summary>
		///     Initialization using a filters, specified in the Tuple
		/// </summary>
		private void InitBased_TubleFilter_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			DefaultSettingsPivotMdxDesigner();

			pivotMdxDesignerControl.Initialize(GreateTuple());
		}
	}
}