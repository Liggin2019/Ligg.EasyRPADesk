using System.Collections.Generic;

namespace Ligg.Infrastructure.Base.DataModel
{
    public class IdValueText
    {
        public IdValueText(){ }
        public IdValueText(string id, string val, string txt)
        {
            Id = id; Value = val; Text = txt;
        }
        public string Id { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public override string ToString() { return $"Id: {Id}, Value: {Value}, Text: {Text}"; }
    }

    public class PagedIdValueTextList
    {
        public List<IdValueText> IdValueTexts;
        public long RecordTotal;

        public PagedIdValueTextList(List<IdValueText> pagedIdValueTextList, long recordTotal)
        {
            IdValueTexts = pagedIdValueTextList;
            RecordTotal = recordTotal;
        }
    }
}
