namespace FonoChampionHub.Infrastructure.Identity;

public static class SystemRoles
{
    public const string Administrator = "Administrador";
    public const string Attendant = "Atendente";
    public const string Therapist = "Terapeuta";

    public static IReadOnlyList<string> All { get; } =
        [Administrator, Attendant, Therapist];
}
