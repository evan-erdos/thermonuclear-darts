using System.Collections.Generic;

class RandList<T> : List<T> {
    System.Random random = new System.Random();
    public T Next() { return this[random.Next(this.Count)]; }
}
