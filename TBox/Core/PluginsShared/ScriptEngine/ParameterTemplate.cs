using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.WpfControls.Code.Content;
using Mnk.TBox.Locales.Localization.PluginsShared;
using Mnk.Library.ScriptEngine.Core.Params;
using Mnk.Library.WpfControls.Code.Dialogs;
using Mnk.Library.WpfControls.Components;
using Mnk.Library.WpfControls.Components.Captioned;
using Mnk.Library.WpfControls.Components.DropDown;
using Mnk.Library.WpfControls.Components.FilesAndFolders;
using Mnk.Library.WpfControls.Components.Units;
using Mnk.Library.WpfControls.Tools;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Core.PluginsShared.ScriptEngine
{
    public sealed class ParameterTemplate : UserControl
    {
        private Window Instance { get { return this.GetParentWindow(); } }
        public ParameterTemplate()
        {
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var parameter = DataContext as Parameter;
            Content = (parameter == null) ? null : CreateChildrenByParameter((dynamic)parameter);
        }

        private static UIElement CreateChildrenByParameter(BoolParameter parameter)
        {
            var el = new CheckBox{Focusable = false};
            el.SetBinding(ToggleButton.IsCheckedProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(StringParameter parameter)
        {
            var el = new TextBox { };
            el.SetBinding(TextBox.TextProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(PasswordParameter parameter)
        {
            var el = new CaptionedPasswordBox {};
            el.SetBinding(CaptionedPasswordBox.ValueProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(GuidParameter parameter)
        {
            var el = new TextBox { };
            el.SetBinding(TextBox.TextProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(IntegerParameter parameter)
        {
            var el = new NumericUpDown { Minimum = parameter.Min, Maximum = parameter.Max };
            el.SetBinding(NumericUpDown.ValueProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(DoubleParameter parameter)
        {
            var el = new DoubleUpDown { Minimum = parameter.Min, Maximum = parameter.Max };
            el.SetBinding(DoubleUpDown.ValueProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(FileParameter parameter)
        {
            var el = new EditPath { PathGetterType = PathGetterType.File, Focusable = false };
            el.SetBinding(EditPath.ValueProperty, new Binding("Value"));
            return el;
        }

        private static UIElement CreateChildrenByParameter(DirectoryParameter parameter)
        {
            var el = new EditPath { PathGetterType = PathGetterType.Folder, Focusable = false };
            el.SetBinding(EditPath.ValueProperty, new Binding("Value"));
            return el;
        }

        private UIElement CreateChildrenByParameter(StringListParameter parameter)
        {
            var el = new CheckableListBoxUnit { CustomDataManager = new StringValueDataManager<CheckableData<string>>(), Focusable = false };
            PrepareListBoxItemTemplate(el, parameter);
            el.ConfigureInputText(PluginsSharedLang.ConfigureString, parameter.Values, Mnk.Library.WpfControls.Code.Dialogs.Templates.Default, x => ParametersValidator.ValidateValue(parameter, x) && parameter.Values.All(y => !y.Value.EqualsIgnoreCase(x)), Instance);
            return el;
        }

        private UIElement CreateChildrenByParameter(GuidListParameter parameter)
        {
            var el = new CheckableListBoxUnit { CustomDataManager = new GuidValueDataManager<CheckableData<Guid>>(), Focusable = false };
            PrepareListBoxItemTemplate(el, parameter);
            Guid tmp;
            el.ConfigureInputText(PluginsSharedLang.ConfigureGuid, parameter.Values, Mnk.Library.WpfControls.Code.Dialogs.Templates.Default, x => Guid.TryParse(x, out tmp) && ParametersValidator.ValidateValue(parameter, tmp) && parameter.Values.All(y => y.Value != tmp), Instance);
            return el;
        }

        private UIElement CreateChildrenByParameter(IntegerListParameter parameter)
        {
            var el = new CheckableListBoxUnit { CustomDataManager = new IntValueDataManager<CheckableData<int>>(), Focusable = false };
            PrepareListBoxItemTemplate(el, parameter);
            int tmp;
            el.ConfigureInputText(PluginsSharedLang.ConfigureInt, parameter.Values, Mnk.Library.WpfControls.Code.Dialogs.Templates.Default, x => int.TryParse(x, out tmp) && ParametersValidator.ValidateValue(parameter, tmp) && parameter.Values.All(y => y.Value != tmp), Instance);
            return el;
        }

        private UIElement CreateChildrenByParameter(DoubleListParameter parameter)
        {
            var el = new CheckableListBoxUnit { CustomDataManager = new DoubleValueDataManager<CheckableData<double>>(), Focusable = false };
            PrepareListBoxItemTemplate(el, parameter);
            double tmp;
            el.ConfigureInputText(PluginsSharedLang.ConfigureDouble, parameter.Values, Mnk.Library.WpfControls.Code.Dialogs.Templates.Default, x => double.TryParse(x, out tmp) && ParametersValidator.ValidateValue(parameter, tmp) && parameter.Values.All(y => y.Value != tmp), Instance);
            return el;
        }

        private UIElement CreateChildrenByParameter(FileListParameter parameter)
        {
            var el = new CheckableListBoxUnit { CustomDataManager = new StringValueDataManager<CheckableData<string>>(), Focusable = false };
            PrepareFileListBoxItemTemplate(el, parameter, PathGetterType.File);
            el.ConfigureInputFilePath(PluginsSharedLang.ConfigureFilePath, parameter.Values, PathTemplates.Default, x => ParametersValidator.ValidateValue(parameter, x) && parameter.Values.All(y => !y.Value.EqualsIgnoreCase(x)), Instance);
            return el;
        }

        private UIElement CreateChildrenByParameter(DirectoryListParameter parameter)
        {
            var el = new CheckableListBoxUnit { CustomDataManager = new StringValueDataManager<CheckableData<string>>(), Focusable = false };
            PrepareFileListBoxItemTemplate(el, parameter, PathGetterType.Folder);
            el.ConfigureInputFolderPath(PluginsSharedLang.ConfigureDirectoryPath, parameter.Values, PathTemplates.Default, x => ParametersValidator.ValidateValue(parameter, x) && parameter.Values.All(y => !y.Value.EqualsIgnoreCase(x)), Instance);
            return el;
        }

        private static void PrepareListBoxItemTemplate<T>(T el, Parameter parameter)
            where T : UserControl, ICheckableUnit
        {

            var tb = new FrameworkElementFactory(typeof(TextBlock));
            tb.SetBinding(TextBlock.TextProperty, new Binding("Value"));
            PrepareListBoxItemTemplate(el, parameter, tb);
        }

        private static void PrepareFileListBoxItemTemplate<T>(T el, Parameter parameter, PathGetterType type)
            where T : UserControl, ICheckableUnit
        {

            var tb = new FrameworkElementFactory(typeof(EditPath));
            tb.SetValue(EditPath.PathGetterTypeProperty, type);
            tb.SetBinding(EditPath.ValueProperty, new Binding("Value"));
            PrepareListBoxItemTemplate(el, parameter, tb);
        }

        private static void PrepareListBoxItemTemplate<T>(T el, Parameter parameter, FrameworkElementFactory tb)
            where T : UserControl, ICheckableUnit
        {
            var layout = new DataTemplate { DataType = parameter.GetType() };
            var factory = new FrameworkElementFactory(typeof(DockPanel));
            var cb = new FrameworkElementFactory(typeof(ExtCheckBox));
            cb.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            cb.SetValue(PaddingProperty, new Thickness(5, 0, 5, 0));
            cb.SetBinding(ToggleButton.IsCheckedProperty, new Binding("IsChecked"));
            var ev = new RoutedEventHandler(el.OnCheckChangedEvent);
            cb.AddHandler(ToggleButton.CheckedEvent, ev);
            cb.AddHandler(ToggleButton.UncheckedEvent, ev);
            factory.AppendChild(cb);
            tb.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            tb.SetValue(TextBlock.PaddingProperty, new Thickness(0));
            tb.SetBinding(IsEnabledProperty, new Binding("IsChecked"));
            factory.AppendChild(tb);
            layout.VisualTree = factory;
            el.ItemTemplate = layout;
        }

        private UIElement CreateChildrenByParameter<T>(DictionaryParameter<T> parameter)
        {
            var el = new CheckableListBoxUnit();
            PrepareDictionaryBoxItemTemplate(el, parameter);
            el.ConfigureInputText(PluginsSharedLang.ConfigureItem, parameter.Values, owner: Instance);
            return el;
        }

        private static void PrepareDictionaryBoxItemTemplate(CheckableListBoxUnit el, Parameter parameter)
        {
            var layout = new DataTemplate { DataType = parameter.GetType() };
            var factory = new FrameworkElementFactory(typeof(DockPanel));
            var cb = new FrameworkElementFactory(typeof(ExtCheckBox));
            cb.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            cb.SetValue(PaddingProperty, new Thickness(5, 0, 5, 0));
            cb.SetValue(MarginProperty, new Thickness(2));
            cb.SetBinding(ToggleButton.IsCheckedProperty, new Binding("IsChecked"));
            var ev = new RoutedEventHandler(el.OnCheckChangedEvent);
            cb.AddHandler(ToggleButton.CheckedEvent, ev);
            cb.AddHandler(ToggleButton.UncheckedEvent, ev);
            factory.AppendChild(cb);
            var tbv = CreateFrameworkElementFactory((dynamic)parameter);
            tbv.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            tbv.SetValue(PaddingProperty, new Thickness(0));
            tbv.SetValue(DockPanel.DockProperty, Dock.Bottom);
            tbv.SetBinding(IsEnabledProperty, new Binding("IsChecked"));
            factory.AppendChild(tbv);
            var tb = new FrameworkElementFactory(typeof(TextBlock));
            tb.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            tb.SetValue(TextBlock.PaddingProperty, new Thickness(0));
            tb.SetValue(TextBlock.FontWeightProperty, FontWeights.Heavy);
            tb.SetBinding(TextBlock.TextProperty, new Binding("Key"));
            factory.AppendChild(tb);
            layout.VisualTree = factory;
            el.ItemTemplate = layout;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IStringParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(TextBox));
            el.SetBinding(TextBox.TextProperty, new Binding("Value"));
            return el;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IBoolParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(CheckBox));
            el.SetBinding(ToggleButton.IsCheckedProperty, new Binding("IsChecked"));
            return el;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IGuidParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(TextBox));
            el.SetBinding(TextBox.TextProperty, new Binding("Value"));
            return el;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IFileParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(EditPath));
            el.SetBinding(EditPath.ValueProperty, new Binding("Value"));
            el.SetValue(EditPath.PathGetterTypeProperty, PathGetterType.File);
            return el;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IDirectoryParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(EditPath));
            el.SetBinding(EditPath.ValueProperty, new Binding("Value"));
            el.SetValue(EditPath.PathGetterTypeProperty, PathGetterType.Folder);
            return el;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IIntegerParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(NumericUpDown));
            el.SetValue(NumericUpDown.MinimumProperty, parameter.Min);
            el.SetValue(NumericUpDown.MaximumProperty, parameter.Max);
            el.SetBinding(NumericUpDown.ValueProperty, new Binding("Value"));
            return el;
        }

        private static FrameworkElementFactory CreateFrameworkElementFactory(IDoubleParameter parameter)
        {
            var el = new FrameworkElementFactory(typeof(DoubleUpDown));
            el.SetValue(DoubleUpDown.MinimumProperty, parameter.Min);
            el.SetValue(DoubleUpDown.MaximumProperty, parameter.Max);
            el.SetBinding(DoubleUpDown.ValueProperty, new Binding("Value"));
            return el;
        }
    }
}
