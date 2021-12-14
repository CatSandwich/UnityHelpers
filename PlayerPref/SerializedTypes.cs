using UnityEngine;

// ReSharper disable once CheckNamespace
namespace CatSandwich
{
    public class SerializedFloat
    {
        private readonly string _key;
        private readonly float _default;
        
        private float? _cached;
    
        public SerializedFloat(string key, float @default)
        {
            _key = key;
            _default = @default;
        }
        
        // Get
        public static implicit operator float(SerializedFloat f) => f._cached ??= PlayerPrefs.GetFloat(f._key, f._default);
        // Set
        public SerializedFloat Set(float val) 
        {
            PlayerPrefs.SetFloat(_key, (_cached = val).Value);
            return this;
        }

        // Allows for compound assignment
        public static SerializedFloat operator +(SerializedFloat s, float f) => s.Set((float)s + f);
    }
    
    public class SerializedInt
    {
        private readonly string _key;
        private readonly int _default;
    
        private int? _cached;
    
        public SerializedInt(string key, int @default)
        {
            _key = key;
            _default = @default;
        }
        
        // Get
        public static implicit operator int(SerializedInt i) => i._cached ??= PlayerPrefs.GetInt(i._key, i._default);
        // Set
        public SerializedInt Set(int i)
        {
            PlayerPrefs.SetInt(_key, (_cached = i).Value);
            return this;
        }

        // Allows for compound assignment
        public static SerializedInt operator +(SerializedInt s, int i) => s.Set((int)s + i);
    }
    
    public class SerializedString
    {
        private readonly string _key;
        private readonly string _default;
    
        private string _cached;
    
        public SerializedString(string key, string @default)
        {
            _key = key;
            _default = @default;
        }
        
        // Get
        public static implicit operator string(SerializedString s) => s._cached ??= PlayerPrefs.GetString(s._key, s._default);
        // Set
        public string Set(string s)
        {
            PlayerPrefs.SetString(_key, s);
            return this;
        }
    }
}
