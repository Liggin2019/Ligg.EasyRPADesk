
namespace Ligg.RpaDesk.WinForm.DataModels
{
    public class TreeItem
    {
        public string Id;
        public string ParentId;
        public string Name;
        public string DisplayName;
        public string Value;
        public string Value1;
        public int ImageIndex;
        public int SelectedImageIndex;
    }

    public enum GetTreeItemValueOption
    {
        Id = 0,
        Name = 1,
        DisplayName = 2,
        Value = 3,
        Value1 = 4,
    }
}
