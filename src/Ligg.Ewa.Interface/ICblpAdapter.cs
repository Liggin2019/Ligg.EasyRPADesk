namespace Ligg.EasyWinApp.Interface
{
    public interface ICblpAdapter
    {

        void Initialize();
        string ResolveConstantsEx(string text);
        string GetTextEx(string funName, string[] paramArray);
        string ActEx(string funcName, string[] paramArray);
        bool Logon(string userCode, string userPassword);
        bool VerifyUserToken(string userCode, string userToken);
    }

}
