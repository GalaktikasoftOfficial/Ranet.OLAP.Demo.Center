using System;
using System.Windows;
using Ranet.Olap.Core.Common;
using Ranet.Olap.Core.CommonUtilities;
using Ranet.Olap.Core.Interfaces;
using Ranet.Olap.Core.Types;
using Ranet.Olap.UI.Services;

namespace Ranet.Demo.Core
{
	public partial class ConnectionEditorControl
	{
		public static readonly DependencyProperty ConnectionStringIdProperty = DependencyProperty.Register(
			"ConnectionStringId", typeof (string), typeof (ConnectionEditorControl), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty ConnectionStringOlapProperty = DependencyProperty.Register(
			"ConnectionStringOlap", typeof (string), typeof (ConnectionEditorControl), new PropertyMetadata(default(string)));

		private string _lastError = "";

		public ConnectionEditorControl()
		{
			InitializeComponent();
		}

		public string ConnectionStringId
		{
			get { return (string) GetValue(ConnectionStringIdProperty); }
			set { SetValue(ConnectionStringIdProperty, value); }
		}

		public string ConnectionStringOlap
		{
			get { return (string) GetValue(ConnectionStringOlapProperty); }
			set { SetValue(ConnectionStringOlapProperty, value); }
		}

		private void CheckConection_OnClick(object sender, RoutedEventArgs e)
		{
			CheckedInfo.Text = "Checking connection...";

			CheckConnection(ConnectionStringOlap, () => { CheckedInfo.Text = "Connection has been succesfully checked."; }
				, () => { CheckedInfo.Text = "There were errors while connection checking:" + _lastError; }
				);
		}

		public void CheckConnection(string connectionString, Action onSuccess, Action onError)
		{
			var dataLoader = RanetServiceLocator.GetDataLoader();

			var schema = new OlapActionBase {ActionType = OlapActionTypes.CheckConnectionString, Connection = connectionString};
			dataLoader.LoadAsync(schema, delegate
			{
				if (onSuccess != null)
					onSuccess();
			}, exception =>
			{
				_lastError = Utilities.UnwrapException(exception).Message;
				if (onError != null)
					onError();
			});
		}
	}
}