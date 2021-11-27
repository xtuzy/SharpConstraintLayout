using NUnit.Framework;
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
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertTrue;



    [TestFixture]
    public class CLParserBenchmarkTest
    {

        internal string simpleFromWiki2 = "{\n" + "  firstName: 'John',\n" + "  lastName: 'Smith',\n" + "  isAlive: true,\n" + "  age: 27,\n" + "  address: {\n" + "    streetAddress: '21 2nd Street',\n" + "    city: 'New York',\n" + "    state: 'NY',\n" + "    postalCode: '10021-3100'\n" + "  },\n" + "  phoneNumbers: [\n" + "    {\n" + "      type: 'home',\n" + "      number: '212 555-1234'\n" + "    },\n" + "    {\n" + "      type: 'office',\n" + "      number: '646 555-4567'\n" + "    }\n" + "  ],\n" + "  children: [],\n" + "  spouse: null\n" + "}";

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void parseAndCheck1000x()
        [Test]
        public virtual void ParseAndCheck1000xTest()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    parseAndeCheck();
                }
            }
            catch (CLParsingException e)
            {
                Console.Fail("Exception " + e.reason());
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Assert.IsTrue(false);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void parse1000x()
        public virtual void parse1000x()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    parseOnce();
                }
                parseAndeCheck();
            }
            catch (CLParsingException e)
            {
                Console.Fail("Exception " + e.reason());
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Assert.IsTrue(false);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void parseOnce() throws androidx.constraintlayout.core.parser.CLParsingException
        private void parseOnce()
        {
            string test = simpleFromWiki2;
            CLObject parsedContent = CLParser.parse(test);
            CLObject o;

            Assert.AreEqual("John", parsedContent.getString("firstName"));
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void parseAndeCheck() throws androidx.constraintlayout.core.parser.CLParsingException
        private void parseAndeCheck()
        {
            string test = simpleFromWiki2;
            CLObject parsedContent = CLParser.parse(test);
            CLObject o;

            Assert.AreEqual("John", parsedContent.getString("firstName"));
            Assert.AreEqual("Smith", parsedContent.getString("lastName"));
            Assert.AreEqual(true, parsedContent.getBoolean("isAlive"));
            Assert.AreEqual(27, parsedContent.getInt("age"));
            Assert.AreEqual("{ streetAddress: '21 2nd Street', city: 'New York', state: 'NY', postalCode: '10021-3100' }", (o = parsedContent.getObject("address")).toJSON());
            Assert.AreEqual("21 2nd Street", o.getString("streetAddress"));
            Assert.AreEqual("New York", o.getString("city"));
            Assert.AreEqual("NY", o.getString("state"));
            Assert.AreEqual("NY", o.getString("state"));
            Assert.AreEqual("NY", o.getString("state"));
            Assert.AreEqual("NY", o.getString("state"));
            Assert.AreEqual("10021-3100", o.getString("postalCode"));
            Assert.AreEqual("{ type: 'home', number: '212 555-1234' }", (o = parsedContent.getArray("phoneNumbers").getObject(0)).toJSON());
            Assert.AreEqual("home", o.getString("type"));
            Assert.AreEqual("212 555-1234", o.getString("number"));
            Assert.AreEqual("{ type: 'office', number: '646 555-4567' }", (o = parsedContent.getArray("phoneNumbers").getObject(1)).toJSON());
            Assert.AreEqual("office", o.getString("type"));
            Assert.AreEqual("646 555-4567", o.getString("number"));
            Assert.AreEqual(0, parsedContent.getArray("children").mElements.Count);
            CLElement element = parsedContent.get("spouse");
            if (element is CLToken)
            {
                CLToken token = (CLToken)element;
                Assert.AreEqual(CLToken.Type.NULL, token.getType());
            }
        }
    }

}