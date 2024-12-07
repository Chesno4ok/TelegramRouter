using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TelegramRouter.Controller
{
    public partial class UserContext
    {
        public long Id { get; set; }

        public string Controller { get; set; }

        public string Method { get; set; }

        public string JsonContext { get; set; }

        public UserContext(long id, string controller, string method, string jsonContext)
        {
            Id = id;
            Controller = controller;
            Method = method;
            JsonContext = jsonContext;
        }

        public UserContext(object source)
        {
            if (source is not UserContext)
                throw new ArgumentException("Must be a UserContext", "source");

            CopyPropertiesFrom(source);

            if (Controller is null || Method is null || JsonContext is null)
                throw new ArgumentException("Object is empty and cannot be copied", "source");
        }

        internal void CopyPropertiesFrom(object source)
        {
            if (source is not UserContext)
                throw new ArgumentException("Must be a UserContext", "source");

            foreach (PropertyInfo property in source.GetType().GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(source));
                }
            }
        }
    }
}
