﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nollan.Visual_Space.Network
{

    public enum WpfUnityPacketType : byte { eError = 0, WallInfo, ObjectInfoPacket, FloorInfoPacket, CommandPacket };

    [ProtoContract]

    public class WpfUnityPacketHeader
    {
        

        [ProtoMember(1)] public WpfUnityPacketType ObjectType;
        public object Data;


        public WpfUnityPacketHeader(WpfUnityPacketType type, object data)
        {
            ObjectType = type;
            Data = data;
        }

        public WpfUnityPacketHeader()
        {

        }
    }


    [ProtoContract]
    public class WallInfo
    {

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


        [ProtoMember(1)] public WallInfoAction Action;
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public float PosX;
        [ProtoMember(4)] public float PosY;
        [ProtoMember(5)] public float PosZ;
        [ProtoMember(6)] public float ScaleX;
        [ProtoMember(7)] public float ScaleY;
        [ProtoMember(8)] public float ScaleZ;
        [ProtoMember(9)] public byte[] ImageData;

    }

    public static class PacketFactory
    {
        public static WpfUnityPacketHeader MakeSelect(string name)
        {
            WpfUnityPacketHeader header = new WpfUnityPacketHeader();
            header.ObjectType = WpfUnityPacketType.WallInfo;
            WallInfo wallInfo = new WallInfo();
            wallInfo.Action = WallInfo.WallInfoAction.SELECT;
            wallInfo.Name = name;
            header.Data = wallInfo;
            return header;

        }


        public static WpfUnityPacketHeader MakeDeselect(string name)
        {
            WpfUnityPacketHeader header = new WpfUnityPacketHeader();
            header.ObjectType = WpfUnityPacketType.WallInfo;
            WallInfo wallInfo = new WallInfo();
            wallInfo.Action = WallInfo.WallInfoAction.DESELECT;
            wallInfo.Name = name;
            header.Data = wallInfo;
            return header;

        }


    }

    [ProtoContract]
    public class ObjectInfoPacket
    {

        public enum ObjectAction
        {
            CREATE = 0,
            MOVE,
            REMOVE,
            SELECT,
            DESELECT,
            ROTATION,
            MOVE3D,
            REMOVE3D,
            SELECT3D,
            DESELECT3D,
            ROTATION3D,
        }

        [ProtoMember(1)] public ObjectAction Action;
        [ProtoMember(2)] public string Name;
        // todo 추가 AssetBundleName
        // todo 변경 AssetBundleName > ObjectName..
        [ProtoMember(3)] public string AssetBundleName;
        [ProtoMember(4)] public float PosX;
        [ProtoMember(5)] public float PosY;
        [ProtoMember(6)] public float PosZ;
        [ProtoMember(7)] public float Rotation;
    }




    [ProtoContract]
    public class FloorInfoPacket
    {

        public enum FloorInfoAction
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


        [ProtoMember(1)] public FloorInfoAction Action;
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public float PosX;
        [ProtoMember(4)] public float PosY;
        [ProtoMember(5)] public float PosZ;
        [ProtoMember(6)] public float ScaleX;
        [ProtoMember(7)] public float ScaleY;
        [ProtoMember(8)] public float ScaleZ;
        [ProtoMember(9)] public byte[] ImageData;

    }

    [ProtoContract]
    public class CommandPacket
    {
        public enum CommandAction
        {
            ALLCLEAR,
        }

        [ProtoMember(1)] public CommandAction Action;
    }
}
