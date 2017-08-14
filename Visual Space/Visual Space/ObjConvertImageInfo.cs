using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nollan.Visual_Space
{
    //드롭된 정보를 캔버스에서 도면 이미지로 변환하고 그것에 대한 정보를 기억.
    public class ObjConvertImageInfo
    {
        public string VisualName; //사용자가 변경할 수 있는 이름.
        public string AssetBundleName; // 에셋번들이름
        public string convertFilePath; //변환시켜줄 이미지
        public string ImgFilePath; //드롭할 때 tag에서 받아올 오른쪽 UI에 있는 원본 이미지 경로
        public string ObjectName; //같은 오브젝트를 여러개 놓는다면 그걸 구분하기 위한 이름이 필요.
        public string ObjectType;
        public Point canvasPoint; //캔버스 좌표
        public double rotationAngle; //시계방향으로 회전하기 위한 2차원 x,y 정보 좌표계.   

    }
}
