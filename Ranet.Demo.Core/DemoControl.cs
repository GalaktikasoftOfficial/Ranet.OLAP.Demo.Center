using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ranet.Demo.Core.Configuration;
using Ranet.Olap.Core.Helpers;
using Ranet.Olap.Core.Interfaces;
using Ranet.Olap.UI.Services;

namespace Ranet.Demo.Core
{
	public class DemoControl : ContentControl, INotifyPropertyChanged
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ViewModeProperty = DependencyProperty.Register(
			"ViewMode", typeof(ViewMode), typeof(DemoControl),
			new PropertyMetadata(default(ViewMode)));

		public static readonly DependencyProperty ShowOptionsProperty = DependencyProperty.Register(
			"ShowOptions", typeof(bool), typeof(DemoControl), new PropertyMetadata(true, ShowOptionsPropertyChanged));

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
			"Description", typeof(string), typeof(DemoControl), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty ControlNameProperty = DependencyProperty.Register(
			"ControlName", typeof(string), typeof(DemoControl), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
			"Options", typeof(FrameworkElement), typeof(DemoControl), new PropertyMetadata(null, OnOptionsPropertyChanged));

		public static readonly DependencyProperty AboutProperty = DependencyProperty.Register(
			"About", typeof(string), typeof(DemoControl), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			"ImageSource", typeof(ImageSource), typeof(DemoControl), new PropertyMetadata(default(ImageSource)));

		public static readonly DependencyProperty IsInDemoCenterProperty = DependencyProperty.Register(
			"IsInDemoCenter", typeof(bool), typeof(DemoControl), new PropertyMetadata(false));

		#endregion

		#region Fields

		private readonly IConfiguration _iConfiguration;
		private string _currentConnection;
		private ConfigurationInfo _data;
		private string _defaultCubeName;
		private Grid _gridContent;
		private Grid _optionsHost;
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler CurrentConnectionChanged;
		public event EventHandler ConfigurationDataChanged;
		public event EventHandler DefaultCubeNameChanged;

		#endregion

		#region Ctors

		public DemoControl()
		{
			DefaultStyleKey = typeof(DemoControl);
			SourceList = GetSourceFiles();
			_iConfiguration = ConnectionServiceProvider.Instance.GetServiceExt<IConfiguration>() ?? new DemoConfiguration();
			UpdateConnectionInformation();
			_iConfiguration.ConfigurationDataChanged += ConfigurationOnConfigurationDataChanged;
		}

		#endregion

		#region Properties

		public bool ShowOptions
		{
			get => (bool) GetValue(ShowOptionsProperty);
			set => SetValue(ShowOptionsProperty, value);
		}

		public string DefaultCubeName
		{
			get => _defaultCubeName;
			set
			{
				_defaultCubeName = value;
				OnDefaultCubeNameChanged();
				OnPropertyChanged("DefaultCubeName");
			}
		}

		public string CurrentConnection
		{
			get => _currentConnection;
			set
			{
				_currentConnection = value;
				OnCurrentConnectionChanged();
				OnPropertyChanged("CurrentConnection");
			}
		}

		public ConfigurationInfo ConfigurationData
		{
			get => _data;
			set
			{
				_data = value;
				OnConfigurationDataChanged();
				OnPropertyChanged("ConfigurationData");
			}
		}

		public ViewMode ViewMode
		{
			get => (ViewMode) GetValue(ViewModeProperty);
			set => SetValue(ViewModeProperty, value);
		}

		public bool IsInDemoCenter
		{
			get => (bool) GetValue(IsInDemoCenterProperty);
			set => SetValue(IsInDemoCenterProperty, value);
		}

		public ImageSource ImageSource
		{
			get => (ImageSource) GetValue(ImageSourceProperty);
			set => SetValue(ImageSourceProperty, value);
		}

		public List<SourceCodeEntry> SourceList { get; }

		public string AssemblyName => new AssemblyName(GetType().Assembly.FullName).Name;

		protected string ControlNameInt => GetType().Name;

