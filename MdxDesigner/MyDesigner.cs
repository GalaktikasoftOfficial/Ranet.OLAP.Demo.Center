using System;
using System.Linq;
using System.Windows.Media.Imaging;
using Ranet.Olap.Core.Types;
using Ranet.Olap.UI;
using Ranet.Olap.UI.Controls;
using Ranet.Olap.UI.Controls.PivotGrid.Controls;
using Zaaml.UI.Controls.Menu;
using Zaaml.UI.Windows;

namespace MdxDesigner
{
	public class MyDesigner : PivotMdxDesignerControl
	{
		#region Constructors

		public MyDesigner()
		{
			PivotGrid.IsAutoOpenExportedExcelFile = true;

			PivotGrid.InitializeContextMenu += PivotGrid_InitializeContextMenu;
		}

		#endregion

		#region Public Methods

		public string GetCurrentMdxQuery()
		{
			var query = PivotGrid.TransformedQuery;
			if (query == null)
			{
				MessageWindow.Show("PivotGrid not initialized!", Localization.Warning, MessageWindowButtons.OK);
				return string.Empty;
			}

			return query;
		}

		#endregion

		#region Nested Typed

		private enum MyContextMenuAction
		{
			MyContextMenuCellsItemActions
		}

		#endregion

		#region Private Helpers

		private void PivotGrid_InitializeContextMenu(object sender, CustomContextMenuEventArgs e)
		{
			var menu = e.Menu;

			if (menu == null) return;

			var cellControl = e.GridItem as CellControl;
			var rowControl = e.GridItem as RowMemberControl;
			var columnControl = e.GridItem as ColumnMemberControl;

			var menuItem = new MenuItem
			{
				Header = "My ContextMenu Item to Cell Area for PivotGrid",
				Icon =
					new BitmapImage(
						new Uri("/Ranet.Olap;component/Controls/MdxDesigner/Images/ChangeFilter.png",
							UriKind.Relative)),
				Tag = MyContextMenuAction.MyContextMenuCellsItemActions
			};

			if (cellControl != null)
			{
				menuItem.Click += (o, eventArgs) =>
				{
					MessageWindow.Show(
						"Selected item: " + cellControl.Cell.ColumnMember.UniqueName + " : " + cellControl.Cell.RowMember.UniqueName,
						Localization.MessageBox_Information,
						MessageWindowButtons.OK);
				};
			}

			if (columnControl != null)
			{
				menuItem.Click += (o, eventArgs) =>
				{
					MessageWindow.Show("Selected item: " + columnControl.Member.UniqueName,
						Localization.MessageBox_Information,
						MessageWindowButtons.OK);
				};
			}

			if (rowControl != null)
			{
				menuItem.Click += (o, eventArgs) =>
				{
					MessageWindow.Show("Selected item: " + rowControl.Member.UniqueName,
						Localization.MessageBox_Information,
						MessageWindowButtons.OK);
				};
			}

			CheckAndAdd(menu, menuItem);
		}

		private void CheckAndAdd(ContextMenu menu, MenuItem menuItem)
		{
			var item = menu.ItemCollection.OfType<MenuItem>().FirstOrDefault(i => (MyContextMenuAction) i.Tag == MyContextMenuAction.MyContextMenuCellsItemActions);

			if (item != null)
			{
				var index = menu.ItemCollection.IndexOf(item);
				menu.ItemCollection.Remove(item);
				menu.ItemCollection.Insert(index, menuItem);
			}
			else
			{
				menu.ItemCollection.Add(new MenuSeparator{ Tag = MyContextMenuAction.MyContextMenuCellsItemActions });
				menu.ItemCollection.Add(menuItem);
			}
		}

		protected override void InitializePivotGrid(string query)
		{
			PivotGrid.ColumnsDefaultMemberAction = MemberClickBehaviorTypes.Sort;
			PivotGrid.ColumnsDefaultSortMode = PivotTableSortTypes.SortByContext;
			PivotGrid.DrillDownMode = DrillDownMode.BySingleDimensionHideSelf;

			base.InitializePivotGrid(query);
		}

		#endregion
	}
}