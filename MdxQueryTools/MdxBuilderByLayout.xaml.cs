using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.CommonUtilities.ColorBrush;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Managers;
using Ranet.Olap.Core.Types;
using Ranet.Olap.Core.Wrappers;
using Zaaml.PresentationCore.Utils;
using TabItem = Zaaml.UI.Controls.TabView.TabViewItem;

namespace MdxQueryTools
{
	[Demo]
	public partial class MdxBuilderByLayout
	{
		public MdxBuilderByLayout()
		{
			InitializeComponent();

			ControlName = MdxToolsResource.MdxBuilderByLayout_Name;
			About = MdxToolsResource.MdxBuilderByLayout_Caption;

			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/MdxBuilderByLayout.txt")))
				Description = reader.ReadToEnd();

			UpdateSelection();
		}

		private string _currentMdxQuery = string.Empty;

		private void InitPivotGrid()
		{
			PivotGridControl.Connection = CurrentConnection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			PivotGridControl.Query = _currentMdxQuery;
			PivotGridControl.UseCellConditionsDesigner = true;
			PivotGridControl.Initialize();
		}

		private void PivotGrid_OnLoaded(object s, RoutedEventArgs e)
		{
			InitPivotGrid();
		}

		private QueryBuilderParameters _queryBuilderParameters = null;
		private QueryBuilderParameters FillDefaultQueryBuilderParameters
		{
			get
			{
				if (_queryBuilderParameters == null)
				{
					//create default properties
					_queryBuilderParameters = new QueryBuilderParameters
					{
						CubeName = "",
						SubCube = "",
						MdxDesignerSetting = new MDXDesignerSettingWrapper(),
						CalculatedMembers = new List<CalcMemberInfo>(),
						CalculatedNamedSets = new List<CalculatedNamedSetInfo>(),
						AreaWrappersFilter = new List<AreaItemWrapper>(),
						AreaWrappersColumns = new List<AreaItemWrapper>(),
						AreaWrappersRows = new List<AreaItemWrapper>(),
						AreaWrappersData = new List<AreaItemWrapper>()
					};
				}
				//define/reset parameters
				_queryBuilderParameters.MdxDesignerSetting.HideEmptyColumns = false;
				_queryBuilderParameters.MdxDesignerSetting.HideEmptyRows = false;
				_queryBuilderParameters.MdxDesignerSetting.UseVisualTotals = false;
				_queryBuilderParameters.MdxDesignerSetting.SubsetCount = 0;

				return _queryBuilderParameters;
			}
			set { _queryBuilderParameters = value; }
		}

		private void FillQueryBuilderParameters()
		{
			if (_queryBuilderParameters == null)
			{
				//create default properties
				_queryBuilderParameters = new QueryBuilderParameters
				{
					CubeName = "",
					SubCube = "",
					MdxDesignerSetting = new MDXDesignerSettingWrapper(),
					CalculatedMembers = new List<CalcMemberInfo>(),
					CalculatedNamedSets = new List<CalculatedNamedSetInfo>(),
					AreaWrappersFilter = new List<AreaItemWrapper>(),
					AreaWrappersColumns = new List<AreaItemWrapper>(),
					AreaWrappersRows = new List<AreaItemWrapper>(),
					AreaWrappersData = new List<AreaItemWrapper>()
				};
			}
			//define/reset parameters
			_queryBuilderParameters.MdxDesignerSetting.HideEmptyColumns = false;
			_queryBuilderParameters.MdxDesignerSetting.HideEmptyRows = false;
			_queryBuilderParameters.MdxDesignerSetting.UseVisualTotals = false;
			_queryBuilderParameters.MdxDesignerSetting.SubsetCount = 0;
		}

		private void ClearQueryBuilderParameters()
		{
			if (_queryBuilderParameters == null) return;
			//Clear all properties
			_queryBuilderParameters.CalculatedMembers.Clear();
			_queryBuilderParameters.CalculatedNamedSets.Clear();
			_queryBuilderParameters.AreaWrappersFilter.Clear();
			_queryBuilderParameters.AreaWrappersColumns.Clear();
			_queryBuilderParameters.AreaWrappersRows.Clear();
			_queryBuilderParameters.AreaWrappersData.Clear();
		}

