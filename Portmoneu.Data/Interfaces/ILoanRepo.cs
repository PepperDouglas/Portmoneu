﻿using Portmoneu.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Data.Interfaces
{
    public interface ILoanRepo
    {
        Task RegisterLoan(Loan loan);
    }
}