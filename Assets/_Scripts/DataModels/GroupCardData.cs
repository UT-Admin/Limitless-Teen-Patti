using System;

namespace TP.Rummy
{
    [Serializable]
    public struct GroupCardData
    {
        public int groupId;
        public CardsCombination cardsCombination;
        public CardData[] cardDataArray;
    }

}