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
using DeviceWakeUpManager;

public class commandFunctions
{
	public commandFunctions(){}
	
	public string RunCommandWithLoadingMessage(string command)
	{
		// Show the message to tell the user the command is loading
		LoadingMessage lm = new LoadingMessage();
		lm.Show();
		string result = RunCommand(command, 0);
		lm.Close();
		return result;
	}

	public String RunCommand(String command, int flag)
	{
		String result = "";
		// if flag = 0, the request is from main window
		// if flag = 1, the request is from bg worker
		String commandFileName;
		if (flag == 0)
		{
			commandFileName = "wudm.bat";

		}
		else
		{
			commandFileName = "BGwudm.bat";
		}

		// create a command file
		using (System.IO.StreamWriter file = new System.IO.StreamWriter(commandFileName))
		{
			file.WriteLine(command);
			file.Close();
		}

		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = commandFileName;
		startInfo.CreateNoWindow = true;
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardOutput = true;
		process.StartInfo = startInfo;

		process.Start();
		result = process.StandardOutput.ReadToEnd();

		return result;
	}

	public String[] ProcessResult(String stringBeforeProcess)
	{
		String[] processedString;

		processedString = stringBeforeProcess.Split('\n');
		// remove the first 2 elements
		processedString = processedString.Skip(2).ToArray();
		// remove the last element
		//processedString = processedString.Take(processedString.Count() - 1).ToArray();

		/*
		// remove all "\r" from result
		for (int i = 0; i < processedString.Length; i++)
		{
			String deviceName = processedString[i];
			deviceName = deviceName.Where(element => element != '\r').ToString();
		}
		*/

		// remove the empty elements
		processedString = processedString.Where(element => element != "").ToArray();
		processedString = processedString.Where(element => element != "\r").ToArray();

		// sort the string array
		Array.Sort(processedString, StringComparer.InvariantCultureIgnoreCase);

		return processedString;
	}
}
