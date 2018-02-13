namespace com.clusterrr.hakchi_gui
{
    public class NesDefaultGame : INesMenuElement
    {
        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string SortName
        {
            get { return name; }
        }

        public override string ToString()
        {
            return Name;
        }

        private int size;

        public int Size
        {
            get { return size; }
            set { size = value; }
        }
    }
}
