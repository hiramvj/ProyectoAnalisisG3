#r "nuget: Npgsql, 8.0.2"
using System.Data;
using Npgsql;

var connString = "Host=aws-0-us-west-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.uohqtzqgddvdfxevvgho;Password=Password123.;Pooling=false;Include Error Detail=true;";
using var conn = new NpgsqlConnection(connString);
conn.Open();

using var cmd = conn.CreateCommand();
cmd.CommandText = "ALTER TABLE \"Producto\" ADD COLUMN IF NOT EXISTS \"Stock\" NUMERIC(18,2) NOT NULL DEFAULT 0;";
cmd.ExecuteNonQuery();
Console.WriteLine("Stock column added successfully!");
