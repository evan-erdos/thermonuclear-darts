/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2016-11-13 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DateTime=System.DateTime;
using Type=System.Type;
using YamlDotNet.Serialization;
using UnityEngine;

/// YamlReader : coroutine
/// Waits for a WWW instance to either download or fail,
/// and then reads in the data as *.yml and deserializes.
/// - path : string
///     unescaped URL to use
/// - func : action
///     invoked after www is done downloading
public abstract class YamlReader : CustomYieldInstruction {

    public static readonly string prefix = "tag:yaml.org,2002:";

    public static readonly List<IYamlTypeConverter> converters =
        new List<IYamlTypeConverter> {
            new RegexYamlConverter(),
            new Vector2YamlConverter()};

    public static readonly Dictionary<string,Type> tags =
        new Dictionary<string,Type> {
            ["regex"] = typeof(Regex),
            ["date"] = typeof(DateTime)};

    public static Deserializer GetDefaultDeserializer() {
        var deserializer = new Deserializer(
            namingConvention: new SemanticNamingConvention(),
            ignoreUnmatched: true);
        converters.ForEach(converter =>
            deserializer.RegisterTypeConverter(converter));
        foreach (var tag in tags)
            deserializer.RegisterTagMapping(
                $"{prefix}{tag.Key}", tag.Value);
        return deserializer;
    }

    public static Type GetMapFromType(string type) =>
        GetMapFromType(tags[type]);

    public static Type GetMapFromType(Type type) =>
        typeof(Dictionary<,>).MakeGenericType(type);

    public static string FromType(Type type) {
        var names = type.FullName.Split('.');
        var name = names[names.Length-1];
        return new Regex(@"\+Data").Replace(name,"").ToLower();
    }

    public override bool keepWaiting => false;
}

public class YamlReader<T> : YamlReader {

    Deserializer deserializer = new Deserializer();
    Action<T> func;
    WWW www;

    public override bool keepWaiting {
        get {
            if (!www.isDone) return true;
            if (string.IsNullOrEmpty(www.error))
                Deserialize(www.text);
            return false;
        }
    }


    public YamlReader(string path, Action<T> func) {

        converters.ForEach(converter =>
            deserializer.RegisterTypeConverter(converter));

        foreach (var tag in tags)
            deserializer.RegisterTagMapping(
                $"{prefix}{tag.Key}", tag.Value);

        www = new WWW(System.Uri.EscapeUriString(path));
        this.func = func;
    }

    void Deserialize(string s) =>
        func(deserializer.Deserialize<T>(new StringReader(s)));

}



public class SemanticNamingConvention : INamingConvention {
    public string Apply(string s) =>
        string.Join(" ", Regex.Split(s,@"(?<!^)(?=[A-Z])")).ToLower().Trim(); }
