using SupportApp.BLL.Interfaces;
using SupportApp.Core.Domain.User;
using SupportApp.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SupportApp.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get the user from database by user identifier
        /// </summary>
        /// <param name="id">represent the user identifier</param>
        /// <returns>A user detail response</returns>
        public async Task<User> GetUserById(Guid id)
        {
            var user = await _userRepository.GetById(id);
            return user;
        }

        /// <summary>
        /// Insert user into database
        /// </summary>
        /// <param name="users">A user dtails</param>
        /// <returns>Get a inserted user details</returns>
        public async Task<User> InsertUser(User users)
        {
            _userRepository.Insert(users);
            var user = await _userRepository.Save();
            return users;
        }
               
        /// <summary>
        /// Get genders
        /// </summary>
        /// <param name="selectedGender">Gender</param>
        /// <returns>Gender list</returns>
        public List<SelectListItem> GetGender(string selectedGender)
        {
            var _genderList = new List<SelectListItem>();
            _genderList.Add(new SelectListItem()
            {
                Text = "Male",
                Value = "Male",
                Selected = "Male" == selectedGender
            });
            _genderList.Add(new SelectListItem()
            {
                Text = "Female",
                Value = "Female",
                Selected = "Female" == selectedGender
            });

            return _genderList;
        }
    }
}
