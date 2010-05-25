using System.Xml;

namespace Heliopolis.UILibrary
{
    public interface IPanel
    {
        void LoadEventHandlers(XmlNode xmlNode);
        void LoadDesignElements();
        void Load(XmlNode xmlNode);
        void LoadDrawRectangle(XmlNode xmlNode);
    }
}
