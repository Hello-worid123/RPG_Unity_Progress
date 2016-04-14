using UnityEngine;
using System.Collections;

public class GameUIManager : MonoBehaviour {
    private static GameUIManager _instance;
    public static GameUIManager Instance {
        get {
            return _instance;
        }
    }
    private enum UIShowState { 
    Init,
    SetUp,
    Finished
    
    }
    private bool _isstart; //是否启用协同
    private UIShowState _state;


    #region Protrait UI Target

    private UISprite _headsprite;
    private UIProgressBar _hpprogress;
    public UILabel _hpLabel;
    private UIProgressBar _mpprogress;
    public UILabel _mpLabel;

    #endregion


    #region ActionBar UI Target

    private UIButton _characterBtn;
    private UIButton _skillBtn;
    private UIButton _questBtn;
    private UIButton _systemBtn;
    private UIButton _inventoryBtn;

    #endregion

    #region CharacterWindow UI Target
    #endregion

    #region SkillWindow UI Target
    #endregion

    #region InventoryWindow UI Target
    #endregion

    #region QuestWindow UI Target
    #endregion

    #region SystemWindow UI Target
    #endregion

    #region EXPBar UI Target
    #endregion

    #region Main Window UI　Target

    private GameObject _CharacterWindow;
    
    private GameObject _SkillWindow;
  
    private GameObject _QuestWindow;
    private GameObject _InventoryWindow;
   
    private GameObject _SystemWindow;

    #endregion

    void Awake() {
        _instance = this;
    }

    // Use this for initialization
	void Start () {
        
        _state = UIShowState.Init;
        _isstart = true;
        StartCoroutine("UISHOWFSM");

 //       ProtraitUIShowUpdate();
	
	}



    public void ProtraitUIShowUpdate() {
   
        _headsprite.spriteName = "Warlock";
        _hpprogress.value = PlayerStatus.Instance.statusCal.Hp / PlayerStatus.Instance.hpMax;
        _mpprogress.value = PlayerStatus.Instance.statusCal.Mp / PlayerStatus.Instance.mpMax;
        _hpLabel.text = PlayerStatus.Instance.statusCal.Hp +"/"+PlayerStatus.Instance.hpMax;
        _mpLabel.text = PlayerStatus.Instance.statusCal.Mp + "/" + PlayerStatus.Instance.mpMax;
        
    }
    private IEnumerator UISHOWFSM() {
        while (_isstart) {
            switch (_state) { 
                case UIShowState.Init:
                    Init();
                    break;
                case UIShowState.SetUp:
                    SetUp();
                    break;
                case UIShowState.Finished:
                    Finished();
                    break;
            
            
            }

            yield return null;
        
        }

      
    }
    void Init() {
        ProtraitGetComponent();
        ActionBarGetComponent();
        MainWindowGetComponent();
        _state = UIShowState.SetUp;
    
    }

    void SetUp() {
        SetUpCheckProtrait();
        SetUpCheckActionBar();
        SetUpCheckMainWindow();
  
        _state = UIShowState.Finished;
    
    }
    void Finished()
    {
        _isstart = false;
        StopCoroutine("UISHOWFSM");
        SetUpOnClickEvent();
        

    }

    void SetUpCheckProtrait() {
        if (!_headsprite) return;
        if (!_hpprogress) return;
        if (!_mpprogress) return;
        if (!_hpLabel) return;
        if (!_mpLabel) return;
    
    }
    void SetUpCheckActionBar() {
        if (!_characterBtn) return;
        if (!_inventoryBtn) return;
        if (!_skillBtn) return;
        if (!_questBtn) return;
        if (!_systemBtn) return;
    
    }
    void SetUpCheckMainWindow() {
        if (!_CharacterWindow) return;
        if (!_SkillWindow) return;
        if (!_InventoryWindow) return;

    }

    void SetUpOnClickEvent() {
        EventDelegate ed = new EventDelegate(this, "OnShowCharacterWindow");
        _characterBtn.onClick.Add(ed);

        EventDelegate ed1 = new EventDelegate(this, "OnShowSkillWindow");
        _skillBtn.onClick.Add(ed1);

        EventDelegate ed2 = new EventDelegate(this, "OnShowInventoryWindow");
        _inventoryBtn.onClick.Add(ed2);


    }

    

    void ProtraitGetComponent() {
        _headsprite = transform.Find("Portrait/Head/headsprite").GetComponent<UISprite>();
        _hpprogress = transform.Find("Portrait/Hp").GetComponent<UIProgressBar>();
        _mpprogress = transform.Find("Portrait/Mp").GetComponent<UIProgressBar>();
        //   _hpLabel = transform.Find("Portrait/Hp/HPValueLabel").GetComponent<UILabel>();
        // _mpLabel = transform.Find("Portrait/Mp/MPValueLabel").GetComponent<UILabel>();
    
    }


    void ActionBarGetComponent() {
        _characterBtn = transform.Find("ActionBar/CharacterSprite").GetComponent<UIButton>();
        _questBtn = transform.Find("ActionBar/QuestSprite").GetComponent<UIButton>();
        _skillBtn = transform.Find("ActionBar/SkillSprite").GetComponent<UIButton>();
        _systemBtn = transform.Find("ActionBar/SystemSprite").GetComponent<UIButton>();
        _inventoryBtn = transform.Find("ActionBar/InventorySprite").GetComponent<UIButton>();
      
    }

    void MainWindowGetComponent()
    {
        _CharacterWindow = transform.Find("CharecterWindow").gameObject;
        _SkillWindow = transform.Find("SkillWindow").gameObject;
        
   //     _QuestWindow = transform.Find("").gameObject;
        _InventoryWindow = transform.Find("InventoryWindow").gameObject;
   //  _SystemWindow = transform.Find("").gameObject;
      
       
       
    }


    void OnShowCharacterWindow() {
        _CharacterWindow.SetActive(true);
    }
    void OnShowSkillWindow() {
        _SkillWindow.SetActive(true);
    }
    void OnShowInventoryWindow() {
        _InventoryWindow.SetActive(true);
    }
  

    
}
