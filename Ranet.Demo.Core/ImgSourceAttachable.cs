using System.Windows;
using System.Windows.Media;

namespace Ranet.Demo.Core
{
	public sealed class ImgSourceAttachable : DependencyObject
	{
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(
			"ImageSource", typeof (ImageSource), typeof (ImgSourceAttachable), new PropertyMetadata(default(ImageSource)));

		public static void SetImageSource(DependencyObject element, ImageSource value)
		{
			element.SetValue(ImageSourceProperty, value);
		}

		public static ImageSource GetImageSource(DependencyObject element)
		{
			return (ImageSource) element.GetValue(ImageSourceProperty);
		}
	}
}