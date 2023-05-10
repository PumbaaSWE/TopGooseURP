using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutOfBoundsUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Image panel;

    [SerializeField]
    MapBoundary mapBoundary;

    int countDown;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "";
        mapBoundary.onOutOfBounds += StartCountDown;
        mapBoundary.onBackInBounds += StopCountDown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartCountDown()
    {
        countDown = mapBoundary.waitForSeconds;
        StartCoroutine(CountDown());
    }

    void StopCountDown()
    {
        var tempColor = panel.color;
        tempColor.a = 0;
        panel.color = tempColor;

        StopAllCoroutines();
        text.text = "";
    }

    IEnumerator CountDown()
    {
        while(countDown > 0)
        {
            var tempColor = panel.color;
            tempColor.a = (mapBoundary.waitForSeconds - countDown) / (float)mapBoundary.waitForSeconds / 5f;
            panel.color = tempColor;

            text.text = $"RETURN TO THE ISLAND!\n{countDown}";
            countDown--;
            yield return new WaitForSeconds(1);
        }

        text.text = "RETURN TO THE ISLAND!";
    }
}
