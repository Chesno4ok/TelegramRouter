using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramRouter.Controller
{
    public interface IControllerContext
    {
        public Task<UserContext?> GetContext(long chatId);
        public Task<UserContext> SetContext(UserContext context);
        public Task<UserContext> CreateContext(UserContext context);
    }
}
