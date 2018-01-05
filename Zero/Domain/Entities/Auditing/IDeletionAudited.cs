namespace Zero.Domain.Entities.Auditing
{
    public interface IDeletionAudited : IHasDeletionTime
    {
        long? DeleterUserId { get; set; }
    }
}
