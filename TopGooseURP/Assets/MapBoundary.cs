using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MapBoundary;

[RequireComponent(typeof(TriggerEventUnit))]
public class MapBoundary : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    public int waitForSeconds;

    [SerializeField]
    int cooldown;

    List<Transform> boats;

    bool triggered;

    public delegate void OnOutOfBounds();
    public OnOutOfBounds onOutOfBounds;

    public delegate void OnBackInBounds();
    public OnBackInBounds onBackInBounds;

    private void Start()
    {
        boats = new List<Transform>();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform child = transform.parent.GetChild(i);

            if (!child.TryGetComponent<BoatController>(out var b))
                continue;

            boats.Add(child);
            b.target = player.transform;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject != player) return;

        //This is because the player consists of multiple colliders
        if(triggered) return;
        triggered = true;

        onOutOfBounds.Invoke();
        StartCoroutine(CountThenFire());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player) return;

        //Basically reset the system
        if (triggered)
        {
            triggered = false;

            //Stop CountThenFire()
            onBackInBounds.Invoke();
            StopAllCoroutines();
            //Debug.Log($"You're safe again :)");
        }
    }

    IEnumerator CountThenFire()
    {
        //Here we should send out a warning of some sort

        //Debug.Log($"Warning!! Return to the map within {waitForSeconds} seconds");
        yield return new WaitForSeconds(waitForSeconds);
        //Debug.Log($"{waitForSeconds} seconds have passed <!> PREPARE FOR DOOM");

        //Will keep telling the closest boat to fire at player in intervals until player enters the playable area.
        while (triggered)
        {
            Transform closestBoat = boats[0];
            for (int i = 1; i < boats.Count; i++)
            {
                if (Vector3.Distance(boats[i].position, player.transform.position) < Vector3.Distance(closestBoat.position, player.transform.position))
                    closestBoat = boats[i];
            }

            closestBoat.GetComponent<BoatMissileLauncher>().Fire(player.transform);

            yield return new WaitForSeconds(cooldown);
        }
    }
}
