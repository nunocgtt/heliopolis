using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.Interface
{
    public class InterfaceModel
    {
        public Point CameraPos;
        public bool ZoomedIn;
        public Point ScreenSize;
        public Point MousePoint;
        public SelectionState CurrentSelectionState;
        public Point MouseXyPoint;

        private Point _startMouseDownPoint;
        private Point _endMouseDownPoint;
        public bool MouseDown;
        public float Fps { get; set; }
        public List<Point> SelectionTiles { get; private set; }

        public InterfaceModel(Point screenSize)
        {
            ZoomedIn = false;
            ScreenSize = screenSize;
            CameraPos = new Point(0, 0);
            CurrentSelectionState = SelectionState.None;
            MouseDown = false;
        }

        public void StartSelection()
        {
            _startMouseDownPoint = MouseXyPoint;
            _endMouseDownPoint = MouseXyPoint;
            MouseDown = true;
        }

        public void UpdateSelection()
        {
            _endMouseDownPoint = MouseXyPoint;
        }

        public void EndSelection()
        {
            _endMouseDownPoint = MouseXyPoint;
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
            SelectionTiles = CalculateSeletionTiles();
        }

        private List<Point> CalculateSeletionTiles()
        {
            List<Point> returnMe = new List<Point>();
            switch (CurrentSelectionState)
            {
                case SelectionState.Single:
                    returnMe.Add(MouseXyPoint);
                    break;
                case SelectionState.Area:
                    if (MouseDown)
                    {
                        int startX = Math.Min(_startMouseDownPoint.X, _endMouseDownPoint.X);
                        int endX = Math.Max(_startMouseDownPoint.X, _endMouseDownPoint.X);
                        int startY = Math.Min(_startMouseDownPoint.Y, _endMouseDownPoint.Y);
                        int endY = Math.Max(_startMouseDownPoint.Y, _endMouseDownPoint.Y);
                        for (int i = startX; i <= endX; i++)
                            for (int j = startY; j <= endY; j++)
                                returnMe.Add(new Point(i, j));
                    }
                    else
                        returnMe.Add(this.MouseXyPoint);
                    break;
                case SelectionState.Line:
                    if (MouseDown)
                    {
                        int startX = Math.Min(_startMouseDownPoint.X, _endMouseDownPoint.X);
                        int endX = Math.Max(_startMouseDownPoint.X, _endMouseDownPoint.X);
                        int startY = Math.Min(_startMouseDownPoint.Y, _endMouseDownPoint.Y);
                        int endY = Math.Max(_startMouseDownPoint.Y, _endMouseDownPoint.Y);
                        if ((endX - startX) > (endY - startY))
                        {
                            for (int i = startX; i <= endX; i++)
                                returnMe.Add(new Point(i, _startMouseDownPoint.Y));
                        }
                        else
                        {
                            for (int j = startY; j <= endY; j++)
                                returnMe.Add(new Point(_startMouseDownPoint.X, j));
                        }
                    }
                    else
                        returnMe.Add(MouseXyPoint);
                    break;
            }
            return returnMe;
        }
    }
}
