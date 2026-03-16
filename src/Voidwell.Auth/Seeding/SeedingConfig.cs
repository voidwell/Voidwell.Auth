namespace Voidwell.Auth.Seeding;

public sealed class SeedingConfig
{
    public bool TrySeeding { get; set; }

    public string AdminUserEmail { get; set; }

    public string AdminUserPassword { get; set; }

    public string AdminApiSecret { get; set; }
}