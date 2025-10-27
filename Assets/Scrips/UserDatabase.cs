using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class UserRecord
{
    public string username;
    public string passwordHash;
}

[Serializable]
class UserDatabaseData
{
    public List<UserRecord> users = new List<UserRecord>();
}

public static class UserDatabase
{
    static UserDatabaseData data = new UserDatabaseData();
    static string SavePath => Path.Combine(Application.persistentDataPath, "users.json");
    public const int MaxUsers = 10;

    public static void Load()
    {
        if (File.Exists(SavePath))
        {
            var json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<UserDatabaseData>(json) ?? new UserDatabaseData();
        }
        else
        {
            data = new UserDatabaseData();
            Save(); // crea archivo vacío
        }
    }

    public static void Save()
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static IReadOnlyList<UserRecord> Users => data.users;

    public static string[] GetUsernames()
    {
        var list = new List<string>();
        foreach (var u in data.users) list.Add(u.username);
        return list.ToArray();
    }

    public static bool Exists(string username)
    {
        return data.users.Exists(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));
    }

    public static bool TryCreate(string username, string password, out string error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(username)) { error = "Escribe un usuario."; return false; }
        if (string.IsNullOrWhiteSpace(password)) { error = "Escribe una contraseña."; return false; }
        if (username.Length < 3) { error = "El usuario debe tener al menos 3 caracteres."; return false; }
        if (password.Length < 4) { error = "La contraseña debe tener al menos 4 caracteres."; return false; }

        if (data.users.Count >= MaxUsers) { error = "Límite de 10 usuarios."; return false; }
        if (Exists(username)) { error = "Ese usuario ya existe."; return false; }

        var rec = new UserRecord
        {
            username = username,
            passwordHash = Hash(password)
        };
        data.users.Add(rec);
        Save();
        return true;
    }

    public static bool TryValidate(string username, string password, out string error)
    {
        error = null;
        var rec = data.users.Find(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));
        if (rec == null) { error = "Usuario no encontrado."; return false; }

        var ok = Verify(password, rec.passwordHash);
        if (!ok) { error = "Contraseña incorrecta."; return false; }
        return true;
    }

    static string Hash(string input)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }

    static bool Verify(string input, string hash) => Hash(input) == hash;
}

