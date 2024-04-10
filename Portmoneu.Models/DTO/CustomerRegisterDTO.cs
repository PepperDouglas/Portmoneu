using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.DTO
{
    //CustomerRegisterDTO is for registering a completely new customer,
    //who will also get a User Identity (login)
    //To connect a new login to an existing Customer,
    //use UserRegisterDTO

    //This record shall contain data to both register a new Customer,
    //as well as an Alias (name) and Password for User Identity (login) 
    public record CustomerRegisterDTO(
            string Alias,
            string Password,
            string Gender,
            string Givenname,
            string Surname,
            string Streetaddress,
            string City,
            string Zipcode,
            string Country,
            string CountryCode
        );
}
