namespace  Ligg.Infrastructure.DataModels
{
    public class ValueText
    {
        public ValueText(string val, string txt) { Value = val; Text = txt; }
        public ValueText(){}
        public string Text { get; set; }
        public string Value { get; set; }

        public override string ToString() { return $"Value: {Value}, Text: {Text}"; }
    }
}
