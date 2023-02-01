# Assignment2_Group4
Student ID's:
Zenabden Alshanan s3947359
Duc Vu 3952670 
Manav Gadhoke s3783375

Link to repository: https://github.com/rmit-wdt-fs-2023/s3947359-3952670-s3783375-a2

Repository contains three projects, Admin API, Admin Website, Customer Website. 
To run Admin Website(port 5100) you must first run Admin API(port 5000)

Admin API and Website project loosely based off Lecture 9 project files.

Layer III - Database/Data
Added Columns: In the customer table the Islocked(bit, not null) and ProfilePicture(varbinary(max), null) columns were added.
The profilepicture contains the bits required to display the profile picture on the customerwebsite.
The islocked column was used in the admin api/website to lock a specified customer out of their account.


