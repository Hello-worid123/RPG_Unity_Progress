using UnityEngine;
using System.Collections;


//首先要建一个空物体
public class HpBar : MonoBehaviour {
 //   public Transform _hpbarPos;
 //   public Transform _hpBarUI;
 //   public Transform _character;
  //  public float _fomat;
    public Transform Target;
    public Camera UICamera;
    public Camera MainCamera;

    public bool disableIfInvisible = true;

    Transform mTrans;
    bool mIsVisible = false;

    /// <summary>
    /// Cache the transform;
    /// </summary>

    void Awake() { mTrans = transform; }

    /// <summary>
    /// Find both the UI camera and the game camera so they can be used for the position calculations
    /// </summary>

    void Start()
    {
        if (Target != null)
        {
            if (MainCamera == null) MainCamera = NGUITools.FindCameraForLayer(Target.gameObject.layer);//??
            if (UICamera== null) UICamera = NGUITools.FindCameraForLayer(gameObject.layer);
            SetVisible(false);//把下面的sprite等设置为false
        }
        else
        {
            Debug.LogError("Expected to have 'target' set to a valid transform", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Enable or disable child objects.
    /// </summary>

    void SetVisible(bool val)
    {
        mIsVisible = val;

        //遍历mtrans的子
        for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
        {
            NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
        }
    }

    /// <summary>
    /// Update the position of the HUD object every frame such that is position correctly over top of its real world object.
    /// </summary>

    void Update()
    {
        Vector3 pos = MainCamera.WorldToViewportPoint(Target.position);
      //  Transforms position from world space into viewport space.

      //从世界空间到视窗空间的变换位置。

     //Viewport space is normalized and relative to the camera. The bottom-left of the camera is (0,0); the top-right is (1,1). The z position is in world units from the camera.

     //视口空间是归一化的并相对于相机的。相机的左下为（0,0）；右上是（1,1），Z的位置是以世界单位衡量的到相机的距离。

        // Determine the visibility and the target alpha
        bool isVisible = (MainCamera.orthographic || pos.z > 0f) && (!disableIfInvisible || (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f));

        // Update the visibility flag
        if (mIsVisible != isVisible) SetVisible(isVisible);

        // If visible, update the position
        if (isVisible)
        {
            
            transform.position = UICamera.ViewportToWorldPoint(pos);
            pos = mTrans.localPosition;
            pos.x = Mathf.FloorToInt(pos.x);
            pos.y = Mathf.FloorToInt(pos.y);
            pos.z = 0f;
            mTrans.localPosition = pos;
        }
        OnUpdate(isVisible);
    }

    /// <summary>
    /// Custom update function.
    /// </summary>

    protected virtual void OnUpdate(bool isVisible) { }


}
