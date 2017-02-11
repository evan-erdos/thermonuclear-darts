/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2015-07-07 */

using System;
using System.Collections;
using UnityEngine;

public class CameraFade : MonoBehaviour {
    GUIStyle style = new GUIStyle();
    Texture2D fadeTexture;
    Color currentColor = new Color(0,0,0,0);
    Color targetColor = new Color(0,0,0,0);
    Color deltaColor = new Color(0,0,0,0);
    int depth = -1000;
    float delay = 0;
    Action then;

    static CameraFade singleton = null;
    static CameraFade Instance {
        get {
            if (singleton==null) {
                singleton = GameObject.FindObjectOfType(
                    typeof(CameraFade)) as CameraFade;
                if (singleton==null)
                    singleton = new GameObject("CameraFade")
                        .AddComponent<CameraFade>();
            } return singleton;
        }
    }

    void Awake() {
        if (singleton!=null) return;
        singleton = this as CameraFade;
        Instance.Init();
    }


    public void Init() {
        Instance.fadeTexture = new Texture2D(1,1);
        Instance.style.normal.background = Instance.fadeTexture;
    }

    void OnGUI() {
        if (Time.time > Instance.delay) {
            if (Instance.currentColor!=Instance.targetColor) {
                if (Mathf.Abs(Instance.currentColor.a-Instance.targetColor.a)
                        <Mathf.Abs(Instance.deltaColor.a)*Time.deltaTime) {
                    Instance.currentColor = Instance.targetColor;
                    SetScreenOverlayColor(Instance.currentColor);
                    Instance.deltaColor = new Color(0,0,0,0);
                    if (Instance.then != null)
                        Instance.then();
                    Die();
                } else SetScreenOverlayColor(
                    Instance.currentColor+
                    Instance.deltaColor * Time.deltaTime);
            }
        } if (currentColor.a > 0) {
            GUI.depth = Instance.depth;
            GUI.Label(new Rect(-10,-10,Screen.width+10,Screen.height+10),
                Instance.fadeTexture,Instance.style);
        }
    }

    public static void SetScreenOverlayColor(Color colorNew) {
        if (!Instance.fadeTexture) return;
        Instance.currentColor = colorNew;
        Instance.fadeTexture.SetPixel(0, 0, Instance.currentColor);
        Instance.fadeTexture.Apply();
    }

    public static void StartAlphaFade(
                    Color colorNew,
                    bool isFade,
                    float timeFade) {
        if (timeFade > 0f) {
            if (isFade) {
                Instance.targetColor = new Color(
                    colorNew.r,colorNew.g,colorNew.b,0);
                SetScreenOverlayColor(colorNew);
            } else {
                Instance.targetColor = colorNew;
                SetScreenOverlayColor(
                    new Color(colorNew.r,colorNew.g,colorNew.b,0));
            } Instance.deltaColor =
                (Instance.targetColor - Instance.currentColor) / timeFade;
        } else SetScreenOverlayColor(colorNew);
    }

    public static void StartAlphaFade(
                    Color colorNew,
                    bool isFade,
                    float timeFade,
                    float fadeDelay) {
        if (timeFade > 0f) {
            Instance.delay = Time.time + fadeDelay;
            if (isFade) {
                Instance.targetColor = new Color(
                    colorNew.r,colorNew.g,colorNew.b,0);
                SetScreenOverlayColor(colorNew);
            } else {
                Instance.targetColor = colorNew;
                SetScreenOverlayColor(new Color(
                    colorNew.r,colorNew.g,colorNew.b,0));
            } Instance.deltaColor =
                (Instance.targetColor - Instance.currentColor) / timeFade;
        } else SetScreenOverlayColor(colorNew);
    }

    public static void StartAlphaFade(
                                    Color colorNew,
                                    bool isFade,
                                    float timeFade,
                                    float fadeDelay,
                                    Action OnFadeFinish) {
        if (timeFade>0f) {
            Instance.then = OnFadeFinish;
            Instance.delay = Time.time + fadeDelay;
            if (isFade) {
                Instance.targetColor = new Color(
                    colorNew.r, colorNew.g, colorNew.b, 0 );
                SetScreenOverlayColor(colorNew);
            } else {
                Instance.targetColor = colorNew;
                SetScreenOverlayColor( new Color(
                    colorNew.r, colorNew.g, colorNew.b, 0 ) );
            } Instance.deltaColor =
                (Instance.targetColor - Instance.currentColor) / timeFade;
        } else SetScreenOverlayColor(colorNew);
    }

    void Die() { singleton = null; Destroy(gameObject); }

    void OnApplicationQuit() { singleton = null; }
}
