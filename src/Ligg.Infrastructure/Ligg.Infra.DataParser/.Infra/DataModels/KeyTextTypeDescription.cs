
//*doing
namespace Ligg.Infrastructure.DataModels
{
    //*doing
    internal class KeyTextTypeDescription
    {
        internal KeyTextTypeDescription() { }
        internal KeyTextTypeDescription(string key, string txt,  string type="", string des="")
        {
            Key = key; Text = txt; Type = type; Description = des;
        }

        internal string Key { get; set; }
        internal string Text { get; set; }
        internal string Type { get; set; }
        internal string Description { get; set; }
    }


}
