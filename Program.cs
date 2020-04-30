using System;

using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace SQLProject
{
    static class Program
    {
        class GrabParams : IFormattable
        {
            SqlParameter Pr;
            public GrabParams(SqlParameter P) { this.Pr = P; }
            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (!string.IsNullOrEmpty(format))
                {
                    Pr.SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), format, true);
                }
                return Pr.ParameterName;
            }
        }
        static SqlCommand NewSQLCommand(this SqlConnection connection, FormattableString Query)
        {
            SqlParameter[] Pr = Query.GetArguments().Select((Arg, Pos) => new SqlParameter($"@p{Pos}", Arg)).ToArray();
            object[] formatArguments = Pr.Select(x => new GrabParams(x)).ToArray();
            string command = string.Format(Query.Format, formatArguments);
            SqlCommand cmd = new SqlCommand(command, connection);
            cmd.Parameters.AddRange(Pr);
            return cmd;
        }

        static void Main(string[] args)
        {
            SqlConnection cn = new SqlConnection();
            int age = 18;
            string Name = "Yassine", table = "TablePersonne", CIN = "D883756";
            
            SqlCommand Cmd = cn.NewSQLCommand($"select * from {table} where CIN={CIN:varchar} age > {age:int} And name like %{Name}%");

            Console.WriteLine("The old Query:\tselect * from {table} where CIN={CIN} age > {age:int} And name like %{Name:nvarchar}%");
            Console.WriteLine($"The new Query:\t\t{Cmd.CommandText}");
            Console.WriteLine("Parameters:");
            foreach (SqlParameter P in Cmd.Parameters)
            {
                Console.WriteLine($"Name={P.ParameterName}\t\tValue:{P.Value}\t\tType:{P.SqlDbType}");
            }

        }
    }


}
