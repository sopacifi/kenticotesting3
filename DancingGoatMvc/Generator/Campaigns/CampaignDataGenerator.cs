﻿using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Activities;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.OnlineForms;
using CMS.Scheduler;
using CMS.WebAnalytics;

namespace DancingGoat.Generator
{
    /// <summary>
    /// Contains methods for generating sample data for the Campaign module.
    /// </summary>
    internal class CampaignDataGenerator
    {
        private readonly ISiteInfo mSite;
        private readonly string mContactFirstNamePrefix;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="site">Site the campaign data will be generated for</param>
        /// <param name="contactFirstNamePrefix">First name prefix of contacts generated for sample campaigns.</param>
        public CampaignDataGenerator(ISiteInfo site, string contactFirstNamePrefix)
        {
            mSite = site;
            mContactFirstNamePrefix = contactFirstNamePrefix;
        }


        /// <summary>
        /// Code name of the running 'Cafe sample promotion' campaign.
        /// </summary>
        private const string CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING = "DancingGoatMVC.CafeSamplePromotion";


        /// <summary>
        /// Code name of the finished 'Cafe sample promotion' campaign.
        /// </summary>
        private const string CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED = "DancingGoatMVC.CafeSamplePromotionTest";


        private const string CONVERSION_PAGEVISIT_COFFEE_SAMPLES = "Coffee samples";        
        private const string CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST = "Coffee sample list";
        private const string CONVERSION_PURCHASE = "Purchase";

        private const string PAGE_PATH_COFFEE_SAMPLES = "/Landing-pages/Coffee-samples";        

        private const string COFFEE_SAMPLE_LIST_FORM_CODE_NAME = "DancingGoatMvcCoffeeSampleList";

        private readonly Guid NEWSLETTER_COLOMBIA_COFFEE_SAMPLE_PROMOTION_TEST_ISSUE_GUID =
            Guid.Parse("36B432DA-1FB6-42CD-99DC-E80738CEE49D");
        private readonly Guid NEWSLETTER_COLOMBIA_COFFEE_SAMPLE_PROMOTION_ISSUE_GUID =
            Guid.Parse("7E118DD3-7770-4BA7-96FE-805DE6A1CF6F");

        private const int CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED_CONTACTS_COUNT = 100;
        /* When 0 is used in TopN method, all records are returned. */
        private const int CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING_CONTACTS_COUNT = 0;


        private readonly Dictionary<string, IEnumerable<ActivityDataParameters>> CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED_HITS = new Dictionary
            <string, IEnumerable<ActivityDataParameters>>
        {
            {
                CONVERSION_PAGEVISIT_COFFEE_SAMPLES, new List<ActivityDataParameters>
                {
                    GetActivityDataParameters("colombia_coffee_sample_promotion", String.Empty, 7),
                    GetActivityDataParameters("colombia_coffee_sample_promotion", "colombia_mail_1", 13),
                    GetActivityDataParameters("colombia_coffee_sample_promotion", "colombia_mail_2", 16),
                    GetActivityDataParameters("facebook", "fb_colombia", 30),
                    GetActivityDataParameters("twitter", String.Empty, 4),
                    GetActivityDataParameters("twitter", "twitter_post_1", 42),
                    GetActivityDataParameters("twitter", "twitter_post_2", 21)
                }
            },
            {
                CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST, new List<ActivityDataParameters>
                {
                    GetActivityDataParameters("colombia_coffee_sample_promotion", String.Empty, 5),
                    GetActivityDataParameters("colombia_coffee_sample_promotion", "colombia_mail_1", 11),
                    GetActivityDataParameters("colombia_coffee_sample_promotion", "colombia_mail_2", 15),
                    GetActivityDataParameters("facebook", String.Empty, 4),
                    GetActivityDataParameters("facebook", "fb_colombia", 21),
                    GetActivityDataParameters("twitter", "twitter_post_1", 9),
                    GetActivityDataParameters("twitter", "twitter_post_2", 7)
                }
            },
            {
                CONVERSION_PURCHASE, new List<ActivityDataParameters>
                {
                    GetActivityDataParameters("colombia_coffee_sample_promotion", String.Empty, 2),
                    GetActivityDataParameters("colombia_coffee_sample_promotion", "colombia_mail_1", 5),
                    GetActivityDataParameters("colombia_coffee_sample_promotion", "colombia_mail_2", 9),
                    GetActivityDataParameters("facebook", String.Empty, 1),
                    GetActivityDataParameters("facebook", "fb_colombia", 13),
                    GetActivityDataParameters("twitter", "twitter_post_1", 7),
                    GetActivityDataParameters("twitter", "twitter_post_2", 4)
                }
            }
        };


