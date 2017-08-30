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
                        objd.convertFilePath = covertImgInfo.convertFilePath;
                        objd.explain = covertImgInfo.explain;
                        objd.ImgFilePath = covertImgInfo.ImgFilePath;
                        objd.price = covertImgInfo.price;
                        objd.rotationAngle = covertImgInfo.rotationAngle;
                        objd.VisualName = covertImgInfo.VisualName;
                        objd.obj_ConvertSize = (int)covertImgInfo.obj_ConvertSize;

                       objd.AssetBundleName = covertImgInfo.AssetBundleName;

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
                        ld.AssetBundleName = wallConvertInfo.AssetBundleName;

                        if (line.Stroke as ImageBrush != null)
                        {

                            for (int i = 0; i < data.imageData.Count; i++)
                            {
                                if (ld.AssetBundleName == data.imageData[i].ImageName)
                                {
                                    ld.ImageIndex = i;
                                    break;
                                }


                            }
                            if(ld.ImageIndex == -1)
                            {
                                ImageData imgData = new ImageData();
                                imgData.ImageName = ld.AssetBundleName;
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
                        fd.AssetBundleName = floorConvertInfo.AssetBundleName;
                        fd.ImageIndex = -1;

                        if (rect.Fill as ImageBrush != null)
                        {

                            for (int i = 0; i < data.imageData.Count; i++)
                            {
                                if (fd.AssetBundleName == data.imageData[i].ImageName)
                                {
                                    fd.ImageIndex = i;
                                    break;
                                }


                            }
                            if (fd.ImageIndex == -1)
                            {
                                ImageData imgData = new ImageData();
                                imgData.ImageName = fd.AssetBundleName;

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


        public Webgl3dData ToWebGLData()
        {
            int zeroPos = 400;
            Webgl3dData webgl3dData = new Webgl3dData();


            for (int i = 0; i < data.objectData.Count; i++)
            {
                Webgl3dObjectData webgl3dObjectData = new Webgl3dObjectData();

                float xc = (float)(data.objectData[i].Left - zeroPos + data.objectData[i].Width / 2);
                float yc = (float)(data.objectData[i].Top - zeroPos + (float)data.objectData[i].Height / 2) * -1;
                webgl3dObjectData.PosX = xc / 40;
                webgl3dObjectData.PosY = 0f;
                webgl3dObjectData.PosZ = yc / 40;
                webgl3dObjectData.Rotation = (float)data.objectData[i].rotationAngle;

                webgl3dData.objectData.Add(webgl3dObjectData);
            }


            for (int i = 0; i < data.FloorData.Count ; i++ )
            {
                Webgl3dFloorData webgl3dFloorData = new Webgl3dFloorData();
                // 계산
                // 2d 좌표 기준으로 200 / 200Canvas.GetTop(img) 센터로 지정한다.

                float xc = (float)(data.FloorData[i].Left - zeroPos + data.FloorData[i].Width / 2);
                float yc = (float)(data.FloorData[i].Top - zeroPos + (float)data.FloorData[i].Height / 2) * -1;
                webgl3dFloorData.PosX = xc / 40;
                webgl3dFloorData.PosY = 0f;
                webgl3dFloorData.PosZ = yc / 40;


                webgl3dFloorData.ScaleX = (float)data.FloorData[i].Width / 40;
                webgl3dFloorData.ScaleY = 0.01f;
                webgl3dFloorData.ScaleZ = (float)data.FloorData[i].Height / 40;

                webgl3dData.floorData.Add(webgl3dFloorData);
            }


            for (int i = 0; i < data.lineData.Count; i++)
            {
                Webgl3dWallData webgl3dWallData = new Webgl3dWallData();


                float wallThickness = 0.2f;
                // 계산
                // 2d 좌표 기준으로 200 / 200 센터로 지정한다.

                float x1 = (float)data.lineData[i].x1 - zeroPos;
                float x2 = (float)data.lineData[i].x2 - zeroPos;
                float y1 = (float)data.lineData[i].y1 - zeroPos;
                float y2 = (float)data.lineData[i].y2 - zeroPos;


                // 중심점을 구한다.
                float xc = (x1 + x2) / 2;
                float yc = (y1 + y2) / 2 * -1;

                // 크기를 구한다.
                float w = Math.Abs(x1 - x2);
                float h = Math.Abs(y1 - y2);

                webgl3dWallData.PosX = xc / 40;
                webgl3dWallData.PosY = 1f;

                webgl3dWallData.PosZ = yc / 40;

                webgl3dWallData.ScaleX = w / 40;
                webgl3dWallData.ScaleY = 2f;
                webgl3dWallData.ScaleZ = h / 40;

                if (webgl3dWallData.ScaleX == 0)
                {
                    webgl3dWallData.ScaleX = wallThickness;
                    webgl3dWallData.ScaleZ += wallThickness;
                }
                else
                {
                    webgl3dWallData.ScaleX += wallThickness;
                    webgl3dWallData.ScaleZ = wallThickness;
                }
                webgl3dData.WallData.Add(webgl3dWallData);
            }

                

                return webgl3dData;
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


    public class Webgl3dData
    {
        public void Clear()
        {
            objectData.Clear();
            WallData.Clear();
            floorData.Clear();
        }

       public List<Webgl3dObjectData> objectData = new List<Webgl3dObjectData>();
        public List<Webgl3dWallData> WallData = new List<Webgl3dWallData>();
        public List<Webgl3dFloorData> floorData = new List<Webgl3dFloorData>();


    }


    public class Webgl3dObjectData
    {
        public string AssetBundleName;
        public float PosX;
        public float PosY;
        public float PosZ;
        public float Rotation;
    }



    public class Webgl3dWallData
    {
        public float PosX;
        public float PosY;
        public float PosZ;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
    }


    public class Webgl3dFloorData
    {
        public float PosX;
        public float PosY;
        public float PosZ;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
    }




    [Serializable]
    public class ObjectDesignData
    {
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public string ObjectType;
        public string AssetBundleName; // 에셋번들이름
        public string VisualName; //사용자가 변경할 수 있는 이름.
        public string convertFilePath; //변환시켜줄 이미지
        public string ImgFilePath; //드롭할 때 tag에서 받아올 오른쪽 UI에 있는 원본 이미지 경로
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
        public string ImgFilePath;
        public string AssetBundleName;
        public int ImageIndex;
    }

    [Serializable]
    public class FloorDesignData
    {
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public string ImgFilePath;
        public string AssetBundleName;
        public int ImageIndex;
    }


    [Serializable]
    public class ImageData
    {
        public string ImageName;
        public List<byte> ImageBinary;


    }

}
