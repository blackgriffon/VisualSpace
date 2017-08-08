
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Packet;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace TransportTCP
{
    public class TCPClientProtoBuf
    {
        // 송신 수신을 위한 데이터
        Queue<Packet.Header> recevieDataQueue = new Queue<Header>();
        Queue<Packet.Header> sendDataQueue = new Queue<Header>();

        // 서버와 통신할 소켓
        TcpClient _Client = null;

        // 직렬화 후 서버에 데이터 보낼때 사용되는 NetworkStream
        NetworkStream _NetworkStream = null;

        // 스레드 루프를 빠져나올때 사용할 플레그
        private bool _ExitLoop = true;

        // 포트번호 / IP주소
        public int Port { get; private set; }
        public string IpAddress { get; private set; }


        public static TCPClientProtoBuf instance = null;
        public static TCPClientProtoBuf Instance
        {
            get
            {
                return instance;
            }
        }

        static TCPClientProtoBuf()
        {
            instance = new TCPClientProtoBuf();
        }


        private TCPClientProtoBuf()
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


            Thread lLoopWrite = new Thread(new ThreadStart(LoopWrite));
            lLoopWrite.IsBackground = true;
            lLoopWrite.Start();

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
                   Packet.Header header = ProtoBuf.Serializer.DeserializeWithLengthPrefix<Packet.Header>(_NetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                    if (header == null) break;


                    switch (header.ObjectType)
                    {
                        case Packet.PacketType.WallInfo:
                            header.Data = ProtoBuf.Serializer.DeserializeWithLengthPrefix<Packet.WallInfo>(_NetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                            break;

                        case Packet.PacketType.ObjectInfo:
                            header.Data = ProtoBuf.Serializer.DeserializeWithLengthPrefix<Packet.ObjectInfo>(_NetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                            break;

                    }

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

        private void LoopWrite()
        {
            while (!_ExitLoop)
            {
                try
                {
                    Packet.Header header;

                    if (sendDataQueue.Count <= 0)
                        continue;
                    lock (sendDataQueue)
                        header = sendDataQueue.Dequeue();

                    ProtoBuf.Serializer.SerializeWithLengthPrefix<Packet.Header>(_NetworkStream, header, ProtoBuf.PrefixStyle.Fixed32);

                    switch (header.ObjectType)
                    {
                        case Packet.PacketType.WallInfo:
                            ProtoBuf.Serializer.SerializeWithLengthPrefix<Packet.WallInfo>(_NetworkStream, (Packet.WallInfo)header.Data, ProtoBuf.PrefixStyle.Fixed32);
                            break;
                    }
                }
                catch (System.IO.IOException)
                {

                }
                catch (Exception ex) { Debug.Log(ex.Message); }
            }
            _ExitLoop = true;
            Debug.Log("client: writer is shutting down");
        } //


        public void Send(Packet.Header xHeader)
        {
            if (xHeader == null)
                return;

            lock (sendDataQueue)
            {

                sendDataQueue.Enqueue(xHeader);
            }
        } 


        public bool Recevie(ref Packet.Header header)
        {

            if (header == null)
                return false;

            lock(recevieDataQueue)
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