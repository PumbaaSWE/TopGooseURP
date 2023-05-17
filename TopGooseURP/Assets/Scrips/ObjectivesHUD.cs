using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesHUD : MonoBehaviour
{

    [SerializeField] private ObjectiveEventManager eventManager;
    [SerializeField] private float displayTime = 3;
    [SerializeField] private float fadeTime = 1;

    //[SerializeField] private float fadeRate = 1;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleBox;
    [SerializeField] private TextMeshProUGUI decriptionBox;

    private readonly Queue<Objective> qObjectives = new();

    private float displayTimer = 0;
    private float fadeTimer = 0;
    private float alpha = 1.0f;

    private Sprite defaultSprite;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if(eventManager == null)
        {
            Debug.LogWarning("ObjectivesHUD - Missing ObjectiveEventManager");
            gameObject.SetActive(false);
            return;
        }

        if(titleBox == null)
        {
            gameObject.SetActive(false);
            Debug.LogWarning("ObjectivesHUD - Missing TextMeshProUGUI titleBox");
            gameObject.SetActive(false);
            return;
        }

        if (decriptionBox == null)
        {
            gameObject.SetActive(false);
            Debug.LogWarning("ObjectivesHUD - Missing TextMeshProUGUI decriptionBox");
            gameObject.SetActive(false);
            return;
        }

        titleBox.gameObject.SetActive(false);
        decriptionBox.gameObject.SetActive(false);

        iconImage.gameObject.SetActive(false);
        defaultSprite = iconImage.sprite;

        audioSource = GetComponent<AudioSource>();

        eventManager.ObjectiveCompleted.AddListener(QueueObjective);
    }

    // Update is called once per frame
    void Update()
    {
        displayTimer += Time.deltaTime;
        fadeTimer += Time.deltaTime;

        if (displayTimer <= displayTime)
        {
            //just displaying...
            fadeTimer = 0;
        }
        else if(fadeTimer <= fadeTime)
        {
            alpha = 1 - fadeTimer / fadeTime;
            titleBox.alpha = alpha;
            decriptionBox.alpha = alpha;
            Color col = iconImage.color;
            col.a = alpha;
            iconImage.color = col;
        }
        else if(qObjectives.Count > 0)
        {
            DisplayObjective(qObjectives.Dequeue());
        }
        else
        {
            iconImage.gameObject.SetActive(false);
            titleBox.gameObject.SetActive(false);
            decriptionBox.gameObject.SetActive(false);
            enabled = false;
        }
    }

    //IEnumerator FadeIn()
    //{
    //    targetAlpha = 1.0f;
    //    Color curColor = image.color;
    //    while (Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
    //    {
    //        Debug.Log(image.material.color.a);
    //        curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
    //        image.color = curColor;
    //        yield return null;
    //    }
    //}

    //private IEnumerator FadeOut()
    //{
    //    float targetAlpha = 0.0f;
    //    Color curColor = iconImage.tintColor;
    //    while (Mathf.Abs(curColor.a - targetAlpha) > 0.0001f)
    //    {
    //        //Debug.Log(iconImage.material.color.a);
    //        curColor.a = Mathf.Lerp(curColor.a, targetAlpha, fadeRate * Time.deltaTime);
    //        iconImage.tintColor = curColor;
    //        titleBox.alpha = curColor.a;
    //        decriptionBox.alpha = curColor.a;
    //        yield return null;
    //    }
    //}

    private void DisplayObjective(Objective objective)
    {
        titleBox.text = objective.Title;
        decriptionBox.text = objective.Description;
        if(objective.Sprite != null)
        {
            iconImage.sprite = objective.Sprite;
        }
        else
        {
            iconImage.sprite = defaultSprite;
        }
        iconImage.color = Color.white;
        iconImage.gameObject.SetActive(true);

        titleBox.alpha = 1.0f;
        decriptionBox.alpha = 1.0f;

        fadeTimer = displayTimer = 0;

        titleBox.gameObject.SetActive(true);
        decriptionBox.gameObject.SetActive(true);

        audioSource.Play();
        //enabled = true;
    }

    private void QueueObjective(Objective objective)
    {
        if(!enabled)
        {
            DisplayObjective(objective);
            enabled = true;
        }
        else
        {
            qObjectives.Enqueue(objective);
        }
    }
}
