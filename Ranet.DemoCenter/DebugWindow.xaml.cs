using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Wrappers;
using Ranet.Olap.UI.Controls;

namespace Ranet.DemoCenter
{
	/// <summary>
	///   Interaction logic for DebugWindow.xaml
	/// </summary>
	public partial class DebugWindow 
	{
		#region Ctors

		public DebugWindow()
		{
			InitializeComponent();

			var connectionString = "Provider=MSOLAP.4;DATA SOURCE=localhost;CATALOG=AdventureWorks;";
			var cubeName = "Finance";

			//dataLoader.CreateLoadCubeMetadataTask(connectionString, cubeName).ContinueWith(t => OnCubeInfoLoaded(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
			//measureChoiceDropDown.IsEnabled = false;

			designer.Connection = connectionString;
			designer.CubeName = cubeName;
			designer.AutoExecuteQuery = true;
			//designer.CanSelectCube = true;

			designer.Initialize();
			//designer.Initialize(DebugResource.MdxLayout);
		}

		private void OnCubeInfoLoaded(CubeDefInfo result)
		{
			//measureChoiceDropDown.CubeInfo = result;
			//measureChoiceDropDown.IsEnabled = true;
		}

		#endregion
	}

	public class CustomDynamicPivotGrid : DynamicPivotGridControl
	{
		public CustomDynamicPivotGrid()
		{
			DefaultDesignerSetting = new MDXDesignerSettingWrapper
			{
				LoadQueryState = true,
				HideEmptyColumns = false,
				HideEmptyRows = false
			};
		}
	}
}