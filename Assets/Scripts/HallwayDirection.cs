using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HallwayDirection { Undefined, Top, Right, Bottom, Left };

public static class  HallwayDirectionExtension
{
    private static readonly Dictionary<HallwayDirection, Color> DirectionToColorMap = new Dictionary<HallwayDirection, Color>
    {
        { HallwayDirection.Top, Color.blue },
        { HallwayDirection.Left, TextureBasedLevel.yellow },
        { HallwayDirection.Bottom, Color.green },
        { HallwayDirection.Right, Color.magenta }
    };

    public static Color GetColor(this HallwayDirection direction)
    {
        return DirectionToColorMap.TryGetValue(direction, out Color color) ? color : Color.red;
    }

    public static Dictionary<Color, HallwayDirection> GetColorToDirectionMap()
    {
        return DirectionToColorMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }

    public static HallwayDirection GetOppositeDirection(this HallwayDirection direction)
    {
        // Copilot suggested a switch-case instead. I think I liked that implementation better than this one.
        Dictionary<HallwayDirection, HallwayDirection> oppositeDirectionMap = new Dictionary<HallwayDirection, HallwayDirection>
        {
            { HallwayDirection.Top, HallwayDirection.Bottom },
            { HallwayDirection.Left, HallwayDirection.Right },
            { HallwayDirection.Bottom, HallwayDirection.Top },
            { HallwayDirection.Right, HallwayDirection.Left }
        };

        return oppositeDirectionMap.TryGetValue(direction, out HallwayDirection oppositeDirection) ? oppositeDirection : HallwayDirection.Undefined;
    }
}
