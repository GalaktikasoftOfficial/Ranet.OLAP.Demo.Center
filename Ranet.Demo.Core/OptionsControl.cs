using System.Windows;
using System.Windows.Controls;
using System;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Ranet.Demo.Core
{
	public class OptionsControl : ContentControl
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register
			("Description", typeof(string), typeof(OptionsControl), 
				new PropertyMetadata(default(string), DescriptionPropertyChanged));

		public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register
			("Options", typeof(FrameworkElement), typeof(OptionsControl), 
				new PropertyMetadata(default(FrameworkElement)));

		#endregion

		#region Fields

		private RichTextBox _richTextBoxDescription;

		#endregion

		#region Ctors

		public OptionsControl()
		{
			DefaultStyleKey = typeof(OptionsControl);
		}

		#endregion

		#region Properties

		public string Description
		{
			get => (string) GetValue(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		public FrameworkElement Options
		{
			get => (FrameworkElement) GetValue(OptionsProperty);
			set => SetValue(OptionsProperty, value);
		}

		#endregion

		#region  Methods

		private static void DescriptionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((OptionsControl) dependencyObject).UpdateDescription();
		}

		private void UpdateDescription()
		{
			if (_richTextBoxDescription == null || Description == null || !Description.Contains("Section")) 
				return;

			if (string.IsNullOrEmpty(Description))
			{
				_richTextBoxDescription.Document.Blocks.Clear();
			}
			else
			{
				try
				{
					var section = XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(Description))) as Section;

					if (section == null)
						return;

					var document = new FlowDocument();

					while (section.Blocks.Count > 0)
						document.Blocks.Add(section.Blocks.FirstBlock);

					_richTextBoxDescription.Document = document;
					_richTextBoxDescription.CaretPosition = _richTextBoxDescription.Selection.Start;
				}
				catch (Exception ex)
				{
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_richTextBoxDescription = (RichTextBox) GetTemplateChild("RichTextDescription");
			UpdateDescription();
		}

		#endregion
	}
}