using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static SceneController SceneController;

    private void Start()
    {
        DontDestroyOnLoad(this);
        SceneController = new SceneController();
    }
}
