namespace com.clusterrr.hakchi_gui
{
    struct GameGenre
    {
        public string LocalizedName { get; private set; }
        public string Name { get; private set; }

        public GameGenre(string localizedName, string name)
        {
            LocalizedName = localizedName;
            Name = name;
        }

        public override string ToString() => LocalizedName;
    }
}
