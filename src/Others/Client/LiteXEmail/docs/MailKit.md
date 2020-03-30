# LiteX Email MailKit
> LiteX.Email.MailKit is a email message library which is based on LiteX.Email.Core and MailKit.

Allow sending email messages via MailKit.

Wrapper around MailKit api to send email messages from any type of application.

Small library for manage email with MailKit. A quick setup for MailKit.

Wrapper library is just written for the purpose to bring a new level of ease to the developers who deal with MailKit integration with your system.



## Basic Usage

### Install the package

> Install via [Nuget](https://www.nuget.org/packages/LiteX.Email.MailKit/).

```Powershell
PM> Install-Package LiteX.Email.MailKit
```

##### AppSettings
```js
{  
  //LiteX MailKit settings
  "MailKitConfig": {
    "Email": "--- REPLACE WITH YOUR Email ---",
    "DisplayName": "--- REPLACE WITH YOUR DisplayName ---",
    "Host": "--- REPLACE WITH Host Host ---",
    "Port": 587, //"--- REPLACE WITH YOUR Port (int) ---",
    "Username": "--- REPLACE WITH YOUR Username ---",
    "Password": "--- REPLACE WITH YOUR Password ---",
    "EnableSsl": false,
    "UseDefaultCredentials": false,
    "EnableLogging": true
  }
}
```

##### Configure Startup Class
```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Use default configuration from appsettings.json's 'MailKitConfig'
        services.AddLiteXMailKitEmail();

        //OR
        // 2. Load configuration settings using options.
        services.AddLiteXMailKitEmail(option =>
        {
            option.Host = "";
            option.Username = "";
            option.Password = "";
            option.Port = 443;
            option.Email = "";
            option.EnableSsl = true;
            option.UseDefaultCredentials = true;
            option.DisplayName = "";
            option.EnableLogging = true;
        });

        //OR
        // 3. Load configuration settings on your own.
        // (e.g. appsettings, database, hardcoded)
        var mailKitConfig = new MailKitConfig()
        {
            Host = "",
            Username = "",
            Password = "",
            Port = 443,
            Email = "",
            EnableSsl = true,
            UseDefaultCredentials = true,
            DisplayName = "",
            EnableLogging = true
        };
        services.AddLiteXMailKitEmail(mailKitConfig);
        
        
        // add logging (optional)
        services.AddLiteXLogging();
    }
}
```

### Sample Usage Example
> Same for all providers. 

For more helpful information about LiteX Email, Please click [here.](https://github.com/a-patel/LiteXEmail/blob/master/README.md#step-3--use-in-controller-or-business-layer-memo)


