using System;

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
	public class CLParser
	{

	  internal static bool DEBUG = false;

	  private string mContent;
	  private bool hasComment = false;
	  private int lineNumber;

	  internal enum TYPE
	  {
		  UNKNOWN,
		  OBJECT,
		  ARRAY,
		  NUMBER,
		  STRING,
		  KEY,
		  TOKEN
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static CLObject parse(String string) throws CLParsingException
	  public static CLObject parse(string @string)
	  {
		return (new CLParser(@string)).parse();
	  }

	  public CLParser(string content)
	  {
		mContent = content;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CLObject parse() throws CLParsingException
	  public virtual CLObject parse()
	  {
		CLObject root = null;

		char[] content = mContent.ToCharArray();
		CLElement currentElement = null;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = content.length;
		int length = content.Length;

		// First, let's find the root element start
		lineNumber = 1;

		int startIndex = -1;
		for (int i = 0; i < length; i++)
		{
		  char c = content[i];
		  if (c == '{')
		  {
			startIndex = i;
			break;
		  }
		  if (c == '\n')
		  {
			lineNumber++;
		  }
		}
		if (startIndex == -1)
		{
		  throw new CLParsingException("invalid json content", null);
		}

		// We have a root object, let's start
		root = CLObject.allocate(content);
		root.Line = lineNumber;
		root.Start = startIndex;
		currentElement = root;

		for (int i = startIndex + 1; i < length; i++)
		{
		  char c = content[i];
		  if (c == '\n')
		  {
			lineNumber++;
		  }
		  if (hasComment)
		  {
			if (c == '\n')
			{
			  hasComment = false;
			}
			else
			{
			  continue;
			}
		  }
		  if (false)
		  {
			Console.WriteLine("Looking at " + i + " : <" + c + ">");
		  }
		  if (currentElement == null)
		  {
			break;
		  }
		  if (currentElement.Done)
		  {
			currentElement = getNextJsonElement(i, c, currentElement, content);
		  }
		  else if (currentElement is CLObject)
		  {
			if (c == '}')
			{
			  currentElement.End = i - 1;
			}
			else
			{
			  currentElement = getNextJsonElement(i, c, currentElement, content);
			}
		  }
		  else if (currentElement is CLArray)
		  {
			if (c == ']')
			{
			  currentElement.End = i - 1;
			}
			else
			{
			  currentElement = getNextJsonElement(i, c, currentElement, content);
			}
		  }
		  else if (currentElement is CLString)
		  {
			char ck = content[(int) currentElement.start];
			if (ck == c)
			{
			  currentElement.Start = currentElement.start + 1;
			  currentElement.End = i - 1;
			}
		  }
		  else
		  {
			if (currentElement is CLToken)
			{
			  CLToken token = (CLToken) currentElement;
			  if (!token.validate(c, i))
			  {
				throw new CLParsingException("parsing incorrect token " + token.content() + " at line " + lineNumber, token);
			  }
			}
			if (currentElement is CLKey || currentElement is CLString)
			{
			  char ck = content[(int) currentElement.start];
			  if ((ck == '\'' || ck == '"') && ck == c)
			  {
				currentElement.Start = currentElement.start + 1;
				currentElement.End = i - 1;
			  }
			}
			if (!currentElement.Done)
			{
			  if (c == '}' || c == ']' || c == ',' || c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == ':')
			  {
				currentElement.End = i - 1;
				if (c == '}' || c == ']')
				{
				  currentElement = currentElement.getContainer();
				  currentElement.End = i - 1;
				  if (currentElement is CLKey)
				  {
					currentElement = currentElement.getContainer();
					currentElement.End = i - 1;
				  }
				}
			  }
			}
		  }

		  if (currentElement.Done && (!(currentElement is CLKey) || ((CLKey) currentElement).mElements.Count > 0))
		  {
			currentElement = currentElement.getContainer();
		  }
		}

		// Close all open elements -- allow us to be more resistant to invalid json, useful during editing.
		while (currentElement != null && !currentElement.Done)
		{
		  if (currentElement is CLString)
		  {
			currentElement.Start = (int) currentElement.start + 1;
		  }
		  currentElement.End = length - 1;
		  currentElement = currentElement.getContainer();
		}

		if (DEBUG)
		{
		  Console.WriteLine("Root: " + root.toJSON());
		}

		return root;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private CLElement getNextJsonElement(int position, char c, CLElement currentElement, char[] content) throws CLParsingException
	  private CLElement getNextJsonElement(int position, char c, CLElement currentElement, char[] content)
	  {
		switch (c)
		{
		  case ' ':
		  case ':':
		  case ',':
		  case '\t':
		  case '\r':
		  case '\n':
		  {
			// skip space
		  }
		  break;
		  case '{':
		  {
			currentElement = createElement(currentElement, position, TYPE.OBJECT, true, content);
		  }
		  break;
		  case '[':
		  {
			currentElement = createElement(currentElement, position, TYPE.ARRAY, true, content);
		  }
		  break;
		  case ']':
		  case '}':
		  {
			currentElement.End = position - 1;
			currentElement = currentElement.getContainer();
			currentElement.End = position;
		  }
		  break;
		  case '"':
		  case '\'':
		  {
			if (currentElement is CLObject)
			{
			  currentElement = createElement(currentElement, position, TYPE.KEY, true, content);
			}
			else
			{
			  currentElement = createElement(currentElement, position, TYPE.STRING, true, content);
			}
		  }
		  break;
		  case '/':
		  {
			if (position + 1 < content.Length && content[position + 1] == '/')
			{
			  hasComment = true;
			}
		  }
		  break;
		  case '-':
		  case '+':
		  case '.':
		  case '0':
		  case '1':
		  case '2':
		  case '3':
		  case '4':
		  case '5':
		  case '6':
		  case '7':
		  case '8':
		  case '9':
		  {
			currentElement = createElement(currentElement, position, TYPE.NUMBER, true, content);
		  }
		  break;
		  default:
		  {
			if (currentElement is CLContainer && !(currentElement is CLObject))
			{
			  currentElement = createElement(currentElement, position, TYPE.TOKEN, true, content);
			  CLToken token = (CLToken) currentElement;
			  if (!token.validate(c, position))
			  {
				throw new CLParsingException("incorrect token <" + c + "> at line " + lineNumber, token);
			  }
			}
			else
			{
			  currentElement = createElement(currentElement, position, TYPE.KEY, true, content);
			}
		  }
	  break;
		}
		return currentElement;
	  }

	  private CLElement createElement(CLElement currentElement, int position, TYPE type, bool applyStart, char[] content)
	  {
		CLElement newElement = null;
		if (DEBUG)
		{
		  Console.WriteLine("CREATE " + type + " at " + content[position]);
		}
		switch (type)
		{
		  case androidx.constraintlayout.core.parser.CLParser.TYPE.OBJECT:
		  {
			newElement = CLObject.allocate(content);
			position++;
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLParser.TYPE.ARRAY:
		  {
			newElement = CLArray.allocate(content);
			position++;
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLParser.TYPE.STRING:
		  {
			newElement = CLString.allocate(content);
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLParser.TYPE.NUMBER:
		  {
			newElement = CLNumber.allocate(content);
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLParser.TYPE.KEY:
		  {
			newElement = CLKey.allocate(content);
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLParser.TYPE.TOKEN:
		  {
			newElement = CLToken.allocate(content);
		  }
		  break;
		  default:
			  break;
		}
		if (newElement == null)
		{
		  return null;
		}
		newElement.Line = lineNumber;
		if (applyStart)
		{
		  newElement.Start = position;
		}
		if (currentElement is CLContainer)
		{
		  CLContainer container = (CLContainer) currentElement;
		  newElement.setContainer(container);
		}
		return newElement;
	  }

	}

}