using System;
using System.Linq;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.FormEngine;
using CMS.OnlineForms;

using Kentico.Forms.Web.Mvc;
using Kentico.Forms.Web.Mvc.Internal;

namespace DancingGoat.Generator
{
    /// <summary>
    /// Contains methods for generating sample data for the Campaign module.
    /// </summary>
    internal class FormConsentGenerator
    {
        private readonly ISiteInfo mSite;
        private const string FORM_FIELD_NAME = "Consent";

        private const string FORM_NAME = "DancingGoatMvcCoffeeSampleList";
        private readonly string FORM_CLASS_NAME = $"BizForm.{FORM_NAME}";
        internal const string CONSENT_NAME = "DancingGoatMvcCoffeeSampleListForm";
        internal const string CONSENT_DISPLAY_NAME = "Dancing Goat MVC - Coffee sample list form";
        private const string CONSENT_SHORT_TEXT_EN = "I hereby accept that these provided information can be used for marketing purposes and targeted website content.";
        private const string CONSENT_SHORT_TEXT_ES = "Por lo presente acepto que esta información proporcionada puede ser utilizada con fines de marketing y contenido de sitios web dirigidos.";
        private const string CONSENT_LONG_TEXT_EN = @"This is a sample consent declaration used for demonstration purposes only. 
                We strongly recommend forming a consent declaration suited for your website and consulting it with a lawyer.";
        private const string CONSENT_LONG_TEXT_ES = @"Esta es una declaración de consentimiento de muestra que se usa sólo para fines de demostración.
                Recomendamos encarecidamente formar una declaración de consentimiento adecuada para su sitio web y consultarla con un abogado.";


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="site">Site the form consent will be generated for</param>
        public FormConsentGenerator(ISiteInfo site)
        {
            mSite = site;
        }


        /// <summary>
        /// Generates sample form consent data. Suitable only for Dancing Goat Mvc demo site.
        /// </summary>
        public void Generate()
        {
            CreateConsent();
            UpdateForm();
        }


        private void CreateConsent()
        {
            if (ConsentInfoProvider.GetConsentInfo(CONSENT_NAME) != null)
            {
                return;
            }

            var consent = new ConsentInfo
            {
                ConsentName = CONSENT_NAME,
                ConsentDisplayName = CONSENT_DISPLAY_NAME,
            };

            consent.UpsertConsentText("en-US", CONSENT_SHORT_TEXT_EN, CONSENT_LONG_TEXT_EN);
            consent.UpsertConsentText("es-ES", CONSENT_SHORT_TEXT_ES, CONSENT_LONG_TEXT_ES);

            ConsentInfoProvider.SetConsentInfo(consent);
        }


        private void UpdateForm()
        {
            var formClassInfo = DataClassInfoProvider.GetDataClassInfo(FORM_CLASS_NAME);
            if (formClassInfo == null)
            {
                return;
            }

            var form = new FormInfo(formClassInfo.ClassFormDefinition);
            if (form.FieldExists(FORM_FIELD_NAME))
            {
                return;
            }

            // Update ClassFormDefinition
            var field = CreateFormField();
            form.AddFormItem(field);
            formClassInfo.ClassFormDefinition = form.GetXmlDefinition();
            formClassInfo.Update();

            // Update Form builder JSON
            IFormBuilderConfigurationSerializer formBuilderConfigurationSerializer = Service.Resolve<IFormBuilderConfigurationSerializer>();
            var contactUsForm = BizFormInfoProvider.GetBizFormInfo(FORM_NAME, mSite.SiteID);
            var formBuilderConfiguration = formBuilderConfigurationSerializer.Deserialize(contactUsForm.FormBuilderLayout);
            formBuilderConfiguration.EditableAreas.LastOrDefault()
                                    .Sections.LastOrDefault()
                                    .Zones.LastOrDefault()
                                    .FormComponents
                                        .Add(new FormComponentConfiguration { Properties = new ConsentAgreementProperties() { Guid = field.Guid } });
            contactUsForm.FormBuilderLayout = formBuilderConfigurationSerializer.Serialize(formBuilderConfiguration, true);
            contactUsForm.Update();
        }


        private static FormFieldInfo CreateFormField()
        {
            var field = new FormFieldInfo
            {
                Name = FORM_FIELD_NAME,
                DataType = FieldDataType.Guid,
                FieldType = FormFieldControlTypeEnum.Unknown,
                System = false,
                Visible = true,
                PublicField = true,
                AllowEmpty = true,
                Guid = Guid.NewGuid()
            };

            field.Settings["componentidentifier"] = ConsentAgreementComponent.IDENTIFIER;
            field.Settings[nameof(ConsentAgreementProperties.ConsentCodeName)] = CONSENT_NAME;

            field.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, String.Empty);
            field.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, String.Empty);
            field.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, String.Empty);

            return field;
        }
    }
}