using System;

[Serializable]
public class SWEvent
{
    public string playerID;
    public string functionName;
    public double reqTime;
    public Action eventAction;
    public string GetEventID()
    {
        return playerID + reqTime;
    }
}
