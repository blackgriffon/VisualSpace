
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WpfUnityPacket;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace TransportTCP
{
    public class WpfUnityTCPClient
    {
        // 송신 수신을 위한 데이터
        Queue<WpfUnityPacketHeader> recevieDataQueue = new Queue<WpfUnityPacketHeader>();
        // 서버와 통신할 소켓
        TcpClient _Client = null;

        // 직렬화 후 서버에 데이터 보낼때 사용되는 NetworkStream
        NetworkStream _NetworkStream = null;

        // 스레드 루프를 빠져나올때 사용할 플레그
        private bool _ExitLoop = true;

        // 포트번호 / IP주소
        public int Port { get; private set; }
        public string IpAddress { get; private set; }


        public static WpfUnityTCPClient instance = null;
        public static WpfUnityTCPClient Instance
        {
            get
            {
                return instance;
            }
        }

        static WpfUnityTCPClient()
        {
            instance = new WpfUnityTCPClient();
        }


        private WpfUnityTCPClient()
        {

        }




        public void Connect(string ipAddress, int port)
        {

            if (!_ExitLoop)
                return; // running already

            IpAddress = ipAddress;
            Port = port;


            _ExitLoop = false;

            _Client = new TcpClient();
            _Client.Connect(IpAddress, Port);
            _NetworkStream = _Client.GetStream();

            Thread lLoopRead = new Thread(new ThreadStart(LoopRead));
            lLoopRead.IsBackground = true;
            lLoopRead.Start();

        } //

        private void LoopRead()
        {
            while (!_ExitLoop)
            {
                try
                {
                    WpfUnityPacketHeader header = ProtoBuf.Serializer.DeserializeWithLengthPrefix<WpfUnityPacketHeader>(_NetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                    if (header == null) break;

                    // 받은 데이터를 큐에 넣는다.
                    lock (recevieDataQueue)
                        recevieDataQueue.Enqueue(header);
                }
                catch (System.IO.IOException)
                {
                    if (_ExitLoop) Debug.Log("user requested client shutdown");
                    else Debug.Log("disconnected");
                    Disconnect();

                }
                catch (Exception ex) { Debug.Log(ex.Message); }
            }
            Debug.Log("client: reader is shutting down");
        }



        public void Send(WpfUnityPacketHeader header)
        {
            if (header == null)
                return;

            try
            {
                ProtoBuf.Serializer.SerializeWithLengthPrefix<WpfUnityPacket.WpfUnityPacketHeader>(_NetworkStream, header, ProtoBuf.PrefixStyle.Fixed32);


            }
            catch (System.IO.IOException)
            {

            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }



        public bool Recevie(ref WpfUnityPacketHeader header)
        {

            lock (recevieDataQueue)
            {
                if (recevieDataQueue.Count <= 0)
                    return false;

                header = recevieDataQueue.Dequeue();
                return true;
            }

        }



        public void Disconnect()
        {
            _ExitLoop = true;
            if (_Client != null && _Client.Connected)
                _Client.Close();

        }

    }


}