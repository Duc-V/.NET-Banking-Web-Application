﻿using Microsoft.EntityFrameworkCore;
using Assignment2.Models;
namespace Assignment2.Data;

public class McbaContext : DbContext
{
    public McbaContext(DbContextOptions<McbaContext> options) : base(options){ 
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<BillPay> BillPay { get; set; }
    public DbSet<Payee> Payee { get; set; }
    public DbSet<Transaction> Transactions { get; set; } 
}
