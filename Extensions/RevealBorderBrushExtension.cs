using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace CalculaterApp.Extensions
{
    public class RevealBorderBrushExtension : MarkupExtension
    {
        public Color FallbackColor { get; set; } = Colors.White;
        public Color Color { get; set; } = Colors.White;
        public Transform Transform { get; set; } = Transform.Identity;
        public Transform RelativeTransform { get; set; } = Transform.Identity;
        public double Opacity { get; set; } = 1.0;
        public double Radius { get; set; } = 100.0;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)) return null;
            if (service.TargetObject.ToString().EndsWith("SharedDp")) return this;
            if (!(service.TargetObject is FrameworkElement element)) return this;
            if (DesignerProperties.GetIsInDesignMode(element)) return new SolidColorBrush(FallbackColor);

            var window = Window.GetWindow(element);
            if (window == null) return this;
            var brush = CreateBrush(window, element);
            return brush;
        }

        private Brush CreateBrush(Window window, FrameworkElement element)
        {
            var brush = new RadialGradientBrush(Colors.White, Colors.Transparent)
            {
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = Radius,
                RadiusY = Radius,
                Opacity = Opacity,
                Transform = Transform,
                RelativeTransform = RelativeTransform,
            };
            window.MouseMove += OnMouseMove;
            window.Closed += OnClosed;
            return brush;

            void OnMouseMove(object sender, MouseEventArgs e)
            {
                var position = e.GetPosition(element);
                brush.GradientOrigin = position;
                brush.Center = position;
            }

            void OnClosed(object o, EventArgs eventArgs)
            {
                window.MouseMove -= OnMouseMove;
                window.Closed -= OnClosed;
            }
        }
    }
}
