﻿using System;

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
namespace androidx.constraintlayout.core
{

    /// <summary>
    /// Store a set of variables and their values in an array-based linked list.
    /// 
    /// The general idea is that we want to store a list of variables that need to be ordered,
    /// space efficient, and relatively fast to maintain (add/remove).
    /// 
    /// ArrayBackedVariables implements a sparse array, so is rather space efficient, but maintaining
    /// the array sorted is costly, as we spend quite a bit of time recopying parts of the array on element deletion.
    /// 
    /// LinkedVariables implements a standard linked list structure, and is able to be faster than ArrayBackedVariables
    /// even though it's more costly to set up (pool of objects...), as the elements removal and maintenance of the
    /// structure is a lot more efficient.
    /// 
    /// This ArrayLinkedVariables class takes inspiration from both of the above, and implement a linked list
    /// stored in several arrays. This allows us to be a lot more efficient in terms of setup (no need to deal with pool
    /// of objects...), resetting the structure, and insertion/deletion of elements.
    /// </summary>
    public class ArrayLinkedVariables : ArrayRow.ArrayRowVariables
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mArrayIndices = new int[ROW_SIZE];
            mArrayNextIndices = new int[ROW_SIZE];
            mArrayValues = new float[ROW_SIZE];
        }

        private const bool DEBUG = false;

        internal const int NONE = -1;
        private const bool FULL_NEW_CHECK = false; // full validation (debug purposes)

        internal int currentSize = 0; // current size, accessed by ArrayRow and LinearSystem

        private readonly ArrayRow mRow; // our owner
        protected internal readonly Cache mCache; // pointer to the system-wide cache, allowing access to SolverVariables

        private int ROW_SIZE = 8; // default array size

        private SolverVariable candidate = null;

        // mArrayIndices point to indexes in mCache.mIndexedVariables (i.e., the SolverVariables)
        private int[] mArrayIndices;

        // mArrayNextIndices point to indexes in mArrayIndices
        private int[] mArrayNextIndices;

        // mArrayValues contain the associated value from mArrayIndices
        private float[] mArrayValues;

        // mHead point to indexes in mArrayIndices
        private int mHead = NONE;

        // mLast point to indexes in mArrayIndices
        //
        // While mDidFillOnce is not set, mLast is simply incremented
        // monotonically in order to be sure to traverse the entire array; the idea here is that
        // when we clear a linked list, we only set the counters to zero without traversing the array to fill
        // it with NONE values, which would be costly.
        // But if we do not fill the array with NONE values, we cannot safely simply check if an entry
        // is set to NONE to know if we can use it or not, as it might contains a previous value...
        // So, when adding elements, we first ensure with this mechanism of mLast/mDidFillOnce
        // that we do traverse the array linearly, avoiding for that first pass the need to check for the value
        // of the item in mArrayIndices.
        // This does mean that removed elements will leave empty spaces, but we /then/ set the removed element
        // to NONE, so that once we did that first traversal filling the array, we can safely revert to linear traversal
        // finding an empty spot by checking the values of mArrayIndices (i.e. finding an item containing NONE).
        private int mLast = NONE;

        // flag to keep trace if we did a full pass of the array or not, see above description
        private bool mDidFillOnce = false;
        private static float epsilon = 0.001f;

        // Example of a basic loop
        // current or previous point to mArrayIndices
        //
        // int current = mHead;
        // int counter = 0;
        // while (current != NONE && counter < currentSize) {
        //  SolverVariable currentVariable = mCache.mIndexedVariables[mArrayIndices[current]];
        //  float currentValue = mArrayValues[current];
        //  ...
        //  current = mArrayNextIndices[current]; counter++;
        // }

        /// <summary>
        /// Constructor </summary>
        /// <param name="arrayRow"> the row owning us </param>
        /// <param name="cache"> instances cache </param>
        public ArrayLinkedVariables(ArrayRow arrayRow, Cache cache)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            mRow = arrayRow;
            mCache = cache;
            if (DEBUG)
            {
                for (int i = 0; i < mArrayIndices.Length; i++)
                {
                    mArrayIndices[i] = NONE;
                }
            }
        }

        /// <summary>
        /// Insert a variable with a given value in the linked list
        /// </summary>
        /// <param name="variable"> the variable to add in the list </param>
        /// <param name="value"> the value of the variable </param>
        public void put(SolverVariable variable, float value)
        {
            if (value == 0)
            {
                remove(variable, true);
                return;
            }
            // Special casing empty list...
            if (mHead == NONE)
            {
                mHead = 0;
                mArrayValues[mHead] = value;
                mArrayIndices[mHead] = variable.id;
                mArrayNextIndices[mHead] = NONE;
                variable.usageInRowCount++;
                variable.addToRow(mRow);
                currentSize++;
                if (!mDidFillOnce)
                {
                    // only increment mLast if we haven't done the first filling pass
                    mLast++;
                    if (mLast >= mArrayIndices.Length)
                    {
                        mDidFillOnce = true;
                        mLast = mArrayIndices.Length - 1;
                    }
                }
                return;
            }
            int current = mHead;
            int previous = NONE;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (mArrayIndices[current] == variable.id)
                {
                    mArrayValues[current] = value;
                    return;
                }
                if (mArrayIndices[current] < variable.id)
                {
                    previous = current;
                }
                current = mArrayNextIndices[current];
                counter++;
            }

            // Not found, we need to insert

            // First, let's find an available spot
            int availableIndice = mLast + 1; // start from the previous spot
            if (mDidFillOnce)
            {
                // ... but if we traversed the array once, check the last index, which might have been
                // set by an element removed
                if (mArrayIndices[mLast] == NONE)
                {
                    availableIndice = mLast;
                }
                else
                {
                    availableIndice = mArrayIndices.Length;
                }
            }
            if (availableIndice >= mArrayIndices.Length)
            {
                if (currentSize < mArrayIndices.Length)
                {
                    // find an available spot
                    for (int i = 0; i < mArrayIndices.Length; i++)
                    {
                        if (mArrayIndices[i] == NONE)
                        {
                            availableIndice = i;
                            break;
                        }
                    }
                }
            }
            // ... make sure to grow the array as needed
            if (availableIndice >= mArrayIndices.Length)
            {
                availableIndice = mArrayIndices.Length;
                ROW_SIZE *= 2;
                mDidFillOnce = false;
                mLast = availableIndice - 1;
                //mArrayValues = Arrays.copyOf(mArrayValues, ROW_SIZE);
                mArrayValues = mArrayValues.Copy<float>(ROW_SIZE);
                //mArrayIndices = Arrays.copyOf(mArrayIndices, ROW_SIZE);
                mArrayIndices = mArrayIndices.Copy<int>(ROW_SIZE);
                //mArrayNextIndices = Arrays.copyOf(mArrayNextIndices, ROW_SIZE);
                mArrayNextIndices = mArrayNextIndices.Copy<int>(ROW_SIZE);
            }

            // Finally, let's insert the element
            mArrayIndices[availableIndice] = variable.id;
            mArrayValues[availableIndice] = value;
            if (previous != NONE)
            {
                mArrayNextIndices[availableIndice] = mArrayNextIndices[previous];
                mArrayNextIndices[previous] = availableIndice;
            }
            else
            {
                mArrayNextIndices[availableIndice] = mHead;
                mHead = availableIndice;
            }
            variable.usageInRowCount++;
            variable.addToRow(mRow);
            currentSize++;
            if (!mDidFillOnce)
            {
                // only increment mLast if we haven't done the first filling pass
                mLast++;
            }
            if (currentSize >= mArrayIndices.Length)
            {
                mDidFillOnce = true;
            }
            if (mLast >= mArrayIndices.Length)
            {
                mDidFillOnce = true;
                mLast = mArrayIndices.Length - 1;
            }
        }

        /// <summary>
        /// Add value to an existing variable
        /// 
        /// The code is broadly identical to the put() method, only differing
        /// in in-line deletion, and of course doing an add rather than a put </summary>
        ///  <param name="variable"> the variable we want to add </param>
        /// <param name="value"> its value </param>
        /// <param name="removeFromDefinition"> </param>
        public virtual void add(SolverVariable variable, float value, bool removeFromDefinition)
        {
            if (value > -epsilon && value < epsilon)
            {
                return;
            }
            // Special casing empty list...
            if (mHead == NONE)
            {
                mHead = 0;
                mArrayValues[mHead] = value;
                mArrayIndices[mHead] = variable.id;
                mArrayNextIndices[mHead] = NONE;
                variable.usageInRowCount++;
                variable.addToRow(mRow);
                currentSize++;
                if (!mDidFillOnce)
                {
                    // only increment mLast if we haven't done the first filling pass
                    mLast++;
                    if (mLast >= mArrayIndices.Length)
                    {
                        mDidFillOnce = true;
                        mLast = mArrayIndices.Length - 1;
                    }
                }
                return;
            }
            int current = mHead;
            int previous = NONE;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                int idx = mArrayIndices[current];
                if (idx == variable.id)
                {
                    float v = mArrayValues[current] + value;
                    if (v > -epsilon && v < epsilon)
                    {
                        v = 0;
                    }
                    mArrayValues[current] = v;
                    // Possibly delete immediately
                    if (v == 0)
                    {
                        if (current == mHead)
                        {
                            mHead = mArrayNextIndices[current];
                        }
                        else
                        {
                            mArrayNextIndices[previous] = mArrayNextIndices[current];
                        }
                        if (removeFromDefinition)
                        {
                            variable.removeFromRow(mRow);
                        }
                        if (mDidFillOnce)
                        {
                            // If we did a full pass already, remember that spot
                            mLast = current;
                        }
                        variable.usageInRowCount--;
                        currentSize--;
                    }
                    return;
                }
                if (mArrayIndices[current] < variable.id)
                {
                    previous = current;
                }
                current = mArrayNextIndices[current];
                counter++;
            }

            // Not found, we need to insert

            // First, let's find an available spot
            int availableIndice = mLast + 1; // start from the previous spot
            if (mDidFillOnce)
            {
                // ... but if we traversed the array once, check the last index, which might have been
                // set by an element removed
                if (mArrayIndices[mLast] == NONE)
                {
                    availableIndice = mLast;
                }
                else
                {
                    availableIndice = mArrayIndices.Length;
                }
            }
            if (availableIndice >= mArrayIndices.Length)
            {
                if (currentSize < mArrayIndices.Length)
                {
                    // find an available spot
                    for (int i = 0; i < mArrayIndices.Length; i++)
                    {
                        if (mArrayIndices[i] == NONE)
                        {
                            availableIndice = i;
                            break;
                        }
                    }
                }
            }
            // ... make sure to grow the array as needed
            if (availableIndice >= mArrayIndices.Length)
            {
                availableIndice = mArrayIndices.Length;
                ROW_SIZE *= 2;
                mDidFillOnce = false;
                mLast = availableIndice - 1;
                //mArrayValues = Arrays.copyOf(mArrayValues, ROW_SIZE);
                mArrayValues = mArrayValues.Copy<float>(ROW_SIZE);
                //mArrayIndices = Arrays.copyOf(mArrayIndices, ROW_SIZE);
                mArrayIndices = mArrayIndices.Copy<int>(ROW_SIZE);
                //mArrayNextIndices = Arrays.copyOf(mArrayNextIndices, ROW_SIZE);
                mArrayNextIndices = mArrayNextIndices.Copy<int>(ROW_SIZE);
            }

            // Finally, let's insert the element
            mArrayIndices[availableIndice] = variable.id;
            mArrayValues[availableIndice] = value;
            if (previous != NONE)
            {
                mArrayNextIndices[availableIndice] = mArrayNextIndices[previous];
                mArrayNextIndices[previous] = availableIndice;
            }
            else
            {
                mArrayNextIndices[availableIndice] = mHead;
                mHead = availableIndice;
            }
            variable.usageInRowCount++;
            variable.addToRow(mRow);
            currentSize++;
            if (!mDidFillOnce)
            {
                // only increment mLast if we haven't done the first filling pass
                mLast++;
            }
            if (mLast >= mArrayIndices.Length)
            {
                mDidFillOnce = true;
                mLast = mArrayIndices.Length - 1;
            }
        }

        /// <summary>
        /// Update the current list with a new definition </summary>
        /// <param name="definition"> the row containing the definition </param>
        /// <param name="removeFromDefinition"> </param>
        public virtual float use(ArrayRow definition, bool removeFromDefinition)
        {
            float value = get(definition.variable);
            remove(definition.variable, removeFromDefinition);
            ArrayRow.ArrayRowVariables definitionVariables = definition.variables;
            int definitionSize = definitionVariables.CurrentSize;
            for (int i = 0; i < definitionSize; i++)
            {
                SolverVariable definitionVariable = definitionVariables.getVariable(i);
                float definitionValue = definitionVariables.get(definitionVariable);
                this.add(definitionVariable, definitionValue * value, removeFromDefinition);
            }
            return value;
        }

        /// <summary>
        /// Remove a variable from the list
        /// </summary>
        /// <param name="variable"> the variable we want to remove </param>
        /// <param name="removeFromDefinition"> </param>
        /// <returns> the value of the removed variable </returns>
        public float remove(SolverVariable variable, bool removeFromDefinition)
        {
            if (candidate == variable)
            {
                candidate = null;
            }
            if (mHead == NONE)
            {
                return 0;
            }
            int current = mHead;
            int previous = NONE;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                int idx = mArrayIndices[current];
                if (idx == variable.id)
                {
                    if (current == mHead)
                    {
                        mHead = mArrayNextIndices[current];
                    }
                    else
                    {
                        mArrayNextIndices[previous] = mArrayNextIndices[current];
                    }

                    if (removeFromDefinition)
                    {
                        variable.removeFromRow(mRow);
                    }
                    variable.usageInRowCount--;
                    currentSize--;
                    mArrayIndices[current] = NONE;
                    if (mDidFillOnce)
                    {
                        // If we did a full pass already, remember that spot
                        mLast = current;
                    }
                    return mArrayValues[current];
                }
                previous = current;
                current = mArrayNextIndices[current];
                counter++;
            }
            return 0;
        }

        /// <summary>
        /// Clear the list of variables
        /// </summary>
        public void clear()
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                SolverVariable variable = mCache.mIndexedVariables[mArrayIndices[current]];
                if (variable != null)
                {
                    variable.removeFromRow(mRow);
                }
                current = mArrayNextIndices[current];
                counter++;
            }

            mHead = NONE;
            mLast = NONE;
            mDidFillOnce = false;
            currentSize = 0;
        }

        /// <summary>
        /// Returns true if the variable is contained in the list
        /// </summary>
        /// <param name="variable"> the variable we are looking for </param>
        /// <returns> return true if we found the variable </returns>
        public virtual bool contains(SolverVariable variable)
        {
            if (mHead == NONE)
            {
                return false;
            }
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (mArrayIndices[current] == variable.id)
                {
                    return true;
                }
                current = mArrayNextIndices[current];
                counter++;
            }
            return false;
        }

        public virtual int indexOf(SolverVariable variable)
        {
            if (mHead == NONE)
            {
                return -1;
            }
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (mArrayIndices[current] == variable.id)
                {
                    return current;
                }
                current = mArrayNextIndices[current];
                counter++;
            }
            return -1;
        }



        /// <summary>
        /// Returns true if at least one of the variable is positive
        /// </summary>
        /// <returns> true if at least one of the variable is positive </returns>
        internal virtual bool hasAtLeastOnePositiveVariable()
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (mArrayValues[current] > 0)
                {
                    return true;
                }
                current = mArrayNextIndices[current];
                counter++;
            }
            return false;
        }

        /// <summary>
        /// Invert the values of all the variables in the list
        /// </summary>
        public virtual void invert()
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                mArrayValues[current] *= -1;
                current = mArrayNextIndices[current];
                counter++;
            }
        }

        /// <summary>
        /// Divide the values of all the variables in the list
        /// by the given amount
        /// </summary>
        /// <param name="amount"> amount to divide by </param>
        public virtual void divideByAmount(float amount)
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                mArrayValues[current] /= amount;
                current = mArrayNextIndices[current];
                counter++;
            }
        }

        public virtual int Head
        {
            get
            {
                return mHead;
            }
        }
        public virtual int CurrentSize
        {
            get
            {
                return currentSize;
            }
        }

        public int getId(int index)
        {
            return mArrayIndices[index];
        }

        public float getValue(int index)
        {
            return mArrayValues[index];
        }

        public int getNextIndice(int index)
        {
            return mArrayNextIndices[index];
        }

        /// <summary>
        /// TODO: check if still needed
        /// Return a pivot candidate </summary>
        /// <returns> return a variable we can pivot on </returns>
        internal virtual SolverVariable PivotCandidate
        {
            get
            {
                if (candidate == null)
                {
                    // if no candidate is known, let's figure it out
                    int current = mHead;
                    int counter = 0;
                    SolverVariable pivot = null;
                    while (current != NONE && counter < currentSize)
                    {
                        if (mArrayValues[current] < 0)
                        {
                            // We can return the first negative candidate as in ArrayLinkedVariables
                            // they are already sorted by id

                            SolverVariable v = mCache.mIndexedVariables[mArrayIndices[current]];
                            if (pivot == null || pivot.strength < v.strength)
                            {
                                pivot = v;
                            }
                        }
                        current = mArrayNextIndices[current];
                        counter++;
                    }
                    return pivot;
                }
                return candidate;
            }
        }

        /// <summary>
        /// Return a variable from its position in the linked list
        /// </summary>
        /// <param name="index"> the index of the variable we want to return </param>
        /// <returns> the variable found, or null </returns>
        public virtual SolverVariable getVariable(int index)
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (counter == index)
                {
                    return mCache.mIndexedVariables[mArrayIndices[current]];
                }
                current = mArrayNextIndices[current];
                counter++;
            }
            return null;
        }

        /// <summary>
        /// Return the value of a variable from its position in the linked list
        /// </summary>
        /// <param name="index"> the index of the variable we want to look up </param>
        /// <returns> the value of the found variable, or 0 if not found </returns>
        public virtual float getVariableValue(int index)
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (counter == index)
                {
                    return mArrayValues[current];
                }
                current = mArrayNextIndices[current];
                counter++;
            }
            return 0;
        }

        /// <summary>
        /// Return the value of a variable, 0 if not found </summary>
        /// <param name="v"> the variable we are looking up </param>
        /// <returns> the value of the found variable, or 0 if not found </returns>
        public float get(SolverVariable v)
        {
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                if (mArrayIndices[current] == v.id)
                {
                    return mArrayValues[current];
                }
                current = mArrayNextIndices[current];
                counter++;
            }
            return 0;
        }


        public virtual int sizeInBytes()
        {
            int size = 0;
            size += 3 * (mArrayIndices.Length * 4);
            size += 9 * 4;
            return size;
        }

        public virtual void display()
        {
            int count = currentSize;
            Console.Write("{ ");
            for (int i = 0; i < count; i++)
            {
                SolverVariable v = getVariable(i);
                if (v == null)
                {
                    continue;
                }
                Console.Write(v + " = " + getVariableValue(i) + " ");
            }
            Console.WriteLine(" }");
        }

        /// <summary>
        /// Returns a string representation of the list
        /// </summary>
        /// <returns> a string containing a representation of the list </returns>
        public override string ToString()
        {
            string result = "";
            int current = mHead;
            int counter = 0;
            while (current != NONE && counter < currentSize)
            {
                result += " -> ";
                result += mArrayValues[current] + " : ";
                result += mCache.mIndexedVariables[mArrayIndices[current]];
                current = mArrayNextIndices[current];
                counter++;
            }
            return result;
        }

    }

}