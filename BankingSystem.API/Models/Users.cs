﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.API.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [MaxLength(50)]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|gov|np|edu)$", ErrorMessage = "Invalid pattern.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public Roles UserType { get; set; }
        public DateTime DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Users(int userId, string username, string fullname, string email, string password, string address, Roles userType, DateTime dateOfBirth, DateTime createdAt)
        {
            UserId = userId;
            Username = username;
            Fullname = fullname;
            Email = email;
            Password = password;
            Address = address;
            UserType = userType;
            DateOfBirth = dateOfBirth;
            CreatedAt = createdAt;
        }

        public Users()
        {

        }
    }
}