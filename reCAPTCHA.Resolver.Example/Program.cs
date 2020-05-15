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
            //browserService.Request("https://patrickhlauke.github.io/recaptcha/");
            //browserService.Request("https://recaptcha-demo.appspot.com/recaptcha-v2-checkbox-explicit.php");
            //browserService.Request("https://recaptcha-demo.appspot.com/recaptcha-v2-checkbox.php");


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
            if (string.IsNullOrEmpty(result))
            {
                Console.WriteLine("false");
            }
            else
            {
                Console.WriteLine("true");
            }
            

            Console.ReadLine();
        }
    }
}
