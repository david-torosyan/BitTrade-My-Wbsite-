﻿using BitTrade.BLL.Extensions;
using BitTrade.Common.Helpers;
using BitTrade.Common.Models;
using BitTrade.DAL;
using BitTrade.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BitTrade.BLL.Services
{
    public class MessageService : IMessageService
    {

        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(MessageModel model)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<MessageModel> GetByUserID(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ConversationModel> GetConversations(int? userID)
        {
            var claimID = ClaimHelper.ID;

            // And also by eager loading we load last message                                                                                                                                       
            var users = _unitOfWork.UserRepository.Get(u => u.SentMessages.Any(m => m.ToUserID == claimID) || u.ReceivedMessages.Any(m => m.FromUserID == claimID), includeProperties: "SentMessages,ReceivedMessages");


            var conversations = new List<ConversationModel>();
            foreach (var user in users)
            {
                var lastMessage = user.ReceivedMessages.Where(m => m.ToUserID == claimID)
                        .Concat(user.SentMessages.Where(m => m.FromUserID == claimID))
                        .OrderByDescending(m => m.Date)
                        .FirstOrDefault()
                        .MapTo<MessageModel>();

                if (lastMessage != null)
                {
                    lastMessage.IsFromUser = lastMessage.FromUserID == claimID;
                }

                conversations.Add(new ConversationModel
                {
                    User = user.MapTo<UserModel>(),
                    LastMessage = lastMessage
                });
            }
            //conversations = conversations.OrderByDescending(c => c.LastMessage.DateSent).ToList();

            if (userID.HasValue && userID.Value > 0)
            {
                var conversation = conversations.FirstOrDefault(c => c.User.ID == userID.Value);
                if (conversation != null)
                {
                    conversation.IsSelected = true;
                }
                else
                {
                    var user = _unitOfWork.UserRepository.GetByID(userID.Value);
                    if (user != null)
                    {
                        conversations.Insert(0, new ConversationModel
                        {
                            User = user.MapTo<UserModel>(),
                            IsSelected = true,
                            LastMessage = new MessageModel { Text = "..." }
                        });
                    }
                }
            }

            return conversations;
        }

        public bool HasNewMessages(int fromUserID, int lastMessageID)
        {
            throw new System.NotImplementedException();
        }
    }
}
