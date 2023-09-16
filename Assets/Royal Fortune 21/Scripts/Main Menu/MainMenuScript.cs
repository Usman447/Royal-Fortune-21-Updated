using RoyalFortune21.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace RoyalFortune21
{
    public class MainMenuScript : MonoBehaviour
    {
        [SerializeField] GameObject exitPanel, ProfilePanel;
        [SerializeField] Text ProfileName, ProfileAge;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            ProfileName.text = PlayerPrefs.GetString("ProfileName");
            ProfileAge.text = PlayerPrefs.GetString("ProfileAge");
        }

        public void ProfileNameInput(string name)
        {
            PlayerPrefs.SetString("ProfileName", name);
            ProfileName.text = PlayerPrefs.GetString("ProfileName");
        }

        public void ProfileAgeInput(string age)
        {
            PlayerPrefs.SetString("ProfileAge", age);
            ProfileAge.text = PlayerPrefs.GetString("ProfileAge");
        }

        public void Exitbtn()
        {
            Application.Quit();
        }

        public void Registerbtn()
        {
            ProfilePanel.SetActive(false);
            PlayerPrefs.SetInt("firstTImeProfile", 1);
        }
    }
}
    