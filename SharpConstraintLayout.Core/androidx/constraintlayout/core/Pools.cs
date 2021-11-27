/*
 * Copyright (C) 2015 The Android Open Source Project
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

namespace androidx.constraintlayout.core
{
	/// <summary>
	/// Helper class for crating pools of objects. An example use looks like this:
	/// <pre>
	/// public class MyPooledClass {
	/// 
	///     private static final SimplePool<MyPooledClass> sPool =
	///             new SimplePool<MyPooledClass>(10);
	/// 
	///     public static MyPooledClass obtain() {
	///         MyPooledClass instance = sPool.acquire();
	///         return (instance != null) ? instance : new MyPooledClass();
	///     }
	/// 
	///     public void recycle() {
	///          // Clear state if needed.
	///          sPool.release(this);
	///     }
	/// 
	///     . . .
	/// }
	/// </pre>
	/// </summary>
	internal sealed class Pools
	{
		private const bool DEBUG = false;

		/// <summary>
		/// Interface for managing a pool of objects.
		/// </summary>
		/// @param <T> The pooled type. </param>
		internal interface Pool<T>
		{

			/// <returns> An instance from the pool if such, null otherwise. </returns>
			T acquire();

			/// <summary>
			/// Release an instance to the pool.
			/// </summary>
			/// <param name="instance"> The instance to release. </param>
			/// <returns> Whether the instance was put in the pool.
			/// </returns>
			/// <exception cref="IllegalStateException"> If the instance is already in the pool. </exception>
			bool release(T instance);

			/// <summary>
			/// Try releasing all instances at the same time
			/// </summary>
			/// <param name="variables"> the variables to release </param>
			/// <param name="count"> the number of variables to release </param>
			void releaseAll(T[] variables, int count);
		}

		private Pools()
		{
			/* do nothing - hiding constructor */
		}

		/// <summary>
		/// Simple (non-synchronized) pool of objects.
		/// </summary>
		/// @param <T> The pooled type. </param>
		internal class SimplePool<T> : Pool<T>
			where T : class
		{
			internal readonly object[] mPool;

			internal int mPoolSize;

			/// <summary>
			/// Creates a new instance.
			/// </summary>
			/// <param name="maxPoolSize"> The max pool size.
			/// </param>
			/// <exception cref="IllegalArgumentException"> If the max pool size is less than zero. </exception>
			internal SimplePool(int maxPoolSize)
			{
				if (maxPoolSize <= 0)
				{
					throw new System.ArgumentException("The max pool size must be > 0");
				}
				mPool = new object[maxPoolSize];
			}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public T acquire()
			public virtual T acquire()
			{
				if (mPoolSize > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastPooledIndex = mPoolSize - 1;
					int lastPooledIndex = mPoolSize - 1;
					T instance = (T) mPool[lastPooledIndex];
					mPool[lastPooledIndex] = null;
					mPoolSize--;
					return instance;
				}
				return default(T);
			}

			public virtual bool release(T instance)
			{
				if (DEBUG)
				{
					if (isInPool(instance))
					{
						throw new System.InvalidOperationException("Already in the pool!");
					}
				}
				if (mPoolSize < mPool.Length)
				{
					mPool[mPoolSize] = instance;
					mPoolSize++;
					return true;
				}
				return false;
			}

			public virtual void releaseAll(T[] variables, int count)
			{
				if (count > variables.Length)
				{
					count = variables.Length;
				}
				for (int i = 0; i < count; i++)
				{
					T instance = variables[i];
					if (DEBUG)
					{
						if (isInPool(instance))
						{
							throw new System.InvalidOperationException("Already in the pool!");
						}
					}
					if (mPoolSize < mPool.Length)
					{
						mPool[mPoolSize] = instance;
						mPoolSize++;
					}
				}
			}

			internal virtual bool isInPool(T instance)
			{
				for (int i = 0; i < mPoolSize; i++)
				{
					if (mPool[i] == instance)
					{
						return true;
					}
				}
				return false;
			}
		}

	}
}