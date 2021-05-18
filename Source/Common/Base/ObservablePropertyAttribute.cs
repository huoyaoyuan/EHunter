using System;

namespace EHunter
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ObservablePropertyAttribute : Attribute
    {
        public ObservablePropertyAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
        public bool IsSetterPublic { get; set; } = true;
        public bool IsNullable { get; set; } = true;
        public string? Initializer { get; set; }
        public string? ChangedAction { get; set; }
    }
}
