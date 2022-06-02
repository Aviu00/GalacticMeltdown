using System;
using System.Xml;

namespace GalacticMeltdown.Data;

public static class ActorStateChangerDataExtractor
{
    public static ActorStateChangerData ParseStateChanger(XmlNode node)
    {
        StateChangerType? type = null;
        int power = 0;
        int duration = 0;
        foreach (XmlNode locNode in node.ChildNodes)
        {
            switch (locNode.Name)
            {
                case "Type":
                    type = Enum.Parse<StateChangerType>(locNode.InnerText);
                    break;
                case "Power":
                    power = Convert.ToInt32(locNode.InnerText);
                    break;
                case "Duration":
                    duration = Convert.ToInt32(locNode.InnerText);
                    break;
            }
        }

        return new ActorStateChangerData(type.Value, power, duration);
    }

}

public record ActorStateChangerData(StateChangerType Type, int Power, int Duration);
