namespace  Ligg.Infrastructure.DataModels
{
    public class KeyValue
    {
        public KeyValue(string key, string val) { Value = val; Key = key; }
        public KeyValue(){}
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString() { return $"Key: {Key}, Value: {Value}"; }
    }
}
