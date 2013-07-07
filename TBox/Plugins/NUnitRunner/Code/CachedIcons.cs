using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    // fix for multithread issue
    static class CachedIcons
    {
        public static Image Spinner { get; private set; }
        public static IDictionary<string, ImageSource> Icons { get; private set; }

        static CachedIcons()
        {
            Icons = new Dictionary<string, ImageSource>();

            Spinner = new Image();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Mnk.TBox.Plugins.NUnitRunner;Component/Resources/spinner.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(Spinner, image);

        }
    }
}
