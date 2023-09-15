using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NanoidDotNet;

namespace EventWise.Api.Persistence;

/// <summary>
/// Custom value generator for creating URL-friendly IDs.
/// </summary>
public sealed class PublicIdValueGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry)
    {
        return Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 12);
    }
}