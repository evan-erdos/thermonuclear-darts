/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-05-17 */

using Type=System.Type;
using System.Text.RegularExpressions;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// YamlConverterBase<T> : IYamlTypeConverter
/// deserializes and emits simple scalar types
public abstract class YamlConverterBase<T> : IYamlTypeConverter {

    /// IsValid : (string) -> bool
    /// determines if a particular scalar is well formed
    public abstract bool IsValid(string s);

    /// Parse : (string) -> <T>
    /// creates an instance of <T> from the parsed scalar
    public abstract T Parse(string s);

    /// Emit : (<T>) -> string
    /// creates a string from an instance of <T>
    public abstract string Emit(T o);

    public bool Accepts(Type type) => type==typeof(T);

    public object ReadYaml(IParser parser, Type type) {
        var current = parser.Current;
        parser.MoveNext();
        if (!Accepts(type)) return null;
        var scalar = current as Scalar;
        if (!IsValid(scalar.Value)) return null;
        return Parse(scalar.Value);
    }


    public void WriteYaml(
                    IEmitter emitter,
                    object value,
                    Type type) =>
        emitter.Emit(new Scalar(Emit((T) value)));
}


class Vector2YamlConverter : YamlConverterBase<Vector2> {
    Regex regex = new Regex(@"[〈<](-?[\d.e]+),(-?[\d.e]+)[>〉]");
    public override bool IsValid(string s) => regex.IsMatch(s);
    public override string Emit(Vector2 o) => $"[〈<]{o.x},{o.y}[>〉]";
    public override Vector2 Parse(string s) {
        var matches = new Regex(@"\-?[\d.e]+").Matches(s);
        return new Vector2(
            float.Parse(matches[0].Value),
            float.Parse(matches[1].Value));
    }

}


class RegexYamlConverter : YamlConverterBase<Regex> {
    Regex regex = new Regex(@"/.*/");
    public override bool IsValid(string s) => regex.IsMatch(s);
    public override string Emit(Regex o) => $"/{o}/";
    public override Regex Parse(string s) => new Regex($@"\b{s.Trim('/')}\b");
}
