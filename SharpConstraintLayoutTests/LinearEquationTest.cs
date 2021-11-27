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

namespace androidx.constraintlayout.core
{
    //using Before = org.junit.Before;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.*;
    //[SetUpFixture]
    [TestFixture]
    public class LinearEquationTest
    {
        internal LinearSystem s;
        internal LinearEquation e;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        [SetUp]
        public virtual void setUp()
        {
            s = new LinearSystem();
            e = new LinearEquation();
            e.System = s;
            LinearEquation.resetNaming();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay1()
        [Test]
        public virtual void testDisplay1()
        {
            e.@var("A").equalsTo().@var(100);
            Assert.AreEqual(e.ToString(), "A = 100");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay2()
        [Test]
        public virtual void testDisplay2()
        {
            e.@var("A").equalsTo().@var("B");
            Assert.AreEqual(e.ToString(), "A = B");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay3()
        [Test]
        public virtual void testDisplay3()
        {
            e.@var("A").greaterThan().@var("B");
            Assert.AreEqual(e.ToString(), "A >= B");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay4()
        [Test]
        public virtual void testDisplay4()
        {
            e.@var("A").lowerThan().@var("B");
            Assert.AreEqual(e.ToString(), "A <= B");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay5()
        [Test]
        public virtual void testDisplay5()
        {
            e.@var("A").greaterThan().@var("B").plus(100);
            Assert.AreEqual(e.ToString(), "A >= B + 100");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay6()
        [Test]
        public virtual void testDisplay6()
        {
            e.@var("A").plus("B").minus("C").plus(50).greaterThan().@var("B").plus("C").minus(100);
            Assert.AreEqual(e.ToString(), "A + B - C + 50 >= B + C - 100");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay7()
        [Test]
        public virtual void testDisplay7()
        {
            e.@var("A").lowerThan().@var("B");
            e.normalize();
            Assert.AreEqual(e.ToString(), "A + s1 = B");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay8()
        [Test]
        public virtual void testDisplay8()
        {
            e.@var("A").greaterThan().@var("B");
            e.normalize();
            Assert.AreEqual(e.ToString(), "A - s1 = B");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplay9()
        [Test]
        public virtual void testDisplay9()
        {
            e.@var("A").greaterThan().@var("B").withError();
            e.normalize();
            Assert.AreEqual(e.ToString(), "A - s1 = B + e1+ - e1-");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplaySimplify()
        [Test]
        public virtual void testDisplaySimplify()
        {
            e.@var("A").plus(5).minus(2).plus(2, "B").minus(3, "B").greaterThan().@var("C").minus(3, "C").withError();
            Assert.AreEqual(e.ToString(), "A + 5 - 2 + 2 B - 3 B >= C - 3 C + e1+ - e1-");
            e.normalize();
            Assert.AreEqual(e.ToString(), "A + 5 - 2 + 2 B - 3 B - s1 = C - 3 C + e1+ - e1-");
            e.simplify();
            Assert.AreEqual(e.ToString(), "3 + A - B - s1 = - 2 C + e1+ - e1-");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplayBalance1()
        [Test]
        public virtual void testDisplayBalance1()
        {
            e.@var("A").plus(5).minus(2).plus(2, "B").minus(3, "B").greaterThan().@var("C").minus(3, "C").withError();
            e.normalize();
            try
            {
                e.balance();
            }
            catch (Exception e)
            {
                Console.Fail("Exception raised: " + e);
            }
            Assert.AreEqual(e.ToString(), "A = - 3 + B - 2 C + e1+ - e1- + s1");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplayBalance2()
        [Test]
        public virtual void testDisplayBalance2()
        {
            e.plus(5).minus(2).minus(2, "A").minus(3, "B").equalsTo().@var(5, "C");
            try
            {
                e.balance();
            }
            catch (Exception e)
            {
                Console.Fail("Exception raised: " + e);
            }
            Assert.AreEqual(e.ToString(), "A = 3/2 - 3/2 B - 5/2 C");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplayBalance3()
        [Test]
        public virtual void testDisplayBalance3()
        {
            e.plus(5).equalsTo().@var(3);
            try
            {
                e.balance();
            }
            catch (Exception)
            {
                Assert.True(true);
            }
            Assert.False(false);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplayBalance4()
        [Test]
        public virtual void testDisplayBalance4()
        {
            // s1 = - 200 - e1- + 236 + e1- + e2+ - e2-
            e.withSlack().equalsTo().@var(-200).withError("e1-", -1).plus(236);
            e.withError("e1-", 1).withError("e2+", 1).withError("e2-", -1);
            try
            {
                e.balance();
            }
            catch (Exception e)
            {
                Console.Fail("Exception raised: " + e);
            }
            Assert.AreEqual(e.ToString(), "s1 = 36 + e2+ - e2-");
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testDisplayBalance5()
        [Test]
        public virtual void testDisplayBalance5()
        {
            // 236 + e1- + e2+ - e2- = e1- - e2+ + e2-
            e.@var(236).withError("e1-", 1).withError("e2+", 1).withError("e2-", -1);
            e.equalsTo().withError("e1-", 1).withError("e2+", -1).withError("e2-", 1);
            try
            {
                e.balance();
            }
            catch (Exception e)
            {
                Console.Fail("Exception raised: " + e);
            }
            // 236 + e1- + e2+ - e2- = e1- - e2+ + e2-
            // 0 = e1- - e2+ + e2- -236 -e1- - e2+ + e2-
            // 0 =     - e2+ + e2- -236      - e2+ + e2-
            // 0 = -236 - 2 e2+ + 2 e2-
            // 2 e2+ = -236 + 2 e2-
            // e2+ = -118 + e2-
            Assert.AreEqual(e.ToString(), "e2+ = - 118 + e2-");
        }
    }

}