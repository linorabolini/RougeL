using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

	// Use this for initialization
	void Awake() {
		SceneManager.LoadScene ("Main");
	}
}
