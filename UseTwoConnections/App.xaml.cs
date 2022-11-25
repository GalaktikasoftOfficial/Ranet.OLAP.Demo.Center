#if !SILVERLIGHT
using System;
#endif

namespace Integrations.Demo
{
	public partial class App
	{
		protected override void OnApplicationStartup()
		{
			base.OnApplicationStartup();

#if SILVERLIGHT
			RootVisual = new UseTwoConnections();
#else
			StartupUri = new Uri("ApplicationWindow.xaml", UriKind.RelativeOrAbsolute);
#endif
		}
	}
}