using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;


public class SceneShortcut
{

 [MenuItem("ArioSoren/Play From Preload Scene #&%p")]
	public static void PlayFromPreloadScene()
	{
		EditorSceneManager.OpenScene(EditorBuildSettings.scenes.First(a => a.path.Contains("Preload")).path);
		EditorApplication.EnterPlaymode();   
	}


 [MenuItem("ArioSoren/Play From Multiplayer Scene #&%p")]
	public static void PlayFromMultiplayerScene()
	{
		EditorSceneManager.OpenScene(EditorBuildSettings.scenes.First(a => a.path.Contains("Multiplayer")).path);
		EditorApplication.EnterPlaymode();   
	}


}
