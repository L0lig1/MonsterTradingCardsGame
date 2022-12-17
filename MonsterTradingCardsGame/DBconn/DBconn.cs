using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardsGame.CardNamespace;
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

        public string RegisterUser(string username, string password)
        {
            using var command = new NpgsqlCommand("INSERT INTO public.users(u_id, username, pw, coins, elo) " +
                                                                           "VALUES(default, @user, @pw, 20, 100); ", Conn);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", password);
            try
            {
                var worked = command.ExecuteNonQuery();
                return worked == 1 ? "User has been added" : "Problem occurred adding user!";
            }
            catch (Exception e)
            {
                return e.Message.Split(Environment.NewLine)[0].ToString();
                //throw DuplicateNameException();
            }
        }

        public string LoginUser(string username, string password)
        {
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
                    reader.Close();
                    return $"Welcome to MTCG, {username}!";
                }
                reader.Close();
                return $"Login failed!";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
                //throw DuplicateNameException();
            }
        }
        
        // Fix this
        public string UpdateUser(string username, string name2, string bio, string img)
        {
            using var command = new NpgsqlCommand("UPDATE public.users SET bio='adgda', image='@img', second_name='adagklg' WHERE username='Me';", Conn);
            command.Parameters.AddWithValue("@name2", name2);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@bio", bio);
            command.Parameters.AddWithValue("@img", img);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    return "Update successfuö!";
                }

                reader.Close();
                return "Update failed!";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
                //throw DuplicateNameException();
            }
        }

        public string GetRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
        public string CreatePackage(dynamic cards)
        {
            var cardsList = new List<Card>();
            Card? newCard = null;
            bool failed  = false;
            foreach (var card in cards)
            {
                newCard = CreateCard(card.Id.ToString(), card.Name.ToString(), (int)card.Damage);
                failed = newCard == null;
                cardsList.Add(newCard);

                using var command = new NpgsqlCommand("INSERT INTO packages(p_id, card_id) " +
                                                                           "VALUES(@p_id, @c_id)", Conn);
                command.Parameters.AddWithValue("@p_id", GetRandomString());
                command.Parameters.AddWithValue("@c_id", card.Id.ToString());
                try
                {
                    var worked = command.ExecuteNonQuery();
                    failed = worked != 1;
                    if (failed)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Disconnect();
                    return null;
                    //throw DuplicateNameException();
                }
            }

            
            return new string("af");

        }

        public Card CreateCard(string id, string name, int dmg)
        {
            using var command = new NpgsqlCommand("INSERT INTO public.cards(name, damage, c_id) " +
                                                                           "VALUES(@name, @dmg, @id)", Conn);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@dmg", dmg);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                var worked = command.ExecuteNonQuery();
                Console.WriteLine(worked == 1 ? "Card has been added" : "Problem occurred adding card!");
                return new Card(id, name, dmg, null, null);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string UserStats(string username)
        {
            using var command = new NpgsqlCommand("Select elo from users where username = @user;", Conn);
            command.Parameters.AddWithValue("@user", username);

            try
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return $"{username} has {reader.GetInt32(0)} points";
                }
                reader.Close();
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
                //throw DuplicateNameException();
            }
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