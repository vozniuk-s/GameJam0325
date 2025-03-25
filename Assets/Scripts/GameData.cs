using UnityEngine;
using System.Collections.Generic;

public static class GameData
{
    public static int ResourcesCollected { get; set; } = 0;
    public static int AnimalsRevived { get; set; } = 0;
    public static bool GameEnded { get; set; } = false;
}
