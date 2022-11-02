using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI dieResultTMP;

    private void Awake()
    {
        Instance = this;
    }

    // Call this function to display the dice result in the UI
    public void DisplayDieResult(int dieResult)
    {
        dieResultTMP.text = dieResult.ToString();
    }
}