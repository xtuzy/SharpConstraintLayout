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
namespace androidx.constraintlayout.core.motion.parse
{
	using TypedBundle = androidx.constraintlayout.core.motion.utils.TypedBundle;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using CLElement = androidx.constraintlayout.core.parser.CLElement;
	using CLKey = androidx.constraintlayout.core.parser.CLKey;
	using CLObject = androidx.constraintlayout.core.parser.CLObject;
	using CLParser = androidx.constraintlayout.core.parser.CLParser;
	using CLParsingException = androidx.constraintlayout.core.parser.CLParsingException;

	public class KeyParser
	{

		private delegate int Ids(string str);

		private delegate int DataType(int str);


		private static TypedBundle parse(string str, Ids table, DataType dtype)
		{
			TypedBundle bundle = new TypedBundle();

			try
			{
				CLObject parsedContent = CLParser.parse(str);
				int n = parsedContent.size();
				for (int i = 0; i < n; i++)
				{
					CLKey clkey = ((CLKey) parsedContent.get(i));
					string type = clkey.content();
					CLElement value = clkey.Value;
					int id = table(type);
					if (id == -1)
					{
						//Console.Error.WriteLine("unknown type " + type);
						Console.Fail("unknown type " + type);
						continue;
					}
					switch (dtype(id))
					{
						case TypedValues.FLOAT_MASK:
							bundle.add(id, value.Float);
							Console.WriteLine("parse " + type + " FLOAT_MASK > " + value.Float);
							break;
						case TypedValues.STRING_MASK:
							bundle.add(id, value.content());
							Console.WriteLine("parse " + type + " STRING_MASK > " + value.content());

							break;
						case TypedValues.INT_MASK:
							bundle.add(id, value.Int);
							Console.WriteLine("parse " + type + " INT_MASK > " + value.Int);
							break;
						case TypedValues.BOOLEAN_MASK:
							bundle.add(id, parsedContent.getBoolean(i));
							break;
					}
				}
			}
			catch (CLParsingException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return bundle;
		}

		public static TypedBundle parseAttributes(string str)
		{
			return parse(str, TypedValues.TypedValues_Attributes.getId, TypedValues.TypedValues_Attributes.getType);
		}

		public static void Main(string[] args)
		{
			string str = "{" + "frame:22,\n" + "target:'widget1',\n" + "easing:'easeIn',\n" + "curveFit:'spline',\n" + "progress:0.3,\n" + "alpha:0.2,\n" + "elevation:0.7,\n" + "rotationZ:23,\n" + "rotationX:25.0,\n" + "rotationY:27.0,\n" + "pivotX:15,\n" + "pivotY:17,\n" + "pivotTarget:'32',\n" + "pathRotate:23,\n" + "scaleX:0.5,\n" + "scaleY:0.7,\n" + "translationX:5,\n" + "translationY:7,\n" + "translationZ:11,\n" + "}";
			parseAttributes(str);
		}
	}

}