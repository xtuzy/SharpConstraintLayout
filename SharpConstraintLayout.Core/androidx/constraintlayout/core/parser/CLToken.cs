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
	public class CLToken : CLElement
	{
	  internal int index = 0;
	  internal Type type = Type.UNKNOWN;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean() throws CLParsingException
	  public virtual bool Boolean
	  {
		  get
		  {
			if (type == Type.TRUE)
			{
			  return true;
			}
			if (type == Type.FALSE)
			{
			  return false;
			}
			throw new CLParsingException("this token is not a boolean: <" + content() + ">", this);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isNull() throws CLParsingException
	  public virtual bool Null
	  {
		  get
		  {
			if (type == Type.NULL)
			{
			  return true;
			}
			throw new CLParsingException("this token is not a null: <" + content() + ">", this);
		  }
	  }

	  public enum Type
	  {
		  UNKNOWN,
		  TRUE,
		  FALSE,
		  NULL
	  }

	  internal char[] tokenTrue = "true".ToCharArray();
	  internal char[] tokenFalse = "false".ToCharArray();
	  internal char[] tokenNull = "null".ToCharArray();

	  public CLToken(char[] content) : base(content)
	  {
	  }

	  public static CLElement allocate(char[] content)
	  {
		return new CLToken(content);
	  }

	  public override string toJSON()
	  {
		if (CLParser.DEBUG)
		{
		  return "<" + content() + ">";
		}
		else
		{
		  return content();
		}
	  }

	  protected internal override string toFormattedJSON(int indent, int forceIndent)
	  {
		StringBuilder json = new StringBuilder();
		addIndent(json, indent);
		json.Append(content());
		return json.ToString();
	  }

	  public virtual Type getType()
	  {
		return type;
	  }

	  public virtual bool validate(char c, long position)
	  {
		bool isValid = false;
		switch (type)
		{
		  case androidx.constraintlayout.core.parser.CLToken.Type.TRUE:
		  {
			isValid = (tokenTrue[index] == c);
			if (isValid && index + 1 == tokenTrue.Length)
			{
			  End = position;
			}
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLToken.Type.FALSE:
		  {
			isValid = (tokenFalse[index] == c);
			if (isValid && index + 1 == tokenFalse.Length)
			{
			  End = position;
			}
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLToken.Type.NULL:
		  {
			isValid = (tokenNull[index] == c);
			if (isValid && index + 1 == tokenNull.Length)
			{
			  End = position;
			}
		  }
		  break;
		  case androidx.constraintlayout.core.parser.CLToken.Type.UNKNOWN:
		  {
			if (tokenTrue[index] == c)
			{
			  type = Type.TRUE;
			  isValid = true;
			}
			else if (tokenFalse[index] == c)
			{
			  type = Type.FALSE;
			  isValid = true;
			}
			else if (tokenNull[index] == c)
			{
			  type = Type.NULL;
			  isValid = true;
			}
		  }
	  break;
		}

		index++;
		return isValid;
	  }

	}

}