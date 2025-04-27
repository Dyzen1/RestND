using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Role
    {
        #region Role Name
        private string _Role_Name;
        public string Role_Name
        {
            get { return _Role_Name; }
            set { _Role_Name = value; }
        }
        #endregion

        #region Role Authorization
        private AuthorizationStatus _Role_Authorization;

        public AuthorizationStatus Role_Authorization
        {
            get { return _Role_Authorization; }
            set { _Role_Authorization = value; }
        }
        #endregion

        #region constructor
        public Role(string roleName, AuthorizationStatus roleAuthorization)
        {
            Role_Name = roleName;
            Role_Authorization = roleAuthorization;
        }
        #endregion



    }
}
