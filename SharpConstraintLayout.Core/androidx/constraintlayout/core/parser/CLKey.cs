using System.Collections.Generic;
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

	public class CLKey : CLContainer
	{

	  private static List<string> sections = new List<string>();

	  static CLKey()
	  {
		sections.Add("ConstraintSets");
		sections.Add("Variables");
		sections.Add("Generate");
		sections.Add("Transitions");
		sections.Add("KeyFrames");
		sections.Add("KeyAttributes");
		sections.Add("KeyPositions");
		sections.Add("KeyCycles");
	  }

	  public CLKey(char[] content) : base(content)
	  {
	  }

	  public static CLElement allocate(char[] content)
	  {
		return new CLKey(content);
	  }

	  public static CLElement allocate(string name, CLElement value)
	  {
		CLKey key = new CLKey(name.ToCharArray());
		key.Start = 0;
		key.End = name.Length - 1;
		key.set(value);
		return key;
	  }

	  public virtual string Name
	  {
		  get
		  {
			  return content();
		  }
	  }

	  public override string toJSON()
	  {
		if (mElements.Count > 0)
		{
		  return DebugName + content() + ": " + mElements[0].toJSON();
		}
		return DebugName + content() + ": <> ";
	  }

	  protected internal override string toFormattedJSON(int indent, int forceIndent)
	  {
		StringBuilder json = new StringBuilder(DebugName);
		addIndent(json, indent);
		string tempcontent = content();
		if (mElements.Count > 0)
		{
		  json.Append(tempcontent);
		  json.Append(": ");
		  if (sections.Contains(tempcontent))
		  {
			forceIndent = 3;
		  }
		  if (forceIndent > 0)
		  {
			json.Append(mElements[0].toFormattedJSON(indent, forceIndent - 1));
		  }
		  else
		  {
			string val = mElements[0].toJSON();
			if (val.Length + indent < MAX_LINE)
			{
			  json.Append(val);
			}
			else
			{
			  json.Append(mElements[0].toFormattedJSON(indent, forceIndent - 1));
			}
		  }
		  return json.ToString();
		}
		return tempcontent + ": <> ";
	  }

	  public virtual void set(CLElement value)
	  {
		if (mElements.Count > 0)
		{
		  mElements[0] = value;
		}
		else
		{
		  mElements.Add(value);
		}
	  }

	  public virtual CLElement Value
	  {
		  get
		  {
			if (mElements.Count > 0)
			{
			  return mElements[0];
			}
			return null;
		  }
	  }
	}

}