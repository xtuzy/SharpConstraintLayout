﻿using NUnit.Framework;
using System;

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
    //using Before = org.junit.Before;
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.*;
    [TestFixture]
    public class LinearSystemTest
    {

        internal LinearSystem s;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        [SetUp]
        public virtual void setUp()
        {
            s = new LinearSystem();
            LinearEquation.resetNaming();
        }

        internal virtual void add(LinearEquation equation)
        {
            ArrayRow row1 = LinearEquation.createRowFromEquation(s, equation);
            s.addConstraint(row1);
        }

        internal virtual void add(LinearEquation equation, int strength)
        {
            Console.WriteLine("Add equation <" + equation + ">");
            ArrayRow row1 = LinearEquation.createRowFromEquation(s, equation);
            Console.WriteLine("Add equation row <" + row1 + ">");
            row1.addError(s, strength);
            s.addConstraint(row1);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testMinMax()
        [Test]
        public virtual void testMinMax()
        {
            // this shows how basic min/max + wrap works. Need to modify ConstraintWidget to generate this.
            //        solver.addConstraint(new ClLinearEquation(Rl, 0));
            //        solver.addConstraint(new ClLinearEquation(Al, 0));
            //        solver.addConstraint(new ClLinearEquation(Bl, 0));
            //        solver.addConstraint(new ClLinearEquation(Br, Plus(Bl, 1000)));
            //        solver.addConstraint(new ClLinearEquation(Al, new ClLinearExpression(Rl), ClStrength.weak));
            //        solver.addConstraint(new ClLinearEquation(Ar, new ClLinearExpression(Rr), ClStrength.weak));
            //        solver.addConstraint(new ClLinearInequality(Ar, GEQ, Plus(Al, 150), ClStrength.medium));
            //        solver.addConstraint(new ClLinearInequality(Ar, LEQ, Plus(Al, 200), ClStrength.medium));
            //        solver.addConstraint(new ClLinearInequality(Rr, GEQ, new ClLinearExpression(Br)));
            //        solver.addConstraint(new ClLinearInequality(Rr, GEQ, new ClLinearExpression(Ar)));
            add((new LinearEquation(s)).@var("Rl").equalsTo().@var(0));
            //        add(new LinearEquation(s).var("Al").equalsTo().var(0));
            //        add(new LinearEquation(s).var("Bl").equalsTo().var(0));
            add((new LinearEquation(s)).@var("Br").equalsTo().@var("Bl").plus(300));
            add((new LinearEquation(s)).@var("Al").equalsTo().@var("Rl"), 1);
            add((new LinearEquation(s)).@var("Ar").equalsTo().@var("Rr"), 1);
            add((new LinearEquation(s)).@var("Ar").greaterThan().@var("Al").plus(150), 2);
            add((new LinearEquation(s)).@var("Ar").lowerThan().@var("Al").plus(200), 2);
            add((new LinearEquation(s)).@var("Rr").greaterThan().@var("Ar"));
            add((new LinearEquation(s)).@var("Rr").greaterThan().@var("Br"));
            add((new LinearEquation(s)).@var("Al").minus("Rl").equalsTo().@var("Rr").minus("Ar"));
            add((new LinearEquation(s)).@var("Bl").minus("Rl").equalsTo().@var("Rr").minus("Br"));
            try
            {
                s.minimize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            Console.WriteLine("Result: ");
            s.displayReadableRows();
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Al" }), 50.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Ar" }), 250.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Bl" }), 0.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Br" }), 300.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Rr" }), 300.0f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPriorityBasic()
        [Test]
        public virtual void testPriorityBasic()
        {
            add((new LinearEquation(s)).@var(2, "Xm").equalsTo().@var("Xl").plus("Xr"));
            add((new LinearEquation(s)).@var("Xl").plus(10).lowerThan().@var("Xr"));
            //       add(new LinearEquation(s).var("Xl").greaterThan().var(0));
            add((new LinearEquation(s)).@var("Xr").lowerThan().@var(100));
            add((new LinearEquation(s)).@var("Xm").equalsTo().@var(50), 2);
            add((new LinearEquation(s)).@var("Xl").equalsTo().@var(30), 1);
            add((new LinearEquation(s)).@var("Xr").equalsTo().@var(60), 1);
            try
            {
                s.minimize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            Console.WriteLine("Result: ");
            s.displayReadableRows();
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Xm" }), 50.0f, 0f); // 50
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Xl" }), 40.0f, 0f); // 30
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "Xr" }), 60.0f, 0f); // 70
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPriorities()
        [Test]
        public virtual void testPriorities()
        {
            // | <- a -> | b
            // a - zero = c - a
            // 2a = c + zero
            // a = (c + zero ) / 2
            add((new LinearEquation(s)).@var("b").equalsTo().@var(100), 3);
            add((new LinearEquation(s)).@var("zero").equalsTo().@var(0), 3);
            add((new LinearEquation(s)).@var("a").equalsTo().@var(300), 0);
            add((new LinearEquation(s)).@var("c").equalsTo().@var(200), 0);

            add((new LinearEquation(s)).@var("c").lowerThan().@var("b").minus(10), 2);
            add((new LinearEquation(s)).@var("a").lowerThan().@var("c"), 2);

            add((new LinearEquation(s)).@var("a").minus("zero").equalsTo().@var("c").minus("a"), 1);

            try
            {
                s.minimize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            Console.WriteLine("Result: ");
            s.displayReadableRows();
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "zero" }), 0.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "a" }), 45.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "b" }), 100.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "c" }), 90.0f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testOptimizeAndPriority()
        [Test]
        public virtual void testOptimizeAndPriority()
        {
            s.reset();
            LinearEquation eq1 = new LinearEquation(s);
            LinearEquation eq2 = new LinearEquation(s);
            LinearEquation eq3 = new LinearEquation(s);
            LinearEquation eq4 = new LinearEquation(s);
            LinearEquation eq5 = new LinearEquation(s);
            LinearEquation eq6 = new LinearEquation(s);
            LinearEquation eq7 = new LinearEquation(s);
            LinearEquation eq8 = new LinearEquation(s);
            LinearEquation eq9 = new LinearEquation(s);
            LinearEquation eq10 = new LinearEquation(s);

            eq1.@var("Root.left").equalsTo().@var(0);
            eq2.@var("Root.right").equalsTo().@var(600);
            eq3.@var("A.right").equalsTo().@var("A.left").plus(100); //*
            eq4.@var("A.left").greaterThan().@var("Root.left"); //*
            eq10.@var("A.left").equalsTo().@var("Root.left"); //*
            eq5.@var("A.right").lowerThan().@var("B.left");
            eq6.@var("B.right").greaterThan().@var("B.left");
            eq7.@var("B.right").lowerThan().@var("Root.right");
            eq8.@var("B.left").equalsTo().@var("A.right");
            eq9.@var("B.right").greaterThan().@var("Root.right");

            ArrayRow row1 = LinearEquation.createRowFromEquation(s, eq1);
            s.addConstraint(row1);

            ArrayRow row2 = LinearEquation.createRowFromEquation(s, eq2);
            s.addConstraint(row2);

            ArrayRow row3 = LinearEquation.createRowFromEquation(s, eq3);
            s.addConstraint(row3);

            ArrayRow row10 = LinearEquation.createRowFromEquation(s, eq10);
            s.RunMethod("addSingleError", new object[] { row10, 1, SolverVariable.STRENGTH_MEDIUM });
            s.RunMethod("addSingleError", new object[] { row10, -1, SolverVariable.STRENGTH_MEDIUM });
            s.addConstraint(row10);

            ArrayRow row4 = LinearEquation.createRowFromEquation(s, eq4);
            s.RunMethod("addSingleError", new object[] { row4, -1, SolverVariable.STRENGTH_HIGH });
            s.addConstraint(row4);

            ArrayRow row5 = LinearEquation.createRowFromEquation(s, eq5);
            s.RunMethod("addSingleError", new object[] { row5, 1, SolverVariable.STRENGTH_MEDIUM });
            s.addConstraint(row5);

            ArrayRow row6 = LinearEquation.createRowFromEquation(s, eq6);
            s.RunMethod("addSingleError", new object[] { row6, -1, SolverVariable.STRENGTH_LOW });
            s.addConstraint(row6);

            ArrayRow row7 = LinearEquation.createRowFromEquation(s, eq7);
            s.RunMethod("addSingleError", new object[] { row7, 1, SolverVariable.STRENGTH_LOW });
            s.addConstraint(row7);

            ArrayRow row8 = LinearEquation.createRowFromEquation(s, eq8);
            row8.addError(s, SolverVariable.STRENGTH_LOW);
            s.addConstraint(row8);

            ArrayRow row9 = LinearEquation.createRowFromEquation(s, eq9);
            s.RunMethod("addSingleError",new object[] { row9, -1, SolverVariable.STRENGTH_LOW });
            s.addConstraint(row9);

            try
            {
                s.minimize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testPriority()
        [Test]
        public virtual void testPriority()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("\n*** TEST PRIORITY ***\n");
                s.reset();
                LinearEquation eq1 = new LinearEquation(s);
                eq1.@var("A").equalsTo().@var(10);
                ArrayRow row1 = LinearEquation.createRowFromEquation(s, eq1);
                row1.addError(s, i % 3);
                s.addConstraint(row1);

                LinearEquation eq2 = new LinearEquation(s);
                eq2.@var("A").equalsTo().@var(100);
                ArrayRow row2 = LinearEquation.createRowFromEquation(s, eq2);
                row2.addError(s, (i + 1) % 3);
                s.addConstraint(row2);

                LinearEquation eq3 = new LinearEquation(s);
                eq3.@var("A").equalsTo().@var(1000);
                ArrayRow row3 = LinearEquation.createRowFromEquation(s, eq3);
                row3.addError(s, (i + 2) % 3);
                s.addConstraint(row3);

                try
                {
                    s.minimize();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                Console.WriteLine("Check at iteration " + i);
                s.displayReadableRows();
                if (i == 0)
                {
                    Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "A" }), 1000.0f, 0f);
                }
                else if (i == 1)
                {
                    Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "A" }), 100.0f, 0f);
                }
                else if (i == 2)
                {
                    Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "A" }), 10.0f, 0f);
                }
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testAddEquation1()
        [Test]
        public virtual void testAddEquation1()
        {
            LinearEquation e1 = new LinearEquation(s);
            e1.@var("W3.left").equalsTo().@var(0);
            s.addConstraint(LinearEquation.createRowFromEquation(s, e1));
            //s.rebuildGoalFromErrors();
            string result = s.GetPropertyValue<LinearSystem.Row>("Goal").ToString();
            //Assert.True((result.Equals("0 = 0.0") || result.Equals(" goal -> (0.0) : ")));
            Assert.True((result.Equals("0 = 0.0") || result.Equals(" goal -> (0) : ")));//我认为他们是一样的
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "W3.left" }), 0.0f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testAddEquation2()
        [Test]
        public virtual void testAddEquation2()
        {
            LinearEquation e1 = new LinearEquation(s);
            e1.@var("W3.left").equalsTo().@var(0);
            LinearEquation e2 = new LinearEquation(s);
            e2.@var("W3.right").equalsTo().@var(600);
            s.addConstraint(LinearEquation.createRowFromEquation(s, e1));
            s.addConstraint(LinearEquation.createRowFromEquation(s, e2));
            //s.rebuildGoalFromErrors();
            string result = s.GetPropertyValue<LinearSystem.Row>("Goal").ToString();
            //Assert.True((result.Equals("0 = 0.0") || result.Equals(" goal -> (0.0) : ")));
            Assert.True((result.Equals("0 = 0.0") || result.Equals(" goal -> (0) : ")));//我认为他们是一样的
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "W3.left" }), 0.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "W3.right" }), 600.0f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testAddEquation3()
        [Test]
        public virtual void testAddEquation3()
        {
            LinearEquation e1 = new LinearEquation(s);
            e1.@var("W3.left").equalsTo().@var(0);
            LinearEquation e2 = new LinearEquation(s);
            e2.@var("W3.right").equalsTo().@var(600);
            LinearEquation left_constraint = new LinearEquation(s);
            left_constraint.@var("W4.left").equalsTo().@var("W3.left"); // left constraint
            s.addConstraint(LinearEquation.createRowFromEquation(s, e1));
            s.addConstraint(LinearEquation.createRowFromEquation(s, e2));
            s.addConstraint(LinearEquation.createRowFromEquation(s, left_constraint)); // left
                                                                                       //s.rebuildGoalFromErrors();
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "W3.left" }), 0.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "W3.right" }), 600.0f, 0f);
            Assert.AreEqual((float)s.RunMethod("getValueFor", new object[] { "W4.left" }), 0.0f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testAddEquation4()
        [Test]
        public virtual void testAddEquation4()
        {
            LinearEquation e1 = new LinearEquation(s);
            LinearEquation e2 = new LinearEquation(s);
            LinearEquation e3 = new LinearEquation(s);
            LinearEquation e4 = new LinearEquation(s);
            e1.@var(2, "Xm").equalsTo().@var("Xl").plus("Xr");
            LinearSystem.Row goalRow = s.GetPropertyValue<LinearSystem.Row>("Goal");
            s.addConstraint(LinearEquation.createRowFromEquation(s, e1)); // 2 Xm = Xl + Xr
            goalRow.addError((SolverVariable)s.RunMethod("getVariable", new object[] { "Xm", SolverVariable.Type.ERROR }));
            goalRow.addError((SolverVariable)s.RunMethod("getVariable", new object[] { "Xl", SolverVariable.Type.ERROR }));
            //        Assert.AreEqual(s.getRow(0).toReadableString(), "Xm = 0.5 Xl + 0.5 Xr", 0f);
            e2.@var("Xl").plus(10).lowerThan().@var("Xr");
            s.addConstraint(LinearEquation.createRowFromEquation(s, e2)); // Xl + 10 <= Xr

            //        Assert.AreEqual(s.getRow(0).toReadableString(), "Xm = 5.0 + Xl + 0.5 s1", 0f);
            //        Assert.AreEqual(s.getRow(1).toReadableString(), "Xr = 10.0 + Xl + s1", 0f);
            e3.@var("Xl").greaterThan().@var(-10);
            s.addConstraint(LinearEquation.createRowFromEquation(s, e3)); // Xl >= -10
                                                                          //        Assert.AreEqual(s.getRow(0).toReadableString(), "Xm = -5.0 + 0.5 s1 + s2", 0f);
                                                                          //        Assert.AreEqual(s.getRow(1).toReadableString(), "Xr = s1 + s2", 0f);
                                                                          //        Assert.AreEqual(s.getRow(2).toReadableString(), "Xl = -10.0 + s2", 0f);
            e4.@var("Xr").lowerThan().@var(100);
            s.addConstraint(LinearEquation.createRowFromEquation(s, e4)); // Xr <= 100
                                                                          //        Assert.AreEqual(s.getRow(0).toReadableString(), "Xm = 45.0 + 0.5 s2 - 0.5 s3", 0f);
                                                                          //        Assert.AreEqual(s.getRow(1).toReadableString(), "Xr = 100.0 - s3", 0f);
                                                                          //        Assert.AreEqual(s.getRow(2).toReadableString(), "Xl = -10.0 + s2", 0f);
                                                                          //        Assert.AreEqual(s.getRow(3).toReadableString(), "s1 = 100.0 - s2 - s3", 0f);
                                                                          //s.rebuildGoalFromErrors();
                                                                          //        Assert.AreEqual(s.getGoal().toString(), "Goal: ", 0f);
            LinearEquation goal = new LinearEquation(s);
            goal.@var("Xm").minus("Xl");
            try
            {
                s.RunMethod("minimizeGoal", new object[] { LinearEquation.createRowFromEquation(s, goal) }); //s.getGoal());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            int xl = (int)s.RunMethod<float>("getValueFor", new object[] { "Xl" });
            int xm = (int)s.RunMethod<float>("getValueFor", new object[] { "Xm" });
            int xr = (int)s.RunMethod<float>("getValueFor", new object[] { "Xr" });
            //        Assert.AreEqual(xl, -10, 0f);
            //        Assert.AreEqual(xm, 45, 0f);
            //        Assert.AreEqual(xr, 100, 0f);
            LinearEquation e5 = new LinearEquation(s);
            e5.@var("Xm").equalsTo().@var(50);
            s.addConstraint(LinearEquation.createRowFromEquation(s, e5));
            try
            {
                //            s.minimizeGoal(s.getGoal());
                //            s.minimizeGoal(LinearEquation.createRowFromEquation(s, goal)); //s.getGoal());
                s.RunMethod("minimizeGoal", new object[] { goalRow });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            xl = (int)s.RunMethod<float>("getValueFor", new object[] { "Xl" });
            xm = (int)s.RunMethod<float>("getValueFor", new object[] { "Xm" });
            xr = (int)s.RunMethod<float>("getValueFor", new object[] { "Xr" });
            Assert.AreEqual(xl, 0);
            Assert.AreEqual(xm, 50);
            Assert.AreEqual(xr, 100);
        }
    }
}