		private void Example_01(int mode)
		{
			switch (mode)
			{
				case 1:
					//use Level example
					//columns
					var itemCol_1 = new Level_AreaItemWrapper
					{
						AreaItemType = AreaItemWrapperType.Level_AreaItemWrapper,
						HierarchyUniqueName = "[Date].[Calendar]",
						UniqueName = "[Date].[Calendar].[Calendar Year]",
					};
					FillDefaultQueryBuilderParameters.AreaWrappersColumns.Add(itemCol_1);
					//rows
					var itemRow_1 = new Level_AreaItemWrapper
					{
						AreaItemType = AreaItemWrapperType.Level_AreaItemWrapper,
						HierarchyUniqueName = "[Product].[Product Categories]",
						UniqueName = "[Product].[Product Categories].[Category]"
					};
					FillDefaultQueryBuilderParameters.AreaWrappersRows.Add(itemRow_1);
					break;
				default:
					//use Hierarchy example
					//columns
					var itemCol1 = new Hierarchy_AreaItemWrapper
					{
						AreaItemType = AreaItemWrapperType.Hierarchy_AreaItemWrapper,
						UniqueName = "[Date].[Calendar]"
					};
					FillDefaultQueryBuilderParameters.AreaWrappersColumns.Add(itemCol1);
					//rows
					var itemRow1 = new Hierarchy_AreaItemWrapper
					{
						AreaItemType = AreaItemWrapperType.Hierarchy_AreaItemWrapper,
						UniqueName = "[Product].[Product Categories]"
					};
					//TODO: incorrectly processed state Selected_With_Children
					itemRow1.CompositeFilter.MembersFilter.IsUsed = true;
					var mcf = new MemberChoiceSettings
					{
						SelectState = SelectStates.Selected_Self,
						Info =
							new MemberData
							{
								UniqueName = "[Product].[Product Categories].[Category].&[4]",
								HierarchyUniqueName = " [Product].[Product Categories]",
								Caption = "Accessories"
							}
					};
					itemRow1.CompositeFilter.MembersFilter.SelectedInfo.Add(mcf);
					mcf = new MemberChoiceSettings
					{
						SelectState = SelectStates.Selected_With_Children,
						Info =
							new MemberData
							{
								UniqueName = "[Product].[Product Categories].[Category].&[1]",
								HierarchyUniqueName = "[Product].[Product Categories]",
								Caption = "Bikes"
							}
					};
					itemRow1.CompositeFilter.MembersFilter.SelectedInfo.Add(mcf);
					// build SET on collection
					itemRow1.CompositeFilter.MembersFilter.BuildFilterSet();

					FillDefaultQueryBuilderParameters.AreaWrappersRows.Add(itemRow1);
					break;
			}
			//data
			var itemData1 = new Measure_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.Measure_AreaItemWrapper,
				UniqueName = "[Measures].[Reseller Sales Amount]"
			};
			FillDefaultQueryBuilderParameters.AreaWrappersData.Add(itemData1);
		}

