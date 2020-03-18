using System;
namespace SAMLPortal.Models
{
    public class SAMLApp
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string Secret { get; set; }

        public SAMLApp()
        {
        }
    }
}
