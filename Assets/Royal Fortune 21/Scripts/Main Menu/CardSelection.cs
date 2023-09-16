using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoyalFortune21
{
    public class CardSelection : MonoBehaviour
    {
        [SerializeField] GameObject LoadingPanel;
        UIScreenScript _uiScreen;

        private void Awake()
        {
            _uiScreen = FindObjectOfType<UIScreenScript>();
        }

        public void OnClickSelectChessCards()
        {
            PlayerPrefs.SetString("SelectedCardType", "ChessCards");
            PlayGame();
        }

        public void OnClickSelectTraditionalCards()
        {
            PlayerPrefs.SetString("SelectedCardType", "TraditionalCards");
            PlayGame();
        }

        public void ClosePanel()
        {
            this.gameObject.SetActive(false);
        }


        void PlayGame()
        {
            _uiScreen.DeductAmount();
            StartCoroutine(LoadYourAsyncScene());
        }

        IEnumerator LoadYourAsyncScene()
        {
            EnableLoadingPanel();
            yield return new WaitForSeconds(2);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            DisableLoadingPanel();
        }



        void EnableLoadingPanel()
        {
            LoadingPanel.SetActive(true);
            LoadingPanel.GetComponent<Animator>().enabled = true;
        }


        void DisableLoadingPanel()
        {
            LoadingPanel.GetComponent<Animator>().enabled = false;
            LoadingPanel.SetActive(false);
        }
    }
}
    