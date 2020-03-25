using System;
namespace SAMLPortal.Models
{
    public class App
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Enabled { get; set; }

        public App()
        {
        }
    }
}
