using System;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Serverless
{
    public class OcrClient
    {
        private const TextRecognitionMode textRecognitionMode = TextRecognitionMode.Printed;
        private const int numberOfCharsInOperationId = 36;
        private ComputerVisionClient computerVision;

        public OcrClient(string endpoint, string subscriptionKey)
        {
            computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = endpoint;
        }

        public async Task<RecognitionResult> RecognizeText(string remoteImageUrl)
        {
            return await ExtractRemoteTextAsync(computerVision, remoteImageUrl);
        }

        private async Task<RecognitionResult> ExtractRemoteTextAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                throw new UriFormatException(imageUrl);
            }

            var textHeaders = await computerVision.RecognizeTextAsync(imageUrl, textRecognitionMode);

            return await GetTextAsync(computerVision, textHeaders.OperationLocation);
        }

        private async Task<RecognitionResult> GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
            var result = await computerVision.GetTextOperationResultAsync(operationId);

            int i = 0;
            int maxRetries = 10;

            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                await Task.Delay(1000);

                result = await computerVision.GetTextOperationResultAsync(operationId);
            }

            return result.RecognitionResult;
        }
    }
}
