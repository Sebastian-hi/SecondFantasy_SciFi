using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour, IGameManager
{
    public int CurLevel { get; private set; }

    public int MaxLevel { get; private set; }
    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        Debug.Log("MissionManager Starting");

        UpdateData(0, 1); // 0 это главное меню
        status = ManagerStatus.Started;
    }

    public void UpdateData(int curLevel,  int maxLevel)
    {
        this.CurLevel = curLevel;
        this.MaxLevel = maxLevel;
    }

    public void GoToNextLevel()
    {
        if (CurLevel < MaxLevel)
        {
            CurLevel++;
            string name = $"Level{CurLevel}";
            Debug.Log($"Loading: {name}");
            SceneManager.LoadScene(name);
        }
        else
        {
            Debug.Log("Final level!");
            Messenger.Broadcast(GameEvent.GAME_COMPLETE);
        }
    }

    public void ReachObjective()
    {
        Messenger.Broadcast(GameEvent.LEVEL_COMPLETE);
    }

    public void RestartCurrent()
    {
        string name = $"level{CurLevel}";
        Debug.Log($"Loading: {name}");
        SceneManager.LoadScene(name);
    }
}
