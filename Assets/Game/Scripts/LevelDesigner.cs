using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    public void NextLevel()
    {
        LevelManager.Instance.NextLevel();
        LevelManager.Instance.RestartLevel();
    }

    public void RestartLevel()
    {
        LevelManager.Instance.RestartLevel();
    }
}
 