        private readonly Dictionary<string, IEnumerable<ActivityDataParameters>> CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING_HITS = new Dictionary
            <string, IEnumerable<ActivityDataParameters>>
        {
            {
                CONVERSION_PAGEVISIT_COFFEE_SAMPLES, new List<ActivityDataParameters>
                {
                    GetActivityDataParameters("linkedin", "linkedin_colombia", 1429),
                    GetActivityDataParameters("facebook", String.Empty, 66),
                    GetActivityDataParameters("facebook", "fb_colombia_1", 1246),
                    GetActivityDataParameters("facebook", "fb_colombia_2", 1152),
                    GetActivityDataParameters("twitter", "twitter_colombia", 310)
                }
            },
            {
                CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST, new List<ActivityDataParameters>
                {
                    GetActivityDataParameters("linkedin", "linkedin_colombia", 175),
                    GetActivityDataParameters("facebook", String.Empty, 77),
                    GetActivityDataParameters("facebook", "fb_colombia_1", 248),
                    GetActivityDataParameters("facebook", "fb_colombia_2", 173),
                    GetActivityDataParameters("twitter", "twitter_colombia", 58)
                }
            },
            {
                CONVERSION_PURCHASE, new List<ActivityDataParameters>
                {
                    GetActivityDataParameters("linkedin", "linkedin_colombia", 113),
                    GetActivityDataParameters("facebook", String.Empty, 51),
                    GetActivityDataParameters("facebook", "fb_colombia_1", 163),
                    GetActivityDataParameters("facebook", "fb_colombia_2", 99),
                    GetActivityDataParameters("twitter", "twitter_colombia", 38)
                }
            }
        };


        private static ActivityDataParameters GetActivityDataParameters(string utmSource, string utmContent, int count)
        {
            return new ActivityDataParameters
            {
                UtmSource = utmSource,
                UtmContent = utmContent,
                Count = count
            };
        }


        /// <summary>
        /// Performs campaigns sample data generating.
        /// </summary>
        public void Generate()
        {
            CampaignInfoProvider.GetCampaigns()
                .WhereStartsWith("CampaignName", "DancingGoatMVC.")
                .OnSite(mSite.SiteID)
                .ToList()
                .ForEach(CampaignInfoProvider.DeleteCampaignInfo);

            /* Generate campaigns. */            
            GenerateCafePromotionSampleCampaign();

            /* Generate campaign objectives */
            GenerateCampaignObjective(CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING, CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST, 600);
            GenerateCampaignObjective(CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED, CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST, 50);

            /* Generate activities */
            GenerateActivities(CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING, CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING_HITS, CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING_CONTACTS_COUNT);
            GenerateActivities(CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED, CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED_HITS, CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED_CONTACTS_COUNT);

            new CalculateCampaignConversionReportTask().Execute(new TaskInfo() { TaskSiteID = mSite.SiteID });
        }


        private void GenerateCampaignObjective(string campaignName, string conversionName, int objectiveValue)
        {
            var campaign = CampaignInfoProvider.GetCampaignInfo(campaignName, mSite.SiteName);
            if (campaign == null)
            {
                return;
            }

            var conversion = CampaignConversionInfoProvider.GetCampaignConversions()
                                                           .WhereEquals("CampaignConversionDisplayName", conversionName)
                                                           .WhereEquals("CampaignConversionCampaignID", campaign.CampaignID)
                                                           .FirstOrDefault();
            if (conversion == null)
            {
                return;
            }

            var objective = new CampaignObjectiveInfo
            {
                CampaignObjectiveCampaignID = campaign.CampaignID,
                CampaignObjectiveCampaignConversionID = conversion.CampaignConversionID,
                CampaignObjectiveValue = objectiveValue
            };

            CampaignObjectiveInfoProvider.SetCampaignObjectiveInfo(objective);
        }


