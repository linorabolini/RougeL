using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.2f;							//Delay between each Player turn.
		public int playerFoodPoints = 100;						//Starting value for Player food points.
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		
		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		public int level = 1;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private List<Player> players;
		private List<Unit> units;
		[HideInInspector] public bool levelFinished = false;
		
		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null)
				
				//if not, set instance to this
				instance = this;
			
			//If instance already exists and it's not this:
			else if (instance != this)
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();

			players = new List<Player>();

			units = new List<Unit> ();
			
			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
		}
		
		//Initializes the game for each level.
		private IEnumerator InitGame()
		{
			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");
			
			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
			//Set the text of levelText to the string "Day" and append the current level number.
			levelText.text = "Day " + level;
			
			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);
			
			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();

			players.Clear();

			units.Clear ();

			levelFinished = false;
			
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);

			yield return new WaitForSeconds (levelStartDelay);

			HideLevelImage ();
		}

		void OnLevelWasLoaded(int index)
		{
			StartCoroutine (GameLoop ());
		}
		
		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
		}
		
		//Update is called every frame.
		private IEnumerator GameLoop()
		{
			yield return StartCoroutine (InitGame ());

			while(!levelFinished && playerFoodPoints > 0)
			{
				for (int i = 0; i < units.Count; i++) {
					if (levelFinished || playerFoodPoints <= 0)
						break;

					if (units [i].isActiveAndEnabled) {
						yield return StartCoroutine (units [i].DoTurn ());
						yield return new WaitForSeconds(turnDelay);
					}
				}
			}

			if (levelFinished)
			{
				OnLevelFinished ();
			} else {
				yield return StartCoroutine (GameOver ());
			}

			yield return null;
		}

		public void OnLevelFinished()
		{
			level++;
			SceneManager.LoadScene ("Main");
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
			units.Add (script);
		}

		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddPlayerToList(Player script)
		{
			//Add Enemy to List enemies.
			players.Add(script);
			units.Add (script);
		}
		
		//GameOver is called when the player reaches 0 food points
		private IEnumerator GameOver()
		{
			//Set levelText to display number of levels passed and game over message
			levelText.text = "After " + level + " days, you starved.";
			
			//Enable black background image gameObject.
			levelImage.SetActive(true);

			yield return new WaitForSeconds (levelStartDelay);
		}
	}
}

