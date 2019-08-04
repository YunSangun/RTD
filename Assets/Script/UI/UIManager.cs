using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager inst;
    public static UIManager Inst
    {
        get
        {
            return UIManager.inst;
        }
    }
    public Text LifeText;
    public Text RoundText;
    public Text GoldText;
    public Button AddTowerButton;
    public Button FastButton;
    public Button StartButton;
    public Button OptionButton;

    private void Awake()
    {
        UIManager.inst = this;
    }
}
