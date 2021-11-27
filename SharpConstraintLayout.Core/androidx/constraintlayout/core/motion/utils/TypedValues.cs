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

namespace androidx.constraintlayout.core.motion.utils
{
    /// <summary>
    /// Provides an interface to values used in KeyFrames and in
    /// Starting and Ending Widgets
    /// </summary>
    public abstract class TypedValues
    {

        /// <summary>
        /// Used to set integer values
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="value"> </param>
        /// <returns> true if it accepted the value </returns>
        public abstract bool setValue(int id, int value);

        /// <summary>
        /// Used to set float values
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="value"> </param>
        /// <returns> true if it accepted the value </returns>
        public abstract bool setValue(int id, float value);

        /// <summary>
        /// Used to set String values
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="value"> </param>
        /// <returns> true if it accepted the value </returns>
        public abstract bool setValue(int id, string value);

        /// <summary>
        /// Used to set boolean values
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="value"> </param>
        /// <returns> true if it accepted the value </returns>
        public abstract bool setValue(int id, bool value);

        public abstract int getId(string name);

        public const string S_CUSTOM = "CUSTOM";
        public const int BOOLEAN_MASK = 1;
        public const int INT_MASK = 2;
        public const int FLOAT_MASK = 4;
        public const int STRING_MASK = 8;
        public const int TYPE_FRAME_POSITION = 100;
        public const int TYPE_TARGET = 101;


