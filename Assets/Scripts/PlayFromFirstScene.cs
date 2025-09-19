using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayFromFirstScene
{
    static SceneSetup[] s_PreviousAuthoringSceneSetup;

    [MenuItem("Examples/PlayFromFirstScene", isValidateFunction: true)]
    static bool CanExecute()
    {
        return SceneManager.sceneCountInBuildSettings > 0 && !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("Examples/PlayFromFirstScene")]
    static void Execute()
    {
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        s_PreviousAuthoringSceneSetup = EditorSceneManager.GetSceneManagerSetup();
        EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0), OpenSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }

    static PlayFromFirstScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.EnteredEditMode && s_PreviousAuthoringSceneSetup != null)
        {
            EditorSceneManager.RestoreSceneManagerSetup(s_PreviousAuthoringSceneSetup);
            s_PreviousAuthoringSceneSetup = null;
        }
    }
}
