using System.Collections;
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

    public class CLObject : CLContainer, IEnumerable //IEnumerable<CLKey>
    {

        public CLObject(char[] content) : base(content)
        {
        }

        public static CLObject allocate(char[] content)
        {
            return new CLObject(content);
        }

        public override string toJSON()
        {
            StringBuilder json = new StringBuilder(DebugName + "{ ");
            bool first = true;
            foreach (CLElement element in mElements)
            {
                if (!first)
                {
                    json.Append(", ");
                }
                else
                {
                    first = false;
                }
                json.Append(element.toJSON());
            }
            json.Append(" }");
            return json.ToString();
        }

        public virtual string toFormattedJSON()
        {
            return toFormattedJSON(0, 0);
        }

        protected internal override string toFormattedJSON(int indent, int forceIndent)
        {
            StringBuilder json = new StringBuilder(DebugName);
            json.Append("{\n");
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
                json.Append(element.toFormattedJSON(indent + BASE_INDENT, forceIndent - 1));
            }
            json.Append("\n");
            addIndent(json, indent);
            json.Append("}");
            return json.ToString();
        }

        public virtual IEnumerator GetEnumerator()
        {
            return new CLObjectIterator(this, this);
        }

        private class CLObjectIterator : IEnumerator
        {
            private readonly CLObject outerInstance;

            internal CLObject myObject;
            internal int index = 0;
            public CLObjectIterator(CLObject outerInstance, CLObject clObject)
            {
                this.outerInstance = outerInstance;
                myObject = clObject;
            }

            public object Current => (CLKey)myObject.mElements[index];

            /*public override bool hasNext()
            {
                return index < myObject.size();
            }*/

            //TODO:验证枚举写法是否正确
            public bool MoveNext()
            {
                if(index+1 < myObject.size())
                {
                    index++;
                    return true;
                }
                
                return false;
            }

            /*public override object next()
            {
                CLKey key = (CLKey)myObject.mElements[index];
                index++;
                return key;
            }*/

            public void Reset()
            {
                index = 0;
            }
        }
    }

}