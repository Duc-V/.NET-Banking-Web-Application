# Assignment2_Group4

Student IDs:

- Zenabden Alshanan (s3947359)
- Duc Vu (3952670)
- Manav Gadhoke (s3783375)

Link to repository: https://github.com/rmit-wdt-fs-2023/s3947359-3952670-s3783375-a2

Repository contains three projects, Admin API, Admin Website, Customer Website.
To run Admin Website(port 5100) you must first run Admin API(port 5000)

Admin API and Website project loosely based off Lecture 9 project files. AdminAPI contains the following endpoints:

1. `CustomerManager` API:

    - GetAll(): returns a list of all customers.
    - Get(int id): returns a single customer with a given id.
    - Update(int id, Customer customer): updates a customer with a given id.
    - Lock(int id): locks a customer with a given id.
    - Unlock(int id): unlocks a customer with a given id.

2. `BillPayManager` API:

    - GetAccounts(int id): returns a list of accounts for a given customer id.
    - GetBillPayTransactions(int accountNumber): returns a list of bill pay transactions for a given account number.
    - Block(int id): blocks a bill pay with a given id.
    - Unblock(int id): unblocks a bill pay with a given id.


Layer III - Database/Data
Added Columns: In the customer table the Islocked(bit, not null) and ProfilePicture(varbinary(max), null) columns were added.
The profilepicture contains the bits required to display the profile picture on the customerwebsite.
The islocked column was used in the admin api/website to lock a specified customer out of their account.


There are 2 appsettings.json, one in the AdminAPI project and one in the CustomerWebsite project which contain the database connection strings.
