/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-11-01 */

using System;
using System.Collections;
using System.Collections.Generic;

/// Map<T> : Dictionary<string,T>
/// A simple wrapper class for Dictionary<string,T>,
/// which drastically shortens the name for maps.
public class Map<T> : Dictionary<string,T> { }
public class Map<K,V> : Dictionary<K,V> { }

/// TypeMap<T> : (type) -> Func<T>
/// Maps from types (T and subclasses thereof)
/// to instances whose type takes the type they're keyed to as a parameter.
public class TypeMap<T> : Dictionary<Type,List<Func<T>>> {

    Dictionary<Type,List<Func<T>>> map =
        new Dictionary<Type,List<Func<T>>>();

    public new List<Func<T>> this[Type type] {
        get { return map[type]; }
        set { map[type] = value; }}

    public List<Func<T>> Get<U>() where U : T => map[typeof(U)];

    public void Set<U>(List<Func<T>> value) where U : T =>
        map[typeof(U)] = (List<Func<T>>) value;
}
