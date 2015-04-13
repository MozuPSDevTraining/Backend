using System;
using Autofac;
using Mozu.Api;
using Mozu.Api.Resources.Platform;
using Mozu.Api.ToolKit;
using Mozu.Api.ToolKit.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Mozu.Api.Resources.Commerce.Catalog.Admin.Attributedefinition;
using System.Linq;
using System.Collections.Generic;
using Mozu.Api.Contracts.ProductAdmin;

namespace MozuDataConnector.Test
{
    [TestClass]
    public class MozuDataConnectorTests
    {
        private IApiContext _apiContext;
        private IContainer _container;


        [TestInitialize]
        public void Init()
        {
            _container = new Bootstrapper().Bootstrap().Container;
            var appSetting = _container.Resolve<IAppSetting>();
            var tenantId = int.Parse(appSetting.Settings["TenantId"].ToString());
            var siteId = int.Parse(appSetting.Settings["SiteId"].ToString());

            _apiContext = new ApiContext(tenantId, siteId);
        }

        [TestMethod]
        public void Should_Connect_To_Tenant()
        {
            var tenantResource = new TenantResource(_apiContext);
        }

        [TestMethod]
        public void Get_Attributes()
        {
            var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

            var attributes = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;
        }

        [TestMethod]
        public void Get_Popularity_Attribute()
        {
            var filter = "attributeFQN eq " + "'tenant~popularity'";

            var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

            var attributes = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, filter).Result;
        }

        [TestMethod]
        public void Add_SunGlass_Style_Attribute()
        {
            var attributeFQN = "tenant~sunglass-style";
            var filter = string.Format("attributeFQN eq '{0}'", attributeFQN);

            var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

            var attributes = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, filter).Result;

            //add a using clause for System.Linq;
            var existingAttribute = attributes.SingleOrDefault(a => a.AttributeFQN == attributeFQN);

