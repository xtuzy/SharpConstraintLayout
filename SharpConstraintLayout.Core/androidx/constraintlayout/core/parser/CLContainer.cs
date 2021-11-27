using System;
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

    public class CLContainer : CLElement
    {
        public List<CLElement> mElements = new List<CLElement>();

        public CLContainer(char[] content) : base(content)
        {
        }

        public static CLElement allocate(char[] content)
        {
            return new CLContainer(content);
        }

        public virtual void add(CLElement element)
        {
            mElements.Add(element);
            if (CLParser.DEBUG)
            {
                Console.WriteLine("added element " + element + " to " + this);
            }
        }

        public override string ToString()
        {
            StringBuilder list = new StringBuilder();
            foreach (CLElement element in mElements)
            {
                if (list.Length > 0)
                {
                    list.Append("; ");
                }
                list.Append(element);
            }
            return base.ToString() + " = <" + list + " >";
        }

        public virtual int size()
        {
            return mElements.Count;
        }

        public virtual List<string> names()
        {
            List<string> names = new List<string>();
            foreach (CLElement element in mElements)
            {
                if (element is CLKey)
                {
                    CLKey key = (CLKey)element;
                    names.Add(key.content());
                }
            }
            return names;
        }

        public virtual bool has(string name)
        {
            foreach (CLElement element in mElements)
            {
                if (element is CLKey)
                {
                    CLKey key = (CLKey)element;
                    if (key.content().Equals(name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void put(string name, CLElement value)
        {
            CLKey key;
            foreach (CLElement element in mElements)
            {
                key = (CLKey)element;
                if (key.content().Equals(name))
                {
                    key.set(value);
                    return;
                }
            }
            key = (CLKey)CLKey.allocate(name, value);
            mElements.Add(key);
        }

        public virtual void putNumber(string name, float value)
        {
            put(name, new CLNumber(value));
        }

        public virtual void remove(string name)
        {
            List<CLElement> toRemove = new List<CLElement>();
            foreach (CLElement element in mElements)
            {
                CLKey key = (CLKey)element;
                if (key.content().Equals(name))
                {
                    toRemove.Add(element);
                }
            }
            foreach (CLElement element in toRemove)
            {
                mElements.Remove(element);
            }
        }

        /////////////////////////////////////////////////////////////////////////
        // By name
        /////////////////////////////////////////////////////////////////////////

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public CLElement get(String name) throws CLParsingException
        public virtual CLElement get(string name)
        {
            foreach (CLElement element in mElements)
            {
                CLKey key = (CLKey)element;
                if (key.content().Equals(name))
                {
                    return key.Value;
                }
            }
            throw new CLParsingException("no element for key <" + name + ">", this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public int getInt(String name) throws CLParsingException
        public virtual int getInt(string name)
        {
            CLElement element = get(name);
            if (element != null)
            {
                return element.Int;
            }
            throw new CLParsingException("no int found for key <" + name + ">," + " found [" + element.StrClass + "] : " + element, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public float getFloat(String name) throws CLParsingException
        public virtual float getFloat(string name)
        {
            CLElement element = get(name);
            if (element != null)
            {
                return element.Float;
            }
            throw new CLParsingException("no float found for key <" + name + ">," + " found [" + element.StrClass + "] : " + element, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public CLArray getArray(String name) throws CLParsingException
        public virtual CLArray getArray(string name)
        {
            CLElement element = get(name);
            if (element is CLArray)
            {
                return (CLArray)element;
            }
            throw new CLParsingException("no array found for key <" + name + ">," + " found [" + element.StrClass + "] : " + element, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public CLObject getObject(String name) throws CLParsingException
        public virtual CLObject getObject(string name)
        {
            CLElement element = get(name);
            if (element is CLObject)
            {
                return (CLObject)element;
            }
            throw new CLParsingException("no object found for key <" + name + ">," + " found [" + element.StrClass + "] : " + element, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String getString(String name) throws CLParsingException
        public virtual string getString(string name)
        {
            CLElement element = get(name);
            if (element is CLString)
            {
                return element.content();
            }
            string strClass = null;
            if (element != null)
            {
                strClass = element.StrClass;
            }
            throw new CLParsingException("no string found for key <" + name + ">," + " found [" + strClass + "] : " + element, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public boolean getBoolean(String name) throws CLParsingException
        public virtual bool getBoolean(string name)
        {
            CLElement element = get(name);
            if (element is CLToken)
            {
                return ((CLToken)element).Boolean;
            }
            throw new CLParsingException("no boolean found for key <" + name + ">," + " found [" + element.StrClass + "] : " + element, this);
        }

        /////////////////////////////////////////////////////////////////////////
        // Optional
        /////////////////////////////////////////////////////////////////////////

        public virtual CLElement getOrNull(string name)
        {
            foreach (CLElement element in mElements)
            {
                CLKey key = (CLKey)element;
                if (key.content().Equals(name))
                {
                    return key.Value;
                }
            }
            return null;
        }

        public virtual CLObject getObjectOrNull(string name)
        {
            CLElement element = getOrNull(name);
            if (element is CLObject)
            {
                return (CLObject)element;
            }
            return null;
        }

        public virtual CLArray getArrayOrNull(string name)
        {
            CLElement element = getOrNull(name);
            if (element is CLArray)
            {
                return (CLArray)element;
            }
            return null;
        }

        public virtual string getStringOrNull(string name)
        {
            CLElement element = getOrNull(name);
            if (element is CLString)
            {
                return element.content();
            }
            return null;
        }

        public virtual float getFloatOrNaN(string name)
        {
            CLElement element = getOrNull(name);
            if (element is CLNumber)
            {
                return element.Float;
            }
            return float.NaN;
        }

        /////////////////////////////////////////////////////////////////////////
        // By index
        /////////////////////////////////////////////////////////////////////////

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public CLElement get(int index) throws CLParsingException
        public virtual CLElement get(int index)
        {
            if (index >= 0 && index < mElements.Count)
            {
                return mElements[index];
            }
            throw new CLParsingException("no element at index " + index, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public int getInt(int index) throws CLParsingException
        public virtual int getInt(int index)
        {
            CLElement element = get(index);
            if (element != null)
            {
                return element.Int;
            }
            throw new CLParsingException("no int at index " + index, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public float getFloat(int index) throws CLParsingException
        public virtual float getFloat(int index)
        {
            CLElement element = get(index);
            if (element != null)
            {
                return element.Float;
            }
            throw new CLParsingException("no float at index " + index, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public CLArray getArray(int index) throws CLParsingException
        public virtual CLArray getArray(int index)
        {
            CLElement element = get(index);
            if (element is CLArray)
            {
                return (CLArray)element;
            }
            throw new CLParsingException("no array at index " + index, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public CLObject getObject(int index) throws CLParsingException
        public virtual CLObject getObject(int index)
        {
            CLElement element = get(index);
            if (element is CLObject)
            {
                return (CLObject)element;
            }
            throw new CLParsingException("no object at index " + index, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String getString(int index) throws CLParsingException
        public virtual string getString(int index)
        {
            CLElement element = get(index);
            if (element is CLString)
            {
                return element.content();
            }
            throw new CLParsingException("no string at index " + index, this);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public boolean getBoolean(int index) throws CLParsingException
        public virtual bool getBoolean(int index)
        {
            CLElement element = get(index);
            if (element is CLToken)
            {
                return ((CLToken)element).Boolean;
            }
            throw new CLParsingException("no boolean at index " + index, this);
        }

        /////////////////////////////////////////////////////////////////////////
        // Optional
        /////////////////////////////////////////////////////////////////////////

        public virtual CLElement getOrNull(int index)
        {
            if (index >= 0 && index < mElements.Count)
            {
                return mElements[index];
            }
            return null;
        }

        public virtual string getStringOrNull(int index)
        {
            CLElement element = getOrNull(index);
            if (element is CLString)
            {
                return element.content();
            }
            return null;
        }
    }

}