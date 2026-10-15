namespace ctf_sandbox.tests.Core.Drivers.CTF.Dtos;

public record GetTeamResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public uint MemberCount { get; set; }
}
