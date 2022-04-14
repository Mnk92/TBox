namespace Mnk.TBox.Plugins.TeamManager.Code.Scripts
{
    class ColumnInfo
    {
        public bool HasTime { get; set; }

        public ColumnInfo Clone()
        {
            return new ColumnInfo{HasTime = HasTime};
        }
    }
}
