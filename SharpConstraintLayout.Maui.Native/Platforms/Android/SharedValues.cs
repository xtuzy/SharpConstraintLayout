using System.Collections.Generic;

/*
 * Copyright (C) 2020 The Android Open Source Project
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

namespace SharpConstraintLayout.Maui.Platforms.Androids
{
	using SparseIntArray = android.util.SparseIntArray;


	/// <summary>
	/// Shared values
	/// </summary>
	public class SharedValues
	{
		public const int UNSET = -1;

		private SparseIntArray mValues = new SparseIntArray();
		private Dictionary<int?, HashSet<WeakReference<SharedValuesListener>>> mValuesListeners = new Dictionary<int?, HashSet<WeakReference<SharedValuesListener>>>();

		public interface SharedValuesListener
		{
			void onNewValue(int key, int newValue, int oldValue);
		}

		public virtual void addListener(int key, SharedValuesListener listener)
		{
			HashSet<WeakReference<SharedValuesListener>> listeners = mValuesListeners[key];
			if (listeners == null)
			{
				listeners = new HashSet<>();
				mValuesListeners[key] = listeners;
			}
			listeners.Add(new WeakReference<>(listener));
		}

		public virtual void removeListener(int key, SharedValuesListener listener)
		{
			HashSet<WeakReference<SharedValuesListener>> listeners = mValuesListeners[key];
			if (listeners == null)
			{
				return;
			}
			IList<WeakReference<SharedValuesListener>> toRemove = new List<WeakReference<SharedValuesListener>>();
			foreach (WeakReference<SharedValuesListener> listenerWeakReference in listeners)
			{
				SharedValuesListener l = listenerWeakReference.get();
				if (l == null || l == listener)
				{
					toRemove.Add(listenerWeakReference);
				}
			}
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
			listeners.removeAll(toRemove);
		}

		public virtual void removeListener(SharedValuesListener listener)
		{
			foreach (int? key in mValuesListeners.Keys)
			{
				removeListener(key.Value, listener);
			}
		}

		public virtual void clearListeners()
		{
			mValuesListeners.Clear();
		}

		public virtual int getValue(int key)
		{
			return mValues.get(key, UNSET);
		}

		public virtual void fireNewValue(int key, int value)
		{
			bool needsCleanup = false;
			int previousValue = mValues.get(key, UNSET);
			if (previousValue == value)
			{
				// don't send the value to listeners if it's the same one.
				return;
			}
			mValues.put(key, value);
			HashSet<WeakReference<SharedValuesListener>> listeners = mValuesListeners[key];
			if (listeners == null)
			{
				return;
			}

			foreach (WeakReference<SharedValuesListener> listenerWeakReference in listeners)
			{
				SharedValuesListener l = listenerWeakReference.get();
				if (l != null)
				{
					l.onNewValue(key, value, previousValue);
				}
				else
				{
					needsCleanup = true;
				}
			}

			if (needsCleanup)
			{
				IList<WeakReference<SharedValuesListener>> toRemove = new List<WeakReference<SharedValuesListener>>();
				foreach (WeakReference<SharedValuesListener> listenerWeakReference in listeners)
				{
					SharedValuesListener listener = listenerWeakReference.get();
					if (listener == null)
					{
						toRemove.Add(listenerWeakReference);
					}
				}
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
				listeners.removeAll(toRemove);
			}
		}
	}

}