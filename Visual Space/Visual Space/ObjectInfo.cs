﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nollan.Visual_Space.classes.ObjSizeInfo;

namespace Nollan.Visual_Space
{
    [ProtoContract]
    public class ObjectInfo
    {

        [ProtoMember(1)] public int number;
        [ProtoMember(2)] public string ObjectType;  //chairs . 컨버트할 경로의 경우 오브젝트타입 밑의 convert에 들어가서 오브젝트타입형태.png 찾으면 됨. >>  ../../pictures/chairs/convert/chairs.png
        [ProtoMember(3)] public string VisualName; //이케아 의자
        [ProtoMember(4)] public string AssetBundleName; // 에셋번들이름
        [ProtoMember(5)] public string FilePath;  //  ../../pictures/chairs/chair{i++}.png    << 파일경로. 이거만 있으면 굳이 이름이 필요없나?
        [ProtoMember(6)] public int price; // 가격
        [ProtoMember(7)] public string brand; //이케아, 삼성, lg 등 가구 브랜드
        [ProtoMember(8)] public string explain; //가구 간단한 설명
        [ProtoMember(9)] public ObjSize obj_ConvertSize;

    }


    [ProtoContract]
    public class ObjectInfoList
    {
        [ProtoMember(1, OverwriteList = true)]
        public List<ObjectInfo> list = new List<ObjectInfo>();
    }

}
