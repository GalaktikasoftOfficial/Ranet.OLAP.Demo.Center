using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Ranet.Olap.Themes;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.Interfaces;
using Ranet.Olap.Service.Adomd.Services;
using Ranet.Olap.Service.Services;
using Ranet.Olap.UI;
using Ranet.Olap.UI.Services;

namespace Ranet.Demo.Core
{
	public class RanetDemoApp : Application
	{
		public RanetDemoApp()
		{
			InitServiceLocator();

			Startup += Application_Startup;
			Exit += Application_Exit;
		}

		private void InitServiceLocator()
		{
			var olapDataService = new AdomdOlapDataService(DummyConnectionStringResolver.Instance, new AdomdConnectionPool(), true);

			RanetServiceLocator.RegisterServiceFactory<IDataLoader>(() => new ServerDataLoader(olapDataService));
			RanetServiceLocator.RegisterServiceFactory<IStorageManager>(() => new ServerStorageManager(olapDataService));
		}


		private void Application_Startup(object sender, StartupEventArgs e)
		{
			OnApplicationStartup();
		}

		protected virtual void OnApplicationStartup()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			RanetThemeManager.CurrentTheme = RanetThemes.Office;
		}

		private void Application_Exit(object sender, EventArgs e)
		{
			OnApplicationExit();
		}

		protected virtual void OnApplicationExit()
		{
		}
	}
}
