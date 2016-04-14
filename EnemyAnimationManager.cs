using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyAnimationManager : MonoBehaviour {
    private bool _isstartFSMenemyAnimation = false;
    private Animation _enemyanimation;

    public delegate void EnemyAnimationHandle();
    public EnemyAnimationHandle enemyanimationState;

    private Transform SkillPos;
   
    
    public enum EnemyAnimationState
    {
        Init,
        SetUp,
        Idle,
        Move,
        StartRun,
        WaitAttack,
        Attack,
        CriticalAttack,
        TakeAttack,
        Death
    }

    [System.Serializable]
    public class EnemyAnimationType01   //idle,death类型动画速度不变
    {
        public AnimationClip animation;
        public float speedAnimation = 1.0f;
    }
    [System.Serializable]
    public class EnemyAnimationType02  //move类型动画速度可能改变
    {
        public AnimationClip animation;
        public float speedAnimation = 1.0f;
        public bool speedTuning;
    }

    [System.Serializable]
    public class EnemyAnimationNormalAttack
    {
        public string _animationname;
        public AnimationClip animation;
        public float speedAnimation = 1.0f;
        public float attackTimer = 0.5f;
        public float multipleDamage = 1f;
        public float flinchValue;
        public bool speedTuning;

        public Transform SkillPos;
        public bool IFIsSkillPosChild;
        public GameObject attackFX;
        public AudioClip soundFX;

    }
    [System.Serializable]
    public class EnemyAnimationSkillAttackNeedCast
    {
        public string _animationname;
        public AnimationClip animation;
        public float castTime;
        public float speedAnimation = 1.0f;
        public float attackTimer = 0.5f;
        public float multipleDamage = 1f;
        public float flinchValue;
        public bool speedTuning;        
        public GameObject attackFX;
        public AudioClip soundFX;
        public float CastAnimationSpeed;//聚气动画速度

        [HideInInspector]
        public float castTimer;//聚气计时器

    }
    //[System.Serializable]
    //public class AnimationCritAttack
    //{
    //    public string _name;
    //    public AnimationClip animation;
    //    public float speedAnimation = 1.0f;
    //    public float attackTimer = 0.5f;
    //    public float multipleDamage = 1f;
    //    public float flinchValue;
    //    public bool speedTuning;

    //    public GameObject attackFX;
    //    public AudioClip soundFX;

    //}

    [System.Serializable]
    public class EnemyAnimationTakeAttack //受到伤害时播放的动画
    {
        public string _animationname;
        public AnimationClip animation;
        public float speedAnimation = 1.0f;

    }

    public GameObject CastEffect;//怪物技能的聚气特效
    public AnimationClip CastAnimationClip;//怪物聚气动画
    
    public EnemyAnimationState _enemyanimationstate;
    public EnemyAnimationType01 _enemyIdle, _enemyDeath;  //Idle , death animation
    public EnemyAnimationType02 _enemyMove; //Move animation
    public List<EnemyAnimationNormalAttack> normalAttack; //normal attack animation
    public List<EnemyAnimationSkillAttackNeedCast> skillattack;
//    public List<AnimationCritAttack> criticalAttack; //critical attack animation
    public List<EnemyAnimationTakeAttack> takeAttack; //take attack animation


    //Private variable
    private  EnemyAIController _enemyAIController;
    private EnemyStatus _enemyStatus;
    private GameObject _skilleffectTemp;

    [HideInInspector]
    public bool checkAttack;

    //Editor Variable
    [HideInInspector]
    public int sizeNAtk = 0;
    [HideInInspector]
    public int sizeSAtk = 0;
   // [HideInInspector]
   // public int sizeCritAtk = 0;
    [HideInInspector]
    public int sizeTakeDmg = 0;
    [HideInInspector]
    public List<bool> showNormalAtkSize = new List<bool>();
    [HideInInspector]
    public List<bool> showSkillAtkSize = new List<bool>();
  //  [HideInInspector]
  //  public List<bool> showCritSize = new List<bool>();
    [HideInInspector]
    public List<bool> showTakeDmgSize = new List<bool>();

    void Start()
    {
        _enemyAIController = this.GetComponent<EnemyAIController>();
        _enemyStatus = this.GetComponent<EnemyStatus>();
        _enemyanimation = this.GetComponent<Animation>();
        _enemyanimation[_enemyIdle.animation.name].wrapMode = WrapMode.Loop;
        _enemyanimation[_enemyMove.animation.name].wrapMode = WrapMode.Loop;
        SkillPos = this.gameObject.transform.Find("SkillPos");
        enemyanimationState = Idle;
        _isstartFSMenemyAnimation = true;
        _enemyanimationstate = EnemyAnimationState.Init;
       // StartCoroutine("EnemyAnimationFSM");
    }

    void Update() {
        if (enemyanimationState != null)
            enemyanimationState();
    }


    //private void EnemyAnimationFSM()
    //{
    //    while (_isstartFSMenemyAnimation)
    //    {
    //        switch (_enemyanimationstate) { 
    //            case EnemyAnimationState.Init:
    //                Init();
    //                break;
    //            case EnemyAnimationState.SetUp:
    //                SetUp();
    //                break;
    //            case EnemyAnimationState.StartRun:
    //                StartRun();
    //                break;
    //            case EnemyAnimationState.Idle:
    //                Idle();
    //                break;
    //            case EnemyAnimationState.Move:
    //                Move();
    //                break;
    //            case EnemyAnimationState.WaitAttack:
    //                WaitAttack();
    //                break;
    //            case EnemyAnimationState.Attack:
    //                Attack();
    //                break;
    //            case EnemyAnimationState.TakeAttack:
    //                TakeAttack();
    //                break;
    //            case EnemyAnimationState.Death:
    //                Death();
    //                break;
                       
    //        }

    //    }
   


    //}


    void Init() {
        if (!GetComponent<EnemyAIController>()) return;
        if (!GetComponent<EnemyStatus>()) return;
        if (!GetComponent<Animation>()) return;
        if (!SkillPos) return;
        _enemyanimationstate = EnemyAnimationState.SetUp;
    
    }
    void SetUp() {
        _enemyanimationstate = EnemyAnimationState.StartRun;
    }
   public void StartRun() {
        _enemyanimationstate = EnemyAnimationState.Idle;
    }
   public  void Idle() {
       
        _enemyanimation.CrossFade(_enemyIdle.animation.name,0.25f);
        _enemyanimation[_enemyIdle.animation.name].speed = _enemyIdle.speedAnimation;
    }
    public void Move() {
        _enemyanimation.CrossFade(_enemyMove.animation.name,0.25f);

        if (_enemyMove.speedTuning)  //Enable Speed Tuning
        {
            _enemyanimation[_enemyMove.animation.name].speed = (_enemyStatus._enemyStatus.Movespd/ 3f) / _enemyMove.speedAnimation;
        }
        else
        {
            _enemyanimation[_enemyMove.animation.name].speed = _enemyMove.speedAnimation;
        }
		
    }
 
    public void Attack() {

        if (_enemyAIController.AttackType == 0)
        {
            _enemyanimation.Play(normalAttack[_enemyAIController.AttackTypeIndex].animation.name);

            if (normalAttack[_enemyAIController.AttackTypeIndex].speedTuning)  //Enable Speed Tuning
            {
                _enemyanimation[normalAttack[_enemyAIController.AttackTypeIndex].animation.name].speed = (_enemyStatus._enemyStatus.AtkSpd / 100f) / normalAttack[_enemyAIController.AttackTypeIndex].speedAnimation;
            }
            else
            {
                _enemyanimation[normalAttack[_enemyAIController.AttackTypeIndex].animation.name].speed = normalAttack[_enemyAIController.AttackTypeIndex].speedAnimation;
            }

            //Calculate Attack
            if (_enemyanimation[normalAttack[_enemyAIController.AttackTypeIndex].animation.name].normalizedTime > normalAttack[_enemyAIController.AttackTypeIndex].attackTimer && !checkAttack)
            {
                if (_skilleffectTemp != null) Destroy(_skilleffectTemp);
                if (normalAttack[_enemyAIController.AttackTypeIndex].attackFX != null)
                {

                    _skilleffectTemp = (GameObject)Instantiate(normalAttack[_enemyAIController.AttackTypeIndex].attackFX, normalAttack[_enemyAIController.AttackTypeIndex].SkillPos.position, normalAttack[_enemyAIController.AttackTypeIndex].SkillPos.rotation);
                    SkillParameters sp = _skilleffectTemp.GetComponent<SkillParameters>();
                    sp.RecieveSkillParameters(normalAttack[_enemyAIController.AttackTypeIndex].multipleDamage, 0, 0, this.gameObject);
                    if (normalAttack[_enemyAIController.AttackTypeIndex].IFIsSkillPosChild == true)
                    _skilleffectTemp.transform.parent = SkillPos.transform;                          
                    
                }
                
                
                //Attack Damage TODO
                //HeroController enemy;
                //enemy = enemyController.target.GetComponent<HeroController>();
                //enemy.GetDamage(enemyStatus.status.atk * normalAttack[enemyController.typeAttack].multipleDamage, enemyStatus.status.hit, normalAttack[enemyController.typeAttack].flinchValue
                //                , normalAttack[enemyController.typeAttack].attackFX, normalAttack[enemyController.typeAttack].soundFX);
                checkAttack = true;
            }

         



            if (_enemyanimation[normalAttack[_enemyAIController.AttackTypeIndex].animation.name].normalizedTime > 0.9f)
            {

                enemyanimationState = Idle;
                _enemyAIController._enemystate = EnemyAIController.EnemyState.Search;
                Debug.Log("Search");
            }

        }

        else {
            
            _enemyanimation.Play(skillattack[_enemyAIController.AttackTypeIndex].animation.name);

            if (skillattack[_enemyAIController.AttackTypeIndex].speedTuning)  //Enable Speed Tuning
            {
                _enemyanimation[skillattack[_enemyAIController.AttackTypeIndex].animation.name].speed = (_enemyStatus._enemyStatus.AtkSpd / 100f) / skillattack[_enemyAIController.AttackTypeIndex].speedAnimation;
            }
            else
            {
                _enemyanimation[skillattack[_enemyAIController.AttackTypeIndex].animation.name].speed = skillattack[_enemyAIController.AttackTypeIndex].speedAnimation;
            }

            //Calculate Attack
            if (_enemyanimation[skillattack[_enemyAIController.AttackTypeIndex].animation.name].normalizedTime > skillattack[_enemyAIController.AttackTypeIndex].attackTimer && !checkAttack)
            {
                checkAttack = true;
                if (_skilleffectTemp != null) Destroy(_skilleffectTemp);
                if (skillattack[_enemyAIController.AttackTypeIndex].attackFX != null)
                {
                    _skilleffectTemp = (GameObject)Instantiate(skillattack[_enemyAIController.AttackTypeIndex].attackFX, SkillPos.position, SkillPos.rotation);
                    SkillParameters sp = _skilleffectTemp.GetComponent<SkillParameters>();
                    sp.RecieveSkillParameters(skillattack[_enemyAIController.AttackTypeIndex].multipleDamage, 0, 0, this.gameObject);

                }
                //Attack Damage TODO
                //HeroController enemy;
                //enemy = enemyController.target.GetComponent<HeroController>();
                //enemy.GetDamage(enemyStatus.status.atk * normalAttack[enemyController.typeAttack].multipleDamage, enemyStatus.status.hit, normalAttack[enemyController.typeAttack].flinchValue
                //                , normalAttack[enemyController.typeAttack].attackFX, normalAttack[enemyController.typeAttack].soundFX);
                
            }



            if (_enemyanimation[skillattack[_enemyAIController.AttackTypeIndex].animation.name].normalizedTime > 0.9f)
            {
               
              
                enemyanimationState = Idle;
                _enemyAIController._enemystate = EnemyAIController.EnemyState.Search;
                Debug.Log("Search");
          
            }

        }
        

        
    }

   public void TakeAttack() {
       _enemyanimation.CrossFade(takeAttack[_enemyAIController.typeTakeAttack].animation.name);
       _enemyanimation[takeAttack[_enemyAIController.typeTakeAttack].animation.name].speed = takeAttack[_enemyAIController.typeTakeAttack].speedAnimation;

        if (_enemyanimation[takeAttack[_enemyAIController.typeTakeAttack].animation.name].normalizedTime > 0.9f)
        {
            _enemyAIController._enemystate = EnemyAIController.EnemyState.WaitAttack;
            _enemyanimationstate=EnemyAnimationState.WaitAttack;
        }
    }
    public void Death() { }







}
