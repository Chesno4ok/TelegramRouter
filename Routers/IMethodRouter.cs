using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramRouter.Controller;

namespace TelegramRouter.Routers
{
    public interface IMethodRouter
    {
        public Task<MethodInfo?> Handle(ControllerBase controllerBase, Update update);
    }
}
