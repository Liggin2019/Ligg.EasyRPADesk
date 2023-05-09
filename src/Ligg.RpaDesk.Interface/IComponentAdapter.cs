

namespace Ligg.RpaDesk.Interface
{
    public interface IStdServiceComponentAdapter
    {
        string Dispatch(string service, string method, string function, string[] paramArray);
        void Initialize(string[] paramArray);
        UniversalResult Validate(string input,string rule);
        string ResolveConstants(string input);
    }


}
