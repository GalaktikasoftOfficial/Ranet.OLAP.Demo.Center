#if !SILVERLIGHT
using System;
#endif

namespace Choices.Demo
{
	public partial class App
	{
		protected override void OnApplicationStartup()
		{
			base.OnApplicationStartup();

#if SILVERLIGHT
			RootVisual = new PopUpChoices();
#else
			StartupUri = new Uri("ApplicationWindow.xaml", UriKind.RelativeOrAbsolute);
#endif
		}
	}
}