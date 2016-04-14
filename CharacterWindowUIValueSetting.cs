using UnityEngine;
using System.Collections;

public class CharacterWindowUIValueSetting : MonoBehaviour {
    private static CharacterWindowUIValueSetting _instance;
    public static CharacterWindowUIValueSetting Instance {
        get {
            return _instance;
        }
    }

    //CharacterWindow UI 
    public UILabel _lvLabel;
    public UILabel _hpLabel;
    public UILabel _mpLabel;
    public UILabel _atkLabel;
    public UILabel _defLabel;
    public UILabel _spdLabel;
    public UILabel _hitRateLabel;
    public UILabel _missRateLabel;
    public UILabel _criRateLabel;
    public UILabel _atkRangeLabel;
    public UILabel _atkSpdLabel;
    public UILabel _moveSpdLabel;

    private UIButton _CharacterWindowCloseBtn;

    private const string CHARACTERUIPATH = "Background/AnotherBG/Stats/Pages/Page 0/Table/";
    private const string VALUELABELPATH = "/Value Label";

    #region setter and getter
   


    #endregion


    void Awake() {
        _instance = this;
        
        _CharacterWindowCloseBtn = transform.Find("Background/Close Button").GetComponent<UIButton>();
    
    }

    // Use this for initialization
	void Start () {

     //   InitGetLabel();
        EventDelegate ed3 = new EventDelegate(this, "OnHideCharacterWindow");
        _CharacterWindowCloseBtn.onClick.Add(ed3);
       



	}

    void InitGetLabel() {
        //_lvLabel = transform.Find("Background/Header/LvLabel").GetComponent<UILabel>();
        //_hpLabel = transform.Find(CHARACTERUIPATH + "HP Column"+VALUELABELPATH).GetComponent<UILabel>();
        //_mpLabel = transform.Find(CHARACTERUIPATH + "MP Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_defLabel = transform.Find(CHARACTERUIPATH + "Defence Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_spdLabel = transform.Find(CHARACTERUIPATH + "Speed Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_hitRateLabel = transform.Find(CHARACTERUIPATH + "HP Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_missRateLabel = transform.Find(CHARACTERUIPATH + "MissRate Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_criRateLabel = transform.Find(CHARACTERUIPATH + "CriticalRate Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_atkRangeLabel = transform.Find(CHARACTERUIPATH + "AttackRange Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_atkSpdLabel = transform.Find(CHARACTERUIPATH + "AttackSpeed Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_moveSpdLabel = transform.Find(CHARACTERUIPATH + "MoveSpeed Column" + VALUELABELPATH).GetComponent<UILabel>();
        //_atkLabel = transform.Find(CHARACTERUIPATH + "Attack Column" + VALUELABELPATH).GetComponent<UILabel>();
    
    }

    public void UpdateShowCharacterUI(PlayerStatus.SumAttribute att,PlayerStatus.Attribute at) {
        _lvLabel.text = "                     LV " + at.Lv;
        _hpLabel.text = att.Hp.ToString();
        _mpLabel.text = att.Mp.ToString();
        _defLabel.text = att.Def.ToString();
        _spdLabel.text = att.Spd.ToString();
        _hitRateLabel.text = att.HitRate.ToString();
        _missRateLabel.text = att.MissRate.ToString();
        _criRateLabel.text = att.CriticalRate.ToString();
        _atkRangeLabel.text = att.AtkRange.ToString();
        _atkSpdLabel.text = att.AtkSpd.ToString();
        _moveSpdLabel.text = att.Movespd.ToString();
        _atkLabel.text = att.Atk.ToString();
         
    }

	// Update is called once per frame
	void Update () {
	
	}
    public void Show() {
        this.gameObject.SetActive(true);
    }
    public void OnHideCharacterWindow()
    {
       
        this.gameObject.SetActive(false);
        
    }
}
