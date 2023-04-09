using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebindScript : MonoBehaviour
{
    [SerializeField] private Button rollLeftButton;
    [SerializeField] private Button rollRightButton;
    [SerializeField] private Button pitchUpButton;
    [SerializeField] private Button pitchDownButton;
    [SerializeField] private Button yawLeftButton;
    [SerializeField] private Button yawRightButton;
    [SerializeField] private Button freelookButton;
    [SerializeField] private Button switchSecondButton;
    [SerializeField] private Button throttleUpButton;
    [SerializeField] private Button throttleDownButton;
    [SerializeField] private Button toggleFireButton;

    [SerializeField] private TextMeshProUGUI rollLeftText;
    [SerializeField] private TextMeshProUGUI rollRightText;
    [SerializeField] private TextMeshProUGUI pitchUpText;
    [SerializeField] private TextMeshProUGUI pitchDownText;
    [SerializeField] private TextMeshProUGUI yawLeftText;
    [SerializeField] private TextMeshProUGUI yawRightText;
    [SerializeField] private TextMeshProUGUI freelookText;
    [SerializeField] private TextMeshProUGUI switchSecondText;
    [SerializeField] private TextMeshProUGUI throttleUpText;
    [SerializeField] private TextMeshProUGUI throttleDownText;
    [SerializeField] private TextMeshProUGUI LMBText;
    [SerializeField] private TextMeshProUGUI RMBText;
}
