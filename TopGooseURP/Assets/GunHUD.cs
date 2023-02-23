using UnityEngine;
using UnityEngine.UI;

public class GunHUD : MonoBehaviour
{
    public Image heatImg;
    public Image flameImg;
    public Gun gun;
    public float heat;
    public float smoothing;
    public Gradient gradient;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!gun)return;

        if(heat < gun.Heat)
        {
            heat += smoothing * gun.FireRate * Time.deltaTime;
        }
        else
        {
            heat = gun.Heat;
        }

        
        heatImg.fillAmount = heat;
        heatImg.color = gradient.Evaluate(heat);
        flameImg.color = gradient.Evaluate(heat);
        
    }
}
