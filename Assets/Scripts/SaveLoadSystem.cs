using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public GridManager gridManager;
    public GameManager gameManager;

    public void SaveGame()
    {
        SaveLoadManager.SaveGame(gridManager, gameManager);
    }
    public void LoadGame()
    {
        SaveLoadManager.LoadGame(gridManager, gameManager);
        gridManager.menuPanel.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Auto saved");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
}
