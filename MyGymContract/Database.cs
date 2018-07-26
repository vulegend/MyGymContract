using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGymContract
{
    public class Database
    {
        private static Database _instance;
        public static Database Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Database();

                return _instance;
            }
        }

        class Config
        {
            public static string Server { get; set; } = "localhost";

            public static int Port { get; set; } = 3306;

            public static string Username { get; set; } = "root";

            public static string Password { get; set; } = "";

            public static string Database { get; set; } = "mygym";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(string.Format("Server={0};Port={1};Username={2};Password={3};Database={4};SslMode=none",
                    Config.Server, Config.Port, Config.Username, Config.Password, Config.Database));
        }

        public bool CanConnect()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.State == System.Data.ConnectionState.Open;
            }
        }

        public void InsertContract(ContractInformation contract)
        {
            //first insert owner
            using (var conn = GetConnection())
            {
                conn.Open();
                long ownerID = -1;
                long businessID = -1;

                using (var com = new MySqlCommand("INSERT INTO owners (name,surname,email,phone,google_id) VALUES (@name,@surname,@email,@phone,'NOT SET')", conn))
                {
                    com.Parameters.Add("@name", MySqlDbType.VarChar).Value = contract.GymOwnerInformation.Name;
                    com.Parameters.Add("@surname", MySqlDbType.VarChar).Value = contract.GymOwnerInformation.Surname;
                    com.Parameters.Add("@email", MySqlDbType.VarChar).Value = contract.GymOwnerInformation.Email;
                    com.Parameters.Add("@phone", MySqlDbType.VarChar).Value = contract.GymOwnerInformation.Mobile;

                    com.ExecuteNonQuery();
                    ownerID = com.LastInsertedId;
                }

                if(ownerID >= 0)
                {
                    using (var com = new MySqlCommand("INSERT INTO business (owner_id,name,address,city,pib,registry_number,dinarski,devizni) VALUES (@owner_id,@name,@address,@city,@pib,@registry_number,@dinarski,@devizni)", conn))
                    {
                        com.Parameters.Add("@owner_id", MySqlDbType.Int64).Value = ownerID;
                        com.Parameters.Add("@name", MySqlDbType.VarChar).Value = contract.GymInformation.Name;
                        com.Parameters.Add("@address", MySqlDbType.VarChar).Value = contract.GymInformation.Address;
                        com.Parameters.Add("@city", MySqlDbType.VarChar).Value = contract.GymInformation.City;
                        com.Parameters.Add("@pib", MySqlDbType.VarChar).Value = contract.GymInformation.PIB;
                        com.Parameters.Add("@registry_number", MySqlDbType.VarChar).Value = contract.GymInformation.RegistryNumber;
                        com.Parameters.Add("@dinarski", MySqlDbType.VarChar).Value = contract.GymInformation.Dinarski;
                        com.Parameters.Add("@devizni", MySqlDbType.VarChar).Value = contract.GymInformation.Devizni;

                        com.ExecuteNonQuery();
                        businessID = com.LastInsertedId;
                    }
                }

                if(businessID >= 0)
                {
                    using (var com = new MySqlCommand("INSERT INTO contracts (contract_length,contract_package,business_id, contract_number, date_signed) VALUES (@length,@package,@id,@number,now())", conn))
                    {
                        com.Parameters.Add("@length", MySqlDbType.Int32).Value = contract.ContractLength;
                        com.Parameters.Add("@package", MySqlDbType.Int32).Value = contract.ContractPackage;
                        com.Parameters.Add("@id", MySqlDbType.Int64).Value = businessID;
                        com.Parameters.Add("@number", MySqlDbType.VarChar).Value = contract.ContractNumber;


                        com.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteContract(ContractInformation contract)
        {

        }

        public void ModifyContract(ContractInformation contract)
        {

        }

        public List<ContractInformation> GetAllContracts()
        {
            List<ContractInformation> toReturn = new List<ContractInformation>();
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var com = new MySqlCommand("SELECT * FROM contracts INNER JOIN business ON contracts.business_id = business.business_id INNER JOIN owners ON business.owner_id = owners.owner_id", conn))
                {
                    using (var reader = com.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            BusinessInformation businessInfo = new BusinessInformation(reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14));
                            OwnerInformation ownerInfo = new OwnerInformation(reader.GetString(16), reader.GetString(17), reader.GetString(18), reader.GetString(19));
                            ContractInformation contractInfo = new ContractInformation(reader.GetInt32(1), reader.GetInt32(2), ownerInfo, businessInfo);
                            contractInfo.ContractNumber = reader.GetString(4);

                            if (!reader.IsDBNull(5))
                                contractInfo.DateSigned = reader.GetDateTime(5);
                            toReturn.Add(contractInfo);
                        }
                    }
                }
            }
            return toReturn;
        }
    }
}
