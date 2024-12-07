using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TelegramRouter.Attributes
{
    public class ControllerRoute(string route) : Attribute
    {
        public string Route { get; set; } = route;
    }
}
