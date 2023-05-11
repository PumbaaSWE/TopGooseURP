using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleSystem : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    //[SerializeField] private TextMeshPro text;
    [SerializeField] private List<SubtitleLine> subtitles = new();

    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        text.text = string.Empty;
        StartSubtitles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSubtitles()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    public void NextSubtitle()
    {
        if(index < subtitles.Count - 1)
        {
            index++;
            if(subtitles[index].reset)
                text.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
    private IEnumerator TypeLine()
    {
        float wait = subtitles[index].textSpeed;
        for (int i = 0; i < subtitles[index].text.Length; i++)
        {
            text.text += subtitles[index].text[i];
            yield return new WaitForSeconds(wait);
        }
        yield return new WaitForSeconds(subtitles[index].nextSpeed);
        NextSubtitle();
    }

    [Serializable]
    public struct SubtitleLine
    {
        public string text;
        public float textSpeed;
        public float nextSpeed;
        public bool reset;

        public SubtitleLine(string text, float speed = 0.1f)
        {
            this.text = text;
            textSpeed = speed;
            nextSpeed = 0;
            reset = true;
        }


    }
}
