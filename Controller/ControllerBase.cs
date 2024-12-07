using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramRouter.Routers;
using TelegramRouter.Controller;
using TelegramRouter.Routers;


namespace TelegramRouter.Controller
{

    [JsonObject(MemberSerialization.OptIn)]
    public class ControllerBase
    {
        public BotRouter BotRouter { get; internal set; }
        public bool IsRedirected { get; internal set; }
        public ControllerState ControllerState { get; internal set; }
        public IMethodRouter MethodRouter { get; internal set; }
        public UserContext UserContext { get; internal set; }

        internal async Task RouteMethod(Update update, UserContext userContext)
        {
            UserContext = new UserContext(userContext);

            var message = update.Message;
            var requestMethod = await MethodRouter.Handle(this, update);

            if (requestMethod is null)
            {
                Console.WriteLine($"Couldn't find method. Method: {UserContext.Method} Type: {update.Type}");

                if(ControllerState == ControllerState.Response)
                {
                    SetMethod("start");
                    SetController("start");
                }
            }
            else 
            {
                var res = requestMethod.Invoke(this, new object[] { update });

                if (res is Task)
                    ((Task)res).Wait();
            }

            if(ControllerState == ControllerState.Response)
            {
                await SetJsonContext(JsonConvert.SerializeObject(this));
                await BotRouter.ControllerContext.SetContext(UserContext);
            }
        }

        public void SetMethod(string method)
        {
            UserContext.Method = method;
            IsRedirected = true;
        }
        public void SetController(string controller)
        {
            UserContext.Controller = controller;
            IsRedirected = true;
        }
        internal async Task SetJsonContext(string json)
        {
            var userContext = UserContext;
            userContext.JsonContext = json;
            await BotRouter.ControllerContext.SetContext(userContext);

        }
    }
}
