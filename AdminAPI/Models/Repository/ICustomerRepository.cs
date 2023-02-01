namespace AdminAPI.Models.Repository;

public interface ICustomerRepository
{
    // return all customers
    IEnumerable<Customer> GetAll();

    // get individual customer from their id
    Customer Get(int id);

    // take in id and update respective customer.
    int Update(int id, Customer customer);

    // lock customer
    void Lock(int id);
    // unlock customer
    void Unlock(int id);
}


