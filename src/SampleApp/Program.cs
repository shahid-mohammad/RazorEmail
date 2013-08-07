﻿using System;
using System.Collections.Generic;
using System.Configuration;
using RazorEmail;

namespace SampleApp
{
    class Program
    {
        static void Main()
        {
            var mySampleModel = new
                        {
                            Link = "http://www.jobping.com",

                            RecentActivity = new List<Tuple<DateTime, string>>
                                                 {
                                                     Tuple.Create(new DateTime(2009,7,4,16,49,23), "Signed up to Jobping"),
                                                     Tuple.Create(new DateTime(2010,1,13,16,49,23), "Created an api toke"),
                                                     Tuple.Create(new DateTime(2011,4,3,16,49,23), "Forgot your password & we sent a reset link to xyz@abc.com"),
                                                     Tuple.Create(new DateTime(2020,2,12,16,49,23), "Found a bug with the date")
                                                 }
                        };

	        var templateDir = ConfigurationManager.AppSettings["razor.email.base.dir"];

	        var mailer = new RazorMailer(templateDir);

	        mailer.Create("Welcome", new {Link = "http://www.jobping.com"}, "Reset Password Request @Model.Link", "john.doe@test.com", "John Doe")
	              .ToMailMessage()
	              .Send();

	        mailer.Create("ForgotPassword", mySampleModel, "Reset Password Request @Model.Link", "john.doe@test.com", "John Doe")
	              .WithHeader("X-RazorMail-Send-At", DateTime.Now.ToLongTimeString())
	              .ToMailMessage()
	              .SendAsync((x, m) =>
		              {
			              Console.WriteLine(x);
			              Console.WriteLine("Message Subject: {0}, Send around: {1}", m.Subject, m.Headers["X-RazorMail-Send-At"]);
		              },
	                         "Sent John Doe his forgot password message");
        }
    }
}
