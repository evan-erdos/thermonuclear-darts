/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2017-01-01 */

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// Extensions
/// a collection of helpful little snippets
public static partial class Extensions {

    /// IsNear : (string) => String
    /// capitalizes strings
    public static string Capitalize(this string s) =>
        s.FirstOrDefault().ToString().ToUpper()+s.Substring(1);

    /// IsNear : () => bool
    /// detects if the transform is close to the location
    public static bool IsNear(
                    this Transform transform,
                    Transform location,
                    float distance=0.01f) =>
        transform.IsNear(location.position,distance);

    /// IsNear : () => bool
    /// detects if the transform is close to the location
    public static bool IsNear(
                    this Transform transform,
                    Vector3 position,
                    float distance=0.01f) =>
        Vector3.Distance(transform.position,position)<distance;

    /// IsNear : () => bool
    /// detects if the vector is close to the location
    public static bool IsNear(this Vector3 o,
                    Vector3 vector,
                    float distance=float.Epsilon) =>
        (o-vector).sqrMagnitude<distance*distance;

    public static bool IsNear(this (float,float,float) o,
                    Vector3 v,
                    float dist=float.Epsilon) =>
        v.IsNear(new Vector3(o.Item1, o.Item2, o.Item3),dist);

    /// Distance : () => real
    /// finds distance between transforms
    public static float Distance(this Transform o, Transform other) =>
        (o.position-other.position).magnitude;

    /// ToColor : (int) => color
    /// convert a number to a color
    public static Color32 ToColor(this int n) => new Color32(
        r: (byte) ((n>>16)&0xFF),
        g: (byte) ((n>>8)&0xFF),
        b: (byte) (n&0xFF),
        a: (byte) (0xFF));

    /// ToColor : (string) => color
    /// convert a string to a color
    public static Color32 ToColor(this string s) => new Color32(
        r: (byte)((System.Convert.ToInt32(s.Substring(1,3),16)>>16)&0xFF),
        g: (byte)((System.Convert.ToInt32(s.Substring(4,6),16)>>8)&0xFF),
        b: (byte)(System.Convert.ToInt32(s.Substring(7,9),16)&0xFF),
        a: (0xFF));

    /// ToInt : (color) => int
    /// convert a color to a number
    public static int ToInt(this Color32 color) =>
        (byte)((color.r>>16)&0xFF) +
        (byte)((color.g >> 8)&0xFF) +
        (byte)(color.b & 0xFF) + (0xFF);

    /// Ellipsis : (string) => string...
    /// shortens a string to length len, and appends ellipsis
    public static string Ellipsis(this string s, int len=100) {
        return (s.Length<len) ? s : s.Substring(0,len-1)+"…"; }

    /// Hide : (Camera, layer) => void
    /// removes one layer from the culling mask
    public static void Hide(this Camera o, string s="Default") =>
        o.cullingMask &= ~(1<<LayerMask.NameToLayer(s));

    /// Show : (Camera, layer) => void
    /// adds one layer to the culling mask
    public static void Show(this Camera o, string s="Default") =>
        o.cullingMask |= 1<<LayerMask.NameToLayer(s);

    /// Add : (T[]) => void
    /// I just don't like that AddRange is a different name than Add
    public static void Add<T>(this List<T> o, params T[] a) => o.AddRange(a);
    public static void Add<T>(this List<T> o, IEnumerable<T> a) => o.AddRange(a);

    /// Many : (T[]) => bool
    /// true if the collection has more than one element
    public static bool Many<T>(this IEnumerable<T> list) {
        var enumerator = list.GetEnumerator();
        return enumerator.MoveNext() && enumerator.MoveNext();
    }

