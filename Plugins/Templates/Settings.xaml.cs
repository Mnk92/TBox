﻿using System.Windows.Controls;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.Templates
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ISettings
    {
        public Settings()
        {
            InitializeComponent();
        }

        public UserControl Control => this;
    }
}
