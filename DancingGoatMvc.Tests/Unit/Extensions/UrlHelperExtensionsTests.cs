using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

using CMS.Tests;

using DancingGoat.Helpers;

using NSubstitute;

using NUnit.Framework;

namespace DancingGoat.Tests
{
    public class UrlHelperExtensionsTests
    {
        [TestFixture]
        public class CanonicalUrlTests : UnitTests
        {
            private RequestContext mRequestContext;


            [SetUp]
            public void SetUp()
            {
                mRequestContext = Substitute.For<RequestContext>();

                mRequestContext.HttpContext.Request.ApplicationPath.Returns("/DancingGoatMvc");
            }


            [Test]
            public void CanonicalUrl_PageMainUrlIsSetInContext_ReturnsPageMainUrl_WithoutTrailingSlash()
            {
                mRequestContext.HttpContext.Items.Returns(new Dictionary<string, object> { { "Kentico.AlternativeUrls.PageMainUrl", "~/Coffee-samples/" } });

                mRequestContext.HttpContext.Request.Url.Returns(new System.Uri("http://localhost/DancingGoatMvc/some-other/page"));

                var helper = new UrlHelper(mRequestContext);

                Assert.AreEqual("http://localhost/DancingGoatMvc/Coffee-samples", helper.CanonicalUrl());
            }


            [Test]
            public void CanonicalUrl_PageMainUrlIsUnknown_ReturnsCurrentAbsoluteUrl_WithoutQueryStringAndTrailingSlash()
            {
                mRequestContext.HttpContext.Items.Returns(new Dictionary<string, object>());
                mRequestContext.HttpContext.Request.Url.Returns(new System.Uri("http://localhost/DancingGoatMvc/en-US/LandingPage/Coffee-samples/?query=string"));

                var helper = new UrlHelper(mRequestContext);

                Assert.AreEqual("http://localhost/DancingGoatMvc/en-US/LandingPage/Coffee-samples", helper.CanonicalUrl());
            }
        }
    }
}
