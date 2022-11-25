using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.CommonUtilities.ColorBrush;
using Ranet.Olap.Core.Types;
using Zaaml.PresentationCore.Utils;

namespace PivotGrid
{
  [Demo(2)]
  public partial class PivotGridStyle
  {
    private string mdxQueryCellStyle = @"WITH 
SET [Set All Month] AS {Descendants ([Date].[Calendar].[All Periods], [Date].[Calendar].[Month])} 
MEMBER [Measures].[Internet Freight Cost 07-08] AS ([Measures].[Internet Freight Cost], [Date].[Calendar Year].&[2007])-([Measures].[Internet Freight Cost], [Date].[Calendar Year].&[2008]) 
MEMBER [Measures].[Internet Sales Amount Dynamic] AS 
IIF(NonEmpty([Set All Month], ([Date].[Calendar].CurrentMember,[Measures].[Internet Sales Amount])).Count > 0, Generate([Set All Month], ([Date].[Calendar].CurrentMember,[Measures].[Internet Sales Amount]), "" | "") ,NULL), FORMAT_STRING = ""Sparkline"" 
MEMBER [Measures].[Internet Gross Profit Dynamic] AS 
IIF(NonEmpty([Set All Month], ([Date].[Calendar].CurrentMember,[Measures].[Internet Gross Profit])).Count > 0, Generate([Set All Month], ([Date].[Calendar].CurrentMember,[Measures].[Internet Gross Profit]), "" | "") ,NULL), FORMAT_STRING = ""Sparkline"" 
SELECT 
NON EMPTY HIERARCHIZE({[Measures].[Internet Sales Amount], [Measures].[Internet Gross Profit], [Measures].[Internet Tax Amount], [Measures].[Internet Freight Cost 07-08], [Measures].[Internet Sales Amount Dynamic], [Measures].[Internet Gross Profit Dynamic]}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 0, 
NON EMPTY HIERARCHIZE(HIERARCHIZE([Customer].[Customer Geography].[Country].Members)) DIMENSION PROPERTIES PARENT_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, CUSTOM_ROLLUP, UNARY_OPERATOR, KEY0 ON 1 
FROM 
(SELECT ({{[Date].[Calendar].[Calendar Year].&[2007]}}) on COLUMNS FROM [Adventure Works]) 
WHERE ({{[Date].[Calendar].[Calendar Year].&[2007]}}) 
CELL PROPERTIES BACK_COLOR, CELL_ORDINAL, FORE_COLOR, FONT_NAME, FONT_SIZE, FONT_FLAGS, FORMAT_STRING, VALUE, FORMATTED_VALUE, UPDATEABLE, ACTION_TYPE";

    public PivotGridStyle()
    {
      InitializeComponent();
      ControlName = PivotGridResource.PivotGridWithStyle_Name;
      About = PivotGridResource.PivotGridWithStyle_Caption;
      using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/GridStylesDescription.txt")))
        Description = reader.ReadToEnd();
    }

    private void InitPivotGrid()
    {
      PivotGridControl.Connection = CurrentConnection;
      PivotGridControl.OlapDataLoader = GetDataLoader();
      PivotGridControl.Query = mdxQueryCellStyle;
      PivotGridControl.UseCellConditionsDesigner = true;

      PivotGridControl.CustomCellsConditions = new List<CellConditionsDescriptorBase> 
      { CellAppearanceStyleGenerate("[Measures].[Internet Tax Amount]"), 
        ImageStyleGenerate("[Measures].[Internet Sales Amount]"), 
        DatabarStyleGenerate("[Measures].[Internet Gross Profit]", Colors.LightSkyBlue, Colors.LightSkyBlue, Colors.LightSkyBlue), 
        DatabarStyleGenerate("[Measures].[Internet Freight Cost 07-08]", Colors.Red, Colors.LightGreen, Colors.LightSlateGray),
        SpaklineStyleGenerate("[Measures].[Internet Sales Amount Dynamic]") };

      PivotGridControl.Initialize();
    }

    private void PivotGridStyle_OnLoaded(object s, RoutedEventArgs e)
    {
      InitPivotGrid();

      CurrentConnectionChanged -= OnCurrentConnectionChanged;
      DefaultCubeNameChanged -= OnDefaultCubeNameChanged;

      CurrentConnectionChanged += OnCurrentConnectionChanged;
      DefaultCubeNameChanged += OnDefaultCubeNameChanged;
    }

    private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
    {
      PivotGridStyle_OnLoaded(null, null);
    }

    private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
    {
      PivotGridStyle_OnLoaded(null, null);
    }

    private CellConditionsDescriptorBase CellAppearanceStyleGenerate(string measure)
    {
      var styleAppearance = new CellConditionsDescriptorBase(measure);
      var cellStyleCond1 = new CellAppearanceObject
      {
        Options = { UseBackColor = true, UseForeColor = true },
        BackColor = Colors.LightGreen,
        ForeColor = Colors.Red
      };
      var condValue = new CellConditionBase(CellConditionType.Greater, 200000, 0.0, cellStyleCond1);
      styleAppearance.Conditions.Add(condValue);

      var cellStyleCond2 = new CellAppearanceObject
      {
        Options = { UseBackColor = true },
        BackColor = Colors.LightYellow
      };
      var condValue2 = new CellConditionBase(CellConditionType.Between, 100000, 200000, cellStyleCond2);
      styleAppearance.Conditions.Add(condValue2);

      var cellStyleCond3 = new CellAppearanceObject
      {
        Options = { UseBackColor = true },
        BackColor = Colors.Red
      };
      var condValue3 = new CellConditionBase(CellConditionType.Less, 100000, 0.0, cellStyleCond3);
      styleAppearance.Conditions.Add(condValue3);
      return styleAppearance;
    }

    private CellConditionsDescriptorBase DatabarStyleGenerate(string measure, Color startColor, Color endColor, Color medianColor)
    {
      var styleDatabar = new CellConditionsDescriptorBase(measure);
      var cellDataBarCond = new CellAppearanceObject
      {
        Options = { UseProgressBar = true },
        ProgressBarOptions = new CellProgressBarOptions
        {
          StartColor = startColor,
          EndColor = endColor,
          IntermediateColor = medianColor,
          UseGradient = true,
          MaxValue = 0.0,
          MaxValueMode = CellProgressBarMode.Auto,
          MinValue = 0.0,
          MinValueMode = CellProgressBarMode.Auto
        }
      };
      var cond = new CellConditionBase(CellConditionType.None, 0.0, 0.0, cellDataBarCond);
      styleDatabar.Conditions.Add(cond);
      return styleDatabar;
    }

    private CellConditionsDescriptorBase ImageStyleGenerate(string measure)
    {
      var styleImage = new CellConditionsDescriptorBase(measure);
      var cellStyleCond1 = new CellAppearanceObject
      {
        Options = { UseImage = true },
        CustomImageUri = "/Ranet.AgOlap;component/controls/images/olap/kpi/trend/arrowcolor_1.png"
      };
      var condValue = new CellConditionBase(CellConditionType.Greater, 2500000, 0.0, cellStyleCond1);
      styleImage.Conditions.Add(condValue);

      var cellStyleCond2 = new CellAppearanceObject
      {
        Options = { UseImage = true },
        CustomImageUri = "/Ranet.AgOlap;component/controls/images/olap/kpi/trend/arrowcolor_0.png"
      };
      var condValue2 = new CellConditionBase(CellConditionType.Between, 1000000, 2000000, cellStyleCond2);
      styleImage.Conditions.Add(condValue2);

      var cellStyleCond3 = new CellAppearanceObject
      {
        Options = { UseImage = true },
        CustomImageUri = "/Ranet.AgOlap;component/controls/images/olap/kpi/trend/arrowcolor_-1.png"
      };
      var condValue3 = new CellConditionBase(CellConditionType.Less, 1000000, 0.0, cellStyleCond3);
      styleImage.Conditions.Add(condValue3);
      return styleImage;
    }

    private CellConditionsDescriptorBase SpaklineStyleGenerate(string measure)
    {
      var styleSparkline = new CellConditionsDescriptorBase(measure);
      var cellSparkline = new CellAppearanceObject
      {
        SparkLineOptions = new SparkLineCellStyleOptions
        {
          Type = SparkLineType.Bar,
          ShowLine = true,
          ShowMarkers = true,
          ShowPoints = true,
          ShowMaxMarker = true
        }
      };
      var cond = new CellConditionBase(CellConditionType.None, 0.0, 0.0, cellSparkline);
      styleSparkline.Conditions.Add(cond);
      return styleSparkline;
    }

  }
}