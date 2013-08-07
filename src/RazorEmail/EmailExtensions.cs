﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace RazorEmail
{
    public static class EmailExtensions
    {
        public static Email WithHeader(this Email email, string key, string value)
        {
            email.Headers.Add(new Email.Header {Key = key, Value = value});
            return email;
        }

        public static MailMessage ToMailMessage(this Email email, Action<Email> action)
        {
            action(email);
           return  email.ToMailMessage();
        }

        public static MailMessage ToMailMessage(this Email email)
        {
            var message = new MailMessage();

            if (email.From != null)
            {
                message.From = email.From.ToMailAddress("From address is null");
            }

            if (email.To != null)
            {
                foreach (var to in email.To)
                {
                    message.To.Add(to.ToMailAddress("To address is null"));
                }
            }

            if (email.ReplyTo != null)
            {
                message.ReplyToList.Add(email.ReplyTo.ToMailAddress());
            }
            
            if (email.Bcc != null)
            {
                foreach (var bcc in email.Bcc)
                    message.Bcc.Add(bcc.ToMailAddress("Bcc address is null"));
            }

            if (email.CC != null)
            {
                foreach (var cc in email.CC)
                {
                    message.CC.Add(cc.ToMailAddress("CC address is null"));
                }
            }

            if(email.Headers != null)
            {
                foreach (var header in email.Headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            message.Subject = email.Subject;

            if (email.Views != null)
            {
                foreach (var view in email.Views)
                {
                    message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(view.Content,
                                                                                           view.Encoding ??
                                                                                           Encoding.Default,
                                                                                           view.MediaType));
                }
            }

            return message;
        }

        public static void Send(this MailMessage message, IEmailSender sender)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (sender == null) throw new ArgumentNullException("sender");
            sender.Send(message);
        }

        public static void Send(this MailMessage message)
        {
            using (var client = SimpleSmtpSender.NewClient())
            {
                client.Send(message);
            }
        }

        public static void SendAsync(this MailMessage message)
        {
            message.SendAsync(x => { }, 0);
        }

        public static void SendAsync<T>(this MailMessage message, Action<T> action, T actionStateArgument)
        {
            message.SendAsync((arg, m) => action(arg), actionStateArgument);
        }

        public static void SendAsync<T>(this MailMessage message, Action<T, MailMessage> action, T actionStateArgument)
        {
            var client = SimpleSmtpSender.NewClient();

            client.SendCompleted += (sender, args) =>
            {
                action(actionStateArgument, message);
                client.Dispose();
            };
            client.SendAsync(message, actionStateArgument);
        }

        
    }
}
