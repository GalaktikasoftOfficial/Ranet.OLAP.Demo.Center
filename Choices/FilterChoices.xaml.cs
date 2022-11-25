using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Helpers;
using Ranet.Olap.Core.Interfaces;
using Ranet.Olap.Core.Managers;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Types;
using Ranet.Olap.Core.Wrappers;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo("Member Choice Widget", 1)]
	public partial class FilterChoices
	{
		private string _queryTemplate = @"SELECT 
NON EMPTY HIERARCHIZE({[Measures].[Internet Sales Amount], [Measures].[Internet Gross Profit]}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
NON EMPTY HIERARCHIZE(HIERARCHIZE([Customer].[Customer Geography].[Customer].Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}) on COLUMNS FROM (SELECT { TopCount([Customer].[Customer Geography].[Customer].Members, 50, [Measures].[Internet Sales Amount])} on COLUMNS
 FROM <%CubeName%>
WHERE ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}))) 
WHERE ({{[Date].[Calendar].[Calendar Year].&[2007]}}, {{[Sales Channel].[Sales Channel].&[Internet]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE";

		private string _hierarchyMdxQueryBuilder = "[Customer].[Customer Geography]";
		private string _levelMdxQueryBuilder = "[Customer].[Customer Geography].[Customer]";

		public FilterChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.MemberFilterChoices_Name;
			About = ChoicesResource.MemberFilterChoices_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/FilterChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}

		/// <summary>
		/// Get Level Properties
		/// </summary>
		private List<LevelPropertyInfo> _levelProperties;
		private List<LevelPropertyInfo> _getLevelProperties
		{
			get { return _levelProperties ?? (_levelProperties = new List<LevelPropertyInfo>()); }
			set { _levelProperties = value; }
		}

		private void ConfigureByDefault()
		{
			string connection = CurrentConnection;
			PivotGridControl.Connection = connection;
			PivotGridControl.OlapDataLoader = GetDataLoader();
			var args = CommandHelper.CreateGetCubeMetadataArgs(connection, DefaultCubeName, MetadataQueryType.GetCubeMetadata);
			var dataLoader = GetDataLoader();
			dataLoader.CreateLoadTask(args).ContinueWith(t =>
			{
				var cube = CubeDefInfo.Deserialize(t.Result);
				if (cube != null)
				{
					_levelProperties = cube.GetHierarchy(_hierarchyMdxQueryBuilder).GetLevel(_levelMdxQueryBuilder).LevelProperties;
				}
				FilterConditionsProperty.Initialize(_getLevelProperties);
				FilterConditionsProperty.RestoreTreeSettings(BuildTreeFilterByProperties(_getLevelProperties));
				PivotGridControl.Query = _queryTemplate.Replace("<%CubeName%>", BuildMdxExpressionFilterByProperties());

				FilterConditionsProperty.ApplyFilter += FilterConditionsPropertyOnApplyFilter;
			}, TaskScheduler.FromCurrentSynchronizationContext());

			PivotGridControl.HideEmptyColumns = PivotGridControl.HideEmptyRows = true;
			PivotGridControl.Initialize();
		}

		/// <summary>
		/// Build filters by Level Properties
		/// </summary>
		/// <param name="levelProperties"></param>
		/// <returns></returns>
		private TreeFilterSettings BuildTreeFilterByProperties(List<LevelPropertyInfo> levelProperties)
		{
			var operationNode = new OperationTreeNode_Wrapper();
			operationNode.Operation = OperationTypes.And;

			var operandNodeCond0 = new OperandTreeNode_Wrapper
			{
				Condition = ConditionTypes.Contains,
				Property = "Caption",
				Value = "Nicole"
			};
			var operandNodeCond1 = new OperandTreeNode_Wrapper
			{
				Condition = ConditionTypes.Contains,
				Property = "Commute Distance",
				Value = "5-10"
			};
			var operandNodeCond2 = new OperandTreeNode_Wrapper
			{
				Condition = ConditionTypes.Contains,
				Property = "Gender",
				Value = "Female"
			};
			var operandNodeCond3 = new OperandTreeNode_Wrapper
			{
				Condition = ConditionTypes.Contains,
				Property = "Yearly Income",
				Value = "80000"
			};
			var operandNodeCond4 = new OperandTreeNode_Wrapper
			{
				Condition = ConditionTypes.Contains,
				Property = "Total Children",
				Value = "4"
			};
			var operandNodeCond5 = new OperandTreeNode_Wrapper
			{
				Condition = ConditionTypes.Contains,
				Property = "Occupation",
				Value = "Prof"
			};
			operationNode.NodeItems = new List<OperationTreeNodeBase> { operandNodeCond0, operandNodeCond1, operandNodeCond2, operandNodeCond3, operandNodeCond4, operandNodeCond5 };
			operationNode.ListLevelPropertyInfo = levelProperties;

			var treeFilters = new TreeFilterSettings();
			treeFilters.Items.Add(operationNode);
			return treeFilters;
		}

		/// <summary>
		/// Build MDX expression
		/// </summary>
		/// <returns></returns>
		private string BuildMdxExpressionFilterByProperties()
		{
			var _mdxExpression = string.Empty;
			var filter = FilterConditionsProperty.GetFilter();
			var dataManager = new QueryProvider
			{
				Cube = DefaultCubeName,
				HierarchyUniqueName = _hierarchyMdxQueryBuilder,
				SubCube = string.Empty,
				UseCalculatedMembers = false
			};
			_mdxExpression = dataManager.BuildSet_QuerySearchCondition(_levelMdxQueryBuilder, filter);
			if (string.IsNullOrEmpty(_mdxExpression))
			{
				_mdxExpression = "(SELECT FROM [Adventure Works])";
			}
			else
			{
				var filterString = string.Concat("{", dataManager.BuildSet_QuerySearchCondition(_levelMdxQueryBuilder, filter), "}");
				_mdxExpression = string.Format("(SELECT {0} on COLUMNS FROM {1})", filterString, DefaultCubeName);
			}
			return _mdxExpression;
		}

		private void FilterConditionsPropertyOnApplyFilter(object sender, EventArgs eventArgs)
		{
			PivotGridControl.Query = _queryTemplate.Replace("<%CubeName%>", BuildMdxExpressionFilterByProperties());
			PivotGridControl.Initialize();
		}

		private void FilterChoices_OnLoaded(object s, RoutedEventArgs e)
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
			FilterConditionsProperty.RestoreTreeSettings(BuildTreeFilterByProperties(_getLevelProperties));
			PivotGridControl.Query = _queryTemplate.Replace("<%CubeName%>", BuildMdxExpressionFilterByProperties());
			PivotGridControl.Initialize();
		}
	}
}