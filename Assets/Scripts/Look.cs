using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

class Look : MonoBehaviour {
    [SerializeField] bool recenter;
    uint ind;
    const int size = 8;
    float ratio;
    [SerializeField] float speed = 2.0f;
    enum rotAxes { MouseXY, MouseX, MouseY }
    [SerializeField] rotAxes rotAxis = rotAxes.MouseXY;
    Vector2 Sensitivity, cr, avg, input;
    Vector2[] rarr = new Vector2[(int) size];
    Vector3 pr;
    Vector4 Maxima;
    Quaternion dr, lr;

    void Awake() {
        ratio = Screen.width/Screen.height;
        Maxima.Set(-360f,360f,-60f,60f);
        lr = transform.localRotation;
        if (GetComponent<Rigidbody>() && !recenter)
            GetComponent<Rigidbody>().freezeRotation = true;
        Sensitivity.Set(speed*ratio, speed);
    }

    void Update() {
        input = new Vector2(
            CrossPlatformInputManager.GetAxis("Horizontal")*Sensitivity.x,
            CrossPlatformInputManager.GetAxis("Vertical")*Sensitivity.y);
    }


    void FixedUpdate() {
        if (recenter) cr.Set(cr.x*0.5f,cr.y*0.5f);
        avg.Set(0f,0f);
        cr.x += input.x;
        cr.y += input.y;
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
