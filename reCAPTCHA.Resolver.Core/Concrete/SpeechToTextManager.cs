/*
 *
 * Osman Yavuz
 * omnyvz.yazilim@gmail.com
 *
 */
using System;
using System.IO;
using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Cloud.SDK.Core.Http.Exceptions;
using IBM.Watson.SpeechToText.v1;
using Newtonsoft.Json.Linq;
using ISpeechToTextService = reCAPTCHA.Resolver.Core.Abstract.ISpeechToTextService;

namespace reCAPTCHA.Resolver.Core.Concrete
{
    public class SpeechToTextManager : ISpeechToTextService
    {
        private readonly SpeechToTextService _speechToTextService;


        public SpeechToTextManager(string apiKey, string serviceUrl)
        {
            try
            {
                var authenticator = new IamAuthenticator(
                    apikey: apiKey
                );
                _speechToTextService = new SpeechToTextService(authenticator);
                _speechToTextService.SetServiceUrl(serviceUrl);
                SetModel();
            }
            catch (ServiceResponseException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public void SetModel(string modelId = "en-US_BroadbandModel")
        {
            _speechToTextService.GetModel(modelId: modelId);
        }


        public string Recognize(string audioPath)
        {
            var result = _speechToTextService.Recognize(
                audio: File.ReadAllBytes(audioPath),
                contentType: "audio/mp3"
            );

            var jsonJObject = JObject.Parse(result.Response);
            return (string)jsonJObject["results"][0]["alternatives"][0]["transcript"];
        }

    }
}