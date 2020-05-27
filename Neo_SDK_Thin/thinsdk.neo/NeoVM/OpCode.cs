﻿namespace ThinSdk.Neo.VM
{
    public enum OpCode : byte
    {
        PUSHINT8 = 0,
        PUSHINT16 = 1,
        PUSHINT32 = 2,
        PUSHINT64 = 3,
        PUSHINT128 = 4,
        PUSHINT256 = 5,
        PUSHA = 10,
        PUSHNULL = 11,
        PUSHDATA1 = 12,
        PUSHDATA2 = 13,
        PUSHDATA4 = 14,
        PUSHM1 = 15,
        PUSH0 = 16,
        PUSH1 = 17,
        PUSH2 = 18,
        PUSH3 = 19,
        PUSH4 = 20,
        PUSH5 = 21,
        PUSH6 = 22,
        PUSH7 = 23,
        PUSH8 = 24,
        PUSH9 = 25,
        PUSH10 = 26,
        PUSH11 = 27,
        PUSH12 = 28,
        PUSH13 = 29,
        PUSH14 = 30,
        PUSH15 = 31,
        PUSH16 = 32,
        NOP = 33,
        JMP = 34,
        JMP_L = 35,
        JMPIF = 36,
        JMPIF_L = 37,
        JMPIFNOT = 38,
        JMPIFNOT_L = 39,
        JMPEQ = 40,
        JMPEQ_L = 41,
        JMPNE = 42,
        JMPNE_L = 43,
        JMPGT = 44,
        JMPGT_L = 45,
        JMPGE = 46,
        JMPGE_L = 47,
        JMPLT = 48,
        JMPLT_L = 49,
        JMPLE = 50,
        JMPLE_L = 51,
        CALL = 52,
        CALL_L = 53,
        CALLA = 54,
        ABORT = 55,
        ASSERT = 56,
        THROW = 58,
        RET = 64,
        SYSCALL = 65,
        DEPTH = 67,
        DROP = 69,
        NIP = 70,
        XDROP = 72,
        CLEAR = 73,
        DUP = 74,
        OVER = 75,
        PICK = 77,
        TUCK = 78,
        SWAP = 80,
        ROT = 81,
        ROLL = 82,
        REVERSE3 = 83,
        REVERSE4 = 84,
        REVERSEN = 85,
        INITSSLOT = 86,
        INITSLOT = 87,
        LDSFLD0 = 88,
        LDSFLD1 = 89,
        LDSFLD2 = 90,
        LDSFLD3 = 91,
        LDSFLD4 = 92,
        LDSFLD5 = 93,
        LDSFLD6 = 94,
        LDSFLD = 95,
        STSFLD0 = 96,
        STSFLD1 = 97,
        STSFLD2 = 98,
        STSFLD3 = 99,
        STSFLD4 = 100,
        STSFLD5 = 101,
        STSFLD6 = 102,
        STSFLD = 103,
        LDLOC0 = 104,
        LDLOC1 = 105,
        LDLOC2 = 106,
        LDLOC3 = 107,
        LDLOC4 = 108,
        LDLOC5 = 109,
        LDLOC6 = 110,
        LDLOC = 111,
        STLOC0 = 112,
        STLOC1 = 113,
        STLOC2 = 114,
        STLOC3 = 115,
        STLOC4 = 116,
        STLOC5 = 117,
        STLOC6 = 118,
        STLOC = 119,
        LDARG0 = 120,
        LDARG1 = 121,
        LDARG2 = 122,
        LDARG3 = 123,
        LDARG4 = 124,
        LDARG5 = 125,
        LDARG6 = 126,
        LDARG = 127,
        STARG0 = 128,
        STARG1 = 129,
        STARG2 = 130,
        STARG3 = 131,
        STARG4 = 132,
        STARG5 = 133,
        STARG6 = 134,
        STARG = 135,
        NEWBUFFER = 136,
        MEMCPY = 137,
        CAT = 139,
        SUBSTR = 140,
        LEFT = 141,
        RIGHT = 142,
        INVERT = 144,
        AND = 145,
        OR = 146,
        XOR = 147,
        EQUAL = 151,
        NOTEQUAL = 152,
        SIGN = 153,
        ABS = 154,
        NEGATE = 155,
        INC = 156,
        DEC = 157,
        ADD = 158,
        SUB = 159,
        MUL = 160,
        DIV = 161,
        MOD = 162,
        SHL = 168,
        SHR = 169,
        NOT = 170,
        BOOLAND = 171,
        BOOLOR = 172,
        NZ = 177,
        NUMEQUAL = 179,
        NUMNOTEQUAL = 180,
        LT = 181,
        LE = 182,
        GT = 183,
        GE = 184,
        MIN = 185,
        MAX = 186,
        WITHIN = 187,
        PACK = 192,
        UNPACK = 193,
        NEWARRAY0 = 194,
        NEWARRAY = 195,
        NEWARRAY_T = 196,
        NEWSTRUCT0 = 197,
        NEWSTRUCT = 198,
        NEWMAP = 200,
        SIZE = 202,
        HASKEY = 203,
        KEYS = 204,
        VALUES = 205,
        PICKITEM = 206,
        APPEND = 207,
        SETITEM = 208,
        REVERSEITEMS = 209,
        REMOVE = 210,
        CLEARITEMS = 211,
        ISNULL = 216,
        ISTYPE = 217,
        CONVERT = 219
    }
}