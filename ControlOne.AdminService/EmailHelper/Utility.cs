using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailHelper
{
    static class Utility
    {
        public static MailAddressCollection MakeBcc(MailMessage message, List<Tuple<string, string>> toList)
        {
            MailAddressCollection r = new MailAddressCollection();
            toList.ForEach(i =>
            {
                message.Bcc.Add(new MailAddress(i.Item1, i.Item2));
            });
            return r;
        }
        public static MailAddressCollection MakeBcc(MailMessage message, List<string> list)
        {
            MailAddressCollection r = new MailAddressCollection();
            list.ForEach(i =>
            {
                message.Bcc.Add(new MailAddress(i));
            });
            return r;
        }
    }
}