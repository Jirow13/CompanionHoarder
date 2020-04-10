using System.Xml;
using TaleWorlds.Library;

namespace Companion_Hoarder
{
    class CompanionHoarderSettings
    {
        public string CompanionHoarderMode;
        public int  CompanionNumber;

         public CompanionHoarderSettings()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(BasePath.Name + "Modules/CompanionHoarder/Config.xml");

            foreach (object obj in xmlDocument.SelectSingleNode("CompanionHoarder").ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                string text = xmlNode.Name.ToLower();
                if (text != null)
                {
                    #region Companions
                    if (text == "CompanionHoarderMode")
                    {
                        if (xmlNode.InnerText == "Number") { this.CompanionHoarderMode = "Number"; }
                         else { this.CompanionHoarderMode = "default"; }
                    }
                    if (text == "CompanionAmount") { this.CompanionNumber = int.Parse(xmlNode.InnerText); }
                    #endregion

                 
               

                

                   
                }
            }
        }
    }
}
