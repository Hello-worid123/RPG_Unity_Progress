using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIController : MonoBehaviour {
    private bool _isstartFSMenemyAI = false;
    
    private CollisionFlags _enemycollisionflags;

    public enum EnemyState
    {
        Init,
        SetUp,
        Search,
        WaitAttack,
        Attack,
        Cast,
        ActiveSkillNeedCast,
        TakeAttack,
        ReBack,
        Flee,
        Death
    }

  

    public enum EnemyBehavior
    { Standing, MoveAround } //（行为：站立，到处移动）

    public enum EnemyNature
    { Natural, Wild } //（怪物特型：自然（不会自动攻击），野生（自动攻击））

    public enum MoveAroundBehaviorState
    { MoveToNext, Waiting }//状态Idle下：向下个随机目标点移动，等待时间）

    public EnemyState _enemystate;
    public EnemyBehavior _enemyBehavior; 
    public EnemyNature _enemynature;   
    public MoveAroundBehaviorState _enemymoveAroundBehaviorstate; 

    public GameObject targetPlayer;     //目标（敌人）
    public bool chaseTarget;	  //是否追赶目标（敌人）
    public List<GameObject> modelMesh = new List<GameObject>(); //model mesh
    public Color colorTakeDamage; //color model when take attack
    public float deadTimer; //destroy timer when enemy dead
    public bool deadTransparent; //if enable this when enemy dead it will transparent
    public float speedFade; //speed transparent
    public bool regenHP;		//regen hp when enemy return to spawn point
    public bool regenMP;		//regen mp when enemy return to spawn point

    

    public float movePhase;  //敌人移动范围
    public float returnPhase; //超过离返回点的约定距离就返回
    public float delayNextTargetMin; //idle状态时间最小值
    public float delayNextTargetMax; //idle状态时间最大值
    public GameObject floatingText;
    
    private float delayAttack = 100;//攻击间隔
    private float delayAttackMin = 30;
    private float delayAttackMax = 150;

    //Private variable
    private EnemyAnimationManager _enemyanimationManager;
    private EnemyStatus _enemyStatus;
    private Quest_Data _questData;
    private CharacterController _enemycharactercontroller;

    private float defaultReturnPhase;  //default return phase
    private Vector3 spawnPoint; 		//first spawn point
    private float timeToWaitBeforeNextMove;  //等待时间（向下一目标移动）
    
    private Vector3 destinationPosition;		// The destination Point
    private float destinationDistance;			// The distance between this.transform and destinationPosition
    private Vector3 movedir;
    private float moveSpeed;						//怪物速度由自身属性而定
    private Vector3 ctargetPos;					//Convert Target Position
    private Vector3 targetPos;					//目标位置
    private Quaternion targetRotation;			//Rotation
    private bool checkCritical;					//是否暴击
    private float flinchValue = 100;			//硬值值
    private Vector3 randomMoveVector;			//待定状态随机位置
    private float randomAngle;					//Random Angle Move
    private Shader alphaShader;				 //Use to transperent model
    private Color[] defaultColor;				//Default Material Color
    private bool enableRegen;					//Check Regen HP/MP
    private GameObject detectArea;				//警戒区
    private float Gravity = 20;
    private bool _setUpCastTimer; //状态位，是否设置了castTimer
    private bool _canActiveSkill;

    private float fadeValue;					// Fade Value
    [HideInInspector]
    public float defaultHP, defaultMP;			//DefaultHP,MP

    [HideInInspector]
    public int AttackTypeIndex;
    [HideInInspector]
    public int AttackType;				
    [HideInInspector]
    public int typeTakeAttack;					//受到攻击的动画类型
    [HideInInspector]
    public bool startFade;					//Check setup fade material


    //Editor Variable
    [HideInInspector]
    public int sizeMesh;

    private GameObject castEffect;//聚气特效
    private GameObject HpBarPosTarget;
    //血条的位置
    private Vector3 _targetPos;
    
    private float DistanceToCamera;
    private GameObject HpBar;

    void Start() {
        _enemyanimationManager = this.GetComponent<EnemyAnimationManager>();
        _enemyStatus = this.GetComponent<EnemyStatus>();
        _enemycharactercontroller = this.GetComponent<CharacterController>();
        HpBarPosTarget = this.gameObject.transform.Find("HpBarPos").gameObject;
  //      _questData = GameObject.Find("QuestData").GetComponent<Quest_Data>();
        _enemystate = EnemyState.Init;
        _isstartFSMenemyAI = true;
        
        StartCoroutine("FSMEnemyAI");
    

    }

 

    #region FSM

    private IEnumerator FSMEnemyAI() {
        while (_isstartFSMenemyAI) {
            switch (_enemystate) { 
                case EnemyState.Init:
                    Init();
                    break;
                case EnemyState.SetUp:
                    SetUp();
                    break;
                case EnemyState.Search:
                    Search();
                    break;
                case EnemyState.WaitAttack:
                    WaitAttack();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.TakeAttack:
                    TakeAttack();
                    break;
                case EnemyState.Cast:
                    Cast();
                    break;
                case EnemyState.ActiveSkillNeedCast:
                    ActiveSkillNeedCast();
                    break;
                case EnemyState.ReBack:
                    ReBack();
                    break;
                case EnemyState.Flee:
                    Flee();
                    break;
                case EnemyState.Death:
                    Death();
                    break;            
            
            }
            yield return null;              
        }
        
    }
    #endregion

   

    #region enemyState FSM Method
    void Init(){
        if (!GetComponent<EnemyAnimationManager>()) return;
        if (!GetComponent<CharacterController>()) return;
        if (!GetComponent<EnemyStatus>()) return;
        if (!HpBarPosTarget) return;
      //  if (!GetComponent<Quest_Data>()) return;
            _enemystate = EnemyState.SetUp;        
        }
    void SetUp(){
            //set spawn point
            destinationPosition = this.transform.position;
            delayAttack = 100;	//Declare delay 100 sec
            flinchValue = 100; //Declare flinch value (if zero it will flinch)
            fadeValue = 1; //Set fade value
             HpBar= HpBarManager._instance.GetHpBar(HpBarPosTarget);
             InvokeRepeating("CheckDistanceToMainCamera",0, 0.1f);

            //Set first spawn point
            spawnPoint = transform.position;

            //set default value
            defaultReturnPhase = returnPhase;
            defaultHP = _enemyStatus._enemyStatus.Hp;
            defaultMP = _enemyStatus._enemyStatus.Mp;

            defaultColor = new Color[modelMesh.Count];
            SetDefaultColor();


            
            if (_enemyBehavior == EnemyBehavior.MoveAround)
            {
                _enemymoveAroundBehaviorstate = MoveAroundBehaviorState.Waiting;
            }

            //warning when enemy isn't detect area
            if (_enemynature == EnemyNature.Wild)
            {
                detectArea = GameObject.Find("DetectArea");

                if (detectArea == null)
                {
                    Debug.LogWarning("Don't found DetectArea in Enemy -" + _enemyStatus.name);
                }

            }
            _enemystate = EnemyState.Search;

        }
        void Search(){
            if (_enemystate != EnemyState.Death)
            {

                if (_enemyBehavior == EnemyBehavior.MoveAround && !chaseTarget && !targetPlayer)//当怪物的行为是到处游走时并且没有追赶目标和目标
                {
                    if (_enemymoveAroundBehaviorstate == MoveAroundBehaviorState.MoveToNext)
                    {
                        UpdateMovingToNextPatrolPoint();
                    }
                    else if (_enemymoveAroundBehaviorstate ==  MoveAroundBehaviorState.Waiting)
                    {
                        UpdateWaitingForNextMove();
                    }

                }
                else//当怪物为战力状态或有追赶目标和目标时处理
                {

                    EnemyMovementBattle();
                }
            }
        
        
        }
        void WaitAttack() 
        {
            if (targetPlayer == null) return;
            _enemyanimationManager.enemyanimationState = _enemyanimationManager.Idle;//
            PlayerStatus playerStatus;
            playerStatus = targetPlayer.GetComponent<PlayerStatus>();
            if (playerStatus.statusCal.Hp <= 0)
            {
              //死亡处理TODO
            }


            if (delayAttack > 0)
            {

                delayAttack -= Time.deltaTime * _enemyStatus._enemyStatus.AtkSpd;//受到怪物本身属性攻击速度影响
            }
            else 
            {

              
                 
                checkCritical = CriticalCal(_enemyStatus._enemyStatus.CriticalRate);//检查是否暴击
                delayAttack = Random.Range(delayAttackMin, delayAttackMax);
                if (RandomIfSkillType()) {
                    
                    _enemystate = EnemyState.Cast;
                }
                else
                {
                    if (checkCritical)
                    {
                        _enemyanimationManager.checkAttack = false;
                        
                    }
                    else if (!checkCritical)
                    {                    
                        _enemyanimationManager.checkAttack = false;
                        
                    }
                    
                    _enemyanimationManager.enemyanimationState = _enemyanimationManager.Attack;
                    _enemystate = EnemyState.Attack;
                }

            }
        }
        void Attack(){
            if (_enemyanimationManager.enemyanimationState != _enemyanimationManager.Attack)
                _enemystate = EnemyState.Search;
        }
        void TakeAttack() { }
        void Cast() {

            CastSkillSetUp(AttackTypeIndex);
        }
        void ActiveSkillNeedCast() {
            _enemyanimationManager.checkAttack = false;
            
            _enemyanimationManager.enemyanimationState = _enemyanimationManager.Attack;
            _enemystate = EnemyState.Attack;
        }
        void ReBack(){}
        void Flee() { }
        void Death() { }
    #endregion

    //伤害数值显示
        public void AddFloatingText(Vector3 pos, string text)
        {
            // Adding Floating Text Effect
            if (floatingText)
            {
                var floattext = (GameObject)Instantiate(floatingText, pos, transform.rotation);
                FloatingText floatingtext = floattext.GetComponent<FloatingText>();
                if (floatingtext)
                {
                    floatingtext.Text = text;
                }
                GameObject.Destroy(floattext, 1);
            }
        }
        void CastSkillSetUp(int skillindex) {
            if (!_setUpCastTimer)
            {              
                if (castEffect != null)
                {
                    Destroy(castEffect);
                }

                 castEffect = (GameObject)Instantiate(_enemyanimationManager.CastEffect, transform.position, transform.rotation);
               _enemyanimationManager.skillattack[skillindex].castTimer = 0;
                GetComponent<Animation>()[_enemyanimationManager.CastAnimationClip.name].speed = _enemyanimationManager.skillattack[skillindex].CastAnimationSpeed;
                _setUpCastTimer = true;
                _canActiveSkill = true;
                
            }
            
            if(_canActiveSkill)
            {
               
               
                if (_enemyanimationManager.skillattack[skillindex].castTimer < _enemyanimationManager.skillattack[skillindex].castTime)
                {
                    _enemyanimationManager.skillattack[skillindex].castTimer += Time.deltaTime;
                    
                    GetComponent<Animation>().CrossFade(_enemyanimationManager.CastAnimationClip.name);
                }
                else {                  
                    Destroy(castEffect);
                    _canActiveSkill = false;
                    _setUpCastTimer = false;
                    _enemystate = EnemyState.ActiveSkillNeedCast;


                }

            }
        

        
        
        }


    //检测与摄像机的距离来判断是否显示血条
        void CheckDistanceToMainCamera() {
            DistanceToCamera = Vector3.Distance(this.transform.position,Camera.main.transform.position);
 
            if (DistanceToCamera > 30.0f)
            {
                HpBar.SetActive(false);
            }
            else {

                HpBar.SetActive(true);
            }
        
        
        }
        bool RandomIfSkillType() {
            //Random AttackType and Index
            int typeOfAttack = Random.Range(0, 10);
            if (typeOfAttack > 3 )
            {
                if (_enemyanimationManager.normalAttack.Count > 0)
                {
                    AttackType = 0;
                    AttackTypeIndex = Random.Range(0, _enemyanimationManager.normalAttack.Count);
                    return false;
                }
                else
                {
                    AttackType = 1;
                    AttackTypeIndex = Random.Range(0, _enemyanimationManager.skillattack.Count);
                    return true;
                }
             
            }
            else 
            {
                if (_enemyanimationManager.skillattack.Count > 0)
                {
                    AttackType = 1;
                    AttackTypeIndex = Random.Range(0, _enemyanimationManager.skillattack.Count);
                    return true;
                }
                else {
                    AttackType = 0;
                    AttackTypeIndex = Random.Range(0, _enemyanimationManager.normalAttack.Count);
                    return false;
                }

            }
       
        
        }

        void UpdateMovingToNextPatrolPoint() {
            destinationDistance = Vector3.Distance(destinationPosition, this.transform.position); //Check Distance Spawnpoint to Enemy
            if (destinationDistance < _enemyStatus._enemyStatus.AtkRange)
            {

                timeToWaitBeforeNextMove = Random.Range(delayNextTargetMin, delayNextTargetMax);//随机出等待时间
                _enemymoveAroundBehaviorstate = MoveAroundBehaviorState.Waiting;
                moveSpeed = 0;
                _enemyanimationManager.enemyanimationState = _enemyanimationManager.Idle;
                
            }

            // Move to destination
            if (  _enemyanimationManager.enemyanimationState == _enemyanimationManager.Move)//动画为move的一些操作
            {

                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, Time.deltaTime * 5);

                if (_enemycharactercontroller.isGrounded)
                {
                    movedir = Vector3.zero;
                    movedir = transform.TransformDirection(Vector3.forward * moveSpeed);//移动向量由怪物自身的属性控制
                }

            }
            else
            {
                movedir = Vector3.Lerp(movedir, Vector3.zero, Time.deltaTime * 10);//把移动向量插值变化为0
            }
            movedir.y -= Gravity * Time.deltaTime;
            _enemycollisionflags=_enemycharactercontroller.Move(movedir * Time.deltaTime);
        }
        void UpdateWaitingForNextMove() {
            timeToWaitBeforeNextMove -= Time.deltaTime;
            if (timeToWaitBeforeNextMove < 0.0f)
            {
                RandomPosition();
                _enemymoveAroundBehaviorstate = MoveAroundBehaviorState.MoveToNext;           
                _enemyanimationManager.enemyanimationState = _enemyanimationManager.Move;
                 moveSpeed =  _enemyStatus._enemyStatus.Movespd;//怪物速度
            }
        
        }
        void EnemyMovementBattle() {
            if (chaseTarget == true)//如果有追赶目标
            {
                LookAtTarget(targetPlayer.transform.position);//忽略y轴旋转自身
                targetPos = targetPlayer.transform.position;
                targetPos.y = transform.position.y;


                destinationDistance = Vector3.Distance(targetPos,this.transform.position); //Check Distance Enemy to Hero
                

                if (destinationDistance <= _enemyStatus._enemyStatus.AtkRange)//小于等于攻击距离时
                {		// Reset speed to 0
                    _enemystate = EnemyState.WaitAttack;
                     moveSpeed = 0;
                }
                else if (destinationDistance > _enemyStatus._enemyStatus.AtkRange)
                {			// To Reset Speed to default
                 
                _enemyanimationManager.enemyanimationState = _enemyanimationManager.Move;
                 moveSpeed = _enemyStatus._enemyStatus.Movespd;
                   
                }
            }
            else if (chaseTarget == false)
            {
                LookAtTarget(spawnPoint);

                destinationDistance = Vector3.Distance(spawnPoint, this.transform.position); //得到出生点与自身的距离

                if (destinationDistance <= (_enemyStatus._enemyStatus.AtkRange-6))
                {		// Reset speed to 0
         
                _enemyanimationManager.enemyanimationState = _enemyanimationManager.Idle;

                    moveSpeed = 0;

                    returnPhase = defaultReturnPhase;

                    if (enableRegen)//能回复生命，全部？
                    {
                        if (regenHP)
                            _enemyStatus._enemyStatus.Hp = Mathf.FloorToInt(defaultHP);

                        if (regenMP)
                            _enemyStatus._enemyStatus.Mp = Mathf.FloorToInt(defaultMP);

                        destinationPosition = this.transform.position;
                        targetPlayer = null;
                    }
                }
                else if (destinationDistance > _enemyStatus._enemyStatus.AtkRange)
                {			// To Reset Speed to default

                  //  if (_enemyanimationManager.enemyanimationState == _enemyanimationManager.Move || _enemyanimationManager.enemyanimationState == _enemyanimationManager.Idle||_enemyanimationManager.enemyanimationState==_enemyanimationManager.Attack)
                        _enemyanimationManager.enemyanimationState = _enemyanimationManager.Move;
                    moveSpeed = _enemyStatus._enemyStatus.Movespd;
                }
            }

            //Check distance spawn
            //float distanceSpawn = Vector3.Distance(spawnPoint, this.transform.position);
            //if (distanceSpawn > returnPhase)//当离家距离大于约定值时，返回
            //{
            //    //if (regenHP || regenMP)
            //    //    enableRegen = true;

            //    chaseTarget = false;
            //    targetPlayer = null;

            //    //returnPhase += 3;//??什么鬼
            //}


            // Move to destination
            if (_enemyanimationManager.enemyanimationState==_enemyanimationManager.Move)
            {

                if (_enemycharactercontroller.isGrounded)
                {
                    movedir = Vector3.zero;
                    movedir = transform.TransformDirection(Vector3.forward * moveSpeed);
                }

            }
            else
            {
                movedir = Vector3.Lerp(movedir, Vector3.zero, Time.deltaTime * 10);
            }
            movedir.y -= Gravity * Time.deltaTime;
            _enemycollisionflags= _enemycharactercontroller.Move(movedir * Time.deltaTime);
        
        }
        void RandomPosition() {
            randomAngle = Random.Range(0f, 360f);
            randomMoveVector.x = Mathf.Sin(randomAngle) * movePhase + spawnPoint.x;
            randomMoveVector.z = Mathf.Cos(randomAngle) * movePhase + spawnPoint.z;
            randomMoveVector.y = transform.position.y;        
            targetRotation = Quaternion.LookRotation(randomMoveVector - transform.position);
            destinationPosition = randomMoveVector;

        
        }
        void LookAtTarget(Vector3 _targetPos)
        {
            targetPos.x = _targetPos.x;
            targetPos.y = this.transform.position.y;
            targetPos.z = _targetPos.z;
            this.transform.LookAt(targetPos);
        }     
        void SetDefaultColor() { }

        //检查是否暴击（自身暴击率-随机数）
        bool CriticalCal(float criticalStat)
        {
            float calCritical = criticalStat - Random.Range(0, 101f);

            if (calCritical >=0)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }

      


     //怪物受攻击伤害处理
     //参数（攻击伤害，命中率，僵直值，被击效果，被击声音）
       public void EnemyGetDamage(float targetAttack, float targetHit, float flichRate,GameObject skillowner) {
      ;
        PlayerStatus skillownerstatus=skillowner.GetComponent<PlayerStatus>();
       targetHit=targetHit+skillownerstatus.statusCal.HitRate - _enemyStatus._enemyStatus.MissRate+Random.Range(-100,0);
        //Calculate Hit
	
		
		//随机受到攻击的动画
		typeTakeAttack = Random.Range(0,_enemyanimationManager.takeAttack.Count);
		
		if(targetHit< 0) //如果闪避率大于目标命中率
		{
            //TODO miss声效

            AddFloatingText(this.transform.position, "Miss");
            if(!chaseTarget)
		    chaseTarget = true;
            _enemyanimationManager.enemyanimationState = _enemyanimationManager.Idle;
			
		}
        
        else
		{
		int damage = Mathf.FloorToInt((targetAttack*skillownerstatus.statusCal.Atk - _enemyStatus._enemyStatus.Def ) * Random.Range(0.8f,1.2f));//伤害数值计算（策划）
			
		if(damage <= 5)
		{
		damage = Random.Range(1,11); 
		}


          AddFloatingText(this.transform.position, damage.ToString());
          _enemyStatus._enemyStatus.Hp -= damage;

		//	GetDamageColorReset();

            if (_enemyStatus._enemyStatus.Hp <= 0)//死亡处理
			{
                _enemyStatus._enemyStatus.Hp = 0;
				
				
			   if(skillowner)//target指player
			   {
						
		       skillownerstatus.status.Exp +=_enemyStatus._expGive;//给经验
			  //AddKillCountQuest();//任务TODO
               }
				
			  Destroy(this.gameObject,deadTimer);
              _enemyanimationManager.enemyanimationState = _enemyanimationManager.Death;
		   } else
			{
				if(!chaseTarget)
					chaseTarget = true;
				
				flinchValue -= flichRate;//flinchValue指的是硬直值，当硬直值大于技能硬直值时不会打断聚气或播放受攻击动画
				
				if(flinchValue <= 0)
				{
                    Debug.Log("takedamage");
                    targetPlayer = skillowner;
                    _enemyanimationManager.enemyanimationState = _enemyanimationManager.TakeAttack;
                    _enemystate = EnemyState.Search;
					flinchValue = 100;//重置硬直值
				}else
				{
                   
                    targetPlayer = skillowner;
                    
				}
				
			}
			
		}

        }


        void ResetEnemyState() { }

    //????
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Random move again when collision collider
            if (hit.gameObject.tag == "Collider" && _enemyBehavior == EnemyBehavior.MoveAround && !chaseTarget && !targetPlayer)
            {
             //   _enemyanimationManager._enemyanimationstate = EnemyAnimationManager.EnemyAnimationState.Idle;
                _enemyanimationManager.enemyanimationState = _enemyanimationManager.Idle;
                _enemymoveAroundBehaviorstate = MoveAroundBehaviorState.Waiting;
            }
        }

      



}









