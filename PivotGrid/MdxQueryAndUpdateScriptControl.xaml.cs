using System;
using System.Windows.Controls;
using Ranet.Olap.Core.Interfaces;
using TabControl = Zaaml.UI.Controls.TabView.TabViewControl;
using TabItem = Zaaml.UI.Controls.TabView.TabViewItem;

namespace PivotGrid
{
	public partial class MdxQueryAndUpdateScriptControl : UserControl
	{
		public MdxQueryAndUpdateScriptControl(string connection, IDataLoader loader)
		{
			InitializeComponent();
			Designer.Connection = connection;
			Designer.OlapDataLoader = loader;
			Designer.CanSelectCube = true;
			Designer.IsUpdateable = false;

			Designer.PivotGrid.ShowFooterArea = false;
			Designer.PivotGrid.ShowHeaderArea = false;
			Designer.PivotGrid.ShowHints = false;
			Designer.PivotGrid.ShowWritebackSettingMenuItem = false;

			Designer.Initialize();
		}

		private void TabControl_OnSelectionChanged(object sender, EventArgs e)
		{
			var tabControl = (TabControl)sender;
			var tabCurrent = (TabItem)tabControl.SelectedItem;
			if (tabCurrent == null) return;
			if (tabCurrent.Name == "TabItemScript")
			{
				Script.Text = Designer.UpdateScript;
			}
		}
	}
}