using System.Xml;

namespace Heliopolis.UILibrary
{
    public class UIPanel_Frame :  UIPanel, IPanel
    {
        public void LoadEventHandlers(XmlNode xmlNode)
        {
            
        }

        public void LoadDesignElements()
        {
            BackdropTexture = base.GetBackdropTexture("Frame-Backdrop");
        }


        public void Load(XmlNode xmlNode)
        {
            
        }


        public void LoadDrawRectangle(XmlNode xmlNode)
        {
           
        }


    }
}
