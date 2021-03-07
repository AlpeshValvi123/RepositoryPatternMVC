using SupportApp.Core.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SupportApp.BLL.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Get the user from database by user identifier
        /// </summary>
        /// <param name="id">represent the user identifier</param>
        /// <returns>A user detail response</returns>
        Task<User> GetUserById(Guid id);

        /// <summary>
        /// Insert user into database
        /// </summary>
        /// <param name="users">A user dtails</param>
        /// <returns>Get a inserted user details</returns>
        Task<User> InsertUser(User users);
        
        /// <summary>
        /// Get gender
        /// </summary>
        /// <param name="selectedGender"></param>
        /// <returns>Gender list</returns>
        List<SelectListItem> GetGender(string selectedGender);
    }
}
