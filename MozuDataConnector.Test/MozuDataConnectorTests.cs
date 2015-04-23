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
        public void Get_Category()
        {
            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();

            var category = productHandler.GetCategory(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 1).Result;
        }

        [TestMethod]
        public void Get_Attribute_Popularity()
        {
            var filter = "attributeFQN eq " + "'tenant~popularity'";

            var attributeHandler = new MozuDataConnector.Domain.Handlers.AttributeHandler();

            var attributes = attributeHandler.GetAttributes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, filter).Result;
        }

        [TestMethod]
        public void Add_Attribute_SunGlass_Style()
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
        public void Update_Attribute_SunGlass_Style()
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
        public void Add_Attribute_SunGlass_Protection()
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
        public void Get_ProductType_Purse()
        {
            var filter = "name eq " + "'Purse'";

            var productTypeHandler = new MozuDataConnector.Domain.Handlers.ProductTypeHandler();

            var productTypes = productTypeHandler.GetProductTypes(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, filter).Result;
        }

        [TestMethod]
        public void Add_ProductType_Sunglasses()
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
        public void Update_ProductType_Sunglasses_Color_Options()
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
        public void Update_ProductType_Sunglasses_Protection_Property()
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
        public void Update_ProductType_Sunglasses_Style_Property()
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
        public void Get_Product_Purse()
        {
            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();

            var products = productHandler.GetProduct(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, "LUC-SUN-003").Result;
        }

        [TestMethod]
        public void Add_Product_Sunglasses()
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
        public void Update_Product_Sunglasses_Properties()
        {
            var productCode = "LUC-SUN-003";

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
        public void Update_Product_Sunglasses_Options()
        {
            var productCode = "LUC-SUN-003";

            var productHandler = new MozuDataConnector.Domain.Handlers.ProductHandler();
            var existingProduct = productHandler.GetProduct(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, productCode).Result;

            if (existingProduct != null)
            {
                existingProduct.Options = new System.Collections.Generic.List<ProductOption>();

                //existingProduct.Options.Add(new ProductOption()

                var productOption = new ProductOption()
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
                };

//                var newProduct = productHandler.UpdateProduct(_apiContext.TenantId, _apiContext.SiteId, _apiContext.MasterCatalogId, existingProduct).Result;

                var newProductOption = productHandler.AddProductOption(_apiContext.TenantId, _apiContext.SiteId,
                    _apiContext.MasterCatalogId, productOption, existingProduct.ProductCode).Result;

                var productVariations = new ProductVariationCollection();
                var productVariation = new ProductVariation() 
                { };

                var newVariantOption = productHandler.AddProductVariation(_apiContext.TenantId, _apiContext.SiteId, _apiContext.MasterCatalogId, 
                    productVariations, existingProduct.ProductCode);
                    
            
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

                var newAccount = customerHandler.AddCustomerAccount(customerAccountAndAuthInfo,                     credit, 
                    wishList).Result;
            }
        }

        [TestMethod]
        public void Add_Shopper_And_Login()
        {
            Mozu.Api.Resources.Commerce.Customer.CustomerAccountResource resource =
                new Mozu.Api.Resources.Commerce.Customer.CustomerAccountResource(_apiContext);

            Mozu.Api.Resources.Commerce.Customer.Accounts.CustomerContactResource cResource =
                new Mozu.Api.Resources.Commerce.Customer.Accounts.CustomerContactResource(_apiContext);

            var limit = 1000;

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"c:\temp\test-import-bf.txt", true))
            {
                var date = DateTime.Now.ToString();
                writer.WriteLine(date);
            }

            Parallel.For(0, limit, new ParallelOptions { MaxDegreeOfParallelism = 50 }, i =>
            {
            var guid = Guid.NewGuid().ToString("N");

                var customerAccountAndAuthInfo = new Mozu.Api.Contracts.Customer.CustomerAccountAndAuthInfo()
                {
                    Account = new Mozu.Api.Contracts.Customer.CustomerAccount()
                    {
                        AcceptsMarketing = false,
                        CompanyOrOrganization = "Candles Unlimited Inc.",
                        EmailAddress = guid + "@mozu.com",
                        ExternalId = guid,
                        FirstName = "FFirst-" + guid,
                        LastName = "SSecond-" + guid,
                        IsActive = true,
                        IsAnonymous = false,
                        LocaleCode = "en-US",
                        TaxExempt = false,
                        IsLocked = false,
                        UserName = guid,
                        Contacts = new System.Collections.Generic
                            .List<Mozu.Api.Contracts.Customer.CustomerContact>() 
                             {
                                 new Mozu.Api.Contracts.Customer.CustomerContact()
                                 {
                                      Email = guid + "@mozu.com",
                                      FirstName = "First-" + guid,
                                      LastNameOrSurname = "Second-" + guid,
                                      Label = "Mrs.",
                                      PhoneNumbers = new Mozu.Api.Contracts.Core.Phone()
                                      { 
                                        Mobile = "555-555-0001"
                                      },
                                      Address = new Mozu.Api.Contracts.Core.Address()
                                      {
                                            Address1 = "100 " + guid,
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
                             }
                    },
                    Password = guid + "!",
                    IsImport = true
                };

                var c = resource.AddAccountAndLoginAsync(customerAccountAndAuthInfo).Result;
                var cr = cResource.AddAccountContactAsync(customerAccountAndAuthInfo.Account.Contacts[0], c.CustomerAccount.Id).Result;
            });

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"c:\temp\test-import-bf.txt", true))
            {
                var date = DateTime.Now.ToString();
                writer.WriteLine(date);
            }
        }
        

        [TestMethod]
        public void Get_Order_CreateAuth()
        {
            var orderNumber = 18;
            var orderHandler = new MozuDataConnector.Domain.Handlers.OrderHandler();

            var orders = orderHandler.GetOrders(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 20, null, "OrderNumber eq '" + orderNumber + "'").Result;

            var existingOrder = orders.FirstOrDefault(d=> d.OrderNumber == orderNumber);

            var authorizedPayment = existingOrder.Payments.FirstOrDefault(d => d.Status == "Authorized");
    
            var totalAmountRequested = authorizedPayment.AmountRequested;
            var paymentServiceTransactionId = authorizedPayment.Interactions.FirstOrDefault(d => d != null).GatewayTransactionId;
            //var paymentServiceTransactionId = authorizedPayment.Id;
    
            var action = new Mozu.Api.Contracts.CommerceRuntime.Payments.PaymentAction()
            {
                Amount = totalAmountRequested,
                CurrencyCode = "USD",
                InteractionDate = DateTime.Now,
                NewBillingInfo = new Mozu.Api.Contracts.CommerceRuntime.Payments.BillingInfo()
                {
                    AuditInfo = authorizedPayment.AuditInfo,
                    BillingContact = authorizedPayment.BillingInfo.BillingContact,
                    Card = authorizedPayment.BillingInfo.Card,
                    IsSameBillingShippingAddress = authorizedPayment.BillingInfo.IsSameBillingShippingAddress,
                    PaymentType = authorizedPayment.BillingInfo.PaymentType,
                    StoreCreditCode = authorizedPayment.BillingInfo.StoreCreditCode
                },
                ReferenceSourcePaymentId = paymentServiceTransactionId
            };

            var voidAction = new Mozu.Api.Contracts.CommerceRuntime.Payments.PaymentAction();
            voidAction.ReferenceSourcePaymentId = paymentServiceTransactionId;


            voidAction.ActionName = "VoidPayment";
            var paymentOrder = orderHandler.PerformPaymentAction(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, voidAction, authorizedPayment, existingOrder.Id).Result;


            //AuthorizePayment Call
            action.ActionName = "AuthorizePayment";
            action.Amount = 250m;
            action.ReferenceSourcePaymentId = null;
            paymentOrder = orderHandler.CreatePaymentAction(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, action, existingOrder).Result;


            //AuthorizePayment Call
            action.Amount = 250m;
            action.ReferenceSourcePaymentId = null;
            action.ActionName = "AuthorizePayment";
            paymentOrder = orderHandler.CreatePaymentAction(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, action, existingOrder).Result;

        }

        [TestMethod]
        public void Get_Orders_With_ExternalIDs()
        {
            Mozu.Api.Resources.Commerce.OrderResource or = 
                new Mozu.Api.Resources.Commerce.OrderResource(_apiContext);

            var filter = "ExternalId ne null";

            var orders = or.GetOrdersAsync(0, 200, null, filter, null, null, null).Result;

            filter = "ExternalId eq null";

            orders = or.GetOrdersAsync(0, 200, null, filter, null, null, null).Result;

        }

        [TestMethod]
        public void Fulfill_Order_Packages()
        {
            var orderResource = new Mozu.Api.Resources.Commerce.OrderResource(_apiContext);
            var packageResource = new Mozu.Api.Resources.Commerce.Orders.PackageResource(_apiContext);
            var shipmentResource = new Mozu.Api.Resources.Commerce.Orders.ShipmentResource(_apiContext);
            var fulfillmentInfoResource = new Mozu.Api.Resources.Commerce.Orders.FulfillmentInfoResource(_apiContext);
            var fulfillmentActionResource = new Mozu.Api.Resources.Commerce.Orders.FulfillmentActionResource(_apiContext);

            var filter = string.Format("OrderNumber eq '{0}'", "26");
            var existingOrder = (orderResource.GetOrdersAsync(startIndex:0, pageSize:1, filter:filter).Result).Items[0];
            var existingOrderItems = existingOrder.Items;
 
            var packageItems = new List<Mozu.Api.Contracts.CommerceRuntime.Fulfillment.PackageItem>();
            foreach(var orderItem in existingOrderItems)
            {
                packageItems.Add(new Mozu.Api.Contracts.CommerceRuntime.Fulfillment.PackageItem()
                { 
                     ProductCode = orderItem.Product.ProductCode,
                      Quantity = orderItem.Quantity,
                      FulfillmentItemType = "Physical"
                });
            }

            var package = new Mozu.Api.Contracts.CommerceRuntime.Fulfillment.Package()
            {
                Measurements = new Mozu.Api.Contracts.CommerceRuntime.Commerce.PackageMeasurements()
                {
                    Height = new Mozu.Api.Contracts.Core.Measurement()
                    {
                        Unit = "in",
                        Value = 10m
                    },
                    Length = new Mozu.Api.Contracts.Core.Measurement()
                    {
                        Unit = "in",
                        Value = 10m
                    },
                    Width = new Mozu.Api.Contracts.Core.Measurement()
                    {
                        Unit = "in",
                        Value = 10m
                    },
                    Weight = new Mozu.Api.Contracts.Core.Measurement()
                    {
                        Unit = "lbs",
                        Value = 10m
                    },
                },
                Items = new List<Mozu.Api.Contracts.CommerceRuntime.Fulfillment.PackageItem>(),
                 PackagingType = "CUSTOM",
            };
            
            package.Items.AddRange(packageItems);

            var availableShippingMethods = shipmentResource.GetAvailableShipmentMethodsAsync(existingOrder.Id).Result;
            package.ShippingMethodCode = availableShippingMethods[0].ShippingMethodCode;
            package.ShippingMethodName = availableShippingMethods[0].ShippingMethodName;
            package.Code = "Package-01";
            var updatedPackage = packageResource.CreatePackageAsync(package, existingOrder.Id).Result;

            var packageIds = new List<string>() { updatedPackage.Id };
            //var updatedPackageShipment = shipmentResource.CreatePackageShipmentsAsync(packageIds, existingOrder.Id).Result;

            var fulfilledShipment = fulfillmentActionResource.PerformFulfillmentActionAsync(
                new Mozu.Api.Contracts.CommerceRuntime.Fulfillment.FulfillmentAction()
                {
                     ActionName = "Ship", // {Ship,Fulfill}
                      DigitalPackageIds = new List<string>(),
                      PackageIds = packageIds,
                      PickupIds = new List<string>()
                }, 
                existingOrder.Id)
                .Result;

            //var updatedOrder = orderResource.UpdateOrderAsync(existingOrder, existingOrder.Id).Result;
        }


        [TestMethod]
        public void Update_Order_Gift_Message()
        {
            var orderNumber = 25;

            var orderHandler = new MozuDataConnector.Domain.Handlers.OrderHandler();

            var orders = orderHandler.GetOrders(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, 0, 1, null, "OrderNumber eq '" + orderNumber + "'").Result;

            var existingOrder = orders.FirstOrDefault(d => d.OrderNumber == orderNumber);

            existingOrder.ExternalId = DateTime.Now.Ticks.ToString();

            if (existingOrder.ShopperNotes == null)
            {
                existingOrder.ShopperNotes = new Mozu.Api.Contracts.CommerceRuntime.Orders.ShopperNotes();
            }

            if (existingOrder.Notes == null)
            {
                existingOrder.Notes = new List<Mozu.Api.Contracts.CommerceRuntime.Orders.OrderNote>();
            }
            else
            {
                foreach(var note in existingOrder.Notes)
                {
                    orderHandler.DeleteOrderNote(_apiContext.TenantId, _apiContext.SiteId, _apiContext.MasterCatalogId,
                        note.Id, existingOrder.Id);
                }
            }
       
            existingOrder.ShopperNotes.GiftMessage = "new gift message via the SDK - " + DateTime.Now.Ticks;
            existingOrder.ShopperNotes.Comments = "comments gift message  via the SDK - " + DateTime.Now.Ticks;

            var orderNote = new Mozu.Api.Contracts.CommerceRuntime.Orders.OrderNote()
            {
                Text = "new note via the SDK - " + DateTime.Now.Ticks,
            };

            var updatedOrderNote = orderHandler.AddOrderNote(_apiContext.TenantId, _apiContext.SiteId, _apiContext.MasterCatalogId,
                orderNote, existingOrder.Id).Result;



            var updatedOrder = orderHandler.UpdateOrder(_apiContext.TenantId, _apiContext.SiteId, _apiContext.MasterCatalogId,
                    existingOrder).Result;

            /*

            if (existingOrder.Notes != null && existingOrder.Notes.Count > 0)
            {
                var orderAttributes = orderHandler.GetOrderAttribute(_apiContext.TenantId, 
                    _apiContext.SiteId, _apiContext.MasterCatalogId, existingOrder.Id).Result;

                var giftCardAttribute = orderAttributes.SingleOrDefault(d => d.FullyQualifiedName.ToLower() == "tenant~gift-message");

                if (giftCardAttribute == null)
                {
                    existingOrder.Attributes = new List<Mozu.Api.Contracts.CommerceRuntime.Orders.OrderAttribute>();
                    existingOrder.Attributes.Add(
                        new Mozu.Api.Contracts.CommerceRuntime.Orders.OrderAttribute()
                        {
                            FullyQualifiedName = "tenant~gift-message",
                        });

                    giftCardAttribute = existingOrder.Attributes.SingleOrDefault(d => d.FullyQualifiedName.ToLower() == "tenant~gift-message");
                }

                giftCardAttribute.Values = new List<object>();

                giftCardAttribute.Values.Add(existingOrder.Notes[0].Text);

                existingOrder.ShopperNotes = new Mozu.Api.Contracts.CommerceRuntime.Orders.ShopperNotes();

                existingOrder.ShopperNotes.GiftMessage = giftCardAttribute.Values[0].ToString();
                existingOrder.Notes[0] = null;

                var existingAttributes = orderHandler.AddOrderAttribute(_apiContext.TenantId, _apiContext.SiteId,
                _apiContext.MasterCatalogId, giftCardAttribute, existingOrder.Id).Result;

                //from comments To giftcard message
            */


            
            }
        
    }
}