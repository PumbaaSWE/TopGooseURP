using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIndicator : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private BombBay bombBay;
    [SerializeField] private MissileLauncher missileLauncher;
    [SerializeField] private Gun gun;
    [SerializeField] private GameInput gameInput;

    [Space]
    [Header("Bomb")]
    [SerializeField] private Image bombIcon;
    [SerializeField] private Image bombBar;
    [SerializeField] private TextMeshProUGUI bombText;

    [Space]
    [Header("Missile")]
    [SerializeField] private Image missileIcon;
    [SerializeField] private Image missileBar;
    [SerializeField] private TextMeshProUGUI missileText;

    [Space]
    [Header("Overheat")]
    [SerializeField] private Image heatIcon;
    [SerializeField] private Image heatBar;


    [SerializeField]private float smoothing;
    private float heat;
    private bool switchedWeapons = false;

    // Start is called before the first frame update
    void Start()
    {
        gameInput.SwitchWeaponAction += GameInput_SwitchWeaponAction;
        bombText.text = bombBay.Magazine + "/" + bombBay.MagazineSize;
        missileText.text = missileLauncher.MissileCount + "/" + missileLauncher.HardpointCount;

        missileBar.fillAmount = 0;
        bombBar.fillAmount = 0;
        heatBar.fillAmount = 0;

        missileIcon.color = Color.gray;
        missileBar.color = Color.gray;
        missileText.color = Color.gray;
    }

    private void GameInput_SwitchWeaponAction(object sender, System.EventArgs e)
    {
        switchedWeapons = !switchedWeapons;
        if (switchedWeapons)
        {
            MissileActive();
        }
        else
        {
            BombActive();
        }
    }

    // Update is called once per frame
    void Update()
    {
        BombHandel();
        MissileHandel();
        GunHandler();

    }

    private void GunHandler()
    {
        if (!gun) return;

        if (heat < gun.Heat)
        {
            heat += smoothing * gun.FireRate * Time.deltaTime;
        }
        else
        {
            heat = gun.Heat;
        }


        heatBar.fillAmount = heat;
        //heatImg.color = gradient.Evaluate(heat);
        //flameImg.color = gradient.Evaluate(heat);
    }
    private void MissileHandel()
    {
        missileText.text = missileLauncher.MissileCount + "/" + missileLauncher.HardpointCount;
        if (missileLauncher.MissileCount != 2)
        {
            missileBar.fillAmount += 1f / missileLauncher.ReloadTime * Time.deltaTime;
        }
        if (missileLauncher.MissileCount == 2)
        {
            missileBar.fillAmount = 0;
        }
    }
    private void BombHandel()
    {
        bombText.text = bombBay.Magazine + "/" + bombBay.MagazineSize;
        if (bombBay.Magazine == 0)
        {
            bombBar.fillAmount += 1f / bombBay.ReloadTime * Time.deltaTime;
        }
        if (bombBay.Magazine == 10)
        {
            bombBar.fillAmount = 0;
        }
    }

    #region Color change
    private void BombActive()
    {
        missileIcon.color = Color.gray;
        missileBar.color = Color.gray;
        missileText.color = Color.gray;

        bombIcon.color = Color.white;
        bombBar.color = Color.white;
        bombText.color = Color.white;
    }

    private void MissileActive()
    {
        missileIcon.color = Color.white;
        missileBar.color = Color.white;
        missileText.color = Color.white;

        bombIcon.color = Color.gray;
        bombBar.color = Color.gray;
        bombText.color = Color.gray;
    }
    #endregion
}
