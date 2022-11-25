using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ranet.Demo.Core.Configuration;
using Ranet.Olap.Core.Common;
using Ranet.Olap.UI.Controls;
using Ranet.Olap.UI.Controls.General;
using Ranet.Olap.UI.Controls.ToolBar;
using Zaaml.UI.Controls.ToolBar;
using Zaaml.UI.Windows;

namespace Ranet.Demo.Core
{
	public partial class ConnectionListControl
	{
		public static readonly DependencyProperty ConnectionsProperty = DependencyProperty.Register(
			"Connections", typeof (List<ConnectionInfo>), typeof (ConnectionListControl),
			new PropertyMetadata(default(List<ConnectionInfo>)));

		private ConfigurationInfo _cloneConfigInfo;
		private ToolBarButton _deleteButton;
		private ToolBarButton _editButton;

		private ToolBarButton _newButton;

		public ConnectionListControl(ConfigurationInfo cloneConfigurationInfo)
		{
			InitializeComponent();
			_cloneConfigInfo = cloneConfigurationInfo;
			Connections = _cloneConfigInfo.Storage.ListConnections;
			InitializeToolBar();
		}

		public ConfigurationInfo CloneConfigInfo
		{
			get { return _cloneConfigInfo; }
		}

		public List<ConnectionInfo> Connections
		{
			get { return (List<ConnectionInfo>) GetValue(ConnectionsProperty); }
			set { SetValue(ConnectionsProperty, value); }
		}

		private void InitializeToolBar()
		{
			_newButton = RanetToolBar.BuildToolbarButton("New connection");
			_newButton.Content = UiHelper.CreateIcon(GetImageSource("new.png"));
			_newButton.Click += NewConnectionButtonOnClick;
			ToolTipService.SetToolTip(_newButton, "New connection");
			ToolBar.ItemCollection.Add(_newButton);


			_editButton = RanetToolBar.BuildToolbarButton("Edit the connection");
			_editButton.Content = UiHelper.CreateIcon(GetImageSource("edit.png"));
			_editButton.Click += EditConnectionButtonOnClick;
			_editButton.SetValue(IsEnabledProperty, false);
			ToolTipService.SetToolTip(_editButton, "Edit the connection");
			ToolBar.ItemCollection.Add(_editButton);

			_deleteButton = RanetToolBar.BuildToolbarButton("Delete the connection");
			_deleteButton.Content = UiHelper.CreateIcon(GetImageSource("remove.png"));
			_deleteButton.Click += DeleteConnectionButtonOnClick;
			_deleteButton.SetValue(IsEnabledProperty, false);
			ToolTipService.SetToolTip(_deleteButton, "Delete the connection");
			ToolBar.ItemCollection.Add(_deleteButton);
		}

		private void DeleteConnectionButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			int index = DataGridConnections.SelectedIndex;
			if (index < 0) return;
			MessageBoxResult m = MessageBox.Show("You want delete current item permanently?", "Warning",
				MessageBoxButton.OKCancel);
			if (m == MessageBoxResult.OK)
			{
				_cloneConfigInfo.Storage.Remove(_cloneConfigInfo.Storage.ListConnections[index].ConnectionID);

				UpdateConnections();
				
				_cloneConfigInfo.SelectedIndex = 0;
				DataGridConnections.SelectedIndex = 0;
			}
		}

