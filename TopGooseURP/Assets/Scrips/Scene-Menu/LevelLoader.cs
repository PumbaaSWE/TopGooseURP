using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private Button playButton;

    private bool pressedPlay = false;
    public bool PressedPlay { get { return pressedPlay; } set {  pressedPlay = value; } }

    public void LoadLevel(int sceneIndex)
    {
        continueText.enabled = false;
        playButton.enabled = false;
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            slider.value = progress;
            progressText.text = progress * 100f + "%";

            if (operation.progress >= 0.9f)
            {                
                playButton.enabled = true;
                continueText.enabled = true;
                if (Input.GetKeyDown(KeyCode.Space) || pressedPlay)
                {
                    operation.allowSceneActivation = true;
                    LockMouse();
                    pressedPlay = false;
                }
            }

            yield return null;
        }
    }

    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
