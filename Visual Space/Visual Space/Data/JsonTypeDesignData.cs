using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Windows.Shapes;
using System.Windows.Media;
using Nollan.Visual_Space.Util;

namespace Nollan.Visual_Space.Data
{
    [Serializable]
    public class JsonTypeDesignDataControler
    {
        public JsonTypeDesignData data = new JsonTypeDesignData();


        public void GetCanvasData(Canvas canvas)
        {
            data.Clear();
            foreach (var item in canvas.Children)
            {
                switch (item)
                {

                    case Image img:
                        ObjConvertImageInfo covertImgInfo = img.Tag as ObjConvertImageInfo;
                        ObjectDesignData objd = new ObjectDesignData();
                        objd.Top = Canvas.GetTop(img);
                        objd.Left = Canvas.GetLeft(img);
                        objd.Width = img.Source.Width;
                        objd.Height = img.Source.Height;

                        objd.ObjectType = covertImgInfo.ObjectType;
                        objd.brand = covertImgInfo.brand;
                        objd.explain = covertImgInfo.explain;
                        objd.price = covertImgInfo.price;
                        objd.rotationAngle = covertImgInfo.rotationAngle;
                        objd.VisualName = covertImgInfo.VisualName;
                        objd.obj_ConvertSize = (int)covertImgInfo.obj_ConvertSize;

                       objd.ObjectName = covertImgInfo.AssetBundleName;

                        data.objectData.Add(objd);
                        break;


                    case Line line:
                        LineDesignData ld = new LineDesignData();
                        WallConvertInfo wallConvertInfo = line.Tag as WallConvertInfo;
                        ld.x1 = line.X1;
                        ld.ImageIndex = -1;
                        ld.y1 = line.Y1;
                        ld.x2 = line.X2;
                        ld.y2 = line.Y2;
                        ld.WallImageName = wallConvertInfo.AssetBundleName;

                        if (line.Stroke as ImageBrush != null)
                        {

                            for (int i = 0; i < data.imageData.Count; i++)
                            {
                                if (ld.WallImageName == data.imageData[i].ImageName)
                                {
                                    ld.ImageIndex = i;
                                    break;
                                }


                            }
                            if(ld.ImageIndex == -1)
                            {
                                ImageData imgData = new ImageData();
                                imgData.ImageName = ld.WallImageName;
                                int len = ImageConvertor.ConvertImageBrushToJpegByteArray(line.Stroke as ImageBrush).Length;
                                byte[] bt = ImageConvertor.ConvertImageBrushToJpegByteArray(line.Stroke as ImageBrush);
                                imgData.ImageBinary = ImageConvertor.ConvertImageBrushToJpegByteArray(line.Stroke as ImageBrush).ToList<byte>();
                                data.imageData.Add(imgData);
                                ld.ImageIndex = data.imageData.Count-1;
                            }

                        }



                        data.lineData.Add(ld);
                        break;

                    case Rectangle rect:
                        FloorDesignData fd = new FloorDesignData();
                        FloorConvertInfo floorConvertInfo = rect.Tag as FloorConvertInfo;
                        fd.Top = Canvas.GetTop(rect);
                        fd.Left = Canvas.GetLeft(rect);
                        fd.Width = rect.ActualWidth;
                        fd.Height = rect.ActualHeight;
                        fd.FloorImageName = floorConvertInfo.AssetBundleName;
                        fd.ImageIndex = -1;

                        if (rect.Fill as ImageBrush != null)
                        {

                            for (int i = 0; i < data.imageData.Count; i++)
                            {
                                if (fd.FloorImageName == data.imageData[i].ImageName)
                                {
                                    fd.ImageIndex = i;
                                    break;
                                }


                            }
                            if (fd.ImageIndex == -1)
                            {
                                ImageData imgData = new ImageData();
                                imgData.ImageName = fd.FloorImageName;

                                imgData.ImageBinary = ImageConvertor.ConvertImageBrushToJpegByteArray(rect.Fill as ImageBrush).ToList<byte>();
                                data.imageData.Add(imgData);
                                fd.ImageIndex = data.imageData.Count - 1;
                            }

                        }

                            data.FloorData.Add(fd);
                        break;
                }
            }


        }



        public void Save(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }
        }

        public void Load(string filePath)
        {
            using (StreamReader sw = new StreamReader(filePath))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = serializer.Deserialize<JsonTypeDesignData>(reader);
            }
        }

        
    }

    [Serializable]
    public class JsonTypeDesignData
    {
        public List<ObjectDesignData> objectData = new List<ObjectDesignData>();
        public List<LineDesignData> lineData = new List<LineDesignData>();
        public List<FloorDesignData> FloorData = new List<FloorDesignData>();
        public List<ImageData> imageData = new List<ImageData>();

        public void Clear()
        {
            objectData.Clear();
            lineData.Clear();
            FloorData.Clear();
            imageData.Clear();

        }
    }


    

    [Serializable]
    public class ObjectDesignData
    {
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public string ObjectType;

        // todo 기존 AssetBundleName을 대체
        // public string objectName; 

        // todo 용도 변경 에셋번들 패키지 이름..
        public string ObjectName; // ObjectName
        public string VisualName; //사용자가 변경할 수 있는 이름.
        public double rotationAngle; //시계방향으로 회전하기 위한 2차원 x,y 정보 좌표계.   
        public int price; //가격
        public string brand; //이케아, 삼성, lg 등 가구 브랜드
        public string explain; //가구 간단한 설명
        public int obj_ConvertSize;
    }

    [Serializable]
    public class LineDesignData
    {
        public double x1;
        public double y1;
        public double x2;
        public double y2;
        public string WallImageName;
        public int ImageIndex;
    }

    [Serializable]
    public class FloorDesignData
    {
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public string FloorImageName;
        public int ImageIndex;
    }


    [Serializable]
    public class ImageData
    {
        public string ImageName;
        public List<byte> ImageBinary;


    }

}
