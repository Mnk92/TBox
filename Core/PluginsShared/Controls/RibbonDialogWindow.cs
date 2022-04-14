using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace Mnk.TBox.Core.PluginsShared.Controls
{
    public class RibbonDialogWindow : RibbonWindow, IDisposable
    {
        protected bool CanClose { get; set; }

        public RibbonDialogWindow()
        {
            CanClose = false;
            ShowActivated = true;
            ShowInTaskbar = false;
            SnapsToDevicePixels = true;
            SetResourceReference(BackgroundProperty, SystemColors.ControlBrushKey);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CanClose)
            {
                e.Cancel = true;
                Hide();
            }
            base.OnClosing(e);
        }

        protected virtual void OnShow()
        {

        }

        public void SafeShowDialog()
        {
            if (!IsVisible)
            {
                OnShow();
                ShowDialog();
            }
            SafeActivate();
        }

        public void ShowAndActivate()
        {
            if (!IsVisible)
            {
                OnShow();
                Show();
            }
            SafeActivate();
        }

        private void SafeActivate()
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            Activate();
        }

        public virtual void Dispose()
        {
            CanClose = true;
            Close();
        }
    }

}
