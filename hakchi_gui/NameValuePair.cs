namespace com.clusterrr.hakchi_gui
{
    struct NameValuePair<T>
    {
        public string Name { get; private set; }
        public T Value { get; private set; }

        public NameValuePair(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => Name;
    }
}
