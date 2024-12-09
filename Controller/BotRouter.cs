
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramRouter.Attributes;
using TelegramRouter.Routers;

namespace TelegramRouter.Controller
{
    public class BotRouter
    {
        public TelegramBotClient BotClient { get; set; }
        public Dictionary<string, Type> Controllers { get; set; } = new(); 
        public IControllerContext ControllerContext { get; set; }

        public BotRouter(string token, IControllerContext controllerContext)
        {
            var assembly = Assembly.GetCallingAssembly();

            IEnumerable<Type> types = assembly.GetTypes().Where(i => i.GetCustomAttribute<ControllerRoute>() is not null
            && i.IsSubclassOf(typeof(ControllerBase)));

            if (types.Count() == 0)
                throw new InvalidOperationException("No controllers were found in the project!");

            foreach (var i in types)
            {
                string route = i.GetCustomAttribute<ControllerRoute>()!.Route;
                Controllers.Add(route, i);
            }


            BotClient = new TelegramBotClient(token);
            BotClient.StartReceiving(new UpdateHandler(this));

            ControllerContext = controllerContext;
        }

        public async Task RouteController(Update update, UserContext userContext, ControllerState controllerState)
        {
            ControllerBase? controller = new ControllerBase();

            var type = Controllers[userContext.Controller];

            if (string.IsNullOrWhiteSpace(userContext.JsonContext))
                controller = Activator.CreateInstance(Controllers[userContext.Controller]) as ControllerBase;
            else
                controller = JsonConvert.DeserializeObject(userContext.JsonContext, type) as ControllerBase;

            if (controller is null)
                throw new ArgumentException("Couldn't map controller");

            controller.BotRouter = this;
            controller.ControllerState = controllerState;

            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Unknown:
                    controller.MethodRouter = new UnknownRouting();
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    controller.MethodRouter = new MessageRouter();
                    break;
                default:
                    controller.MethodRouter = new UpdateRouter();
                    break;
            }

            await controller.RouteMethod(update, userContext);

            if (controller.IsRedirected && controllerState == ControllerState.Response)
            {
                await RouteController(new Update(), await ControllerContext.GetContext(userContext.Id), ControllerState.First);
                await RouteController(new Update(), userContext, ControllerState.Last);
            }
        }
        


    }

}