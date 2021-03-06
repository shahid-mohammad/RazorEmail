RazorMail
=========

Razor Email is a email templating library that uses the RazorEngine for templating services.

You specify you email in an xml file, e.g. ForgotPassword.xml

	<?xml version="1.0" encoding="utf-8" ?>
	<email>
		<subject>Reset Password Request @Model.Link</subject>
		<from display="jobping">noreply@jobping.com</from>
		<cc />
		<bcc />
	</email>

The xml is used to specify the template for your email. Email bodies can be added inline (into the xml) or as external files.
By default the engine will look for an external razor templates..

 -	A html view called: ForgotPassword.text_html.cshtml 
 -	A text view called: ForgotPassword.text_plain.cshtml

Sending an email looks like this: 

	 RazorMailer.Build("ForgotPassword",  new { Link = "http://www.jobping.com" },"john.doe@test.com", "John Doe")
				.ToMailMessage()	
				.Send();



Or a move involved example: 

	var mySampleModel = new
	{
		Link = "http://www.jobping.com",

		RecentActivity = new List<Tuple<DateTime, string>>
							 {
								 {Tuple.Create(new DateTime(2009,7,4,16,49,23), "Signed up to Jobping")},
								 {Tuple.Create(new DateTime(2010,1,13,16,49,23), "Created an api toke")},
								 {Tuple.Create(new DateTime(2011,4,3,16,49,23), "Forgot your password & we sent a reset link to xyz@abc.com")},
								 {Tuple.Create(new DateTime(2020,2,12,16,49,23), "Found a bug with the date")}
							 }
	};
                    
	RazorMailer .Build("ForgotPassword", mySampleModel,"john.doe@test.com", "John Doe")
				.WithHeader("X-RazorMail-Send-At", DateTime.Now.ToLongTimeString())
				.ToMailMessage()
				.SendAsync( (x, m) =>
								{
									Console.WriteLine(x);
									Console.WriteLine("Message Subject: {0}, Send around: {1}", m.Subject, m.Headers["X-RazorMail-Send-At"]);
								}, 
								"Sent John Doe his forgot password message");

There is a console app sample and a test project for further reference. Nuget: RazorMail