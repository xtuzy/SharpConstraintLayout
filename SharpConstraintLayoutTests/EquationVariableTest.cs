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

using NUnit.Framework;

namespace androidx.constraintlayout.core
{
    //using Before = org.junit.Before;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.*;

    //[SetUpFixture]
    [TestFixture]
    public class EquationVariableTest
    {
        internal LinearSystem s;
        internal EquationVariable e1;
        internal EquationVariable e2;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        [SetUp]
        public virtual void setUp()
        {
            s = new LinearSystem();
            e1 = new EquationVariable(s, 200);
            e2 = new EquationVariable(s, 200);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testEquality()
        [Test]
        public virtual void testEquality()
        {
            Assert.True(e1.Amount.Equals(e2.Amount));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testAddition()
        [Test]
        public virtual void testAddition()
        {
            e1.add(e2);
            Assert.AreEqual(e1.Amount.Numerator, 400);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testSubtraction()
        [Test]
        public virtual void testSubtraction()
        {
            e1.subtract(e2);
            Assert.AreEqual(e1.Amount.Numerator, 0);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testMultiply()
        [Test]
        public virtual void testMultiply()
        {
            e1.multiply(e2);
            Assert.AreEqual(e1.Amount.Numerator, 40000);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDivide()
        [Test]
        public virtual void testDivide()
        {
            e1.divide(e2);
            Assert.AreEqual(e1.Amount.Numerator, 1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testCompatible()
        [Test]
        public virtual void testCompatible()
        {
            Assert.True(e1.isCompatible(e2));
            e2 = new EquationVariable(s, 200, "TEST", SolverVariable.Type.UNRESTRICTED);
            Assert.False(e1.isCompatible(e2));
        }
    }

}