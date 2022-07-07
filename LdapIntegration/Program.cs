using System;

namespace LdapIntegration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LdapLogin.GetAllUsers();
            LdapLogin.GetAUser("user_mail");
            LdapLogin.GetUserGroups("user_mail");
            bool resultStudent = LdapLogin.LoginStudent();
            bool resultPersonel = LdapLogin.LoginPersonel();
            Console.WriteLine(resultStudent + " " + resultPersonel);
            Console.ReadKey();
        }
    }
}
