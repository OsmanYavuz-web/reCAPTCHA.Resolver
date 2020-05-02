/*
 *
 * Osman Yavuz
 * omnyvz.yazilim@gmail.com
 *
 */
namespace reCAPTCHA.Resolver.Core.Abstract
{
    public interface ISpeechToTextService
    {
        void SetModel(string modelId = "en-US_BroadbandModel");
        string Recognize(string audioPath);
    }
}
