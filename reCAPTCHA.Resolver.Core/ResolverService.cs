/*
 *
 * Osman Yavuz
 * omnyvz.yazilim@gmail.com
 *
 */
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using reCAPTCHA.Resolver.Core.Abstract;
using reCAPTCHA.Resolver.Core.Concrete;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace reCAPTCHA.Resolver.Core
{
    public class ResolverService
    {
        private readonly ISpeechToTextService _speechToTextService; // IBM speech to text servisi
        private WebDriverWait _webDriverWait; // sürücüde yapılan işlemleri bekletme
        private IWebDriver _webDriver; // web sürücü

        private const int DelayTime = 100; // milisaniye cinsinden

        /// <summary>
        /// Resolver Servisi
        /// </summary>
        /// <param name="webDriver">WebDriver</param>
        /// <param name="apiKey">IBM Speech to Text Api Key</param>
        /// <param name="serviceUrl">IBM Speech to Text Service Url</param>
        public ResolverService(string apiKey, string serviceUrl)
        {
            _speechToTextService = new SpeechToTextManager(apiKey, serviceUrl);
        }

        /// <summary>
        /// Web sürücü aktarma
        /// </summary>
        /// <param name="webDriver"></param>
        public void SetDriver(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            _webDriverWait = new WebDriverWait(
                webDriver,
                new TimeSpan(0, 0, 10)
            );
        }

        /// <summary>
        /// Captcha çözümleme
        /// </summary>
        /// <returns></returns>
        public string GetResolve()
        {
            var result = "false";

            if (_webDriver.PageSource.IndexOf("g-recaptcha", StringComparison.Ordinal) != -1)
            {
                var task = Task.Run(async () =>
                {
                    // captcha tıklama
                    var resultCaptchaClick = CaptchaClick();
                    if (!resultCaptchaClick)
                    {
                        return "Error:CaptchaNotClicked";
                    }

                    // captcha onaylanırsa
                    if (_webDriver.PageSource.IndexOf("recaptcha-checkbox-checkmark", StringComparison.Ordinal) != -1)
                    {
                        return "Success:Captcha";
                    }

                    // captcha çözmek için sese tıklama
                    var resultAudioChallenge = AudioChallengeSubmit();
                    if (!resultAudioChallenge)
                    {
                        return "Error:AudioChallengeNotClicked";
                    }

                    // Try again later - spam
                    if (_webDriver.PageSource.IndexOf("Try again later", StringComparison.Ordinal) != -1)
                    {
                        return "Error:Spam";
                    }

                    // Onaylanıncaya kadar dene
                    do
                    {
                        await RecognizeAgain();
                        await Task.Delay(DelayTime);
                    } while (await RecognizeAgain());

                    await Task.Delay(DelayTime);

                    // captcha onaylanırsa
                    if (_webDriver.PageSource.IndexOf("recaptcha-checkbox-checkmark", StringComparison.Ordinal) != -1)
                    {
                        return "Success:Captcha";
                    }
                    else
                    {
                        return "Error:Captcha";
                    }
                });
                task.Wait();

                result = task.Result;
            }
            else
            {
                result = "Error:CaptchaIsNot";
            }

            return result;
        }

        /// <summary>
        /// Captcha çözümlemesi için tekrar çalışma
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RecognizeAgain()
        {
            // Ses linki
            var resultDownLink = GetAudioUrl();
            if (string.IsNullOrEmpty(resultDownLink))
            {
                //return "Error:AudioLinkNotReceived";
                return true;
            }

            // Sesi indir
            var filePath = ExitsFile();
            
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(
                    new Uri(resultDownLink),
                    filePath
                );
                webClient.Dispose();
            }
            await Task.Delay(DelayTime);

            // Sesi tanıla
            var resultTranscript = _speechToTextService.Recognize(filePath);
            await Task.Delay(DelayTime);

            // Metni gönderme
            var resultCaptchaSolver = GetCaptchaSolver(resultTranscript);
            if (!resultCaptchaSolver)
            {
                //return "Error:CaptchaTranscriptSolve";
                return true;
            }

            await Task.Delay(DelayTime);

            // Onayla
            var resultCaptchaVerifySubmit = CaptchaVerifySubmit();
            if (!resultCaptchaVerifySubmit)
            {
                //return "Error:CaptchaVerifySubmit";
                return true;
            }
            await Task.Delay(DelayTime);

            // rc-audiochallenge-error-message - hatalı
            return _webDriver.PageSource.IndexOf("rc-audiochallenge-error-message", StringComparison.Ordinal) != -1;
        }

        /// <summary>
        /// Captcha onaylama butonuna tıklama
        /// </summary>
        /// <returns></returns>
        private bool CaptchaVerifySubmit()
        {
            try
            {
                _webDriverWait.Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.Id("recaptcha-verify-button")
                    )).Click();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Captcha için çözümlenen sesi kutucuğa aktarma
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        private bool GetCaptchaSolver(string solver)
        {
            try
            {
                _webDriverWait.Until(
                   ExpectedConditions.ElementToBeClickable(
                       By.Id("audio-response")
                   )).SendKeys(solver);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Captcha tıklama
        /// </summary>
        /// <returns></returns>
        private bool CaptchaClick()
        {
            var result = false;
            if (_webDriver.PageSource.IndexOf("grecaptcha-badge", StringComparison.Ordinal) != -1)
            {
                result = true;
            }
            else
            {
                try
                {
                    _webDriver.FindElement(By.XPath("//div[@class='g-recaptcha']//iframe"))
                        .Click();
                    result = true;
                }
                catch
                {
                    var elements = _webDriver.FindElements(By.ClassName("g-recaptcha"));
                    foreach (var element in elements)
                    {
                        element.FindElement(By.TagName("iframe")).Click();
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Captcha'yı sesle çözmek için sese tıklama
        /// </summary>
        /// <returns></returns>
        private bool AudioChallengeSubmit()
        {
            try
            {
                _webDriverWait.Until(
                    ExpectedConditions.FrameToBeAvailableAndSwitchToIt(
                        By.CssSelector("iframe[title='recaptcha challenge']")
                    ));

                _webDriverWait.Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.CssSelector("button#recaptcha-audio-button")
                    )).Click();

                _webDriverWait.Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.XPath("//div[@class='rc-audiochallenge-play-button']//button")
                    )).Click();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Captcha çözümü için verilen sesin indirme linki
        /// </summary>
        /// <returns></returns>
        private string GetAudioUrl()
        {
            try
            {
                var result = _webDriverWait.Until(
                    ExpectedConditions.ElementToBeClickable(
                        By.ClassName("rc-audiochallenge-tdownload-link")
                    )).GetAttribute("href");

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Audio dosyasının konumu
        /// </summary>
        /// <returns></returns>
        private static string ExitsFile()
        {
            var folder = Path.GetTempPath() + @"CaptchaAudio\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var random = new Random();
            var rndNext = random.Next(0, 9999999);


            return folder + rndNext + ".mp3";
        }

    }
}
