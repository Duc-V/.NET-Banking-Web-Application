namespace AdminAPI.Models.Repository;

public interface ICustomerRepository
{
    // return all customers
    IEnumerable<Customer> GetAll();

    // get individual customer from their id
    Customer Get(int id);

    // take in id and update respective customer.
    void UpdateCustomer(Customer customer, int id);

}


