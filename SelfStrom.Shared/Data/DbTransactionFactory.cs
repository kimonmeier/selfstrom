using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace SelfStrom.Shared.Data;

public class DbTransactionFactory 
{
    private readonly DbContext _context;
    private readonly ILogger _logger;

    public DbTransactionFactory(ILogger<DbTransaction> logger, DbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public DbTransactionFactory(ILogger logger, DbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public DbTransaction CreateTransaction() { 
        return new DbTransaction(_logger, _context);
    }
}
