using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Mnk.TBox.Plugins.NUnitRunner.Code;
using NUnit.Core;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    [ValueConversion(typeof(ResultState), typeof(string))]
    class ResultStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetIcon(value);
        }

        private static ImageSource GetIcon(object value)
        {
            var name = value.ToString();
            switch ((ResultState)value)
            {
                case ResultState.Error:
                    name = "Failure";
                    break;
                case ResultState.Cancelled:
                case ResultState.NotRunnable:
                    name = "Skipped";
                    break;
            }
            return CachedIcons.Icons[name];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
