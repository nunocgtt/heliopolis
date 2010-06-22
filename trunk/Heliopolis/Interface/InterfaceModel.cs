using System;
using System.Collections.Generic;
using Heliopolis.GraphicsEngine;
using Heliopolis.UILibrary;
using Heliopolis.World;
using Heliopolis.World.InteractableObjects;
using Heliopolis.World.JobSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Heliopolis.Interface
{
    public interface IGameValueProvider
    {
        string GetGameValue(string valueToGet);
    }

    public enum InterfaceActionType
    {
        ApplyDesignation
    }
    public class InterfaceAction
    {
        public InterfaceActionType ActionType { get; set; }
        public string TargetHarvestType { get; set; }
    }

    public enum InterfaceState
    {
        None,
        MoveCamera,
        MakeSelection
    }

    public class InterfaceModel : IGameValueProvider
    {
        public const string UIFileToLoad = @"Content\UI\Interface.xml";
        public Point CameraPos;
        public bool ZoomedIn;
        public Point ScreenSize;
        public Point MousePoint;
        public Point MousePointOld;
        public Point MousePointDelta;
        public SelectionState CurrentSelectionState;
        public Point MousePointIsometricGrid;

        private Stack<InterfaceState> _interfaceState;
        private Point _startMouseDownPoint;
        private Point _endMouseDownPoint;
        public bool MouseDown;
        private readonly GameWorld _world;
        public float Fps { get; set; }
        public InterfaceAction CurrentAction { get; set; }
        private UserInterface _userInterface;
        public bool MovingCamera { get; set; }

        public bool GameIsPaused
        {
            get
            {
                return _world.TimedEventManager.Paused;
            } 
        }

        public string MouseCursor
        {
            get
            {
                if (_interfaceState.Peek() == InterfaceState.MoveCamera)
                    return "Arrows";
                return "Normal";
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

        public InterfaceModel(Point screenSize, GameWorld world, Game xnaGame)
        {
            _world = world;
            ZoomedIn = false;
            ScreenSize = screenSize;
            CameraPos = new Point(0, 0);
            CurrentSelectionState = SelectionState.None;
            MouseDown = false;
            CurrentAction = null;
            UserInterface = new InterfaceFactory().CreateUserInterface(xnaGame, UIFileToLoad, screenSize.X, screenSize.Y);
            UserInterface.GameValueProvider = this;
            _interfaceState = new Stack<InterfaceState>();
            _interfaceState.Push(InterfaceState.None);
        }

        public void StartMoveCamera()
        {
            _interfaceState.Push(InterfaceState.MoveCamera);
            MovingCamera = true;
        }

        public void FinishMoveCamera()
        {
            _interfaceState.Pop();
            MovingCamera = false;
        }

        public void StartSelection()
        {
            if (_interfaceState.Peek() != InterfaceState.MoveCamera)
            {
                _startMouseDownPoint = MousePointIsometricGrid;
                _endMouseDownPoint = MousePointIsometricGrid;
                MouseDown = true;
                _interfaceState.Push(InterfaceState.MakeSelection);
            }
        }

        public void SetNewMousePosition(Point newPosition, IsometricEngine gameEngine)
        {
            MousePointOld = MousePoint;
            MousePoint = newPosition;
            MousePointDelta = new Point(MousePointOld.X - MousePoint.X, MousePointOld.Y - MousePoint.Y);

            Point cameraOffset = new Point((CameraPos.X * -1 * ZoomLevel), ((CameraPos.Y * -1 * ZoomLevel)));
            MousePointIsometricGrid = Iso2D.ConvertScreenToTile(MousePoint, (int)(gameEngine.TileSize.X * ZoomLevel), (int)(gameEngine.TileSize.Y * ZoomLevel), gameEngine.FirstTileXyPosition(ZoomLevel), cameraOffset);
        }

        public void EndSelection()
        {
            if (_interfaceState.Peek() == InterfaceState.MakeSelection)
            {
                _endMouseDownPoint = MousePointIsometricGrid;
                MouseDown = false;
                ApplyAction();
                _interfaceState.Pop();
            }
        }

        public int ZoomLevel
        {
            get
            {
                return ZoomedIn ? 2 : 1;
            }
        }

        public UserInterface UserInterface
        {
            get { return _userInterface; }
            set { _userInterface = value; }
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
                    returnMe.Add(MousePointIsometricGrid);
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
                        returnMe.Add(this.MousePointIsometricGrid);
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
                        returnMe.Add(MousePointIsometricGrid);
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

        // Initially designed to bind in-game values to controls.
        public string GetGameValue(string valueToGet)
        {
            switch (valueToGet)
            {
                case "totalunits":
                    return _world.ActorManager.LiveActors.Count.ToString();
                case "debuginfo":
                    return string.Format("X: {0} Y: {1} FPS:{2} {3}",
                                         MousePointIsometricGrid.X,
                                         MousePointIsometricGrid.Y,
                                         Fps,
                                         GameIsPaused ? "Paused" : "");
            }
            return valueToGet + " invalid";
        }
        private int _frameRate;
        private int _frameCounter;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        public void UpdateFps(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            _frameCounter++;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }

            Fps = _frameRate;
            UpdateSelectionInfo();
        }

        public void UpdateInternalMetrics(GameTime gameTime)
        {
            UserInterface.Update();
            if (_interfaceState.Peek() == InterfaceState.MakeSelection)
            {
                _endMouseDownPoint = MousePointIsometricGrid;
            }
            if (_interfaceState.Peek() == InterfaceState.MoveCamera)
            {
                CameraPos.X += MousePointDelta.X;
                CameraPos.Y += MousePointDelta.Y;
            }
            UpdateFps(gameTime);
        }
    }
}
