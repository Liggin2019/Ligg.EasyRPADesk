
namespace Ligg.StandardServices
{
    public interface IStandardService
    {
        void Do(string function, string[] paramArr);
        string Get(string function, string[] paramArr);
        //UniversalResult Validate(string function, string[] paramArr);
        //ScoreResult Score(string function, string[] paramArr);
        //bool Judge(string function, string[] paramArr);
    }

}