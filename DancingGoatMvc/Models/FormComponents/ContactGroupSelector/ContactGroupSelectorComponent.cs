﻿using System.Collections.Generic;
using System.Linq;

using CMS.ContactManagement;
using Kentico.Forms.Web.Mvc;

using DancingGoat.Models.FormComponents.ContactGroupSelector;


[assembly: RegisterFormComponent("DancingGoat.ContactGroupSelector", typeof(ContactGroupSelectorComponent), "Contact group selector", IsAvailableInFormBuilderEditor = false, ViewName = "FormComponents/_ContactGroupSelector")]
namespace DancingGoat.Models.FormComponents.ContactGroupSelector
{
    /// <summary>
    /// Represents a contact group selector
    /// </summary>
    public class ContactGroupSelectorComponent : FormComponent<ContactGroupSelectorProperties, List<string>>
    {
        [BindableProperty]
        public List<ContactGroupSelectorListItem> Items
        {
            get;
            set;
        } = GetItems();


        public override List<string> GetValue()
        {
            var selectedCodeName = Items.Where(x => x.Checked)
                                        .Select(x => x.CodeName).ToList();

            return selectedCodeName;
        }


        public override void SetValue(List<string> value)
        {           
            var items = GetItems();
            if (value != null)
            {
                items.ForEach(x => x.Checked = value.Contains(x.CodeName));
            }

            Items = items;
        }


        private static List<ContactGroupSelectorListItem> GetItems()
        {
            return ContactGroupInfoProvider.GetContactGroups().Select(x => new ContactGroupSelectorListItem
            {
                CodeName = x.ContactGroupName,
                DisplayName = x.ContactGroupDisplayName,
                Checked = false
            }).ToList();
        }
    }
}