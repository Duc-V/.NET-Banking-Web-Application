namespace AdminAPI.Models.Repository;

public interface ICustomerRepository<TEntity, TKey> where TEntity : class
{
    // return all customers
    IEnumerable<TEntity> GetAll();

    // get individual customer from their id
    Customer Get(int id);

    // take in id and update respective customer.
    void Update(TEntity entity, TKey id);


}