        /// <summary>
        /// Generates campaign.
        /// </summary>
        /// <param name="campaignData">Campaign data for generating.</param>
        private void GenerateCampaign(CampaignData campaignData)
        {
            var siteName = mSite.SiteName;
            var campaign = CampaignInfoProvider.GetCampaignInfo(campaignData.CampaignName, siteName);
            if (campaign != null)
            {
                return;
            }

            campaign = new CampaignInfo
            {
                CampaignName = campaignData.CampaignName,
                CampaignDisplayName = campaignData.CampaignDisplayName,
                CampaignDescription = campaignData.CampaignDescription,
                CampaignOpenFrom = campaignData.CampaignOpenFrom,
                CampaignCalculatedTo = campaignData.CampaignOpenFrom,
                CampaignOpenTo = campaignData.CampaignOpenTo,
                CampaignSiteID = mSite.SiteID,
                CampaignUTMCode = campaignData.CampaignUTMCode
            };

            CampaignInfoProvider.SetCampaignInfo(campaign);

            /* Add email to campaign promotion section  */
            CampaignDataGeneratorHelpers.AddNewsletterAsset(campaign, campaignData.CampaignEmailPromotion);
            var issue = ProviderHelper.GetInfoByGuid(PredefinedObjectType.NEWSLETTERISSUE, campaignData.CampaignEmailPromotion, campaign.CampaignSiteID);

            if (issue != null)
            {
                issue.SetValue("IssueStatus", 5);
                issue.SetValue("IssueMailoutTime", campaignData.CampaignOpenFrom);

                issue.Update();
            }

            /* Add page assets to campaign content inventory section */
            foreach (var assetUrlGuid in campaignData.CampaignContentInventory)
            {
                CampaignDataGeneratorHelpers.AddPageAsset(campaign.CampaignID, assetUrlGuid);
            }

            /* Add conversions to campaign report setup section */
            foreach (var conversion in campaignData.CampaignReportSetup)
            {
                CampaignDataGeneratorHelpers.CreateConversion(campaign.CampaignID, conversion);
            }
        }


        private void GenerateCafePromotionSampleCampaign()
        {
            var campaignCafePromotion = new CampaignData
            {
                CampaignName = CAMPAIGN_CAFE_SAMPLE_PROMOTION_RUNNING,
                CampaignDisplayName = "Cafe sample promotion",
                CampaignDescription = "The goal of this campaign is to increase the number of visitors in our cafes. We want to achieve that by sending out free coffee sample coupons that customers can redeem at the cafes.",
                CampaignUTMCode = "cafe_sample_promotion_mvc_running",
                CampaignOpenFrom = DateTime.Now.AddDays(-14),
                CampaignOpenTo = DateTime.MinValue,
                CampaignEmailPromotion = NEWSLETTER_COLOMBIA_COFFEE_SAMPLE_PROMOTION_ISSUE_GUID,
                CampaignContentInventory = new List<Guid>
                {
                      new Guid("0964F55A-6BB3-49FB-9A8D-CE82738292C5")
                },
                CampaignReportSetup = PrepareCafeSamplePromotionConversions()
            };

            /* Generate running 'Cafe sample promotion' campaign. */
            GenerateCampaign(campaignCafePromotion);

            /* Generate finished 'Cafe sample promotion' campaign. */
            campaignCafePromotion.CampaignName = CAMPAIGN_CAFE_SAMPLE_PROMOTION_FINISHED;
            campaignCafePromotion.CampaignEmailPromotion = NEWSLETTER_COLOMBIA_COFFEE_SAMPLE_PROMOTION_TEST_ISSUE_GUID;
            campaignCafePromotion.CampaignDisplayName = "Cafe sample promotion test";
            campaignCafePromotion.CampaignOpenTo = campaignCafePromotion.CampaignOpenFrom.AddDays(6);            
            campaignCafePromotion.CampaignUTMCode = "cafe_sample_promotion_mvc_finished";
            campaignCafePromotion.CampaignContentInventory = new List<Guid>
                {
                      new Guid("0964F55A-6BB3-49FB-9A8D-CE82738292C6")
                };

            GenerateCampaign(campaignCafePromotion);
        }


