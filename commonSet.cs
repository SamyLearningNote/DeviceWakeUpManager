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

using Microsoft.Win32;
using System;
using System.Linq;
using System.Reflection;

public class CommonSet
{
	// define the variables for the configuration save load
	public string language;
	public string autoStart;
	public string autoDisableAll;
	public string autoDisableMouse;
	public string autoDisableKeyboard;
	public string autoShowWindow;

	public string loadedLanguage = "English";
	public string loadedAutoStart = "True";
	public string loadedAutoDisableAll = "False";
	public string loadedAutoDisableMouse = "False";
	public string loadedAutoDisableKeyboard = "False";
	public string loadedAutoShowWindow = "True";

	public string booterProgramExecutable = "DeviceWakeUpManager.exe";
	//public string MainWindowProgramExecutable = "DeviceWakeUpManager.exe";

	const int LANGUAGE_INDEX = 0;
	const int AUTOSTART_INDEX = 1;
	const int AUTODISABLE_INDEX = 2;

	string filePath = @"\configuration";

	public CommonSet()
	{

	}


	// start dpecified process
	public bool StartProcess(string targetExecutable)
	{
		try
		{

			System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + targetExecutable);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public void ApplyAutoStartSetting(bool autoStartChecked)
	{
		if (autoStartChecked)
		{
			try
			{
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				Assembly curAssembly = Assembly.GetExecutingAssembly();
				key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
			}
			catch { }

		}
		else
		{
			try
			{
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				Assembly curAssembly = Assembly.GetExecutingAssembly();
				key.DeleteValue(curAssembly.GetName().Name);
			}
			catch { }

		}
	}

	public bool SaveConfiguration()
	{
		try
		{
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + filePath, false))
			{
				// save the setting
				file.WriteLine("LANGUAGE " + language);
				file.WriteLine("AUTOSTART " + autoStart);
				file.WriteLine("AUTODISABLEALL " + autoDisableAll);
				file.WriteLine("AUTODISABLEMOUSE " + autoDisableMouse);
				file.WriteLine("AUTODISABLEKEYBOARD " + autoDisableKeyboard);
				file.WriteLine("AUTOSHOWWINDOW " + autoShowWindow);
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	public bool LoadConfiguration()
	{
		try
		{
			using (System.IO.StreamReader file = new System.IO.StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + filePath, false))
			{
				// load the setting
				string line;
				string[] content;
				while((line = file.ReadLine()) != null)
                {
					content = line.Split(' ');
					if (content.Length < 2)
                    {
						return false;
                    }
					// load language index
					if (content[0].Equals("LANGUAGE"))
                    {
						loadedLanguage = content[1];
                    }
					else if (content[0].Equals("AUTOSTART")){
						loadedAutoStart = content[1];
					}
					else if (content[0].Equals("AUTODISABLEALL"))
					{
						loadedAutoDisableAll = content[1];
					}
					else if (content[0].Equals("AUTODISABLEMOUSE"))
					{
						loadedAutoDisableMouse = content[1];
					}
					else if (content[0].Equals("AUTODISABLEKEYBOARD"))
					{
						loadedAutoDisableKeyboard = content[1];
					}
					else if (content[0].Equals("AUTOSHOWWINDOW"))
					{
						loadedAutoShowWindow = content[1];
					}
				}
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	// define a function to help get the targeted string array
	public string[] GetStringsWithWord(string[] stringArray, string word)
	{
		string[] processedString = stringArray;

		// check if the string contain the target word in case insensative way
		processedString = processedString.Where(element => element.ToLower().Contains(word.ToLower())).ToArray();
		return processedString;
	}
}
