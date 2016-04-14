using UnityEngine;
using System.Collections;

public class SkillWindowUIManager : MonoBehaviour {

    private UISprite skillSprite;
    private UILabel skillName;
    private UILabel skillDes;
    private UITexture skillIconTexture;
    private Transform SkillUITransformParent;
    public GameObject skillUIGameObj;
    public UIGrid skillUIgird;

    private UIButton _SkillWindowCloseBtn;

    void Awake() {
        _SkillWindowCloseBtn = transform.Find("Background/Close Button").GetComponent<UIButton>();
    }
    void Start() {
        EventDelegate ed4 = new EventDelegate(this, "OnHideSkillWindow");
        _SkillWindowCloseBtn.onClick.Add(ed4);

        
        UpdateSkillWindowUI();
    }


    public void UpdateSkillWindowUI() {

        for (int i = 0; i < SkillManager._instance.passiveSkill.Count; i++)
        {

            FindUIComponet();
            skillIconTexture.mainTexture = SkillManager._instance.passiveSkill[i].skillIcon;
            skillDes.text = SkillManager._instance.passiveSkill[i].description;
            skillName.text = SkillManager._instance.passiveSkill[i].skillName;
            NGUITools.AddChild(skillUIgird.gameObject, skillUIGameObj);
            skillUIgird.AddChild(skillUIgird.transform);
        }
        for (int i = 0; i < SkillManager._instance.normalSKill.Count; i++) {
            FindUIComponet();
            skillIconTexture.mainTexture = SkillManager._instance.normalSKill[i].skillIcon;
            skillDes.text = SkillManager._instance.normalSKill[i].description;
            skillName.text = SkillManager._instance.normalSKill[i].skillName;
            NGUITools.AddChild(skillUIgird.gameObject, skillUIGameObj);
            skillUIgird.AddChild(skillUIgird.transform);
           
        }
        for (int i = 0; i < SkillManager._instance.activeSkillAttack.Count; i++)
            {
                FindUIComponet();
                skillIconTexture.mainTexture = SkillManager._instance.activeSkillAttack[i].skillIcon;
                skillDes.text = SkillManager._instance.activeSkillAttack[i].description;
                skillName.text = SkillManager._instance.activeSkillAttack[i].skillName;
                NGUITools.AddChild(skillUIgird.gameObject, skillUIGameObj);
                skillUIgird.AddChild(skillUIgird.transform);
            }
        for (int i = 0; i < SkillManager._instance.activeSkillSupport.Count;i++ ) {
            FindUIComponet();
            skillIconTexture.mainTexture = SkillManager._instance.activeSkillSupport[i].skillIcon;
            skillDes.text = SkillManager._instance.activeSkillSupport[i].description;
            skillName.text = SkillManager._instance.activeSkillSupport[i].skillName;
            NGUITools.AddChild(skillUIgird.gameObject, skillUIGameObj);
            skillUIgird.AddChild(skillUIgird.transform);
        }

        
    
    
    }

    private void FindUIComponet() {
        skillIconTexture = skillUIGameObj.transform.Find("Spell Slot/Icon").GetComponent<UITexture>();
        skillDes = skillUIGameObj.transform.Find("Spell name").GetComponent<UILabel>();
        skillName = skillUIGameObj.transform.Find("Spell description").GetComponent<UILabel>();
    }


    void OnHideSkillWindow()
    {
        this.gameObject.SetActive(false);
    }
	 
}
