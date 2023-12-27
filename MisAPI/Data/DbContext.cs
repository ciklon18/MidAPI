using Microsoft.EntityFrameworkCore;

namespace MisAPI.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
        // если база данных не существует, то она будет создана
        // но сперва необходимо применить миграции затем попытаться найти существующую базу данных
        // если база данных не существует, то она будет создана

        try
        {
            Database.Migrate();
            
        }
        catch (Exception e)
        {
            
        }
    }
    
    
}