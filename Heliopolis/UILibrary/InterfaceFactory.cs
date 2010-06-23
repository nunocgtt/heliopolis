using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Heliopolis.UILibrary
{
    public class InterfaceFactory
    {
        private UserInterface _userInterface;
        private XmlDocument _xmlDocument;
        //protected Type InheritedClassType;
        //protected object InheritedClass;
        public static Dictionary<string, Type> ReferencedTypes;

        private object _eventHandler;

        public UserInterface CreateUserInterface(Game xnaGame, string filename, int width, int height, object eventHandler)
        {
            _eventHandler = eventHandler;
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(filename);
            _userInterface = new UserInterface {Game = xnaGame};
            _userInterface.XmlSetVariables = new Dictionary<string, int>();
            _userInterface.XmlSetVariables["screenwidth"] = width;
            _userInterface.XmlSetVariables["screenheight"] = width;
            if (ReferencedTypes == null)
                ReferencedTypes = GetReferencedTypes();
            Initialize();
            return _userInterface;
        }

        protected void Initialize()
        {
            _userInterface.Panels = new Dictionary<string, Panel>();
            _userInterface.PanelGroups = new Dictionary<string, List<Panel>>();
            XmlNode inheritsAttribute = _xmlDocument.SelectSingleNode(@"/Interface/@inherits");
            XmlNode themeAttribute = _xmlDocument.SelectSingleNode(@"/Interface/@theme");

            if (themeAttribute != null)
            {
                string themeFile = @"Content\" + themeAttribute.Value;

                try
                {
                    var themeXML = new XmlDocument();
                    themeXML.Load(themeFile);
                    _userInterface.Theme = new Theme(themeXML, _userInterface);
                }
                catch (Exception ex)
                {
                    throw new Exception("Interface Initialization Failed: Theme could not be loaded: " + themeFile, ex);
                }
            }
            else
            {
                try
                {
                    var themeXML = new XmlDocument();
                    themeXML.Load(@"Content\Themes\SimpleUI\SimpleLight\SimpleLight.xml");
                    _userInterface.Theme = new Theme(themeXML, _userInterface);
                }
                catch (Exception ex)
                {
                    throw new Exception("Interface Initialization Failed: Default theme could not be loaded.", ex);
                }
            }

            _userInterface.InheritedClassType = null;
            _userInterface.InheritedClass = null;

            if (inheritsAttribute != null)
            {
                string inheritedClassName = inheritsAttribute.Value;

                if (ReferencedTypes.Keys.Contains(inheritedClassName))
                {
                    try
                    {
                        _userInterface.InheritedClassType = ReferencedTypes[inheritedClassName];
                        _userInterface.InheritedClass = _eventHandler;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            "Interface Initialization Failed: Error creating instance of inherited class: " +
                            inheritedClassName, ex);
                    }
                }
            }
            setVariables(_xmlDocument.SelectNodes(@"/Interface/Variables/Variable"));
            addPanels(_xmlDocument.SelectNodes(@"/Interface/Panels/Panel"));
        }

        private void setVariables(XmlNodeList xmlNodeList)
        {
            if (xmlNodeList != null)
                foreach (XmlNode panelNode in xmlNodeList)
                {
                    XmlNode nameNode = panelNode.SelectSingleNode(@"@name");
                    XmlNode valueNode = panelNode.SelectSingleNode(@"@value");

                    if (nameNode != null && valueNode != null)
                    {
                        _userInterface.XmlSetVariables[nameNode.Value] = Int32.Parse(valueNode.Value);
                    }
                }
        }

        private void addPanels(XmlNodeList panelNodes)
        {
            if (panelNodes != null)
                foreach (XmlNode panelNode in panelNodes)
                {
                    XmlNode inheritNode = panelNode.SelectSingleNode(@"@type");

                    if (inheritNode != null)
                    {
                        string className = inheritNode.Value;

                        if (ReferencedTypes.Keys.Contains(className))
                        {
                            _userInterface.RegisterPanel(new PanelFactory().CreateNewPanelByType(panelNode, _userInterface,
                                                                                                 ReferencedTypes[className]));
                        }
                    }
                    else
                    {
                        _userInterface.RegisterPanel(new PanelFactory().CreateNewPanelByType(panelNode, _userInterface, typeof(Panel)));
                    }
                }
        }

        private Dictionary<string, Type> GetReferencedTypes()
        {
            var inheritanceTypes = new Dictionary<string, Type>();

            XmlNodeList inheritsNodes = _xmlDocument.SelectNodes(@"//@*[name() = 'inherits' or name() = 'type']");
            if (inheritsNodes != null)
                foreach (XmlNode inheritsNode in inheritsNodes)
                {
                    string className = inheritsNode.Value;

                    if (!inheritanceTypes.Keys.Contains(className))
                    {
                        inheritanceTypes.Add(className, null);
                    }
                }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (inheritanceTypes.Keys.Contains(type.FullName))
                    {
                        inheritanceTypes[type.FullName] = type;
                    }
                }
            }

            return inheritanceTypes;
        }

    }
}
