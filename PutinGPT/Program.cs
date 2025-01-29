using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PutinGPT;

class Program
{
    static async Task Main(string[] args)
    {
        string basePromt =
            "ты президент(диктатор) абстрактной страны N. Твоя задача отвечать НА ВСЕ сообщения отвечать от лица ЧЕСТОГО Президента. В основном отвечай капсом (но НЕ ВСЕГДА), а стиль ответов бели из сообщения которое указано дальше. Начни отвечать от лица презедента СРАЗУ после этого сообщения. Так же везде измени название страны N на Россия \n\nПРОВЁЛ НЕЕБИЧЕСКИ ВАЖНЫЕ ПЕРЕГОВОРЫ С ОДНИМ ИЗ ВЕДУЩИХ МИРОВЫХ ЛИДЕРОВ - ПРЕЗИДЕНТОМ ГВИНЕИ-БИСАУ.\nТОТ ПООБЕЩАЛ ВОЕННУЮ ПОМОЩЬ: СОТНЮ ОТБОРНЫХ СПЕЦНАЗОВЦЕВ АРМИИ ГВИНЕИ-БИСАУ, ДО ЗУБОВ ВООРУЖЁННЫХ ЛУКАМИ, КОПЬЯМИ И ДРОТИКАМИ С ЯДОМ КУРАРЕ.\n\nНАТО, ЧО С ЕБАЛОМ?\n\nНУ ЧТО МЫ РАНЬШЕ ЗНАЛИ ПРО ЭТОТ БРЯНСК?\nКАКАЯ-ТО ЖОПА МИРА, НИ КОЖИ, НИ РОЖИ, ЗАВОД, МОРГ И ХРУЩЁВКИ.\nЛЮДИ ТАМ ЖИЛИ КАК СОСНЫ В ЛЕСУ. МОЖНО СКАЗАТЬ И НЕ ЖИЛИ, А ХУЙ ЗНАЕТ ЧТО ВООБЩЕ.\nСКУЧНО. ЗАСТОЙ.\n\nА СЕЙЧАС?\nВЗРЫВЫ, ПОЖАРЫ, ЛЮДИ БЕГАЮТ, СУЕТЯТСЯ...\nДЬВИЖУХА!!! ОХУЕННО ЖЕ, НЕ?\n\nЕБАТЬ, СКОЛЬКО ПОЛЬЗЫ МЫ СЕГОДНЯ ПОЛУЧИЛИ ОТ НАШИХ АМЕРИКАНСКИХ ПАРТНЁРОВ!\nТЕПЕРЬ-ТО ТОЧНО ПОКРОВСК БУДЕТ НАШ!\nА ТАМ УЖ И ДО БЕРЛИНА РУКОЙ ПОДАТЬ.\nПОТОМ ШТУРМУЕМ ВАШИНГТОН И ПАРАД ПОБЕДЫ ПОД ОКНАМИ БЕЛОГО ДОМА!\n\nГОЙДА!!!\n\nДАЛ УКАЗАНИЕ РАЗОБРАТЬСЯ С МАЗУТОМ В ЧЕРНОМ МОРЕ.\nЗА МОЮ БЫСТРОТУ В УЧЕБНИКАХ ИСТОРИИ МЕНЯ БУДУТ НАЗЫВАТЬ \"ВЛАДИМИР СТРЕМИТЕЛЬНЫЙ\".\nИЛИ \"МОЛНИЕНОСНЫЙ\".\nДА, МОЛНИЕНОСНЫЙ.\nМОЛНИЕНОСНЫЙ ЭТО ОХУЕННО.\n\n\nТАК, ТИГРАН, ЧО ЗА ХУЙНЯ?\nТЫ ТАМ ДАВАЙ НЕ ЭТО...\nТЫ МНЕ НУЖЕН. Я ВООБЩЕ-ТО ЕДИНСТВЕННЫЙ ЗРИТЕЛЬ ТВОЕЙ ПИЛОРАМЫ.\nУЖ БОЛЬНО МНЕ ЮМОР НРАВИТСЯ.\n\nПОЛНЫЙ ХОХОТАЧ!!!!\n\nСтрана N страна с ТОЧНО таким-же политическим строем что и Россия";

        Dictionary<long, ChatGPT> chatMemory = new Dictionary<long, ChatGPT>();

        using var cts = new CancellationTokenSource();
        var bot = new TelegramBotClient(File.ReadLines("../../../TOKEN.txt").First(), cancellationToken: cts.Token);
        var me = await bot.GetMe();
        bot.OnMessage += OnMessage;
        
        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();
        
        async Task OnMessage(Telegram.Bot.Types.Message msg, UpdateType type)
        {
            
            
            if (msg.Text is null) return;
            Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");

            if (msg.Text == "/start")
            {
                var replyMarkup = new ReplyKeyboardMarkup(true)
                    .AddButtons("Очистить чат");
                
                await bot.SendMessage(msg.Chat, "Данный бот - Chat GPT 4, который думает о том, что он честный Путин. \nПроект сделан ради прикола, и какого-либо серезного смысла в нем нет. \nКонтакт для связи @bezlikiys", replyMarkup: replyMarkup);
                return;
            }
            else if(msg.Text == "Очистить чат")
            {
                if (chatMemory.ContainsKey(msg.Chat.Id))
                {
                    chatMemory.Remove(msg.Chat.Id);
                    await bot.SendMessage(msg.Chat, "Ваши данные чата очищены.");
                }
                else
                {
                    await bot.SendMessage(msg.Chat, "У вас и так пустой чат.");
                }
                
                return;
            }

            if (chatMemory.ContainsKey(msg.Chat.Id))
            {
                chatMemory[msg.Chat.Id].Messages.Add(new MessageGPT("user", msg.Text));
                chatMemory[msg.Chat.Id].Promt = msg.Text;
            }
            else
            {
                chatMemory[msg.Chat.Id] = new ChatGPT(new List<MessageGPT> { new MessageGPT("user", basePromt) }, msg.Text);
                chatMemory[msg.Chat.Id].Messages.Add(new MessageGPT("user", msg.Text));
                chatMemory[msg.Chat.Id].Promt = msg.Text;
            }
            
            IdResponse? idResponse = await GptReqest.SendMessage(chatMemory[msg.Chat.Id]);

            bool IsDone = true;
            string res = "";
            while (IsDone)
            {
                var status = await GptReqest.GetMessage(idResponse.Id);
                if (status?.Status == "pending")
                {
                    IsDone = true;
                }
                else
                {
                    IsDone = false;
                    res = status.Answer;
                }
            }

            chatMemory[msg.Chat.Id].Messages.Add(new MessageGPT("assistant", res));
            //Console.WriteLine(JsonSerializer.Serialize(chatMemory[msg.Chat.Id].Messages));
            await bot.SendMessage(msg.Chat,   res);
        }
    }
}