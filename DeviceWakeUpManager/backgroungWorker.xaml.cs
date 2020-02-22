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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DeviceWakeUpManager;

namespace DeviceWakeUpManager
{
    /// <summary>
    /// backgroungWorker.xaml 的互動邏輯
    /// </summary>
    public partial class backgroungWorker : Window
    {
        MainWindow mainWindow = new MainWindow(0);
        NotifyIcon nIcon = new NotifyIcon();
        System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
        CommonSet commonSet = new CommonSet();
        commandFunctions cmd = new commandFunctions();
        public backgroungWorker()
        {
            System.Windows.Forms.MessageBox.Show("001");
            this.Width = 0;
            this.Height = 0;
            this.WindowStyle = WindowStyle.None;
            this.ShowInTaskbar = false;
            this.ShowActivated = false;
            //InitializeComponent()
            System.Windows.Forms.MessageBox.Show("002"); ;

            // init context menu
            System.Windows.Forms.MenuItem mwMenuItem = new System.Windows.Forms.MenuItem();
            mwMenuItem.Text = "Main Window";
            cm.MenuItems.Add(mwMenuItem);
            mwMenuItem.Click += mwMenuItem_Click;
            System.Windows.Forms.MenuItem eMenuItem = new System.Windows.Forms.MenuItem();
            eMenuItem.Text = "Exit";
            cm.MenuItems.Add(eMenuItem);
            eMenuItem.Click += eMenuItem_Click;
            System.Windows.Forms.MessageBox.Show("003");

            // init the notify icon
            //ComponentResourceManager resources = new ComponentResourceManager(typeof(backgroungWorker));
            nIcon.Icon = new Icon(System.AppDomain.CurrentDomain.BaseDirectory + "icon.ico");
            nIcon.Visible = true;
            nIcon.ContextMenu = cm;

            System.Windows.Forms.MessageBox.Show("004");
            // load the configuration
            commonSet.LoadConfiguration();
            CheckNeedToShowWindow();

            System.Windows.Forms.MessageBox.Show("005");
            // register auto start
            AutoRunRegister();

            // use the auto disable function
            AutoDisable();

            System.Windows.Forms.MessageBox.Show("006");
        }

        public void AutoDisable()
        {
            string[] enabledList;
            string[] enabledMouseList;
            string[] enabledKeyboardList;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    commonSet.LoadConfiguration();
                    // get the list of all enabled device
                    enabledList = cmd.ProcessResult(cmd.RunCommand("powercfg -devicequery wake_armed", 1));
                    if (commonSet.loadedAutoDisableAll.ToLower().Equals("true"))
                    {
                        // disable all the enabled devices
                        for (int i = 0; i < enabledList.Length; i++)
                        {
                            cmd.RunCommand("powercfg -devicedisablewake \"" + enabledList[i] + "\"", 1);
                        }
                    }
                    // check for mouse and keyboard disable
                    else
                    {
                        if (commonSet.loadedAutoDisableMouse.ToLower().Equals("true"))
                        {
                            // get the list of all enabled mouse
                            enabledMouseList = commonSet.GetStringsWithWord(enabledList, "mouse");
                            // disable all the enabled mouses
                            for (int i = 0; i < enabledMouseList.Length; i++)
                            {
                                cmd.RunCommand("powercfg -devicedisablewake \"" + enabledMouseList[i] + "\"", 1);
                            }
                        }
                        if (commonSet.loadedAutoDisableKeyboard.ToLower().Equals("true"))
                        {
                            // get the list of all enabled mouse
                            enabledKeyboardList = commonSet.GetStringsWithWord(enabledList, "keyboard");
                            // disable all the enabled keyboard
                            for (int i = 0; i < enabledKeyboardList.Length; i++)
                            {
                                cmd.RunCommand("powercfg -devicedisablewake \"" + enabledKeyboardList[i] + "\"", 1);
                            }
                        }
                    }
                    Thread.Sleep(10000);
                }
            });
        }
        
        public void AutoRunRegister()
        {
            if (commonSet.loadedAutoStart.ToLower().Equals("true"))
            {
                commonSet.ApplyAutoStartSetting(true);
            }
            else
            {
                commonSet.ApplyAutoStartSetting(false);
            }
        }

        public void CheckNeedToShowWindow()
        {
            if (commonSet.loadedAutoShowWindow.ToLower().Equals("true"))
            {
                showMainWindow();
            }
        }

        private void eMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void mwMenuItem_Click(object sender, EventArgs e)
        {
            showMainWindow();
        }

        public void showMainWindow()
        {
            mainWindow.Close();
            mainWindow = new MainWindow(0);
            mainWindow.initComponent();
            mainWindow.Show();
        }
        
    }
}
