using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public void LoadLevel(string sceneName){
		Debug.Log ("New Level load: " + sceneName);
		SceneManager.LoadScene(sceneName);
	}

	public void LoadLevel(string sceneName, float time)
	{
		StartCoroutine(LoadLevelAfter(sceneName, time));
	}

	public IEnumerator LoadLevelAfter(string sceneName, float time)
	{
		yield return new WaitForSeconds(time);

		Debug.Log("New Level load: " + sceneName);
		SceneManager.LoadScene(sceneName);
	}

	public void QuitRequest(){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}

}
