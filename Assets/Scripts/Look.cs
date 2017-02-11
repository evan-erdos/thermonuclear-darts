/* Ben Scott * bescott@andrew.cmu.edu * 2015-07-11 * Look */

using UnityEngine;
using System.Collections;

class Look : MonoBehaviour {
    [SerializeField] bool recenter, usePrev, inControl = true;
    uint ind, size = 8;
    float ratio;
    public float speed = 2f;
    public enum rotAxes { MouseXY, MouseX, MouseY }
    public rotAxes rotAxis = rotAxes.MouseXY;
    Vector2 Sensitivity, cr, avg; // current rotation, avg
    Vector2[] rarr; // rotation array
    Vector3 pr; // previous rotation
    Vector4 Maxima;
    Quaternion dr, lr; // delta rotation, last rotation


    void Awake() {
        ratio = Screen.width/Screen.height;
        rarr = new Vector2[(int)size];
        Maxima.Set(-360f,360f,-90f,90f);
        lr = transform.localRotation;
        if (GetComponent<Rigidbody>() && !recenter)
            GetComponent<Rigidbody>().freezeRotation = true;
        Sensitivity.Set(speed*ratio, speed);
    }

    void FixedUpdate() {
        if (!inControl) return;// || (am && am.isPlaying)) return;
        if (Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        pr = Vector3.zero;
        //(usePrev)?transform.localEulerAngles:Vector3.zero;
        if (recenter) cr.Set(cr.x*0.5f,cr.y*0.5f);
        avg.Set(0f,0f);
        cr.x += Input.GetAxis("Mouse X")*Sensitivity.x;
        cr.y += Input.GetAxis("Mouse Y")*Sensitivity.y;
        cr.y = ClampAngle(cr.y, Maxima.z, Maxima.w);
        rarr[(int)ind] = cr;
        foreach (var elem in rarr) avg += elem;
        avg /= (int)size;
        avg.y = ClampAngle(avg.y, Maxima.z, Maxima.w);
        switch (rotAxis) {
            case rotAxes.MouseXY :
                dr = Quaternion.Euler(-avg.y,avg.x,pr.z); break;
            case rotAxes.MouseX :
                dr = Quaternion.Euler(-pr.y,avg.x,pr.z); break;
            case rotAxes.MouseY :
                dr = Quaternion.Euler(-avg.y,pr.x,pr.z); break;
        } ind++;
        transform.localRotation = lr*dr;
        if ((int)ind >= (int)size) ind -= size;
    }

    static float ClampAngle(float delta, float maximaL, float maximaH) {
        delta %= 360f;
        if (delta<=-360f) delta += 360f;
        else if (delta>=360f) delta -= 360f;
        return Mathf.Clamp(delta, maximaL, maximaH);
    }
}
