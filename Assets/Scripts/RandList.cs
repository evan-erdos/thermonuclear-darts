using System.Collections;
using System.Collections.Generic;

public class RandList<T> : List<T>
{
    System.Random random = new System.Random();
    public T Next() { return this[random.Next(this.Count)]; }
    public RandList(IEnumerable<T> collection) : base(collection) { }
}