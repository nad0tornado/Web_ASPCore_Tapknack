﻿using _NadoMapper = NadoMapper.NadoMapper;
using NadoMapper.Enums;
using NadoMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tapknack_Server.Models;

namespace Tapknack_Tests.Contexts
{
    public class MockUserDbService : MockDBService<User>, IDbService
    {
        public MockUserDbService(List<User> mockUsers = null) : base(mockUsers) { }

        public Task<object> ExecuteScalarAsync(string command, CRUDType crudType, string parameterName, object parameterValue)
             => ExecuteScalarAsync(command, crudType, new Dictionary<string, object>() { { parameterName, parameterValue } });

        public new Task<object> ExecuteScalarAsync(string command, CRUDType crudType, IDictionary<string, object> parameters = null)
            => base.ExecuteScalarAsync(command, crudType, parameters);


        public new Task<long> ExecuteNonQueryAsync(string command, CRUDType crudType, IDictionary<string, object> parameters = null)
            => base.ExecuteNonQueryAsync(command, crudType, parameters);


        public new Task<IEnumerable<IDictionary<string, object>>> ExecuteReaderAsync(string command, string parameterName, object parameterValue)
            => ExecuteReaderAsync(command, new Dictionary<string, object>() { { parameterName, parameterValue } });

        public new Task<IEnumerable<IDictionary<string, object>>> ExecuteReaderAsync(string command, IDictionary<string, object> parameters = null)
        {
            if (command == $"GetUserByUsername")
            {
                if (!parameters.ContainsKey("username") || parameters["username"] == null)
                    throw new ArgumentException($"Parameter \"username\" is required in call to GetUserByUsername");

                string name = parameters["username"].ToString();
                var entity = Entities.FirstOrDefault(e => e.Username == name);

                if (entity == null)
                    return Task.Run(() => new List<Dictionary<string, object>>().Cast<IDictionary<string,object>>());

                var entityAsDictionary = _NadoMapper.ReflectPropsFromSingle(entity);
                var entityList = new List<IDictionary<string, object>>() { entityAsDictionary };

                return Task.Run(() => entityList.Cast<IDictionary<string, object>>());
            }
            else if (command == $"GetUserByEmail")
            {
                if (!parameters.ContainsKey("email") || parameters["email"] == null)
                    throw new ArgumentException($"Parameter \"email\" is required in call to GetUserByEmail");

                string email = parameters["email"].ToString();
                var entity = Entities.FirstOrDefault(e => e.Email == email);

                if (entity == null)
                    return Task.Run(() => new List<IDictionary<string, object>>().Cast<IDictionary<string, object>>());

                var entityAsDictionary = _NadoMapper.ReflectPropsFromSingle(entity);
                var entityList = new List<IDictionary<string, object>>() { entityAsDictionary };

                return Task.Run(() => entityList.Cast<IDictionary<string, object>>());
            }
            else if(command == $"SearchUserByUsernameEmail")
            {
                if (parameters.ContainsKey("username") && string.IsNullOrEmpty(parameters["username"]?.ToString()))
                    throw new ArgumentException($"Parameter \"username\" cannot be null or empty in call to SearchUserByUsernameEmail");

                if (parameters.ContainsKey("email") && string.IsNullOrEmpty(parameters["email"]?.ToString()))
                    throw new ArgumentException($"Parameter \"email\" cannot be null or empty in call to SearchUserByUsernameEmail");

                string username = parameters["username"]?.ToString() ?? string.Empty;
                string email = parameters["email"]?.ToString() ?? string.Empty;

                var entities = Entities.Where(e => 
                    (username != string.Empty && (e.Username.Contains(username) || e.Username.StartsWith(username) || e.Username.EndsWith(username))) ||
                    (email != string.Empty && (e.Email.Contains(email) || e.Email.StartsWith(email) || e.Email.EndsWith(email)))
                );

                if (!entities.Any())
                    return Task.Run(() => new List<IDictionary<string, object>>().Cast<IDictionary<string,object>>());

                var entitiesAsDictionary = entities.Select(entity => _NadoMapper.ReflectPropsFromSingle(entity));

                return Task.Run(() => entitiesAsDictionary);
            }
            else
                return base.ExecuteReaderAsync(command, parameters);
        }
    }
}
