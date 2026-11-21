namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;

public record CreateTutorialResource(string Title, string Summary, int CategoryId);