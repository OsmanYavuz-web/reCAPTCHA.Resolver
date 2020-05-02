# reCAPTCHA.Resolver
Google reCaptcha Resolver


## IBM Speech to Text Servisi İle Google reCaptcha Çözme
https://www.ibm.com/tr-tr/cloud/watson-speech-to-text


## Örnek
```
// Browser
IBrowserService browserService = new BrowserManager();
browserService.Request("https://www.google.com/recaptcha/api2/demo");


// reCaptcha.Resolver
ResolverService resolverService = new ResolverService(
    "IBM Speech to Text Api Key",
    "IBM Speech to Text Service Url"
);

// Set WebDriver
resolverService.SetDriver(browserService.Driver);

// Result
var result = resolverService.GetResolve();
Console.WriteLine(result);

Console.ReadLine();
```
