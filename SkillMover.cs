using UnityEngine;
using System.Collections;

public class SkillMover : MonoBehaviour {
    public float Speed = 1;
    public Vector3 Noise = Vector3.zero;
    public float Damping = 0.3f;
    Quaternion direction;

    void Start()
    {
        direction = Quaternion.LookRotation(this.transform.forward * 1000);
        this.transform.Rotate(new Vector3(Random.Range(-Noise.x, Noise.x), Random.Range(-Noise.y, Noise.y), Random.Range(-Noise.z, Noise.z)));//自身旋转
    }

    void LateUpdate()
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, direction, Damping);
        this.transform.position += this.transform.forward * Speed * Time.deltaTime;//向前移动
    }
}
