﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryCupcake.SharedLibrary.DTOs
{
    public class LoginDTO
    {
        [Required, EmailAddress, DataType(DataType.EmailAddress)]
        public string? Email {  get; set; }
        [Required, DataType(DataType.Password)]
        public string? Senha { get; set; }
    }
}