        public static class TypedValues_Attributes
        {

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_CURVE_FIT:
                        return TYPE_CURVE_FIT;
                    case S_VISIBILITY:
                        return TYPE_VISIBILITY;
                    case S_ALPHA:
                        return TYPE_ALPHA;
                    case S_TRANSLATION_X:
                        return TYPE_TRANSLATION_X;
                    case S_TRANSLATION_Y:
                        return TYPE_TRANSLATION_Y;
                    case S_TRANSLATION_Z:
                        return TYPE_TRANSLATION_Z;
                    case S_ELEVATION:
                        return TYPE_ELEVATION;
                    case S_ROTATION_X:
                        return TYPE_ROTATION_X;
                    case S_ROTATION_Y:
                        return TYPE_ROTATION_Y;
                    case S_ROTATION_Z:
                        return TYPE_ROTATION_Z;
                    case S_SCALE_X:
                        return TYPE_SCALE_X;
                    case S_SCALE_Y:
                        return TYPE_SCALE_Y;
                    case S_PIVOT_X:
                        return TYPE_PIVOT_X;
                    case S_PIVOT_Y:
                        return TYPE_PIVOT_Y;
                    case S_PROGRESS:
                        return TYPE_PROGRESS;
                    case S_PATH_ROTATE:
                        return TYPE_PATH_ROTATE;
                    case S_EASING:
                        return TYPE_EASING;
                    case S_FRAME:
                        return TYPE_FRAME_POSITION;
                    case S_TARGET:
                        return TYPE_TARGET;
                    case S_PIVOT_TARGET:
                        return TYPE_PIVOT_TARGET;
                }
                return -1;
            }

            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            internal  static int getType(int name)
            {
                switch (name)
                {
                    case TYPE_CURVE_FIT:
                    case TYPE_VISIBILITY:
                    case TYPE_FRAME_POSITION:
                        return INT_MASK;
                    case TYPE_ALPHA:
                    case TYPE_TRANSLATION_X:
                    case TYPE_TRANSLATION_Y:
                    case TYPE_TRANSLATION_Z:
                    case TYPE_ELEVATION:
                    case TYPE_ROTATION_X:
                    case TYPE_ROTATION_Y:
                    case TYPE_ROTATION_Z:
                    case TYPE_SCALE_X:
                    case TYPE_SCALE_Y:
                    case TYPE_PIVOT_X:
                    case TYPE_PIVOT_Y:
                    case TYPE_PROGRESS:
                    case TYPE_PATH_ROTATE:
                        return FLOAT_MASK;
                    case TYPE_EASING:
                    case TYPE_TARGET:
                    case TYPE_PIVOT_TARGET:
                        return STRING_MASK;
                }
                return -1;
            }
        

        
            public const string NAME = "KeyAttributes";
            public const int TYPE_CURVE_FIT = 301;
            public const int TYPE_VISIBILITY = 302;
            public const int TYPE_ALPHA = 303;
            public const int TYPE_TRANSLATION_X = 304;
            public const int TYPE_TRANSLATION_Y = 305;
            public const int TYPE_TRANSLATION_Z = 306;
            public const int TYPE_ELEVATION = 307;
            public const int TYPE_ROTATION_X = 308;
            public const int TYPE_ROTATION_Y = 309;
            public const int TYPE_ROTATION_Z = 310;
            public const int TYPE_SCALE_X = 311;
            public const int TYPE_SCALE_Y = 312;
            public const int TYPE_PIVOT_X = 313;
            public const int TYPE_PIVOT_Y = 314;
            public const int TYPE_PROGRESS = 315;
            public const int TYPE_PATH_ROTATE = 316;
            public const int TYPE_EASING = 317;
            public const int TYPE_PIVOT_TARGET = 318;
            public const string S_CURVE_FIT = "curveFit";
            public const string S_VISIBILITY = "visibility";
            public const string S_ALPHA = "alpha";
            public const string S_TRANSLATION_X = "translationX";
            public const string S_TRANSLATION_Y = "translationY";
            public const string S_TRANSLATION_Z = "translationZ";
            public const string S_ELEVATION = "elevation";
            public const string S_ROTATION_X = "rotationX";
            public const string S_ROTATION_Y = "rotationY";
            public const string S_ROTATION_Z = "rotationZ";
            public const string S_SCALE_X = "scaleX";
            public const string S_SCALE_Y = "scaleY";
            public const string S_PIVOT_X = "pivotX";
            public const string S_PIVOT_Y = "pivotY";
            public const string S_PROGRESS = "progress";
            public const string S_PATH_ROTATE = "pathRotate";
            public const string S_EASING = "easing";
            public const string S_CUSTOM = "CUSTOM";
            public const string S_FRAME = "frame";
            public const string S_TARGET = "target";
            public const string S_PIVOT_TARGET = "pivotTarget";
            public static readonly string[] KEY_WORDS = new string[] { S_CURVE_FIT, S_VISIBILITY, S_ALPHA, S_TRANSLATION_X, S_TRANSLATION_Y, S_TRANSLATION_Z, S_ELEVATION, S_ROTATION_X, S_ROTATION_Y, S_ROTATION_Z, S_SCALE_X, S_SCALE_Y, S_PIVOT_X, S_PIVOT_Y, S_PROGRESS, S_PATH_ROTATE, S_EASING, S_CUSTOM, S_FRAME, S_TARGET, S_PIVOT_TARGET };
        }

        public static class TypedValues_Cycle
        {

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_CURVE_FIT:
                        return TYPE_CURVE_FIT;
                    case S_VISIBILITY:
                        return TYPE_VISIBILITY;
                    case S_ALPHA:
                        return TYPE_ALPHA;
                    case S_TRANSLATION_X:
                        return TYPE_TRANSLATION_X;
                    case S_TRANSLATION_Y:
                        return TYPE_TRANSLATION_Y;
                    case S_TRANSLATION_Z:
                        return TYPE_TRANSLATION_Z;
                    case S_ROTATION_X:
                        return TYPE_ROTATION_X;
                    case S_ROTATION_Y:
                        return TYPE_ROTATION_Y;
                    case S_ROTATION_Z:
                        return TYPE_ROTATION_Z;
                    case S_SCALE_X:
                        return TYPE_SCALE_X;
                    case S_SCALE_Y:
                        return TYPE_SCALE_Y;
                    case S_PIVOT_X:
                        return TYPE_PIVOT_X;
                    case S_PIVOT_Y:
                        return TYPE_PIVOT_Y;
                    case S_PROGRESS:
                        return TYPE_PROGRESS;
                    case S_PATH_ROTATE:
                        return TYPE_PATH_ROTATE;
                    case S_EASING:
                        return TYPE_EASING;
                }
                return -1;
            }

            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            static int getType(int name)
            {
                switch (name)
                {
                    case TYPE_CURVE_FIT:
                    case TYPE_VISIBILITY:
                    case TYPE_FRAME_POSITION:
                        return INT_MASK;
                    case TYPE_ALPHA:
                    case TYPE_TRANSLATION_X:
                    case TYPE_TRANSLATION_Y:
                    case TYPE_TRANSLATION_Z:
                    case TYPE_ELEVATION:
                    case TYPE_ROTATION_X:
                    case TYPE_ROTATION_Y:
                    case TYPE_ROTATION_Z:
                    case TYPE_SCALE_X:
                    case TYPE_SCALE_Y:
                    case TYPE_PIVOT_X:
                    case TYPE_PIVOT_Y:
                    case TYPE_PROGRESS:
                    case TYPE_PATH_ROTATE:
                    case TYPE_WAVE_PERIOD:
                    case TYPE_WAVE_OFFSET:
                    case TYPE_WAVE_PHASE:
                        return FLOAT_MASK;
                    case TYPE_EASING:
                    case TYPE_TARGET:
                    case TYPE_WAVE_SHAPE:
                        return STRING_MASK;
                }
                return -1;
            }
        

        
            public const string NAME = "KeyCycle";
            public const int TYPE_CURVE_FIT = 401;
            public const int TYPE_VISIBILITY = 402;
            public const int TYPE_ALPHA = 403;
            public const int TYPE_TRANSLATION_X = TypedValues_Attributes.TYPE_TRANSLATION_X;
            public const int TYPE_TRANSLATION_Y = TypedValues_Attributes.TYPE_TRANSLATION_Y;
            public const int TYPE_TRANSLATION_Z = TypedValues_Attributes.TYPE_TRANSLATION_Z;
            public const int TYPE_ELEVATION = TypedValues_Attributes.TYPE_ELEVATION;
            public const int TYPE_ROTATION_X = TypedValues_Attributes.TYPE_ROTATION_X;
            public const int TYPE_ROTATION_Y = TypedValues_Attributes.TYPE_ROTATION_Y;
            public const int TYPE_ROTATION_Z = TypedValues_Attributes.TYPE_ROTATION_Z;
            public const int TYPE_SCALE_X = TypedValues_Attributes.TYPE_SCALE_X;
            public const int TYPE_SCALE_Y = TypedValues_Attributes.TYPE_SCALE_Y;
            public const int TYPE_PIVOT_X = TypedValues_Attributes.TYPE_PIVOT_X;
            public const int TYPE_PIVOT_Y = TypedValues_Attributes.TYPE_PIVOT_Y;
            public const int TYPE_PROGRESS = TypedValues_Attributes.TYPE_PROGRESS;
            public const int TYPE_PATH_ROTATE = 416;
            public const int TYPE_EASING = 420;
            public const int TYPE_WAVE_SHAPE = 421;
            public const int TYPE_CUSTOM_WAVE_SHAPE = 422;
            public const int TYPE_WAVE_PERIOD = 423;
            public const int TYPE_WAVE_OFFSET = 424;
            public const int TYPE_WAVE_PHASE = 425;
            public const string S_CURVE_FIT = "curveFit";
            public const string S_VISIBILITY = "visibility";
            public const string S_ALPHA = TypedValues_Attributes.S_ALPHA;
            public const string S_TRANSLATION_X = TypedValues_Attributes.S_TRANSLATION_X;
            public const string S_TRANSLATION_Y = TypedValues_Attributes.S_TRANSLATION_Y;
            public const string S_TRANSLATION_Z = TypedValues_Attributes.S_TRANSLATION_Z;
            public const string S_ELEVATION = TypedValues_Attributes.S_ELEVATION;
            public const string S_ROTATION_X = TypedValues_Attributes.S_ROTATION_X;
            public const string S_ROTATION_Y = TypedValues_Attributes.S_ROTATION_Y;
            public const string S_ROTATION_Z = TypedValues_Attributes.S_ROTATION_Z;
            public const string S_SCALE_X = TypedValues_Attributes.S_SCALE_X;
            public const string S_SCALE_Y = TypedValues_Attributes.S_SCALE_Y;
            public const string S_PIVOT_X = TypedValues_Attributes.S_PIVOT_X;
            public const string S_PIVOT_Y = TypedValues_Attributes.S_PIVOT_Y;
            public const string S_PROGRESS = TypedValues_Attributes.S_PROGRESS;
            public const string S_PATH_ROTATE = "pathRotate";
            public const string S_EASING = "easing";
            public const string S_WAVE_SHAPE = "waveShape";
            public const string S_CUSTOM_WAVE_SHAPE = "customWave";
            public const string S_WAVE_PERIOD = "period";
            public const string S_WAVE_OFFSET = "offset";
            public const string S_WAVE_PHASE = "phase";
            public static readonly string[] KEY_WORDS = new string[] { S_CURVE_FIT, S_VISIBILITY, S_ALPHA, S_TRANSLATION_X, S_TRANSLATION_Y, S_TRANSLATION_Z, S_ELEVATION, S_ROTATION_X, S_ROTATION_Y, S_ROTATION_Z, S_SCALE_X, S_SCALE_Y, S_PIVOT_X, S_PIVOT_Y, S_PROGRESS, S_PATH_ROTATE, S_EASING, S_WAVE_SHAPE, S_CUSTOM_WAVE_SHAPE, S_WAVE_PERIOD, S_WAVE_OFFSET, S_WAVE_PHASE };
        }

        public static class TypedValues_Trigger
        {

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case VIEW_TRANSITION_ON_CROSS:
                        return TYPE_VIEW_TRANSITION_ON_CROSS;
                    case VIEW_TRANSITION_ON_POSITIVE_CROSS:
                        return TYPE_VIEW_TRANSITION_ON_POSITIVE_CROSS;
                    case VIEW_TRANSITION_ON_NEGATIVE_CROSS:
                        return TYPE_VIEW_TRANSITION_ON_NEGATIVE_CROSS;
                    case POST_LAYOUT:
                        return TYPE_POST_LAYOUT;
                    case TRIGGER_SLACK:
                        return TYPE_TRIGGER_SLACK;
                    case TRIGGER_COLLISION_VIEW:
                        return TYPE_TRIGGER_COLLISION_VIEW;
                    case TRIGGER_COLLISION_ID:
                        return TYPE_TRIGGER_COLLISION_ID;
                    case TRIGGER_ID:
                        return TYPE_TRIGGER_ID;
                    case POSITIVE_CROSS:
                        return TYPE_POSITIVE_CROSS;
                    case NEGATIVE_CROSS:
                        return TYPE_NEGATIVE_CROSS;
                    case TRIGGER_RECEIVER:
                        return TYPE_TRIGGER_RECEIVER;
                    case CROSS:
                        return TYPE_CROSS;
                }
                return -1;
            }
        

        
            public const string NAME = "KeyTrigger";
            public const string VIEW_TRANSITION_ON_CROSS = "viewTransitionOnCross";
            public const string VIEW_TRANSITION_ON_POSITIVE_CROSS = "viewTransitionOnPositiveCross";
            public const string VIEW_TRANSITION_ON_NEGATIVE_CROSS = "viewTransitionOnNegativeCross";
            public const string POST_LAYOUT = "postLayout";
            public const string TRIGGER_SLACK = "triggerSlack";
            public const string TRIGGER_COLLISION_VIEW = "triggerCollisionView";
            public const string TRIGGER_COLLISION_ID = "triggerCollisionId";
            public const string TRIGGER_ID = "triggerID";
            public const string POSITIVE_CROSS = "positiveCross";
            public const string NEGATIVE_CROSS = "negativeCross";
            public const string TRIGGER_RECEIVER = "triggerReceiver";
            public const string CROSS = "CROSS";
            public static readonly string[] KEY_WORDS = new string[] { VIEW_TRANSITION_ON_CROSS, VIEW_TRANSITION_ON_POSITIVE_CROSS, VIEW_TRANSITION_ON_NEGATIVE_CROSS, POST_LAYOUT, TRIGGER_SLACK, TRIGGER_COLLISION_VIEW, TRIGGER_COLLISION_ID, TRIGGER_ID, POSITIVE_CROSS, NEGATIVE_CROSS, TRIGGER_RECEIVER, CROSS };
            public const int TYPE_VIEW_TRANSITION_ON_CROSS = 301;
            public const int TYPE_VIEW_TRANSITION_ON_POSITIVE_CROSS = 302;
            public const int TYPE_VIEW_TRANSITION_ON_NEGATIVE_CROSS = 303;
            public const int TYPE_POST_LAYOUT = 304;
            public const int TYPE_TRIGGER_SLACK = 305;
            public const int TYPE_TRIGGER_COLLISION_VIEW = 306;
            public const int TYPE_TRIGGER_COLLISION_ID = 307;
            public const int TYPE_TRIGGER_ID = 308;
            public const int TYPE_POSITIVE_CROSS = 309;
            public const int TYPE_NEGATIVE_CROSS = 310;
            public const int TYPE_TRIGGER_RECEIVER = 311;
            public const int TYPE_CROSS = 312;
        }

        public static class TypedValues_Position
        {

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_TRANSITION_EASING:
                        return TYPE_TRANSITION_EASING;
                    case S_DRAWPATH:
                        return TYPE_DRAWPATH;
                    case S_PERCENT_WIDTH:
                        return TYPE_PERCENT_WIDTH;
                    case S_PERCENT_HEIGHT:
                        return TYPE_PERCENT_HEIGHT;
                    case S_SIZE_PERCENT:
                        return TYPE_SIZE_PERCENT;
                    case S_PERCENT_X:
                        return TYPE_PERCENT_X;
                    case S_PERCENT_Y:
                        return TYPE_PERCENT_Y;
                }
                return -1;
            }

            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            static int getType(int name)
            {
                switch (name)
                {
                    case TYPE_CURVE_FIT:
                    case TYPE_FRAME_POSITION:
                        return INT_MASK;
                    case TYPE_PERCENT_WIDTH:
                    case TYPE_PERCENT_HEIGHT:
                    case TYPE_SIZE_PERCENT:
                    case TYPE_PERCENT_X:
                    case TYPE_PERCENT_Y:
                        return FLOAT_MASK;
                    case TYPE_TRANSITION_EASING:
                    case TYPE_TARGET:
                    case TYPE_DRAWPATH:
                        return STRING_MASK;
                }
                return -1;
            }


        
            public const string NAME = "KeyPosition";
            public const string S_TRANSITION_EASING = "transitionEasing";
            public const string S_DRAWPATH = "drawPath";
            public const string S_PERCENT_WIDTH = "percentWidth";
            public const string S_PERCENT_HEIGHT = "percentHeight";
            public const string S_SIZE_PERCENT = "sizePercent";
            public const string S_PERCENT_X = "percentX";
            public const string S_PERCENT_Y = "percentY";
            public const int TYPE_TRANSITION_EASING = 501;
            public const int TYPE_DRAWPATH = 502;
            public const int TYPE_PERCENT_WIDTH = 503;
            public const int TYPE_PERCENT_HEIGHT = 504;
            public const int TYPE_SIZE_PERCENT = 505;
            public const int TYPE_PERCENT_X = 506;
            public const int TYPE_PERCENT_Y = 507;
            public const int TYPE_CURVE_FIT = 508;
            public const int TYPE_PATH_MOTION_ARC = 509;
            public const int TYPE_POSITION_TYPE = 510;
            public static readonly string[] KEY_WORDS = new string[] { S_TRANSITION_EASING, S_DRAWPATH, S_PERCENT_WIDTH, S_PERCENT_HEIGHT, S_SIZE_PERCENT, S_PERCENT_X, S_PERCENT_Y };
        }

        public static class TypedValues_Motion
        {

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_STAGGER:
                        return TYPE_STAGGER;
                    case S_PATH_ROTATE:
                        return TYPE_PATH_ROTATE;
                    case S_QUANTIZE_MOTION_PHASE:
                        return TYPE_QUANTIZE_MOTION_PHASE;
                    case S_EASING:
                        return TYPE_EASING;
                    case S_QUANTIZE_INTERPOLATOR:
                        return TYPE_QUANTIZE_INTERPOLATOR;
                    case S_ANIMATE_RELATIVE_TO:
                        return TYPE_ANIMATE_RELATIVE_TO;
                    case S_ANIMATE_CIRCLEANGLE_TO:
                        return TYPE_ANIMATE_CIRCLEANGLE_TO;
                    case S_PATHMOTION_ARC:
                        return TYPE_PATHMOTION_ARC;
                    case S_DRAW_PATH:
                        return TYPE_DRAW_PATH;
                    case S_POLAR_RELATIVETO:
                        return TYPE_POLAR_RELATIVETO;
                    case S_QUANTIZE_MOTIONSTEPS:
                        return TYPE_QUANTIZE_MOTIONSTEPS;
                    case S_QUANTIZE_INTERPOLATOR_TYPE:
                        return TYPE_QUANTIZE_INTERPOLATOR_TYPE;
                    case S_QUANTIZE_INTERPOLATOR_ID:
                        return TYPE_QUANTIZE_INTERPOLATOR_ID;
                }
                return -1;
            }

        
            public const string NAME = "Motion";
            public const string S_STAGGER = "Stagger";
            public const string S_PATH_ROTATE = "PathRotate";
            public const string S_QUANTIZE_MOTION_PHASE = "QuantizeMotionPhase";
            public const string S_EASING = "TransitionEasing";
            public const string S_QUANTIZE_INTERPOLATOR = "QuantizeInterpolator";
            public const string S_ANIMATE_RELATIVE_TO = "AnimateRelativeTo";
            public const string S_ANIMATE_CIRCLEANGLE_TO = "AnimateCircleAngleTo";
            public const string S_PATHMOTION_ARC = "PathMotionArc";
            public const string S_DRAW_PATH = "DrawPath";
            public const string S_POLAR_RELATIVETO = "PolarRelativeTo";
            public const string S_QUANTIZE_MOTIONSTEPS = "QuantizeMotionSteps";
            public const string S_QUANTIZE_INTERPOLATOR_TYPE = "QuantizeInterpolatorType";
            public const string S_QUANTIZE_INTERPOLATOR_ID = "QuantizeInterpolatorID";
            public static readonly string[] KEY_WORDS = new string[] { S_STAGGER, S_PATH_ROTATE, S_QUANTIZE_MOTION_PHASE, S_EASING, S_QUANTIZE_INTERPOLATOR, S_ANIMATE_RELATIVE_TO, S_ANIMATE_CIRCLEANGLE_TO, S_PATHMOTION_ARC, S_DRAW_PATH, S_POLAR_RELATIVETO, S_QUANTIZE_MOTIONSTEPS, S_QUANTIZE_INTERPOLATOR_TYPE, S_QUANTIZE_INTERPOLATOR_ID };
            public const int TYPE_STAGGER = 600;
            public const int TYPE_PATH_ROTATE = 601;
            public const int TYPE_QUANTIZE_MOTION_PHASE = 602;
            public const int TYPE_EASING = 603;
            public const int TYPE_QUANTIZE_INTERPOLATOR = 604;
            public const int TYPE_ANIMATE_RELATIVE_TO = 605;
            public const int TYPE_ANIMATE_CIRCLEANGLE_TO = 606;
            public const int TYPE_PATHMOTION_ARC = 607;
            public const int TYPE_DRAW_PATH = 608;
            public const int TYPE_POLAR_RELATIVETO = 609;
            public const int TYPE_QUANTIZE_MOTIONSTEPS = 610;
            public const int TYPE_QUANTIZE_INTERPOLATOR_TYPE = 611;
            public const int TYPE_QUANTIZE_INTERPOLATOR_ID = 612;
        }

        public static class TypedValues_Custom
        {

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_INT:
                        return TYPE_INT;
                    case S_FLOAT:
                        return TYPE_FLOAT;
                    case S_COLOR:
                        return TYPE_COLOR;
                    case S_STRING:
                        return TYPE_STRING;
                    case S_BOOLEAN:
                        return TYPE_BOOLEAN;
                    case S_DIMENSION:
                        return TYPE_DIMENSION;
                    case S_REFERENCE:
                        return TYPE_REFERENCE;
                }
                return -1;
            }
       
            public const string NAME = "Custom";
            public const string S_INT = "integer";
            public const string S_FLOAT = "float";
            public const string S_COLOR = "color";
            public const string S_STRING = "string";
            public const string S_BOOLEAN = "boolean";
            public const string S_DIMENSION = "dimension";
            public const string S_REFERENCE = "refrence";
            public static readonly string[] KEY_WORDS = new string[] { S_FLOAT, S_COLOR, S_STRING, S_BOOLEAN, S_DIMENSION, S_REFERENCE };
            public const int TYPE_INT = 900;
            public const int TYPE_FLOAT = 901;
            public const int TYPE_COLOR = 902;
            public const int TYPE_STRING = 903;
            public const int TYPE_BOOLEAN = 904;
            public const int TYPE_DIMENSION = 905;
            public const int TYPE_REFERENCE = 906;
        }

        public static class TypedValues_MotionScene
        {

            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getType(int name)
            {
                switch (name)
                {
                    case TYPE_DEFAULT_DURATION:
                        return INT_MASK;
                    case TYPE_LAYOUT_DURING_TRANSITION:
                        return BOOLEAN_MASK;
                }
                return -1;
            }

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_DEFAULT_DURATION:
                        return TYPE_DEFAULT_DURATION;
                    case S_LAYOUT_DURING_TRANSITION:
                        return TYPE_LAYOUT_DURING_TRANSITION;
                }
                return -1;
            }
        
            public const string NAME = "MotionScene";
            public const string S_DEFAULT_DURATION = "defaultDuration";
            public const string S_LAYOUT_DURING_TRANSITION = "layoutDuringTransition";
            public const int TYPE_DEFAULT_DURATION = 600;
            public const int TYPE_LAYOUT_DURING_TRANSITION = 601;
            public static readonly string[] KEY_WORDS = new string[] { S_DEFAULT_DURATION, S_LAYOUT_DURING_TRANSITION };
        }

        public static class TypedValues_Transition
        {


            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getType(int name)
            {
                switch (name)
                {
                    case TYPE_DURATION:
                    case TYPE_PATH_MOTION_ARC:
                        return INT_MASK;
                    case TYPE_FROM:
                    case TYPE_TO:
                    case TYPE_INTERPOLATOR:
                    case TYPE_TRANSITION_FLAGS:
                        return STRING_MASK;

                    case TYPE_STAGGERED:
                        return FLOAT_MASK;
                }
                return -1;
            }

            /// <summary>
            /// Method to go from String names of values to id of the values
            /// IDs are use for efficiency
            /// </summary>
            /// <param name="name"> the name of the value </param>
            /// <returns> the id of the vlalue or -1 if no value exist </returns>
            //JAVA TO C# CONVERTER TODO TASK: There is no equivalent in C# to Java static interface methods:
            public static int getId(string name)
            {
                switch (name)
                {
                    case S_DURATION:
                        return TYPE_DURATION;
                    case S_FROM:
                        return TYPE_FROM;
                    case S_TO:
                        return TYPE_TO;
                    case S_PATH_MOTION_ARC:
                        return TYPE_PATH_MOTION_ARC;
                    case S_AUTO_TRANSITION:
                        return TYPE_AUTO_TRANSITION;
                    case S_INTERPOLATOR:
                        return TYPE_INTERPOLATOR;
                    case S_STAGGERED:
                        return TYPE_STAGGERED;
                    case S_TRANSITION_FLAGS:
                        return TYPE_TRANSITION_FLAGS;
                }
                return -1;
            }
        
            public const string NAME = "Transitions";
            public const string S_DURATION = "duration";
            public const string S_FROM = "from";
            public const string S_TO = "to";
            public const string S_PATH_MOTION_ARC = "pathMotionArc";
            public const string S_AUTO_TRANSITION = "autoTransition";
            public const string S_INTERPOLATOR = "motionInterpolator";
            public const string S_STAGGERED = "staggered";
            public const string S_TRANSITION_FLAGS = "transitionFlags";
            public const int TYPE_DURATION = 700;
            public const int TYPE_FROM = 701;
            public const int TYPE_TO = 702;
            public const int TYPE_PATH_MOTION_ARC = TypedValues_Position.TYPE_PATH_MOTION_ARC;
            public const int TYPE_AUTO_TRANSITION = 704;
            public const int TYPE_INTERPOLATOR = 705;
            public const int TYPE_STAGGERED = 706;
            public const int TYPE_TRANSITION_FLAGS = 707;
            public static readonly string[] KEY_WORDS = new string[] { S_DURATION, S_FROM, S_TO, S_PATH_MOTION_ARC, S_AUTO_TRANSITION, S_INTERPOLATOR, S_STAGGERED, S_FROM, S_TRANSITION_FLAGS };
        }

        public static class TypedValues_OnSwipe
        {


       
            public const string DRAG_SCALE = "dragscale";
            public const string DRAG_THRESHOLD = "dragthreshold";
            public const string MAX_VELOCITY = "maxvelocity";
            public const string MAX_ACCELERATION = "maxacceleration";
            public const string SPRING_MASS = "springmass";
            public const string SPRING_STIFFNESS = "springstiffness";
            public const string SPRING_DAMPING = "springdamping";
            public const string SPRINGS_TOP_THRESHOLD = "springstopthreshold";
            public const string DRAG_DIRECTION = "dragdirection";
            public const string TOUCH_ANCHOR_ID = "touchanchorid";
            public const string TOUCH_ANCHOR_SIDE = "touchanchorside";
            public const string ROTATION_CENTER_ID = "rotationcenterid";
            public const string TOUCH_REGION_ID = "touchregionid";
            public const string LIMIT_BOUNDS_TO = "limitboundsto";
            public const string MOVE_WHEN_SCROLLAT_TOP = "movewhenscrollattop";
            public const string ON_TOUCH_UP = "ontouchup";
            public static readonly string[] ON_TOUCH_UP_ENUM = new string[] { "autoComplete", "autoCompleteToStart", "autoCompleteToEnd", "stop", "decelerate", "decelerateAndComplete", "neverCompleteToStart", "neverCompleteToEnd" };
            public const string SPRING_BOUNDARY = "springboundary";
            public static readonly string[] SPRING_BOUNDARY_ENUM = new string[] { "overshoot", "bounceStart", "bounceEnd", "bounceBoth" };
            public const string AUTOCOMPLETE_MODE = "autocompletemode";
            public static readonly string[] AUTOCOMPLETE_MODE_ENUM = new string[] { "continuousVelocity", "spring" };
            public const string NESTED_SCROLL_FLAGS = "nestedscrollflags";
            public static readonly string[] NESTED_SCROLL_FLAGS_ENUM = new string[] { "none", "disablePostScroll", "disableScroll", "supportScrollUp" };
        }
    }

}