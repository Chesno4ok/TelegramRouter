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
    class UnknownRouting : IMethodRouter
    {
        public async Task<MethodInfo?> Handle(ControllerBase controllerBase, Update update)
        {

            var methods = controllerBase.GetType().GetMethods().Where(i => i.GetCustomAttribute<MethodRoute>() != null)
                .Where(i => i.GetCustomAttribute<MethodRoute>()!.Method == controllerBase.UserContext.Method &&
                            i.GetCustomAttribute<MethodRoute>()!.ControllerState == controllerBase.ControllerState && 
                            i.GetCustomAttribute<MethodRoute>()!.UpdateType == null);

            var method = methods.FirstOrDefault();

            return method;
        }
    }
}