		private void EditConnectionButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			var selectedIndex = DataGridConnections.SelectedIndex;
			if (selectedIndex < 0) return;
			var selectedConnectionInfo = _cloneConfigInfo.Storage.ListConnections[selectedIndex];
			_connectionEditorWindow = new FloatingDialog()
			{
				Title = "Edit Connection",
				Buttons = FloatingDialogButtons.OkCancel,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				MinHeight = 150,
				MinWidth = 400,
				Height = 400,
				Width = 600,
				Content = new ConnectionEditorControl
				{
					ConnectionStringId = selectedConnectionInfo.ConnectionID,
					ConnectionStringOlap = selectedConnectionInfo.ConnectionString
				}
			}.InitWindow(this);
			_connectionEditorWindow.Closing += EditorWindowOnClosing;
			_connectionEditorWindow.ShowDialog();
		}

		private FloatingDialog _connectionEditorWindow;

		private void NewConnectionButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			_connectionEditorWindow = new FloatingDialog
			{
				Title = "New Connection",
				Buttons = FloatingDialogButtons.OkCancel,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				MinHeight = 150,
				MinWidth = 400,
				Height = 400,
				Width = 600,
				Content = new ConnectionEditorControl()
			}.InitWindow(this);
			_connectionEditorWindow.Closing += CreaterWindowOnClosing;
			_connectionEditorWindow.ShowDialog();
		}

		private void CreaterWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			if (_connectionEditorWindow.DialogResult != true) return;
			cancelEventArgs.Cancel = true;

			var newConnection = (ConnectionEditorControl)_connectionEditorWindow.Content;
			if (!IsValidConnection(newConnection.ConnectionStringId, newConnection.ConnectionStringOlap))
			{
				MessageWindow.Show("Connection Id and Connection String can't be empty.", Ranet.Olap.UI.Localization.Error,
					MessageWindowButtons.OK);
				return;
			}
			Action closeAction = OnCreatorDialogOk;
			_connectionEditorWindow.Dispatcher.BeginInvoke(closeAction);
		}
		
		private void EditorWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			if (_connectionEditorWindow.DialogResult != true) return;
			cancelEventArgs.Cancel = true;
			var newConnection = (ConnectionEditorControl)_connectionEditorWindow.Content;
			if (!IsValidConnection(newConnection.ConnectionStringId, newConnection.ConnectionStringOlap))
			{
				MessageWindow.Show("Connection Id and Connection String can't be empty.", Ranet.Olap.UI.Localization.Error, MessageWindowButtons.OK);
				return;
			}
			Action closeAction = OnEditorDialogOk;
			_connectionEditorWindow.Dispatcher.BeginInvoke(closeAction);
		}

		private bool IsValidConnection(string stringId, string stringOlap)
		{
			return !string.IsNullOrEmpty(stringId) && !string.IsNullOrEmpty(stringOlap);
		}

		private void OnCreatorDialogOk()
		{
			var newConnection = (ConnectionEditorControl)_connectionEditorWindow.Content;
			_cloneConfigInfo.Storage.Add(newConnection.ConnectionStringId, newConnection.ConnectionStringOlap);

			UpdateConnections();
			CloseConnectionEditorWindow();
		}

		private void OnEditorDialogOk()
		{
			var newConnection = (ConnectionEditorControl)_connectionEditorWindow.Content;
			var index = DataGridConnections.SelectedIndex;
			_cloneConfigInfo.Storage.ListConnections[index].ConnectionID = newConnection.ConnectionStringId;
			_cloneConfigInfo.Storage.ListConnections[index].ConnectionString = newConnection.ConnectionStringOlap;

			UpdateConnections();
			CloseConnectionEditorWindow();
		}

		private void UpdateConnections()
		{
			Connections = null;
			Connections = _cloneConfigInfo.Storage.ListConnections;
		}


		private void CloseConnectionEditorWindow()
		{
			_connectionEditorWindow.Closing -= CreaterWindowOnClosing;
			_connectionEditorWindow.Closing -= EditorWindowOnClosing;

			_connectionEditorWindow.Close();

			if (_connectionEditorWindow != null)
				_connectionEditorWindow.Content = null;

			_connectionEditorWindow = null;
		}

		private ImageSource GetImageSource(string fileName)
		{
			var assemblyName = new AssemblyName(GetType().Assembly.FullName).Name;

#if !SILVERLIGHT
			var packApplication = "pack://application:,,,/";
#else
			var packApplication = "/";
#endif

			var uriString = packApplication + assemblyName + ";component/icons/" + fileName;
			return new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
		}

		private void DataGridConnections_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedIndex = DataGridConnections.SelectedIndex;
			bool result = selectedIndex > 0;
			_editButton.SetValue(IsEnabledProperty, result);
			_deleteButton.SetValue(IsEnabledProperty, result);
			if (selectedIndex >= 0)
				CloneConfigInfo.SelectedIndex = selectedIndex;
		}
	}
}