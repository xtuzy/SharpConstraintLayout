﻿using NUnit.Framework;
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
    //using Test = org.junit.Test;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertTrue;


    /// <summary>
    /// Test nested layout
    /// </summary>
    [TestFixture]
    public class ArrayLinkedVariablesTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testNestedLayout()
        [Test]
        public virtual void testNestedLayout()
        {
            Cache cache = new Cache();
            ArrayRow row = new ArrayRow(cache);
            ArrayLinkedVariables variables = new ArrayLinkedVariables(row, cache);
            SolverVariable[] v = new SolverVariable[9];
            for (int i = 0; i < v.Length; i++)
            {
                int p = i ^ 3;
                v[i] = new SolverVariable("dog" + p + "(" + i + ")" + p, SolverVariable.Type.UNRESTRICTED);
                cache.GetFieldValue<SolverVariable[]>("mIndexedVariables")[i] = v[i];
                v[i].id = i;
                variables.add(v[i], 20f, true);
                if (i % 2 == 1)
                {
                    variables.remove(v[i / 2], true);

                }
                variables.display();
                //Console.WriteLine();
            }
            for (int i = 0; i < v.Length; i++)
            {
                if (i % 2 == 1)
                {
                    variables.display();
                    variables.add(v[i / 2], 24f, true);
                }
            }
            Assert.True(true);
        }
    }
}