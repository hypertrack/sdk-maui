// ReSharper disable CheckNamespace

using System.Globalization;

namespace HyperTrack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static partial class HyperTrack
    {
        public abstract class Json : IEquatable<Json>, IComparable, IComparable<Json>
        {
            public abstract bool Equals(Json? other);
            public override bool Equals(object? obj) => Equals(obj as Json);
            public abstract override int GetHashCode();
            public static bool operator ==(Json? left, Json? right) => left?.Equals(right) ?? right is null;
            public static bool operator !=(Json? left, Json? right) => !(left == right);

            public abstract int CompareTo(Json? other);

            public virtual int CompareTo(object? obj)
            {
                if (obj is null) return 1;
                if (obj is Json other) return CompareTo(other);
                throw new ArgumentException("Object is not a Json");
            }

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

                public override bool Equals(Json? other) =>
                    other is Object obj &&
                    Fields.Count == obj.Fields.Count &&
                    Fields.All(kvp => obj.Fields.TryGetValue(kvp.Key, out var val) && kvp.Value.Equals(val));

                public override int GetHashCode() =>
                    Fields.Aggregate(0, (hash, kvp) => hash ^ (kvp.Key.GetHashCode() * 397) ^ kvp.Value.GetHashCode());

                public override int CompareTo(Json? other)
                {
                    if (other is null) return 1;
                    if (other is not Object obj) return GetType().Name.CompareTo(other.GetType().Name);
                    return string.Compare(ToString(), obj.ToString(), StringComparison.Ordinal);
                }
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

                public override bool Equals(Json? other) =>
                    other is Array arr &&
                    Items.OrderBy(t => t).SequenceEqual(arr.Items.OrderBy(t => t));

                public override int GetHashCode() =>
                    Items.Aggregate(0, (hash, item) => hash ^ item.GetHashCode());

                public override int CompareTo(Json? other)
                {
                    if (other is null) return 1;
                    if (other is not Array arr) return GetType().Name.CompareTo(other.GetType().Name);
                    return string.Compare(ToString(), arr.ToString(), StringComparison.Ordinal);
                }
            }

            public class String : Json
            {
                public string Value { get; }

                public String(string value)
                {
                    Value = value;
                }

                public override bool Equals(Json? other) =>
                    other is String str && Value == str.Value;

                public override int GetHashCode() => Value.GetHashCode();

                public override int CompareTo(Json? other)
                {
                    if (other is null) return 1;
                    if (other is not String str) return GetType().Name.CompareTo(other.GetType().Name);
                    return string.Compare(Value, str.Value, StringComparison.Ordinal);
                }
            }

            public class Number : Json
            {
                public double Value { get; }

                public Number(double value)
                {
                    Value = value;
                }

                public override bool Equals(Json? other) =>
                    other is Number num && Value == num.Value;

                public override int GetHashCode() => Value.GetHashCode();

                public override int CompareTo(Json? other)
                {
                    if (other is null) return 1;
                    if (other is not Number num) return GetType().Name.CompareTo(other.GetType().Name);
                    return Value.CompareTo(num.Value);
                }
            }

            public class Bool : Json
            {
                public bool Value { get; }

                public Bool(bool value)
                {
                    Value = value;
                }

                public override bool Equals(Json? other) =>
                    other is Bool b && Value == b.Value;

                public override int GetHashCode() => Value.GetHashCode();

                public override int CompareTo(Json? other)
                {
                    if (other is null) return 1;
                    if (other is not Bool b) return GetType().Name.CompareTo(other.GetType().Name);
                    return Value.CompareTo(b.Value);
                }
            }

            public class Null : Json
            {
                public Null() { }
                public static Null Instance { get; } = new Null();
                public override string ToString() => "null";

                public override bool Equals(Json? other) => other is Null;
                public override int GetHashCode() => 0;

                public override int CompareTo(Json? other)
                {
                    if (other is null) return 1;
                    if (other is Null) return 0;
                    return GetType().Name.CompareTo(other.GetType().Name);
                }
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
                    JsonValueKind.Array => new Array(element.EnumerateArray().Select(TryFromJsonElement).Where(v => v != null).Cast<Json>()),
                    JsonValueKind.False => new Bool(false),
                    JsonValueKind.Null => Null.Instance,
                    JsonValueKind.Number => new Number(element.GetDouble()),
                    JsonValueKind.Object => new Object(element.EnumerateObject().ToDictionary(p => p.Name, p => TryFromJsonElement(p.Value) ?? Null.Instance)),
                    JsonValueKind.String => new String(element.GetString() ?? ""),
                    JsonValueKind.True => new Bool(true),
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
            HyperTrack.Json.Number n => n.Value.ToString(CultureInfo.InvariantCulture),
            HyperTrack.Json.String s => $"\"{s.Value}\"",
            HyperTrack.Json.Array a => $"[{string.Join(", ", a.Items.Select(ToJsonString))}]",
            HyperTrack.Json.Object o => $"{{{string.Join(", ", o.Fields.Select(kvp => $"\"{kvp.Key}\": {ToJsonString(kvp.Value)}"))}}}",
            _ => throw new InvalidOperationException()
        };
    }
}
