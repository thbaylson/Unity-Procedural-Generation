using System;
using UnityEngine;

[Serializable]
public class RoomTemplate
{
    [SerializeField] string name;

    [SerializeField] int roomCount;
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;

    public int RoomCount { get => roomCount; }
    public int RoomWidthMin { get => roomWidthMin; }
    public int RoomWidthMax { get => roomWidthMax; }
    public int RoomLengthMin { get => roomLengthMin; }
    public int RoomLengthMax { get => roomLengthMax; }
}
