using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Interface
{
    public class InterfaceModel
    {
        public Point CameraPos;
        public bool ZoomedIn = false;
        public Point ScreenSize;
        public Point MousePoint;
        public SelectionState CurrentSelectionState;
        public Point MouseXYPoint;

        public Point StartMouseDownPoint;
        public Point EndMouseDownPoint;
        public bool MouseDown;
        public float FPS { get; set; }
        public List<Point> SelectionTiles { get; set; }

        public InterfaceModel(Point screenSize)
        {
            this.ScreenSize = screenSize;
            CameraPos = new Point(0, 0);
            CurrentSelectionState = SelectionState.None;
            MouseDown = false;
        }

        public void StartSelection()
        {
            StartMouseDownPoint = MouseXYPoint;
            EndMouseDownPoint = MouseXYPoint;
            MouseDown = true;
        }

        public void UpdateSelection()
        {
            EndMouseDownPoint = MouseXYPoint;
        }

        public void EndSelection()
        {
            EndMouseDownPoint = MouseXYPoint;
            MouseDown = false;
        }

        public int ZoomLevel
        {
            get
            {
                return ZoomedIn ? 2 : 1;
            }
        }

        public void UpdateSelectionInfo()
        {
            SelectionTiles = calculateSeletionTiles();
        }

        private List<Point> calculateSeletionTiles()
        {
            List<Point> returnMe = new List<Point>();
            if (CurrentSelectionState == SelectionState.Single)
            {
                returnMe.Add(this.MouseXYPoint);
            }
            else if (CurrentSelectionState == SelectionState.Area)
            {
                if (this.MouseDown)
                {
                    int startX = Math.Min(StartMouseDownPoint.X, EndMouseDownPoint.X);
                    int endX = Math.Max(StartMouseDownPoint.X, EndMouseDownPoint.X);
                    int startY = Math.Min(StartMouseDownPoint.Y, EndMouseDownPoint.Y);
                    int endY = Math.Max(StartMouseDownPoint.Y, EndMouseDownPoint.Y);
                    for (int i = startX; i <= endX; i++)
                        for (int j = startY; j <= endY; j++)
                            returnMe.Add(new Point(i, j));
                }
                else
                    returnMe.Add(this.MouseXYPoint);
            }
            else if (CurrentSelectionState == SelectionState.Line)
            {
                if (this.MouseDown)
                {
                    int startX = Math.Min(StartMouseDownPoint.X, EndMouseDownPoint.X);
                    int endX = Math.Max(StartMouseDownPoint.X, EndMouseDownPoint.X);
                    int startY = Math.Min(StartMouseDownPoint.Y, EndMouseDownPoint.Y);
                    int endY = Math.Max(StartMouseDownPoint.Y, EndMouseDownPoint.Y);
                    if ((endX - startX) > (endY - startY))
                    {
                        for (int i = startX; i <= endX; i++)
                            returnMe.Add(new Point(i, StartMouseDownPoint.Y));
                    }
                    else
                    {
                        for (int j = startY; j <= endY; j++)
                            returnMe.Add(new Point(StartMouseDownPoint.X, j));
                    }
                }
                else
                    returnMe.Add(this.MouseXYPoint);
            }
            return returnMe;
        }
    }
}
