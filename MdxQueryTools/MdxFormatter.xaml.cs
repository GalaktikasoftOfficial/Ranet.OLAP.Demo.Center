using System;
using System.IO;
using System.Text;
using System.Windows;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace MdxQueryTools
{
	[Demo]
	public partial class MdxFormatter
	{
		public MdxFormatter()
		{
			InitializeComponent();
			ControlName = MdxToolsResource.MdxFormatter_Name;
			About = MdxToolsResource.MdxFormatter_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/Description.txt")))
				Description = reader.ReadToEnd();

			txtPlainMdx.Text = @"with member [TestIIF] as iif(1=1,'equal','notequal')
            SELECT HIERARCHIZE ( DRILLDOWNMEMBER ( FILTER ( DRILLDOWNMEMBER ( FILTER ( DRILLDOWNMEMBER ( { [Date].[Calendar].Levels ( 0 ).Members } , [Date].[Calendar].[All Periods] ) , ( ( ( not ( ( [Date].[Calendar].CURRENTMEMBER is [Date].[Calendar].[All Periods] ) AND ( [Date].[Calendar].[All Periods].Children.Count <> 0 ) ) AND not ( IsSibling ( [Date].[Calendar].CURRENTMEMBER , [Date].[Calendar].[All Periods] ) AND not ( [Date].[Calendar].CURRENTMEMBER is [Date].[Calendar].[All Periods] ) ) ) AND not IsAncestor ( [Date].[Calendar].CURRENTMEMBER , [Date].[Calendar].[All Periods] ) ) AND ( IsAncestor ( [Date].[Calendar].[All Periods] , [Date].[Calendar].CURRENTMEMBER ) OR ( [Date].[Calendar].CURRENTMEMBER is [Date].[Calendar].[All Periods] ) ) ) ) , [Date].[Calendar].[Calendar Year].&[2002] ) , ( ( ( not ( ( [Date].[Calendar].CURRENTMEMBER is [Date].[Calendar].[Calendar Year].&[2002] ) AND ( [Date].[Calendar].[Calendar Year].&[2002].Children.Count <> 0 ) ) AND not ( IsSibling ( [Date].[Calendar].CURRENTMEMBER , [Date].[Calendar].[Calendar Year].&[2002] ) AND not ( [Date].[Calendar].CURRENTMEMBER is [Date].[Calendar].[Calendar Year].&[2002] ) ) ) AND not IsAncestor ( [Date].[Calendar].CURRENTMEMBER , [Date].[Calendar].[Calendar Year].&[2002] ) ) AND ( IsAncestor ( [Date].[Calendar].[Calendar Year].&[2002] , [Date].[Calendar].CURRENTMEMBER ) OR ( [Date].[Calendar].CURRENTMEMBER is [Date].[Calendar].[Calendar Year].&[2002] ) ) ) ) , [Date].[Calendar].[Calendar Semester].&[2002]&[1] ) ) DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 ON 0, HIERARCHIZE ( DRILLUPMEMBER ( FILTER ( DRILLDOWNMEMBER ( { [Product].[Product Categories].Levels ( 0 ).Members } , [Product].[Product Categories].[All Products] ) , ( ( ( not ( ( [Product].[Product Categories].CURRENTMEMBER is [Product].[Product Categories].[All Products] ) AND ( [Product].[Product Categories].[All Products].Children.Count <> 0 ) ) AND not ( IsSibling ( [Product].[Product Categories].CURRENTMEMBER , [Product].[Product Categories].[All Products] ) AND not ( [Product].[Product Categories].CURRENTMEMBER is [Product].[Product Categories].[All Products] ) ) ) AND not IsAncestor ( [Product].[Product Categories].CURRENTMEMBER , [Product].[Product Categories].[All Products] ) ) AND ( IsAncestor ( [Product].[Product Categories].[All Products] , [Product].[Product Categories].CURRENTMEMBER ) OR ( [Product].[Product Categories].CURRENTMEMBER is [Product].[Product Categories].[All Products] ) ) ) ) , [Product].[Product Categories].[Category].&[4] ) ) DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 ON 1FROM [Adventure Works]WHERE { [Measures].[Sales Amount] } CELL PROPERTIES BACK_COLOR , CELL_ORDINAL , FORE_COLOR , FONT_NAME , FONT_SIZE , FONT_FLAGS , FORMAT_STRING , VALUE , FORMATTED_VALUE , UPDATEABLE
            ";
		}
		string FormatMdx(string mdx, out string errors)
		{
			using (var dp = Ranet.Olap.Mdx.Compiler.MdxDomProvider.CreateProvider())
			{
				var mdxObj = dp.ParseMdx(mdx);
				var sb = new StringBuilder();
				try
				{
					var op = new Ranet.Olap.Mdx.Compiler.MdxGeneratorOptions();
					op.EvaluateConstantExpressions = (bool)ckbIIF_Subst.IsChecked;
					dp.GenerateMdxFromDom(mdxObj, sb, op);
					errors = string.Empty;
				}
				catch (Exception E)
				{
					errors = E.Message + @"";
				}
				errors += dp.Errors.ToString();
				return sb.ToString();
			}
		}

		private void btnParseAndGen_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string e1;
				string mdx1 = FormatMdx(txtPlainMdx.Text, out e1);
				if (string.IsNullOrEmpty(e1))
				{
					string e2;
					string mdx2 = FormatMdx(mdx1, out e2);
					if (!string.IsNullOrEmpty(e2) || (mdx1 != mdx2))
						txtParsedMdx.Text = @"Error: re-formatting lead to different results. First formatting:"
						+ mdx1 + @"The second formatting:"
						+ e2 + mdx2;
					else
						txtParsedMdx.Text = mdx1;
				}
				else
				{
					txtParsedMdx.Text = e1 + mdx1;
				}
			}
			catch (Exception exc)
			{
				txtParsedMdx.Text = exc.ToString();
			}
		}

		private void txtPlainMdx_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{

		}

		private void LayoutRoot_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{

		}
	}
}
