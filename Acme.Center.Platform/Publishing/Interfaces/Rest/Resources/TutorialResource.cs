namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;

public record TutorialResource(int Id, string Title, string Summary, CategoryResource Category, string Status);