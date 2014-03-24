using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Branding365Web.Services
{
    public class Logger : ISingletonService
    {
        private StringBuilder builder = new StringBuilder();

        public void Append(string format, params object[] args)
        {
            builder.AppendFormat(format, args);
        }

        public void AppendLine(string format, params object[] args)
        {
            builder.AppendFormat(format, args);
            builder.AppendLine();
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public void Clear()
        {
            builder.Clear();
        }
    }
}