using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ranet.Demo.Core;
using Ranet.Olap.UI.Controls;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Windows;

namespace MdxDynamic
{
	[Demo(0)]
	public partial class MdxDynamicModule
	{
		public MdxDynamicModule()
		{
			InitializeComponent();
			ControlName = DemoResources.Overview_Name;
			About = DemoResources.Overview_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/Description.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitializeMdxDynamic()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;

			pivotMdxDynamicControl.Connection = CurrentConnection;
			pivotMdxDynamicControl.OlapDataLoader = GetDataLoader();
			pivotMdxDynamicControl.AutoExecuteQuery = true;
			pivotMdxDynamicControl.CubeName = DefaultCubeName;
			pivotMdxDynamicControl.CubeCaption = DefaultCubeName;
			pivotMdxDynamicControl.UseCellConditionsDesigner = true;
			pivotMdxDynamicControl.ShowStateBar = true;
			pivotMdxDynamicControl.ShowActiveFiltersArea = false;
			pivotMdxDynamicControl.IsUpdateable = false;
			pivotMdxDynamicControl.IsAutomaticUpdateScript = true;
			pivotMdxDynamicControl.IsEnabledProcessCube = true;
			//Clear initialize filters
			pivotMdxDynamicControl.ClearMetadataFilterList();
			pivotMdxDynamicControl.ClearDefaultTuples();

			pivotMdxDynamicControl.Initialize();
		}

		/*private void InitMdxDynamicByLayoutButtonClick(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(tbMdxDesignerLayout.Text))
			{
				var layout = XmlSerializationUtility.XmlStr2Obj<DynamicPivotGridLayoutWrapper>(tbMdxDesignerLayout.Text);
				if (layout == null)
				{
					MessageWindow.Show(
						string.Format(Localization.ImportXmlLayout_DeserializeType_Error,
							StorageContentTypes.DynamicPivotGridLayout),
						Localization.MessageBox_Error, MessageWindowButtons.OK, null);
					return;
				}

				pivotMdxDynamicControl.ClearMetadataFilterList();

				pivotMdxDynamicControl.Initialize(tbMdxDesignerLayout.Text);
			}
		}*/

		private void MdxDynamicModule_OnLoaded(object s, RoutedEventArgs e)
		{
			InitializeMdxDynamic();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			InitializeMdxDynamic();
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			InitializeMdxDynamic();
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
					HorizontalAlignment = HorizontalAlignment.Stretch
				}
			}.InitWindow(this);
			mdxQueryWindow.ShowDialog();
		}

		private void ShowLayout_OnClick(object sender, RoutedEventArgs e)
		{
			ShowDialog("Layout", pivotMdxDynamicControl.ExportMdxLayoutInfo(), "Layout is empty.");
		}

		private void ShowQuery_OnClick(object sender, RoutedEventArgs e)
		{
			ShowDialog("Mdx Query", pivotMdxDynamicControl.MDXQuery, "Mdx query is empty.");
		}
	}
}