    /// ForEach : (T[]) => void
    /// applies a function to each element of a builtin array
    public static void ForEach<T>(this T[] list, Action<T> func) {
        foreach (var item in list) func(item); }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> func) {
        foreach (var item in list) func(item); }

    public static T Pick<T>(this IList<T> list) =>
        (list.Count==0) ? default(T) : list[new System.Random().Next(list.Count)];

    /// GetTypes : (type) => type[]
    /// gets a list of all parent types on a given type
    public static IEnumerable<Type> GetTypes(this Type type) {
        if (type == null || type.BaseType == null) yield break;
        foreach (var i in type.GetInterfaces()) yield return i;
        var currentBaseType = type.BaseType;
        while (currentBaseType != null) {
            yield return currentBaseType;
            currentBaseType = currentBaseType.BaseType;
        }
    }

    /// GetTypes : (type) => type[]
    /// gets a list of all parent types, up to a certain superclass
    public static IEnumerable<Type> GetTypes(this Type type, Type root=null) {
        if (type==null || type.BaseType == null) yield break;
        if (root==null) root = typeof(object);
        var current = type;
        while ((current = current.BaseType)!=null)
            if (current==root.BaseType) yield break;
            else yield return current;
    }

    /// md : (markdown) => html
    /// Adds Markdown formatting capability to any string.
    /// Formats the Markdown syntax into HTML.
    /// Currently removes all <p> tags.
    public static string md(this string s) => new StringBuilder(Markdown.Transform(s))
        .Replace("<em>","<i>").Replace("</em>","</i>")
        .Replace("<blockquote>","<i>").Replace("</blockquote>","</i>")
        .Replace("<strong>","<b>").Replace("</strong>","</b>")
        .Replace("<h1>", $"<size={24}><color=#{0x98C8FC:X}>")
        .Replace("</h1>", "</color></size>")
        .Replace("<h2>", $"<size={18}><color=#{0x98C8FC:X}>")
        .Replace("</h2>", "</color></size>")
        .Replace("<h3>", $"<size={16}><color=#{0x98C8FC:X}>")
        .Replace("</h3>","</color></size>")
        .Replace("<h4>", $"<size={14}><color=#{0x98C8FC:X}>")
        .Replace("</h4>","</color></size>")
        .Replace("<pre>","").Replace("</pre>","")
        .Replace("<code>","").Replace("</code>","")
        .Replace("<ul>","").Replace("</ul>","")
        .Replace("<li>","").Replace("</li>","")
        .Replace("<p>","").Replace("</p>","")
        .Replace("<warn>", $"<color=#{0xFA2363:X}>").Replace("</warn>", $"</color>")
        .Replace("<cost>", $"<color=#{0xFFDBBB:X}>").Replace("</cost>", $"</color>")
        .Replace("<item>", $"<color=#{0xFFFFFF:X}>").Replace("</item>", $"</color>")
        .Replace("<cmd>", $"<color=#{0xAAAAAA:X}>").Replace("</cmd>", $"</color>")
        .Replace("<life>", $"<color=#{0x7F1116:X}>").Replace("</life>", $"</color>")
        .ToString();
    // RegexOptions.Multiline |
    // RegexOptions.Singleline |
    // RegexOptions.ExplicitCapture |
    // RegexOptions.IgnorePatternWhitespace |
    // RegexOptions.Compiled);

    public static void Disable<T>(this GameObject o) where T : Component => o.Disable<T>();
    public static void Disable<T>(this Component o) where T : Component => o.Disable<T>();
    public static void Disable(this Behaviour o) => o.Enable(false);

    public static void Enable<T>(this GameObject o) where T : Component => o.Enable<T>();
    public static void Enable<T>(this Component o) where T : Component => o.Enable<T>();
    public static void Enable(this Behaviour o, bool isOn=true) => o.enabled = isOn;

    static T GetComponentOrNull<T>(T o) => (o==null)?default(T):o;

    public static T Get<T>(this GameObject gameObject) =>
        GetComponentOrNull<T>(gameObject.GetComponent<T>());

    public static T Get<T>(this Component component) =>
        GetComponentOrNull<T>(component.GetComponent<T>());

    public static T GetParent<T>(this GameObject gameObject) =>
        GetComponentOrNull<T>(gameObject.GetComponentInParent<T>());

    public static T GetParent<T>(this Component component) =>
        GetComponentOrNull<T>(component.GetComponentInParent<T>());

    public static T GetChild<T>(this GameObject gameObject) =>
        GetComponentOrNull<T>(gameObject.GetComponentInChildren<T>());

    public static T GetChild<T>(this Component component) =>
        GetComponentOrNull<T>(component.GetComponentInChildren<T>());

    public static T GetChild<T>(this GameObject o, string s) {
        foreach (Transform child in o.transform)
            if (child.tag == s) return child.GetComponent<T>();
        return default(T);
    }

    public static IEnumerable<T> GetChildren<T>(
                    this GameObject gameObject,
                    string tag) =>
        from Transform child in gameObject.transform
        where child.tag==tag
        select child.GetComponent<T>();

    public static T Create<T>(GameObject original) where T : Component =>
        Create<T>(original, Vector3.zero, Quaternion.identity);

    public static T Create<T>(
                    GameObject original,
                    Vector3 position)
                    where T : Component =>
        Create<T>(original, position, Quaternion.identity);

    public static T Create<T>(
                    GameObject original,
                    Vector3 position,
                    Quaternion rotation) where T : Component {
        var instance = UnityEngine.Object.Instantiate(
            original: original,
            position: position,
            rotation: rotation) as GameObject;
        return instance.GetComponent<T>();
    }
}
