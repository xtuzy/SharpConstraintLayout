using NUnit.Framework;
using System;
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
namespace androidx.constraintlayout.core
{
    //using Test = org.junit.Test;


    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    [TestFixture]
    public class SolverVariableValuesTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testOperations()
        [Test]
        public virtual void testOperations()
        {

            Cache mCache = new Cache();
            SolverVariable variable5 = new SolverVariable("v5", SolverVariable.Type.SLACK);
            SolverVariable variable1 = new SolverVariable("v1", SolverVariable.Type.SLACK);
            SolverVariable variable3 = new SolverVariable("v3", SolverVariable.Type.SLACK);
            SolverVariable variable7 = new SolverVariable("v7", SolverVariable.Type.SLACK);
            SolverVariable variable11 = new SolverVariable("v11", SolverVariable.Type.SLACK);
            SolverVariable variable12 = new SolverVariable("v12", SolverVariable.Type.SLACK);

            variable5.id = 5;
            variable1.id = 1;
            variable3.id = 3;
            variable7.id = 7;
            variable11.id = 11;
            variable12.id = 12;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable5.id] = variable5;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable1.id] = variable1;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable3.id] = variable3;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable7.id] = variable7;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable11.id] = variable11;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable12.id] = variable12;

            SolverVariableValues values = new SolverVariableValues(null, mCache);
            values.put(variable5, 1f);
            Console.WriteLine(values);
            values.put(variable1, -1f);
            Console.WriteLine(values);
            values.put(variable3, -1f);
            Console.WriteLine(values);
            values.put(variable7, 1f);
            Console.WriteLine(values);
            values.put(variable11, 1f);
            Console.WriteLine(values);
            values.put(variable12, -1f);
            Console.WriteLine(values);
            values.remove(variable1, true);
            Console.WriteLine(values);
            values.remove(variable3, true);
            Console.WriteLine(values);
            values.remove(variable7, true);
            Console.WriteLine(values);
            values.add(variable5, 1f, true);
            Console.WriteLine(values);

            int currentSize = values.CurrentSize;
            for (int i = 0; i < currentSize; i++)
            {
                SolverVariable variable = values.getVariable(i);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic()
        [Test]
        public virtual void testBasic()
        {

            Cache mCache = new Cache();
            SolverVariable variable1 = new SolverVariable("A", SolverVariable.Type.SLACK);
            SolverVariable variable2 = new SolverVariable("B", SolverVariable.Type.SLACK);
            SolverVariable variable3 = new SolverVariable("C", SolverVariable.Type.SLACK);
            variable1.id = 0;
            variable2.id = 1;
            variable3.id = 2;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable1.id] = variable1;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable2.id] = variable2;
            mCache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[variable3.id] = variable3;
            SolverVariableValues values = new SolverVariableValues(null, mCache);

            variable1.id = 10;
            variable2.id = 100;
            variable3.id = 1000;

            values.put(variable1, 1);
            values.put(variable2, 2);
            values.put(variable3, 3);

            float v1 = values.get(variable1);
            float v2 = values.get(variable2);
            float v3 = values.get(variable3);
            Assert.AreEqual(v1, 1f, 0f);
            Assert.AreEqual(v2, 2f, 0f);
            Assert.AreEqual(v3, 3f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic2()
        [Test]
        public virtual void testBasic2()
        {
            Cache mCache = new Cache();
            SolverVariableValues values = new SolverVariableValues(null, mCache);
            SolverVariable variable1 = new SolverVariable("A", SolverVariable.Type.SLACK);
            SolverVariable variable2 = new SolverVariable("B", SolverVariable.Type.SLACK);
            SolverVariable variable3 = new SolverVariable("C", SolverVariable.Type.SLACK);

            variable1.id = 32;
            variable2.id = 32 * 2;
            variable3.id = 32 * 3;

            values.put(variable1, 1);
            values.put(variable2, 2);
            values.put(variable3, 3);

            float v1 = values.get(variable1);
            float v2 = values.get(variable2);
            float v3 = values.get(variable3);
            Assert.AreEqual(v1, 1f, 0f);
            Assert.AreEqual(v2, 2f, 0f);
            Assert.AreEqual(v3, 3f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic3()
        [Test]
        public virtual void testBasic3()
        {
            Cache mCache = new Cache();
            SolverVariableValues values = new SolverVariableValues(null, mCache);
            List<SolverVariable> variables = new List<SolverVariable>();
            for (int i = 0; i < 10000; i++)
            {
                SolverVariable variable = new SolverVariable("A" + i, SolverVariable.Type.SLACK);
                variable.id = i * 32;
                values.put(variable, i);
                variables.Add(variable);
            }
            int j = 0;
            foreach (SolverVariable variable in variables)
            {
                float value = j;
                Assert.AreEqual(value, values.get(variable), 0f);
                j++;
            }
            //        System.out.println("array size: count: " + values.count + " keys: " + values.keys.length + " values: " + values.values.length);
            //        values.maxDepth();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic4()
        [Test]
        public virtual void testBasic4()
        {
            Cache mCache = new Cache();
            SolverVariableValues values = new SolverVariableValues(null, mCache);
            List<SolverVariable> variables = new List<SolverVariable>();
            for (int i = 0; i < 10000; i++)
            {
                SolverVariable variable = new SolverVariable("A" + i, SolverVariable.Type.SLACK);
                variable.id = i;
                values.put(variable, i);
                variables.Add(variable);
            }
            int j = 0;
            foreach (SolverVariable variable in variables)
            {
                float value = j;
                Assert.AreEqual(value, values.get(variable), 0f);
                j++;
            }
            //        System.out.println("array size: count: " + values.count + " keys: " + values.keys.length + " values: " + values.values.length);
            //        values.maxDepth();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic5()
        [Test]
        public virtual void testBasic5()
        {
            Cache mCache = new Cache();
            SolverVariableValues values = new SolverVariableValues(null, mCache);
            List<SolverVariable> variables = new List<SolverVariable>();
            for (int i = 0; i < 10000; i++)
            {
                SolverVariable variable = new SolverVariable("A" + i, SolverVariable.Type.SLACK);
                variable.id = i;
                values.put(variable, i);
                variables.Add(variable);
            }
            int j = 0;
            foreach (SolverVariable variable in variables)
            {
                if (j % 2 == 0)
                {
                    values.remove(variable, false);
                }
                j++;
            }
            j = 0;
            foreach (SolverVariable variable in variables)
            {
                float value = j;
                if (j % 2 != 0)
                {
                    Assert.AreEqual(value, values.get(variable), 0f);
                }
                j++;
            }
            //        System.out.println("array size: count: " + values.count + " keys: " + values.keys.length + " values: " + values.values.length);
            //        values.maxDepth();
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic6()
        [Test]
        public virtual void testBasic6()
        {
            Cache mCache = new Cache();
            SolverVariableValues values = new SolverVariableValues(null, mCache);
            List<SolverVariable> variables = new List<SolverVariable>();
            Dictionary<SolverVariable, float?> results = new Dictionary<SolverVariable, float?>();
            for (int i = 0; i < 100; i++)
            {
                SolverVariable variable = new SolverVariable("A" + i, SolverVariable.Type.SLACK);
                variable.id = i;
                values.put(variable, i);
                results[variable] = (float)i;
                variables.Add(variable);
            }
            List<SolverVariable> toRemove = new List<SolverVariable>();
            Random random = new Random(1234);
            foreach (SolverVariable variable in variables)
            {
                //if (random.nextFloat() > 0.3f)
                if (random.NextDouble() > 0.3f)
                {
                    toRemove.Add(variable);
                }
            }
            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //variables.removeAll(toRemove);
            foreach(var item in toRemove)
            {
                variables.Remove(item);
            }

            for (int i = 0; i < 100; i++)
            {
                SolverVariable variable = new SolverVariable("B" + i, SolverVariable.Type.SLACK);
                variable.id = 100 + i;
                values.put(variable, i);
                results[variable] = (float)i;
                variables.Add(variable);
            }
            foreach (SolverVariable variable in variables)
            {
                float value = results[variable].Value;
                Assert.AreEqual(value, values.get(variable), 0f);
            }
            //        System.out.println("array size: count: " + values.count + " keys: " + values.keys.length + " values: " + values.values.length);
            //        values.maxDepth();
        }
    }

}