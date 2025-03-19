using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public static class GameEvent
{
    public const string GAME_COMPLETE = "GAME_CONPLETE"; // дальше идём в MissionManager?
    public const string LEVEL_COMPLETE = "LEVEL_COMPLETE"; // дальше идём в MissionManager?
    public const string LEVEL_FAILED = "LEVEL_FAILED";
    public const string MONEY_UPDATED = "MONEY_UPDATED"; // дальше идём в UI Controller ( Broadcast - в PlayerManager)?
    public const string SHIELD_UPDATED = "SHIELD_UPDATED"; // далее в PlayerManager И SoldierEnemy
    public const string AMMO_UPDATED = "AMMO_UPDATED"; // в UI и player.
    public const string ALL_MONEY_COLLECTED = "ALL_MONEY_COLLECTED";

}
