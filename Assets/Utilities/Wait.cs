/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-12-16 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Wait : coroutine
/// Very simple coroutine, executes the function and returns.
/// Allows very simple things to be made yield statements.
/// - func : action
///     invoked rather immediately
public class Wait : CustomYieldInstruction {
    Action func;
    public override bool keepWaiting { get { func(); return false; } }
    public Wait(Action func) { this.func = func; }
}
