namespace Ligg.Infrastructure.Base.DataModel
{
    public class IntIdName
    {
        public IntIdName(){}
        public IntIdName(int id, string name) { _id = id; _name = name; }

        private int _id;
        private string _name;

        public int Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public override string ToString() { return $"Id: {_id}, Name: {_name}"; }
    }
}
