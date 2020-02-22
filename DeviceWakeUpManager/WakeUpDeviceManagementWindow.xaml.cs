/*
 * MIT License
 * 
 * Copyright (c) 2020 SamyLearningNote
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * GitHub:
 * https://github.com/SamyLearningNote
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Navigation;

namespace DeviceWakeUpManager
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // create a command function object
        commandFunctions cmd = new commandFunctions();
        // create common function object
        CommonSet commonSet = new CommonSet();

        // define index value of languages
        const int ENGLISH_INDEX = 0;

        int resultSize = 0;
        public MainWindow()
        {
            initComponent();
        }

        public MainWindow(int flag)
        {
            // do not load the information
            if (flag == 0)
            {

            }
        }

        public void initComponent()
        {
            InitializeComponent();

            GetDeviceList();

            InitUIElement();
        }

        private void InitUIElement()
        {

            // Show the message to tell the user the command is loading
            LoadingMessage lm = new LoadingMessage();
            lm.Show();
            commonSet.LoadConfiguration();
            lm.Close();

            // set language
            if (commonSet.loadedLanguage.ToLower().Equals("english"))
            {
                LanguageComboBox.SelectedIndex = ENGLISH_INDEX;
            }
            // set autostart
            if (commonSet.loadedAutoStart.ToLower().Equals("true"))
            {
                AutoStartCheckBox.IsChecked = true;
            }
            else
            {
                AutoStartCheckBox.IsChecked = false;
            }
            // set auto disable
            if (commonSet.loadedAutoDisableAll.ToLower().Equals("true"))
            {
                AutoDisableAllCheckBox.IsChecked = true;
            }
            else
            {
                AutoDisableAllCheckBox.IsChecked = false;
            }
            if (commonSet.loadedAutoDisableMouse.ToLower().Equals("true"))
            {
                AutoDisableMouseCheckBox.IsChecked = true;
            }
            else
            {
                AutoDisableMouseCheckBox.IsChecked = false;
            }
            if (commonSet.loadedAutoDisableKeyboard.ToLower().Equals("true"))
            {
                AutoDisableKeyboardCheckBox.IsChecked = true;
            }
            else
            {
                AutoDisableKeyboardCheckBox.IsChecked = false;
            }
            // set auto show window
            if (commonSet.loadedAutoShowWindow.ToLower().Equals("true"))
            {
                ShowWindowCheckBox.IsChecked = true;
            }
            else
            {
                ShowWindowCheckBox.IsChecked = false;
            }

            // put the value into variable
            commonSet.language = commonSet.loadedLanguage;
            commonSet.autoStart = commonSet.loadedAutoStart;
            commonSet.autoDisableAll = commonSet.loadedAutoDisableAll;
            commonSet.autoDisableMouse = commonSet.loadedAutoDisableMouse;
            commonSet.autoDisableKeyboard = commonSet.loadedAutoDisableKeyboard;
            commonSet.autoShowWindow = commonSet.loadedAutoShowWindow;

            commonSet.SaveConfiguration();
        }

        private String[] GetDevicesStatus(String[] enabledDevices, String[] targetDevices)
        {
            String[] deviceStatus = (String[]) targetDevices.Clone();
            for (int i = 0; i < targetDevices.Length; i++)
            {
                // set the default status as disable
                deviceStatus[i] = "[Disabled]";
                for (int j = 0; j < enabledDevices.Length; j++)
                {
                    if (targetDevices[i].Equals(enabledDevices[j]))
                    {
                        // set the status as enabled if the device name can be found in enabled list
                        deviceStatus[i] = "[Enabled]";
                        break;
                    }
                }
            }

            return deviceStatus;
        }

        private void GetDeviceList()
        {
            String[] result;
            if(CurrentDeviceRadioButton.IsChecked == true)
            {
                result = cmd.ProcessResult(cmd.RunCommandWithLoadingMessage("powercfg -devicequery wake_armed"));
            }
            else if (AllDeviceRadioButton.IsChecked == true)
            {
                result = cmd.ProcessResult(cmd.RunCommandWithLoadingMessage("powercfg -devicequery wake_programmable"));
            }
            else if (MouseDeviceRadioButton.IsChecked == true)
            {
                result = cmd.ProcessResult(cmd.RunCommandWithLoadingMessage("powercfg -devicequery wake_programmable"));
                result = commonSet.GetStringsWithWord(result, "mouse");
            }
            // it is the case of keyboard radio button is checked
            else
            {
                result = cmd.ProcessResult(cmd.RunCommandWithLoadingMessage("powercfg -devicequery wake_programmable"));
                result = commonSet.GetStringsWithWord(result, "keyboard");
            }

            // Get the device status
            String[] enabledDeviceList = cmd.ProcessResult(cmd.RunCommandWithLoadingMessage("powercfg -devicequery wake_armed"));
            String[] deviceStatusList = GetDevicesStatus(enabledDeviceList, result);

            // put the result into check box
            resultSize = result.Length;
            String checkBoxName;
            for (int i = 0; i < result.Length; i++)
            {
                CheckBox cb = new CheckBox();
                cb.Content = result[i] + deviceStatusList[i];
                // set the name of the checkbox
                checkBoxName = "cb" + i;
                this.RegisterName(checkBoxName, cb);
                cb.Height = 40;
                // center the checkbox
                cb.VerticalAlignment = VerticalAlignment.Center;
                // add the check box into the list
                PossibleWakeUpDeviceList.Items.Add(cb); 
                 
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < resultSize; i++)
            {
                String checkBoxName = "cb" + i;
                var deviceCheckBox = (CheckBox)this.FindName(checkBoxName);
                deviceCheckBox.IsChecked = true;
            }
        }

        private void DeSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < resultSize; i++)
            {
                String checkBoxName = "cb" + i;
                var deviceCheckBox = (CheckBox)this.FindName(checkBoxName);
                deviceCheckBox.IsChecked = false;
            }
        }

        private void InvertSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < resultSize; i++)
            {
                String checkBoxName = "cb" + i;
                var deviceCheckBox = (CheckBox)this.FindName(checkBoxName);
                if (deviceCheckBox.IsChecked == true)
                {
                    deviceCheckBox.IsChecked = false;
                }
                else
                {
                    deviceCheckBox.IsChecked = true;
                }
            }

        }

        private void DisableCurrentWakeUpDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to disable the wake up function of selected devices?",
                "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            // disable the wake up function if the user clicked "OK"
            if (result.Equals(MessageBoxResult.OK))
            {
                for (int i = 0; i < resultSize; i++)
                {
                    String checkBoxName = "cb" + i;
                    var deviceCheckBox = (CheckBox)this.FindName(checkBoxName);
                    // disable the wake up function if the device is selected
                    if (deviceCheckBox.IsChecked == true)
                    {
                        String deviceName = deviceCheckBox.Content.ToString().Replace("[Enabled]", "");
                        deviceName = deviceName.Replace("[Disabled]", "");
                        cmd.RunCommandWithLoadingMessage("powercfg -devicedisablewake \"" + deviceName + "\"");
                    }
                }
                MessageBox.Show("Wake up function of selected devices is disabled");
                RefreshDeviceList();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDeviceList();
        }

        // define a function to refresh the device list
        void RefreshDeviceList()
        {
            // Unregister all the checkbox before get the list

            for (int i = 0; i < resultSize; i++)
            {
                String checkBoxName = "cb" + i;
                var deviceCheckBox = (CheckBox)this.FindName(checkBoxName);
                PossibleWakeUpDeviceList.Items.Remove(deviceCheckBox);
                this.UnregisterName(checkBoxName);
            }


            GetDeviceList();
        }

        private void CurrentDeviceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)this.FindName("CurrentDeviceRadioButton");
            if(rb != null)
            {
                CurrentDeviceRadioButton.IsChecked = true;
            }
            rb = (RadioButton)this.FindName("MouseDeviceRadioButton");
            if (rb != null)
            {
                MouseDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("AllDeviceRadioButton");
            if (rb != null)
            {
                AllDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("KeyboardDeviceRadioButton");
            if (rb != null)
            {
                KeyboardDeviceRadioButton.IsChecked = false;
            }

            // if the list is not rendered yet, do not get the list information
            var lb = (ListBox)this.FindName("PossibleWakeUpDeviceList");
            if (lb != null)
            {
                RefreshDeviceList();
            }
        }

        private void MouseDeviceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)this.FindName("CurrentDeviceRadioButton");
            if (rb != null)
            {
                CurrentDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("MouseDeviceRadioButton");
            if (rb != null)
            {
                MouseDeviceRadioButton.IsChecked = true;
            }
            rb = (RadioButton)this.FindName("AllDeviceRadioButton");
            if (rb != null)
            {
                AllDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("KeyboardDeviceRadioButton");
            if (rb != null)
            {
                KeyboardDeviceRadioButton.IsChecked = false;
            }

            // if the list is not rendered yet, do not get the list information
            var lb = (ListBox)this.FindName("PossibleWakeUpDeviceList");
            if (lb != null)
            {
                RefreshDeviceList();
            }
        }

        private void AllDeviceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)this.FindName("CurrentDeviceRadioButton");
            if (rb != null)
            {
                CurrentDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("MouseDeviceRadioButton");
            if (rb != null)
            {
                MouseDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("AllDeviceRadioButton");
            if (rb != null)
            {
                AllDeviceRadioButton.IsChecked = true;
            }
            rb = (RadioButton)this.FindName("KeyboardDeviceRadioButton");
            if (rb != null)
            {
                KeyboardDeviceRadioButton.IsChecked = false;
            }

            // if the list is not rendered yet, do not get the list information
            var lb = (ListBox)this.FindName("PossibleWakeUpDeviceList");
            if (lb != null)
            {
                RefreshDeviceList();
            }
        }

        private void KeyboardDeviceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)this.FindName("CurrentDeviceRadioButton");
            if (rb != null)
            {
                CurrentDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("MouseDeviceRadioButton");
            if (rb != null)
            {
                MouseDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("AllDeviceRadioButton");
            if (rb != null)
            {
                AllDeviceRadioButton.IsChecked = false;
            }
            rb = (RadioButton)this.FindName("KeyboardDeviceRadioButton");
            if (rb != null)
            {
                KeyboardDeviceRadioButton.IsChecked = true;
            }

            // if the list is not rendered yet, do not get the list information
            var lb = (ListBox)this.FindName("PossibleWakeUpDeviceList");
            if (lb != null)
            {
                RefreshDeviceList();
            }
        }


        private void EnableWakeUpDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to enable the wake up function of selected devices?",
                "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            // disable the wake up function if the user clicked "OK"
            if (result.Equals(MessageBoxResult.OK))
            {
                for (int i = 0; i < resultSize; i++)
                {
                    String checkBoxName = "cb" + i;
                    var deviceCheckBox = (CheckBox)this.FindName(checkBoxName);
                    // disable the wake up function if the device is selected
                    if (deviceCheckBox.IsChecked == true)
                    {
                        String deviceName = deviceCheckBox.Content.ToString().Replace("[Enabled]", "");
                        deviceName = deviceName.Replace("[Disabled]", "");
                        cmd.RunCommandWithLoadingMessage("powercfg -deviceenablewake \"" + deviceName + "\"");
                    }
                }
                MessageBox.Show("Wake up function of selected devices is enabled");
                RefreshDeviceList();
            }

        }

        // Reference:
        // https://stackoverflow.com/questions/10238694/example-using-hyperlink-in-wpf
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void AutoStartCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            commonSet.autoStart = "true";
            commonSet.SaveConfiguration();
            commonSet.ApplyAutoStartSetting(true);
        }

        private void AutoStartCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            commonSet.autoStart = "false";
            commonSet.SaveConfiguration();
            commonSet.ApplyAutoStartSetting(false);

        }

        private void AutoDisableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableAll = "true";
            commonSet.SaveConfiguration();
        }

        private void AutoDisableCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableAll = "false";
            commonSet.SaveConfiguration();
        }

        private void ShowWindowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            commonSet.autoShowWindow = "true";
            commonSet.SaveConfiguration();
        }

        private void ShowWindowCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            commonSet.autoShowWindow = "false";
            commonSet.SaveConfiguration();
        }

        private void AutoDisableAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableAll = "true";
            commonSet.SaveConfiguration();
            // also check the disable mouse and keyboard checkbox
            AutoDisableMouseCheckBox.IsChecked = true;
            AutoDisableKeyboardCheckBox.IsChecked = true;
        }

        private void AutoDisableAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableAll = "false";
            commonSet.SaveConfiguration();
        }

        private void AutoDisableMouseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableMouse = "true";
            commonSet.SaveConfiguration();
        }

        private void AutoDisableMouseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableMouse = "false";
            commonSet.SaveConfiguration();
            // also uncheck the disable all checkbox
            AutoDisableAllCheckBox.IsChecked = false;
        }

        private void AutoDisableKeyboardCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableKeyboard = "true";
            commonSet.SaveConfiguration();
        }

        private void AutoDisableKeyboardCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            commonSet.autoDisableKeyboard = "false";
            commonSet.SaveConfiguration();
            // also uncheck the disable all checkbox
            AutoDisableAllCheckBox.IsChecked = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("The program is running in taskbar\nIf you want to exit the program, please exit the program by right-click the taskbar icon");
        }
    }
}
