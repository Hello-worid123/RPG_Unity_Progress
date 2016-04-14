using UnityEngine;
using System.Collections;

public enum _ClassType
{
    None,
    Magican,
    Warrior
}
public class AdvanceMovement : MonoBehaviour {
    private static AdvanceMovement _instance;
    public static AdvanceMovement Instance {
        get {
            return _instance;
        }
    }

    private float RotateSpeed = 80.0f;


    public enum _Turn { 
    Left=-1,
    None=0,
    Right=1
    }
    public enum _Forward { 
    Forward=1,
    None=0,
    Back=-1
    }
    public enum _State { 
    Idle,
    Init,
    Setup,
    Run,
    Cast,
    ActiveSkill,
    Attack
    
    
    }

    public GameObject target;     //敌人目标

   

    private CollisionFlags _collisionflags;
    private CharacterController _charactercontroller;
    private SkillManager _skillmanager;
    private AnimationManager _animationmanager;
    private Animation _anim;
    private PlayerStatus _playerstatus;
    private Transform _myTransform;

    private Vector3 _myDirection;
    private float gravity = 10;

    private float airtime = 0;
    private float falltime = 0.5f;

    private float jumpTime = 1.5f;
    private float jumpHeight = 3.5f;

    private Vector3 targetDirection = Vector3.zero;
    private Vector3 MoveDirection = Vector3.zero;
    

    private _Turn _turn;
    private _Forward _forward;
    private _Turn _strafe;

    public float SpeedMultWhileAttack = 0.5f; // Move speed while attaking
    private float speedMoveMult;
  
    public bool _isRun;
    private bool _isJump;
    [HideInInspector]
    public int _castid;
    [HideInInspector]
    public bool isUseSkill;
    public GameObject FloatingText;
    private float flinchvalue;

    public _State _state;

    [HideInInspector]
    public float Slip = 5;
    public float _rotateSpeed = 250f;
    public float _moveSpeed;
    public float _runSpeed = 8.0f;
    public float _sidewalkSpeed = 1.0f;
    public float TurnSpeed = 10; // turning speed
    public bool hited;
    public bool attacking;
    public float Speed = 1; // Move speed

    void Awake() {
        _instance = this;
        _charactercontroller = this.GetComponent<CharacterController>();
        _skillmanager = this.GetComponent<SkillManager>();
        _animationmanager = this.GetComponent<AnimationManager>();
        _anim = this.GetComponent<Animation>();
        _playerstatus = this.GetComponent<PlayerStatus>();

      //  _state = _State.Init;
    
    }


   IEnumerator  Start() {
       while (true)
       {
           switch (_state)
           {
               case _State.Init:
                   Init();
                   break;
               case _State.Setup:
                   SetUp();
                   break;
               case _State.Run:
                   Action() ;
                   break;
               case _State.Cast:
                   Cast();
                   break;
               case _State.ActiveSkill:
                   ActiveSkill();
                   break;
               case _State.Attack:
                   Attack();
                   break;
               default:
                   break;

           }
           yield return null;
       }
        
    }

    void Init() {
        if (!GetComponent<CharacterController>()) return;
        if (!GetComponent<Animation>()) return;
        if (!GetComponent<PlayerStatus>()) return;

        _state = _State.Setup;
     
    }

    private void SetUp() {
        _turn = AdvanceMovement._Turn.None;
        _forward = AdvanceMovement._Forward.None;
        _strafe = AdvanceMovement._Turn.None;
        _isJump = false;
        _isRun =false;
        _moveSpeed = _playerstatus.statusCal.Movespd;//将人物属性的速度赋值
        isUseSkill = false;
      

        _myDirection = Vector3.zero;
        _myTransform = transform;
        _anim.wrapMode = WrapMode.Loop;
        _anim["jump"].layer = 1;
        _anim["jump"].wrapMode = WrapMode.Once;
        _anim.Play("idle");

        _state = _State.Run;
    }

    private void OnBtnSkill(int castid) {
        if (!isUseSkill)
        {
            isUseSkill = true;
            _state = _State.Cast;
            _castid = castid;
        }


    }
    private void Attack() { }

    private void ActiveSkill() {
        _animationmanager.checkAttack = false;
        _animationmanager.ActiveSkill();
        
    
    }
    private void Cast() {       
      _skillmanager.CastSkill(_skillmanager.FindSkillType(_castid), _skillmanager.FindSkillIndex(_castid));
              
    }
 
    private void ActionPicker() {
        
        
     transform.Rotate(0, ((int)_turn) * RotateSpeed * Time.deltaTime, 0);
   
        if (_charactercontroller.isGrounded)
        {
          
           
            _myDirection = new Vector3((int)_strafe, 0, (int)_forward);
           
            _myDirection = _myTransform.TransformDirection(_myDirection).normalized;
            
            _myDirection *= _moveSpeed;
            

            if (_forward != _Forward.None)
            {
               

                if (_isRun)
                {
                    _myDirection *= 2;
                    Run();
                }
                else
                {
                    
                    Walk();
                }

            }
            else
            {
                Idle();
            }
            if (_strafe!=_Turn.None)
            {
                if (_isRun)
                {
                    _myDirection *= 2;
                    Run();
                }
                else
                {

                    Walk();
                }
            }

            airtime = 0;
            if (_isJump)
            {
                if (airtime < jumpTime)
                {
                    _myDirection.y += jumpHeight;
                    Jump();
                }
            }


        }
        else
        {
            if ((_collisionflags & CollisionFlags.CollidedBelow) == 0)
            {
                _isJump = false;
                airtime += Time.deltaTime;
                if (airtime > falltime)
                {
                    fall();
                }
            }

        }

        _myDirection.y -= gravity * Time.deltaTime;
        _collisionflags = _charactercontroller.Move(_myDirection * Time.deltaTime);
    
    }

