﻿namespace MyApp.DTOs.Users
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool? IsActive { get; set; }
    }
}
