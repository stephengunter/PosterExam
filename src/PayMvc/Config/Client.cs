using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PayMvc.Config
{
    public class ClientSettings : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public string Id => this["Id"] as string;

        [ConfigurationProperty("Url")]
        public string Url => this["Url"] as string;
    }


    [ConfigurationCollection(typeof(ClientSettings))]
    public class ClientCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "client";

        protected override string ElementName => PropertyName;

        public override ConfigurationElementCollectionType CollectionType
            => ConfigurationElementCollectionType.BasicMapAlternate;

        protected override bool IsElementName(string elementName)
            => elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);

        public override bool IsReadOnly() => false;

        protected override ConfigurationElement CreateNewElement() => new ClientSettings();

        protected override object GetElementKey(ConfigurationElement element) => ((ClientSettings)(element)).Id;

        public ClientSettings this[int idx] => (ClientSettings)BaseGet(idx);

    }

    public class ClientSection : ConfigurationSection
    {
        [ConfigurationProperty("clients")]
        public ClientCollection Clients
        {
            get { return ((ClientCollection)(base["clients"])); }
            set { base["clients"] = value; }
        }
    }
}