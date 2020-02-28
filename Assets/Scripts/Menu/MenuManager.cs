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
        private int CurScore = 0;
        private readonly int MaxScore = 99;

        [HideInInspector] public GameObject Player;
        public bool IsPaused = false;
        public static KeyCode KeyCodePause = KeyCode.Escape;

        void Start()
        {
            PauseMenu = GameObject.Find("PauseMenu");

            SettingsSubmenu = GameObject.Find("SettingsMenu");
            DebugToggle = GameObject.Find("DebugToggle").GetComponent<Toggle>();

            DebugOverlay = GameObject.Find("DebugOverlay");
            SpringjointText = GameObject.Find("SpringjointText").GetComponent<Text>();

            ScoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();

            Player = GameObject.Find("Player");

            ResumeGame();
            UpdateScore(0);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCodePause))
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
        }

        /// <summary>
        /// Update the score
        /// </summary>
        /// <param name="_ScoreChange"> Change the score by this amount </param>
        public void UpdateScore(int _ScoreChange)
        {
            CurScore += _ScoreChange;

            ScoreText.text = "Destroyed: \n" + CurScore.ToString() + "/" + MaxScore.ToString();
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
    }
}