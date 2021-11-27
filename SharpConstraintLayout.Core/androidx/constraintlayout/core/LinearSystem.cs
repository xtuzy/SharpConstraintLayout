using System;
using System.Collections.Generic;

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
    using Chain = androidx.constraintlayout.core.widgets.Chain;
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;


    /// <summary>
    /// Represents and solves a system of linear equations.
    /// </summary>
    public class LinearSystem
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mMaxColumns = TABLE_SIZE;
            mAlreadyTestedCandidates = new bool[TABLE_SIZE];
            mMaxRows = TABLE_SIZE;
        }


        public const bool FULL_DEBUG = false;
        public const bool DEBUG = false;
        public const bool MEASURE = false;

        private const bool DEBUG_CONSTRAINTS = FULL_DEBUG;

        public static bool USE_DEPENDENCY_ORDERING = false;
        public static bool USE_BASIC_SYNONYMS = true;
        public static bool SIMPLIFY_SYNONYMS = true;
        public static bool USE_SYNONYMS = true;
        public static bool SKIP_COLUMNS = true;
        public static bool OPTIMIZED_ENGINE = false;

        /*
		 * Default size for the object pools
		 */
        private static int POOL_SIZE = 1000;
        public bool hasSimpleDefinition = false;

        /*
		 * Variable counter
		 */
        internal int mVariablesID = 0;

        /*
		 * Store a map between name->SolverVariable and SolverVariable->Float for the resolution.
		 */
        private Dictionary<string, SolverVariable> mVariables = null;

        /*
		 * The goal that is used when minimizing the system.
		 */
        private Row mGoal;

        private int TABLE_SIZE = 32; // default table size for the allocation
        private int mMaxColumns;
        internal ArrayRow[] mRows = null;

        // if true, will use graph optimizations
        public bool graphOptimizer = false;
        public bool newgraphOptimizer = false;

        // Used in optimize()
        private bool[] mAlreadyTestedCandidates;

        internal int mNumColumns = 1;
        internal int mNumRows = 0;
        private int mMaxRows;

        internal readonly Cache mCache;

        private SolverVariable[] mPoolVariables = new SolverVariable[POOL_SIZE];
        private int mPoolVariablesCount = 0;

        public static Metrics sMetrics;
        private Row mTempGoal;

        internal class ValuesRow : ArrayRow
        {
            private readonly LinearSystem outerInstance;

            public ValuesRow(LinearSystem outerInstance, Cache cache)
            {
                this.outerInstance = outerInstance;
                variables = new SolverVariableValues(this, cache);
            }
        }

        public LinearSystem()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            mRows = new ArrayRow[TABLE_SIZE];
            releaseRows();
            mCache = new Cache();
            mGoal = new PriorityGoalRow(mCache);
            if (OPTIMIZED_ENGINE)
            {
                mTempGoal = new ValuesRow(this, mCache);
            }
            else
            {
                mTempGoal = new ArrayRow(mCache);
            }
        }

        public virtual void fillMetrics(Metrics metrics)
        {
            sMetrics = metrics;
        }

        public static Metrics Metrics
        {
            get
            {
                return sMetrics;
            }
        }

        public interface Row
        {
            SolverVariable getPivotCandidate(LinearSystem system, bool[] avoid);
            void clear();
            void initFromRow(Row row);
            void addError(SolverVariable variable);
            void updateFromSystem(LinearSystem system);
            SolverVariable Key { get; }
            bool Empty { get; }

            void updateFromRow(LinearSystem system, ArrayRow definition, bool b);
            void updateFromFinalVariable(LinearSystem system, SolverVariable variable, bool removeFromDefinition);
        }

        /*--------------------------------------------------------------------------------------------*/
        // Memory management
        /*--------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Reallocate memory to accommodate increased amount of variables
        /// </summary>
        private void increaseTableSize()
        {
            if (DEBUG)
            {
                Console.WriteLine("###########################");
                Console.WriteLine("### INCREASE TABLE TO " + (TABLE_SIZE * 2) + " (num rows: " + mNumRows + ", num cols: " + mNumColumns + "/" + mMaxColumns + ")");
                Console.WriteLine("###########################");
            }

            TABLE_SIZE *= 2;
            //mRows = Arrays.copyOf(mRows, TABLE_SIZE);
            mRows = mRows.Copy<ArrayRow>(TABLE_SIZE);
            //mCache.mIndexedVariables = Arrays.copyOf(mCache.mIndexedVariables, TABLE_SIZE);
            mCache.mIndexedVariables = mCache.mIndexedVariables.Copy<SolverVariable>(TABLE_SIZE);
            mAlreadyTestedCandidates = new bool[TABLE_SIZE];
            mMaxColumns = TABLE_SIZE;
            mMaxRows = TABLE_SIZE;
            if (sMetrics != null)
            {
                sMetrics.tableSizeIncrease++;
                sMetrics.maxTableSize = Math.Max(sMetrics.maxTableSize, TABLE_SIZE);
                sMetrics.lastTableSize = sMetrics.maxTableSize;
            }
        }

        /// <summary>
        /// Release ArrayRows back to their pool
        /// </summary>
        private void releaseRows()
        {
            if (OPTIMIZED_ENGINE)
            {
                for (int i = 0; i < mNumRows; i++)
                {
                    ArrayRow row = mRows[i];
                    if (row != null)
                    {
                        mCache.optimizedArrayRowPool.release(row);
                    }
                    mRows[i] = null;
                }
            }
            else
            {
                for (int i = 0; i < mNumRows; i++)
                {
                    ArrayRow row = mRows[i];
                    if (row != null)
                    {
                        mCache.arrayRowPool.release(row);
                    }
                    mRows[i] = null;
                }
            }
        }

        /// <summary>
        /// Reset the LinearSystem object so that it can be reused.
        /// </summary>
        public virtual void reset()
        {
            if (DEBUG)
            {
                Console.WriteLine("##################");
                Console.WriteLine("## RESET SYSTEM ##");
                Console.WriteLine("##################");
            }
            for (int i = 0; i < mCache.mIndexedVariables.Length; i++)
            {
                SolverVariable variable = mCache.mIndexedVariables[i];
                if (variable != null)
                {
                    variable.reset();
                }
            }
            mCache.solverVariablePool.releaseAll(mPoolVariables, mPoolVariablesCount);
            mPoolVariablesCount = 0;

            //Arrays.fill(mCache.mIndexedVariables, null);
            mCache.mIndexedVariables.Fill<SolverVariable>(null);
            if (mVariables != null)
            {
                mVariables.Clear();
            }
            mVariablesID = 0;
            mGoal.clear();
            mNumColumns = 1;
            for (int i = 0; i < mNumRows; i++)
            {
                if (mRows[i] != null)
                {
                    mRows[i].used = false;
                }
            }
            releaseRows();
            mNumRows = 0;
            if (OPTIMIZED_ENGINE)
            {
                mTempGoal = new ValuesRow(this, mCache);
            }
            else
            {
                mTempGoal = new ArrayRow(mCache);
            }
        }

        /*--------------------------------------------------------------------------------------------*/
        // Creation of rows / variables / errors
        /*--------------------------------------------------------------------------------------------*/

        public virtual SolverVariable createObjectVariable(object anchor)
        {
            if (anchor == null)
            {
                return null;
            }
            if (mNumColumns + 1 >= mMaxColumns)
            {
                increaseTableSize();
            }
            SolverVariable variable = null;
            if (anchor is ConstraintAnchor)
            {
                variable = ((ConstraintAnchor)anchor).SolverVariable;
                if (variable == null)
                {
                    ((ConstraintAnchor)anchor).resetSolverVariable(mCache);
                    variable = ((ConstraintAnchor)anchor).SolverVariable;
                }
                if (variable.id == -1 || variable.id > mVariablesID || mCache.mIndexedVariables[variable.id] == null)
                {
                    if (variable.id != -1)
                    {
                        variable.reset();
                    }
                    mVariablesID++;
                    mNumColumns++;
                    variable.id = mVariablesID;
                    variable.mType = SolverVariable.Type.UNRESTRICTED;
                    mCache.mIndexedVariables[mVariablesID] = variable;
                }
            }
            return variable;
        }

        public static long ARRAY_ROW_CREATION = 0;
        public static long OPTIMIZED_ARRAY_ROW_CREATION = 0;

        public virtual ArrayRow createRow()
        {
            ArrayRow row;
            if (OPTIMIZED_ENGINE)
            {
                row = mCache.optimizedArrayRowPool.acquire();
                if (row == null)
                {
                    row = new ValuesRow(this, mCache);
                    OPTIMIZED_ARRAY_ROW_CREATION++;
                }
                else
                {
                    row.reset();
                }
            }
            else
            {
                row = mCache.arrayRowPool.acquire();
                if (row == null)
                {
                    row = new ArrayRow(mCache);
                    ARRAY_ROW_CREATION++;
                }
                else
                {
                    row.reset();
                }
            }
            SolverVariable.increaseErrorId();
            return row;
        }

        public virtual SolverVariable createSlackVariable()
        {
            if (sMetrics != null)
            {
                sMetrics.slackvariables++;
            }
            if (mNumColumns + 1 >= mMaxColumns)
            {
                increaseTableSize();
            }
            SolverVariable variable = acquireSolverVariable(SolverVariable.Type.SLACK, null);
            mVariablesID++;
            mNumColumns++;
            variable.id = mVariablesID;
            mCache.mIndexedVariables[mVariablesID] = variable;
            return variable;
        }

        public virtual SolverVariable createExtraVariable()
        {
            if (sMetrics != null)
            {
                sMetrics.extravariables++;
            }
            if (mNumColumns + 1 >= mMaxColumns)
            {
                increaseTableSize();
            }
            SolverVariable variable = acquireSolverVariable(SolverVariable.Type.SLACK, null);
            mVariablesID++;
            mNumColumns++;
            variable.id = mVariablesID;
            mCache.mIndexedVariables[mVariablesID] = variable;
            return variable;
        }

        private void addError(ArrayRow row)
        {
            row.addError(this, SolverVariable.STRENGTH_NONE);
        }

        private void addSingleError(ArrayRow row, int sign)
        {
            addSingleError(row, sign, SolverVariable.STRENGTH_NONE);
        }

        internal virtual void addSingleError(ArrayRow row, int sign, int strength)
        {
            string prefix = null;
            if (DEBUG)
            {
                if (sign > 0)
                {
                    prefix = "ep";
                }
                else
                {
                    prefix = "em";
                }
                prefix = "em";
            }
            SolverVariable error = createErrorVariable(strength, prefix);
            row.addSingleError(error, sign);
        }

        private SolverVariable createVariable(string name, SolverVariable.Type type)
        {
            if (sMetrics != null)
            {
                sMetrics.variables++;
            }
            if (mNumColumns + 1 >= mMaxColumns)
            {
                increaseTableSize();
            }
            SolverVariable variable = acquireSolverVariable(type, null);
            variable.Name = name;
            mVariablesID++;
            mNumColumns++;
            variable.id = mVariablesID;
            if (mVariables == null)
            {
                mVariables = new Dictionary<string, SolverVariable>();
            }
            mVariables[name] = variable;
            mCache.mIndexedVariables[mVariablesID] = variable;
            return variable;
        }

        public virtual SolverVariable createErrorVariable(int strength, string prefix)
        {
            if (sMetrics != null)
            {
                sMetrics.errors++;
            }
            if (mNumColumns + 1 >= mMaxColumns)
            {
                increaseTableSize();
            }
            SolverVariable variable = acquireSolverVariable(SolverVariable.Type.ERROR, prefix);
            mVariablesID++;
            mNumColumns++;
            variable.id = mVariablesID;
            variable.strength = strength;
            mCache.mIndexedVariables[mVariablesID] = variable;
            mGoal.addError(variable);
            return variable;
        }

        /// <summary>
        /// Returns a SolverVariable instance of the given type </summary>
        /// <param name="type"> type of the SolverVariable </param>
        /// <returns> instance of SolverVariable </returns>
        private SolverVariable acquireSolverVariable(SolverVariable.Type type, string prefix)
        {
            SolverVariable variable = mCache.solverVariablePool.acquire();
            if (variable == null)
            {
                variable = new SolverVariable(type, prefix);
                variable.setType(type, prefix);
            }
            else
            {
                variable.reset();
                variable.setType(type, prefix);
            }
            if (mPoolVariablesCount >= POOL_SIZE)
            {
                POOL_SIZE *= 2;
                //mPoolVariables = Arrays.copyOf(mPoolVariables, POOL_SIZE);
                mPoolVariables = mPoolVariables.Copy<SolverVariable>(POOL_SIZE);
            }
            mPoolVariables[mPoolVariablesCount++] = variable;
            return variable;
        }

        /*--------------------------------------------------------------------------------------------*/
        // Accessors of rows / variables / errors
        /*--------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Simple accessor for the current goal. Used when minimizing the system's goal. </summary>
        /// <returns> the current goal. </returns>
        internal virtual Row Goal
        {
            get
            {
                return mGoal;
            }
        }

        internal virtual ArrayRow getRow(int n)
        {
            return mRows[n];
        }

        internal virtual float getValueFor(string name)
        {
            SolverVariable v = getVariable(name, SolverVariable.Type.UNRESTRICTED);
            if (v == null)
            {
                return 0;
            }
            return v.computedValue;
        }

        public virtual int getObjectVariableValue(object @object)
        {
            ConstraintAnchor anchor = (ConstraintAnchor)@object;
            if (Chain.USE_CHAIN_OPTIMIZATION)
            {
                if (anchor.hasFinalValue())
                {
                    return anchor.FinalValue;
                }
            }
            SolverVariable variable = anchor.SolverVariable;
            if (variable != null)
            {
                return (int)(variable.computedValue + 0.5f);
            }
            return 0;
        }

        /// <summary>
        /// Returns a SolverVariable instance given a name and a type.
        /// </summary>
        /// <param name="name"> name of the variable </param>
        /// <param name="type"> <seealso cref="SolverVariable.Type type"/> of the variable </param>
        /// <returns> a SolverVariable instance </returns>
        internal virtual SolverVariable getVariable(string name, SolverVariable.Type type)
        {
            if (mVariables == null)
            {
                mVariables = new Dictionary<string, SolverVariable>();
            }

            SolverVariable variable = mVariables.ContainsKey(name)?mVariables[name]:null;

            if (variable == null)
            {
                variable = createVariable(name, type);
            }
            return variable;
        }

        /*--------------------------------------------------------------------------------------------*/
        // System resolution
        /*--------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Minimize the current goal of the system.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void minimize() throws Exception
        public virtual void minimize()
        {
            if (sMetrics != null)
            {
                sMetrics.minimize++;
            }
            if (mGoal.Empty)
            {
                if (DEBUG)
                {
                    Console.WriteLine("\n*** SKIPPING MINIMIZE! ***\n");
                }
                computeValues();
                return;
            }
            if (DEBUG)
            {
                Console.WriteLine("\n*** MINIMIZE ***\n");
            }
            if (graphOptimizer || newgraphOptimizer)
            {
                if (sMetrics != null)
                {
                    sMetrics.graphOptimizer++;
                }
                bool fullySolved = true;
                for (int i = 0; i < mNumRows; i++)
                {
                    ArrayRow r = mRows[i];
                    if (!r.isSimpleDefinition)
                    {
                        fullySolved = false;
                        break;
                    }
                }
                if (!fullySolved)
                {
                    minimizeGoal(mGoal);
                }
                else
                {
                    if (sMetrics != null)
                    {
                        sMetrics.fullySolved++;
                    }
                    computeValues();
                }
            }
            else
            {
                minimizeGoal(mGoal);
            }
            if (DEBUG)
            {
                Console.WriteLine("\n*** END MINIMIZE ***\n");
            }
        }

        /// <summary>
        /// Minimize the given goal with the current system. </summary>
        /// <param name="goal"> the goal to minimize. </param>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void minimizeGoal(Row goal) throws Exception
        internal virtual void minimizeGoal(Row goal)
        {
            if (sMetrics != null)
            {
                sMetrics.minimizeGoal++;
                sMetrics.maxVariables = Math.Max(sMetrics.maxVariables, mNumColumns);
                sMetrics.maxRows = Math.Max(sMetrics.maxRows, mNumRows);
            }
            // First, let's make sure that the system is in Basic Feasible Solved Form (BFS), i.e.
            // all the constants of the restricted variables should be positive.
            if (DEBUG)
            {
                Console.WriteLine("minimize goal: " + goal);
            }
            // we don't need this for now as we incrementally built the system
            // goal.updateFromSystem(this);
            if (DEBUG)
            {
                displayReadableRows();
            }
            enforceBFS(goal);
            if (DEBUG)
            {
                Console.WriteLine("Goal after enforcing BFS " + goal);
                displayReadableRows();
            }
            optimize(goal, false);
            if (DEBUG)
            {
                Console.WriteLine("Goal after optimization " + goal);
                displayReadableRows();
            }
            computeValues();
        }

        internal void cleanupRows()
        {
            int i = 0;
            while (i < mNumRows)
            {
                ArrayRow current = mRows[i];
                if (current.variables.CurrentSize == 0)
                {
                    current.isSimpleDefinition = true;
                }
                if (current.isSimpleDefinition)
                {
                    current.variable.computedValue = current.constantValue;
                    current.variable.removeFromRow(current);
                    for (int j = i; j < mNumRows - 1; j++)
                    {
                        mRows[j] = mRows[j + 1];
                    }
                    mRows[mNumRows - 1] = null;
                    mNumRows--;
                    i--;
                    if (OPTIMIZED_ENGINE)
                    {
                        mCache.optimizedArrayRowPool.release(current);
                    }
                    else
                    {
                        mCache.arrayRowPool.release(current);
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Add the equation to the system </summary>
        /// <param name="row"> the equation we want to add expressed as a system row. </param>
        public virtual void addConstraint(ArrayRow row)
        {
            if (row == null)
            {
                return;
            }
            if (sMetrics != null)
            {
                sMetrics.constraints++;
                if (row.isSimpleDefinition)
                {
                    sMetrics.simpleconstraints++;
                }
            }
            if (mNumRows + 1 >= mMaxRows || mNumColumns + 1 >= mMaxColumns)
            {
                increaseTableSize();
            }
            if (DEBUG)
            {
                Console.WriteLine("addConstraint <" + row.toReadableString() + ">");
                displayReadableRows();
            }

            bool added = false;
            if (!row.isSimpleDefinition)
            {
                // Update the equation with the variables already defined in the system
                row.updateFromSystem(this);

                if (row.Empty)
                {
                    return;
                }

                // First, ensure that if we have a constant it's positive
                row.ensurePositiveConstant();

                if (DEBUG)
                {
                    Console.WriteLine("addConstraint, updated row : " + row.toReadableString());
                }

                // Then pick a good variable to use for the row
                if (row.chooseSubject(this))
                {
                    // extra variable added... let's try to see if we can remove it
                    SolverVariable extra = createExtraVariable();
                    row.variable = extra;
                    int numRows = mNumRows;
                    addRow(row);
                    if (mNumRows == numRows + 1)
                    {
                        added = true;
                        mTempGoal.initFromRow(row);
                        optimize(mTempGoal, true);
                        if (extra.definitionId == -1)
                        {
                            if (DEBUG)
                            {
                                Console.WriteLine("row added is 0, so get rid of it");
                            }
                            if (row.variable == extra)
                            {
                                // move extra to be parametric
                                SolverVariable pivotCandidate = row.pickPivot(extra);
                                if (pivotCandidate != null)
                                {
                                    if (sMetrics != null)
                                    {
                                        sMetrics.pivots++;
                                    }
                                    row.pivot(pivotCandidate);
                                }
                            }
                            if (!row.isSimpleDefinition)
                            {
                                row.variable.updateReferencesWithNewDefinition(this, row);
                            }
                            if (OPTIMIZED_ENGINE)
                            {
                                mCache.optimizedArrayRowPool.release(row);
                            }
                            else
                            {
                                mCache.arrayRowPool.release(row);
                            }
                            mNumRows--;
                        }
                    }
                }

                if (!row.hasKeyVariable())
                {
                    // Can happen if row resolves to nil
                    if (DEBUG)
                    {
                        Console.WriteLine("No variable found to pivot on " + row.toReadableString());
                        displayReadableRows();
                    }
                    return;
                }
            }
            if (!added)
            {
                addRow(row);
            }
        }

        private void addRow(ArrayRow row)
        {
            if (SIMPLIFY_SYNONYMS && row.isSimpleDefinition)
            {
                row.variable.setFinalValue(this, row.constantValue);
            }
            else
            {
                mRows[mNumRows] = row;
                row.variable.definitionId = mNumRows;
                mNumRows++;
                row.variable.updateReferencesWithNewDefinition(this, row);
            }
            if (DEBUG)
            {
                Console.WriteLine("Row added: " + row);
                Console.WriteLine("here is the system:");
                displayReadableRows();
            }
            if (SIMPLIFY_SYNONYMS && hasSimpleDefinition)
            {
                // compact the rows...
                for (int i = 0; i < mNumRows; i++)
                {
                    if (mRows[i] == null)
                    {
                        Console.WriteLine("WTF");
                    }
                    if (mRows[i] != null && mRows[i].isSimpleDefinition)
                    {
                        ArrayRow removedRow = mRows[i];
                        removedRow.variable.setFinalValue(this, removedRow.constantValue);
                        if (OPTIMIZED_ENGINE)
                        {
                            mCache.optimizedArrayRowPool.release(removedRow);
                        }
                        else
                        {
                            mCache.arrayRowPool.release(removedRow);
                        }
                        mRows[i] = null;
                        int lastRow = i + 1;
                        for (int j = i + 1; j < mNumRows; j++)
                        {
                            mRows[j - 1] = mRows[j];
                            if (mRows[j - 1].variable.definitionId == j)
                            {
                                mRows[j - 1].variable.definitionId = j - 1;
                            }
                            lastRow = j;
                        }
                        if (lastRow < mNumRows)
                        {
                            mRows[lastRow] = null;
                        }
                        mNumRows--;
                        i--;
                    }
                }
                hasSimpleDefinition = false;
            }
        }

        public virtual void removeRow(ArrayRow row)
        {
            if (row.isSimpleDefinition && row.variable != null)
            {
                if (row.variable.definitionId != -1)
                {
                    for (int i = row.variable.definitionId; i < mNumRows - 1; i++)
                    {
                        SolverVariable rowVariable = mRows[i + 1].variable;
                        if (rowVariable.definitionId == i + 1)
                        {
                            rowVariable.definitionId = i;
                        }
                        mRows[i] = mRows[i + 1];
                    }
                    mNumRows--;
                }
                if (!row.variable.isFinalValue)
                {
                    row.variable.setFinalValue(this, row.constantValue);
                }
                if (OPTIMIZED_ENGINE)
                {
                    mCache.optimizedArrayRowPool.release(row);
                }
                else
                {
                    mCache.arrayRowPool.release(row);
                }
            }
        }

        /// <summary>
        /// Optimize the system given a goal to minimize. The system should be in BFS form. </summary>
        /// <param name="goal"> goal to optimize. </param>
        /// <param name="b"> </param>
        /// <returns> number of iterations. </returns>
        private int optimize(Row goal, bool b)
        {
            if (sMetrics != null)
            {
                sMetrics.optimize++;
            }
            bool done = false;
            int tries = 0;
            for (int i = 0; i < mNumColumns; i++)
            {
                mAlreadyTestedCandidates[i] = false;
            }

            if (DEBUG)
            {
                Console.WriteLine("\n****************************");
                Console.WriteLine("*       OPTIMIZATION       *");
                Console.WriteLine("* mNumColumns: " + mNumColumns);
                Console.WriteLine("* GOAL: " + goal);
                Console.WriteLine("****************************\n");
            }

            while (!done)
            {
                if (sMetrics != null)
                {
                    sMetrics.iterations++;
                }
                tries++;
                if (DEBUG)
                {
                    Console.WriteLine("\n******************************");
                    Console.WriteLine("* iteration: " + tries);
                }
                if (tries >= 2 * mNumColumns)
                {
                    if (DEBUG)
                    {
                        Console.WriteLine("=> Exit optimization because tries " + tries + " >= " + (2 * mNumColumns));
                    }
                    return tries;
                }

                if (goal.Key != null)
                {
                    mAlreadyTestedCandidates[goal.Key.id] = true;
                }
                SolverVariable pivotCandidate = goal.getPivotCandidate(this, mAlreadyTestedCandidates);
                if (DEBUG)
                {
                    Console.WriteLine("* Pivot candidate: " + pivotCandidate);
                    Console.WriteLine("******************************\n");
                }
                if (pivotCandidate != null)
                {
                    if (mAlreadyTestedCandidates[pivotCandidate.id])
                    {
                        if (DEBUG)
                        {
                            Console.WriteLine("* Pivot candidate " + pivotCandidate + " already tested, let's bail");
                        }
                        return tries;
                    }
                    else
                    {
                        mAlreadyTestedCandidates[pivotCandidate.id] = true;
                    }
                }

                if (pivotCandidate != null)
                {
                    if (DEBUG)
                    {
                        Console.WriteLine("valid pivot candidate: " + pivotCandidate);
                    }
                    // there's a negative variable in the goal that we can pivot on.
                    // We now need to select which equation of the system we should do
                    // the pivot on.

                    // Let's try to find the equation in the system that we can pivot on.
                    // The rules are simple:
                    // - only look at restricted variables equations (i.e. Cs)
                    // - only look at equations containing the column we are trying to pivot on (duh)
                    // - select preferably an equation with strong strength over weak strength

                    float min = float.MaxValue;
                    int pivotRowIndex = -1;

                    for (int i = 0; i < mNumRows; i++)
                    {
                        ArrayRow current = mRows[i];
                        SolverVariable variable = current.variable;
                        if (variable.mType == SolverVariable.Type.UNRESTRICTED)
                        {
                            // skip unrestricted variables equations (to only look at Cs)
                            continue;
                        }
                        if (current.isSimpleDefinition)
                        {
                            continue;
                        }

                        if (current.hasVariable(pivotCandidate))
                        {
                            if (DEBUG)
                            {
                                Console.WriteLine("equation " + i + " " + current + " contains " + pivotCandidate);
                            }
                            // the current row does contains the variable
                            // we want to pivot on
                            float a_j = current.variables.get(pivotCandidate);
                            if (a_j < 0)
                            {
                                float value = -current.constantValue / a_j;
                                if (value < min)
                                {
                                    min = value;
                                    pivotRowIndex = i;
                                }
                            }
                        }
                    }
                    // At this point, we ought to have an equation to pivot on

                    if (pivotRowIndex > -1)
                    {
                        // We found an equation to pivot on
                        if (DEBUG)
                        {
                            Console.WriteLine("We pivot on " + pivotRowIndex);
                        }
                        ArrayRow pivotEquation = mRows[pivotRowIndex];
                        pivotEquation.variable.definitionId = -1;
                        if (sMetrics != null)
                        {
                            sMetrics.pivots++;
                        }
                        pivotEquation.pivot(pivotCandidate);
                        pivotEquation.variable.definitionId = pivotRowIndex;
                        pivotEquation.variable.updateReferencesWithNewDefinition(this, pivotEquation);
                        if (DEBUG)
                        {
                            Console.WriteLine("new system after pivot:");
                            displayReadableRows();
                            Console.WriteLine("optimizing: " + goal);
                        }
                        /*
						try {
						    enforceBFS(goal);
						} catch (Exception e) {
						    System.out.println("### EXCEPTION " + e);
						    e.printStackTrace();
						}
						*/
                        // now that we pivoted, we're going to continue looping on the next goal
                        // columns, until we exhaust all the possibilities of improving the system
                    }
                    else
                    {
                        if (DEBUG)
                        {
                            Console.WriteLine("we couldn't find an equation to pivot upon");
                        }
                    }

                }
                else
                {
                    // There is no candidate goals columns we should try to pivot on,
                    // so let's exit the loop.
                    if (DEBUG)
                    {
                        Console.WriteLine("no more candidate goals to pivot on, let's exit");
                    }
                    done = true;
                }
            }
            return tries;
        }

        /// <summary>
        /// Make sure that the system is in Basic Feasible Solved form (BFS). </summary>
        /// <param name="goal"> the row representing the system goal </param>
        /// <returns> number of iterations </returns>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private int enforceBFS(Row goal) throws Exception
        private int enforceBFS(Row goal)
        {
            int tries = 0;
            bool done;

            if (DEBUG)
            {
                Console.WriteLine("\n#################");
                Console.WriteLine("# ENFORCING BFS #");
                Console.WriteLine("#################\n");
            }

            // At this point, we might not be in Basic Feasible Solved form (BFS),
            // i.e. one of the restricted equation has a negative constant.
            // Let's check if that's the case or not.
            bool infeasibleSystem = false;
            for (int i = 0; i < mNumRows; i++)
            {
                SolverVariable variable = mRows[i].variable;
                if (variable.mType == SolverVariable.Type.UNRESTRICTED)
                {
                    continue; // C can be either positive or negative.
                }
                if (mRows[i].constantValue < 0)
                {
                    infeasibleSystem = true;
                    break;
                }
            }

            // The system happens to not be in BFS form, we need to go back to it to properly solve it.
            if (infeasibleSystem)
            {
                if (DEBUG)
                {
                    Console.WriteLine("the current system is infeasible, let's try to fix this.");
                }

                // Going back to BFS form can be done by selecting any equations in Cs containing
                // a negative constant, then selecting a potential pivot variable that would remove
                // this negative constant. Once we have
                done = false;
                tries = 0;
                while (!done)
                {
                    if (sMetrics != null)
                    {
                        sMetrics.bfs++;
                    }
                    tries++;
                    if (DEBUG)
                    {
                        Console.WriteLine("iteration on infeasible system " + tries);
                    }
                    float min = float.MaxValue;
                    int strength = 0;
                    int pivotRowIndex = -1;
                    int pivotColumnIndex = -1;

                    for (int i = 0; i < mNumRows; i++)
                    {
                        ArrayRow current = mRows[i];
                        SolverVariable variable = current.variable;
                        if (variable.mType == SolverVariable.Type.UNRESTRICTED)
                        {
                            // skip unrestricted variables equations, as C
                            // can be either positive or negative.
                            continue;
                        }
                        if (current.isSimpleDefinition)
                        {
                            continue;
                        }
                        if (current.constantValue < 0)
                        {
                            // let's examine this row, see if we can find a good pivot
                            if (DEBUG)
                            {
                                Console.WriteLine("looking at pivoting on row " + current);
                            }
                            if (SKIP_COLUMNS)
                            {
                                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                                //ORIGINAL LINE: final int size = current.variables.getCurrentSize();
                                int size = current.variables.CurrentSize;
                                for (int j = 0; j < size; j++)
                                {
                                    SolverVariable candidate = current.variables.getVariable(j);
                                    float a_j = current.variables.get(candidate);
                                    if (a_j <= 0)
                                    {
                                        continue;
                                    }
                                    if (DEBUG)
                                    {
                                        Console.WriteLine("candidate for pivot " + candidate);
                                    }
                                    for (int k = 0; k < SolverVariable.MAX_STRENGTH; k++)
                                    {
                                        float value = candidate.strengthVector[k] / a_j;
                                        if ((value < min && k == strength) || k > strength)
                                        {
                                            min = value;
                                            pivotRowIndex = i;
                                            pivotColumnIndex = candidate.id;
                                            strength = k;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 1; j < mNumColumns; j++)
                                {
                                    SolverVariable candidate = mCache.mIndexedVariables[j];
                                    float a_j = current.variables.get(candidate);
                                    if (a_j <= 0)
                                    {
                                        continue;
                                    }
                                    if (DEBUG)
                                    {
                                        Console.WriteLine("candidate for pivot " + candidate);
                                    }
                                    for (int k = 0; k < SolverVariable.MAX_STRENGTH; k++)
                                    {
                                        float value = candidate.strengthVector[k] / a_j;
                                        if ((value < min && k == strength) || k > strength)
                                        {
                                            min = value;
                                            pivotRowIndex = i;
                                            pivotColumnIndex = j;
                                            strength = k;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (pivotRowIndex != -1)
                    {
                        // We have a pivot!
                        ArrayRow pivotEquation = mRows[pivotRowIndex];
                        if (DEBUG)
                        {
                            Console.WriteLine("Pivoting on " + pivotEquation.variable + " with " + mCache.mIndexedVariables[pivotColumnIndex]);
                        }
                        pivotEquation.variable.definitionId = -1;
                        if (sMetrics != null)
                        {
                            sMetrics.pivots++;
                        }
                        pivotEquation.pivot(mCache.mIndexedVariables[pivotColumnIndex]);
                        pivotEquation.variable.definitionId = pivotRowIndex;
                        pivotEquation.variable.updateReferencesWithNewDefinition(this, pivotEquation);

                        if (DEBUG)
                        {
                            Console.WriteLine("new goal after pivot: " + goal);
                            displayRows();
                        }
                    }
                    else
                    {
                        done = true;
                    }
                    if (tries > mNumColumns / 2)
                    {
                        // fail safe -- tried too many times
                        done = true;
                    }
                }
            }

            if (DEBUG)
            {
                Console.WriteLine("the current system should now be feasible [" + infeasibleSystem + "] after " + tries + " iterations");
                displayReadableRows();

                // Let's make sure the system is correct
                //noinspection UnusedAssignment
                infeasibleSystem = false;
                for (int i = 0; i < mNumRows; i++)
                {
                    SolverVariable variable = mRows[i].variable;
                    if (variable.mType == SolverVariable.Type.UNRESTRICTED)
                    {
                        continue; // C can be either positive or negative.
                    }
                    if (mRows[i].constantValue < 0)
                    {
                        //noinspection UnusedAssignment
                        infeasibleSystem = true;
                        break;
                    }
                }

                if (DEBUG && infeasibleSystem)
                {
                    Console.WriteLine("IMPOSSIBLE SYSTEM, WTF");
                    throw new Exception();
                }
                if (infeasibleSystem)
                {
                    return tries;
                }
            }

            return tries;
        }

        private void computeValues()
        {
            for (int i = 0; i < mNumRows; i++)
            {
                ArrayRow row = mRows[i];
                row.variable.computedValue = row.constantValue;
            }
        }

        /*--------------------------------------------------------------------------------------------*/
        // Display utility functions
        /*--------------------------------------------------------------------------------------------*/

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private void displayRows()
        private void displayRows()
        {
            displaySolverVariables();
            string s = "";
            for (int i = 0; i < mNumRows; i++)
            {
                s += mRows[i];
                s += "\n";
            }
            s += mGoal + "\n";
            Console.WriteLine(s);
        }

        public virtual void displayReadableRows()
        {
            displaySolverVariables();
            string s = " num vars " + mVariablesID + "\n";
            for (int i = 0; i < mVariablesID + 1; i++)
            {
                SolverVariable variable = mCache.mIndexedVariables[i];
                if (variable != null && variable.isFinalValue)
                {
                    s += " $[" + i + "] => " + variable + " = " + variable.computedValue + "\n";
                }
            }
            s += "\n";
            for (int i = 0; i < mVariablesID + 1; i++)
            {
                SolverVariable variable = mCache.mIndexedVariables[i];
                if (variable != null && variable.isSynonym)
                {
                    SolverVariable synonym = mCache.mIndexedVariables[variable.synonym];
                    s += " ~[" + i + "] => " + variable + " = " + synonym + " + " + variable.synonymDelta + "\n";
                }
            }
            s += "\n\n #  ";
            for (int i = 0; i < mNumRows; i++)
            {
                s += mRows[i].toReadableString();
                s += "\n #  ";
            }
            if (mGoal != null)
            {
                s += "Goal: " + mGoal + "\n";
            }
            Console.WriteLine(s);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") public void displayVariablesReadableRows()
        public virtual void displayVariablesReadableRows()
        {
            displaySolverVariables();
            string s = "";
            for (int i = 0; i < mNumRows; i++)
            {
                if (mRows[i].variable.mType == SolverVariable.Type.UNRESTRICTED)
                {
                    s += mRows[i].toReadableString();
                    s += "\n";
                }
            }
            s += mGoal + "\n";
            Console.WriteLine(s);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") public int getMemoryUsed()
        public virtual int MemoryUsed
        {
            get
            {
                int actualRowSize = 0;
                for (int i = 0; i < mNumRows; i++)
                {
                    if (mRows[i] != null)
                    {
                        actualRowSize += mRows[i].sizeInBytes();
                    }
                }
                return actualRowSize;
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") public int getNumEquations()
        public virtual int NumEquations
        {
            get
            {
                return mNumRows;
            }
        }
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") public int getNumVariables()
        public virtual int NumVariables
        {
            get
            {
                return mVariablesID;
            }
        }

        /// <summary>
        /// Display current system information
        /// </summary>
        internal virtual void displaySystemInformation()
        {
            int count = 0;
            int rowSize = 0;
            for (int i = 0; i < TABLE_SIZE; i++)
            {
                if (mRows[i] != null)
                {
                    rowSize += mRows[i].sizeInBytes();
                }
            }
            int actualRowSize = 0;
            for (int i = 0; i < mNumRows; i++)
            {
                if (mRows[i] != null)
                {
                    actualRowSize += mRows[i].sizeInBytes();
                }
            }

            Console.WriteLine("Linear System -> Table size: " + TABLE_SIZE + " (" + getDisplaySize(TABLE_SIZE * TABLE_SIZE) + ") -- row sizes: " + getDisplaySize(rowSize) + ", actual size: " + getDisplaySize(actualRowSize) + " rows: " + mNumRows + "/" + mMaxRows + " cols: " + mNumColumns + "/" + mMaxColumns + " " + count + " occupied cells, " + getDisplaySize(count));
        }

        private void displaySolverVariables()
        {
            string s = "Display Rows (" + mNumRows + "x" + mNumColumns + ")\n";
            /*
			s += ":\n\t | C | ";
			for (int i = 1; i <= mNumColumns; i++) {
			    SolverVariable v = mCache.mIndexedVariables[i];
			    s += v;
			    s += " | ";
			}
			s += "\n";
			*/
            Console.WriteLine(s);
        }

        private string getDisplaySize(int n)
        {
            int mb = (n * 4) / 1024 / 1024;
            if (mb > 0)
            {
                return "" + mb + " Mb";
            }
            int kb = (n * 4) / 1024;
            if (kb > 0)
            {
                return "" + kb + " Kb";
            }
            return "" + (n * 4) + " bytes";
        }

        public virtual Cache Cache
        {
            get
            {
                return mCache;
            }
        }

        private string getDisplayStrength(int strength)
        {
            if (strength == SolverVariable.STRENGTH_LOW)
            {
                return "LOW";
            }
            if (strength == SolverVariable.STRENGTH_MEDIUM)
            {
                return "MEDIUM";
            }
            if (strength == SolverVariable.STRENGTH_HIGH)
            {
                return "HIGH";
            }
            if (strength == SolverVariable.STRENGTH_HIGHEST)
            {
                return "HIGHEST";
            }
            if (strength == SolverVariable.STRENGTH_EQUALITY)
            {
                return "EQUALITY";
            }
            if (strength == SolverVariable.STRENGTH_FIXED)
            {
                return "FIXED";
            }
            if (strength == SolverVariable.STRENGTH_BARRIER)
            {
                return "BARRIER";
            }
            return "NONE";
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        // Equations
        ////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Add an equation of the form a >= b + margin </summary>
        /// <param name="a"> variable a </param>
        /// <param name="b"> variable b </param>
        /// <param name="margin"> margin </param>
        /// <param name="strength"> strength used </param>
        public virtual void addGreaterThan(SolverVariable a, SolverVariable b, int margin, int strength)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> " + a + " >= " + b + (margin != 0 ? " + " + margin : "") + " " + getDisplayStrength(strength));
            }
            ArrayRow row = createRow();
            SolverVariable slack = createSlackVariable();
            slack.strength = 0;
            row.createRowGreaterThan(a, b, slack, margin);
            if (strength != SolverVariable.STRENGTH_FIXED)
            {
                float slackValue = row.variables.get(slack);
                addSingleError(row, (int)(-1 * slackValue), strength);
            }
            addConstraint(row);
        }

        public virtual void addGreaterBarrier(SolverVariable a, SolverVariable b, int margin, bool hasMatchConstraintWidgets)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> Barrier " + a + " >= " + b);
            }
            ArrayRow row = createRow();
            SolverVariable slack = createSlackVariable();
            slack.strength = 0;
            row.createRowGreaterThan(a, b, slack, margin);
            addConstraint(row);
        }

        /// <summary>
        /// Add an equation of the form a <= b + margin </summary>
        /// <param name="a"> variable a </param>
        /// <param name="b"> variable b </param>
        /// <param name="margin"> margin </param>
        /// <param name="strength"> strength used </param>
        public virtual void addLowerThan(SolverVariable a, SolverVariable b, int margin, int strength)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> " + a + " <= " + b + (margin != 0 ? " + " + margin : "") + " " + getDisplayStrength(strength));
            }
            ArrayRow row = createRow();
            SolverVariable slack = createSlackVariable();
            slack.strength = 0;
            row.createRowLowerThan(a, b, slack, margin);
            if (strength != SolverVariable.STRENGTH_FIXED)
            {
                float slackValue = row.variables.get(slack);
                addSingleError(row, (int)(-1 * slackValue), strength);
            }
            addConstraint(row);
        }

        public virtual void addLowerBarrier(SolverVariable a, SolverVariable b, int margin, bool hasMatchConstraintWidgets)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> Barrier " + a + " <= " + b);
            }
            ArrayRow row = createRow();
            SolverVariable slack = createSlackVariable();
            slack.strength = 0;
            row.createRowLowerThan(a, b, slack, margin);
            addConstraint(row);
        }

        /// <summary>
        /// Add an equation of the form (1 - bias) * (a - b) = bias * (c - d) </summary>
        /// <param name="a"> variable a </param>
        /// <param name="b"> variable b </param>
        /// <param name="m1"> margin 1 </param>
        /// <param name="bias"> bias between ab - cd </param>
        /// <param name="c"> variable c </param>
        /// <param name="d"> variable d </param>
        /// <param name="m2"> margin 2 </param>
        /// <param name="strength"> strength used </param>
        public virtual void addCentering(SolverVariable a, SolverVariable b, int m1, float bias, SolverVariable c, SolverVariable d, int m2, int strength)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> [center bias: " + bias + "] : " + a + " - " + b + " - " + m1 + " = " + c + " - " + d + " - " + m2 + " " + getDisplayStrength(strength));
            }
            ArrayRow row = createRow();
            row.createRowCentering(a, b, m1, bias, c, d, m2);
            if (strength != SolverVariable.STRENGTH_FIXED)
            {
                row.addError(this, strength);
            }
            addConstraint(row);
        }

        public virtual void addRatio(SolverVariable a, SolverVariable b, SolverVariable c, SolverVariable d, float ratio, int strength)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> [ratio: " + ratio + "] : " + a + " = " + b + " + (" + c + " - " + d + ") * " + ratio + " " + getDisplayStrength(strength));
            }
            ArrayRow row = createRow();
            row.createRowDimensionRatio(a, b, c, d, ratio);
            if (strength != SolverVariable.STRENGTH_FIXED)
            {
                row.addError(this, strength);
            }
            addConstraint(row);
        }

        public virtual void addSynonym(SolverVariable a, SolverVariable b, int margin)
        {
            if (a.definitionId == -1 && margin == 0)
            {
                if (DEBUG_CONSTRAINTS)
                {
                    Console.WriteLine("(S) -> " + a + " = " + b + (margin != 0 ? " + " + margin : ""));
                }
                if (b.isSynonym)
                {
                    margin += (int)b.synonymDelta;
                    b = mCache.mIndexedVariables[b.synonym];
                }
                if (a.isSynonym)
                {
                    margin -= (int)a.synonymDelta;
                    a = mCache.mIndexedVariables[a.synonym];
                }
                else
                {
                    a.setSynonym(this, b, 0);
                }
            }
            else
            {
                addEquality(a, b, margin, SolverVariable.STRENGTH_FIXED);
            }
        }

        /// <summary>
        /// Add an equation of the form a = b + margin </summary>
        /// <param name="a"> variable a </param>
        /// <param name="b"> variable b </param>
        /// <param name="margin"> margin used </param>
        /// <param name="strength"> strength used </param>
        public virtual ArrayRow addEquality(SolverVariable a, SolverVariable b, int margin, int strength)
        {
            if (USE_BASIC_SYNONYMS && strength == SolverVariable.STRENGTH_FIXED && b.isFinalValue && a.definitionId == -1)
            {
                if (DEBUG_CONSTRAINTS)
                {
                    Console.WriteLine("=> " + a + " = " + b + (margin != 0 ? " + " + margin : "") + " = " + (b.computedValue + margin) + " (Synonym)");
                }
                a.setFinalValue(this, b.computedValue + margin);
                return null;
            }
            if (false && USE_SYNONYMS && strength == SolverVariable.STRENGTH_FIXED && a.definitionId == -1 && margin == 0)
            {
                if (DEBUG_CONSTRAINTS)
                {
                    Console.WriteLine("(S) -> " + a + " = " + b + (margin != 0 ? " + " + margin : "") + " " + getDisplayStrength(strength));
                }
                if (b.isSynonym)
                {
                    margin += (int)b.synonymDelta;
                    b = mCache.mIndexedVariables[b.synonym];
                }
                if (a.isSynonym)
                {
                    margin -= (int)a.synonymDelta;
                    a = mCache.mIndexedVariables[a.synonym];
                }
                else
                {
                    a.setSynonym(this, b, margin);
                    return null;
                }
            }
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> " + a + " = " + b + (margin != 0 ? " + " + margin : "") + " " + getDisplayStrength(strength));
            }
            ArrayRow row = createRow();
            row.createRowEquals(a, b, margin);
            if (strength != SolverVariable.STRENGTH_FIXED)
            {
                row.addError(this, strength);
            }
            addConstraint(row);
            return row;
        }

        /// <summary>
        /// Add an equation of the form a = value </summary>
        /// <param name="a"> variable a </param>
        /// <param name="value"> the value we set </param>
        public virtual void addEquality(SolverVariable a, int value)
        {
            if (USE_BASIC_SYNONYMS && a.definitionId == -1)
            {
                if (DEBUG_CONSTRAINTS)
                {
                    Console.WriteLine("=> " + a + " = " + value + " (Synonym)");
                }
                a.setFinalValue(this, value);
                for (int i = 0; i < mVariablesID + 1; i++)
                {
                    SolverVariable variable = mCache.mIndexedVariables[i];
                    if (variable != null && variable.isSynonym && variable.synonym == a.id)
                    {
                        variable.setFinalValue(this, value + variable.synonymDelta);
                    }
                }
                return;
            }
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> " + a + " = " + value);
            }
            int idx = a.definitionId;
            if (a.definitionId != -1)
            {
                ArrayRow row = mRows[idx];
                if (row.isSimpleDefinition)
                {
                    row.constantValue = value;
                }
                else
                {
                    if (row.variables.CurrentSize == 0)
                    {
                        row.isSimpleDefinition = true;
                        row.constantValue = value;
                    }
                    else
                    {
                        ArrayRow newRow = createRow();
                        newRow.createRowEquals(a, value);
                        addConstraint(newRow);
                    }
                }
            }
            else
            {
                ArrayRow row = createRow();
                row.createRowDefinition(a, value);
                addConstraint(row);
            }
        }

        /// <summary>
        /// Create a constraint to express A = C * percent </summary>
        /// <param name="linearSystem"> the system we create the row on </param>
        /// <param name="variableA"> variable a </param>
        /// <param name="variableC"> variable c </param>
        /// <param name="percent"> the percent used </param>
        /// <returns> the created row </returns>
        public static ArrayRow createRowDimensionPercent(LinearSystem linearSystem, SolverVariable variableA, SolverVariable variableC, float percent)
        {
            if (DEBUG_CONSTRAINTS)
            {
                Console.WriteLine("-> " + variableA + " = " + variableC + " * " + percent);
            }
            ArrayRow row = linearSystem.createRow();
            return row.createRowDimensionPercent(variableA, variableC, percent);
        }

        /// <summary>
        /// Add the equations constraining a widget center to another widget center, positioned
        /// on a circle, following an angle and radius
        /// </summary>
        /// <param name="widget"> </param>
        /// <param name="target"> </param>
        /// <param name="angle"> from 0 to 360 </param>
        /// <param name="radius"> the distance between the two centers </param>
        public virtual void addCenterPoint(ConstraintWidget widget, ConstraintWidget target, float angle, int radius)
        {

            SolverVariable Al = createObjectVariable(widget.getAnchor(ConstraintAnchor.Type.LEFT));
            SolverVariable At = createObjectVariable(widget.getAnchor(ConstraintAnchor.Type.TOP));
            SolverVariable Ar = createObjectVariable(widget.getAnchor(ConstraintAnchor.Type.RIGHT));
            SolverVariable Ab = createObjectVariable(widget.getAnchor(ConstraintAnchor.Type.BOTTOM));

            SolverVariable Bl = createObjectVariable(target.getAnchor(ConstraintAnchor.Type.LEFT));
            SolverVariable Bt = createObjectVariable(target.getAnchor(ConstraintAnchor.Type.TOP));
            SolverVariable Br = createObjectVariable(target.getAnchor(ConstraintAnchor.Type.RIGHT));
            SolverVariable Bb = createObjectVariable(target.getAnchor(ConstraintAnchor.Type.BOTTOM));

            ArrayRow row = createRow();
            float angleComponent = (float)(Math.Sin(angle) * radius);
            row.createRowWithAngle(At, Ab, Bt, Bb, angleComponent);
            addConstraint(row);
            row = createRow();
            angleComponent = (float)(Math.Cos(angle) * radius);
            row.createRowWithAngle(Al, Ar, Bl, Br, angleComponent);
            addConstraint(row);
        }

    }

}