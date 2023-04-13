using UnityEngine;

public class MissileHUD : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    [SerializeField] private MissileLauncher missileLauncher = null;

    [Header("HUD Elements")]
    [SerializeField] private RectTransform seekerView = null;
    [SerializeField] private RectTransform lockedView = null;

    private Camera playerCam = null;

    private void Awake()
    {
        playerCam = Camera.main;

        if (playerCam == null)
            Debug.LogWarning(name + ": MissileHUD - No camera found on Camera.main!");
        missileLauncher = GetComponent<MissileLauncher>();
        if (missileLauncher == null)
            Debug.LogError(name + ": MissileHUD - No Missile Launcher found!");
        missileLauncher.OnActivationChange += Enable;
    }

    private void OnDestroy()
    {
        missileLauncher.OnActivationChange -= Enable;
    }


    private void Enable(bool enable)
    {
        enabled = enable;
        lockedView.gameObject.SetActive(false); //aways false so UpdateGraphics() renders the in the correct spot!
        seekerView.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        UpdateGraphics();
    }

    private void UpdateGraphics()
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
}
