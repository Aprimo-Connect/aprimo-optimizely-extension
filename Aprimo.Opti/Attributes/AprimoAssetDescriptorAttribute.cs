using System;
using System.Linq;

namespace Aprimo.Opti.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AprimoAssetDescriptorAttribute : Attribute
    {
        public string[] Extensions { get; set; }

        public string ExtensionString
        {
            get
            {
                return string.Join(",", Extensions);
            }
            set
            {
                Extensions = (from x in value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                              select x.Trim() into x
                              where !string.IsNullOrWhiteSpace(x)
                              select x.ToUpperInvariant()).ToArray();
            }
        }
    }
}