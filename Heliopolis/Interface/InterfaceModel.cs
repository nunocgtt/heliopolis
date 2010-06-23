using System;
using System.Collections.Generic;
using Heliopolis.GraphicsEngine;
using Heliopolis.UILibrary;
using Heliopolis.World;
using Heliopolis.World.InteractableObjects;
using Heliopolis.World.JobSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Heliopolis.World.BuildingManagement;

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
        MakeSelection,
        CurrentlyMakingSelection,
        PlacingBuilding
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
        private readonly GameWorld _world;
        public float Fps { get; set; }
        public InterfaceAction CurrentAction { get; set; }
        private UserInterface _userInterface;
        public bool MovingCamera { get; set; }

        private Building _buildingToPlace;

        public Building BuildingToPlace
        {
            get
            {
                return _buildingToPlace;
            }
        }

        public InterfaceState CurrentInterfaceState
        {
            get
            {
                return _interfaceState.Peek();
            }
        }

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
                if (CurrentInterfaceState == InterfaceState.MoveCamera)
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
            CurrentAction = null;
            UserInterface = new InterfaceFactory().CreateUserInterface(xnaGame, UIFileToLoad, screenSize.X, screenSize.Y, this);
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
            if (CurrentInterfaceState == InterfaceState.MakeSelection)
            {
                _startMouseDownPoint = MousePointIsometricGrid;
                _endMouseDownPoint = MousePointIsometricGrid;
                _interfaceState.Pop();
                _interfaceState.Push(InterfaceState.CurrentlyMakingSelection);
            }
        }

        public void EndSelection()
        {
            if (CurrentInterfaceState == InterfaceState.CurrentlyMakingSelection)
            {
                _endMouseDownPoint = MousePointIsometricGrid;
                ApplyAction();
                _interfaceState.Pop();
            }
        }

        public void SetNewMousePosition(Point newPosition, IsometricEngine gameEngine)
        {
            MousePointOld = MousePoint;
            MousePoint = newPosition;
            MousePointDelta = new Point(MousePointOld.X - MousePoint.X, MousePointOld.Y - MousePoint.Y);

            Point cameraOffset = new Point((CameraPos.X * -1 * ZoomLevel), ((CameraPos.Y * -1 * ZoomLevel)));
            MousePointIsometricGrid = Iso2D.ConvertScreenToTile(MousePoint, (gameEngine.TileSize.X * ZoomLevel), (gameEngine.TileSize.Y * ZoomLevel), gameEngine.FirstTileXyPosition(ZoomLevel), cameraOffset);
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
            if (CurrentInterfaceState == InterfaceState.CurrentlyMakingSelection || CurrentInterfaceState == InterfaceState.MakeSelection)
            {
                _endMouseDownPoint = MousePointIsometricGrid;
                SelectionTiles = CalculateSeletionTiles();
            }
            else
            {
                SelectionTiles = null;
            }
        }

        private List<Point> CalculateSeletionTiles()
        {
            var returnMe = new List<Point>();
            switch (CurrentSelectionState)
            {
                case SelectionState.Single:
                    returnMe.Add(MousePointIsometricGrid);
                    break;
                case SelectionState.Area:
                    if (CurrentInterfaceState == InterfaceState.CurrentlyMakingSelection)
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
                        returnMe.Add(MousePointIsometricGrid);
                    break;
                case SelectionState.Line:
                    if (CurrentInterfaceState == InterfaceState.CurrentlyMakingSelection)
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
        }

        public void UpdateInternalMetrics(GameTime gameTime)
        {   
            if (_interfaceState.Peek() == InterfaceState.MakeSelection)
            {
                _endMouseDownPoint = MousePointIsometricGrid;
            }
            if (_interfaceState.Peek() == InterfaceState.MoveCamera)
            {
                CameraPos.X += MousePointDelta.X;
                CameraPos.Y += MousePointDelta.Y;
            }
            if (CameraPos.Y < 0)
                CameraPos.Y = 0;
            if (CameraPos.X < 0)
                CameraPos.X = 0;
            UpdateFps(gameTime);
            UpdateSelectionInfo();
        }

        public bool CatchUIEvents()
        {
            return UserInterface.Update();
        }

        public void ChopWood(Object sender, EventArgs e)
        {
            CurrentAction = new InterfaceAction { ActionType = InterfaceActionType.ApplyDesignation, TargetHarvestType = "wood" };
            CurrentSelectionState = SelectionState.Area;
            _interfaceState.Push(InterfaceState.MakeSelection);
        }

        public void PlaceBuilding(Object sender, EventArgs e)
        {
            _interfaceState.Push(InterfaceState.PlacingBuilding);
            _buildingToPlace = BuildingFactory.Instance.GetNewBuilding("Carpenters");
        }

        public void ClickMe(Object sender, EventArgs e)
        {
            //DO STUFFF
        }
    }
}
