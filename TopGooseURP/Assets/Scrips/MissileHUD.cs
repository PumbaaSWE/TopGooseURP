using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileHUD : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    [SerializeField] private ManualFlightInput mouseFlight = null;
    [SerializeField] private MissileLauncher missileLauncher = null;

    [Header("HUD Elements")]
    [SerializeField] private RectTransform seekerView = null;

    private Camera playerCam = null;

    private void Awake()
    {
        if (mouseFlight == null)
            Debug.LogError(name + ": FlightHUD - Mouse Flight Controller not assigned!");

        playerCam = mouseFlight.GetComponentInChildren<Camera>();

        if (playerCam == null)
            Debug.LogError(name + ": FlightHUD - No camera found on assigned Mouse Flight Controller!");
    }

    private void FixedUpdate()
    {
        if (mouseFlight == null || playerCam == null)
            return;

        UpdateGraphics(mouseFlight);
    }

    private void UpdateGraphics(ManualFlightInput controller)
    {
        if (seekerView != null)
        {
            seekerView.position = playerCam.WorldToScreenPoint(missileLauncher.SeekerViewPositon());
            seekerView.gameObject.SetActive(seekerView.position.z > 1f);
        }

    }

    public void SetReferenceMouseFlight(ManualFlightInput controller)
    {
        mouseFlight = controller;
    }
}
