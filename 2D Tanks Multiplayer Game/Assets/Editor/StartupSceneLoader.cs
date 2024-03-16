using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartupSceneLoader
{
    static StartupSceneLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChange;
    }

    public static void OnPlayStateChange(PlayModeStateChange mode)
    {
        if (mode == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (mode == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorSceneManager.GetActiveScene().name != SceneController.LOGIN_SCENE)
            {
                EditorSceneManager.LoadScene(SceneController.LOGIN_SCENE);
            }
        }
    }
}
