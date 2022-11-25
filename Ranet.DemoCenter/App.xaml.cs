using System;
using System.Windows;

namespace Ranet.DemoCenter
{
	public partial class App
	{
		#region  Methods

		protected override void OnApplicationStartup()
		{
			base.OnApplicationStartup();

			StartupUri = new Uri("ApplicationWindow.xaml", UriKind.RelativeOrAbsolute);
		}

		#endregion
	}
}