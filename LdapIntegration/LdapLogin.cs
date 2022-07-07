using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace LdapIntegration
{
    public static class LdapLogin
    {
        public static bool resultStudent { get; set; } = false;
        public static bool resultPersonel { get; set; } = false;
        #region Ldap Info
        public static string LdapDomainStudent { get; set; }
        public static string LdapDomainPersonel { get; set; }
        public static string UserStudent { get; set; } 
        public static string PassStudent { get; set; } 
        public static string UserPersonel { get; set; } 
        public static string PassPersonel { get; set; }
        #endregion

        public static Boolean LoginStudent()
        {
            using (PrincipalContext pContext = new PrincipalContext(ContextType.Domain, LdapDomainStudent))
            {
                resultStudent = pContext.ValidateCredentials(UserStudent, PassStudent);
            }

            return resultStudent;
        }

        public static Boolean LoginPersonel()
        {
            using (PrincipalContext pContext = new PrincipalContext(ContextType.Domain, LdapDomainPersonel))
            {
                resultPersonel = pContext.ValidateCredentials(UserPersonel, PassPersonel);
            }

            return resultPersonel;
        }

        public static DirectoryEntry GetLdapInfo()
        {
            //search with PrincipalContext
            var domainContext = new PrincipalContext(ContextType.Domain);
            var up = new UserPrincipal(domainContext);
            var searcher = new PrincipalSearcher(up);
            PrincipalSearchResult<Principal> result = searcher.FindAll();
            foreach (Principal item in result)
            {
                Debug.WriteLine(item.Name);
            }

            DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://domain.local", "user", "password", AuthenticationTypes.Secure);
            return directoryEntry;
        }

        public static void GetAllUsers()
        {
            SearchResultCollection results;
            DirectorySearcher directorySearcher = null;
            DirectoryEntry directoryEntry = GetLdapInfo();

            directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = "(&(objectCategory=User)(objectClass=person))";

            results = directorySearcher.FindAll();

            foreach (SearchResult searchResult in results)
            {
                // Using the index zero (0) is required!
                Debug.WriteLine(searchResult.Properties["name"][0].ToString());
            }
        }

        public static void GetAdditionalUserInfo()
        {
            SearchResultCollection results;
            DirectorySearcher directorySearcher = null;
            DirectoryEntry directoryEntry = GetLdapInfo();

            directorySearcher = new DirectorySearcher(directoryEntry);

            // Full Name
            directorySearcher.PropertiesToLoad.Add("name");

            // Email Address
            directorySearcher.PropertiesToLoad.Add("mail");

            // First Name
            directorySearcher.PropertiesToLoad.Add("givenname");

            // Last Name (Surname)
            directorySearcher.PropertiesToLoad.Add("sn");

            // Login Name
            directorySearcher.PropertiesToLoad.Add("userPrincipalName");

            // Distinguished Name
            directorySearcher.PropertiesToLoad.Add("distinguishedName");

            directorySearcher.Filter = "(&(objectCategory=User)(objectClass=person))";

            results = directorySearcher.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                    Debug.WriteLine(sr.Properties["name"][0].ToString());

                // If not filled in, then you will get an error
                if (sr.Properties["mail"].Count > 0)
                    Debug.WriteLine(sr.Properties["mail"][0].ToString());

                if (sr.Properties["givenname"].Count > 0)
                    Debug.WriteLine(sr.Properties["givenname"][0].ToString());

                if (sr.Properties["sn"].Count > 0)
                    Debug.WriteLine(sr.Properties["sn"][0].ToString());

                if (sr.Properties["userPrincipalName"].Count > 0)
                    Debug.WriteLine(sr.Properties["userPrincipalName"][0].ToString());

                if (sr.Properties["distinguishedName"].Count > 0)
                    Debug.WriteLine(sr.Properties["distinguishedName"][0].ToString());
            }
        }

        public static DirectorySearcher BuildUserSearcher(DirectoryEntry directoryEntry)
        {
            DirectorySearcher directorySearcher = null;

            directorySearcher = new DirectorySearcher(directoryEntry);

            // Full Name
            directorySearcher.PropertiesToLoad.Add("name");

            // Email Address
            directorySearcher.PropertiesToLoad.Add("mail");

            // First Name
            directorySearcher.PropertiesToLoad.Add("givenname");

            // Last Name (Surname)
            directorySearcher.PropertiesToLoad.Add("sn");

            // Login Name
            directorySearcher.PropertiesToLoad.Add("userPrincipalName");

            // Distinguished Name
            directorySearcher.PropertiesToLoad.Add("distinguishedName");

            directorySearcher.PropertiesToLoad.Add("ExtensionAttribute11");

            directorySearcher.PropertiesToLoad.Add("ExtensionAttribute7");

            directorySearcher.PropertiesToLoad.Add("ExtensionAttribute5");

            directorySearcher.PropertiesToLoad.Add("EmployeeType");

            directorySearcher.PropertiesToLoad.Add("Title");

            directorySearcher.PropertiesToLoad.Add("EmployeeId");

            directorySearcher.PropertiesToLoad.Add("department");

            return directorySearcher;
        }

        public static string GetPropertyValue(this SearchResult sr, string propertyName)
        {
            string ret = string.Empty;

            if (sr.Properties[propertyName].Count > 0)
                ret = sr.Properties[propertyName][0].ToString();

            return ret;
        }

        public static void SearchForUsers(string username)
        {
            SearchResultCollection results;
            DirectorySearcher directorySearcher = null;
            DirectoryEntry directoryEntry = GetLdapInfo();

            //Build User Searcher
            directorySearcher = BuildUserSearcher(directoryEntry);

            directorySearcher.Filter = "(&(objectCategory=User)(objectClass=person)(name=" + username + "*))";
            //directorySearcher.Filter = $"(mail={username})";

            results = directorySearcher.FindAll();

            foreach (SearchResult sr in results)
            {
                var mail = sr.GetPropertyValue("mail");
            }
        }

        public static void GetAUser(string username)
        {
            DirectorySearcher directorySearcher = null;
            DirectoryEntry directoryEntry = GetLdapInfo();
            SearchResult sr;

            // Build User Searcher
            directorySearcher = BuildUserSearcher(directoryEntry);
            directorySearcher.Filter = $"(mail={username})";

            sr = directorySearcher.FindOne();

            if (sr != null)
            {
                var fullname = sr.GetPropertyValue("name");
                var mail = sr.GetPropertyValue("mail");
                var firstName = sr.GetPropertyValue("givenname");
                var lastName = sr.GetPropertyValue("sn");
                var userPrincipalName = sr.GetPropertyValue("userPrincipalName");
                var distinguishedName = sr.GetPropertyValue("distinguishedName");
                var tcIdentityNo = sr.GetPropertyValue("ExtensionAttribute11");
                var employeeType = sr.GetPropertyValue("EmployeeType");
                var title = sr.GetPropertyValue("Title");
                var regnumber = sr.GetPropertyValue("EmployeeId");
                var department = sr.GetPropertyValue("department");
                var studentNumber = sr.GetPropertyValue("ExtensionAttribute7");
                var studentFaculty = sr.GetPropertyValue("ExtensionAttribute5");
            }
         }

        public static void GetUserGroups(string username)
        {
            DirectorySearcher directorySearcher = null;
            DirectoryEntry directoryEntry = GetLdapInfo();
            SearchResultCollection results; 

            directorySearcher = new DirectorySearcher(directoryEntry);

            directorySearcher.Sort = new SortOption("name", SortDirection.Ascending);
            directorySearcher.PropertiesToLoad.Add("name");
            directorySearcher.PropertiesToLoad.Add("memberof");
            directorySearcher.PropertiesToLoad.Add("member");
            directorySearcher.PropertiesToLoad.Add("distinguishedName");
            directorySearcher.Filter = $"(mail={username})";
            results = directorySearcher.FindAll();

            foreach(SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        int indexNo = sr.Properties["memberof"][i].ToString().IndexOf(",");
                        Console.WriteLine(sr.Properties["memberof"][i].ToString().Substring(3,(indexNo-3)));
                    }
                }
                else
                    Console.WriteLine("error");
            }
        }

        public static void GetAllGroups()
        {
            SearchResultCollection results;
            DirectoryEntry directoryEntry = GetLdapInfo();
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);

            directorySearcher.Sort = new SortOption("name", SortDirection.Ascending);
            directorySearcher.PropertiesToLoad.Add("name");
            directorySearcher.PropertiesToLoad.Add("memberof");
            directorySearcher.PropertiesToLoad.Add("member");

            directorySearcher.Filter = "(&(objectCategory=Group))";

            results = directorySearcher.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                    Debug.WriteLine(sr.Properties["name"][0].ToString());

                if (sr.Properties["memberof"].Count > 0)
                {
                    Debug.WriteLine("  Member of...");
                    foreach (string item in sr.Properties["memberof"])
                    {
                        Debug.WriteLine("    " + item);
                    }
                }
                if (sr.Properties["member"].Count > 0)
                {
                    Debug.WriteLine("  Members");
                    foreach (string item in sr.Properties["member"])
                    {
                        Debug.WriteLine("    " + item);
                    }
                }
            }
        }

    }
}
