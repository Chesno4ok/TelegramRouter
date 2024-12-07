using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramRouter.Controller;
namespace TelegramRouter.Attributes
{
    public class MethodRoute : Attribute
    {
        public string Method;
        public ControllerState ControllerState = ControllerState.Response; 
        public MessageType? MessageType = null;
        public UpdateType? UpdateType = null;

        public MethodRoute(string route)
        {
            Method = route;
        }
        public MethodRoute(string route, MessageType messageType)
        {
            Method = route;
            MessageType = messageType;
            UpdateType = Telegram.Bot.Types.Enums.UpdateType.Message;
        }
        public MethodRoute(string route, UpdateType updateType)
        {
            Method = route;
            UpdateType = updateType;
        }
        public MethodRoute(string route, ControllerState controllerState)
        {
            Method = route;
            ControllerState = controllerState;
        }
    }
}
