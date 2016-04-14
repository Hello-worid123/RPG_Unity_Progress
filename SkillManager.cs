/// <summary>
/// 玩家技能管理类
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager _instance;

    private Transform SkillPos;

    private const string MAGICANSKILLNPATH = "magican/skill/";
    public enum SkillAttribute { Hp, Mp, Atk, Def, Spd, HitRate,MissRate, CriticalRate, AtkSpd, AtkRange, MoveSpd }
    public enum SkillsTypeSupport { Buff, Heal };//buff类型，治疗类型
    public enum SkillPosType { FreeTarget, Instance, None };//自由目标，自身周围，没（被动技能）
    public enum TargetSkill { SingleTarget, MultipleTarget, OwnTarget };//单人，多人，自身

    [System.Serializable]
    public class PassiveSkill//被动技能{按排序：技能名字，技能id，技能开锁的等级，技能图标，技能贡献类型（生命，mp。。），添加多少属性}
    {
        public string skillName = "Passive Skill";
        public int skillID;
        public int unlockLevel = 1;
        public Texture2D skillIcon;
        public SkillAttribute skillAttribute;
        public float addValue;
        [Multiline]//描述
        public string description = "This is passive skill.";
        public string typeSkill = "PassiveSkill";

        [HideInInspector]//isadd
        public bool isAdd;
    }
    [System.Serializable]
    public class NormalSkill//普通技能（不需要聚气）
    {
        public string skillName = "Normal Skill Attack";
        public int skillID;
        public int unlockLevel = 1;
        public Texture2D skillIcon;
        public int mpUse;
        public TargetSkill targetSkill = SkillManager.TargetSkill.SingleTarget;
        public float skillArea;//技能释放地区
        public SkillPosType skillPosType = SkillPosType.FreeTarget;
        public float skillRange;//技能范围
        public AnimationClip animationNormalSkillAttack;
        public float speedAnimation = 1;    
        public float attackTimer = 0.5f;
        public float multipleDamage = 1;
        public float flichValue = 0;
        public float skillAccurate;
        [Multiline]//描述
        public string description = "This is Normal skill.";
        public string typeSkill = "NormalSkillAttack";

        public bool speedTuning;//speedtuning

        public GameObject skillFX;//技能特效？
        public AudioClip soundFX;//技能声音

        public GameObject skillFxTarget;//被攻击目标产生的特效
        public AudioClip soundFxTarget;//被攻击目标产生的声音

        [HideInInspector]
        public bool isAdd;
        
    }


    [System.Serializable]
    public class ActiveSkill//主动技能{按排序：技能名字，技能id，技能开锁的等级，技能图标，mp使用量，技能是锁定目标还是不用锁定，
    //skillarea，技能的动画，动画速度，cast time，attackTimer（激活技能），multipledamage，flichvalue，skillaccurate}
    {
        public string skillName = "Skill Attack";
        public int skillID;
        public int unlockLevel = 1;
        public Texture2D skillIcon;
        public int mpUse;
        public TargetSkill targetSkill = SkillManager.TargetSkill.SingleTarget;
        public float skillArea;//技能释放地区
        public SkillPosType skillPosType = SkillPosType.FreeTarget;
        public float skillRange;//技能范围
        public AnimationClip animationAttack;
        public float speedAnimation = 1;
        public float castTime = 0;
        public float attackTimer = 0.5f;
        public float multipleDamage = 1;
        public float flichValue = 0;
        public float skillAccurate;
        [Multiline]//描述
        public string description = "This is active skill.";
        public string typeSkill = "AttackSkillNeedCast";

        public bool speedTuning;//speedtuning

        public GameObject skillFX;//技能特效？
        public AudioClip soundFX;//技能声音

        public GameObject skillFxTarget;//被攻击目标产生的特效
        public AudioClip soundFxTarget;//被攻击目标产生的声音

        [HideInInspector]
        public bool isAdd;
        [HideInInspector]
        public float castTimer;
    }

    [System.Serializable]
    public class ActiveSkillSupport//主动{按排序：技能名字，技能id，技能开锁的等级，技能图标，技能使用的mp，技能动作，
    //技能速度，casttime，activerTimer，持续的时间？，技能贡献的类型（生命，mp。。），add value ，描述 ，buff特效，特效声音}
    {
        public string skillName = "Skill Support";
        public int skillID;
        public int unlockLevel = 1;
        public Texture2D skillIcon;
        public int mpUse;
        public SkillsTypeSupport skillTypeSupport = SkillsTypeSupport.Buff;
        public AnimationClip animationBuff;
        public float speedAnimation = 1;
        public float castTime = 0;
        public float activeTimer = 0.5f;
        public float duration;
        public SkillAttribute skilladdAttribute;
        public float addValue;
        [Multiline]
        public string description = "This is Support skill.";
        public string typeSkill = "SupportSkill";

        public GameObject buffFX;
        public AudioClip soundFX;

        [HideInInspector]
        public bool isAdd;
        [HideInInspector]
        public float castTimer;

    }

    [System.Serializable]
    public class DurationBuff//持续buff（buff名，技能索引，技能图标，持续的时间，iscount，持续时间计数器？）
    {
        public string buffName;
        public int skillIndex;
        public Texture2D skillIcon;
        public float duration;
        public bool isCount;//???
        public float durationTimer;

    }


    public List<PassiveSkill> passiveSkill = new List<PassiveSkill>();  //Passive skill（被动skill List）
    public List<NormalSkill> normalSKill = new List<NormalSkill>();
    public List<ActiveSkill> activeSkillAttack = new List<ActiveSkill>(); //Active Attack Skill（主动skill List）
    public List<ActiveSkillSupport> activeSkillSupport = new List<ActiveSkillSupport>(); //Buff Skill（主动support List）

    [HideInInspector]
    public DurationBuff[] durationBuff = new DurationBuff[20]; //持续Buff的组


    //Private Variable
    private AdvanceMovement controller;
    private PlayerStatus playerStatus;
    private AnimationManager animationManager;
    private bool _SetUpSkillTimer;
    [HideInInspector]
    public bool oneShotResetTarget;


    [HideInInspector]
    public bool canCast;


    private string typeofSkill;
    private int currentUseSkill;
    private GameObject magicCircle;


    //Editor Variable
    [HideInInspector]
    public int sizePassiveSkill = 0;
    [HideInInspector]
    public int sizeActiveAttack = 0;
    [HideInInspector]
    public int sizeActiveSupport = 0;
    [HideInInspector]
    public int sizeNormalAttack = 0;
    [HideInInspector]
    public List<bool> showPassiveSize = new List<bool>();
    [HideInInspector]
    public List<bool> showActiveAttackSize = new List<bool>();
    [HideInInspector]
    public List<bool> showActiveSupportSize = new List<bool>();
    [HideInInspector]
    public List<bool> showNormalAttackSize = new List<bool>();

    private GameObject tempSkillEffect;//
    void Awake() {
        _instance = this;
    }
    void Start()
    {

        //GameObject go;
        //go = GameObject.Find("GUI Manager/HeroBotBar");
        //botBar = go.GetComponent<BottomBar>();

        //controller = this.GetComponent<HeroController>();
    
        //animationManager = this.GetComponent<AnimationManager>();


        controller = this.GetComponent<AdvanceMovement>();
        playerStatus = this.GetComponent<PlayerStatus>();
        animationManager = this.GetComponent<AnimationManager>();
        SkillPos = this.gameObject.transform.Find("SkillPos").transform;
        _SetUpSkillTimer = false;

        UpdatePassiveStatus();
      

    }

    void Update() {
        CountDurationBuff();
    }


    //计算buff剩余时间
    void CountDurationBuff()
    {
        for (int i = 0; i < durationBuff.Length; i++)
        {
            if (durationBuff[i].isCount)//iscount?(状态位，因为这里有update不停调用)
            {
                if (durationBuff[i].duration > 0)
                {
                    durationBuff[i].duration -= Time.deltaTime;

                }
                else if (durationBuff[i].duration <= 0)
                {
                    activeSkillSupport[durationBuff[i].skillIndex].isAdd = false;//撤销buff，isadd为false，skillindex为技能索引
                    CheckAddAttributeBuff(durationBuff[i].skillIndex, "Debuff");
                    playerStatus.UpdateAttribute();//更新人物状态！！！
                    durationBuff[i].buffName = "";
                    durationBuff[i].skillIndex = 0;
                    durationBuff[i].duration = 0;
                    durationBuff[i].skillIcon = null;
                    durationBuff[i].isCount = false;
                }
            }
        }
    }

    //Text damage
    //public void InitTextDamage(Color colorText, string damageGet)
    //{
    //    // Init text damage
    //    GameObject loadPref = (GameObject)Resources.Load("TextDamage");
    //    GameObject go = (GameObject)Instantiate(loadPref, transform.position + (Vector3.up * 1.0f), Quaternion.identity);
    //    go.GetComponentInChildren<TextDamage>().SetDamage(damageGet, colorText);
    //}


    //
    void CheckAddAttributeBuff(int indexSkill, string command)
    {
        switch (activeSkillSupport[indexSkill].skilladdAttribute) {
            case SkillAttribute.Hp:
                if (command == "Buff") {
                    playerStatus.statusAdd.Hp += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.Hp -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                break;
            case SkillAttribute.Mp:
                if (command == "Buff") {
                    playerStatus.statusAdd.Mp += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.Mp -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
            case SkillAttribute.Atk:
                if (command == "Buff") {
                    playerStatus.statusAdd.Atk += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.Atk -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
            case SkillAttribute.Def:
                if (command == "Buff") {
                    playerStatus.statusAdd.Def += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.Def -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
            case SkillAttribute.Spd:
                if (command == "Buff") {
                    playerStatus.statusAdd.Spd += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.Spd -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
            case SkillAttribute.HitRate:
                if (command == "Buff") {
                    playerStatus.statusAdd.HitRate += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.HitRate -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
              
                break;
            case SkillAttribute.MissRate:
                if (command == "Buff") {
                    playerStatus.statusAdd.MissRate += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.MissRate -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
              
                break;
            case SkillAttribute.CriticalRate:
                if (command == "Buff") {
                    playerStatus.statusAdd.CriticalRate += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.CriticalRate -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
              
                break;
            case SkillAttribute.AtkRange:
                if (command == "Buff") {
                    playerStatus.statusAdd.AtkRange += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.AtkRange-= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
            case SkillAttribute.MoveSpd:
                if (command == "Buff") {
                    playerStatus.statusAdd.MoveSpd += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.MoveSpd -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
            case SkillAttribute.AtkSpd:
                if (command == "Buff") {
                    playerStatus.statusAdd.AtkSpd += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
                if (command == "Debuff")
                {
                    playerStatus.statusAdd.AtkSpd -= Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                }
               
                break;
                         
        }
    }



    //更新被动状态
    void UpdatePassiveStatus()
    {
        for (int i = 0; i < passiveSkill.Count; i++)
        {
            if (passiveSkill[i].unlockLevel <= playerStatus.status.Lv && !passiveSkill[i].isAdd)//当解锁等级大于等于人物等级和isadd为false时(有问题这里。。)
            {
                CheckAddAttributePassive(i);
                passiveSkill[i].isAdd = true;//这里状态改变（已经添加）
              
            }

           // i++;

        }
    }

    void CheckAddAttributePassive(int indexSkill)
    {

        switch(passiveSkill[indexSkill].skillAttribute){
            case SkillAttribute.Hp:
                playerStatus.statusAdd.Hp += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.Mp:
                playerStatus.statusAdd.Mp += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.Atk:
                playerStatus.statusAdd.Atk += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.Def:
                playerStatus.statusAdd.Def += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.Spd:
                playerStatus.statusAdd.Spd += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.HitRate:
                playerStatus.statusAdd.HitRate += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.MissRate:
                playerStatus.statusAdd.MissRate += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.CriticalRate:
                playerStatus.statusAdd.CriticalRate += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.AtkRange:
                playerStatus.statusAdd.AtkRange += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.MoveSpd:
                playerStatus.statusAdd.MoveSpd += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
            case SkillAttribute.AtkSpd:
                playerStatus.statusAdd.AtkSpd += Mathf.FloorToInt(passiveSkill[indexSkill].addValue);
                break;
              
        }
    
    }


  

    //加buff或减buff
  

    //更加ID找技能类型
    public string FindSkillType(int skillID)
    {
        for (int i = 0; i < passiveSkill.Count; i++)
        {
            if (skillID == passiveSkill[i].skillID)
            {
                typeofSkill = "PassiveSkill";
                return typeofSkill;
            }
        }
        for (int i = 0; i <normalSKill.Count; i++)
        {
            if (skillID == normalSKill[i].skillID)
            {
                typeofSkill = "NormalSkillAttack";
                return typeofSkill;
            }
        }

        
        for (int i = 0; i <activeSkillSupport.Count; i++)
        {
            if (skillID == activeSkillSupport[i].skillID)
            {
                typeofSkill = "SupportSkill";
                return typeofSkill;
            }

        }

        for (int i = 0; i < activeSkillAttack.Count; i++)
        {
            if (skillID == activeSkillAttack[i].skillID)
            {
                typeofSkill = "AttackSkillNeedCast";
                return typeofSkill;
            }
        }

        return "";
    }

    //（根据skillid找出技能在list的索引）
    public int FindSkillIndex(int skillID)
    {
        for (int i = 0; i < passiveSkill.Count; i++)
        {
            if (skillID == passiveSkill[i].skillID)
            {
                currentUseSkill = i;
                return currentUseSkill;
            }

        }
        for (int i = 0; i < normalSKill.Count; i++)
        {
            if (skillID == normalSKill[i].skillID)
            {
                currentUseSkill = i;
                return currentUseSkill;
            }

        }


        for (int i = 0; i < activeSkillSupport.Count; i++)
        {
            if (skillID == activeSkillSupport[i].skillID)
            {
                currentUseSkill = i;
                return currentUseSkill;
            }

        }

        for (int i = 0; i < activeSkillAttack.Count; i++)
        {
            if (skillID == activeSkillAttack[i].skillID)
            {
                currentUseSkill = i;
                return currentUseSkill;
            }

        }

        return 0;
    }

    //Cast Break Method
    public void CastBreak()//打断技能？引用
    {
        if (magicCircle != null)
            Destroy(magicCircle);
     //   controller.target = null;
       // setupSkillTimer = false;
        canCast = false;
       
    }

    //Cast skill method（人物状态为cast时引用）
    public void CastSkill(string skillType, int indexSkill)//技能类型，技能索引
    {
        if (skillType == "NormalSkillAttack")
        {
            SendParameterSkill(skillType, indexSkill);//传参给animationManager
            controller._state = AdvanceMovement._State.ActiveSkill;
            return;
        
        }

            
        if(!_SetUpSkillTimer){
          switch (skillType) { 
            case "SupportSkill":
                if ( activeSkillSupport[indexSkill].castTime > 0)
                {
                   // controller.ResetBeforeCast();
                  
                    canCast=true;
                }
                break;
            case "AttackSkillNeedCast":
                if (activeSkillAttack[indexSkill].castTime > 0)
                {
                    
                   // controller.ResetBeforeCast();
                    canCast=true;                        
                }
                break;
               }

              if(canCast){

                 //SoundManager.instance.PlayingSound("Cast_Skill");//播放cast（聚气）的声音
                 if (magicCircle != null)
                  Destroy(magicCircle);
                 magicCircle = (GameObject)Instantiate(Gamesetting._instance.castCircleEffect, new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z), Gamesetting._instance.castCircleEffect.transform.rotation);//产生聚气特效
                 _SetUpSkillTimer = true;	//设计技能计时器，进入下个阶段
                 activeSkillAttack[indexSkill].castTimer = 0;//把主动buff的casttimer设置为0
            
                }
           }


        if (canCast)
        {
            //mp扣减处理
            //TODO
            switch (skillType)
            {
                case "SupportSkill":
                    if (activeSkillSupport[indexSkill].castTimer < activeSkillSupport[indexSkill].castTime)
                    {
                        activeSkillSupport[indexSkill].castTimer += Time.deltaTime;
                    }
                    else
                    {
                        activeSkillSupport[indexSkill].castTimer = 0;
                        SendParameterSkill(skillType, indexSkill);//传参给animationManager
                        controller._state = AdvanceMovement._State.ActiveSkill;
                        _SetUpSkillTimer = false;
                        canCast = false;

                    }

                    break;
                case "AttackSkillNeedCast":

                    if (activeSkillAttack[indexSkill].castTimer < activeSkillAttack[indexSkill].castTime)
                    {
                        activeSkillAttack[indexSkill].castTimer += Time.deltaTime;
                    }
                    else
                    {
                        activeSkillAttack[indexSkill].castTimer = 0;
                        SendParameterSkill(skillType, indexSkill);//传参给animationManager
                        controller._state = AdvanceMovement._State.ActiveSkill;
                        _SetUpSkillTimer = false;
                        canCast = false;

                    }
                    break;
            }
        }
        else
        {
            controller._state = AdvanceMovement._State.Run;
        }

    }


    //传参给animationManager
    void SendParameterSkill(string skillType, int indexSkill)
    {
        if (skillType == "SupportSkill")
        {
            animationManager.animationskillsetup.skillType = skillType;
            animationManager.animationskillsetup.skillIndex = indexSkill;
            animationManager.animationskillsetup.animationSkill = activeSkillSupport[indexSkill].animationBuff;
            animationManager.animationskillsetup.speedAnimation = activeSkillSupport[indexSkill].speedAnimation;
            animationManager.animationskillsetup.activeTimer = activeSkillSupport[indexSkill].activeTimer;
            animationManager.animationskillsetup.speedTuning = false;

        }
        else if (skillType == "AttackSkillNeedCast")
        {
            animationManager.animationskillsetup.skillType = skillType;
            animationManager.animationskillsetup.skillIndex = indexSkill;
            animationManager.animationskillsetup.animationSkill = activeSkillAttack[indexSkill].animationAttack;
            animationManager.animationskillsetup.speedAnimation = activeSkillAttack[indexSkill].speedAnimation;
            animationManager.animationskillsetup.activeTimer = activeSkillAttack[indexSkill].attackTimer;
            animationManager.animationskillsetup.speedTuning = activeSkillAttack[indexSkill].speedTuning;
        }
        else {
            animationManager.animationskillsetup.skillType = skillType;
            animationManager.animationskillsetup.skillIndex = indexSkill;
            animationManager.animationskillsetup.animationSkill =normalSKill[indexSkill].animationNormalSkillAttack;
            animationManager.animationskillsetup.speedAnimation = normalSKill[indexSkill].speedAnimation;
            animationManager.animationskillsetup.activeTimer = normalSKill[indexSkill].attackTimer;
            animationManager.animationskillsetup.speedTuning = normalSKill[indexSkill].speedTuning;
        
        }
    }

    //Active Skill method
    public void Activeskill(string skillType, int indexSkill)
    {
        if (magicCircle != null)
            Destroy(magicCircle);

        if (skillType == "SupportSkill")
        {

            if (activeSkillSupport[indexSkill].skillTypeSupport == SkillsTypeSupport.Buff)
            {
                if (activeSkillSupport[indexSkill].buffFX != null)
                    Instantiate(activeSkillSupport[indexSkill].buffFX, transform.position, Quaternion.identity);

                if (activeSkillSupport[indexSkill].soundFX != null)
                    AudioSource.PlayClipAtPoint(activeSkillSupport[indexSkill].soundFX, transform.position);


                if (!activeSkillSupport[indexSkill].isAdd)//如果isadd为false
                {
                    CheckAddAttributeBuff(indexSkill, "Buff");
                    activeSkillSupport[indexSkill].isAdd = true;
                }
                SentBuffParameter(indexSkill);//重新重置buff的时间
                playerStatus.UpdateAttribute();//更新属性
            }
            else if (activeSkillSupport[indexSkill].skillTypeSupport == SkillsTypeSupport.Heal)
            {
                if (activeSkillSupport[indexSkill].buffFX != null)
                   // Instantiate(activeSkillSupport[indexSkill].buffFX, transform.position, Quaternion.identity);
                    GameObject.Instantiate(activeSkillSupport[indexSkill].buffFX, transform.position + Vector3.up * 15, Quaternion.identity);

                if (activeSkillSupport[indexSkill].soundFX != null)
                    AudioSource.PlayClipAtPoint(activeSkillSupport[indexSkill].soundFX, transform.position);


                // InitTextDamage(Color.green, activeSkillSupport[indexSkill].addValue.ToString());
                playerStatus.statusCal.Hp += (int)activeSkillSupport[indexSkill].addValue;
                playerStatus.UpdateAttribute();//更新人物属性
            }
        }

        else if (skillType == "AttackSkillNeedCast")
        {
           
                if (activeSkillAttack[indexSkill].skillFX != null)
                    Instantiate(activeSkillAttack[indexSkill].skillFX, transform.position, Quaternion.identity);

                if (activeSkillAttack[indexSkill].soundFX != null)
                    AudioSource.PlayClipAtPoint(activeSkillAttack[indexSkill].soundFX, transform.position);

                if (activeSkillAttack[indexSkill].skillAccurate == 0)//如果技能不附加命中率
                    activeSkillAttack[indexSkill].skillAccurate = playerStatus.status.HitRate;

    
        }
        else {

            if (tempSkillEffect != null) Destroy(tempSkillEffect);
            if (normalSKill[indexSkill].skillFX != null)
             tempSkillEffect=(GameObject)Instantiate(normalSKill[indexSkill].skillFX, SkillPos.position,SkillPos.rotation);
             SkillParameters skillparameters=tempSkillEffect.GetComponent<SkillParameters>();
             skillparameters.RecieveSkillParameters(normalSKill[indexSkill].multipleDamage,0,0,this.gameObject);

            if (normalSKill[indexSkill].soundFX != null)
                AudioSource.PlayClipAtPoint(normalSKill[indexSkill].soundFX, transform.position);

            if (normalSKill[indexSkill].skillAccurate == 0)//如果技能不附加命中率
                normalSKill[indexSkill].skillAccurate = playerStatus.status.HitRate;
            
        }

            #region TEST TODO
            //else if (skillType == "Attack")
            //{
            //    if (activeSkillAttack[indexSkill].skillType == SkillPosType.Instance)
            //    {
            //      //  controller.ResetBeforeCast(); 
            //        if (activeSkillAttack[indexSkill].skillName != "")
            //        {
            //           GameObject skilleffect = Resources.Load(MAGICANSKILLNPATH + activeSkillAttack[indexSkill].skillName) as GameObject;
            //            switch (activeSkillAttack[indexSkill].skillName) {
            //                case "Ice":
            //                    GameObject.Instantiate(skilleffect, transform.position + transform.rotation * Vector3.forward * 8+Vector3.up*10, Quaternion.identity);
            //                    break;
            //                case "shine":
            //                    GameObject.Instantiate(skilleffect, transform.position + Vector3.up * 15, Quaternion.identity);
            //                    break;
            //                case "Explosion": 
            //                    GameObject.Instantiate(skilleffect, transform.position + transform.rotation * Vector3.forward * 8, Quaternion.identity);
            //                    break;



            //            }
            #endregion

      
    }

    //传送参数
    void SentBuffParameter(int indexSkill)
    {
        for (int i = 0; i < durationBuff.Length; i++)
        {
            if (durationBuff[i].buffName == "" || durationBuff[i].buffName == activeSkillSupport[indexSkill].skillName)//找到原来已添加还持续的buff
            {
                durationBuff[i].buffName = activeSkillSupport[indexSkill].skillName;
                durationBuff[i].skillIndex = indexSkill;
                durationBuff[i].skillIcon = activeSkillSupport[indexSkill].skillIcon;
                durationBuff[i].duration = activeSkillSupport[indexSkill].duration;
                durationBuff[i].isCount = true;
                break;
            }

        }
    }

    //Count duration buff(更新持续型的buff)


    //Gizmoz Variable

    public List<float> skillAreaCurrent = new List<float>();


    //Draw Gizmoz
    [ExecuteInEditMode]
    void OnDrawGizmosSelected()
    {
        while (skillAreaCurrent.Count != activeSkillAttack.Count)
        {
            if (activeSkillAttack.Count > skillAreaCurrent.Count)
            {
                skillAreaCurrent.Add(0);
            }
            else
            {
                skillAreaCurrent.RemoveAt(skillAreaCurrent.Count - 1);
            }
        }

        for (int i = 0; i < activeSkillAttack.Count; i++)
        {
            if (skillAreaCurrent[i] != activeSkillAttack[i].skillArea)
            {
                Gizmos.color = new Color(0.5f, 0f, 0f, 0.3f);
                Gizmos.DrawSphere(transform.position, activeSkillAttack[i].skillArea);
            }
            skillAreaCurrent[i] = activeSkillAttack[i].skillArea;
        }


    }

}

