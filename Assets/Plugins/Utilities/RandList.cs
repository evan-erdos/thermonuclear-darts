/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-01-01 */

using System.Collections;
using System.Collections.Generic;

/// RandList<T> : List<T>
/// A simple wrapper class for List<T>, which adds the
/// ability to return a random element from the list.
public class RandList<T> : List<T> {
    System.Random random = new System.Random();
    public T Pick() {
        return (Count==0) ? default(T) : this[random.Next(this.Count)]; }
}
