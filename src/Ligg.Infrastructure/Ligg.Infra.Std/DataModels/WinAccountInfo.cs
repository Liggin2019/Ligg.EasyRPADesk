using System;
using System.Collections.Generic;
using System.Text;

namespace  Ligg.Infrastructure.DataModels
{
    public class WinAccountInfo
    {
        public string Domain;
        public string UserName;
        public string Password;
        public WinAccountInfo(string domain, string userName, string password) { Domain = domain; UserName = userName; Password = password; }

        public WinAccountInfo(string domainPlusAccount, string password)
        {
            Domain = "";
            if (domainPlusAccount.Contains("\\"))
            {
                Domain = domainPlusAccount.Split('\\')[0]; 
                UserName = domainPlusAccount.Split('\\')[1]; Password = password;
            }
            else
            {
                UserName = domainPlusAccount; Password = password;
            }
            
        }

        public string GetFullUserName(string domain,string userName)
        {
            return domain + "\\" + userName;
        }



    }

}
