using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Helpers;
using Ranet.Olap.UI.Controls.Choices;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo("Member Choice Widget", 2)]
	public partial class MemberChoicesSubcube
	{
		public MemberChoicesSubcube()
		{
			InitializeComponent();
			ControlName = ChoicesResource.MemberChoicesSubcube_Name;
			About = ChoicesResource.MemberChoicesSubcube_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/MemberChoicesSubcubeDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void InitPopUpMemberControl()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) 
          return;

			PopUpMemberChoiceControl.Connection = CurrentConnection;
			PopUpMemberChoiceControlSubCube.Connection = CurrentConnection;
			PopUpMemberChoiceControlInitBySet.Connection = CurrentConnection;
		}

		private void PopUpMemberChoiceControl_OnApplySelectionEvent(object sender, EventArgs e)
		{
			var mcc = sender as MemberChoicePopUp;
			if (mcc == null)
				return;
			RtbUpMemberChoiceControlResult.Text = mcc.ChoiceControl.SelectedSet;
			PopUpMemberChoiceControlSubCube.ASubCube = !String.IsNullOrEmpty(RtbUpMemberChoiceControlResult.Text)
				? String.Format("SELECT ({0}) on COLUMNS FROM {1}", RtbUpMemberChoiceControlResult.Text,
					OlapHelper.CheckSquareBrackets(PopUpMemberChoiceControlSubCube.ACubeName))
				: string.Empty;

			PopUpMemberChoiceControlInitBySet.SelectedSet = mcc.ChoiceControl.SelectedSet;
			PopUpMemberChoiceControlInitBySet.SelectedInfo = mcc.ChoiceControl.SelectedInfo;
		}

		private void ConfigureByDefault()
		{
			string levelUniqueName = "[Date].[Calendar].[Date]";
			string hierarchyUniqueName = "[Date].[Calendar]";

			InitPopUpMemberControl();

			PopUpMemberChoiceControl.ACubeName = DefaultCubeName;
			PopUpMemberChoiceControl.AHierarchyName = hierarchyUniqueName;
			PopUpMemberChoiceControl.AStartLevelUniqueName = levelUniqueName;

			PopUpMemberChoiceControlSubCube.ACubeName = DefaultCubeName;
			PopUpMemberChoiceControlSubCube.AHierarchyName = hierarchyUniqueName;
			PopUpMemberChoiceControlSubCube.AStartLevelUniqueName = levelUniqueName;

			RtbUpMemberChoiceControlResult.Text = string.Empty;

			PopUpMemberChoiceControlInitBySet.ACubeName = DefaultCubeName;
			PopUpMemberChoiceControlInitBySet.AHierarchyName = hierarchyUniqueName;
		}

		private void MemberPopUpChoices_OnLoaded(object s, RoutedEventArgs e)
		{
			InitPopUpMemberControl();
			ConfigureByDefault();
			CurrentConnectionChanged -= ConnectionChanged;
			DefaultCubeNameChanged -= ConnectionChanged;

			CurrentConnectionChanged += ConnectionChanged;
			DefaultCubeNameChanged += ConnectionChanged;
		}

		private void ConnectionChanged(object sender, EventArgs args)
		{
			ConfigureByDefault();
		}
	}
}