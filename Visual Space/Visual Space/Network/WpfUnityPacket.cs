using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nollan.Visual_Space.Network
{

    public enum WpfUnityPacketType : byte { eError = 0, WallInfo };

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
            RESIZE,
            MOVE3D,
            REMOVE3D,
        }


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
