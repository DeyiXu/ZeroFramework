namespace Zero.Domain.Entities.Auditing
{
    public interface ICreationAudited : IHasCreationTime
    {
        long? CreatorUserId { get; set; }
    }
}
