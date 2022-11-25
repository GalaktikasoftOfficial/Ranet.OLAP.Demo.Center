using System;
using System.ComponentModel;
using System.Windows;

namespace Ranet.Demo.Core
{
	public partial class ButtonGroupControl : INotifyPropertyChanged
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ViewModeProperty = DependencyProperty.Register(
			"ViewMode", typeof (ViewMode), typeof (ButtonGroupControl),
			new PropertyMetadata(ViewMode.Demo, ViewModePropertyChanged));

		public static readonly DependencyProperty ShowOptionsProperty = DependencyProperty.Register(
			"ShowOptions", typeof (bool), typeof (ButtonGroupControl), new PropertyMetadata(true, ShowOptionsPropertyChanged));

		#endregion

		#region Fields

		private bool _ignoreCheckedHandler;

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Ctors

		public ButtonGroupControl()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

		public bool ShowOptions
		{
			get { return (bool) GetValue(ShowOptionsProperty); }
			set { SetValue(ShowOptionsProperty, value); }
		}

		public ViewMode ViewMode
		{
			get { return (ViewMode) GetValue(ViewModeProperty); }
			set { SetValue(ViewModeProperty, value); }
		}

		#endregion

		#region  Methods

		private static void ShowOptionsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((ButtonGroupControl) dependencyObject).OnShowOptionsChanged();
		}

		private void OnShowOptionsChanged()
		{
			ShowOptionsButton.IsChecked = ShowOptions;
		}

		private static void ViewModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ButtonGroupControl) d).OnViewModeChanged();
		}

		private void OnViewModeChanged()
		{
			_ignoreCheckedHandler = true;

			switch (ViewMode)
			{
				case ViewMode.Source:
					CodeButton.IsChecked = true;
					break;
			}

			_ignoreCheckedHandler = false;
		}


		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void CodeButton_OnChecked(object sender, RoutedEventArgs e)
		{
			if (_ignoreCheckedHandler) return;

#if SILVERLIGHT
			ViewMode = ViewMode.Source;
#else
			SetCurrentValue(ViewModeProperty, ViewMode.Source);
#endif
		}

		private void PreviewButton_OnChecked(object sender, RoutedEventArgs e)
		{
			if (_ignoreCheckedHandler) return;

#if SILVERLIGHT
			ViewMode = ViewMode.Demo;
#else
			SetCurrentValue(ViewModeProperty, ViewMode.Demo);
#endif
		}

		private void ShowOptionsButton_OnChecked(object sender, RoutedEventArgs e)
		{
			if (ShowOptionsButton == null) return;
			var result = Convert.ToBoolean(ShowOptionsButton.IsChecked);
#if SILVERLIGHT
			ShowOptions = result;
#else
			SetCurrentValue(ShowOptionsProperty, result);
#endif
		}

		#endregion
	}

	public enum ViewMode
	{
		Demo,
		Source
	}
}