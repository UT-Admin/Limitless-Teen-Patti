namespace TP.Rummy
{

    public enum CardsCombination
    {
        NONE = 0,
        FIRST_LIFE,
        SECOND_LIFE,
        PURE_SEQUENCE,
        SEQUENCE,
        SET,
        FIRST_LIFE_REQUIRED,
        SECOND_LIFE_REQUIRED,
        JOKER_ONLY,
        INVALID
    }

    public enum ShiftDirectionEnum
    {
        NONE = 0,
        LEFT,
        RIGHT,        
    }


    public enum GrpInfoOptionEnum
    {
        HIDE_ALL = 0,
        SHOW_INFO_OPTION,
        SHOW_ADD_HERE,        
    }

    public enum GrpInfoShowEnum
    {
        HIDE_ALL = 0,
        SHOW_INFO_ONLY,
        SHOW_INFO_N_MOVE_OPTION,
        SHOW_ADD_HERE_ONLY,
        SHOW_ADD_HERE_N_INFO,
        SHOW_ALL,
    }

    public enum GrpMoveOptionEnum
    {
        SHOW_BOTH,
        SHOW_RIGHT,
        SHOW_LEFT,
        HIDE_ALL
    }

    public enum MeldOption
    {
        NONE = 0,
        //PURE7_IMPURE3_IMPURE3,
        //PURE6_IMPURE4_IMPURE3,
        //PURE5_IMPURE4_IMPURE4,
        //PURE4_PURE3_IMPURE6,
        PURE4_PURE3_IMPURE3_IMPURE3,
        PURE4_PURE3_IMPURE3_SET3,
        PURE4_PURE3_SET3_SET3,
        //PURE4_IMPURE6_SET3,
        PURE3_PURE3_IMPURE4_IMPURE3,
        PURE3_PURE3_IMPURE4_SET3,
        PURE3_PURE3_IMPURE3_SET4,
        PURE3_IMPURE4_IMPURE3_SET3,
        PURE3_IMPURE3_SET4_SET3,
    }
}