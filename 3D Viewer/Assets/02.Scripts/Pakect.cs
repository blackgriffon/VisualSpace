using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Packet
{
    public enum PacketType : byte { eError = 0, WallInfo, ObjectInfo };


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


    public enum WallInfoAction
    {
        CREATE = 0,
        MOVE,
        REMOVE,
        SELECT,
        DESELECT,
        RESIZE,
        MOVE3D,
        REMOVE3D,
        SELECT3D,
        DESELECT3D,
    }


    public enum ObjectAction
    {
        CREATE = 0,
        MOVE,
        REMOVE,
        SELECT,
        DESELECT,
        MOVE3D,
        REMOVE3D,
        SELECT3D,
        DESELECT3D,
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

    public static class PacketFactory
    {
        public static Header MakeDeselect(string name, PacketType packetType)
        {
            Header header = new Header();

            switch (packetType)
            {
                case PacketType.WallInfo:
                    header.ObjectType = PacketType.WallInfo;
                    WallInfo wallInfo = new WallInfo();
                    wallInfo.Action = WallInfoAction.DESELECT3D;
                    wallInfo.Name = name;
                    header.Data = wallInfo;
                    break;


                case PacketType.ObjectInfo:
                    header.ObjectType = PacketType.ObjectInfo;
                    ObjectInfo objectInfo = new ObjectInfo();
                    objectInfo.Action = ObjectAction.DESELECT3D;
                    objectInfo.Name = name;
                    header.Data = objectInfo;
                    break;
            }


            return header;
        }

        public static Header MakeSelect(string name, PacketType packetType)
        {
            Header header = new Header();

            switch (packetType)
            {
                case PacketType.WallInfo:
                    header.ObjectType = PacketType.WallInfo;
                    WallInfo wallInfo = new WallInfo();
                    wallInfo.Action = WallInfoAction.SELECT3D;
                    wallInfo.Name = name;
                    header.Data = wallInfo;
                    break;


                case PacketType.ObjectInfo:
                    header.ObjectType = PacketType.ObjectInfo;
                    ObjectInfo objectInfo = new ObjectInfo();
                    objectInfo.Action = ObjectAction.SELECT3D;
                    objectInfo.Name = name;
                    header.Data = objectInfo;
                    break;
            }


            return header;
        }
    }


    [ProtoContract]
    public class ObjectInfo
    {
        [ProtoMember(1)] public ObjectAction Action;
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public string ObjectType;
        [ProtoMember(4)] public float PosX;
        [ProtoMember(5)] public float PosY;
        [ProtoMember(6)] public float PosZ;
    }



}