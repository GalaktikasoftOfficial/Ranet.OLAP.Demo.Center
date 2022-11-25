#if !SILVERLIGHT
using System;
#endif

namespace ServerExplorer.Demo
{
	public partial class App
	{
		protected override void OnApplicationStartup()
		{
			base.OnApplicationStartup();

#if SILVERLIGHT
            RootVisual = new ServerExplorerModule();
#else
			StartupUri = new Uri("ApplicationWindow.xaml", UriKind.RelativeOrAbsolute);
#endif
		}
	}
}