/*
 *
 * Osman Yavuz
 * omnyvz.yazilim@gmail.com
 *
 */
using OpenQA.Selenium.Chrome;

namespace reCAPTCHA.Resolver.Example
{
    public interface IBrowserService
    {
        ChromeDriver Driver { get; set; }
        ChromeDriverService Service { get; set; }
        ChromeOptions Options { get; set; }
        void Request(string url);
        void LoadSetting();
        void BrowserQuit();
        string PageSource();
    }
}
