/*
 *
 * Osman Yavuz
 * omnyvz.yazilim@gmail.com
 *
 */
using System;
using reCAPTCHA.Resolver.Core;

namespace reCAPTCHA.Resolver.Example
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
