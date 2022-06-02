// <copyright file="SettingsDialog.xaml.cs" company="alterNERDtive">
// Copyright 2019–2022 alterNERDtive.
//
// This file is part of alterNERDtive VoiceAttack profiles for Elite Dangerous.
//
// alterNERDtive VoiceAttack profiles for Elite Dangerous is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// alterNERDtive VoiceAttack profiles for Elite Dangerous is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with alterNERDtive VoiceAttack profiles for Elite Dangerous.  If not, see &lt;https://www.gnu.org/licenses/&gt;.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace alterNERDtive
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml.
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        private readonly List<Setting> values = new List<Setting>();
        private util.Configuration config;
        private util.VoiceAttackLog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDialog"/> class.
        /// </summary>
        /// <param name="config">The plugin Configuration.</param>
        /// <param name="log">The plugin Log.</param>
        public SettingsDialog(util.Configuration config, util.VoiceAttackLog log)
        {
            this.InitializeComponent();

            this.config = config;
            this.log = log;

            foreach (TabItem tab in this.tabs.Items)
            {
                string profile = tab.Name;
                if (profile == "general")
                {
                    profile = "alterNERDtive-base";
                }

                tab.IsEnabled = BasePlugin.IsProfileActive(profile);

                StackPanel panel = new StackPanel();
                util.Configuration.OptDict<string, util.Configuration.Option> options = util.Configuration.GetOptions(profile);

                foreach (dynamic option in options.Values)
                {
                    dynamic value = config.GetConfig(profile, option.Name);

                    if (option is util.Configuration.Option<bool>)
                    {
                        WrapPanel row = new WrapPanel();

                        CheckBox checkBox = new CheckBox();
                        checkBox.IsChecked = value;
                        checkBox.VerticalAlignment = VerticalAlignment.Center;
                        row.Children.Add(checkBox);
                        this.values.Add(new Setting(profile, option, value, checkBox));

                        Label label = new Label();
                        label.Content = option.Description;
                        row.Children.Add(label);

                        panel.Children.Add(row);
                    }
                    else
                    {
                        StackPanel row = new StackPanel();

                        Label label = new Label();
                        label.Content = option.Description;
                        row.Children.Add(label);

                        TextBox input = new TextBox();
                        input.Text = value.ToString();
                        row.Children.Add(input);
                        this.values.Add(new Setting(profile, option, value, input));

                        panel.Children.Add(row);
                    }
                }

                tab.Content = panel;
            }
        }

        private bool ApplySettings()
        {
            bool success = true;

            foreach (Setting setting in this.values)
            {
                dynamic state = null;

                try
                {
                    if (setting.Option is util.Configuration.Option<bool>)
                    {
                        state = ((CheckBox)setting.UiElement).IsChecked ?? false;
                    }
                    else if (setting.Option is util.Configuration.Option<DateTime>)
                    {
                        state = DateTime.Parse(((TextBox)setting.UiElement).Text);
                    }
                    else if (setting.Option is util.Configuration.Option<decimal>)
                    {
                        state = decimal.Parse(((TextBox)setting.UiElement).Text);
                    }
                    else if (setting.Option is util.Configuration.Option<int>)
                    {
                        state = int.Parse(((TextBox)setting.UiElement).Text);
                    }
                    else if (setting.Option is util.Configuration.Option<short>)
                    {
                        state = short.Parse(((TextBox)setting.UiElement).Text);
                    }
                    else if (setting.Option is util.Configuration.Option<string>)
                    {
                        state = ((TextBox)setting.UiElement).Text;
                    }

                    if (state != setting.Value)
                    {
                        this.log.Log($@"Configuration changed via settings dialog: ""{setting.Profile}.{setting.Option.Name}"" → ""{state}""", util.LogLevel.DEBUG);
                        this.config.SetConfig(setting.Profile, setting.Option.Name, state);
                    }
                }
                catch (Exception e) when (e is ArgumentNullException || e is FormatException || e is OverflowException)
                {
                    this.log.Log($@"Invalid value for ""{setting.Profile}.{setting.Option.Name}"": ""{((TextBox)setting.UiElement).Text}""", util.LogLevel.ERROR);
                    success = false;
                }
            }

            return success;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
            this.log.Log("Settings dialog cancelled.", util.LogLevel.DEBUG);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ApplySettings())
            {
                Window.GetWindow(this).Close();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs reeargs)
        {
            this.ApplySettings();
        }

        private struct Setting
        {
            public Setting(string profile, dynamic option, dynamic value, dynamic uiElement)
                => (this.Profile, this.Option, this.Value, this.UiElement) = (profile, option, value, uiElement);

            public string Profile { get; }

            public dynamic Option { get; }

            public dynamic Value { get; }

            public dynamic UiElement { get; }
        }
    }
}
