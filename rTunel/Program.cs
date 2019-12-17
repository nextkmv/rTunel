/* rTunel
 *
 * Параметры командной строки
 * -from-ip - ip адрес на котором мы принимаем соединение
 * -from-port - порт на котором мы принимаем соединение
 * -to-ip - ip адрес на который мы перенаправляем соединение
 * -to-port - порт на который мы перенаправляем соединение
 * -protocol - используемый протокол, допустимы значения tcp
 * -encryption - используемое шифрование, допустимо значение none, XOR
 * -encryption-private-key - строка ключа шифрования
 */
using System;
using System.Threading.Tasks;
using rTunel.ProxyServer;



namespace rTunel.CoreApp
{


    class Program
    {
        
        static void Main(string[] args)
        {
            AppConfiguration appConfiguration = new AppConfiguration(args);

            Console.WriteLine("from ip -  " + appConfiguration.GetEndPoints().FromIp);
            Console.WriteLine("from port  - " + appConfiguration.GetEndPoints().FromPort);
            Console.WriteLine("to ip - " + appConfiguration.GetEndPoints().ToIp);
            Console.WriteLine("to port - " + appConfiguration.GetEndPoints().ToPort);
            Console.WriteLine("protocol - " + appConfiguration.GetEndPoints().Protocol);
            Console.WriteLine("encryption method - " + appConfiguration.GetCredential().Method);

            Server server = new Server(appConfiguration.GetEndPoints(),appConfiguration.GetCredential());
            Task.WhenAll(server.Start()).Wait();
        }
    }
}
