using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database.Entities;

namespace TravelBlog.Database;

public static class DatabaseExtensions
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
    {
        return exception.InnerException is SqliteException ex && ex.SqliteErrorCode == 19 && ex.SqliteExtendedErrorCode == 2067;
    }

    public static string GetName(this Subscriber subscriber)
    {
        return subscriber.GivenName + ' ' + subscriber.FamilyName;
    }
}
