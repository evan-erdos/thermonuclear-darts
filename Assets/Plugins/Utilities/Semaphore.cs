/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-11-13 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Semaphore : YieldInstruction
/// A simple object which encapsulates a pattern of blocking in coroutines,
/// only starting a new coroutine if the prior coroutine has finished.
public class Semaphore : YieldInstruction {
    Func<IEnumerator,Coroutine> action;
    Dictionary<string,Func<IEnumerator>> coroutines =
        new Dictionary<string,Func<IEnumerator>>();
    public bool AreAnyYielding => coroutines.Count>0;
    public Semaphore(Func<IEnumerator,Coroutine> action) { this.action = action; }
    public void Clear() => coroutines.Clear();
    public bool IsYielding(string name) => coroutines.ContainsKey(name);
    public void Invoke(Func<IEnumerator> coroutine) =>
        Invoke(coroutine.Method.Name, coroutine);
    public void Invoke(string name, Func<IEnumerator> coroutine) {
        if (!coroutines.ContainsKey(name)) action(Waiting(name, coroutine)); }
    IEnumerator Waiting(string name, Func<IEnumerator> coroutine) {
        coroutines[name] = coroutine;
        yield return action(coroutine());
        coroutines.Remove(name);
    }
}
