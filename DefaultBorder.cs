namespace com.clusterrr.hakchi_gui
{
    class DefaultBorder
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
