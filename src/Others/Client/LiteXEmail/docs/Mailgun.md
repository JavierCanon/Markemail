# LiteX Email Mailgun
> LiteX.Email.Mailgun is a email message library which is based on LiteX.Email.Core and Mailgun API.

Allow sending email messages via Mailgun.

Wrapper around Mailgun api to send email messages from any type of application.

Small library for manage email with Mailgun. A quick setup for Mailgun.

Wrapper library is just written for the purpose to bring a new level of ease to the developers who deal with Mailgun integration with your system.



## Basic Usage

### Install the package

> Install via [Nuget](https://www.nuget.org/packages/LiteX.Email.Mailgun/).

```Powershell
PM> Install-Package LiteX.Email.Mailgun
```

##### AppSettings
```js
{  
  //LiteX Mailgun settings
  "MailgunConfig": {
    "ApiKey": "api:key-fakeapikey",
    "ApiBaseUri": "https://api.mailgun.net/v3/",
    "RequestUri": "fakesandbox.mailgun.org/messages",
    "From": "postmaster@fakesandbox.mailgun.org",
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
        // 1. Use default configuration from appsettings.json's 'MailgunConfig'
        services.AddLiteXMailgunEmail();

        //OR
        // 2. Load configuration settings using options.
        services.AddLiteXMailgunEmail(option =>
        {
            option.ApiKey = "";
            option.ApiBaseUri = "";
            option.RequestUri = "";
            option.From = "";
            option.EnableLogging = true;
        });

        //OR
        // 3. Load configuration settings on your own.
        // (e.g. appsettings, database, hardcoded)
        var mailgunConfig = new MailgunConfig()
        {
            ApiKey = "",
            ApiBaseUri = "",
            RequestUri = "",
            From = "",
            EnableLogging = true
        };
        services.AddLiteXMailgunEmail(mailgunConfig);
        
        
        // add logging (optional)
        services.AddLiteXLogging();
    }
}
```

### Sample Usage Example
> Same for all providers. 

For more helpful information about LiteX Email, Please click [here.](https://github.com/a-patel/LiteXEmail/blob/master/README.md#step-3--use-in-controller-or-business-layer-memo)


