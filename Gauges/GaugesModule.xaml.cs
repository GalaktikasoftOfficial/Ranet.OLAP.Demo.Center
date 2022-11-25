using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Ranet.Demo.Core;
using Zaaml.PresentationCore.Utils;

namespace Gauges
{
	[Demo]
	public partial class GaugesModule
	{
		public GaugesModule()
		{
			InitializeComponent();
			ControlName = GaugeResource.GaugeCustomization_Name;
			About = GaugeResource.GaugeCustomization_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/GaugeModuleDescription.txt")))
				Description = reader.ReadToEnd();

			var gaugeBrushes = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public).Select
					  (c => new GaugeNamedBrush
					  {
						  Brush = new SolidColorBrush((Color)c.GetValue(null, null)),
						  Name = c.Name
					  }).ToList();

			var dimGreen = new SolidColorBrush(Color.FromArgb(255, 154, 186, 89));
			gaugeBrushes.Add(new GaugeNamedBrush
			{
				Brush = dimGreen,
				Name = "DimGreen"
			});

			var dimYellow = new SolidColorBrush(Color.FromArgb(255, 243, 244, 107));
			gaugeBrushes.Add(new GaugeNamedBrush
			{
				Brush = dimYellow,
				Name = "DimYellow"
			});

			var dimRed = new SolidColorBrush(Color.FromArgb(255, 230, 49, 65));
			gaugeBrushes.Add(new GaugeNamedBrush
			{
				Brush = dimRed,
				Name = "DimRed"
			});

			LowSectorColorCombo.ItemsSource = gaugeBrushes;
			MiddleSectorColorCombo.ItemsSource = gaugeBrushes;
			HighSectorColorCombo.ItemsSource = gaugeBrushes;

			LowSectorColorCombo.SelectedValue = dimGreen;
			MiddleSectorColorCombo.SelectedValue = dimYellow;
			HighSectorColorCombo.SelectedValue = dimRed;
		}
	}

	public class GaugeNamedBrush
	{
		#region Properties

		public Brush Brush { get; set; }
		public string Name { get; set; }

		#endregion
	}
}