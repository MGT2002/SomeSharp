using MyExtensions;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int DefaultEchoPort = 7;

if (args.Length > 1)
    throw new ArgumentException("Parameters: <Port>");

int servPort = args.Length == 1 ? int.Parse(args[0]) : DefaultEchoPort;

UdpClient client = null!;

try
{
    client = new(servPort);
}
catch (SocketException e)
{
    e.ErrorCode.Log(": ", e.Message);
    Environment.Exit(e.ErrorCode);
}

IPEndPoint remote = null!;
while (true)
    ReceiveAndEchoDatagrams();

void ReceiveAndEchoDatagrams()
{
    try
    {
        $"Listening to the port {servPort}...".Log();
        byte[] rcvdData = client.Receive(ref remote);
        $"{rcvdData.Length} bytes received:".Log(
            Encoding.ASCII.GetString(rcvdData));

        $"Handling client at {remote} - ".LogW();
        client.Send(rcvdData, rcvdData.Length, remote);
        $"Echoed {rcvdData.Length} bytes".Log();
    }
    catch (SocketException e)
    {
        e.ErrorCode.Log(": ", e.Message);
    }
    catch (Exception e)
    {
        e.Message.Log();
    }
}