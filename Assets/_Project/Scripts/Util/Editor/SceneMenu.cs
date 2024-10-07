#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneMenu : MonoBehaviour {

    static void CheckScene()
    {
        if(EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
    }

    [MenuItem("PxP/Scene/-PlayGame %#&p")]
    static public void PlayGame()
    {
        MoveToSplashScene();
        EditorApplication.isPlaying = true;
    }

    [MenuItem("PxP/Scene/Splash %#&1")]
    static public void MoveToSplashScene()
    {
        CheckScene();
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/Splash.unity");
    }

    [MenuItem("PxP/Scene/Game %#&2")]
    static public void MoveToGameScene()
    {
        CheckScene();
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/Game.unity");
    }

    [MenuItem("PxP/Scene/HeroEdit %#&3")]
    static public void MoveToUnitEditScene()
    {
        CheckScene();
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/HeroEdit.unity");
    }

    [MenuItem("PxP/Scene/VFX %#&4")]
    static public void MoveToVFXScene()
    {
        CheckScene();
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/VFX.unity");
    }

}
#endif
