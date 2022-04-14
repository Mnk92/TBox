using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Mnk.ParallelTests.Contracts;
using Mnk.TBox.Plugins.NUnitRunner.Code;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    [ValueConversion(typeof(ResultMessage.Types.TestResultState), typeof(string))]
    class ResultStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetIcon(value);
        }

        private static ImageSource GetIcon(object value)
        {
            var name = value.ToString();
            switch ((ResultMessage.Types.TestResultState)value)
            {
                case ResultMessage.Types.TestResultState.Error:
                    name = "Failure";
                    break;
                case ResultMessage.Types.TestResultState.Cancelled:
                case ResultMessage.Types.TestResultState.NotRunnable:
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
