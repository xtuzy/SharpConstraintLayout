using System.Collections.Generic;
using System.Linq;

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

namespace androidx.constraintlayout.core.scout
{
    /// <summary>
    /// Possible directions for a connection
    /// </summary>
    public sealed class Direction
    {
        public static readonly Direction NORTH = new Direction("NORTH", InnerEnum.NORTH, 0);
        public static readonly Direction SOUTH = new Direction("SOUTH", InnerEnum.SOUTH, 1);
        public static readonly Direction WEST = new Direction("WEST", InnerEnum.WEST, 2);
        public static readonly Direction EAST = new Direction("EAST", InnerEnum.EAST, 3);
        public static readonly Direction BASE = new Direction("BASE", InnerEnum.BASE, 4);

        private static  IList<Direction> valueList;
        private static  IList<Direction> ValueList 
        {
            get
            {
                if (valueList == null)
                {
                    valueList = new List<Direction>();
                    InitDirection();
                }
                return valueList; 
            }
        }
        static void InitDirection()
        {
            ValueList.Add(NORTH);
            ValueList.Add(SOUTH);
            ValueList.Add(WEST);
            ValueList.Add(EAST);
            ValueList.Add(BASE);
        }

        internal Direction(string name, InnerEnum innerEnum, int n)
        {
            mDirection = n;

            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        public enum InnerEnum
        {
            NORTH,
            SOUTH,
            WEST,
            EAST,
            BASE
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;
        private readonly int mDirection;

        internal const int ORIENTATION_VERTICAL = 0;
        internal const int ORIENTATION_HORIZONTAL = 1;

        private static Direction[] sAllDirections = Direction.values().ToArray();
        private static Direction[] sVertical = new Direction[] { NORTH, SOUTH, BASE };
        private static Direction[] sHorizontal = new Direction[] { WEST, EAST };


        /// <summary>
        /// Get an array of all directions
        /// </summary>
        /// <returns> array of all directions </returns>
        internal static Direction[] AllDirections
        {
            get
            {
                return sAllDirections;
            }
        }

        /// <summary>
        /// get a String representing the direction integer
        /// </summary>
        /// <param name="directionInteger"> direction as an integer </param>
        /// <returns> single letter string to describe the direction </returns>
        internal static string toString(int directionInteger)
        {
            return Direction.get(directionInteger).ToString();
        }

        public override string ToString()
        {
            /*switch (this)
			{
				case NORTH:
					return "N";
				case SOUTH:
					return "S";
				case EAST:
					return "E";
				case WEST:
					return "W";
				case BASE:
					return "B";
			}*/
            if (this == NORTH)
                return "N";
            if (this == SOUTH)
                return "S";
            if (this == EAST)
                return "E";
            if (this == WEST)
                return "W";
            if (this == BASE)
                return "B";

            return "?";
        }

        /// <summary>
        /// get the direction as an integer
        /// </summary>
        /// <returns> direction as an integer </returns>
        internal int getDirection()
        {
            return mDirection;
        }

        /// <summary>
        /// gets the opposite direction
        /// </summary>
        /// <returns> the opposite direction </returns>
        internal Direction Opposite
        {
            get
            {
                /*switch (this)
                {
                    case NORTH:
                        return SOUTH;
                    case SOUTH:
                        return NORTH;
                    case EAST:
                        return WEST;
                    case WEST:
                        return EAST;
                    case BASE:
                        return BASE;
                    default:
                        return BASE;
                }*/

                if (this == NORTH)
                    return SOUTH;
                if (this == SOUTH)
                    return NORTH;
                if (this == EAST)
                    return WEST;
                if (this == WEST)
                    return EAST;
                if (this == BASE)
                    return BASE;

                return BASE;
            }
        }


        /// <summary>
        /// convert from an ordinal of direction to actual direction
        /// </summary>
        /// <param name="directionInteger"> </param>
        /// <returns> Enum member equivalent to integer </returns>
        internal static Direction get(int directionInteger)
        {
            return sAllDirections[directionInteger];
        }

        /// <summary>
        /// Directions can be a positive or negative (right and down) being positive
        /// reverse indicates the direction is negative
        /// </summary>
        /// <returns> true for north and east </returns>
        internal bool reverse()
        {
            return (this == NORTH || this == WEST);
        }

        /// <summary>
        /// gets the viable directions for horizontal or vertical
        /// </summary>
        /// <param name="orientation"> 0 = vertical 1 = horizontal </param>
        /// <returns> array of directions for vertical or horizontal </returns>
        internal static Direction[] getDirections(int orientation)
        {
            if (orientation == ORIENTATION_VERTICAL)
            {
                return sVertical;
            }
            return sHorizontal;
        }

        /// <summary>
        /// Return the number of connection types support by this direction
        /// </summary>
        /// <returns> number of types allowed for this connection </returns>
        public int connectTypes()
        {
            /*switch (this)
            {
                case NORTH:
                case SOUTH:
                    return 2;
                case EAST:
                case WEST:
                    return 2;
                case BASE:
                    return 1;
            }*/
            if (this == NORTH || this == SOUTH)
                return 2;
            if (this == EAST || this == WEST)
                return 2;
            if (this == BASE)
                return 1;

            return 1;
        }

        public static IList<Direction> values()
        {
            return ValueList;
        }

        public int ordinal()
        {
            return ordinalValue;
        }

        public static Direction valueOf(string name)
        {
            foreach (Direction enumInstance in Direction.ValueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }

}