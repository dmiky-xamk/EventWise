﻿using Microsoft.EntityFrameworkCore;

namespace EventWise.Api;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
}