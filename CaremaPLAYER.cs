using UnityEngine;
using System.Collections;

public class CaremaPLAYER : MonoBehaviour {
    //观察目标
    private Transform Target;
    //观察距离
    public float Distance = 5F;
    //旋转速度
    private float SpeedX = 10F;
    private float SpeedY = 10F;
    //角度限制
    private float MinLimitY = 1;
    private float MaxLimitY = 75;

    //旋转角度
    private float mX = 0.0F;
    private float mY = 0.0F;

    //鼠标缩放距离最值
    private float MaxDistance = 10;
    private float MinDistance = 1.5F;
    //鼠标缩放速率
    private float ZoomSpeed = 2F;

    private Transform _mTransform;
    private bool CamButtonDown = false;


    public bool isNeedDamping = true;
    public float Damping = 2.5F;
    public Vector3 offset;
    public float Height;
    public float HeightDamping;
    public float RotateDamping;



    void Awake() {
        _mTransform = transform;
    }
    void Start()
    {
       
        Target = GameObject.FindGameObjectWithTag("PlayerN").transform;
        if (Target == null)
        {
            Debug.LogWarning("Don't have a target for Camare");
        }
        else {
            CameraSetup();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CamButtonDown = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            CamButtonDown = false;
        }

    }


    void LateUpdate()
    {

        if (Target != null) {
            //鼠标右键旋转
            if (Target != null && Input.GetMouseButton(1))
            {


                //获取鼠标输入
                mX += Input.GetAxis("Mouse X") * SpeedX;
                mY -= Input.GetAxis("Mouse Y") * SpeedY;
                //范围限制
                mY = Mathf.Clamp(mY, MinLimitY, MaxLimitY);
                mX = ClampAngle(mX);


            }
            #region
            //  else {
          //   //   mY = 0;
          ////      mX = 0;

          //      float wantedRotateAngle = Target.eulerAngles.y;
          //      float wantedRotateX = Target.eulerAngles.x;
          //      float wantededHeight = Target.position.y+Height;

          //      float currentRotateAngle = transform.eulerAngles.y;
          //      float currentHeight = transform.position.y;
                

          //      currentRotateAngle = Mathf.LerpAngle(currentRotateAngle, wantedRotateAngle, RotateDamping * Time.deltaTime);
          //      currentHeight = Mathf.Lerp(currentHeight, wantededHeight, HeightDamping * Time.deltaTime);
              

          //      Quaternion currentRotation=Quaternion.Euler(0,currentRotateAngle,0);

          //      _mTransform.position = Target.position - currentRotation * Vector3.forward * Distance;
          //      _mTransform.position = new Vector3(_mTransform.position.x, currentHeight, _mTransform.position.z);
          //      transform.position = Vector3.Lerp(transform.position, _mTransform.position, Damping * Time.deltaTime);
          //     _mTransform.LookAt(Target);

            //  }
            #endregion

            //重新计算位置和角度
            Quaternion mRotation = Quaternion.Euler(mY, mX, 0);
            Vector3 mPosition = mRotation * new Vector3(0.0F, 0.0F, -Distance) + Target.position + offset;


            //设置相机的角度和位置
            if (isNeedDamping)
            {
                
                transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);               
                transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
            }
            else
            {
                transform.rotation = mRotation;
                transform.position = mPosition;
            }

            //鼠标滚轮缩放

            Distance -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

          
        }
       
    }

    private float ClampAngle(float angle)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return angle;
    }

    private void CameraSetup() {
        _mTransform.position = new Vector3(Target.position.x, Target.position.y, Target.position.z-Distance)+offset;
        _mTransform.LookAt(Target);
    
    }
}
