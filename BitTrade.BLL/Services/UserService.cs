﻿using BitTrade.BLL.Extensions;
using BitTrade.Common.Models;
using BitTrade.DAL.Interfaces;

namespace BitTrade.BLL.Services
{
    public class UserService : IUserService
    {
        readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public UserModel GetUserByID(int id)
        {
            var user = _unitOfWork.UserRepository.GetByID(id);

            return user.MapTo<UserModel>();

        }

        public void Updaet(UserModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
