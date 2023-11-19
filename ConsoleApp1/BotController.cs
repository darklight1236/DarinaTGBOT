using Concentus.Oggfile;
using Concentus.Structs;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using GiphyDotNet.Model.Results;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
//using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message = Telegram.Bot.Types.Message;

namespace ConsoleApp1
{
    internal class BotController
    {
        private TelegramBotClient _client;
        private Giphy _giphyClient;
        private ReceiverOptions _receiverOptions;
        private CancellationTokenSource _cancellationTokenSource;

        public BotController()
        {
            _client = new TelegramBotClient("6881118559:AAGCEqghqqweG3WzhUOHhLy8g7xpfk0JXaY");
            _giphyClient = new Giphy("kGxDg6HqOaK8qOIvfauWI4y3ZI3vZ9Ty");
            _cancellationTokenSource = new CancellationTokenSource();
            _receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }
            };
        }

        public void Start()
        {
            _client.StartReceiving(
             updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: _receiverOptions,
            cancellationToken: _cancellationTokenSource.Token);
            Console.WriteLine("Бот запущен!");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            Console.WriteLine("бот выключен! ");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch(update.Message.Type)
            {
                case MessageType.Voice:
                    await HandleVoiceMessage(update.Message);
                    break;
            }
        }
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                Console.WriteLine($"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}");

            }

            return Task.CompletedTask;

        }

        private async Task HandleVoiceMessage(Message message)
        {
            using (FileStream fileStream = new FileStream("../voice_message.ogg", FileMode.Create))
            {
                await _client.GetInfoAndDownloadFileAsync(message.Voice.FileId, fileStream, _cancellationTokenSource.Token);
                await ConvertOggAudioToWav(fileStram);
                string voiceText = await WavAudioToText();
                voiceText = voiceText.ToLower();
                if (voiceText.Contains("выключи пк"))
                {
                    ExceptionTurnOffPCCommand();
                }
                if (voiceText.Contains("перезагрузи пк"))
                {
                    ExceptionRestartPCCommand();

                }
                if (voiceText.Contains("сделай скрин"))
                {
                    await ExecuteTakeScreenShotCommand(message.Chat.Id);
                }
                if (voiceText.Contains("рандомная гифка"))
                {
                    await ExecuteRandomGif Command(message.Chat.Id);
                }
            }
        }
        private async Task<string> WavAudioToText()
        {
            byte[] wavAudioBytes = System.IO.File.ReadAllBytes("../voice_message.wav");
            WebRequest request = WebRequest.Create("");
            request.Method = "POST";
            request.ContentType = "audio/l16; rate=48000";
            request.ContentLength = wavAudioBytes.Length;
            using (Stream requestStream = await request.GetRequestStreamAsync()) 
            {
                await requestStream.WriteAsync(wavAudioBytes, 0, wavAudioBytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
            
            }
        }
    }
}