		/// <summary>
		/// Use more One Measures in Data Area...
		/// </summary>
		private void Example_01_AddMeasure()
		{
			//data
			var itemData1 = new Measure_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.Measure_AreaItemWrapper,
				UniqueName = "[Measures].[Reseller Tax Amount]"
			};
			FillDefaultQueryBuilderParameters.AreaWrappersData.Add(itemData1);
			//if Measures in Data area > 1 - add special element VALUES in COLUMNS or ROWS areas
			var itemCol2 = new Values_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.Values_AreaItemWrapper,
			};
			FillDefaultQueryBuilderParameters.AreaWrappersColumns.Add(itemCol2);
		}

		/// <summary>
		/// Use Filters
		/// </summary>
		/// <param name="mode"></param>
		private void Example_01_AddFilters(int mode)
		{
			//create filter by Level
			var itemFilter1 = new Level_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.Level_AreaItemWrapper,
				UniqueName = "[Date].[Calendar Year].[Calendar Year]",
				HierarchyUniqueName = "[Date].[Calendar Year]",
				CompositeFilter = new Composite_FilterWrapper()
			};
			//set Filter Properties
			itemFilter1.CompositeFilter.MembersFilter.IsUsed = true;
			// two options to choose
			switch (mode)
			{
				case 1:
					//filter through collection of Members
					var mcf = new MemberChoiceSettings
					{
						SelectState = SelectStates.Selected_Self,
						Info =
							new MemberData
							{
								UniqueName = "[Date].[Calendar Year].&[2007]",
								HierarchyUniqueName = "[Date].[Calendar Year]",
								Caption = "2007"
							}
					};
					itemFilter1.CompositeFilter.MembersFilter.SelectedInfo.Add(mcf);
					mcf = new MemberChoiceSettings
					{
						SelectState = SelectStates.Selected_Self,
						Info =
							new MemberData
							{
								UniqueName = "[Date].[Calendar Year].&[2008]",
								HierarchyUniqueName = "[Date].[Calendar Year]",
								Caption = "2008"
							}
					};
					itemFilter1.CompositeFilter.MembersFilter.SelectedInfo.Add(mcf);
					// build SET on collection
					itemFilter1.CompositeFilter.MembersFilter.BuildFilterSet();
					break;
				default:
					// filter through simply SET
					itemFilter1.CompositeFilter.MembersFilter.FilterSet = "{[Date].[Calendar Year].&[2007], [Date].[Calendar Year].&[2008]}";
					break;
			}
			//add filter to Filters Area
			FillDefaultQueryBuilderParameters.AreaWrappersFilter.Add(itemFilter1);
		}

		/// <summary>
		/// Use Custom Calculations
		/// </summary>
		private void Example_02()
		{
			//create Calc Member
			var itemCalcMember1 = new CalcMemberInfo
			{
				Name = "[Measures].[Calculated Member]",
				Expression = "[Measures].[Reseller Sales Amount]/[Measures].[Reseller Order Quantity]",
				NonEmptyBehavior = new List<string> { "Reseller Order Quantity" },
				FormatString = "#,#.00",
				BackColor = new Color(0, 255, 255, 224),
				ForeColor = new Color(0, 165, 42, 42)
			};
			FillDefaultQueryBuilderParameters.CalculatedMembers.Add(itemCalcMember1);
			//add Calc Member to Query template
			//data
			var itemData1 = new Measure_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.CalculatedMember_AreaItemWrapper,
				UniqueName = itemCalcMember1.Name
			};
			FillDefaultQueryBuilderParameters.AreaWrappersData.Add(itemData1);

			//create Named Set
			var itemCustomSet1 = new CalculatedNamedSetInfo
			{
				Name = "[Bike_Set]",
				Hierarchies = new List<string> { "[Product].[Product Categories]" },
				Expression =
					"{[Product].[Product Categories].[Subcategory].&[1], [Product].[Product Categories].[Subcategory].&[2], [Product].[Product Categories].[Subcategory].&[3]}"
			};
			FillDefaultQueryBuilderParameters.CalculatedNamedSets.Add(itemCustomSet1);
			//add Named Set to Query template
			//rows
			var itemRow1 = new CalculatedNamedSet_AreaItemWrapper
			{
				AreaItemType = AreaItemWrapperType.CalculatedNamedSet_AreaItemWrapper,
				Name = itemCustomSet1.Name
			};
			FillDefaultQueryBuilderParameters.AreaWrappersRows.Add(itemRow1);
		}

		private void TabControl_OnSelectionChanged(object sender, EventArgs e)
		{
				if (IsInitialized)
            UpdateSelection();
		}

		private void UpdateSelection()
		{
			var tabControl = TabControlMdxdesigner;
			var tabCurrent = (TabItem)tabControl.SelectedItem;
			if (tabCurrent == null) return;
			var mqb = FillDefaultQueryBuilderParameters;
			ClearQueryBuilderParameters();
			mqb.CubeName = "[Adventure Works]";

			if (tabCurrent.Name == "CubeDefaultQuery")
			{
				txtDescription.Text = "The simplest Mdx Query. The property is set only the Cube name.";
				txtPlainMdx.Text = MdxQueryBuilder.Default.BuildQuery(mqb, null);
				_currentMdxQuery = txtPlainMdx.Text;
			}
			else if (tabCurrent.Name == "SimpleQuery")
			{
				txtDescription.Text = "This Mdx Query demonstrates a Cross Table. Analysis of Sales Resellers Amounts by Product Category and Period.";
				Example_01(0);
				Example_01_AddMeasure();
				txtPlainMdx2.Text = MdxQueryBuilder.Default.BuildQuery(FillDefaultQueryBuilderParameters, null);
				_currentMdxQuery = txtPlainMdx2.Text;
			}
			else if (tabCurrent.Name == "CustomCalcQuery")
			{
				txtDescription.Text = "This Mdx Query demonstrates the use of Custom Calculations: Custom Member and Named Set.";
				Example_02();
				txtPlainMdx3.Text = MdxQueryBuilder.Default.BuildQuery(FillDefaultQueryBuilderParameters, null);
				_currentMdxQuery = txtPlainMdx3.Text;
			}
			else if (tabCurrent.Name == "UseFilters")
			{
				txtDescription.Text = "This query demonstrates the use of filters. Filter is installed for 2007 and 2008 year.";
				Example_01(1);
				Example_01_AddFilters(1);
				txtPlainMdx4.Text = MdxQueryBuilder.Default.BuildQuery(FillDefaultQueryBuilderParameters, null);
				_currentMdxQuery = txtPlainMdx4.Text;
			}

			if (PivotGridControl == null)
				return;

			PivotGridControl.Query = _currentMdxQuery;
			PivotGridControl.Initialize();
		}

		private void btnUpdatePivotGrid_Click(object sender, RoutedEventArgs e)
		{
			PivotGridControl.Initialize();
		}
	}
}
