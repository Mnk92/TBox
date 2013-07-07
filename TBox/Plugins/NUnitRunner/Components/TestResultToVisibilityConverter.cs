using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Mnk.Library.ParallelNUnit.Core;
using Mnk.TBox.Plugins.NUnitRunner.Code;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    public class TestResultToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TestsStateSingleton.IsRunning((Result)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
