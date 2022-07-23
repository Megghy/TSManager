namespace TSManager.Shared.TSMDatastructs
{
    /// <summary>
    /// 简易tr物品信息
    /// </summary>
    public struct TItem
    {
        public int Id { get; set; }
        public byte Prefix { get; set; }
        public int Stack { get; set; }
    }
}
