using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using GolioFunctions.DTOs;
using GolioFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GolioFunctions
{
    public class SuggestionNotification
    {
        private readonly ILogger<SuggestionNotification> _logger;
        private readonly IEmailService _emailService;

        public SuggestionNotification(ILogger<SuggestionNotification> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [Function(nameof(SuggestionNotification))]
        public async Task Run(
            [ServiceBusTrigger("my-queue", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation($"C# Queue trigger function started");

            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var suggestionVoteDTO = JsonSerializer.Deserialize<SuggestionVoteDTO>(message.Body);

            var template = new EmailTemplateDTO
            {
                Subject = "Olá, sua sugestão no Golio foi {0}!",
                Content = "Após a avaliação de outros usuários, sua sugestão foi {0}. Agradecemos por sua interação conosco e aguardamos pela próxima!",
            };

            var subject = string.Format(template.Subject!, suggestionVoteDTO!.IsValid ? "aceita" : "rejeitada");
            var content = string.Format(template.Content!, suggestionVoteDTO.IsValid ? "aceita" : "rejeitada");

            try
            {
                _emailService.SendEmail(suggestionVoteDTO.SuggestionAutor!, subject, content);
                _logger.LogInformation("E-mail successfully sent");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending the e-mail: {ex.Message}");
            }

            await messageActions.CompleteMessageAsync(message);
        }
    }
}
