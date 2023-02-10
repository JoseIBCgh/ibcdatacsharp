﻿using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Intrinsics.X86;

namespace ibcdatacsharp.UI
{
    public static class Config
    {
        public static string[] allowedExtensions = new string[] { ".txt", ".csv", ".c3d", ".avi", ".mov", ".mp4" };
        public const bool showOnlyInitialPath = true;
        public static string INITIAL_PATH
        {
            get
            {
                string userName = Environment.UserName;
                return "C:\\Users\\" + userName + "\\Documents";
            }
        }
        public const int VIDEO_FPS = 30;
        public const int VIDEO_FPS_SAVE = 25;
        public const int FRAME_HEIGHT = 480;
        public const int FRAME_WIDTH = 640;
        public const int RENDER_MS_CAPTUE = 40;
        public static readonly MatType DEFAULT_MAT_TYPE = MatType.CV_8UC3; //Se tienen que grabar todos los frames con el mismo tipo de datos
        public static MatType? MAT_TYPE = null; 
        public const string csvHeader1IMU = @"DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT
            TIME	TIME	ACC_X	ACC_Y	ACC_Z	GYR_X	GYR_Y	GYR_Z	MAG_X	MAG_Y	MAG_Z	LACC_X	LACC_Y	LACC_Z	QUAT_X	QUAT_Y	QUAT_Z	QUAT_W
            FRAME_NUMBERS	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG
            ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL
        ITEM	0	0	x	x	x	x	x	x	x	x	x	x	x	x	x	x	x	x"
        +"\n";
        public const string csvHeader2IMUs = @"DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT
            TIME	TIME	ANG_X	ANG_Y	ANG_Z	VEL_X	VEL_Y	VEL_Z	ACC_X	ACC_Y	ACC_Z
            FRAME_NUMBERS	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG
            ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL
        ITEM	0	0	x	x	x	x	x	x	x	x	x"
        + "\n";
        public const string csvHeaderSagital = @"DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT
            TIME	TIME	RIGHT_ANKLE	RIGHT_HIP	RIGHT_KNEE
            FRAME_NUMBERS	ANALOG	ANALOG	ANALOG	ANALOG
            ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL
        ITEM	0	0	x	x	x"
        + "\n";
        public static Color colorX = Color.Red;
        public static Color colorY = Color.Green;
        public static Color colorZ = Color.Blue;
        public static Color colorW = Color.Orange;
    }
}
