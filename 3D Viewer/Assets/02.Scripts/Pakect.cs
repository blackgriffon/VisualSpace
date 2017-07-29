using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Packet
{
    public enum PacketType : byte { eError = 0, WallInfo };


    [ProtoContract]
    public class Header
    {
        [ProtoMember(1)] public PacketType ObjectType;
        public object Data;


        public Header(PacketType type, object data)
        {
            ObjectType = type;
            Data = data;
        }

        public Header()
        {

        }
    }


    enum WallInfoAction
    {
        CREATE = 0,
        MOVE,
        REMOVE,
        RESIZE,
        MOVE3D,
        REMOVE3D,
    }

    [ProtoContract]
    class WallInfo
    {
        [ProtoMember(1)] public WallInfoAction Action;
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public float PosX;
        [ProtoMember(4)] public float PosY;
        [ProtoMember(5)] public float PosZ;
        [ProtoMember(6)] public float ScaleX;
        [ProtoMember(7)] public float ScaleY;
        [ProtoMember(8)] public float ScaleZ;
    }
}