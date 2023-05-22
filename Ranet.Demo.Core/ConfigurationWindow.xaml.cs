using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ranet.Olap.Themes;
using Ranet.Demo.Core.Configuration;
using Ranet.Olap.Core.Helpers;
using Ranet.Olap.UI;
using Ranet.Olap.UI.Controls;
using Ranet.Olap.UI.Controls.Choices;
using Zaaml.UI.Windows;

namespace Ranet.Demo.Core
{
	public partial class ConfigurationWindow
	{
		private ConfigurationInfo _cloneConfigInfo;

		public ConfigurationWindow(ConfigurationInfo info)
		{
			InitializeComponent();
			LoadConnections(info);
			LoadCubeName();
			LoadThemes();
			LoadCultures();
			var tooltipMessage = "Application restart is needed to apply this setting";
#if !SILVERLIGHT
			RefreshImage.ToolTip = tooltipMessage;
#else
			ToolTipService.SetToolTip(RefreshImage, tooltipMessage);
#endif
			IsWaiting = false;
		}

		public ConfigurationInfo CloneConfigInfo
		{
			get { return _cloneConfigInfo; }
		}

		private void LoadCubeName()
		{
			CubeChoice.SelectedItemTextBox.Text = _cloneConfigInfo.CubeName;
			SetUpCubeName();
		}

		public event EventHandler CloneConfigInfoChanged;

		protected virtual void OnCloneConfigInfoChanged()
		{
			var handler = CloneConfigInfoChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}


		public void LoadConnections(ConfigurationInfo cloneConfigurationInfo)
		{
			_cloneConfigInfo = cloneConfigurationInfo;
			int index = _cloneConfigInfo.SelectedIndex; //selected index before cleaning combobox
			ComboListConnections.ItemsSource = null;

			ComboListConnections.ItemsSource = _cloneConfigInfo.Storage.ListConnections;
			if (index >= 0)
			{
				ComboListConnections.SelectedIndex = index;
			}
		}


		private void LoadThemes()
		{
			ThemesComboBox.SelectionChanged -= ThemesComboBoxOnSelectionChanged;
			ThemesComboBox.Items.Clear();
			ThemesComboBox.Items.Add(RanetThemes.MetroLight);
			ThemesComboBox.Items.Add(RanetThemes.MetroDark);
			ThemesComboBox.Items.Add(RanetThemes.Office);
			ThemesComboBox.SelectedItem = RanetThemeManager.CurrentTheme;
			ThemesComboBox.SelectionChanged += ThemesComboBoxOnSelectionChanged;
		}

		private void LoadCultures()
		{
			LanguagesCombo.SelectionChanged -= LanguagesComboOnSelectionChanged;
			LanguagesCombo.Items.Clear();
			LanguagesCombo.ItemsSource = new List<string>
			{
				"en-US",
				"ru",
				"pt",
				"it",
				"de",
				"es",
				"fr",
				"pl",
				"cs-CZ",
				"zh-CN",
				"be",
				"uk",
				"kk-KZ"
			};
			LanguagesCombo.SelectedItem = Thread.CurrentThread.CurrentUICulture.Name;
			LanguagesCombo.SelectionChanged += LanguagesComboOnSelectionChanged;
		}

		private void LanguagesComboOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			_cloneConfigInfo.Culture = LanguagesCombo.SelectedValue.ToString();
		}

		private void ThemesComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			var selectedTheme = (RanetTheme)ThemesComboBox.SelectedItem;
			_cloneConfigInfo.CurrentTheme = selectedTheme.Name;
			RanetThemeManager.CurrentTheme = selectedTheme;
		}

		private void ComboListConnections_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var index = ComboListConnections.SelectedIndex;
			if (index < 0) return;
			_cloneConfigInfo.SelectedIndex = index;
			SetUpCubeName();
			CubeValidation();
		}

		private void CubeValidation()
		{
			if (string.IsNullOrEmpty(_cloneConfigInfo.CubeName)) return;
			IsWaiting = true;
			var cubeChoice = new CubeChoiceCombo()
			{
				Connection = _cloneConfigInfo.Storage.ListConnections[_cloneConfigInfo.SelectedIndex].ConnectionString,
			};
			cubeChoice.CubesLoaded += (sender, args) => Validate(cubeChoice);
			cubeChoice.LoadCubes();
		}

		private void Validate(CubeChoiceCombo cubeChoice)
		{
			IsWaiting = false;

			var cubeName = OlapHelper.ConvertToNormalStyle(_cloneConfigInfo.CubeName);
			if (cubeChoice.CubeCombo.Items.Cast<CubeItemComboBox>().Any(item => item.CubeName == cubeName))
				return;
			if (cubeChoice.CubeCombo.Items.Count > 0)
				MessageWindow.Show("Current cube doesn't exist",
					Ranet.Olap.UI.Localization.MessageBox_Warning, MessageWindowButtons.OK);
			_cloneConfigInfo.CubeName = String.Empty;
			CubeChoice.SelectedItemTextBox.Text = String.Empty;
		}

		private void UpdateComboListConnections()
		{
			ComboListConnections.ItemsSource = null;
			ComboListConnections.ItemsSource = _cloneConfigInfo.Storage.ListConnections;
			if (_cloneConfigInfo.SelectedIndex >= ComboListConnections.Items.Count) return;
			ComboListConnections.SelectedItem = _cloneConfigInfo.Storage.ListConnections[_cloneConfigInfo.SelectedIndex];
		}


		private void SetUpConnections_OnClick(object sender, RoutedEventArgs e)
		{
			var newWindow = new FloatingDialog()
			{
				Title = "Connections",
				Buttons = FloatingDialogButtons.OkCancel,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				MinHeight = 300,
				MinWidth = 400,
				Height = 300,
				Width = 800,
				Content = new ConnectionListControl(_cloneConfigInfo)
			}.InitWindow(this);

			newWindow.Closed += ConnectionSettingsDialog_Closed;
			newWindow.ShowDialog();
		}

		private void ConnectionSettingsDialog_Closed(object sender, EventArgs e)
		{
			var window = (FloatingDialog)sender;
			if (window.DialogResult != true) return;
			var connectionControl = (ConnectionListControl)window.Content;
			_cloneConfigInfo = connectionControl.CloneConfigInfo;
			UpdateComboListConnections();
		}

		private void SetUpCubeName()
		{
			CubeChoice.Connection = _cloneConfigInfo.Storage.ListConnections[_cloneConfigInfo.SelectedIndex].ConnectionString;
			CubeChoice.NeedReload = true;
			CubeChoice.CubeSelected -= ChoicePopUpOnCubeSelected;
			CubeChoice.CubeSelected += ChoicePopUpOnCubeSelected;
		}

		private void ChoicePopUpOnCubeSelected(object sender, CubeEventArgs cubeEventArgs)
		{
			_cloneConfigInfo.CubeName = CubeChoice.CubeName;
		}

		private bool m_IsWaiting;

		public bool IsWaiting
		{
			get { return m_IsWaiting; }
			set
			{
				if (m_IsWaiting != value)
				{
					if (value)
					{
						Cursor = Cursors.Wait;
						if (CubeValBusyFrame != null)
							CubeValBusyFrame.Visibility = Visibility.Visible;
					}
					else
					{
						Cursor = Cursors.Arrow;
						if (CubeValBusyFrame != null)
							CubeValBusyFrame.Visibility = Visibility.Collapsed;
					}
					IsEnabled = !value;
					m_IsWaiting = value;
				}
			}
		}
	}
}