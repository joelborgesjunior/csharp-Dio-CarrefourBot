using System;
using Telegram.Bot;

namespace DioCarrefourBot
{    
    class Program
    {
        //Token do telegram Bot
        public static readonly ITelegramBotClient botClient = new TelegramBotClient("Coloque o token do Telegram Bot aqui");  

        static void Main(string[] args)
        {               
            //Bot iniciado                    
            botClient.StartReceiving();
            //Bot recebendo o Evento 
            botClient.OnMessage += DialogFlow.sendMessage;                  
            //Ao iniciar o bot, ele só fechará depois de executa uma tecla
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();            
            //Bot parando de receber ações
            botClient.StopReceiving();
        }           
        
    }
}
