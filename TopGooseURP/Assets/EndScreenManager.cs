using System;
using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{

    [SerializeField] private InGameMenu inGameMenu;
    [SerializeField] private Health playerHealth;
    [SerializeField] private ObjectiveEventManager objectiveEventManager;
    

    [SerializeField]
    private float waitAfterDeath = 12;
    [SerializeField]
    private float waitAfterRagdoll = 3;
    [SerializeField]
    private float waitAfterWin = 5;


    [Space]
    [SerializeField] private SpeedRunTimie speedRunTimie;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timeHeader;

    private void Start()
    {
        playerHealth.OnDead += OnPlayerDeath;
        playerHealth.gameObject.GetComponent<RagdollHandler>().onRagdollEnable += OnPlayerRagdoll;
        objectiveEventManager.AllPrimaryCompleted.AddListener(OnPlayerWin);

        timeHeader.enabled = false;
        timeText.enabled = false;
    }

    private void OnPlayerDeath()
    {
        //StopAllCoroutines();
        StartCoroutine(ShowEndScreen(waitAfterDeath));
    }

    private void OnPlayerRagdoll()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEndScreen(waitAfterRagdoll));
    }

    private IEnumerator ShowEndScreen(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (inGameMenu != null)
        {
            inGameMenu.EndScreen();
        }
        objectiveEventManager.OnEnable();
    }

    private void OnPlayerWin(bool win)
    {
        //StopAllCoroutines();
        StartCoroutine(ShowEndScreen(waitAfterWin));
        timeText.text = TimeSpan.FromSeconds(speedRunTimie.StopTimeer()).ToString(@"mm\:ss");
        timeHeader.enabled = true;
        timeText.enabled = true;

    }

}
