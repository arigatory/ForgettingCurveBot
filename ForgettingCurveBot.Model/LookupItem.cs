namespace ForgettingCurveBot.Model
{
    public class LookupItem
    {
        public long Id { get; set; }
        public string DisplayMember { get; set; }
    }
    public class NullLookupItem: LookupItem
    {
        public new int? Id { get { return null; } }
    }
}
