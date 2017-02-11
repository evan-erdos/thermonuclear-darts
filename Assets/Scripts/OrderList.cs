using System.Collections;
using System.Collections.Generic;

public class OrderList<T> : List<T>
{
    private int index;
    public T Next() {
		if (index >= this.Count) return default (T);
		return this[(index++)%this.Count];
    }
    public OrderList(IEnumerable<T> collection) : base(collection) { }
}