using System;
namespace SAMLPortal.Models
{
    public class SAMLAppDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public SAMLAppDTO()
        {
        }
    }
}
