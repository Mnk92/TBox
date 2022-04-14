using System.Windows.Media;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;
using Mnk.Library.WpfControls.Components.Drawings.Graphics.Painters;
using Mnk.TBox.Plugins.LeaksInterceptor.Code.Getters;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Standard
{
    class CounterInfo
    {
        public IGraphic Graphic { get; private set; }
        public IGetter Getter { get; private set; }

        public CounterInfo(IGetter getter)
        {
            Getter = getter;
            Graphic = new Graphic(new Pen(Brushes.DarkBlue, 1), new PolylinesStretchGraphicPainter());
        }
    }
}
