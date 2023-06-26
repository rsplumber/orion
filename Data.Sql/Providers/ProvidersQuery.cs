﻿using Microsoft.EntityFrameworkCore;
using Queries.Providers;

namespace Data.Sql.Providers;

internal sealed class ProvidersQuery : IProvidersQuery
{
    private readonly OrionDbContext _dbContext;

    public ProvidersQuery(OrionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ProviderResponse>> QueryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Providers
            .Select(provider => new ProviderResponse
            {
                Name = provider.Name,
                Status = provider.Status.ToString(),
                Metas = provider.Metas
            })
            .ToListAsync(cancellationToken);
    }
}