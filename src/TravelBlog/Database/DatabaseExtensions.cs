using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Database
{
    public static class DatabaseExtensions
    {
        public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
        {
            return exception.InnerException is SqliteException ex && ex.SqliteErrorCode == 19 && ex.SqliteExtendedErrorCode == 2067;
        }
    }
}
