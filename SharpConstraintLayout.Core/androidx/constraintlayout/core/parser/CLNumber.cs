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
	public class CLNumber : CLElement
	{

	  internal float value = float.NaN;
	  public CLNumber(char[] content) : base(content)
	  {
	  }

	  public CLNumber(float value) : base(null)
	  {
		this.value = value;
	  }

	  public static CLElement allocate(char[] content)
	  {
		return new CLNumber(content);
	  }

	  public override string toJSON()
	  {
		float value = Float;
		int intValue = (int) value;
		if ((float) intValue == value)
		{
		  return "" + intValue;
		}
		return "" + value;
	  }

	  protected internal override string toFormattedJSON(int indent, int forceIndent)
	  {
		StringBuilder json = new StringBuilder();
		addIndent(json, indent);
		float value = Float;
		int intValue = (int) value;
		if ((float) intValue == value)
		{
		  json.Append(intValue);
		}
		else
		{
		  json.Append(value);
		}
		return json.ToString();
	  }

	  public virtual bool isInt
	  {
		  get
		  {
			float value = Float;
			int intValue = (int) value;
			return ((float) intValue == value);
		  }
	  }

	  public override int Int
	  {
		  get
		  {
			if (float.IsNaN(value))
			{
			  value = int.Parse(content());
			}
			return (int) value;
		  }
	  }

	  public override float Float
	  {
		  get
		  {
			if (float.IsNaN(value))
			{
			  value = float.Parse(content());
			}
			return value;
		  }
	  }

	  public virtual void putValue(float value)
	  {
		this.value = value;
	  }

	}
}