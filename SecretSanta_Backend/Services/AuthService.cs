using Novell.Directory.Ldap;
using SecretSanta_Backend.ModelsDTO;

namespace SecretSanta_Backend.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ILdapConnection GetConnection()
        {
            string ladpHost = _configuration[("Ldap:Host")];
            int ldapPort = Convert.ToInt32(_configuration[("Ldap:Port")]);
            string loginDn = $"{_configuration[("Ldap:ServiceAccount")]},{_configuration[("Ldap:DN")]}";
            string passwordDn = _configuration[("Ldap:Password")];

            var ldapConnection = new LdapConnection() { SecureSocketLayer = false };

            ldapConnection.Connect(ladpHost, ldapPort); 
            ldapConnection.Bind(loginDn, passwordDn);

            return ldapConnection;
        }

        public bool ValidUser(MemberLogin data)
        {
            if (data.UserName == "admin" && data.Password == _configuration[("Ldap:AdminPass")])
            {
                return true;
            }
            string ladpHost = _configuration[("Ldap:Host")];
            int ldapPort = Convert.ToInt32(_configuration[("Ldap:Port")]);
            //var user = $"sAMAccountName={data.UserName}";


            using (var ldapConnection = new LdapConnection() { SecureSocketLayer = false })
            {
                ldapConnection.Connect(ladpHost, ldapPort);

                var distinguishedName = $"uid={data.UserName},{_configuration[("Ldap:DN")]}";

                try
                {
                    ldapConnection.Bind(distinguishedName, data.Password);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
