using UnityEngine;

public class MissileHUD : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    [SerializeField] private ManualFlightInput mouseFlight = null;
    [SerializeField] private MissileLauncher missileLauncher = null;

    [Header("HUD Elements")]
    [SerializeField] private RectTransform seekerView = null;
    [SerializeField] private RectTransform lockedView = null;

    private Camera playerCam = null;

    private void Awake()
    {
        if (mouseFlight == null)
            Debug.LogError(name + ": MissileHUD - Mouse Flight Controller not assigned!");

        playerCam = mouseFlight.GetComponentInChildren<Camera>();

        if (playerCam == null)
            Debug.LogError(name + ": MissileHUD - No camera found on assigned Mouse Flight Controller!");
    }

    private void FixedUpdate()
    {
        if (mouseFlight == null || playerCam == null)
            return;

        UpdateGraphics(mouseFlight);
    }

    private void UpdateGraphics(ManualFlightInput controller)
    {

        if (!missileLauncher.NoMissile)
        {
            SeekerHead seekerHead = missileLauncher.GetSeekerHead();
            if (seekerHead.TargetLocked)
            {
                lockedView.position = playerCam.WorldToScreenPoint(seekerHead.TargetPosition);
                lockedView.gameObject.SetActive(true);
            }
            else
            {
                lockedView.gameObject.SetActive(false);
            }
            seekerView.position = playerCam.WorldToScreenPoint(seekerHead.SeekerViewPositon);



        }
        else
        {
            lockedView.gameObject.SetActive(false);
        }
        
        seekerView.gameObject.SetActive(seekerView.position.z > 1f && !missileLauncher.NoMissile);
        
    }

    public void SetReferenceMouseFlight(ManualFlightInput controller)
    {
        mouseFlight = controller;
    }
}
