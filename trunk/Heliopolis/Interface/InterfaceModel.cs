﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.World;
using Heliopolis.World.InteractableObjects;
using Heliopolis.World.JobSystem;

namespace Heliopolis.Interface
{
    public enum InterfaceActionType
    {
        ApplyDesignation
    }
    public class InterfaceAction
    {
        public InterfaceActionType ActionType { get; set; }
        public string TargetHarvestType { get; set; }
    }

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
        private readonly GameWorld _world;
        public float Fps { get; set; }
        public InterfaceAction CurrentAction { get; set; }

        public bool GameIsPaused
        {
            get
            {
                return _world.TimedEventManager.Paused;
            } 
        }

        public List<Point> SelectionTiles { get; private set; }

        private void ApplyAction()
        {
            if (CurrentAction != null)
            {
                if (CurrentAction.ActionType == InterfaceActionType.ApplyDesignation)
                {
                    // Apply the designation
                    for (int i = _startMouseDownPoint.X; i <= _endMouseDownPoint.X; i++)
                    {
                        for (int j = _startMouseDownPoint.Y; j <= _endMouseDownPoint.Y; j++)
                        {
                            if (_world.Environment[i, j].InteractableObject != null)
                            {
                                if (_world.Environment[i, j].InteractableObject is HarvestableInteractableObject)
                                {
                                    var harvestMe = (HarvestableInteractableObject)_world.Environment[i, j].InteractableObject;
                                    if (harvestMe.ResourceType == CurrentAction.TargetHarvestType)
                                    {
                                        _world.DesignationManager.AddDesignation(new HarvestDesignation(_world, harvestMe));
                                    }
                                }
                            }
                        }
                    }
                }
                CurrentAction = null;
            }
        }

        public InterfaceModel(Point screenSize, GameWorld world)
        {
            _world = world;
            ZoomedIn = false;
            ScreenSize = screenSize;
            CameraPos = new Point(0, 0);
            CurrentSelectionState = SelectionState.None;
            MouseDown = false;
            CurrentAction = null;
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
            ApplyAction();
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

        internal void SelectTreesForHarvest()
        {
            CurrentAction = new InterfaceAction {ActionType = InterfaceActionType.ApplyDesignation, TargetHarvestType = "wood"};
            CurrentSelectionState = SelectionState.Area;
        }

        internal void TogglePause()
        {
            _world.TimedEventManager.Paused = !_world.TimedEventManager.Paused;
        }
    }
}