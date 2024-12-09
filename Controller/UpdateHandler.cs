using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TelegramRouter.Controller
{
    internal class UpdateHandler : IUpdateHandler
    {
        BotRouter BotRouter;

        public UpdateHandler(BotRouter botRouter)
        {
            BotRouter = botRouter;
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);

            await Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            User? user = GetUser(update);

            if (user == null)
                return;

            UserContext? userContext = await BotRouter.ControllerContext.GetContext(user.Id);

            if(userContext is null)
            {
                var newContext = new UserContext(user.Id, "start", "start", "{}");

                await BotRouter.ControllerContext.CreateContext(newContext);
                userContext = newContext;
            }

            if (IsCommand(update))
            {
                await BotRouter.RouteController(new Update(), userContext, ControllerState.Last);

                var rx = new Regex(@"/\w*");

                var command = rx.Match(update.Message.Text).Value;

                userContext.Controller = "commands";
                userContext.Method = command;


                await BotRouter.RouteController(new Update(), userContext, ControllerState.First);
                update.Message.Text = rx.Replace(update.Message.Text, "");
            }


            await BotRouter.RouteController(update, userContext, ControllerState.Response);
        }

        public User? GetUser(Update update)
        {
            PropertyInfo? property = update.GetType()
                .GetProperties()
                .FirstOrDefault(i => i.GetValue(update) != null && i.PropertyType != typeof(int));
            
            if (property is null)
                return null;

            var message = property.GetValue(update);

            var user = property.PropertyType
                .GetProperties()
                .FirstOrDefault(i => i.PropertyType == typeof(User) && i.Name == "From")?.GetValue(message);

            return user as User ?? null;
        }

        private bool IsCommand(Update update)
        {
            if (update.Message is null || update.Message.Type != MessageType.Text)
                return false;

            var rx = new Regex(@"/\w*");
            return rx.IsMatch(update.Message.Text!);
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            await Task.CompletedTask;
        }
    }
}
