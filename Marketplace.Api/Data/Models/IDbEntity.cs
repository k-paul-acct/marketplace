namespace Marketplace.Api.Data.Models;

public interface IDbEntity<TId> where TId : IComparable
{
    public TId Id { get; set; }
}