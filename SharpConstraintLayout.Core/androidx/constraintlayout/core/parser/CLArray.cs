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
	public class CLArray : CLContainer
	{
	  public CLArray(char[] content) : base(content)
	  {
	  }

	  public static CLElement allocate(char[] content)
	  {
		return new CLArray(content);
	  }

	  public override string toJSON()
	  {
		StringBuilder content = new StringBuilder(DebugName + "[");
		bool first = true;
		for (int i = 0; i < mElements.Count; i++)
		{
		  if (!first)
		  {
			content.Append(", ");
		  }
		  else
		  {
			first = false;
		  }
		  content.Append(mElements[i].toJSON());
		}
		return content + "]";
	  }

	  protected internal override string toFormattedJSON(int indent, int forceIndent)
	  {
		StringBuilder json = new StringBuilder();
		string val = toJSON();
		if (forceIndent <= 0 && val.Length + indent < MAX_LINE)
		{
		  json.Append(val);
		}
		else
		{
		  json.Append("[\n");
		  bool first = true;
		  foreach (CLElement element in mElements)
		  {
			if (!first)
			{
			  json.Append(",\n");
			}
			else
			{
			  first = false;
			}
			addIndent(json, indent + BASE_INDENT);
			json.Append(element.toFormattedJSON(indent + BASE_INDENT, forceIndent - 1));
		  }
		  json.Append("\n");
		  addIndent(json, indent);
		  json.Append("]");
		}
		return json.ToString();
	  }
	}

}