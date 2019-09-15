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
    public Text TowerTierText;
    public Text TowerTypeText;
    public Text TowerAttackText;
    public Text TowerDelayText;
    public Button AddTowerButton;
    public Button FastButton;
    public Button StartButton;
    public Button OptionButton;
    public GameObject TowerExplainPanel;
    public GameObject EmptyPanel;

    public Sprite StartSpr;
    public Sprite StopSpr;


    private TileController[,] Tiles = new TileController[9, 9];
    private TileController SelectedTile = null;
    private GameObject entry;
    private GameObject exit;
    private GameObject rangeMask;

    private void Awake()
    {
        UIManager.inst = this;
    }
    private void Start()
    {
        //버튼 이벤트 할당
        StartButton.onClick.AddListener(GameManager.Inst.RoundStart);
        AddTowerButton.onClick.AddListener(GameManager.Inst.BM.AddRandomTower);
        OptionButton.onClick.AddListener(GameManager.Inst.SetPause);
    }

    public void ChangeStartButtonImage(int state = -1)
    {
        if (state == -1) { return; }
        else if (state == 0)
        {
            StartButton.GetComponent<Image>().sprite = StopSpr;
        }
        else if (state == 1)
        {
            StartButton.GetComponent<Image>().sprite = StartSpr;
        }
    }
}
