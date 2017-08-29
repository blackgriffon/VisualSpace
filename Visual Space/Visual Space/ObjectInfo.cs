using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nollan.Visual_Space.classes.ObjSizeInfo;

namespace Nollan.Visual_Space
{
    public class ObjectInfo
    {
        public string AssetBundleName;  // 에셋번들이름
        public string ObjectType; //chairs . 컨버트할 경로의 경우 오브젝트타입 밑의 convert에 들어가서 오브젝트타입형태.png 찾으면 됨. >>  ../../pictures/chairs/convert/chairs.png
        public string FilePath; //  ../../pictures/chairs/chair{i++}.png    << 파일경로. 이거만 있으면 굳이 이름이 필요없나?
        public string VisualName; //이케아 의자 
        public int price; //가격
        public string brand; //이케아, 삼성, lg 등 가구 브랜드
        public string explain; //가구 간단한 설명

        public ObjSize obj_ConvertSize;            
    }
}
