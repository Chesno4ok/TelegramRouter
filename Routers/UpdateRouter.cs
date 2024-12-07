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
    internal class UpdateRouter : IMethodRouter
    {
        public async Task<MethodInfo?> Handle(ControllerBase controllerBase, Update update)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                controllerBase.UserContext.Method = update.CallbackQuery.Data;
            }

            var requestMethod = controllerBase.GetType().GetMethods().Where(i => i.GetCustomAttribute<MethodRoute>() != null)
                .FirstOrDefault(i => i.GetCustomAttribute<MethodRoute>()!.Method == controllerBase.UserContext.Method &&
                            i.GetCustomAttribute<MethodRoute>()!.UpdateType == update.Type);

            // If not, send user to the default routing
            if (requestMethod == null)
            {
                return null;
            }

            return requestMethod;
        }
    }
}
