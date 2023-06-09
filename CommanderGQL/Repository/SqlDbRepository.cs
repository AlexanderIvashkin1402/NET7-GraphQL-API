﻿using CommanderGQL.Data;
using CommanderGQL.Models;
using Microsoft.EntityFrameworkCore;

namespace CommanderGQL.Repository;

public class SqlDbRepository : IRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SqlDbRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public IEnumerable<Command> GetCommandsByPlatform(int platformId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Commands.Where(x => x.PlatformId == platformId).ToList();
    }

    public IEnumerable<Platform> GetPlatforms()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Platforms.Include(x => x.Commands).ToList();
    }
    public IEnumerable<Command> GetCommands()
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Commands.Include(x => x.Platform).ToList();
    }

    public Platform GetPlatform(int platformId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Platforms.Include(x => x.Commands).SingleOrDefault(x => x.Id == platformId);
    }

    public async Task<ILookup<int, Command>> GetCommandsByPlatformAsync(IEnumerable<int> platformIds)
    {
        using var context = _contextFactory.CreateDbContext();
        var commands = await context.Commands.Where(x => platformIds.Contains(x.PlatformId)).ToListAsync();
        return commands.ToLookup(c => c.PlatformId);
    }

    public async Task<Platform> AddPlatformAsync(Platform platform)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();
        return platform;
    }

    public async Task<Command> AddCommandAsync(Command command)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Commands.Add(command);
        await context.SaveChangesAsync();
        return command;
    }
}
