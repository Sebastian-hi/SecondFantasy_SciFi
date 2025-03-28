using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [NonSerialized] public bool UseUltraPower = false;

    public void Startup()
    {
        Debug.Log("BattleManager Starting");

        status = ManagerStatus.Started;
    }
}
