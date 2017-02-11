/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2011-05-21 */

using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
   static GameObject management;
   protected static T instance;
   public static T Instance {
      get {
         if (!(instance is null)) return instance;
         instance = FindObjectOfType<T>();
         if (!(instance is null)) return instance;
         if (management is null) management = GameObject.Find($"Management");
         if (management is null) management = new GameObject("Management");
         instance = management.GetComponent<T>();
         if (instance is null) instance = management.AddComponent<T>();
         return instance;
      }
   }
}
