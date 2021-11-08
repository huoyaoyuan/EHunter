namespace EHunter
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DependencyPropertyAttribute : Attribute
    {
        public DependencyPropertyAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
        public bool IsSetterPublic { get; set; } = true;
        public bool IsNullable { get; set; } = true;
        public string DefaultValue { get; set; } = "null";
        public bool InstanceChangedCallback { get; set; }
    }
}
