using CarInsurance.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace CarInsurance.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lione\OneDrive\Desktop\CarInsurance\CarInsurance\App_Data\Insurance.mdf;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateQuote(string firstName, string lastName, string email, string dateOfBirth, string carYear, string carMake,
           string carModel, bool duiCheck, string speedingTickets, bool coverageType)
        {
            var date = Convert.ToDateTime(dateOfBirth);

            string dui = duiCheck.ToString();
            if (dui == "True") { dui = "Yes"; }
            else if (dui == "False") { dui = "No"; }

            var tix = Convert.ToInt32(speedingTickets);

            string covType = coverageType.ToString();
            if (covType == "True") { covType = "Full Coverage"; }
            else if (covType == "False") { covType = "Liability"; }

            double quote = 50;
            DateTime d1 = DateTime.Now;
            TimeSpan difference = d1.Subtract(date);
            var stringAge = (difference.TotalDays / 365.25).ToString();
            var age = Convert.ToDouble(stringAge);

            if (age < 25 && age >= 18) { quote = (quote + 25); }
            else if (age < 18) { quote = (quote + 100); }
            else if (age > 100) { quote = (quote + 25); }

            var year = Convert.ToInt32(carYear);
            if (year < 2000 || year > 2015) { quote = (quote + 25); }

            if (carMake.ToLower() == "porsche") { quote = (quote + 25); }

            if (carMake.ToLower() == "porsche" && carModel.ToLower() == "911 carrera") { quote = (quote + 25); }

            quote = quote + (tix * 10);

            if (dui == "Yes") { quote = (quote * 1.25); }

            if (covType == "Full Coverage") { quote = (quote * 1.5); }

            decimal Quote = Convert.ToDecimal(quote);

            string queryString = @"INSERT INTO Insurees (firstName, lastName, emailAddress, dateOfBirth, carYear, carMake, carModel,
                                    DUI, speedingTickets, coverageType, quote) VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @CarYear, @CarMake,
                                    @CarModel, @Dui, @SpeedingTickets, @CovType, @Quote)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                command.Parameters.Add("@LastName", SqlDbType.VarChar);
                command.Parameters.Add("@Email", SqlDbType.VarChar);
                command.Parameters.Add("@DateOfBirth", SqlDbType.Date);
                command.Parameters.Add("@CarYear", SqlDbType.VarChar);
                command.Parameters.Add("@CarMake", SqlDbType.VarChar);
                command.Parameters.Add("@CarModel", SqlDbType.VarChar);
                command.Parameters.Add("@Dui", SqlDbType.VarChar);
                command.Parameters.Add("@SpeedingTickets", SqlDbType.Int);
                command.Parameters.Add("@CovType", SqlDbType.VarChar);
                command.Parameters.Add("@Quote", SqlDbType.Money);

                command.Parameters["@FirstName"].Value = firstName;
                command.Parameters["@LastName"].Value = lastName;
                command.Parameters["@Email"].Value = email;
                command.Parameters["@DateOfBirth"].Value = date;
                command.Parameters["@CarYear"].Value = carYear;
                command.Parameters["@CarMake"].Value = carMake;
                command.Parameters["@CarModel"].Value = carModel;
                command.Parameters["@Dui"].Value = dui;
                command.Parameters["@SpeedingTickets"].Value = tix;
                command.Parameters["@CovType"].Value = covType;
                command.Parameters["@Quote"].Value = Quote;

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            string queryStringQuote = @"SELECT Id, firstName, lastName, coverageType, quote FROM Insurees WHERE Id=(SELECT max(Id) FROM Insurees)";
            var newCustomer = new Insuree();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryStringQuote, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    newCustomer.Id = Convert.ToInt32(reader["Id"]);
                    newCustomer.FirstName= reader["firstName"].ToString();
                    newCustomer.LastName = reader["lastName"].ToString();
                    newCustomer.CoverageType = Convert.ToBoolean(reader["coverageType"]);
                    newCustomer.Quote = Convert.ToDecimal(reader["quote"]);
                }
            }
            return View(newCustomer);
        }
        public ActionResult Admin()
        {
            string queryString = @"SELECT Id, firstName, lastName, emailAddress, dateOfBirth, carYear, carMake, carModel,
                                    DUI, speedingTickets, coverageType, quote FROM Insurees";
            List<Insuree> customers = new List<Insuree>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var customer = new Insuree();
                    customer.Id = Convert.ToInt32(reader["Id"]);
                    customer.FirstName = reader["firstName"].ToString();
                    customer.LastName = reader["lastName"].ToString();
                    customer.EmailAddress = reader["emailAddress"].ToString();
                    customer.Quote = Convert.ToDecimal(reader["quote"]);
                    customers.Add(customer);
                }
            }
            return View(customers);
        }


    }
}