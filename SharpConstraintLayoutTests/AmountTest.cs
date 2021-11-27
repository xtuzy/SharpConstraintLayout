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
    using NUnit.Framework;
    //using Before = org.junit.Before;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.*;

   // [SetUpFixture]
    [TestFixture]
    public class AmountTest
    {
        internal Amount a1 = new Amount(2, 3);
        internal Amount a2 = new Amount(3, 5);

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        [SetUp]
        public virtual void setUp()
        {
            a1.set(2, 3);
            a2.set(3, 5);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testAdd()
        [Test]
        public virtual void testAdd()
        {
            a1.add(a2);
            Assert.AreEqual(a1.Numerator, 19);
            Assert.AreEqual(a1.Denominator, 15);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSubtract()
        [Test]
        public virtual void testSubtract()
        {
            a1.subtract(a2);
            Assert.AreEqual(a1.Numerator, 1);
            Assert.AreEqual(a1.Denominator, 15);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testMultiply()
        [Test]
        public virtual void testMultiply()
        {
            a1.multiply(a2);
            Assert.AreEqual(a1.Numerator, 2);
            Assert.AreEqual(a1.Denominator, 5);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDivide()
        [Test]
        public virtual void testDivide()
        {
            a1.divide(a2);
            Assert.AreEqual(a1.Numerator, 10);
            Assert.AreEqual(a1.Denominator, 9);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSimplify()
        [Test]
        public virtual void testSimplify()
        {
            a1.set(20, 30);
            Assert.AreEqual(a1.Numerator, 2);
            Assert.AreEqual(a1.Denominator, 3);
            a1.set(77, 88);
            Assert.AreEqual(a1.Numerator, 7);
            Assert.AreEqual(a1.Denominator, 8);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testEquality()
        [Test]
        public virtual void testEquality()
        {
            a2.set(a1.Numerator, a1.Denominator);
            Assert.True(a1.Equals(a2));
        }
    }

}