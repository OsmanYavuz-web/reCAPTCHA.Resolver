/*
 *
 * Osman Yavuz
 * omnyvz.yazilim@gmail.com
 *
 */
using OpenQA.Selenium.Chrome;

namespace reCAPTCHA.Resolver.Example
{
    public class BrowserManager : IBrowserService
    {
        public ChromeDriver Driver { get; set; }
        public ChromeDriverService Service { get; set; } = ChromeDriverService.CreateDefaultService();
        public ChromeOptions Options { get; set; } = new ChromeOptions();

        public BrowserManager()
        {
            // Tarayıcı ayarlarını aktarma
            LoadSetting();
            Driver = new ChromeDriver(Service, Options);
        }

        public void LoadSetting()
        {
            // Driver servis ayarları
            Service.HideCommandPromptWindow = true; // komut satırı gizleme

            // Parametreler
            Options.AddArguments("--incognito"); // gizli tarayıcı
            Options.AddArgument("--disable-default-apps"); // varsayılan appleri kaldırma
            Options.AddArgument("--disable-extensions"); // varsayılan appleri kaldırma
            Options.AddArgument("--disable-gpu"); // gpu kullanımını kaldırma
            Options.AddExcludedArgument("enable-automation"); // info bar gizleme
            Options.AddAdditionalCapability("useAutomationExtension", false); // otomatik eklentiler                                      
            Options.AddArgument("--lang=en-US"); // dil ayarı
        }

        public void BrowserQuit()
        {
            try
            {
                Driver.Close();
                Driver.Quit();
                Driver.Dispose();
            }
            catch
            {
                Driver.Quit();
            }
        }

        

        public void Request(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public string PageSource()
        {
            return Driver?.PageSource;
        }

    }
}