		public string About
		{
			get => (string) GetValue(AboutProperty);
			set => SetValue(AboutProperty, value);
		}

		public string Description
		{
			get => (string) GetValue(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		public string ControlName
		{
			get
			{
				var controlName = (string) GetValue(ControlNameProperty);

				return controlName ?? string.Empty;
			}
			set => SetValue(ControlNameProperty, value);
		}

		public FrameworkElement Options
		{
			get => (FrameworkElement) GetValue(OptionsProperty);
			set => SetValue(OptionsProperty, value);
		}

		#endregion

		#region  Methods

		private static void ShowOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((DemoControl) d).OnShowOptionsChanged();
		}

		private void OnShowOptionsChanged()
		{
			_gridContent.SetValue(Grid.ColumnSpanProperty, ShowOptions ? 1 : 2);
			_optionsHost.SetValue(VisibilityProperty, ShowOptions ? Visibility.Visible : Visibility.Collapsed);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_gridContent = (Grid) GetTemplateChild("GridContent");
			_optionsHost = (Grid) GetTemplateChild("OptionsHost");
		}

		protected IDataLoader GetDataLoader()
		{
			return RanetServiceLocator.GetDataLoader();
		}

		protected virtual void OnConfigurationDataChanged()
		{
			ConfigurationDataChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnDefaultCubeNameChanged()
		{
			DefaultCubeNameChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnCurrentConnectionChanged()
		{
			CurrentConnectionChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private static void OnOptionsPropertyChanged(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs e)
		{
			((DemoControl) dependencyObject).OnOptionsChanged((FrameworkElement) e.OldValue, (FrameworkElement) e.NewValue);
		}

	  protected override IEnumerator LogicalChildren
	  {
	    get
	    {
	      if (Options != null)
	        yield return Options;

	      var baseChildren = base.LogicalChildren;

	      if (baseChildren != null)
	        while (baseChildren.MoveNext())
	          yield return baseChildren.Current;
	    }
	  }

	  private void OnOptionsChanged(FrameworkElement oldOptions, FrameworkElement newOptions)
    {
      if (oldOptions != null)
        RemoveLogicalChild(oldOptions);

      if (newOptions != null)
        AddLogicalChild(newOptions);
    }


		private void ConfigurationOnConfigurationDataChanged(object sender, EventArgs eventArgs)
		{
			UpdateConnectionInformation();
		}

		private void UpdateConnectionInformation()
		{
			if (_iConfiguration == null) 
				return;

			ConfigurationData = _iConfiguration.Load();

			if (ConfigurationData.SelectedIndex < 0 ||
			    ConfigurationData.SelectedIndex >= ConfigurationData.Storage.ListConnections.Count) 
				return;

			var currentConnection = ConfigurationData.Storage.ListConnections[ConfigurationData.SelectedIndex].ConnectionString;

			if (string.IsNullOrEmpty(CurrentConnection) || !currentConnection.Equals(CurrentConnection))
				CurrentConnection = currentConnection;

			if (string.IsNullOrEmpty(DefaultCubeName) || !ConfigurationData.CubeName.Equals(DefaultCubeName))
				DefaultCubeName = ConfigurationData.CubeName;

		  DefaultCubeName = OlapHelper.CheckSquareBrackets(DefaultCubeName);
		}

		public List<SourceCodeEntry> GetSourceFiles()
		{
			var sourceFile = new SourceFile(ControlNameInt);

			return
				sourceFile.GetFileNames()
					.Select(fileName => new SourceCodeEntry(fileName, sourceFile.GetSource(fileName, AssemblyName)))
					.ToList();
		}

		#endregion
	}

	public class SourceCodeEntry
	{
		#region Ctors

		public SourceCodeEntry(string fileName, string sourceCode)
		{
			FileName = fileName;
			SourceCode = sourceCode;
		}

		#endregion

		#region Properties

		public string FileName { get; set; }
		public string SourceCode { get; set; }

		#endregion
	}

	public class TreeViewEx : TreeView
	{
	}
}