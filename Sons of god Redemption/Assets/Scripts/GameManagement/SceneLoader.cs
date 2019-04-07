using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public SceneController sceneController;
	public void LoadLevel()
    {
        SaveData saveData;
        if (SaveSystem.LoadData() != null)
        {
            saveData = SaveSystem.LoadData();

            switch (saveData.level)
            {
                case 1:
                    sceneController.changeScene("LevelGame");
                    break;

                case 2:
                    sceneController.changeScene("LevelGame2");
                    break;

                case 3:
                    sceneController.changeScene("LevelGame3");
                    break;

                default:
                    sceneController.changeScene("Introduction");
                    break;
            }
        }
    }
}
