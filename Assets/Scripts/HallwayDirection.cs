using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HallwayDirection { Undefined, Top, Right, Bottom, Left };

public static class  HallwayDirectionExtension
{
    private static readonly Dictionary<HallwayDirection, Color> DirectionToColorMap = new Dictionary<HallwayDirection, Color>
    {
        { HallwayDirection.Top, Color.blue },
        { HallwayDirection.Left, Color.yellow },
        { HallwayDirection.Bottom, Color.green },
        { HallwayDirection.Right, Color.magenta }
    };

    public static Color GetColor(this HallwayDirection direction)
    {
        return DirectionToColorMap.TryGetValue(direction, out Color color) ? color : Color.red;
    }
}
