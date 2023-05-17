using System;
using System.Collections;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{

    [SerializeField] private InGameMenu inGameMenu;
    [SerializeField] private Health playerHealth;
    [SerializeField] private ObjectiveEventManager objectiveEventManager;

    [SerializeField]
    private float waitAfterDeath = 12;
    [SerializeField]
    private float waitAfterWin = 5;


    private void Start()
    {
        playerHealth.OnDead += OnPlayerDeath;
        objectiveEventManager.AllPrimaryCompleted.AddListener(OnPlayerWin);
    }

    private void OnPlayerDeath()
    {
        //StopAllCoroutines();
        StartCoroutine(ShowEndScreen(waitAfterDeath));
    }

    private IEnumerator ShowEndScreen(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (inGameMenu != null)
        {
            inGameMenu.EndScreen();
        }
    }

    private void OnPlayerWin(bool win)
    {
        //StopAllCoroutines();
        StartCoroutine(ShowEndScreen(waitAfterWin));
    }
}
