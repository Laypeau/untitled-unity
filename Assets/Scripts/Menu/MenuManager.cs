using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MenuManagement
{
	public class MenuManager : MonoBehaviour
	{
		[HideInInspector] public GameObject PauseMenu;

		[HideInInspector] public GameObject SettingsSubmenu;
		[HideInInspector] public Toggle DebugToggle;

		[HideInInspector] public GameObject DebugOverlay;
		[HideInInspector] public Text SpringjointText;

		[HideInInspector] public TextMeshProUGUI ScoreText;

		[HideInInspector] public TextMeshProUGUI TimerText;
		private float timer = 0;

		private int curScore = 0;
		public int maxScore = 1;

		[HideInInspector] public GameObject Player;
		public bool IsPaused = false;
		public bool Pausable = true;
		public static KeyCode KeyCodePause = KeyCode.Escape;

		void Start()
		{
			PauseMenu = GameObject.Find("PauseMenu");

			SettingsSubmenu = GameObject.Find("SettingsMenu");
			DebugToggle = GameObject.Find("DebugToggle").GetComponent<Toggle>();

			DebugOverlay = GameObject.Find("DebugOverlay");
			SpringjointText = GameObject.Find("SpringjointText").GetComponent<Text>();

			ScoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();

			TimerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();

			Player = GameObject.Find("Player");
			Pausable = true;
			ResumeGame();
			UpdateScore(0);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCodePause) && Pausable)
			{
				if (IsPaused)
				{
					ResumeGame();
				}
				else
				{
					PauseGame();
				}
			}

			if (DebugToggle.isOn)
			{
				if (Player.TryGetComponent(out SpringJoint _))
				{
					SpringjointText.text = "Desired Distance: " + Player.GetComponent<PlayerControl.CharacterControl>().PlayerSpringJoint.maxDistance + "\n" + "Actual Distance: " + Vector3.Distance(Player.GetComponent<Rigidbody>().position, Player.GetComponent<PlayerControl.CharacterControl>().linepos[1]);
				}
				else
				{
					SpringjointText.text = "";
				}
			}
			else
			{
				SpringjointText.text = "";
			}

			UpdateTimerText();
		}

		/// <summary>
		/// Update the score
		/// </summary>
		/// <param name="_ScoreChange"> Change the score by this amount </param>
		public void UpdateScore(int _ScoreChange)
		{
			curScore += _ScoreChange;

			ScoreText.text = "Destroyed: \n" + curScore.ToString() + "/" + maxScore.ToString();

			if (curScore >= maxScore)
			{
				WinGame();
			}
		}

		/// <summary>
		/// Updates the timer with proper formatting
		/// </summary>
		public void UpdateTimerText()
		{
			if (!IsPaused)
			{
				timer += Time.deltaTime;
			}
			TimerText.text = FormatTime();
		}

		/// <summary>
		/// Behold the majesty of strings
		/// </summary>
		private string FormatTime()
		{
			int decimalpos = timer.ToString().IndexOf(".");
			string ms = timer.ToString().Substring(decimalpos + 1, 2);
			int todivide = int.Parse(timer.ToString().Substring(0, decimalpos));
			string sec = (todivide % 60).ToString();
			if (sec.Length == 1)
			{
				sec = "0" + sec;
			}
			string min = (todivide / 60).ToString();
			if (min.Length == 1)
			{
				min = "0" + min;
			}

			return min + ":" + sec + ":" + ms;
		}

		public void ResumeGame()
		{
			IsPaused = false;
			Time.timeScale = 1f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			SettingsSubmenu.SetActive(false);
			PauseMenu.SetActive(false);
		}

		public void PauseGame()
		{
			IsPaused = true;
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			PauseMenu.SetActive(true);
			SettingsSubmenu.SetActive(false);
		}

		public void WinGame()
		{
			if (PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + ".HighScore", 3600f) > timer)
			{
				PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + ".HighScore", (float)timer);

				//Pausable = false;
				StartCoroutine(SlowTime(0.2f, 5f));
			}
			else
			{
				//SceneManager.LoadScene();
				StartCoroutine(SlowTime(0.2f, 5f));

			}
		}
		
		/// <summary>
		/// Lerps Time.timeScale over a number of seconds
		/// </summary>
		IEnumerator SlowTime(float _finalTimeScale, float _seconds)
		{
			float _startTime = Time.unscaledTime;

			while (Time.unscaledTime < _startTime + _seconds)
			{
				Time.timeScale = _finalTimeScale * (((_startTime + _seconds) - Time.unscaledTime)/_seconds);

				yield return null;
			}

			Debug.Log("finished the thing");
		}
	}
}