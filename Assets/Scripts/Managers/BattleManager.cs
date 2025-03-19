using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public bool battleStart = false;

    public bool haveUltraPower = false;

    public void Startup()
    {
        Debug.Log("BattleManager Starting");

        status = ManagerStatus.Started;
    }
}
