using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nollan.Visual_Space
{
    public class ObjectInfo
    {     

            public string ObjectType; //chairs . 컨버트할 경로의 경우 오브젝트타입 밑의 convert에 들어가서 오브젝트타입형태.png 찾으면 됨. >>  ../../pictures/chairs/convert/chairs.png
            public string FilePath; //  ../../pictures/chairs/chair{i++}.png    << 파일경로. 이거만 있으면 굳이 이름이 필요없나?
            public string VisualName; //이케아 의자 


    }
}
