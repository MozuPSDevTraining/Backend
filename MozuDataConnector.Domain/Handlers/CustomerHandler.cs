﻿using Mozu.Api;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.Resources.Commerce.Customer.Accounts;
using Mozu.Api.Resources.Commerce.Customer.Credits;
using Mozu.Api.Resources.Commerce.Customer.Attributedefinition;
using Mozu.Api.Contracts.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MozuDataConnector.Domain.Handlers
{
    public class CustomerHandler
    { 
        private Mozu.Api.IApiContext _apiContext;

        public async Task<CustomerAccount> GetCustomerAccount(int tenantId, int? siteId,
            int? masterCatalogId, int accountId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var customerAccountResource = new CustomerAccountResource(_apiContext);
            var account = await customerAccountResource.GetAccountAsync(accountId);

            return account;
        }

        public async Task<IEnumerable<CustomerAccount>> GetCustomerAccounts(int tenantId, int? siteId,
            int? masterCatalogId, int? startIndex, int? pageSize, string sortBy = null, string filter = null)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var customerAccountResource = new CustomerAccountResource(_apiContext);
            var accounts = await customerAccountResource.GetAccountsAsync(startIndex, pageSize, sortBy, filter, null);

            return accounts.Items;
        }

        public async Task<CustomerAuthTicket> AddCustomerAccount(int tenantId, int? siteId,
            int? masterCatalogId, CustomerAccountAndAuthInfo account)
        {
            _apiContext = new ApiContext(tenantId, siteId);

            var customerAccountResource = new CustomerAccountResource(_apiContext);
            var newAccount = await customerAccountResource.AddAccountAndLoginAsync(account);
          
            return newAccount;
        }

        public async Task<CustomerAccount> UpdateCustomerAccount(int tenantId, int? siteId,
            int? masterCatalogId, CustomerAccount account)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var customerAccountResource = new CustomerAccountResource(_apiContext);
            var updatedAccount = await customerAccountResource.UpdateAccountAsync(account, 
                account.Id);

            return updatedAccount;
        }

        public async Task<CustomerContact> GetCustomerContact(int tenantId, int? siteId,
            int? masterCatalogId, int accountId, int contactId)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var customerContactResource = new CustomerContactResource(_apiContext);
            var contact = await customerContactResource.GetAccountContactAsync(accountId, contactId);

            return contact;
        }

        public async Task<IEnumerable<CustomerContact>> GetCustomerContacts(int accountId, int tenantId, 
            int? siteId, int? masterCatalogId, int? startIndex, int? pageSize, string sortBy = null, string filter = null)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var customerContactResource = new CustomerContactResource(_apiContext);
            var contacts = await customerContactResource.GetAccountContactsAsync(accountId, startIndex, 
                pageSize, sortBy, filter, null);

            return contacts.Items;
        }

        public async Task<CustomerContact> AddCustomerContact(int accountId, CustomerContact contact, 
            int tenantId, int? siteId, int? masterCatalogId)
        {
            _apiContext = new ApiContext(tenantId, siteId);

            var customerContactResource = new CustomerContactResource(_apiContext);
            var newContact = await customerContactResource.AddAccountContactAsync(contact, accountId);

            return newContact;
        }

        public async Task<CustomerContact> UpdateCustomerContact(int tenantId, int? siteId,
            int? masterCatalogId, CustomerContact contact)
        {
            _apiContext = new ApiContext(tenantId, siteId, masterCatalogId);

            var customerContactResource = new CustomerContactResource(_apiContext);
            var updatedcontact = await customerContactResource.UpdateAccountContactAsync(contact,
                contact.AccountId, contact.Id);

            return updatedcontact;
        }
    }
}