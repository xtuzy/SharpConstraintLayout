using System.Collections.Generic;

/*
 * Copyright 2021 The Android Open Source Project
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
namespace androidx.constraintlayout.core.state
{

	public class Registry
	{

		private static readonly Registry sRegistry = new Registry();

		public static Registry Instance
		{
			get
			{
				return sRegistry;
			}
		}

		private Dictionary<string, RegistryCallback> mCallbacks = new Dictionary<string, RegistryCallback>();

		public virtual void register(string name, RegistryCallback callback)
		{
			mCallbacks[name] = callback;
		}

		public virtual void unregister(string name, RegistryCallback callback)
		{
			mCallbacks.Remove(name);
		}

		public virtual void updateContent(string name, string content)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				callback.onNewMotionScene(content);
			}
		}

		public virtual void updateProgress(string name, float progress)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				callback.onProgress(progress);
			}
		}

		public virtual string currentContent(string name)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				return callback.currentMotionScene();
			}
			return null;
		}

		public virtual string currentLayoutInformation(string name)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				return callback.currentLayoutInformation();
			}
			return null;
		}

		public virtual void setDrawDebug(string name, int debugMode)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				callback.DrawDebug = debugMode;
			}
		}

		public virtual void setLayoutInformationMode(string name, int mode)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				callback.LayoutInformationMode = mode;
			}
		}

		//public virtual ISet<string> LayoutList
		public virtual ICollection<string> LayoutList
		{
			get
			{
				return mCallbacks.Keys;
			}
		}

		public virtual long getLastModified(string name)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				return callback.LastModified;
			}
			return long.MaxValue;
		}

		public virtual void updateDimensions(string name, int width, int height)
		{
			RegistryCallback callback = mCallbacks[name];
			if (callback != null)
			{
				callback.onDimensions(width, height);
			}
		}
	}

}