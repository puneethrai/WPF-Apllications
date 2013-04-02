using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Authentication
{
    public class Authentication
    {
        #region private
        private Dictionary<Socket,string> authorizedList = null;
        //TODO:Able to query SQL server & return validity authorized
        private bool isValidToken(string token)
        {
            return true;
        }
        private int authenticationLimit;
        #endregion
        #region public
        
        public Authentication(uint maxAuthentication)
        {
            authenticationLimit = maxAuthentication;
            authorizedList = new Dictionary<Socket,string>(maxAuthentication);
        }
        /// <summary>
        /// Validates the User against the given token
        /// </summary>
        /// <param name="UserSocket">User Socket to register</param>
        /// <param name="token">Secret token given to user</param>
        /// <returns>True if is token is valid else unregistered the user</returns>
        public bool ValidateUser(Socket UserSocket, string token)
        {
            if (isValidToken(token))
            {
                authorizedList[UserSocket] = token;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Determines weather the user is valid or not
        /// </summary>
        /// <param name="UserSocket">User Socket to be validated against</param>
        /// <returns>True if registered</returns>
        public bool isRegistered(Socket UserSocket)
        {
            return authorizedList.ContainsKey(UserSocket);
        }
        /// <summary>
        /// Removes the user info from Auth list
        /// </summary>
        /// <param name="UserSocket">User Socket to be deregistered</param>
        public void DeRegisterUser(Socket UserSocket)
        {
            if (authorizedList.ContainsKey(UserSocket))
                authorizedList.Remove(UserSocket);
        }
        #endregion
    }
}
