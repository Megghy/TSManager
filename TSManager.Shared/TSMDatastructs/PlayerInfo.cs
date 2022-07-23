namespace TSManager.Shared.TSMDatastructs
{
    public record PlayerInfo
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public TItem[] Inventory { get; set; } = new TItem[260];
    }
}
