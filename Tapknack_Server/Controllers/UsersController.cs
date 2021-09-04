﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tapknack_Server.Models;
using Tapknack_Server.Repositories;

namespace Tapknack_Server.Controllers
{

    [Route("api/users")]
    public class UsersController : CRUDApiController<User,UsersRepository>
    {
        [HttpPost]
        public new async Task<long> AddAsync([FromBody] User entity)
            => (await base.AddAsync(entity)).Id;
    }
}
