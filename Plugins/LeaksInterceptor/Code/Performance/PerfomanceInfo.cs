﻿using System.Diagnostics;
using System.Windows.Media;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;
using Mnk.Library.WpfControls.Components.Drawings.Graphics.Painters;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Performance
{
    class PerformanceInfo
    {
        public IGraphic Graphic { get; private set; }
        public PerformanceCounter Getter { get; private set; }

        public PerformanceInfo(PerformanceCounter getter)
        {
            Getter = getter;
            Graphic = new Graphic(new Pen(Brushes.DarkGreen, 1), new PolylinesStretchGraphicPainter());
        }
    }
}
