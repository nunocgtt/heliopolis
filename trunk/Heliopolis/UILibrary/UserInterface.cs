﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Reflection;
using Heliopolis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Heliopolis.UILibrary
{
    public class UserInterface
    {
        public Game Game { get; set; }
        public XmlDocument InterfaceXML { get; set; }       
        public Dictionary<string, UIPanel> Panels { get; set; }
        public Dictionary<string, List<UIPanel>> PanelGroups { get; set; }

        public IGameValueProvider GameValueProvider { get; set; }

        public UITheme Theme { get; set; }

        public UIPanel Focus { get; set; }

        public KeyboardState PreviousKeyboardState { get; set; }
        public KeyboardState CurrentKeyboardState { get; set; }

        public MouseState PreviousMouseState { get; set; }
        public MouseState CurrentMouseState { get; set; }
                
        public Dictionary<string, Type> ReferencedTypes;

        public Dictionary<string, int> XmlSetVariables;

        public Type InheritedClassType;
        public object InheritedClass;

        internal UserInterface()
        {
            
        }

        public EventHandler GetInheritedEventHandler(string methodName)
        {
            MethodInfo methodInfo = this.InheritedClassType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            EventHandler eventHandler = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), this.InheritedClass, methodInfo);

            return eventHandler;
        }

        public void RegisterPanel(UIPanel panel)
        {
            if (Panels.Keys.Contains(panel.ID))
            {
                throw new Exception("UIPanel ID already in use: " + panel.ID);
            }
            Panels.Add(panel.ID, panel);
        }

        public void AddPanelToGroup(UIPanel panel, string groupId)
        {
            if (!PanelGroups.ContainsKey(groupId))
            {
                PanelGroups.Add(groupId, new List<UIPanel>());
            }
            PanelGroups[groupId].Add(panel);
        }

        public void SetPanelGroupVisibilty(string groupId, string panelId)
        {
            if (PanelGroups.ContainsKey(groupId))
            {
                foreach (var uiPanel in PanelGroups[groupId])
                {
                    uiPanel.Visible = (uiPanel.ID == panelId);
                }
            }
        }

        public UIPanel GetPanel(string panelID)
        {
            UIPanel panel = null;

            if (Panels.Keys.Contains(panelID))
            {
                panel = Panels[panelID];
            }

            return panel;
        }

        public void Update()
        {
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();

            foreach (UIPanel panel in Panels.Values)
            {
                if (!panel.Update())
                {
                    break;
                }
            }

            PreviousKeyboardState = CurrentKeyboardState;
            PreviousMouseState = CurrentMouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bool oldScissorTestEnable = Game.GraphicsDevice.RenderState.ScissorTestEnable;

            //Game.GraphicsDevice.RenderState.ScissorTestEnable = true;

            foreach (UIPanel panel in Panels.Values)
            {
                panel.Draw(spriteBatch);
            }

            Game.GraphicsDevice.RenderState.ScissorTestEnable = oldScissorTestEnable;
        }
    }
}
