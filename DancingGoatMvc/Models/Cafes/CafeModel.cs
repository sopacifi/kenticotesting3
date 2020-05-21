using CMS.DocumentEngine;

using DancingGoat.Models.Contacts;

namespace DancingGoat.Models.Cafes
{
    public class CafeModel
    {
        public DocumentAttachment Photo { get; set; }


        public string Note { get; set; }


        public ContactModel Contact { get; set; }
    }
}