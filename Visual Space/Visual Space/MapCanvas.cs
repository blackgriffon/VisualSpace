using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

//using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace Nollan.Visual_Space
{

    public class MapCanvas : Canvas
    {
        //격자 크기 값﻿
        private int _gridSize = 20;

        public static List<Point> ListRenderPoint = new List<Point>();

        //현재 Canvas가 화면에 그려질때 ( Canvas 크기가 변경될때마다 갱신된다 )
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            //부모 OnRender호출 ( 기본 Canvas에서 특정 요소만 추가하여 화면에 표시하기때문에 호출되어야한다. )
            base.OnRender(dc);


            //x축 그려질 개수 구하기
            var xlineCount = Math.Round(this.ActualWidth / _gridSize);


            //y축 그려질 개수 구하기
            var ylineCount = Math.Round(this.ActualHeight / _gridSize);


            //윤곽선을 그릴 개체 ( http://msdn.microsoft.com/ko-kr/library/system.windows.media.pen(v=vs.90).aspx )
            Pen pen = new Pen(Brushes.IndianRed, 0.5d);



            for (int i = 1; i < xlineCount + 1; i++)
            {

                //정해진 격자 크기만큼 계산하면서  X축으로 ㅣㅣㅣㅣㅣ  버티컬 선 그리기
                var xPosition = i * _gridSize;
                dc.DrawLine(pen, new Point(xPosition, 0), new Point(xPosition, this.ActualHeight));

                /*
                40부터 시작. 40, 80, 120...etc
                xlineCount까지 점이 나온단 소리?

             */




                // ListRenderPoint


            }


            for (int i = 0; i < ylineCount + 1; i++)
            {
                //정해진 격자 크기만큼 계산하면서  Y축으로 ㅡㅡㅡㅡ 호리즌탈 선 그리기
                var yPosition = i * _gridSize;
                dc.DrawLine(pen, new Point(0, yPosition), new Point(this.ActualWidth, yPosition));

                /*
                0부터 시작. 0, 40, 80...etc    
             */




            }


        }
    }



}