            if (existingAttribute == null)
            {
                var attributeCode = attributeFQN.Replace("tenant~", string.Empty);

                var attribute = new Mozu.Api.Contracts.ProductAdmin.Attribute()
                {
                    AdminName = attributeCode,
                    AttributeFQN = attributeFQN,
                    AttributeCode = attributeCode,
                    Content = new AttributeLocalizedContent()
                    {
                        LocaleCode = "en-US",
                        Name = attributeCode.Replace("-", " ")
                    },
                    DataType = "String",
                    InputType = "List",
                    IsExtra = false,
                    IsOption = true,
                    IsProperty = true,
                    LocalizedContent = null,
                    MasterCatalogId = _apiContext.MasterCatalogId,
                    Namespace = "tenant",
                    SearchSettings = new AttributeSearchSettings()
                    {
                        SearchableInAdmin = true,
                        SearchableInStorefront = true,
                        SearchDisplayValue = true
                    },
                    ValueType = "Predefined",
                    VocabularyValues = new System.Collections.Generic.List<AttributeVocabularyValue>() 
                    {
                        new AttributeVocabularyValue()
                        {
                            Value = "Pilot",
                            Content = new Mozu.Api.Contracts.ProductAdmin.AttributeVocabularyValueLocalizedContent()
                            { 
                                  LocaleCode = "en-US",
                                  StringValue = "Pilot"
                            }
                        }
                    }

                };

                var newAttribute = attributeHandler.AddAttribute(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, attribute).Result;
            };
        }

        [TestMethod]
        public void Update_SunGlass_Style_Attribute()
        {
            var attributeFQN = "tenant~sunglass-style";
            var attributeValues = "Rectangle|Rimless|Butterfly|Oval|Wrap";
            var filter = string.Format("attributeFQN eq '{0}'", attributeFQN);

            var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();
            var attributes = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 1, null, filter).Result;

            var existingAttribute = attributes.SingleOrDefault(a => a.AttributeFQN == attributeFQN);

            if (existingAttribute != null && existingAttribute.VocabularyValues != null)
            {
                foreach (var value in attributeValues.Split('|'))
                {
                    existingAttribute.VocabularyValues.Add(new AttributeVocabularyValue()
                    {
                        Value = value,
                        Content = new AttributeVocabularyValueLocalizedContent()
                        {
                            LocaleCode = "en-US",
                            StringValue = value
                        }
                    });
                }

                var newAttribute = attributeHandler.UpdateAttribute(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, existingAttribute).Result;
            }
        }

        [TestMethod]
        public void Add_SunGlass_Protection_Attribute()
        {
            var attributeFQN = "tenant~sunglass-protection";
            var filter = string.Format("attributeFQN eq '{0}'", attributeFQN);

            var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();
            var attributes = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 1, null, filter).Result;

            var existingAttribute = attributes.SingleOrDefault(a => a.AttributeFQN == attributeFQN);

            if (existingAttribute == null)
            {
                var attributeCode = attributeFQN.Replace("tenant~", string.Empty);

                var attribute = new Mozu.Api.Contracts.ProductAdmin.Attribute()
                {
                    AdminName = attributeCode,
                    AttributeFQN = attributeFQN,
                    AttributeCode = attributeCode,
                    Content = new AttributeLocalizedContent()
                    {
                        LocaleCode = "en-US",
                        Name = attributeCode.Replace("-", " ")
                    },
                    DataType = "String",
                    InputType = "TextBox",
                    IsExtra = false,
                    IsOption = false,
                    IsProperty = true,
                    LocalizedContent = null,
                    MasterCatalogId = _apiContext.MasterCatalogId,
                    Namespace = "tenant",
                    SearchSettings = new AttributeSearchSettings()
                    {
                        SearchableInAdmin = true,
                        SearchableInStorefront = true,
                        SearchDisplayValue = true
                    },
                    ValueType = "AdminEntered",
                    VocabularyValues = null
                };

                var newAttribute = attributeHandler.AddAttribute(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, attribute).Result;
            };
        }

        [TestMethod]
        public void Get_ProductTypes()
        {
            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;
        }

        [TestMethod]
        public void Get_Purse_ProductType()
        {
            var filter = "name eq " + "'Purse'";

            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, filter).Result;
        }

        [TestMethod]
        public void Add_Sunglasses_ProductType()
        {
            var productTypeName = "Sunglasses";
            var filter = string.Format("name eq '{0}'", productTypeName);

            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;

            var existingProductType = productTypes.SingleOrDefault(a => a.Name == productTypeName);

            if (existingProductType == null)
            {
                var productType = new ProductType()
                {
                    Name = productTypeName,
                    GoodsType = "Physical",
                    MasterCatalogId = _apiContext.MasterCatalogId,
                    Options = new System.Collections.Generic.List<AttributeInProductType>(),
                    Properties = new System.Collections.Generic.List<AttributeInProductType>(),
                    Extras = null,
                    IsBaseProductType = false,
                    ProductUsages = new System.Collections.Generic.List<string>
                     {
                        "Standard",
                        "Configurable",
                        "Bundle",
                        "Component"
                     },
                };

                var newProductType = productTypeHandler.AddProductType(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, productType).Result;
            }
        }

        [TestMethod]
        public void Update_Sunglasses_Color_Options_ProductType()
        {
            var productTypeName = "Sunglasses";
            var filter = string.Format("name eq '{0}'", productTypeName);

            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;

            var existingProductType = productTypes.SingleOrDefault(a => a.Name == productTypeName);

            if (existingProductType != null)
            {
                var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

                var attributeColor = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, 0, 1, null, "attributeFQN eq 'tenant~color'").Result.First();

                AttributeInProductType productTypeOptionColor = new AttributeInProductType()
                {
                    AttributeFQN = attributeColor.AttributeFQN,
                    IsInheritedFromBaseType = false,
                    AttributeDetail = attributeColor,
                    IsHiddenProperty = false,
                    IsMultiValueProperty = false,
                    IsRequiredByAdmin = false,
                    Order = 0,
                    VocabularyValues = new System.Collections.Generic.List<AttributeVocabularyValueInProductType>()
                };

                productTypeOptionColor.VocabularyValues.Add(new AttributeVocabularyValueInProductType()
                {
                    Value = "Black",
                    Order = 0,
                    VocabularyValueDetail = new AttributeVocabularyValue()
                    {
                        Content = new AttributeVocabularyValueLocalizedContent()
                        {
                            LocaleCode = "en-US",
                            StringValue = "Black"
                        },
                        Value = "Black",
                        ValueSequence = 10
                    }
                });

                productTypeOptionColor.VocabularyValues.Add(new AttributeVocabularyValueInProductType()
                {
                    Value = "Brown",
                    Order = 0,
                    VocabularyValueDetail = new AttributeVocabularyValue()
                    {
                        Content = new AttributeVocabularyValueLocalizedContent()
                        {
                            LocaleCode = "en-US",
                            StringValue = "Brown"
                        },
                        Value = "Brown",
                        ValueSequence = 11
                    }
                });

                existingProductType.Options.Add(productTypeOptionColor);

                var newProductType = productTypeHandler.UpdateProductType(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, existingProductType).Result;
            }
        }

        [TestMethod]
        public void Update_Sunglasses_Protection_Property_ProductType()
        {
            var productTypeName = "Sunglasses";
            var filter = string.Format("name eq '{0}'", productTypeName);

            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;

            var existingProductType = productTypes.SingleOrDefault(a => a.Name == productTypeName);

            if (existingProductType != null)
            {
                var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

                var attributeProtection = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, 0, 1, null, "attributeFQN eq 'tenant~sunglass-protection'").Result.First();

                AttributeInProductType productTypePropertyProtection = new AttributeInProductType()
                {
                    AttributeFQN = attributeProtection.AttributeFQN,
                    IsInheritedFromBaseType = false,
                    AttributeDetail = attributeProtection,
                    IsHiddenProperty = false,
                    IsMultiValueProperty = false,
                    IsRequiredByAdmin = false,
                    Order = 0
                };

                existingProductType.Properties.Add(productTypePropertyProtection);

                var newProductType = productTypeHandler.UpdateProductType(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, existingProductType).Result;
            }
        }

        [TestMethod]
        public void Update_Sunglasses_Style_Property_ProductType()
        {
            var productTypeName = "Sunglasses";
            var filter = string.Format("name eq '{0}'", productTypeName);

            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;

            var existingProductType = productTypes.SingleOrDefault(a => a.Name == productTypeName);

            if (existingProductType != null)
            {
                var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

                var attributeStyle = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, 0, 1, null, "attributeFQN eq 'tenant~sunglass-style'").Result.First();

                AttributeInProductType productTypePropertyStyle = new AttributeInProductType()
                {
                    AttributeFQN = attributeStyle.AttributeFQN,
                    IsInheritedFromBaseType = false,
                    AttributeDetail = attributeStyle,
                    IsHiddenProperty = false,
                    IsMultiValueProperty = true,
                    IsRequiredByAdmin = false,
                    Order = 0,
                    VocabularyValues = new System.Collections.Generic.List<AttributeVocabularyValueInProductType>()
                };

                var productTypePropertyStyleValues = "Pilot|Rectangle|Rimless";
                var seq = 30;

                foreach (var value in productTypePropertyStyleValues.Split('|'))
                {
                    productTypePropertyStyle.VocabularyValues.Add(new AttributeVocabularyValueInProductType()
                    {
                        Value = value,
                        Order = 0,
                        VocabularyValueDetail = new AttributeVocabularyValue()
                        {
                            Content = new AttributeVocabularyValueLocalizedContent()
                            {
                                LocaleCode = "en-US",
                                StringValue = value
                            },
                            Value = value,
                            ValueSequence = seq++
                        }
                    });
                }

                existingProductType.Properties.Add(productTypePropertyStyle);

                var newProductType = productTypeHandler.UpdateProductType(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, existingProductType).Result;
            }
        }

        [TestMethod]
        public void Get_Products()
        {
            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();

            var products = productHandler.GetProducts(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, null).Result;
        }

        [TestMethod]
        public void Get_Purse_Product()
        {
            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();

            var products = productHandler.GetProduct(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, "LUC-sun-002").Result;
        }

        [TestMethod]
        public void Add_Sunglasses_Product()
        {
            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, "name eq 'Sunglasses'").Result;

            var existingProductType = productTypes.SingleOrDefault(a => a.Name == "Sunglasses");

            if (existingProductType != null)
            {
                var productCode = "LUC-SUN-003";

                var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();
                var existingProduct = productHandler.GetProduct(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, productCode).Result;

                if (existingProduct == null)
                {
                    var product = new Product()
                    {
                        ProductCode = productCode,
                        ProductUsage = "Configurable",
                        FulfillmentTypesSupported = new System.Collections.Generic.List<string> { "DirectShip" },
                        MasterCatalogId = 1,//_apiContext.MasterCatalogId,
                        ProductTypeId = existingProductType.Id,
                        IsValidForProductType = true,
                        ProductInCatalogs = new System.Collections.Generic.List<ProductInCatalogInfo>
                        {
                            new ProductInCatalogInfo()
                            { 
                                CatalogId = 1,
                                IsActive = true,
                                IsContentOverridden = false,
                                 Content = new ProductLocalizedContent()
                                 {
                                     LocaleCode = "en-US",
                                     ProductName = "Commander Sunglasses", 
                                     ProductShortDescription = "This minimalistic design is a great fit for those seeking adventure.",
                                 },
                                IsPriceOverridden = false,
                                Price = new ProductPrice()
                                {
                                     Price = 685.00m,
                                     SalePrice = 615.00m
                                },
                                IsseoContentOverridden = false,
                                SeoContent = new ProductLocalizedSEOContent()
                                {
                                     LocaleCode = "en-US",
                                     MetaTagTitle = "Euro Commander Sunglasses",
                                     SeoFriendlyUrl = "euro-commander-sunglasses"
                                },
                            }
                        },
                        HasConfigurableOptions = true,
                        HasStandAloneOptions = false,
                        IsVariation = false,
                        IsTaxable = false,
                        InventoryInfo = new ProductInventoryInfo()
                        {
                            ManageStock = false
                        },
                        IsRecurring = false,
                        SupplierInfo = new ProductSupplierInfo()
                        {
                            Cost = new ProductCost()
                            {
                                Cost = 0m,
                                IsoCurrencyCode = "USD"
                            }
                        },
                        IsPackagedStandAlone = false,
                        StandAlonePackageType = "CUSTOM",
                        PublishingInfo = new ProductPublishingInfo()
                        {
                            PublishedState = "Live"
                        },
                        Content = new ProductLocalizedContent()
                        {
                            LocaleCode = "en-US",
                            ProductShortDescription = "This minimalistic design is a great fit for those seeking adventure.",
                            ProductName = "Commander Sunglasses",
                        },
                        SeoContent = new ProductLocalizedSEOContent()
                        {
                            LocaleCode = "en-US",
                            MetaTagTitle = "Euro Commander Sunglasses",
                            SeoFriendlyUrl = "euro-commander-sunglasses"
                        },
                        Price = new ProductPrice()
                        {
                            Price = 685.00m,
                            SalePrice = 615.00m
                        },
                        PricingBehavior = new ProductPricingBehaviorInfo()
                        {
                            DiscountsRestricted = false
                        },
                        PackageWeight = new Mozu.Api.Contracts.Core.Measurement()
                        {
                            Unit = "lbs",
                            Value = .5m
                        },
                        PackageLength = new Mozu.Api.Contracts.Core.Measurement()
                        {
                            Unit = "in",
                            Value = 3.75m
                        },
                        PackageWidth = new Mozu.Api.Contracts.Core.Measurement()
                        {
                            Unit = "in",
                            Value = 5.5m
                        },
                        PackageHeight = new Mozu.Api.Contracts.Core.Measurement()
                        {
                            Unit = "in",
                            Value = 1.85m
                        }
                    };

                    var newProduct = productHandler.AddProduct(_apiContext.TenantId, _apiContext.SiteId,
                        _apiContext.MasterCatalogId, product).Result;
                }
            }
        }

        [TestMethod]
        public void Update_Sunglasses_Properties_Product()
        {
            var productCode = "LUC-SUN-001";

            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();
            var existingProduct = productHandler.GetProduct(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, productCode).Result;

            if (existingProduct != null)
            {
                existingProduct.Properties = new System.Collections.Generic.List<ProductProperty>();

                existingProduct.Properties.Add(new ProductProperty()
                {
                    AttributeFQN = "tenant~product-crosssell",
                    Values = new System.Collections.Generic.List<ProductPropertyValue>() 
                      {
                          new ProductPropertyValue()
                          {
                               Value = "LUC-SCF-001",
                                Content = new ProductPropertyValueLocalizedContent()
                                {
                                     LocaleCode = "en-US",
                                     StringValue = "LUC-SCF-001"
                                },
                          }
                      }
                });

                existingProduct.Properties.Add(new ProductProperty()
                {
                    AttributeFQN = "tenant~sunglass-protection",
                    Values = new System.Collections.Generic.List<ProductPropertyValue>() 
                      {
                          new ProductPropertyValue()
                          {
                               Value = "90% UV Protected",
                                Content = new ProductPropertyValueLocalizedContent()
                                {
                                     LocaleCode = "en-US",
                                     StringValue = "90% UV Protected"
                                },
                          }
                      }
                });

                existingProduct.Properties.Add(new ProductProperty()
                {
                    AttributeFQN = "tenant~sunglass-style",
                    Values = new System.Collections.Generic.List<ProductPropertyValue>() 
                      {
                          new ProductPropertyValue()
                          {
                               Value = "Pilot",
                                AttributeVocabularyValueDetail = new AttributeVocabularyValue()
                                {
                                    Content = new AttributeVocabularyValueLocalizedContent()
                                    {
                                         LocaleCode = "en-US",
                                         StringValue = "Pilot"
                                    },                                                                   
                                },
                          },
                          new ProductPropertyValue()
                          {
                               Value = "Rectangle",
                                AttributeVocabularyValueDetail = new AttributeVocabularyValue()
                                {
                                    Content = new AttributeVocabularyValueLocalizedContent()
                                    {
                                         LocaleCode = "en-US",
                                         StringValue = "Rectangle"
                                    },                                                                   
                                },
                          }
                      }
                });

                var newProduct = productHandler.UpdateProduct(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, existingProduct).Result;
            }
        }

        [TestMethod]
        public void Update_Sunglasses_Options_Product()
        {
            var productCode = "LUC-SUN-001";

            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();
            var existingProduct = productHandler.GetProduct(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, productCode).Result;

            if (existingProduct != null)
            {
                existingProduct.Options = new System.Collections.Generic.List<ProductOption>();

                existingProduct.Options.Add(new ProductOption()
                {
                    AttributeFQN = "tenant~color",
                    Values = new System.Collections.Generic.List<ProductOptionValue>() 
                    {
                        new ProductOptionValue()
                        {
                             Value = "Black",
                             AttributeVocabularyValueDetail = new AttributeVocabularyValue()
                             {
                                   Value = "Black",
                                    Content = new AttributeVocabularyValueLocalizedContent()
                                    {
                                         LocaleCode = "en-US",
                                         StringValue = "Black"
                                    }, 
                              }
                        },                        
                        new ProductOptionValue()
                        {
                             Value = "Brown",
                             AttributeVocabularyValueDetail = new AttributeVocabularyValue()
                             {
                                   Value = "Brown",
                                    Content = new AttributeVocabularyValueLocalizedContent()
                                    {
                                         LocaleCode = "en-US",
                                         StringValue = "Brown"
                                    }, 
                              }
                        }
                    }
                });

                var newProduct = productHandler.UpdateProduct(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, existingProduct).Result;
            }
        }

        [TestMethod]
        public void Add_Shopper()
        {
            var customerHandler = new MozuDataConnector.Domain.Handlers.CustomerHandler(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId);

            var filter = "ExternalId eq " + "'m|00449'";

            var account = customerHandler.GetCustomerAccounts(0,1,null, filter).Result;

            if (account.Count() == 0)
            {
                var customerAccountAndAuthInfo = new Mozu.Api.Contracts.Customer.CustomerAccountAndAuthInfo()
                {
                    Account = new Mozu.Api.Contracts.Customer.CustomerAccount() 
                    {
                         AcceptsMarketing = false,
                         CompanyOrOrganization = "Candles Unlimited Inc.",
                         EmailAddress = "alice.wick6@mozu.com",
                         ExternalId = "m|00449",
                         FirstName = "Alice", 
                         LastName = "Wick", 
                         IsActive = true,
                         IsAnonymous = false,
                         LocaleCode = "en-US",
                         TaxExempt = false, 
                         IsLocked = false,
                         UserName = "alice.wick6",
                         Contacts = new System.Collections.Generic
                             .List<Mozu.Api.Contracts.Customer.CustomerContact>() 
                             {
                                 new Mozu.Api.Contracts.Customer.CustomerContact()
                                 {
                                      Email = "alice.wick6@mozu.com",
                                      FirstName = "Alice", 
                                      LastNameOrSurname = "Wick",
                                      Label = "Mrs.",
                                      PhoneNumbers = new Mozu.Api.Contracts.Core.Phone()
                                      { 
                                        Mobile = "555-555-0001"
                                      },
                                      Address = new Mozu.Api.Contracts.Core.Address()
                                      {
                                            Address1 = "One Lightning Bug Way",
                                            AddressType = "Residentail",
                                            CityOrTown = "Austin",
                                            CountryCode = "US",
                                            PostalOrZipCode = "78702",
                                            StateOrProvince = "TX"
                                      },
                                       Types = new System.Collections.Generic
                                           .List<Mozu.Api.Contracts.Customer.ContactType>()
                                           {
                                               new Mozu.Api.Contracts.Customer.ContactType()
                                               {
                                                    IsPrimary = true,
                                                     Name = "Billing"
                                               }
                                           }
                                 },
                                 new Mozu.Api.Contracts.Customer.CustomerContact()
                                 {
                                      Email = "paul.wick@mozu.com",
                                      FirstName = "Paul", 
                                      LastNameOrSurname = "Wick",
                                      Label = "Mr.",
                                      PhoneNumbers = new Mozu.Api.Contracts.Core.Phone()
                                      { 
                                        Mobile = "555-555-0002"
                                      },
                                      Address = new Mozu.Api.Contracts.Core.Address()
                                      {
                                            Address1 = "1300 Comanche Trail",
                                            AddressType = "Residentail",
                                            CityOrTown = "San Marcos",
                                            CountryCode = "US",
                                            PostalOrZipCode = "78666",
                                            StateOrProvince = "TX"
                                      },
                                       Types = new System.Collections.Generic
                                           .List<Mozu.Api.Contracts.Customer.ContactType>()
                                           {
                                               new Mozu.Api.Contracts.Customer.ContactType()
                                               {
                                                    IsPrimary = true,
                                                     Name = "Shipping"
                                               }
                                           }
                                 },

                             }
                    },
                    Password = "16!Candles", 
                    IsImport = true
                };

                var credit = new Mozu.Api.Contracts.Customer.Credit.Credit()
                {
                    Code = "credit0001",
                    ActivationDate = System.DateTime.Now,
                    CreditType = "StoreCredit",
                    CurrencyCode = "USD",
                    CurrentBalance = 50m,
                    ExpirationDate = null,
                    InitialBalance = 50m
                };

                var wishList = new Mozu.Api.Contracts.CommerceRuntime.Wishlists.Wishlist()
                {
                    IsImport = true,
                    Name = "wishlist-001",
                    Items = new List<Mozu.Api.Contracts.CommerceRuntime.Wishlists.WishlistItem>() 
                        {
                            new Mozu.Api.Contracts.CommerceRuntime.Wishlists.WishlistItem()
                            {
                                 Product = new Mozu.Api.Contracts.CommerceRuntime.Products.Product()
                                 {
                                      ProductCode = "LUC-SCF-001"
                                 }
                            }
                        }
                };

                var newAccount = customerHandler.AddCustomerAccount(customerAccountAndAuthInfo, 
                    credit, 
                    wishList).Result;
            }
        }

        [TestMethod]
        public void Get_Order_CreateAuth()
        {
            var orderHandler = new MozuDataConnector.Domain.Handlers.OrderHandler();

            var orders = orderHandler.GetOrders(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, "OrderNumber eq '21'").Result;

            var existingOrder = orders.FirstOrDefault(d=> d.OrderNumber == 21);

            var action = new Mozu.Api.Contracts.CommerceRuntime.Payments.PaymentAction()
            {
                ActionName = "AuthorizePayment",
                //ActionName = "CreatePayment",
                Amount = 5.55m,
                CurrencyCode = "USD",
                InteractionDate = DateTime.Now,
                NewBillingInfo = new Mozu.Api.Contracts.CommerceRuntime.Payments.BillingInfo()
                {
                    AuditInfo = existingOrder.Payments[0].AuditInfo,
                    BillingContact = existingOrder.Payments[0].BillingInfo.BillingContact,
                    Card = existingOrder.Payments[0].BillingInfo.Card,
                    IsSameBillingShippingAddress = existingOrder.Payments[0].BillingInfo.IsSameBillingShippingAddress,
                    PaymentType = existingOrder.Payments[0].BillingInfo.PaymentType,
                    StoreCreditCode = existingOrder.Payments[0].BillingInfo.StoreCreditCode
                }
            };

            var paymentOrder = orderHandler.CreatePaymentAction(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, action, existingOrder).Result;


        }

    }
}