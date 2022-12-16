using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardsGame.DBconn
{
    public class DB
    {
        private const string ConnString = "Server=127.0.0.1;" + 
                                          "Username=postgres;" + 
                                          "Database=MonterTradingCardGame;" + 
                                          "Port=5432;" + 
                                          "Password=bruhchungus;"+
                                          "SSLMode=Prefer";

        public NpgsqlConnection? Conn;



        public void Connect()
        {
            try
            {
                Conn = new NpgsqlConnection(ConnString);
                Console.Out.WriteLine("Opening connection");
                Conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Disconnect()
        {
            Conn.Close();
        }

        public void RegisterUser(string username, string password)
        {
            Connect();
            using var command = new NpgsqlCommand("INSERT INTO public.users(u_id, username, pw, coins, elo) " +
                                                                           "VALUES(default, @user, @pw, 20, 100); ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", password);
            try
            {
                var worked = command.ExecuteNonQuery();
                Console.WriteLine(worked == 1 ? "User has been added" : "Problem occurred adding user!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.Split(Environment.NewLine)[0]);
                //throw DuplicateNameException();
            }
            Disconnect();
        }

        public void LoginUser(string username, string password)
        {
            Connect();
            using var command = new NpgsqlCommand("SELECT username, pw " +
                                                         "FROM users " +
                                                         "WHERE username = @user " +
                                                           "AND pw = @pw; ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", password);
            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine($"Welcome to MTCG, {username}!");
                }
                else
                {
                    Console.WriteLine($"Login failed!");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }
        
        public void UpdateUser(string username, string password, string bio, string img)
        {
            Connect();
            using var command = new NpgsqlCommand("UPDATE public.users " +
                                                         "SET username = @user, pw = @pw, bio = @bio, image = @img " +
                                                         "WHERE username = @user; ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", password);
            command.Parameters.AddWithValue("@bio", bio);
            command.Parameters.AddWithValue("@img", img);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine($"Welcome to MTCG, {username}!");
                }
                else
                {
                    Console.WriteLine("Login failed!");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }

        public void CreateCard(string id, string name, int dmg, string cardType, string ElementType)
        {
            Connect();
            using var command = new NpgsqlCommand("INSERT INTO public.cards(name, damage, cardtype, elemtype, c_id) " +
                                                                           "VALUES(@name, @dmg, @ct, @el, @id)", Conn);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@dmg", dmg);
            command.Parameters.AddWithValue("@ct", cardType);
            command.Parameters.AddWithValue("@el", ElementType);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                var worked = command.ExecuteNonQuery();
                Console.WriteLine(worked == 1 ? "Card has been added" : "Problem occurred adding card!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }

        public void UserStats(string username)
        {
            Connect();
            using var command = new NpgsqlCommand("Select elo from users where username = @user;", Conn);
            command.Parameters.AddWithValue("@user", username);

            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{username} has {reader.GetInt32(0)} points");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }
        public void UpdateUserStats(string username, int points)
        {
            Connect();
            using var command = new NpgsqlCommand("UPDATE users SET elo = (Select elo from users where username = @user) + @pts WHERE username = @user; ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pts", points);

            try
            {
                var worked = command.ExecuteNonQuery();
                Console.WriteLine(worked == 1 ? "ELO has been changed" : "Problem occurred while changing ELO!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }

        public void Scoreboard()
        {
            Connect();
            using var command = new NpgsqlCommand("Select username, elo from users;", Conn);
            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetString(0)}: {reader.GetInt32(1)}");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }
        
        public void SelectAllUsers()
        {
            Connect();
            using var command = new NpgsqlCommand("Select * from users;", Conn);
            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"Reading from user id: {reader.GetInt32(0)}, {reader.GetString(1)}");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw DuplicateNameException();
            }
            Disconnect();
        }
    }
}