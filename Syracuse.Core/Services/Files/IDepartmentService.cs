﻿using Syracuse.Mobitheque.Core.Models;
using System.Threading.Tasks;

namespace Syracuse.Mobitheque.Core.Services.Files
{
    public interface IDepartmentService
    {
        Task<Library> GetLibraries(string url, bool refresh = false);
    }
}
