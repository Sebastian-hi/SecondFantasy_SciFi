using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public static class GameEvent
{
    public const string GAME_COMPLETE = "GAME_CONPLETE"; // ������ ��� � MissionManager?
    public const string LEVEL_COMPLETE = "LEVEL_COMPLETE"; // ������ ��� � MissionManager?
    public const string LEVEL_FAILED = "LEVEL_FAILED";
    public const string MONEY_UPDATED = "MONEY_UPDATED"; // ������ ��� � UI Controller ( Broadcast - � PlayerManager)?
    public const string SHIELD_UPDATED = "SHIELD_UPDATED"; // ����� � PlayerManager � SoldierEnemy
    public const string AMMO_UPDATED = "AMMO_UPDATED"; // � UI � player.
    public const string ALL_MONEY_COLLECTED = "ALL_MONEY_COLLECTED";

}
