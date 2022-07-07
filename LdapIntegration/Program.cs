using System;

namespace LdapIntegration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var result = LdapLogin.GetLdapInfo();
            //LdapLogin.GetAllUsers();
            //LdapLogin.GetAUser("naimatalar@halic.edu.tr");
            //LdapLogin.GetUserGroups("naimatalar@halic.edu.tr");
            bool resultStudent = LdapLogin.LoginStudent();
            bool resultPersonel = LdapLogin.LoginPersonel();
            Console.WriteLine(resultStudent + " " + resultPersonel);
            Console.ReadKey();
        }
    }
}
