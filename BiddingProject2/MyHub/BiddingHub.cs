using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BiddingProject2.DAL;
using Microsoft.AspNet.SignalR;

namespace BiddingProject2.MyHub
{
    public class BiddingHub : Hub
    {
        BiddingDbContext db = new BiddingDbContext();
        public void Notification(string groupName, string price)
        {
            string username = Context.User.Identity.Name;
            string name = db.Accounts.FirstOrDefault(m => m.Username == username).Profile.Name;
            Clients.Group(groupName).notificationMessage(name, price);
        }

        public void UpdatePrice(string price, int idProduct)
        {
            Clients.All.updatePrice(price);
        }

        public void SendMessage(string groupName, string message)
        {
            string username = Context.User.Identity.Name;
            string name = db.Accounts.FirstOrDefault(m => m.Username == username).Profile.Name;
            Clients.Group(groupName).sendUserMessage($"<b>{name}</b>:", message);
        }

        public void SendIndividual(string username, string message)
        {
            if(username == Context.User.Identity.Name)
            {
                string idUser = Context.ConnectionId;
                Clients.Client(idUser).displayPopup(message);
            }
        }

        public async Task JoinGroup(string groupName)
        {
            string username = Context.User.Identity.Name;
            string name = db.Accounts.FirstOrDefault(m => m.Username == username).Profile.Name;
            string idUser = Context.ConnectionId;
            await Groups.Add(idUser, groupName);
            Clients.Group(groupName).addMessage($"{ name }  has joined");
        }

        public async Task LeaveGroup(string groupName)
        {
            string name = Context.User.Identity.Name;
            string idUser = Context.ConnectionId;
            await Groups.Remove(idUser, groupName);
            Clients.Group(groupName).addMessage($"{ name }  has left");
        }
    }
}