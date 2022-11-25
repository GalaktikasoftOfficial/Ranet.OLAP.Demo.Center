using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Ranet.Demo.Core
{
	public partial class SourceViewerControl
	{
		public static readonly DependencyProperty SourceTextProperty = DependencyProperty.Register(
			"SourceText", typeof (SourceCodeEntry), typeof (SourceViewerControl),
			new PropertyMetadata(default(SourceCodeEntry), SourceTextPropertyChangedCallback));

		public SourceViewerControl()
		{
			InitializeComponent();
		}

		public SourceCodeEntry SourceText
		{
			get { return (SourceCodeEntry) GetValue(SourceTextProperty); }
			set { SetValue(SourceTextProperty, value); }
		}

		private static void SourceTextPropertyChangedCallback(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((SourceViewerControl) dependencyObject).UpdateSourceText();
		}

		private void UpdateSourceText()
		{
#if SILVERLIGHT
			if (string.IsNullOrEmpty(SourceText.SourceCode))
			{
				OutText.Blocks.Clear();
			}
			else
			{
				OutText.Xaml = SourceText.SourceCode;
				OutText.Selection.Select(OutText.ContentStart, OutText.ContentStart);
			}
#else
			if (string.IsNullOrEmpty(SourceText.SourceCode))
			{
				OutText.Document.Blocks.Clear();
			}
			else
			{
				OutText.Document = XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(SourceText.SourceCode))) as FlowDocument;
				OutText.CaretPosition = OutText.Selection.Start;
			}
#endif
		}
	}
}