        private void GenerateActivities(string campaignName, Dictionary<string, IEnumerable<ActivityDataParameters>> conversionHits, int contactsCount)
        {
            var siteName = mSite.SiteName;
            var campaignCafePromotion = CampaignInfoProvider.GetCampaignInfo(campaignName, siteName);
            
            var pageVisitColombia = CampaignDataGeneratorHelpers.GetDocument(PAGE_PATH_COFFEE_SAMPLES);
            var formFreeSample = BizFormInfoProvider.GetBizFormInfo(COFFEE_SAMPLE_LIST_FORM_CODE_NAME, mSite.SiteID);

            /* Generate activities for campaign */
            CampaignDataGeneratorHelpers.DeleteOldActivities(campaignCafePromotion.CampaignUTMCode);
            var campaignContactsIDs = new ContactsIDData(mContactFirstNamePrefix, contactsCount);

            CampaignDataGeneratorHelpers.GenerateActivities(conversionHits[CONVERSION_PAGEVISIT_COFFEE_SAMPLES], campaignCafePromotion, PredefinedActivityType.PAGE_VISIT, campaignContactsIDs, activityUrl: DocumentURLProvider.GetAbsoluteLiveSiteURL(pageVisitColombia));
            CampaignDataGeneratorHelpers.GenerateActivities(conversionHits[CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST], campaignCafePromotion, PredefinedActivityType.BIZFORM_SUBMIT, campaignContactsIDs, formFreeSample.FormID);
            CampaignDataGeneratorHelpers.GenerateActivities(conversionHits[CONVERSION_PURCHASE], campaignCafePromotion, PredefinedActivityType.PURCHASE, campaignContactsIDs);
        }


        private IEnumerable<CampaignConversionData> PrepareCafeSamplePromotionConversions()        {
            
            var pageCoffeeSamples = CampaignDataGeneratorHelpers.GetDocument(PAGE_PATH_COFFEE_SAMPLES);
            var formCoffeeSampleList = BizFormInfoProvider.GetBizFormInfo(COFFEE_SAMPLE_LIST_FORM_CODE_NAME, mSite.SiteID);

            var liveUrl = DocumentURLProvider.GetAbsoluteLiveSiteURL(pageCoffeeSamples);

            return new List<CampaignConversionData>
            {
                /*  Campaign conversions. */
                new CampaignConversionData
                {
                    ConversionName = "coffee_samples",
                    ConversionDisplayName = pageCoffeeSamples.DocumentName,
                    ConversionActivityType = PredefinedActivityType.PAGE_VISIT,
                    ConversionItemID = null,
                    ConversionOrder = 1,
                    ConversionIsFunnelStep = false,
                    ConversionUrl = liveUrl
                },
                new CampaignConversionData
                {
                    ConversionName = "coffee_sample_list",
                    ConversionDisplayName = CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST,
                    ConversionActivityType = PredefinedActivityType.BIZFORM_SUBMIT,
                    ConversionItemID = formCoffeeSampleList.FormID,
                    ConversionOrder = 2,
                    ConversionIsFunnelStep = false
                },                

                /* Campaign journey steps. */
                new CampaignConversionData
                {
                    ConversionName = "coffee_samples_1",
                    ConversionDisplayName = pageCoffeeSamples.DocumentName,
                    ConversionActivityType = PredefinedActivityType.PAGE_VISIT,
                    ConversionItemID = null,
                    ConversionOrder = 1,
                    ConversionIsFunnelStep = true,
                    ConversionUrl = liveUrl
                },
                new CampaignConversionData
                {
                    ConversionName = "coffee_sample_list_1",
                    ConversionDisplayName = CONVERSION_FORMSUBMISSION_COFFEE_SAMPLE_LIST,
                    ConversionActivityType = PredefinedActivityType.BIZFORM_SUBMIT,
                    ConversionItemID = formCoffeeSampleList.FormID,
                    ConversionOrder = 2,
                    ConversionIsFunnelStep = true
                },
                new CampaignConversionData
                {
                    ConversionName = "purchase",
                    ConversionDisplayName = CONVERSION_PURCHASE,
                    ConversionActivityType = PredefinedActivityType.PURCHASE,
                    ConversionItemID = 0,
                    ConversionOrder = 3,
                    ConversionIsFunnelStep = true
                }
            };
        }        
    }
}