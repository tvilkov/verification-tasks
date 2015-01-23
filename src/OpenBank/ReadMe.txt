PREREQUIREMENTS
	Windows 7 (or above)
	.NET 4.5 
	VS 2012 (or above) + Nuget 2.8 (or above)
	ASP.NET MVC 4 (or above)
	MSSQL SERVER 2005 (or above) + MSSQL SERVER Management Studio
	IISExpress/IIS 7 (or above)

SETUP GUIDE

OPTION 1
	This requires IISExpress to be installed and access to MSSQL standart port at 466de2fe-db50-4694-8889-a1a40137808b.sqlserver.sequelizer.com (cloud SQLServer) must be granted in your Firewall.
	Also note that the password specified to acess cloud DB is a subject to change over the next 5 days.
	1. Open OpenBank.sln solution in Visual Studio
	2. Build it and hit F5 to run. You should see index page with data graph. 

OPTION 2
	1. Open OpenBank.sln solution in Visual Studio
	2. Open Web.config file from OpenBank.Web project and make the following changes:
		a) Change connection string 'main' to point at your database, e.g.
			<add name="main" providerName="System.Data.SqlClient" connectionString="Data Source=<your.host.name>;Initial Catalog=OpenBankDb;Integrated Security=SSPI;" />
			or (for SQLServer authentication)
			<add name="main" providerName="System.Data.SqlClient" connectionString="Data Source=<your.host.name>;Initial Catalog=OpenBankDb;User Id=<yourUsername>;Password=<yourPassword>" />
		b) In appSettings section change the value of parameter 'UseInMemoryStorage' to 'false'. 
		   This tells the application to use SQL Server storage instead of in-memory one.
		c) There is also a parameter named 'DataGenerationPeriod'. It's value is a number of seconds between sequential data generation acts.  
	3. Build the solution using Release configuration. 
	   Note that the solution uses Nuget package manager and is configured for automatic Nuget packages restore. 
	   So it can take some time for VS to download missing packages on the first build.
	4. Open MSSQL Management Studio, connect to your SQL Server and create database named OpenBankDb.
	   Create login and map it to DB user. If you're going to use integrated security mode skip user creation.
	   The application creates required db structure on the first run so no futher actions regarding db setup are required.
	5. If you have IISExpress installed you can run the application by hitting F5 button.
	   It should open your default browser with index page of the application.
	   If IISExpress is not installed - see 6, 7.
	6. Open IIS console and create a new web site. 
	  Set content path of the site to point at the folder %SolutionDir%/OpenBank.Web, where %SolutionDir% - full path to the solution's folder on your machine.
	  Setup site's http binding to be at any free port available on your machine. Then run the site.
	7. Open http://localhost:<yourPort> in your browser. You should see index page of the application.

