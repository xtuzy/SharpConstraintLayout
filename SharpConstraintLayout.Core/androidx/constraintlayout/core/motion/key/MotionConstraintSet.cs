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
namespace androidx.constraintlayout.core.motion.key
{
	public class MotionConstraintSet
	{
		private const string ERROR_MESSAGE = "XML parser error must be within a Constraint ";

		private const int INTERNAL_MATCH_PARENT = -1;
		private const int INTERNAL_WRAP_CONTENT = -2;
		private const int INTERNAL_MATCH_CONSTRAINT = -3;
		private const int INTERNAL_WRAP_CONTENT_CONSTRAINED = -4;

		private bool mValidate;
		public string mIdString;
		public const int ROTATE_NONE = 0;
		public const int ROTATE_PORTRATE_OF_RIGHT = 1;
		public const int ROTATE_PORTRATE_OF_LEFT = 2;
		public const int ROTATE_RIGHT_OF_PORTRATE = 3;
		public const int ROTATE_LEFT_OF_PORTRATE = 4;
		public int mRotate = 0;
	}

}