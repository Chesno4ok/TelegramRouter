using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramRouter.Attributes;
using TelegramRouter.Controller;

namespace TelegramRouter.Routers
{
    internal class MessageRouter : IMethodRouter
    {
        public async Task<MethodInfo?> Handle(ControllerBase controllerBase, Update update)
        {
            var message = update.Message;

            var methods = controllerBase.GetType().GetMethods().Where(i => i.GetCustomAttribute<MethodRoute>() != null)
                .Where(i => i.GetCustomAttribute<MethodRoute>()!.Method == controllerBase.UserContext.Method &&
                            i.GetCustomAttribute<MethodRoute>()!.ControllerState == controllerBase.ControllerState &&
                            (i.GetCustomAttribute<MethodRoute>()!.UpdateType == update.Type || i.GetCustomAttribute<MethodRoute>()!.UpdateType is null));

            // If not, send user to the default routing
            if (methods.Count() == 0)
            {
                return null;
            }

            // Out of methods get one with matching MessageType
            var requestMethod = methods
                .FirstOrDefault(i => i.GetCustomAttribute<MethodRoute>()!.MessageType == message?.Type);

            // If not found, send user to the method with null MessageType
            if (requestMethod == null)
            {
                requestMethod = methods.FirstOrDefault(i => i.GetCustomAttribute<MethodRoute>()!.MessageType == null);
            }

            return requestMethod;
        }
    }
}
