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
	public class CLParsingException : Exception
	{
	  private readonly string mReason;
	  private readonly int mLineNumber;
	  private readonly string mElementClass;

	  public CLParsingException(string reason, CLElement element)
	  {
		mReason = reason;
		if (element != null)
		{
		  mElementClass = element.StrClass;
		  mLineNumber = element.Line;
		}
		else
		{
		  mElementClass = "unknown";
		  mLineNumber = 0;
		}
	  }

	  public virtual string reason()
	  {
		return mReason + " (" + mElementClass + " at line " + mLineNumber + ")";
	  }

	  public override string ToString()
	  {
		return "CLParsingException (" + this.GetHashCode() + ") : " + reason();
	  }
	}

}