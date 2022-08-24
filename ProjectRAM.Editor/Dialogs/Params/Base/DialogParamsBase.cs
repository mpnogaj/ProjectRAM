using Avalonia.Controls;

namespace ProjectRAM.Editor.Dialogs.Params.Base
{
    internal abstract class DialogParamsBase
    {
        public WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.CenterOwner;
        public double Width { get; set; } = double.NaN;
        public double Height { get; set; } = double.NaN;
        public double MinWidth { get; set; } = 200;
        public double MinHeight { get; set; } = 100;
        public double MaxWidth { get; set; } = double.PositiveInfinity;
        public double MaxHeight { get; set; } = double.PositiveInfinity;
        public SizeToContent SizeToContent { get; set; } = SizeToContent.WidthAndHeight;
        public bool CanResize { get; set; } = false;
    }
}
