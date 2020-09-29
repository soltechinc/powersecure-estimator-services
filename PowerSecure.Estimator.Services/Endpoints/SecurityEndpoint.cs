﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.ActionResults;
using PowerSecure.Estimator.Services.Repositories;
using PowerSecure.Estimator.Services.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Endpoints
{
    public static class SecurityEndpoint
    {
        [FunctionName("ListUsers")]
        public static async Task<IActionResult> ListUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/users")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListUsers");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosAuthorizedUserRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("GetUser")]
        public static async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/users/{id}")] HttpRequest req,
            string id,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetUser (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosAuthorizedUserRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditUser")]
        public static async Task<IActionResult> UpsertUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/users")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditUser");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new SecurityService(new CosmosAuthorizedUserRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("ListRoles")]
        public static async Task<IActionResult> ListRoles(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/roles")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log)
        {
            try
            {
                log.LogDebug("Function called - ListRoles");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosRoleRepository(dbClient)).List(queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("GetRole")]
        public static async Task<IActionResult> GetRole(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/roles/{id}")] HttpRequest req,
        string id,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetRole (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosRoleRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("EditRole")]
        public static async Task<IActionResult> UpsertRole(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/roles")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
            ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditRole");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new SecurityService(new CosmosRoleRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }

        [FunctionName("GetPermissions")]
        public static async Task<IActionResult> GetPermissions(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "security/permissions/{id}")] HttpRequest req,
        string id,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log)
        {
            try
            {
                log.LogDebug($"Function called - GetPermission (Id: {id})");

                var queryParams = req.GetQueryParameterDictionary();

                (object returnValue, string message) = await new SecurityService(new CosmosPermissionsRepository(dbClient)).Get(id, queryParams);
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }


        [FunctionName("EditPermissions")]
        public static async Task<IActionResult> UpsertPermissions(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "security/permissions")] HttpRequest req,
        [CosmosDB(ConnectionStringSetting = "dbConnection")] DocumentClient dbClient,
        ILogger log)
        {
            try
            {
                log.LogDebug("Function called - EditPermission");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                (object returnValue, string message) = await new SecurityService(new CosmosPermissionsRepository(dbClient)).Upsert(JObject.Parse(requestBody));
                return returnValue.ToOkObjectResult(message: message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Caught exception");
                return new object().ToServerErrorObjectResult();
            }
        }
    }
}
