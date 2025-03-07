// ReSharper disable CheckNamespace

namespace HyperTrack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static partial class HyperTrack
    {
        public abstract class Json
        {
            public class Object : Json
            {
                public Dictionary<string, Json> Fields { get; }

                public Object(Dictionary<string, Json> fields)
                {
                    Fields = fields;
                }

                public override string ToString() => Helper.ToJsonString(this);

                public Dictionary<string, object?> ToDictionary() => Fields.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value switch
                    {
                        Null => null as object,
                        Bool b => b.Value as object,
                        Number n => n.Value as object,
                        String s => s.Value as object,
                        Array a => a.ToList() as object,
                        Object o => o.ToDictionary() as object,
                        _ => throw new InvalidOperationException()
                    }
                );
            }

            public class Array : Json
            {
                public IEnumerable<Json> Items { get; }

                public Array(IEnumerable<Json> items)
                {
                    Items = items;
                }

                public List<object?> ToList() => Items.Select(
                    item => item switch
                    {
                        Null => null as object,
                        Bool b => b.Value as object,
                        Number n => n.Value as object,
                        String s => s.Value as object,
                        Array a => a.ToList() as object,
                        Object o => o.ToDictionary() as object,
                        _ => throw new InvalidOperationException()
                    }
                ).ToList();
            }

            public class String : Json
            {
                public string Value { get; }

                public String(string value)
                {
                    Value = value;
                }
            }

            public class Number : Json
            {
                public double Value { get; }

                public Number(double value)
                {
                    Value = value;
                }
            }

            public class Bool : Json
            {
                public bool Value { get; }

                public Bool(bool value)
                {
                    Value = value;
                }
            }

            public class Null : Json
            {
                public Null() { }
                public static Null Instance { get; } = new Null();
                public override string ToString() => "null";
            }

            public static Json.Object? FromDictionary(Dictionary<string, object?> map) => TryFromJsonMap(map);
            public static Json.Object? FromString(string json) => TryFromJsonObjectString(json);
            public static T? FromStringToObject<T>(string json) => JsonSerializer.Deserialize<T>(json);

            private static Json.Object? TryFromJsonMap(Dictionary<string, object?> jsonMap)
            {
                try
                {
                    return new Json.Object(
                        jsonMap.ToDictionary(
                            kvp => kvp.Key,
                            kvp => TryToJsonValue(kvp.Value) ?? throw new Exception($"Invalid JSON value: {kvp.Value}")
                        )
                    );
                }
                catch
                {
                    return null;
                }
            }

            private static Json.Object? TryFromJsonObjectString(string json)
            {
                try
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                    return TryFromJsonElement(jsonElement) as Json.Object;
                }
                catch
                {
                    return null;
                }
            }

            private static Json? TryToJsonValue(object? value)
            {
                return value switch
                {
                    null => Null.Instance,
                    string s => new String(s),
                    bool b => new Bool(b),
                    double d => new Number(d),
                    int i => new Number(i),
                    float f => new Number(f),
                    long l => new Number(l),
                    IEnumerable<object> arr => new Array(arr.Select(TryToJsonValue).Where(v => v != null).Cast<Json>()),
                    Dictionary<string, object?> map => TryFromJsonMap(map),
                    _ => null
                };
            }

            private static Json? TryFromJsonElement(JsonElement element)
            {
                return element.ValueKind switch
                {
                    JsonValueKind.Null => Null.Instance,
                    JsonValueKind.True => new Bool(true),
                    JsonValueKind.False => new Bool(false),
                    JsonValueKind.Number => new Number(element.GetDouble()),
                    JsonValueKind.String => new String(element.GetString() ?? ""),
                    JsonValueKind.Array => new Array(element.EnumerateArray().Select(TryFromJsonElement).Where(v => v != null).Cast<Json>()),
                    JsonValueKind.Object => new Object(element.EnumerateObject().ToDictionary(p => p.Name, p => TryFromJsonElement(p.Value) ?? Null.Instance)),
                    _ => null
                };
            }
        }


    }

    static class Helper
    {
        public static string ToJsonString(this HyperTrack.Json jsonValue) => jsonValue switch
        {
            HyperTrack.Json.Null => "null",
            HyperTrack.Json.Bool b => b.Value.ToString().ToLowerInvariant(),
            HyperTrack.Json.Number n => n.Value.ToString(),
            HyperTrack.Json.String s => $"\"{s.Value}\"",
            HyperTrack.Json.Array a => $"[{string.Join(", ", a.Items.Select(ToJsonString))}]",
            HyperTrack.Json.Object o => $"{{{string.Join(", ", o.Fields.Select(kvp => $"\"{kvp.Key}\": {ToJsonString(kvp.Value)}"))}}}",
            _ => throw new InvalidOperationException()
        };
    }
}
