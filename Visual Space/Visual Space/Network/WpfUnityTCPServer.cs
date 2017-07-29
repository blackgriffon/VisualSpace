using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nollan.Visual_Space.Network
{
    class WpfUnityTCPServer
    {
        // 송신 수신을 위한 데이터
        Queue<WpfUnityPacketHeader> recevieDataQueue = new Queue<WpfUnityPacketHeader>();
        Queue<WpfUnityPacketHeader> sendDataQueue = new Queue<WpfUnityPacketHeader>();

        // 전송완료 이벤트
        public event Action<WpfUnityPacketHeader> OnReceviedCompleted;

        // 클라이언트와 통신할 소켓
        TcpListener _Listener = null;
        TcpClient _Client = null;

        // 직렬화 후 서버에 데이터 보낼때 사용되는 NetworkStream
        NetworkStream _NetworkStream = null;

        // 스레드 루프를 빠져나올때 사용할 플레그
        public bool IsConnected  = false;

        // 포트번호 / IP주소
        public int Port { get; private set; }
        public string IpAddress { get; private set; }


        public WpfUnityTCPServer(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }



        public void Connect()
        {
            if (IsConnected)
                return; // running already

            _Listener = new TcpListener(IPAddress.Parse(IpAddress), Port);
            _Listener.Start();

            new Thread(() =>
            {
                _Client = _Listener.AcceptTcpClient();
                IsConnected = true;
                _NetworkStream = _Client.GetStream();

                Thread lLoopRead = new Thread(new ThreadStart(LoopRead));
                lLoopRead.IsBackground = true;
                lLoopRead.Start();

                //Connectcompleted();
            }).Start();

        } //

        private void LoopRead()
        {
            while (IsConnected)
            {
                try
                {
                    WpfUnityPacketHeader header = ProtoBuf.Serializer.DeserializeWithLengthPrefix<WpfUnityPacketHeader>(_NetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                    if (header == null) break;


                    switch (header.ObjectType)
                    {
                        case Network.WpfUnityPacketType.WallInfo:
                            WallInfo lineInfo = ProtoBuf.Serializer.DeserializeWithLengthPrefix<WallInfo>(_NetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                            header.Data = lineInfo;
                            break;
                    }

                    if (OnReceviedCompleted != null)
                        OnReceviedCompleted(header);
                }
                catch (System.IO.IOException)
                {
                    if (!IsConnected) Console.WriteLine("user requested client shutdown");
                    else Console.WriteLine("disconnected");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            Console.WriteLine("client: reader is shutting down");
        }


        object sendlockObj = new object();
        public void Send(WpfUnityPacketHeader header)
        {
            if (header != null && IsConnected)
            {

                try
                {

                    ProtoBuf.Serializer.SerializeWithLengthPrefix<WpfUnityPacketHeader>(_NetworkStream, header, ProtoBuf.PrefixStyle.Fixed32);

                    switch (header.ObjectType)
                    {
                        case WpfUnityPacketType.WallInfo:
                            ProtoBuf.Serializer.SerializeWithLengthPrefix<WallInfo>(_NetworkStream, (WallInfo)header.Data, ProtoBuf.PrefixStyle.Fixed32);
                            break;
                    }
                }
                catch (System.IO.IOException)
                {

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }


        }


        object recevieockObj = new object();


        public void Disconnect()
        {
            IsConnected = false;
            if (_Client != null)
                _Client.Close();

        }
    }
}
