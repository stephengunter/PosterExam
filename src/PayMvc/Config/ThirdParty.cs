using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PayMvc.Config
{
    public class ThirdPartySettings : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public string Id => this["Id"] as string;

        [ConfigurationProperty("Url")]
        public string Url => this["Url"] as string;

        [ConfigurationProperty("HashKey")]
        public string HashKey => this["HashKey"] as string;

        [ConfigurationProperty("HashIV")]
        public string HashIV => this["HashIV"] as string;

        [ConfigurationProperty("MerchantID")]
        public string MerchantID => this["MerchantID"] as string;
    }


    [ConfigurationCollection(typeof(ThirdPartySettings))]
    public class ThirdPartyCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "provider";

        protected override string ElementName => PropertyName;

        public override ConfigurationElementCollectionType CollectionType
            => ConfigurationElementCollectionType.BasicMapAlternate;

        protected override bool IsElementName(string elementName)
            => elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);

        public override bool IsReadOnly() => false;

        protected override ConfigurationElement CreateNewElement() => new ThirdPartySettings();

        protected override object GetElementKey(ConfigurationElement element) => ((ThirdPartySettings)(element)).Id;

        public ThirdPartySettings this[int idx] => (ThirdPartySettings)BaseGet(idx);

    }

    public class ThirdPartySection : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ThirdPartyCollection Providers
        {
            get { return ((ThirdPartyCollection)(base["providers"])); }
            set { base["providers"] = value; }
        }
    }
}