using System;
using System.IO;
using System.Net.Sockets;

/*
 * Copyright (C) 2021 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace androidx.constraintlayout.core.motion.utils
{

	public class Utils
	{
		public static void log(string tag, string value)
		{
			Console.WriteLine(tag + " : " + value);
		}
		public static void loge(string tag, string value)
		{
			//Console.Error.WriteLine(tag + " : " + value);
			Console.Fail(tag + " : " + value);
		}

		//应该用不到
	   //public static void socketSend(string str)
	   //{
		  // try
		  // {
			 //  Socket socket = new Socket("127.0.0.1", 5327);
			 //  System.IO.Stream @out = socket.OutputStream;
			 //  @out.WriteByte(str.GetBytes());
			 //  @out.Close();
		  // }
		  // catch (IOException e)
		  // {
			 //  Console.WriteLine(e.ToString());
			 //  Console.Write(e.StackTrace);
		  // }
	   //}

		private static int clamp(int c)
		{
			int N = 255;
			c &= ~(c >> 31);
			c -= N;
			c &= (c >> 31);
			c += N;
			return c;
		}

		public virtual int getInterpolatedColor(float[] value)
		{
			int r = clamp((int)((float) Math.Pow(value[0], 1.0 / 2.2) * 255.0f));
			int g = clamp((int)((float) Math.Pow(value[1], 1.0 / 2.2) * 255.0f));
			int b = clamp((int)((float) Math.Pow(value[2], 1.0 / 2.2) * 255.0f));
			int a = clamp((int)(value[3] * 255.0f));
			int color = (a << 24) | (r << 16) | (g << 8) | b;
			return color;
		}

		public static int rgbaTocColor(float r, float g, float b, float a)
		{
			int ir = clamp((int)(r * 255f));
			int ig = clamp((int)(g * 255f));
			int ib = clamp((int)(b * 255f));
			int ia = clamp((int)(a * 255f));
			int color = (ia << 24) | (ir << 16) | (ig << 8) | ib;
			return color;
		}

		//应该用不到
		//public static void logStack(string msg, int n)
		//{
		//	StackTraceElement[] st = (new Exception()).StackTrace;
		//	string s = " ";
		//	n = Math.Min(n, st.Length - 1);
		//	for (int i = 1; i <= n; i++)
		//	{
		//		StackTraceElement ste = st[i];
		//		string stack = ".(" + st[i].FileName + ":" + st[i].LineNumber + ") " + st[i].MethodName;
		//		s += " ";
		//		Console.WriteLine(msg + s + stack + s);
		//	}
		//}

		public static void log(string str)
		{
			//StackTraceElement s = (new Exception()).StackTrace[1];
			System.Diagnostics.StackFrame s = new System.Diagnostics.StackTrace(new Exception(), true).GetFrame(0);//需要pdb文件,https://stackoverflow.com/questions/3328990/how-can-i-get-the-line-number-which-threw-exception
			string methodName = s.GetMethod().Name;
			methodName = (methodName + "                  ").Substring(0,17);
			string npad = "    ".Substring(Convert.ToString(s.GetFileLineNumber()).Length);
			string ss = ".(" + s.GetFileName() + ":" + s.GetFileLineNumber() + ")" + npad + methodName;
			Console.WriteLine(ss + " " + str);
		}

	}

}