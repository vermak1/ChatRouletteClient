using System;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ChatRouletteClient
{
    public class Client : IDisposable
    {
        private readonly Socket _socket;

        private const String SERVER_URL = "127.0.0.1";
        
        private const Int32 SERVER_PORT = 6666;

        private const Int32 BUFFER_SIZE = 512;

        private ExceptionDispatchInfo _dispatchInfo;

        public string Name { get; }

        public Client(string name)
        {
            Name = name;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        

        public async Task<Boolean> ConnectToServer()
        {
            try
            {
                await _socket.ConnectAsync(SERVER_URL, SERVER_PORT);
                if (_socket.Connected)
                {
                    Console.WriteLine("Successfully connected");
                    _socket.Send(Encoding.UTF8.GetBytes(Name));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return _socket.Connected;
        }

        private String ReceiveMessage()
        {
            Byte[] buffer = new Byte[BUFFER_SIZE];
            StringBuilder sb = new StringBuilder();
            try
            {
                do
                {
                    Int32 bytes = _socket.Receive(buffer, BUFFER_SIZE, 0);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                } 
                while (_socket.Available != 0);
            }
            catch
            {
                throw;
            }
            return sb.ToString();
        }

        public void StartReceiveMessagesCycle()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                try
                {
                    String message;
                    while (true)
                    {
                        message = ReceiveMessage();
                        MessageProcessor.VerifyAndPrintMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Receiveing messages failed\nError: {0}", ex.Message);
                    _dispatchInfo = ExceptionDispatchInfo.Capture(ex);
                }
            });
        }

        public void StartSendMessagesCycle()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                try
                {
                    while (true)
                    {
                        String message = Console.ReadLine();
                        SendMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Sending messages failed\nError: {0}", ex.Message);
                    _dispatchInfo = ExceptionDispatchInfo.Capture(ex);
                }
            });
        }

        private void SendMessage(String message)
        {
            try
            {
                Byte[] buffer = Encoding.UTF8.GetBytes(message);
                _socket.Send(buffer, buffer.Length, SocketFlags.None);
            }
            catch
            {
                throw;
            }
        }

        public void ExceptionHandleCycle()
        {
            while (true)
            {
                if (_dispatchInfo != null)
                    _dispatchInfo.Throw();
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}
