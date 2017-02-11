/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-11-30 */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Pool : GameObject[]
/// encapsulates the interaction of instantiating groups of objects,
/// reusing them as their terminal events disable them.
/// Useful for memory efficiency and also for dependency injection.
public class Pool : IEnumerable<GameObject> {
    Stack<GameObject> stack = new Stack<GameObject>();
    Queue<GameObject> queue = new Queue<GameObject>();
    public int Count => stack.Count + queue.Count;
    public Pool() { }
    public Pool(IEnumerable<GameObject> list) { Add(list.ToArray()); }
    public Pool(GameObject original,int count=1) {
        if (count<1) count = 1;
        if (!original) throw new System.Exception("Pool original instance is null");
        for (var i=1;i<count;++i) Drop(GameObject.Instantiate(original) as GameObject);
    }

    public IEnumerator<GameObject> GetEnumerator() => queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => queue.GetEnumerator() as IEnumerator;
    void Drop() => Drop(queue.Dequeue());
    void Drop(GameObject o) { o.SetActive(false); stack.Push(o); }
    public void Add(params GameObject[] objs) => objs.ForEach(o => stack.Push(o));
    public T Create<T>(Transform o) { return Create<T>(o.position,o.rotation); }
    public T Create<T>(Vector3 o) { return Create(o).GetComponent<T>(); }
    public T Create<T>(Vector3 o, Quaternion r) => Create(o,r).GetComponent<T>();
    public GameObject Create(Transform o) => Create(o.position,o.rotation);
    public GameObject Create(Vector3 o) => Create(o,Quaternion.identity);
    public GameObject Create(Vector3 position, Quaternion rotation) {
        if (stack.Count<=0 && queue.Count>0) Drop();
        var instance = stack.Pop();
        instance.transform.parent = null;
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetActive(true);
        queue.Enqueue(instance);
        return instance;
    }
}
