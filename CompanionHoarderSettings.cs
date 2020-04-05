using System.Xml;
using TaleWorlds.Library;

namespace ComapanionHoarder
{
    class CompanionHoarderSettings
    {
        public string companionMode;
        public int CompanionNumber, CompanionLimit;

        
        public CompanionHoarderSettings()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(BasePath.Name + "Modules/ClanTweaker/Config.xml");

            foreach (object obj in xmlDocument.SelectSingleNode("ClanTweaker").ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                string text = xmlNode.Name.ToLower();
                if (text != null)
                {
                    

                    #region Companion
                    if (text == "CompanionNumber") { this.CompanionNumber = int.Parse(xmlNode.InnerText); }
                    if (text == "CompanionLimit") { this.CompanionLimit = int.Parse(xmlNode.InnerText); }
                    #endregion

                 

                }
            }
        }
    }
}
