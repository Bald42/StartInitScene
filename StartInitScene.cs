using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEditor;

[InitializeOnLoadAttribute]
public static class StartInitScene
{
    private static bool isClearDebug = false;
    private const string IS_ACTIVE = "IS_ACTIVE_START_INIT_SCENE";

#if UNITY_EDITOR
    [MenuItem("Tools/StartInitScene/ChangeActive")]
    private static void ChangeActive()
    {
        EditorPrefs.SetBool(IS_ACTIVE, !EditorPrefs.GetBool(IS_ACTIVE));
    }
#endif

    static StartInitScene()
    {
#if UNITY_EDITOR
        //делаю через отрицание, тк по дефолту false а функция должна работать сразу
        if (!EditorPrefs.GetBool(IS_ACTIVE))
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChangedHandler;
            EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
            SceneManager.activeSceneChanged -= ActiveSceneChangedHandler;
            SceneManager.activeSceneChanged += ActiveSceneChangedHandler;
        }
#endif
    }

    private static void PlayModeStateChangedHandler(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
        {
            CheckScene();
        }
    }

    private static void ActiveSceneChangedHandler(Scene arg0, Scene arg1)
    {
        if (isClearDebug)
        {
            isClearDebug = false;
            ClearLog();
        }
    }

    private static void CheckScene()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            SceneManager.LoadScene(0);
            isClearDebug = true;
        }
    }

    public static void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}