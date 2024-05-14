using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartupSceneLoader
{
    static StartupSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadStartupScene;
    }

    private static void LoadStartupScene(PlayModeStateChange change)
    {
        switch (change)
        {
            case PlayModeStateChange.ExitingEditMode:
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                break;
            case PlayModeStateChange.EnteredPlayMode:
                if (EditorSceneManager.GetActiveScene().buildIndex != 0)
                {
                    EditorSceneManager.LoadScene(0);
                }
                break;

        }
    }
}
