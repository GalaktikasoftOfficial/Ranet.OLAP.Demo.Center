using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.UI.Controls.Choices;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo("Metadata Choice Widget", 0)]
	public partial class PopUpChoices
	{
		public PopUpChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.PopupChoices_Name;
			About = ChoicesResource.PopupChoices_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/PopUpChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitPopUpChoiceControl()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			//Cube choice - Popup
			PopUpCubeChoiceControl.Connection = CurrentConnection;
			PopUpCubeChoiceControl.Loader = GetDataLoader();
			PopUpCubeChoiceControl.NeedReload = true;
			//Dimension choice - Popup
			PopUpDimensionChoiceControl.Connection = CurrentConnection;
			PopUpDimensionChoiceControl.NeedReload = true;
			//Hierarchy choice - Popup
			PopUpHierarchyChoiceControl.Connection = CurrentConnection;
			PopUpHierarchyChoiceControl.NeedReload = true;
			//Level choice - Popup
			PopUpLevelChoiceControl.Connection = CurrentConnection;
			PopUpLevelChoiceControl.NeedReload = true;
		}

		private void PopUpCubeChoiceControl_OnCubeSelected(object sender, CubeEventArgs e)
		{
			PopUpDimensionChoiceControl.CubeName = e.CubeName;
			PopUpDimensionChoiceControl.NeedReload = true;
			PopUpHierarchyChoiceControl.CubeName = e.CubeName;
			PopUpHierarchyChoiceControl.NeedReload = true;
			PopUpLevelChoiceControl.ACubeName = e.CubeName;
			PopUpLevelChoiceControl.NeedReload = true;
		}

		private void PopUpDimensionChoiceControl_OnDimensionSelected(object sender, DimensionEventArgs e)
		{
			PopUpHierarchyChoiceControl.DimensionUniqueName = e.DimensionInfo.UniqueName;
			PopUpHierarchyChoiceControl.NeedReload = true;
			PopUpLevelChoiceControl.ADimensionUniqueName = e.DimensionInfo.UniqueName;
			PopUpLevelChoiceControl.NeedReload = true;
		}

		private void PopUpHierarchyChoiceControl_OnHierarchySelected(object sender, HierarchyEventArgs e)
		{
			PopUpLevelChoiceControl.AHierarchyUniqueName = e.HierarchyInfo.UniqueName;
			PopUpLevelChoiceControl.NeedReload = true;
		}

		private void PopUpChoices_OnLoaded(object s, RoutedEventArgs e)
		{
			InitPopUpChoiceControl();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			InitPopUpChoiceControl();
		}
	}
}