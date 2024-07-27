using System.Net.Sockets;
using System.Text;

if (args.Length < 2 || args.Length > 3)
    throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");

string server = args[0];//Server

byte[] byteBuffer = Encoding.ASCII.GetBytes(args[1]);//Word

int servPort = args.Length == 3 ? int.Parse(args[2]) : 7;

try
{
    using TcpClient client = new(server, servPort);

    Console.WriteLine("Connected to server!\nSending echo string...");

    using NetworkStream netStream = client.GetStream();
    netStream.Write(byteBuffer, 0, byteBuffer.Length);

    Console.WriteLine($"Sent {byteBuffer.Length} bytes to server!" +
        $"\nReceiving bytes...");

    int totalBytesRcvd = 0;
    int bytesRcvdn = 0;
    byte[] bufferRcvd = new byte[byteBuffer.Length];

    while (totalBytesRcvd < bufferRcvd.Length)
    {
        if ((bytesRcvdn = netStream.Read(bufferRcvd, totalBytesRcvd,
            bufferRcvd.Length - totalBytesRcvd)) == 0)
        {
            Console.WriteLine("Connection closed prematurely");
            break;
        }

        totalBytesRcvd += bytesRcvdn;
    }

    Console.WriteLine($"Received {totalBytesRcvd} bytes from server: " +
        $"{Encoding.ASCII.GetString(bufferRcvd, 0, bufferRcvd.Length)}");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}