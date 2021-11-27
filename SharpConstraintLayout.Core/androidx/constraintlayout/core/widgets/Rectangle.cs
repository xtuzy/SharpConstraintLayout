/*
 * Copyright (C) 2016 The Android Open Source Project
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
namespace androidx.constraintlayout.core.widgets
{
	/// <summary>
	/// Simple rect class
	/// </summary>
	public class Rectangle
	{
		public int x;
		public int y;
		public int width;
		public int height;

		public virtual void setBounds(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		internal virtual void grow(int w, int h)
		{
			x -= w;
			y -= h;
			width += 2 * w;
			height += 2 * h;
		}

		internal virtual bool intersects(Rectangle bounds)
		{
			return x >= bounds.x && x < bounds.x + bounds.width && y >= bounds.y && y < bounds.y + bounds.height;
		}

		public virtual bool contains(int x, int y)
		{
			return x >= this.x && x < this.x + this.width && y >= this.y && y < this.y + this.height;
		}

		public virtual int CenterX
		{
			get
			{
				return (x + width) / 2;
			}
		}
		public virtual int CenterY
		{
			get
			{
				return (y + height) / 2;
			}
		}
	}

}