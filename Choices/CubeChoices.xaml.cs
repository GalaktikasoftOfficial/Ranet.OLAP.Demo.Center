using System;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace Choices
{
	[Demo("Metadata Choice Widget", 0)]
	public partial class CubeChoices
	{
		public CubeChoices()
		{
			InitializeComponent();
			ControlName = ChoicesResource.CubeChoices_Name;
			About = ChoicesResource.CubeChoices_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/CubeChoicesDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void Update()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			ComboCubeChoiceControl.Connection = CurrentConnection;
			ComboCubeChoiceControl.Loader = GetDataLoader();
			ComboCubeChoiceControl.ForceReloadCubes();

			PopUpCubeChoiceControl.Connection = CurrentConnection;
			PopUpCubeChoiceControl.Loader = GetDataLoader();
			PopUpCubeChoiceControl.NeedReload = true;
		}

		private void CubeChoices_OnLoaded(object s, RoutedEventArgs e)
		{
			Update();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			Update();
		}
	}
}