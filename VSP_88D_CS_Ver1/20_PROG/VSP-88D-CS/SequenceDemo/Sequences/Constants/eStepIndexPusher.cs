using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceDemo.Sequences.Constants
{
    public enum eStepIndexPusher
    {
        IDLE                                    = -1,

        START                                   = 0,

        CHECK_PLASMA                            = 1,

        M_STEP10_LOAD_STRIP_TO_CHAMBER          = 10,
            S_Step11_Servo_X_Load_Start,
            S_Step12_Cylinder_Down,
            S_Step13_Servo_X_Load_End,
            S_Step14_Servo_X_In_Edge,
            S_Step15_Cylinder_Up,

        M_STEP50_LOAD_STRIP_TO_UNLOAD           = 50,
            S_Step51_Servo_X_Unload_Start,
            S_Step52_Cylinder_Down,
            S_Step53_Servo_X_Unload_End,
            S_Step54_Cylinder_Up,
            S_Step55_Servo_X_Ready,

        END
    }
}
