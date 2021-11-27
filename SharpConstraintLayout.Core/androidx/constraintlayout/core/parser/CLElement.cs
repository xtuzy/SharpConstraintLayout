using System;
using System.Text;

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
namespace androidx.constraintlayout.core.parser
{
	public class CLElement
	{

		private readonly char[] mContent;
		protected internal long start = -1;
		protected internal long end = long.MaxValue;
		protected internal CLContainer mContainer;
		private int line;

		protected internal static int MAX_LINE = 80; // Max number of characters before the formatter indents
		protected internal static int BASE_INDENT = 2; // default indentation value

		public CLElement(char[] content)
		{
			mContent = content;
		}

		public virtual bool notStarted()
		{
			return start == -1;
		}

		public virtual int Line
		{
			set
			{
				this.line = value;
			}
			get
			{
				return line;
			}
		}


		public virtual long Start
		{
			set
			{
				this.start = value;
			}
			get
			{
				return this.start;
			}
		}


		/// <summary>
		/// The character index this element was ended on
		/// 
		/// @return
		/// </summary>
		public virtual long End
		{
			get
			{
				return this.end;
			}
			set
			{
				if (this.end != long.MaxValue)
				{
					return;
				}
				this.end = value;
				if (CLParser.DEBUG)
				{
					Console.WriteLine("closing " + this.GetHashCode() + " -> " + this);
				}
				if (mContainer != null)
				{
					mContainer.add(this);
				}
			}
		}


		protected internal virtual void addIndent(StringBuilder builder, int indent)
		{
			for (int i = 0; i < indent; i++)
			{
				builder.Append(' ');
			}
		}

		public override string ToString()
		{
			if (start > end || end == long.MaxValue)
			{
				return this.GetType() + " (INVALID, " + start + "-" + end + ")";
			}
			string content = new string(mContent);
			content = StringHelperClass.SubstringSpecial(content, (int) start, (int) end + 1);

			return StrClass + " (" + start + " : " + end + ") <<" + content + ">>";
		}

		protected internal virtual string StrClass
		{
			get
			{
				string myClass = this.GetType().ToString();
				return myClass.Substring(myClass.LastIndexOf('.') + 1);
			}
		}

		protected internal virtual string DebugName
		{
			get
			{
				if (CLParser.DEBUG)
				{
					return StrClass + " -> ";
				}
				return "";
			}
		}

		public virtual string content()
		{
			string content = new string(mContent);
			if (end == long.MaxValue || end < start)
			{
				return content.Substring((int) start, 1);
			}
			return StringHelperClass.SubstringSpecial(content, (int) start, (int) end + 1);
		}

		public virtual bool Done
		{
			get
			{
				return end != long.MaxValue;
			}
		}

		public virtual void setContainer(CLContainer element)
		{
			mContainer = element;
		}

		public virtual CLElement getContainer()
		{
			return mContainer;
		}

		public virtual bool Started
		{
			get
			{
				return start > -1;
			}
		}

		public virtual string toJSON()
		{
			return "";
		}

		protected internal virtual string toFormattedJSON(int indent, int forceIndent)
		{
			return "";
		}

		public virtual int Int
		{
			get
			{
				if (this is CLNumber)
				{
					return ((CLNumber) this).Int;
				}
				return 0;
			}
		}

		public virtual float Float
		{
			get
			{
				if (this is CLNumber)
				{
					return ((CLNumber) this).Float;
				}
				return float.NaN;
			}
		}
	}

}