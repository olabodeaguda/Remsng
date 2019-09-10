﻿using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IStateRepository
    {
        Task<List<StateModel>> All();
        Task<StateModel> ByLcda(Guid lcdaId);
    }
}
