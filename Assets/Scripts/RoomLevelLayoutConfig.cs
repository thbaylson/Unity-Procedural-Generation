using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room Level Layout", menuName = "ScriptableObjects/Procedural Generation/Room Level Layout Config", order = 1)]
public class RoomLevelLayoutConfig : ScriptableObject
{
    [Header("Level Layout Settings")]
    // Width and length should be powers of 2, otherwise behavior may be undefined.
    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelLength = 64;
    [SerializeField] int levelPadding = 1;
    [SerializeField] int roomMargin = 1;
    [SerializeField] int roomCountMin = 3;
    [SerializeField] int roomCountMax = 5;

    [Header("Room Settings")]
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;
    [SerializeField] int doorwayDistanceFromCorner = 1;

    [Header("Hallway Settings")]
    [SerializeField] int hallwayWidthMin = 2;
    [SerializeField] int hallwayWidthMax = 3;

    public int LevelWidth { get => levelWidth; }
    public int LevelLength { get => levelLength; }
    public int LevelPadding { get => levelPadding; }
    public int RoomMargin { get => roomMargin; }
    public int RoomCountMin { get => roomCountMin; }
    public int RoomCountMax { get => roomCountMax; }
    public int RoomWidthMin { get => roomWidthMin; }
    public int RoomWidthMax { get => roomWidthMax; }
    public int RoomLengthMin { get => roomLengthMin; }
    public int RoomLengthMax { get => roomLengthMax; }
    public int DoorwayDistanceFromCorner { get => doorwayDistanceFromCorner; }
    public int HallwayWidthMin { get => hallwayWidthMin; }
    public int HallwayWidthMax { get => hallwayWidthMax; }

}