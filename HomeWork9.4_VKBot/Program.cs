using System;

namespace HomeWork9._4_VKBot
{
    class Program
    {
        static void Main(string[] args)
        {
            BotWorker botWorker = new BotWorker();
            Console.WriteLine("Бот запущен!");
            while (true)
            {
                botWorker.Start();
            }

        }
    }
}
