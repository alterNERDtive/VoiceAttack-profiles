using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace alterNERDtive
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        private struct Setting
        {
            public string Profile { get; }
            public dynamic Option { get; }
            public dynamic Value { get; }
            public dynamic UiElement { get; }

            public Setting(string profile, dynamic option, dynamic value, dynamic uiElement)
                => (Profile, Option, Value, UiElement) = (profile, option, value, uiElement);
        }

        private List<Setting> values = new List<Setting>();
        private util.Configuration config;
        private util.VoiceAttackLog log;

        public SettingsDialog(util.Configuration config, util.VoiceAttackLog log)
        {
            InitializeComponent();

            this.config = config;
            this.log = log;

            foreach (TabItem tab in tabs.Items)
            {
                string profile = tab.Name;
                if (profile == "general")
                {
                    profile = "alterNERDtive-base";
                }
                
                tab.IsEnabled = BasePlugin.IsProfileActive(profile);

                StackPanel panel = new StackPanel();
                util.Configuration.OptDict<string, util.Configuration.Option> options = config.GetOptions(profile);

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
                        values.Add(new Setting(profile, option, value, checkBox));

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
                        values.Add(new Setting(profile, option, value, input));

                        panel.Children.Add(row);
                    }
                }

                tab.Content = panel;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
            log.Log("Settings dialog cancelled.", util.LogLevel.DEBUG);
        }

        private void okButton_Click(object sender, RoutedEventArgs reargs)
        {
            bool error = false;

            foreach (Setting setting in values)
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
                        log.Log($@"Configuration changed via settings dialog: ""{setting.Profile}.{setting.Option.Name}"" → ""{state}""", util.LogLevel.DEBUG);
                        config.SetConfig(setting.Profile, setting.Option.Name, state);
                    }
                }
                catch (Exception e) when (e is ArgumentNullException || e is FormatException || e is OverflowException)
                {
                    log.Log($@"Invalid value for ""{setting.Profile}.{setting.Option.Name}"": ""{((TextBox)setting.UiElement).Text}""", util.LogLevel.ERROR);
                    error = true;
                }
            }

            if (!error)
            {
                Window.GetWindow(this).Close();
            }
        }
    }
}
