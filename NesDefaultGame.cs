
namespace com.clusterrr.hakchi_gui
{
    public class NesDefaultGame : NesMiniApplication, INesMenuElement
    {
        //private  string code;

        public new string Code
        {
            get { return code; }
            set { code = value; }
        }
        //private string name;

        /* public string Name
         {
             get { return name; }
             set { name = value; }
         }*/
        public override bool Save()
        {
            return true;
        }
        public override string ToString()
        {
            return Name;
        }

        private int size;

        public new int Size
        {
            get { return size; }
            set { size = value; }
        }
    }
}
