namespace Acme.Center.Platform.Publishing.Domain.Model;

public enum PublishingError
{
    CategoryNotFound,
    TutorialNotFound,
    DuplicateTutorialTitle,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}