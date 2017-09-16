using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace WpfUnityPacket
{

    public enum WpfUnityPacketType : byte { eError = 0, WallInfoPacket, ObjectInfoPacket, FloorInfoPacket, CommandPacket };

    public enum WpfUnityCommand
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
        ALLCLEAR,
    }


    [ProtoContract]
    [ProtoInclude(1, typeof(WallInfoPacket))]
    [ProtoInclude(2, typeof(ObjectInfoPacket))]
    [ProtoInclude(3, typeof(FloorInfoPacket))]
    [ProtoInclude(4, typeof(CommandPacket))]
    public class WpfUnityPacketHeader
    {
        public WpfUnityPacketType PacketType;
    }


    [ProtoContract]
    public class WallInfoPacket : WpfUnityPacketHeader
    {

        public WallInfoPacket()
        {
            PacketType = WpfUnityPacketType.WallInfoPacket;
        }



        [ProtoMember(1)] public WpfUnityCommand Command;
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
    public class ObjectInfoPacket : WpfUnityPacketHeader
    {



        public ObjectInfoPacket()
        {
            PacketType = WpfUnityPacketType.ObjectInfoPacket;
        }


        [ProtoMember(1)] public WpfUnityCommand Command;
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public string AssetBundleName;
        [ProtoMember(4)] public string ObjectName;
        [ProtoMember(5)] public float PosX;
        [ProtoMember(6)] public float PosY;
        [ProtoMember(7)] public float PosZ;
        [ProtoMember(8)] public float Rotation;
    }




    [ProtoContract]
    public class FloorInfoPacket : WpfUnityPacketHeader
    {


        public FloorInfoPacket()
        {
            PacketType = WpfUnityPacketType.FloorInfoPacket;
        }



        [ProtoMember(1)] public WpfUnityCommand Command;
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
    public class CommandPacket : WpfUnityPacketHeader
    {




        public CommandPacket()
        {
            PacketType = WpfUnityPacketType.CommandPacket;
        }



        [ProtoMember(1)] public WpfUnityCommand Command;
    }





    public static class PacketFactory
    {
        public static WpfUnityPacketHeader MakeDeselect(string name, WpfUnityPacketType packetType)
        {


            switch (packetType)
            {
                case WpfUnityPacketType.WallInfoPacket:
                    WallInfoPacket wallInfo = new WallInfoPacket();
                    wallInfo.Command = WpfUnityCommand.DESELECT3D;
                    wallInfo.Name = name;
                    return wallInfo;


                case WpfUnityPacketType.ObjectInfoPacket:
                    ObjectInfoPacket objectInfo = new ObjectInfoPacket();
                    objectInfo.Command = WpfUnityCommand.DESELECT3D;
                    objectInfo.Name = name;
                    return objectInfo;

                case WpfUnityPacketType.FloorInfoPacket:
                    FloorInfoPacket floorInfo = new FloorInfoPacket();
                    floorInfo.Command = WpfUnityCommand.DESELECT3D;
                    floorInfo.Name = name;
                    return floorInfo;

            }

            return null;

        }

        public static WpfUnityPacketHeader MakeSelect(string name, WpfUnityPacketType packetType)
        {

            switch (packetType)
            {
                case WpfUnityPacketType.WallInfoPacket:
                    WallInfoPacket wallInfo = new WallInfoPacket();
                    wallInfo.Command = WpfUnityCommand.SELECT3D;
                    wallInfo.Name = name;
                    return wallInfo;


                case WpfUnityPacketType.ObjectInfoPacket:
                    ObjectInfoPacket objectInfo = new ObjectInfoPacket();
                    objectInfo.Command = WpfUnityCommand.SELECT3D;
                    objectInfo.Name = name;
                    return objectInfo;
                    

                case WpfUnityPacketType.FloorInfoPacket:
                    FloorInfoPacket floorInfo = new FloorInfoPacket();
                    floorInfo.Command = WpfUnityCommand.SELECT3D;
                    floorInfo.Name = name;
                    return floorInfo;
            }


            return null;
        }
    }


}