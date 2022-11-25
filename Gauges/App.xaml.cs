#if !SILVERLIGHT
using System;
#endif

namespace Gauges.Demo
{
	public partial class App
	{
		protected override void OnApplicationStartup()
		{
			base.OnApplicationStartup();

#if SILVERLIGHT
			RootVisual = new GaugesModule();
#else
			StartupUri = new Uri("ApplicationWindow.xaml", UriKind.RelativeOrAbsolute);
#endif
		}
	}
}