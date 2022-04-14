using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Mnk.TBox.Plugins.NUnitRunner.Code;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    class TestResultToVisibilityRevertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!TestsStateSingleton.IsRunning(value as Result)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
