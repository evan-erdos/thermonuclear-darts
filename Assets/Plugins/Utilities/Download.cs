/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2015-11-13 */

using System;
using UnityEngine;

/// Download : coroutine
/// Waits for a WWW instance to either download or fail,
/// and then invokes the callback.
/// - path : string
///     unescaped URL to use
/// - func : action
///     invoked after www is done downloading
public class Download : CustomYieldInstruction, IDisposable {
    bool disposed;
    Action<WWW> func;
    WWW www;

    public float Progress => www?.progress ?? 0;

    public override bool keepWaiting {
        get {
            if (!www.isDone) return true;
            if (string.IsNullOrEmpty(www.error)) func(www);
            else Debug.Log($"error: {www.error}\nurl: {www.url}");
            return false;
        }
    }

    public Download(string path, Action<WWW> func) {
        www = new WWW(System.Uri.EscapeUriString(path));
        this.func = func;
    }

    ~Download() { Dispose(false); }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool dispose) {
        if (disposed || !dispose) return;
        www.Dispose();
        www = null;
        func = null;
        disposed = true;
    }
}
