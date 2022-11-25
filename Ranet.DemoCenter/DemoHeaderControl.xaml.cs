using System.Windows;
using System.Windows.Data;
using Ranet.Demo.Core;


namespace Ranet.DemoCenter
{
	public partial class DemoHeaderControl
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ActualDemoControlProperty = DependencyProperty.Register(
			"ActualDemoControl", typeof (DemoControl), typeof (DemoHeaderControl),
			new PropertyMetadata(null, OnActualDemoControlPropertyChanged));

		public static readonly DependencyProperty ShowSourceProperty = DependencyProperty.Register(
			"ShowSource", typeof (bool), typeof (DemoHeaderControl),
			new PropertyMetadata(false, OnShowSourceDemoPropertyChanged));

		public static readonly DependencyProperty ShowDemoProperty = DependencyProperty.Register(
			"ShowDemo", typeof (bool), typeof (DemoHeaderControl),
			new PropertyMetadata(true, OnShowSourceDemoPropertyChanged));

		public static readonly DependencyProperty TreeViewDemoProperty = DependencyProperty.Register(
			"TreeViewDemo", typeof (TreeViewDemo), typeof (DemoHeaderControl), new PropertyMetadata(default(TreeViewDemo)));

		public static string DemoTitle = "Ranet UI OLAP for WPF";

		#endregion

		#region Fields

		private bool _skipViewModeChanged;

		#endregion

		#region Ctors

		public DemoHeaderControl()
		{
			InitializeComponent();

			SetBinding(ActualDemoControlProperty, new Binding
			{
				Path = new PropertyPath("TreeViewDemo.CurrentDemo"),
				Source = this
			});
		}

		#endregion

		#region Properties

		public bool ShowSource
		{
			get => (bool) GetValue(ShowSourceProperty);
			set => SetValue(ShowSourceProperty, value);
		}

		public bool ShowDemo
		{
			get => (bool) GetValue(ShowDemoProperty);
			set => SetValue(ShowDemoProperty, value);
		}

		public DemoControl ActualDemoControl
		{
			get => (DemoControl) GetValue(ActualDemoControlProperty);
			set => SetValue(ActualDemoControlProperty, value);
		}

		public TreeViewDemo TreeViewDemo
		{
			get => (TreeViewDemo) GetValue(TreeViewDemoProperty);
			set => SetValue(TreeViewDemoProperty, value);
		}

		#endregion

		#region  Methods

		private static void OnShowSourceDemoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DemoHeaderControl) d).OnShowSourceDemoChanged();
		}


		private void OnShowSourceDemoChanged()
		{
			if (_skipViewModeChanged)
				return;

			_skipViewModeChanged = true;

			if (ActualDemoControl != null)
				ActualDemoControl.ViewMode = ShowSource ? ViewMode.Source : ViewMode.Demo;

			_skipViewModeChanged = false;
		}

		private static void OnActualDemoControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DemoHeaderControl) d).OnActualDemoControlChanged();
		}

		private void OnActualDemoControlChanged()
		{
			if (ActualDemoControl != null && _skipViewModeChanged == false)
			{
				_skipViewModeChanged = true;

				ShowDemo = ActualDemoControl.ViewMode == ViewMode.Demo;
				ShowSource = ActualDemoControl.ViewMode == ViewMode.Source;

				_skipViewModeChanged = false;
			}
		}

		private void OnSettingsClick(object sender, RoutedEventArgs e)
		{
			if (TreeViewDemo == null)
				return;

			TreeViewDemo.ShowConfigurationWindow();
		}

		#endregion

		private void BuyButton_OnClick(object sender, RoutedEventArgs e)
		{
			var uri = "http://galaktika-soft.com/ranet-olap/";

			System.Diagnostics.Process.Start(uri);
		}
	}
}