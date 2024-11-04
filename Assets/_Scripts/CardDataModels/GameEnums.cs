namespace TP
{
    public enum GameType
    {
        ONLINE = 0,
        PRIVATE,
    }
    public enum CashType
    {
        CASH = 0,
        SILVER = 1,
        NONE = 2,
    }
    public enum CoinType
    {
        GOLD = 0,
        SILVER = 1,
    }

    public enum GameMode
    {
        NONE = 0,
        NOLIMITS = 3
        /* CLASSIC = 1,
         JOKER = 2,
         NOLIMITS = 3,
         ZANDU = 4,
         AK47 = 5,
         MUFLIS = 6,
         CROREPATI = 7,
         ROYAL = 8,
         HUKAM = 9,
         POTBLIND = 10,
         POKER = 11,        
         POINTRUMMY = 12,
         ANTHARBAHAR = 13,
         POOLRUMMY = 14,
         DEALRUMMY = 15,
         OMAHAPOKER = 16,
         NOLIMITPOKER = 17*/

    }

    public enum SubGameMode
    {
        GENERAL = 0,
        DELUX = 1,
        TOURNAMENT =2,
        PRIVATE = 3,
    }

    public enum GameTable
    {
        NONE = 0,
        ROOKIE,
        PRIMARY,
        MIDDLE,
        HIGH,
    }

    public enum CardRankDesignEnum
    {
        NONE = 0,
        ACE = 14,
        JACK = 11,
        QUEEN = 12,
        KINGS = 13,
        JOKER = 15
    }

    public enum CardSuit
    {
        None = 0,
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
        Spade = 4,
        Joker = 5
    }

    public enum CardsCombination
    {
        Empty = 0,
        HighCard = 1,
        Pair = 3,
        Flush = 5,
        Sequence = 7,
        StraightFlush = 9,
        Trail = 10
    }
    public enum RPCActions
    {
        Remove_Friends = 0,

        others = 9
    }

}