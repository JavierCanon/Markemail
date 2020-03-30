# LiteX Email AmazonSES
> LiteX.Email.AmazonSES is a email message library which is based on LiteX.Email.Core and AmazonSES.

Allow sending email messages via AmazonSES.

Wrapper around AmazonSES api to send email messages from any type of application.

Small library for manage email with AmazonSES. A quick setup for AmazonSES.

Wrapper library is just written for the purpose to bring a new level of ease to the developers who deal with AmazonSES integration with your system.



## Basic Usage

### Install the package

> Install via [Nuget](https://www.nuget.org/packages/LiteX.Email.AmazonSES/).

```Powershell
PM> Install-Package LiteX.Email.AmazonSES
```

##### AppSettings
```js
{  
  //LiteX AmazonSES settings
  "AmazonSESConfig": {
    "AmazonSESAccessKey": "--- REPLACE WITH YOUR AmazonSESAccessKey ---",
    "AmazonSESSecretKey": "--- REPLACE WITH YOUR AmazonSESSecretKey ---",
    "AmazonRegion": "--- REPLACE WITH YOUR AmazonRegion ---",
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
        // 1. Use default configuration from appsettings.json's 'AmazonSESConfig'
        services.AddLiteXAmazonSESEmail();

        //OR
        // 2. Load configuration settings using options.
        services.AddLiteXAmazonSESEmail(option =>
        {
            option.AmazonSESAccessKey = "";
            option.AmazonSESSecretKey = "";
            option.AmazonRegion = "";
            option.EnableLogging = true;
        });

        //OR
        // 3. Load configuration settings on your own.
        // (e.g. appsettings, database, hardcoded)
        var amazonSESConfig = new AmazonSESConfig()
        {
            AmazonSESAccessKey = "",
            AmazonSESSecretKey = "",
            AmazonRegion = "",
            EnableLogging = true
        };
        services.AddLiteXAmazonSESEmail(amazonSESConfig);
        
        
        // add logging (optional)
        services.AddLiteXLogging();
    }
}
```

### Sample Usage Example
> Same for all providers. 

For more helpful information about LiteX Email, Please click [here.](https://github.com/a-patel/LiteXEmail/blob/master/README.md#step-3--use-in-controller-or-business-layer-memo)