    private void Action() {
        var direction = Vector3.zero;
        if (_charactercontroller.isGrounded)
        {
            

            var forward = Quaternion.AngleAxis(-90, Vector3.up) * Camera.main.transform.right;

            direction += forward * Input.GetAxis("Vertical");
            direction += Camera.main.transform.right * Input.GetAxis("Horizontal");

            direction.Normalize();
            if (direction.magnitude > 0.1f)
            {
                //transform.Rotate(0, Vector3.Angle(direction, forward), 0);
               var newRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * TurnSpeed);
                Run();
            }
            else {
                Idle();
            }
            direction *= Speed * (Vector3.Dot(transform.forward, direction) + 1) * 5;
            MoveDirection = Vector3.Lerp(MoveDirection, direction, Time.deltaTime * Slip);

            airtime = 0;
            if (_isJump)
            {
               //  Debug.Log(_isJump);
                if (airtime < jumpTime)
                {
                    MoveDirection.y += jumpHeight;
                    Jump();
                }
            }

    
        }
        else {

            if ((_collisionflags & CollisionFlags.CollidedBelow) == 0)
            {

                _isJump = false;

                airtime += Time.deltaTime;
                if (airtime > falltime)
                {
                    fall();
                }
            }
                   
        }
        MoveDirection.y -= gravity * Time.deltaTime;
        _collisionflags = _charactercontroller.Move(MoveDirection * Time.deltaTime);
          
    }
   

    public void MoveForward(_Forward z) {
        _forward = z;
    }
    public void Strafe(_Turn x) {
       _strafe = x;
    }
    public void IsJump( ) {
       
       _isJump = true;
       
    }
    public void IsRun(bool isRun) {
        _isRun = isRun;
    }

    public void Rotate(_Turn y) {
        _turn = y;
        
    }


    private void fall() {
        _anim.CrossFade("fall_a_loop");
    }

    public void Idle() {
        _anim.CrossFade("idle");
    }
    private void Run() {
        _anim.CrossFade("run");
    }

    private void Walk() {
        _anim.CrossFade("walk");
    }
    private void Jump() {
        _anim.CrossFade("jump");
    }
    private void Turn() {
        _anim.CrossFade("walk");
    }


    public void ResetState() {
        _moveSpeed = 0;
        _myDirection= Vector3.zero;
        target = null;
       
      //  alreadyLockSkill = false;
        Invoke("ResetCheckAttack", 0.1f);   
    }
    //public void ResetBeforeCast()
    //{
    //    _moveSpeed = 0;//移动速度为0
    //    _myDirection = Vector3.zero;	//vector3方向为0


    ///   Invoke("ResetCheckAttack", 0.1f);//把animation的checkattack设为false?
    //}


    void ResetCheckAttack() {
        AnimationManager._instance.checkAttack = false;
    }
    public void AddFloatingText(Vector3 pos, string text)
    {
        // Adding Floating Text Effect
        if (FloatingText)
        {
            var floattext = (GameObject)Instantiate(FloatingText, pos, transform.rotation);
            FloatingText floatingtext = floattext.GetComponent<FloatingText>();
            if (floatingtext)
            {
                floatingtext.Text = text;
            }
            GameObject.Destroy(floattext, 1);
        }
    }
    public void GetDamage(float targetAttack, float targetHit, float flinchRate, GameObject skillowner)
    {
        EnemyStatus enemystatus=skillowner.GetComponent<EnemyStatus>();
        //Calculate Hit
        targetHit = targetHit + enemystatus._enemyStatus.HitRate - _playerstatus.statusCal.MissRate + Random.Range(-100, 0);

        if (targetHit < 0) //Attack Miss
        {
            AddFloatingText(this.transform.position, "Miss");
           // SoundManager.instance.PlayingSound("Attack_Miss");

        }
        else
        {
            int damage = Mathf.FloorToInt((targetAttack*enemystatus._enemyStatus.Atk - _playerstatus.statusCal.Def) * Random.Range(0.8f, 1.2f));

            if (damage <= 5)
            {
                damage = Random.Range(1, 11); // if def < enemy attack
            }

            AddFloatingText(this.transform.position, damage.ToString());

            _playerstatus.statusCal.Hp -= damage;
           

            if (_playerstatus.statusCal.Hp <= 0)
            {
               // playerSkill.CastBreak();
                _playerstatus.statusCal.Hp = 0;
               // ctrlAnimState = ControlAnimationState.Death;
            }
            else
            {
                flinchvalue -= flinchRate;

                if (flinchvalue <= 0)
                {
                    ////if (ctrlAnimState == ControlAnimationState.Cast || ctrlAnimState == ControlAnimationState.ActiveSkill)
                    ////    playerSkill.CastBreak();

                    ////ctrlAnimState = ControlAnimationState.TakeAtk;
                    ////flinchValue = 100;
                    ////playerSkill.oneShotResetTarget = false;
                }

            }
        }
    }

   

 
}
